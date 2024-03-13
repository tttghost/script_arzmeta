//using FrameWork.Network;
//using FrameWork.UI;
//using Google.Protobuf;
//using Google.Protobuf.Collections;
//using MEC;
//using Protocol;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class View_OfficeEnterPlayers : UIBase
//{
//    #region 변수

//    [HideInInspector] public Scene_OfficeRoom sceneOfficeRoom;
//    [SerializeField] private Item_OfficeUserInfoEnter enterItem;

//    private ScrollRect scroll;

//    //public List<RoomUserData> receiveUserDatas = new List<RoomUserData>(); // 호스트 제외

//    private Toggle tog_Host_Screen;
//    private Toggle tog_MasterChat;
//    private Toggle tog_MasterVoice;
//    private Toggle tog_MasterVideo;   

//    private TMP_Text txtmp_Position;
//    private TMP_Text txtmp_Display; 
//    private TMP_Text txtmp_Chat;
//    private TMP_Text txtmp_Video; 
//    private TMP_Text txtmp_Voice; 
//    private TMP_Text txtmp_KickOut;
//    private TMP_Text txtmp_ChangeSendData;

//    private TMP_Text txtmp_Tog_Screen;
//    private TMP_Text txtmp_Tog_HostTransfer;
//    private TMP_Text txtmp_Tog_Chat;
//    private TMP_Text txtmp_Tog_Video;
//    private TMP_Text txtmp_Tog_KickOut;

//    private TMP_Text txtmp_hostPlayerName; 
    
//    // 중복으로 클릭되어야하는 것들은 리스트에 저장
//    [SerializeField] public List<Toggle> chatToggles = new List<Toggle>();
//    [SerializeField] public List<Toggle> videoToggles = new List<Toggle>();
//    [SerializeField] public List<Toggle> voiceToggles = new List<Toggle>();

//    [HideInInspector] public ToggleGroup togg_Screen;

//    private Button btn_ChangeSendData;

//    private eOfficeAuthority changeHostType = eOfficeAuthority.관리자;

//    public Action toggle_Callback;
//    public Action kick_Callback;
//    public Action dataUpdate_Callback;

//    public Button SaveButton { get => btn_ChangeSendData; }

//    #endregion



//    #region 초기화

//    protected override void SetMemberUI()
//    {
//        base.SetMemberUI();

//        sceneOfficeRoom = FindObjectOfType<Scene_OfficeRoom>();

//        scroll = Util.Search<ScrollRect>(gameObject, "sview_Content");
//        scroll.content = Util.Search<RectTransform>(scroll.gameObject, "Content");

//        toggle_Callback += () =>
//        {
//            btn_ChangeSendData.interactable = true;
//        };

//        kick_Callback += RefreshItems;

//        btn_ChangeSendData = GetUI_Button("btn_ChangeSendData", OnClick_SendPermission);

//        Util.RunCoroutine(Initialization());
//    }

//    /// <summary>
//    /// 인원 정보 팝업 초기화 (게스트/호스트 마다 다르게 활성화)
//    /// </summary>
//    /// <returns></returns>
//    private IEnumerator<float> Initialization()
//    {
//        yield return Timing.WaitUntilTrue(() => sceneOfficeRoom.curOfficeInfo != null);
//        yield return Timing.WaitUntilTrue(() => MyPlayer.instance);

//        togg_Screen = GetUI<ToggleGroup>(nameof(togg_Screen)); // 화면권한 토글 그룹

//        tog_Host_Screen   = GetUI_Toggle(nameof(tog_Host_Screen));
//        tog_MasterChat    = GetUI_Toggle(nameof(tog_MasterChat));
//        tog_MasterVideo   = GetUI_Toggle(nameof(tog_MasterVideo));
//        tog_MasterVoice   = GetUI_Toggle(nameof(tog_MasterVoice));

//        txtmp_Position       = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Position), new MasterLocalData("office_participant_type"));     // 참여자 구분
//        txtmp_Display        = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Display), new MasterLocalData("office_permission_screen"));     // 화면 권한 
//        txtmp_Chat           = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Chat), new MasterLocalData("office_permission_chat"));          // 채팅 권한 
//        txtmp_Voice          = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Voice), new MasterLocalData("office_permission_voicechat"));    // 음성 권한 
//        txtmp_Video          = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Video), new MasterLocalData("office_permission_videochat"));    // 영상 권한 
//        txtmp_KickOut        = GetUI_TxtmpMasterLocalizing(nameof(txtmp_KickOut), new MasterLocalData("office_kick"));                  // 강퇴 기능
//        txtmp_ChangeSendData = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChangeSendData), new MasterLocalData("1087"));                  // 정보 반영

//        AddListener();
//    }

//    protected override void Awake()
//    {
//        base.Awake();
//        //AddHandler();
//    }

//    protected void OnEnable()
//    {
//        // base.Start();

