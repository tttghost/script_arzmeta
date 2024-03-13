using UnityEngine;
using FrameWork.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using FrameWork.Network;
using Google.Protobuf;
using Protocol;
using Gpm.Ui;
using Gpm.Ui.Sample;

/// <summary>
/// 실질적 채팅 기능
/// </summary>
public class View_Chat : UIBase
{
    #region Fields
    private CHAT_TYPE _toggleType;

    private List<Item_S_ChatSystem> systemChat = new List<Item_S_ChatSystem>();
    private Dictionary<string, GameObject> go_Players = new Dictionary<string, GameObject>();

    private string prevText = string.Empty;
    private bool isWhisperTarget = false;
    private bool isColon = false;
    private bool isTogOn = true;

    private bool _isNewWhisper = false;
    private bool _isNewSystem = false;

    private NetworkHandler networkHandler;
    public ChatModeController chatModeController { get; private set; }

    private GameObject item_Chat;
    private GameObject go_UserList;

    private TMP_InputField input_Chat;

    private InfiniteScroll ScrollView_Preview;
    private InfiniteScroll ScrollView_Chat;
    private InfiniteScroll ScrollView_UserList;

    private Toggle tog_ChatType;
    private TMP_Text txtmp_ChatType;

    private Button btn_Report;
    private Button btn_Send;

    private Image img_newIcon_On;
    private Image img_newIcon_Off;
    #endregion


    #region 1:1 채팅과 시스템 채팅에서 New 아이콘 설정
    /// <summary>
    /// 1:1 채팅 new 아이콘을 띄우기 위한 bool 값
    /// </summary>
    public bool IsNewWhisper
    {
        get => _isNewWhisper;
        set
        {
            _isNewWhisper = value;

            OnNewMEssageStatusChanged();
        }
    }

    /// <summary>
    /// 시스템 채팅 new 아이콘을 띄우기 위한 bool 값
    /// </summary>
    public bool IsNewSystem
    {
        get => _isNewSystem;
        set
        {
            _isNewSystem = value;

            OnNewMEssageStatusChanged();
        }
    }

    /// <summary>
    /// IsNewWhisper와 IsNewSystem 둘 중 하나라도 true이면 new 아이콘 활성화
    /// </summary>
    private void OnNewMEssageStatusChanged()
    {
        bool b = IsNewWhisper || IsNewSystem;

        isTogOn = chatModeController.togplus_Chat.GetToggleIsOn();
        if (isTogOn)
            img_newIcon_On.gameObject.SetActive(b);
        else
            img_newIcon_Off.gameObject.SetActive(b);
    }
    #endregion


    #region 채팅 타입에 따른 UI 설정
    /// <summary>
    /// 순환 토글 변환 변수
    /// </summary>
    public CHAT_TYPE ToggleType
    {
        get => _toggleType;
        set
        {
            _toggleType = value;

            ChangeToggleType();
        }
    }
    public void ChangeToggleType()
    {
        // 토글 타입에 따라 글자, new 아이콘 비활성화
        MasterLocalData result = null;
        switch (ToggleType)
        {
            case CHAT_TYPE.ALL:
                result = new MasterLocalData("common_all");
                break;
            case CHAT_TYPE.WHISPER:
                result = new MasterLocalData("chat_type_1on1");
                if (IsNewWhisper) IsNewWhisper = false;
                break;
            case CHAT_TYPE.SYSTEM:
                result = new MasterLocalData("setting_system");
                if (IsNewSystem) IsNewSystem = false;
                break;
        }

        Util.SetMasterLocalizing(txtmp_ChatType, result);

        // 각 토글에 해당되는 채팅 데이터가 있으면 불러오기
        RefreshScrollView();

        // 채팅모드일 때, 토글타입에 따른 UI 변경
        if (chatModeController.ChatMode == CHAT_MODE.CHATTING)
            ChangeUI();
        else if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
            SceneLogic.instance.isEscapeLock = false; 

#if UNITY_EDITOR || UNITY_STANDALONE
        // inputField에 Focusing
        input_Chat.ActivateInputField();
#endif
    }

