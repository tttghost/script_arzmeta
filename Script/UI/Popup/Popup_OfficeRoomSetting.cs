using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using db;
using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using MEC;
using Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_OfficeRoomSetting : Popup_Basic
{
    #region members
    public bool IsWaitingRoom = true;
    [SerializeField] GameObject HostUI;
    CancellationTokenSource source = new CancellationTokenSource();

    Scene_OfficeRoom sceneOfficeRoom;
    OfficeSpaceInfo masterOfficeRoomInfo = new OfficeSpaceInfo();

    // Common
    TMP_Text txtmp_title, txtmp_RoomTime, txtmp_RoomNameTitle, txtmp_RoomName, txtmp_HostNameTitle, txtmp_HostName, txtmp_PlayerCountTitle, txtmp_PlayerCount;

    // 비공개 여부
    Toggle tog_IsPrivate;
    TMP_Text txtmp_IsPrivateTitle;

    // 룸코드
    TMP_Text txtmp_RoomCodeTItle, txtmp_RoomCode, txtmp_RoomCodeCopyMessage;
    Button btn_RoomCodeCopy;
    Image img_RoomCodeCopyMessage;

    // 비밀번호
    TMP_Text txtmp_PasswordTitle;
    TMP_InputField input_Password;

    // 닫기 버튼
    Button btn_close;

    // 호스트 UI 
    Toggle tog_SetPassword;
    Toggle tog_SetShutDown;
    Button btn_Dismiss;
    TMP_Text txtmp_Dismiss;
    TMP_Text txtmp_SetShutdown;
    Button btn_SaveData;
    Button btn_SubPlayerCount;
    Button btn_AddPlayerCount;

    string privateTogStr;
    #endregion

    #region 초기화
    protected override void SetMemberUI()
    {
        popupAnimator = GetComponent<Animator>();
        sceneOfficeRoom = FindObjectOfType<Scene_OfficeRoom>();

        AddHandler(); //MWC
    }

    protected override void Start()
    {
        base.Start();
        Util.RunCoroutine(Init());
    }

    protected override void OnEnable()
    {
        // 활성화 할 때마다 최신 정보 받아오기
        //sceneOfficeRoom.officeModuleHandler.SendGetRoomInfo();
        ShowRemainTime().Forget();
    }

    protected override void OnDisable()
    {
        //// 코루틴 끄기
        base.OnDisable();
        source.Cancel();
    }

    void OnDestroy()
    {
        if (RealtimeManager.Instance == null) return;
        RemoveHandler();
    }

    IEnumerator<float> Init()
    {
        yield return Timing.WaitUntilTrue(() => sceneOfficeRoom.curOfficeRoomInfo != null);
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance);

        GetData();
        GetUI();
        AddListener();
    }

    /// <summary>
    /// 핸들러 등록
    /// </summary>
    void AddHandler()
    {
        //RealtimeUtils.AddHandler(SceneLogic.instance.sesionType, PacketManager.MsgId.PKT_S_MEETING_SET_ROOM_INFO, this, S_SetRoomInfo);
        //RealtimeUtils.AddHandler(SceneLogic.instance.sesionType, PacketManager.MsgId.PKT_S_MEETING_GET_HOST, this, S_GetHostId);
    }

    void AddListener()
    {
        // sceneOfficeRoom.officeModuleHandler.realtimeRoomInfoAction += SetUI;
    }

    void RemoveHandler()
    {
        //RealtimeUtils.RemoveHandler(SceneLogic.instance.sesionType, PacketManager.MsgId.PKT_S_MEETING_SET_ROOM_INFO, this);
        //RealtimeUtils.RemoveHandler(SceneLogic.instance.sesionType, PacketManager.MsgId.PKT_S_MEETING_GET_HOST, this);
    }

    void GetData()
    {
        masterOfficeRoomInfo = Single.MasterData.GetOfficeSpaceInfoDatas(sceneOfficeRoom.officeModeType).
            FirstOrDefault(x => x.sceneName == SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    void GetUI()
    {
        #region 텍스트

        txtmp_title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_title), new MasterLocalData("1088"));
        txtmp_RoomNameTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomNameTitle), new MasterLocalData("1097"));
        txtmp_HostNameTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_HostNameTitle), new MasterLocalData("1025"));
        txtmp_RoomCodeTItle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCodeTItle), new MasterLocalData("1010"));
        txtmp_PasswordTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PasswordTitle), new MasterLocalData("1015"));
        txtmp_RoomCodeCopyMessage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCodeCopyMessage), new MasterLocalData("1158"));
        txtmp_Dismiss = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Dismiss), new MasterLocalData("1092"));
        txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
        txtmp_HostName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_HostName));
        txtmp_PlayerCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerCount));
        txtmp_RoomTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomTime));
        txtmp_RoomCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCode));
        txtmp_SetShutdown = GetUI_TxtmpMasterLocalizing(nameof(txtmp_SetShutdown));
        txtmp_IsPrivateTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsPrivateTitle));
        #endregion

        #region 버튼


        btn_SaveData = GetUI_Button(nameof(btn_SaveData), OnClickSaveData);
        btn_AddPlayerCount = GetUI_Button(nameof(btn_AddPlayerCount), () =>
        {
            string countText = txtmp_PlayerCount.text;

            // 마스터에 저장되어있는 최대 인원 수만큼 늘릴 수 있음
            if (int.Parse(countText) < masterOfficeRoomInfo.maxCapacity)
                Util.SetMasterLocalizing(txtmp_PlayerCount, (int.Parse(countText) + 1).ToString());
        });
        btn_SubPlayerCount = GetUI_Button(nameof(btn_SubPlayerCount), () =>
        {
            string countText = txtmp_PlayerCount.text;
            if (int.Parse(countText) > masterOfficeRoomInfo.minCapacity)
                Util.SetMasterLocalizing(txtmp_PlayerCount, (int.Parse(countText) - 1).ToString());
        });
        //btn_Dismiss = GetUI_Button("btn_Dismiss", sceneOfficeRoom.officeModuleHandler.SendDisMiss);
        btn_close = GetUI_Button(nameof(btn_close), () => SceneLogic.instance.Back());

        btn_RoomCodeCopy = GetUI_Button(nameof(btn_RoomCodeCopy), () =>
        {
            Util.RunCoroutine(ShowCopyImg());
            CopyRoomId();
        });

        #endregion

        // Input
        input_Password = GetUI_TMPInputField(nameof(input_Password));
        input_Password.interactable = false;

        // Image
        img_RoomCodeCopyMessage = GetUI_Img(nameof(img_RoomCodeCopyMessage)); // 복사 완료 이미지 오브젝트

        // Toggle
        tog_SetShutDown = GetUI_Toggle(nameof(tog_SetShutDown),
            () => Util.SetMasterLocalizing(txtmp_SetShutdown, new MasterLocalData("1159")),
            () => Util.SetMasterLocalizing(txtmp_SetShutdown, new MasterLocalData("1160")));

        tog_SetPassword = GetUI_Toggle(nameof(tog_SetPassword),
            () => input_Password.interactable = true,
            () => input_Password.interactable = false);

        tog_IsPrivate = GetUI_Toggle(nameof(tog_IsPrivate),
            () => Util.SetMasterLocalizing(txtmp_IsPrivateTitle, new MasterLocalData("1016")),
            () => Util.SetMasterLocalizing(txtmp_IsPrivateTitle, new MasterLocalData("1100")));

    }

    /// <summary>
    /// 남은 시간 보기 
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid ShowRemainTime()
    {
        // 남아있는 시간을 받고 -deltatime만 해줘도 될꺼같음.. 
        //yield return Timing.WaitUntilTrue(() => txtmp_RoomTime != null);
        await UniTask.WaitUntil(() => txtmp_RoomTime != null);
        TimeSpan remainTime;

        //// TODO : 시간 다시 계산할 것 
        //while (true)
        //{
        //    remainTime = Convert.ToDateTime(sceneOfficeRoom.curOfficeRoomInfo.endTime) - DateTime.Now;
        //    txtmp_RoomTime.text = $"{remainTime.Hours.ToString().PadLeft(2,'0')}:{remainTime.Minutes.ToString().PadLeft(2,'0')}";

        //    await UniTask.Yield(cancellationToken: source.Token);
        //    //yield return Timing.WaitForOneFrame;
        //}
    }
    #endregion

    #region GET/SET RoomInfo

    /// <summary>
    /// 호스트 / 게스트 상태에 따라 값 설정
    /// </summary>
    private void SetUI()
    {
        bool isHost = sceneOfficeRoom.IsAdmin();
        HostUI.SetActive(isHost);

        // 호스트가 아니면 건드리지 못함
        tog_IsPrivate.interactable = isHost;
        tog_SetPassword.interactable = isHost;
        tog_SetShutDown.interactable = isHost;
        input_Password.interactable = isHost;
        btn_AddPlayerCount.gameObject.SetActive(isHost);
        btn_SubPlayerCount.gameObject.SetActive(isHost);

        Util.SetMasterLocalizing(txtmp_RoomName, sceneOfficeRoom.curOfficeRoomInfo.RoomName); // 방 이름
        tog_IsPrivate.isOn = !sceneOfficeRoom.curOfficeRoomInfo.IsAdvertising; // 비공개 여부
        privateTogStr = tog_IsPrivate.isOn ? "1016" : "1100";  // 비공개 | 공개
        Util.SetMasterLocalizing(txtmp_IsPrivateTitle, new MasterLocalData(privateTogStr));

        Util.SetMasterLocalizing(txtmp_PlayerCount, sceneOfficeRoom.curOfficeRoomInfo.Personnel.ToString()); // 방 인원
        input_Password.text = sceneOfficeRoom.curOfficeRoomInfo.Password; // 비밀번호
        Util.SetMasterLocalizing(txtmp_RoomCode, sceneOfficeRoom.curOfficeRoomInfo.Roomcode);
        Util.SetMasterLocalizing(txtmp_HostName, sceneOfficeRoom.curOfficeRoomInfo.HostNickname);

        if (!sceneOfficeRoom.curOfficeRoomInfo.IsShutdown)
        {
            tog_SetShutDown.isOn = true;
            Util.SetMasterLocalizing(txtmp_SetShutdown, new MasterLocalData("1159")); //개방 중
        }
        else
        {
            tog_SetShutDown.isOn = false;
            Util.SetMasterLocalizing(txtmp_SetShutdown, new MasterLocalData("1160")); // 폐쇄
        }


        if (isHost)  // 호스트 UI 설정
        {
            tog_SetPassword.isOn = sceneOfficeRoom.curOfficeRoomInfo.Password != string.Empty;
            input_Password.interactable = tog_SetPassword.isOn;
        }

    }


    // callback으로 수정하기
    private void S_SetRoomInfo(PacketSession session, IMessage packet)
    {
        //S_MEETING_SET_ROOM_INFO setRoomInfo = packet as S_MEETING_SET_ROOM_INFO;
        //if (setRoomInfo.Success)
        //{
        //    Debug.Log("방 정보 보내기 성공");
        //    // 최신정보 받아오기
        //    sceneOfficeRoom.officeModuleHandler.SendGetRoomInfo();
        //    SceneLogic.instance.PopPopup();
        //}
    }
    #endregion

    #region 호스트 기능

    private void OnClickSaveData()
    {
        //sceneOfficeRoom.officeModuleHandler.SendSetRoomInfo(new C_MEETING_SET_ROOM_INFO
        //{
        //    Personnel = int.Parse(txtmp_PlayerCount.text),
        //    Password = input_Password.text.ToUpper(),
        //    IsShutdown = !tog_SetShutDown.isOn,
        //    IsAdvertising = tog_IsPrivate.isOn,
        //    IsWaitingRoom = IsWaitingRoom // 사용자가 설정하는 버튼을 만들어야됨
        //});
    }


    #endregion

    /// <summary>
    /// 복사하기 이미지 3초만 보이기 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> ShowCopyImg()
    {
        img_RoomCodeCopyMessage.gameObject.SetActive(true);
        yield return Timing.WaitForSeconds(3f);

        img_RoomCodeCopyMessage.gameObject.SetActive(false);
        yield return Timing.WaitForOneFrame;
    }


    /// <summary>
    /// 방 정보 복사하기 기능
    /// </summary>
    StringBuilder sb = new StringBuilder();
    private void CopyRoomId()
    {
        /*
        testnickname 방으로 초대합니다!
        제목 : 태영의 미팅룸
        설명 : 태영 회의실에 어서오세요.
        테마 : 회의
        일시 : 2022.12.02 PM 07:21
        ----------------------------
        룸 코드 : 00000003
        비밀번호 : LN7791
        ----------------------------
         */

        sb.Clear();
        sb.AppendLine($"{txtmp_HostName.text} 방으로 초대합니다!");
        sb.AppendLine($"제목 : {sceneOfficeRoom.curOfficeRoomInfo.RoomName}");
        sb.AppendLine($"설명 : {sceneOfficeRoom.curOfficeRoomInfo.Description}");
        sb.AppendLine($"테마 : {sceneOfficeRoom.curOfficeRoomInfo.TopicType}"); // 토픽 주제 데이터 넣기 
        sb.AppendLine($"일시 : {sceneOfficeRoom.curOfficeRoomInfo.StartTime}");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"룸 코드 : {sceneOfficeRoom.curOfficeRoomInfo.Roomcode}");

        if (sceneOfficeRoom.curOfficeRoomInfo.Password != string.Empty)
        {
            sb.AppendLine($"비밀번호 : {sceneOfficeRoom.curOfficeRoomInfo.Password}");
        }
        sb.AppendLine("----------------------------------------");

        UniClipboard.SetText(sb.ToString());
    }


    /// <summary>
    /// 방장 정보가 바뀔 때 자동으로 방 데이터 업데이트
    /// </summary>
    /// <param name="session"></param>
    /// <param name="packet"></param>
    private void S_GetHostId(PacketSession session, IMessage packet)
    {
        //sceneOfficeRoom.officeModuleHandler.SendGetRoomInfo();

    }

    /// <summary>
    /// 룸코드 공유 버튼 이벤트
    /// </summary>
    public void OnShareButtonClick()
    {
        Debug.Log("룸코드 공유 버튼 이벤트");
    }
}