//        btn_ChangeSendData.interactable = false;

//        txtmp_hostPlayerName = GetUI_Txtmp("txtmp_hostPlayerName", new LocalData(Cons.Local_Arzmeta, "1095", new LocalData(sceneOfficeRoom.curOfficeInfo.hostName)));

//        //if(init) sceneOfficeRoom.officeModuleHandler.SendGetPermission(); // 권한 정보 요청

//        RefreshItems();
//    }
    
//    private bool init;

//    protected override void Start()
//    {
//        base.Start();
        
//        //sceneOfficeRoom.officeModuleHandler.SendGetPermission(); // 권한 정보 요청
        
//        //if (!init)
//        //{
//        //    sceneOfficeRoom.officeModuleHandler.SendGetPermission(); // 권한 정보 요청
//        //    init = true;
//        //} 
//    }

//    private void OnDestroy()
//    {
//        if (!RealtimeManager.Instance) return;

//        //RemoveHandler();
//    }

//    /// <summary>
//    /// 데이터 초기화
//    /// </summary>
//    private void InitUserData()
//    {
//        //var clientDataList = sceneOfficeRoom.officeModuleHandler.ClientDataList;
        
//        //receiveUserDatas.Clear();

//        //foreach (var data in clientDataList)
//        //{
//        //    if (data.clientId == sceneOfficeRoom.curOfficeInfo.hostId) continue;

//        //    var roomUserData = new RoomUserData
//        //    {
//        //        ClientData = data,
//        //        permissions = new Userpermission()
//        //    };
            
//        //    receiveUserDatas.Add(roomUserData);
//        //}

//        //Debug.Log("InitUserData: " + receiveUserDatas.Count);
//    }

//    /// <summary>
//    /// 유저 데이터 가져오기
//    /// </summary>
//    /// <param name="clientId"></param>
//    /// <returns></returns>
//    //private RoomUserData GetUserData( string clientId )
//    //{
//    //    return receiveUserDatas.FirstOrDefault(userData => userData.ClientData.clientId == clientId);
//    //}

//    /// <summary>
//    /// 마스터 토글 버튼 이벤트 등록
//    /// </summary>
//    private void AddListener()
//    {
//        tog_MasterChat.onValueChanged.AddListener(( check ) =>
//        {
//            for (int i = 0; i < chatToggles.Count; i++)
//            {
//                chatToggles[i].isOn = tog_MasterChat.isOn;
//            }
//        });

//        tog_MasterVideo.onValueChanged.AddListener(( check ) =>
//        {
//            for (int i = 0; i < videoToggles.Count; i++)
//            {
//                videoToggles[i].isOn = tog_MasterVideo.isOn;
//            }
//        });

//        tog_MasterVoice.onValueChanged.AddListener(( check ) =>
//        {
//            for (int i = 0; i < voiceToggles.Count; i++)
//            {
//                voiceToggles[i].isOn = tog_MasterVoice.isOn;
//            }
//        });
//    }

//    #endregion



//    #region 핸들러
    
//    //private void AddHandler()
//    //{
//    //    sceneOfficeRoom.officeModuleHandler.onHostIdReceived.AddListener(S_GetHostID);
//    //    sceneOfficeRoom.officeModuleHandler.onPermissionReceived.AddListener(S_MeetGetPermission);
//    //}
    
//    //private void RemoveHandler()
//    //{
//    //    sceneOfficeRoom.officeModuleHandler.onHostIdReceived.RemoveListener(S_GetHostID);
//    //    sceneOfficeRoom.officeModuleHandler.onPermissionReceived.RemoveListener(S_MeetGetPermission);
//    //}
    
//    #endregion



//    #region 버튼 이벤트

//    /// <summary>
//    /// 방장이 사람들 권한을 설정
//    /// 정보 반영 버튼을 누르면 실행됨
//    /// 저장이 되면 사람들한테 권한이 전달됨
//    /// </summary>
//    private void OnClick_SendPermission()
//    {
//        //C_MEETING_SET_PERMISSION setPermission = new C_MEETING_SET_PERMISSION();

//        //USER_PERMISSION userPermission = new USER_PERMISSION();
//        //// 호스트 정보부터 순서대로 보냄 // TODO : 수정해야됨... 새로운 방장정보도 같이 수정해서 보내야될 것 같다
//        //userPermission.ClientId = sceneOfficeRoom.curOfficeInfo.hostId;
//        //userPermission.ScreenPermission = tog_Host_Screen.isOn;
//        //userPermission.ChatPermission = true; // 방장은 항상 true
//        //userPermission.VideoPermission = true;
//        //userPermission.VoicePermission = true;
//        //userPermission.Type = (int)changeHostType;

//        //setPermission.Permissions.Add(userPermission);