    /// <summary>
    /// 토글 전환 시, 스크롤뷰 초기화
    /// 각 토글에 해당되는 채팅 데이터가 있으면 불러오기
    /// </summary>
    public void RefreshScrollView()
    {
        ScrollView_Chat.ClearData();

        // 토글 타입에 맞게 데이터 변환
        List<InfiniteScrollData> chatData = ChangeDataFormat();

        // 이전 채팅 기록 있는 경우, 스크롤뷰 리프레쉬 및 하단 고정
        MoveToLastData(ScrollView_Chat, chatData);

        // 기본 모드에서 최신 채팅 데이터 한줄만 노출
        if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
            ShowLastChat(chatData);
    }

    /// <summary>
    /// 최상위 부모인 InfiniteScrollData 형식에 맞게 변환
    /// </summary>
    /// <returns></returns>
    private List<InfiniteScrollData> ChangeDataFormat()
    {
        List<InfiniteScrollData> parents = new List<InfiniteScrollData>();
        switch (ToggleType)
        {
            case CHAT_TYPE.ALL:
                parents.AddRange(LocalPlayerData.Method.allChat);
                break;
            case CHAT_TYPE.WHISPER:
                parents.AddRange(LocalPlayerData.Method.dmChat);
                break;
            case CHAT_TYPE.SYSTEM:
                parents.AddRange(systemChat);
                break;
        }
        return parents;
    }

    /// <summary>
    /// 이전 채팅 기록(List)이 있는 경우, 스크롤뷰 리로드 및 하단 고정
    /// 최근 1:1 채팅 유저 리스트 보여주기
    /// </summary>
    /// <param name="scrollView"></param>
    /// <param name="itemData"></param>
    private void MoveToLastData(InfiniteScroll scrollView, List<InfiniteScrollData> itemData)
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            scrollView.InsertData(itemData[i]);
            scrollView.MoveToLastData();
        }

        if (!scrollView.IsMoveToLastData()) scrollView.MoveToLastData();
    }
    /// <summary>
    /// 새로운 채팅이 있는 경우, 스크롤뷰 리로드 및 하단 고정
    /// </summary>
    /// <param name="scrollView"></param>
    /// <param name="itemData"></param>
    private void MoveToLastData(InfiniteScroll scrollView, InfiniteScrollData itemData)
    {
        scrollView.InsertData(itemData);
        scrollView.MoveToLastData();

        if (!scrollView.IsMoveToLastData()) scrollView.MoveToLastData();
    }

    /// <summary>
    /// 기본 모드에서 최신 채팅 데이터 한줄만 보여주기
    /// </summary>
    /// <param name="chatData"></param>
    private void ShowLastChat(List<InfiniteScrollData> chatData)
    {
        ScrollView_Preview.ClearData();

        // 1:1 채팅 안내 문구 미노출
        if ((ToggleType == CHAT_TYPE.ALL || ToggleType == CHAT_TYPE.WHISPER)
            && (LocalPlayerData.Method.IsFirst && chatData.Count == 1))
            return;
        if (chatData.Count == 0) return;

        ScrollView_Preview.InsertData(chatData[chatData.Count - 1]);
    }

    public void ChangeUI(bool active = true)
    {
        chatModeController.go_Bottom.SetActive(!active);
        ScrollView_Chat.gameObject.SetActive(!active);

        switch (ToggleType)
        {
            case CHAT_TYPE.ALL:
            case CHAT_TYPE.WHISPER:
                ScrollView_Chat.gameObject.SetActive(active);
                chatModeController.go_Bottom.SetActive(active);
                break;
            case CHAT_TYPE.SYSTEM:
                ScrollView_Chat.gameObject.SetActive(active);
                break;
        }
    }
    #endregion


    #region Init & Basic Method

    protected override void Awake()
    {
        base.Awake();

        AddHandler();

        // @상대닉네임:에서 :지워지면 인풋필드 비우기
        input_Chat.onValueChanged.AddListener(OnInputFieldValueChanged);

        input_Chat.onSubmit.AddListener(OnInputReturn);
    }

    private void OnInputReturn(string inputText)
    {
        if (chatModeController.ChatMode == CHAT_MODE.CHATTING)
        {
            if (GetChatInputField().isFocused)
                SetSendMessage(inputText);
            else
                return;
        }
    }

    /// <summary>
    /// @상대닉네임 뒤에 :자동생성
    /// @상대닉네임:에서 :지워지면 인풋필드 비우기
    /// </summary>
    /// <param name="_text"></param>
    private void OnInputFieldValueChanged(string _text)
    {
        // @ 입력 시, 최근 1:1 채팅 상대 리스트 보여주기
        if (_text.Equals("@"))
            ShowUserList();
        else go_UserList.SetActive(false);


        // 1:1 채팅 조건 만족 여부
        if (_text.StartsWith("@") && _text.EndsWith(" "))
            isWhisperTarget = true;
        else isWhisperTarget = false;


        // @상대닉네임: 형태에서 :이 지워졌을 때만 인풋필드 비우기
        if (!_text.Contains(":")) isColon = false;
        else isColon = true;

        if (prevText.StartsWith("@") && prevText.EndsWith(":") && prevText.Trim().Length >= 3 && !isColon)
        {
            input_Chat.text = string.Empty;
            isColon = false;
        }
        prevText = _text;
    }

    /// <summary>
    /// @ 입력 시, 1:1 채팅 유저 리스트 보여주기
    /// </summary>
    private void ShowUserList()
    {
        go_UserList.SetActive(true);

        ScrollView_UserList.ClearData();

        // 스크롤뷰에 InsertData 하기 위해 부모 형식에 맞게 변환
        List<InfiniteScrollData> parents = new List<InfiniteScrollData>();
        parents.AddRange(LocalPlayerData.Method.userList);

        // 스크롤뷰에 데이터 추가 및 스크롤뷰 하단 고정
        MoveToLastData(ScrollView_UserList, parents);
    }

    /// <summary>
    /// 핸들러 등록
    /// </summary>
    private void AddHandler()
    {
        // 채팅 핸들러 등록
        Single.Socket.item_S_ChatMessage_Handler += S_ChatMessage;
        Single.Socket.item_S_ChatDM_Handler += S_ChatDM;
        Single.Socket.item_S_ChatSystem_Handler += S_ChatSystem;
        // 닉네임 변경 콜백 핸들러 등록

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this, S_SET_NICKNAME_NOTICE);
    }

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        networkHandler = FindObjectOfType<NetworkHandler>();
        chatModeController = GetComponent<ChatModeController>();

        input_Chat = GetUI_TMPInputField(nameof(input_Chat));

        tog_ChatType = GetUI_Toggle(nameof(tog_ChatType), ToggleAction);
        txtmp_ChatType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChatType));

        InitInfiniteScroll();

        go_UserList = GetChildGObject(nameof(go_UserList));

        btn_Report = GetUI_Button(nameof(btn_Report), () => PushPopup<Popup_ChatReport>());
        btn_Send = GetUI_Button(nameof(btn_Send), ChatSend);

        img_newIcon_On = GetUI_Img(nameof(img_newIcon_On));
        img_newIcon_Off = GetUI_Img(nameof(img_newIcon_Off));
    }

    /// <summary>
    /// 순환 토글
    /// </summary>
    private void ToggleAction()
    {
        switch (ToggleType)
        {
            case CHAT_TYPE.ALL:
                ToggleType = CHAT_TYPE.WHISPER;
                break;
            case CHAT_TYPE.WHISPER:
                ToggleType = CHAT_TYPE.SYSTEM;
                break;
            case CHAT_TYPE.SYSTEM:
                ToggleType = CHAT_TYPE.ALL;
                break;
        }

        tog_ChatType.isOn = false;

        InputUpdate(input_Chat);
    }

    /// <summary>
    /// 인피니트 스크롤 초기화
    /// </summary>
    private void InitInfiniteScroll()
    {
        ScrollView_Preview = GetUI<ScrollRect>(nameof(ScrollView_Preview)).GetComponent<InfiniteScroll>();
        ScrollView_Chat = GetUI<ScrollRect>(nameof(ScrollView_Chat)).GetComponent<InfiniteScroll>();
        ScrollView_UserList = GetUI<ScrollRect>(nameof(ScrollView_UserList)).GetComponent<InfiniteScroll>();

        item_Chat = Single.Resources.Load<GameObject>(Cons.Path_Prefab_Item + nameof(item_Chat));
        ScrollView_Preview.itemPrefab = item_Chat.GetComponent<InfiniteScrollItem>();
        ScrollView_Chat.itemPrefab = item_Chat.GetComponent<InfiniteScrollItem>();
        ScrollView_UserList.itemPrefab = item_Chat.GetComponent<InfiniteScrollItem>();
    }

    protected override void Start()
    {
        base.Start();

        // 채팅 기본 모드, 전체 채팅 토글로 생성자 초기화
        chatModeController.ChatMode = CHAT_MODE.STANDARD;
        ToggleType = CHAT_TYPE.ALL;

        ocean = FindObjectOfType<OceanSwimTrigger>();
    }
    OceanSwimTrigger ocean;

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 채팅할때 플레이어 이동 BLOCK
        if (MyPlayer.instance != null && MyPlayer.instance.TPSController.StarterInputs != null && SceneLogic.instance.isChatEnabled)
        {
            if (GetChatInputField().isFocused)
            {
                MyPlayer.instance.TPSController.StarterInputs.blockSprint = true;
                MyPlayer.instance.TPSController.StarterInputs.blockJump = true;
				MyPlayer.instance.TPSController.StarterInputs.blockMove = true;
			}
			else
			{
                if (ocean?.isSwiming ?? false) return;

                MyPlayer.instance.TPSController.StarterInputs.blockSprint = false;
				MyPlayer.instance.TPSController.StarterInputs.blockJump = false;
				MyPlayer.instance.TPSController.StarterInputs.blockMove = false;
			}
		}