//        //// 게스트 정보 content 자식으로 있는 객체들의 정보를 가지고와서 보내기!
//        //for (int i = 0; i < scroll.content.childCount; i++)
//        //{
//        //    USER_PERMISSION permission = new USER_PERMISSION();
//        //    RoomUserData data = new RoomUserData();
//        //    data = scroll.content.GetChild(i).GetComponent<Item_OfficeUserInfoEnter>().roomUserData;

//        //    permission.ClientId = data.ClientData.clientId;
//        //    permission.ScreenPermission = data.permissions.Screen;
//        //    permission.ChatPermission = data.permissions.Chat;
//        //    permission.VideoPermission = data.permissions.Video;
//        //    permission.VoicePermission = data.permissions.Voice;
//        //    permission.Type = (int)data.positionType;

//        //    setPermission.Permissions.Add(permission);
//        //}

//        //// Debug.Log(setPermission.Permissions.Count);

//        //Single.RealTime.Send(sceneOfficeRoom.sesionType, setPermission);
//        //Debug.Log("setPermission 전송");

//        //// 전송 누르고나면 인터렉션 비활성화
//        //btn_ChangeSendData.interactable = false;
//        //SceneLogic.instance.Back();
//    }

//    #endregion



//    #region 실시간 패킷


//    private void S_GetHostID( PacketSession session, IMessage packet )
//    {
//        if (!sceneOfficeRoom.IsMyOfficeAuthority_Admin())
//        {
//            SceneLogic.instance.GetPopup<Popup_OfficeUserInfo>(Cons.Popup_OfficeUserInfo).SetPopupUI();
//        }
//        else
//        {
//            // sceneOfficeRoom.officeModuleHandler.SendGetPermission();
//        }
//    }

//    /// <summary>
//    /// 서버한테 인원 정보 받아서 데이터 저장
//    /// </summary>
//    /// <param name="session"></param>
//    /// <param name="packet"></param>
//    public void S_MeetGetPermission( PacketSession session, IMessage packet )
//    {
//        Debug.Log("S_MeetGetPermission");

//        // 데이터 초기화
//        InitUserData();

//        //S_MEETING_GET_PERMISSION getPermission = packet as S_MEETING_GET_PERMISSION;

//        //var permissions = getPermission.Permissions;

//        //for (int i = 0; i < permissions.Count; i++)
//        //{
//        //    if(permissions[i].ClientId.Contains(LocalPlayerData.MemberID)) continue;
            
//        //    RoomUserData data = GetUserData(permissions[i].ClientId);

//        //    if (data == null || data.ClientData.clientId == sceneOfficeRoom.curOfficeInfo.hostId) continue;

//        //    // 서버 정보를 저장
//        //    data.permissions.Screen = permissions[i].ScreenPermission;
//        //    data.permissions.Chat = permissions[i].ChatPermission;
//        //    data.permissions.Voice = permissions[i].VoicePermission;
//        //    data.permissions.Video = permissions[i].VideoPermission;
//        //    data.positionType = (eOfficeAuthority)permissions[i].Type;
//        //}

//        RefreshItems();
//    }

//    #endregion



//    #region 유저 item 업데이트

//    /// <summary>
//    /// 스크롤 업데이트
//    /// </summary>
//    private void RefreshItems()
//    {
//        //if (RealtimeUtils.IsSyncStop(sceneOfficeRoom.sessionType)) return;

//        dataUpdate_Callback?.Invoke();

//        //var number = scroll.content.transform.childCount - receiveUserDatas.Count; // 호스트 정보 제외한 나머지를 item으로 생성해야됨

//        // content 자식 수 > meeting 데이터 수  
//        //if (number > 0)
//        //{
//        //    for (var i = 0; i < number; i++)
//        //    {
//        //        Destroy(scroll.content.transform.GetChild(i).gameObject);
//        //    }
//        //}
//        //else if (number < 0)// content 자식 수 < meeting 데이터 수  
//        //{
//        //    for (var i = 0; i > number; i--)
//        //    {
//        //        Item_OfficeUserInfoEnter item = Instantiate(enterItem, scroll.content.transform); // content 자식으로 생성
//        //        item.transform.SetAsLastSibling();
//        //    }
//        //}

//		//if (receiveUserDatas.Count == scroll.content.transform.childCount)
//		//{
//		//	for (var i = 0; i < scroll.content.transform.childCount; i++)
//		//	{
//		//		Item_OfficeUserInfoEnter item = scroll.content.transform.GetChild(i).GetComponent<Item_OfficeUserInfoEnter>();
//		//		item.SetItem(receiveUserDatas[i]);
//		//	}
//		//}
//    }

//    #endregion

//    public void InitItems()
//	{
//        for(int i = 0; i < scroll.content.childCount; i++)
//		{
//            scroll.content.GetChild(i).GetComponent<Item_OfficeUserInfoEnter>().InitItem();
//        }
//	}
//}