#endif
        // @상대닉네임 뒤에 :자동생성
        if (isWhisperTarget)
            WhisperChatException(input_Chat.text);
    }

    /// <summary>
    /// @로 시작해서 스페이스바가 눌리면 :추가
    /// @상대닉네임 -> @상대닉네임: 
    /// </summary>
    /// <param name="_message"></param>
    private void WhisperChatException(string _message)
    {
        if (isWhisperTarget && _message.Trim().Length >= 2 && !isColon)
            input_Chat.text = _message.Trim() + ":";
    }

    private void OnDestroy()
    {
        if (!Single.Socket) return;

        Single.Socket.C_ExitChatRoom();

        // 앱 최초 실행 시, 1:1 채팅 안내 문구를 최상단에 한번만 보여주기 위해 
        if (LocalPlayerData.Method.IsFirst && LocalPlayerData.Method.dmChat.Count > 0)
        {
            LocalPlayerData.Method.IsFirst = false;
            LocalPlayerData.Method.dmChat.RemoveAt(0);
        }

        LocalPlayerData.Method.allChat.Clear();

        RemoveHandler();
    }

    /// <summary>
    /// 핸들러 해제
    /// </summary>
    private void RemoveHandler()
    {
        // 채팅 핸들러 해제
        Single.Socket.item_S_ChatMessage_Handler -= S_ChatMessage;
        Single.Socket.item_S_ChatDM_Handler -= S_ChatDM;
        Single.Socket.item_S_ChatSystem_Handler -= S_ChatSystem;

        // 닉네임 변경 콜백 핸들러 해제
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this);
    }
    #endregion


    #region 실시간 닉네임 업데이트
    private void S_SET_NICKNAME_NOTICE(PacketSession arg1, IMessage arg2)
    {
        S_SET_NICKNAME_NOTICE packet = arg2 as S_SET_NICKNAME_NOTICE;

        // 웹소켓에 닉네임 변경되었다고 알려주기
        Single.Socket.C_ChangeNickname();

        string prevNickName = networkHandler.Clients[packet.ClientId].nickname;

        // 바뀐 닉네임으로 월드 채팅 변경
        foreach (Item_ChatDefault chatData in LocalPlayerData.Method.allChat)
        {
            if (chatData is Item_S_ChatMessage item_S_ChatMessage)
            {
                if (prevNickName == item_S_ChatMessage.sendNickName)
                    item_S_ChatMessage.sendNickName = packet.Nickname;
            }

            if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
                ScrollView_Preview.UpdateData(chatData);
            else
                ScrollView_Chat.UpdateData(chatData);
        }

        // 바뀐 닉네임으로 1:1 채팅 변경
        foreach (Item_ChatDefault chatData in LocalPlayerData.Method.dmChat)
        {
            if (chatData is Item_S_ChatDM item_S_ChatDM)
            {
                // 받은 채팅
                if (prevNickName == item_S_ChatDM.sendNickName)
                    item_S_ChatDM.sendNickName = packet.Nickname;

                // 내가 보낸 채팅 ( [@상대닉네임]: )
                if (prevNickName == item_S_ChatDM.recvNickName)
                    item_S_ChatDM.recvNickName = packet.Nickname;
            }

            if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
                ScrollView_Preview.UpdateData(chatData);
            else
                ScrollView_Chat.UpdateData(chatData);
        }
    }
    #endregion


    #region 채팅 보내기
    /// <summary>
    /// PC의 경우, 메세지 전송
    /// </summary>
    private void ChatSend()
    {
        string message = input_Chat.text;

        SetSendMessage(message);
    }

    /// <summary>
    /// 모바일의 경우, 메세지 전송
    /// </summary>
    /// <param name="message"></param>
    private void SendMoblieChat(string message)
    {
        if (Application.isEditor || string.IsNullOrEmpty(message) || message.Trim().Length == 0) return;

        SetSendMessage(message);
    }

    /// <summary>
    /// 웹서버로 메세지 전송
    /// </summary>
    /// <param name="_message"></param>
    private void SetSendMessage(string _sendMessage)
    {
        if (!string.IsNullOrEmpty(_sendMessage) || _sendMessage.Trim().Length != 0)
        {
            // 금칙어 * 처리
            var _message = Util.ChangeForbiddenWordsToStar(_sendMessage);

            // 닉네임으로 1:1 채팅 보내기
            if (_message.Split(':').Length >= 2)
            {
                // 닉네임 : 메세지로 파싱 (메세지에 :이 들어갈 경우 예외처리)
                string[] parsed = _message.Split(new char[] { ':' }, 2);
                string target = parsed[0].Substring(1);
                string message = parsed[1].Trim();

                if (!string.IsNullOrEmpty(message))
                {
                    C_ChatDM(target, message);
                }
            }
            // 월드 채팅 보내기
            else
            {
                C_ChatMessage(_message);
            }
        }

        InputUpdate(input_Chat);
    }

    /// <summary>
    /// InputField 비우고 focusing
    /// </summary>
    /// <param name="input"></param>
    private void InputUpdate(TMP_InputField input)
    {
        if (input.text != string.Empty)
        {
            input.text = string.Empty;
            isColon = false;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // inputField에 Focusing
        input.ActivateInputField();
#endif
    }
    #endregion


    #region 월드 채팅
    /// <summary>
    /// 월드 채팅 메세지 보내기
    /// </summary>
    /// <param name="_message"></param>
    private void C_ChatMessage(string _message)
    {
        // roomCode는 오피스룸, 상담 등과 같은 roomCode가 있는 경우 해당 값이 저장되고 없으면 String.Empty가 들어감
        Item_C_ChatMessage chatData = new Item_C_ChatMessage
        {
            message = _message,
            color = Cons.ChatColor_White,
            roomCode = LocalContentsData.roomCode,
            roomName = LocalContentsData.roomName,
        };

        Single.Socket.C_ChatMessage(chatData);
    }

    /// <summary>
    /// S_ChatMessage 패킷 받았을 때,
    /// DynamicScroll을 이용한 월드 채팅 데이터 추가 및 업데이트
    /// </summary>
    /// <param name="_message"></param>
    private void S_ChatMessage(Item_S_ChatMessage item_S_ChatMessage)
    {
        // 플레이어 머리 위 말풍선
        SpeechBubble(item_S_ChatMessage);

        if (ToggleType == CHAT_TYPE.ALL)
        {
            if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
            {
                MoveToLastData(ScrollView_Preview, item_S_ChatMessage);
            }
            else
            {
                MoveToLastData(ScrollView_Chat, item_S_ChatMessage);
            }
        }

        LocalPlayerData.Method.allChat.Add(item_S_ChatMessage);
    }

    /// <summary>
    /// 플레이어 headDisplay에 말풍선 띄우기
    /// </summary>
    private void SpeechBubble(Item_S_ChatMessage item)
    {
        GameObject player = GetPlayerHolder(item.sendNickName);
        if (player == null) return;
        HUDParent HUDParent = Util.Search<HUDParent>(player, nameof(HUDParent));

        HUDParent.SpeechBubble(item.message);
    }

    /// <summary>
    /// 현재 씬의 Player들 딕셔너리에 추가 / 찾기
    /// </summary>
    /// <param name="playerNickName"></param>
    /// <returns></returns>
    private GameObject GetPlayerHolder(string playerNickName)
    {
        // 딕셔너리에 해당 유저가 있는 경우,
        if (go_Players.TryGetValue(playerNickName, out GameObject player))
        {
            return player;
        }
        // 없는 경우, 추가
        else
        {
            Transform playersTrans = GameObject.Find("Players").transform;
            Transform playerTrans = playersTrans.Find(playerNickName);

            if (playerTrans != null)
            {
                go_Players[playerNickName] = playerTrans.gameObject;

                return playerTrans.gameObject;
            }
            else return null;
        }
    }
    #endregion

    #region 1:1
    /// <summary>
    /// 1:1 채팅 메세지 보내기
    /// </summary>
    /// <param name="_message"></param>
    private void C_ChatDM(string _target, string _message)
    {
        Item_C_ChatDM Item_C_ChatDM = new Item_C_ChatDM
        {
            recvNickName = _target,
            message = _message,
            color = Cons.ChatColor_Yellow,
        };

        Single.Socket.C_ChatDM(Item_C_ChatDM);
    }

    /// <summary>
    /// S_ChatDirectMessage 패킷 받았을 때,
    /// DynamicScroll을 이용한 1:1 채팅 데이터 추가 및 업데이트
    /// </summary>
    /// <param name="_message"></param>
    private void S_ChatDM(Item_S_ChatDM item_S_ChatDM)
    {
        if (chatModeController.ChatMode == CHAT_MODE.HIDDEN || ToggleType == CHAT_TYPE.SYSTEM)
            IsNewWhisper = true;

        if (!(ToggleType == CHAT_TYPE.SYSTEM))
        {
            if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
            {
                MoveToLastData(ScrollView_Preview, item_S_ChatDM);
            }
            else
            {
                MoveToLastData(ScrollView_Chat, item_S_ChatDM);
            }
        }

        // 최근 1:1 채팅 유저 리스트 생성
        CreateUserList(item_S_ChatDM);
    }

    /// <summary>
    /// 최근 1:1 채팅 유저 리스트 생성
    /// </summary>
    /// <param name="item_S_ChatDM"></param>
    private void CreateUserList(Item_S_ChatDM item_S_ChatDM)
    {
        // 1:1 채팅 유저 닉네임 저장
        string nickName = string.Empty;
        if (string.IsNullOrEmpty(item_S_ChatDM.sendNickName))
            nickName = item_S_ChatDM.recvNickName;
        else if (!string.IsNullOrEmpty(item_S_ChatDM.sendNickName) && !string.IsNullOrEmpty(item_S_ChatDM.recvNickName))
            nickName = item_S_ChatDM.sendNickName;

        // 유저 리스트에 닉네임 추가 (중복 X)
        if (LocalPlayerData.Method.userList.Count == 0)
        {
            Item_UserList userNickName = new Item_UserList
            {
                NickName = nickName,
                color = Cons.ChatColor_White,
            };

            LocalPlayerData.Method.userList.Add(userNickName);
        }
        else
        {
            foreach (var user in LocalPlayerData.Method.userList)
            {
                if (user.NickName == nickName) return;
                else
                {
                    Item_UserList userNickName = new Item_UserList
                    {
                        NickName = nickName,
                        color = Cons.ChatColor_White,
                    };

                    LocalPlayerData.Method.userList.Add(userNickName);
                }
            }
        }
    }
    #endregion

    #region 시스템 채팅 (1:1 시스템 메세지 포함)
    /// <summary>
    /// S_SystemMessage 패킷 받았을 때,
    /// DynamicScroll을 이용한 시스템 채팅 데이터 추가 및 업데이트
    /// </summary>
    /// <param name="_message"></param>
    private void S_ChatSystem(Item_S_ChatSystem item_S_ChatSystem)
    {
        string _message = item_S_ChatSystem.message;
        string sysMsg = string.Empty;

        // 해당 토글에서 메세지를 보고 있지 않을 때, true
        if (chatModeController.ChatMode == CHAT_MODE.HIDDEN || ToggleType == CHAT_TYPE.WHISPER)
            IsNewSystem = true;

        switch (_message)
        {
            // 1:1 시스템 메세지일 경우,
            case "2100":
            case "2101":
            case "2102":
                {
                    WhisperSysMsg(_message);
                }
                break;
            // 일반 시스템 메세지일 경우,
            default:
                {
                    // TODO : 차후 시스템 메세지 관련 기획추가되면 시스템 메세지에 따라 메세지 세팅
                    sysMsg = SetSysMsg(_message);

                    Item_S_ChatSystem systemData = new Item_S_ChatSystem
                    {
                        message = sysMsg,
                        color = item_S_ChatSystem.color,
                    };

                    LocalPlayerData.Method.allChat.Add(systemData);
                    systemChat.Add(systemData);

                    if (!(ToggleType == CHAT_TYPE.WHISPER))
                    {
                        if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
                        {
                            MoveToLastData(ScrollView_Preview, systemData);
                        }
                        else
                        {
                            MoveToLastData(ScrollView_Chat, systemData);
                        }
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 1:1 시스템 메세지 세팅 및 스크롤뷰에 추가
    /// </summary>
    /// <param name="_message"></param>
    private void WhisperSysMsg(string _message)
    {
        string sysMsg = string.Empty;

        switch (_message)
        {
            case "2100":
                sysMsg = "chat_notice_nonexist";
                break;
            case "2101":
                sysMsg = "chat_notice_notconneted";
                break;
            case "2102":
                sysMsg = "chat_notice_selfchat";
                break;
        }

        Item_S_ChatDM dmSysData = new Item_S_ChatDM
        {
            sendNickName = "",
            recvNickName = "",
            message = sysMsg,
            color = Cons.ChatColor_Green,
        };

        if (chatModeController.ChatMode == CHAT_MODE.STANDARD)
        {
            MoveToLastData(ScrollView_Preview, dmSysData);
        }
        else
        {
            MoveToLastData(ScrollView_Chat, dmSysData);
        }
    }

    // TODO : 차후 시스템 메세지 관련 기획추가되면 시스템 메세지에 따라 메세지 세팅
    /// <summary>
    /// 일반 시스템 메세지 세팅 및 스크롤뷰에 추가
    /// </summary>
    /// <param name="_message"></param>
    private string SetSysMsg(string _message)
    {
        string sysMsg = string.Empty;

        //switch (_message)
        //{

        //}

        //// 임시 코드
        //sysMsg = "시스템채팅 메세지 테스트 중입니다.";

        return sysMsg;
    }
    #endregion


    public TMP_InputField GetChatInputField()
    {
        return input_Chat;
    }
}
