using Cysharp.Threading.Tasks;
using db;
using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using MEC;
using Office;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using Unity.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Popup_OfficeRoomSave : PopupBase
{
    private ShareLinkInfo shareLinkInfo = new ShareLinkInfo();

    // 룸 진행시간 계산을 위한 변수
    private float processTime;
    private float disableTime = 0.0f;
    private Coroutine co_processTime;

    // view
    private View_NumberInput view_NumberInput;

    #region 변수
    // go
    private GameObject go_Advenced;
    private GameObject go_ExitSave;
    private GameObject go_UpgradeAdvenced;
    private GameObject go_Thumbnail;
    private GameObject go_time;
    private GameObject go_Topic;
    private GameObject go_Password;
    private GameObject go_Space;
    private GameObject go_PlayerCountAndPlayTime;


    // 룸 진행 시간
    private TMP_Text txtmp_RoomTime;

    // 룸이름  
    private TMP_Text txtmp_RoomName;

    // 룸 코드
    private TMP_Text txtmp_RoomCode;

    // 토픽    
    private TMP_Text txtmp_Topic;

    // 설명    
    private TMP_Text txtmp_RoomDesc;

    // 비밀번호
    private TMP_Text txtmp_Password;
    private TMP_Text txtmp_PasswordRandom;

    // 인원    
    private TMP_Text txtmp_PlayerCount;
    private TMP_Text txtmp_PlayerCountChange;

    // 진행시간
    private TMP_Text txtmp_PlayTime;
    private TMP_Text txtmp_PlayTimeChange;

    // 마지막
    private TMP_Text txtmp_Advenced;
    private TMP_Text txtmp_IsWaitingRoom;
    private TMP_Text txtmp_IsWaitingRoomDesc;
    private TMP_Text txtmp_IsAdvertising;
    private TMP_Text txtmp_IsAdvertisingDesc;
    private TMP_Text txtmp_IsShutDown;
    private TMP_Text txtmp_IsShutDownDesc;
    private TMP_Text txtmp_ExitRoom;
    private TMP_Text txtmp_SaveRoom;

    // input
    private TMP_InputField input_Password;

    // img
    private Image img_Thumbnail;

    private Button btn_RoomCode;
    private Button btn_RoomCodeCopy;

    // 비밀번호 랜덤
    private Button btn_PasswordRandom;

    // 인원 설정
    private Button btn_PlayerCount;

    // 진행시간
    private Button btn_PlayTime;

    // 방생성
    private Button btn_ExitRoom;
    private Button btn_SaveRoom;
    private Button btn_Exit;

    // 비밀번호
    private TogglePlus togplus_PasswordLock;

    // 공개비공개(폐쇄)
    private TogglePlus togplus_IsShutDown;

    // 홍보노출
    private TogglePlus togplus_IsAdvertising;

    // 대기실
    private TogglePlus togplus_IsWaitingRoom;

    // PRO 로 업그레이드 안내
    private TMP_Text txtmp_MyOfficeGrade;

    // 관전자 입장
    private GameObject go_ObserverTog;
    private TogglePlus togplus_Observer;
    private TMP_InputField input_ObserverCount;
    private Button btn_SubObserverCount;
    private Button btn_AddObserverCount;

    public OfficeGradeAuthority officeGradeAuthority => Single.MasterData.dataOfficeGradeAuthority.GetData(LocalPlayerData.OfficeGradeType);

    private OfficeSpaceInfo masterOfficeRoomInfo = new OfficeSpaceInfo();

    private Scene_OfficeRoom sceneOfficeRoom;

    private CancellationTokenSource source = new CancellationTokenSource();

    private (int hour, int min) playTime;
    private (int hour, int min) callbackPlayTime;

    //private int curPlayerCount;
    //private int callbackCurPlayerCount;

    private S_OFFICE_GET_ROOM_INFO getRoomInfo;


    // thumbnail
    private Button btn_ThumbnailPreviewClose;
    private GameObject go_ThumbnailPreview;
    private Image img_ThumbnailPreview;
    public string thumbnailName;
    public string thumbnailPath;

    #endregion


    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        // view
        view_NumberInput = GetView<View_NumberInput>();
        //view_NumberInput = (View_NumberInput)GetUI<UIBase>( Cons.View_NumberInput );

        go_Advenced = GetChildGObject(nameof(go_Advenced));
        go_ExitSave = GetChildGObject(nameof(go_ExitSave));
        go_UpgradeAdvenced = GetChildGObject(nameof(go_UpgradeAdvenced));
        go_Thumbnail = GetChildGObject(nameof(go_Thumbnail));
        go_time = GetChildGObject(nameof(go_time));
        go_Topic = GetChildGObject(nameof(go_Topic));
        go_Password = GetChildGObject(nameof(go_Password));
        go_Space = GetChildGObject(nameof(go_Space));
        go_PlayerCountAndPlayTime = GetChildGObject(nameof(go_PlayerCountAndPlayTime));

        // 고정된 UI 텍스트 로컬라이징
        GetUI_TxtmpMasterLocalizing("txtmp_PopupTitle", new MasterLocalData("office_room_info"));
        GetUI_TxtmpMasterLocalizing("txtmp_RoomNameTitle", new MasterLocalData("office_room_name"));
        GetUI_TxtmpMasterLocalizing("txtmp_RoomCodeTitle", new MasterLocalData("1010"));
        GetUI_TxtmpMasterLocalizing("txtmp_TopicTitle", new MasterLocalData("office_topic_set"));
        GetUI_TxtmpMasterLocalizing("txtmp_RoomDescTitle", new MasterLocalData("office_room_desc"));
        GetUI_TxtmpMasterLocalizing("txtmp_PasswordTitle", new MasterLocalData("office_room_password"));
        GetUI_TxtmpMasterLocalizing("txtmp_PlayerCountTitle", new MasterLocalData("office_room_people"));
        GetUI_TxtmpMasterLocalizing("txtmp_SpaceTitle", new MasterLocalData("office_space_select"));
        GetUI_TxtmpMasterLocalizing("txtmp_PlayTimeTitle", new MasterLocalData("office_room_time"));

        GetUI_TxtmpMasterLocalizing("txtmp_Advenced", new MasterLocalData("1048"));
        GetUI_TxtmpMasterLocalizing("txtmp_UpgradeAdvencedDesc", new MasterLocalData("office_grade_limit_info_common"));
        GetUI_TxtmpMasterLocalizing("txtmp_UpgradeConfirm", new MasterLocalData("office_confirm_upgrade"));
        GetUI_TxtmpMasterLocalizing("txtmp_UpgradeAdvenced", new MasterLocalData("office_upgrade"));

        GetUI_TxtmpMasterLocalizing("txtmp_Thumbnail", new MasterLocalData("office_upload_thumbnail"));

        txtmp_MyOfficeGrade = GetUI_TxtmpMasterLocalizing(
            nameof(txtmp_MyOfficeGrade)
            //,new MasterLocalData("office_confirm_upgrade")
            );

        // 남은 시간
        txtmp_RoomTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomTime));

        // 룸이름
        txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));

        // 룸코드
        txtmp_RoomCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCode));

        // 토픽
        txtmp_Topic = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Topic));

        // 설명
        txtmp_RoomDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomDesc));

        // 비밀번호
        txtmp_Password = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Password));
        txtmp_PasswordRandom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PasswordRandom), new MasterLocalData("common_auto"));

        // 인원
        txtmp_PlayerCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerCount));
        txtmp_PlayerCountChange = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerCountChange));

        // 진행시간
        txtmp_PlayTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayTime));
        txtmp_PlayTimeChange = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayTimeChange));


        txtmp_IsWaitingRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsWaitingRoom), new MasterLocalData("office_anteroom"));

        txtmp_IsWaitingRoomDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsWaitingRoomDesc), new MasterLocalData("office_info_anteroom"));

        txtmp_IsAdvertising = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsAdvertising), new MasterLocalData("office_advertising"));

        txtmp_IsAdvertisingDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsAdvertisingDesc), new MasterLocalData("office_info_advertising"));

        txtmp_IsShutDown = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsShutDown), new MasterLocalData("office_room_close"));

        txtmp_IsShutDownDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_IsShutDownDesc), new MasterLocalData("office_info_room_close"));

        txtmp_ExitRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ExitRoom), new MasterLocalData("office_room_dismiss"));
        txtmp_SaveRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_SaveRoom), new MasterLocalData("common_save"));

        // input
        input_Password = GetUI<TMP_InputField>(nameof(input_Password));
        input_Password.onValueChanged.AddListener( (value) => 
        {
            getRoomInfo.Password = value;
        });

        input_Password.characterLimit = 10;

        // 닫기
        btn_Exit = GetUI_Button(nameof(btn_Exit), OnClick_Exit);

        // 룸코드
        btn_RoomCode = GetUI_Button(nameof(btn_RoomCode), () => OnClick_ShareLink());
        btn_RoomCodeCopy = GetUI_Button(nameof(btn_RoomCodeCopy), () => OnClick_RoomCodeCopy());

        // 비밀번호
        btn_PasswordRandom = GetUI_Button(nameof(btn_PasswordRandom), () =>
        {
            input_Password.text = Util.MakeRandomPassword();
            getRoomInfo.Password = input_Password.text;
        });

        // 인원
        btn_PlayerCount = GetUI_Button(nameof(btn_PlayerCount), OnClick_PlayerCount);

        // 진행시간 : 시시:분분
        btn_PlayTime = GetUI_Button(nameof(btn_PlayTime), OnClick_PlayTime);

        // 방파괴 or 방저장
        btn_ExitRoom = GetUI_Button(nameof(btn_ExitRoom), OnClick_ExitRoom);
        btn_SaveRoom = GetUI_Button(nameof(btn_SaveRoom), OnClick_SaveRoom);

        img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));

        togplus_PasswordLock = GetUI<TogglePlus>(nameof(togplus_PasswordLock)); //패스워드
        togplus_PasswordLock.SetToggleAction((b) =>
        {
            input_Password.text = !b ? "" : input_Password.text;
            input_Password.gameObject.SetActive(b);
            //btn_PasswordRandom.interactable = b;
            //togplus_PasswordLock.transform.parent.GetComponent<CanvasGroup>().interactable = b;
        });

        togplus_IsShutDown = GetUI<TogglePlus>(nameof(togplus_IsShutDown));
        togplus_IsShutDown.SetToggleAction((b) => getRoomInfo.IsShutdown = b);
        togplus_IsAdvertising = GetUI<TogglePlus>(nameof(togplus_IsAdvertising));
        togplus_IsAdvertising.SetToggleAction((b) => getRoomInfo.IsAdvertising = b);
        togplus_IsWaitingRoom = GetUI<TogglePlus>(nameof(togplus_IsWaitingRoom));
        togplus_IsWaitingRoom.SetToggleAction((b) => getRoomInfo.IsWaitingRoom = b);

        // thumbnail
        go_ThumbnailPreview = GetChildGObject(nameof(go_ThumbnailPreview));
        img_ThumbnailPreview = GetUI_Img(nameof(img_ThumbnailPreview));
        btn_ThumbnailPreviewClose = GetUI_Button(nameof(btn_ThumbnailPreviewClose), () => OnClick_ResetThumbnail());

        GetUI_Button("btn_ThumbnailLoad", OnClick_ThumbnailLoad);

        // 관전자 입장 관련 UI
        go_ObserverTog = GetChildGObject(nameof(go_ObserverTog));
        togplus_Observer = GetUI<TogglePlus>(nameof(togplus_Observer));
        togplus_Observer.SetToggleAction( (isOn) =>
        {
            go_ObserverTog.SetActive(isOn);
            if (isOn == false)
                input_ObserverCount.text = "0";
        });
        
        input_ObserverCount = GetUI_TMPInputField(nameof(input_ObserverCount), (value) => 
        {
            getRoomInfo.Observer = Convert.ToInt32(input_ObserverCount.text);
        });

        btn_SubObserverCount = GetUI_Button(nameof(btn_SubObserverCount), () =>
        {
            getRoomInfo.Observer -= 1;
            if (getRoomInfo.Observer < 0)
                getRoomInfo.Observer = 0;

            input_ObserverCount.text = getRoomInfo.Observer.ToString();
        });

        btn_AddObserverCount = GetUI_Button(nameof(btn_AddObserverCount), () =>
        {
            getRoomInfo.Observer += 1;
            if (getRoomInfo.Observer > masterOfficeRoomInfo.maxObserver)
                getRoomInfo.Observer = masterOfficeRoomInfo.maxObserver;

            input_ObserverCount.text = getRoomInfo.Observer.ToString();
        });
    }

    private void OnClick_RoomCodeCopy()
    {
        Scene_Room_Exposition_Booth scene_Booth = SceneLogic.instance as Scene_Room_Exposition_Booth;
        Booth booth = scene_Booth.GetBooth();
        string shareMessage = $"{booth.name} {Util.GetMasterLocalizing("office_booth_code")} -> {booth.roomCode}";

        Util.CopyToClipboard(shareMessage);
        OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("10301")));
    }

    private void OnValueChanged_ObserverCount( string value )
	{
        getRoomInfo.Observer = Convert.ToInt32(value);
    }

    protected override void Start()
    {
        base.Start();

        sceneOfficeRoom = SceneLogic.instance as Scene_OfficeRoom;

        if(sceneOfficeRoom != null)
        {
            // 마스터 오피스룸정보 가져오기
            masterOfficeRoomInfo =
                Single.MasterData.GetOfficeSpaceInfoDatas(sceneOfficeRoom.officeModeType).
                FirstOrDefault(x => x.sceneName == SceneManager.GetActiveScene().name);
        }

        //
        // TODO: 6월 업데이트 버전에서는 썸네일 기능이 못들어가기 때문에 비활성화
        //
        go_Thumbnail.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Util.UtilOffice.IsOffice()) 
            Init_Office();
        else if (Util.UtilOffice.IsExposition()) 
            Init_Exposition();
    }

    private void Init_Office()
    {

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_ROOM_INFO, this, S_OFFICE_GET_ROOM_INFO);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_ROOM_INFO, this, S_OFFICE_SET_ROOM_INFO);

        go_time.SetActive(true);
        go_Topic.SetActive(true);
        go_Password.SetActive(true);
        go_Space.SetActive(true);
        go_PlayerCountAndPlayTime.SetActive(true);
        go_Advenced.SetActive(true);
        go_ExitSave.SetActive(true);
        btn_RoomCode.gameObject.SetActive(true);
        btn_RoomCodeCopy.gameObject.SetActive(false);

        txtmp_RoomTime.gameObject.SetActive(false);

        // 룸정보 패킷 받아오기
        C_OFFICE_GET_ROOM_INFO roominfo = new C_OFFICE_GET_ROOM_INFO();
        Single.RealTime.Send(roominfo);

        // 룸정보 경과 시간 표시를 위한 수치 계산
        // 팝업이 활성화 됐을 때 : 현재 시간 - 이전 비활성화 시간을 같이 계산해 준다
        if (processTime > 0.0f)
        {
            processTime += Time.fixedTime - disableTime;
            if (processTime > getRoomInfo.RunningTime * 60)
                processTime = getRoomInfo.RunningTime * 60;
        }
    }

    private void Init_Exposition()
    {
        go_time.SetActive(false);
        go_Topic.SetActive(false);
        go_Password.SetActive(false);
        go_Space.SetActive(false);
        go_PlayerCountAndPlayTime.SetActive(false);
        go_Advenced.SetActive(false);
        go_ExitSave.SetActive(false);

        btn_RoomCode.gameObject.SetActive(false);
        btn_RoomCodeCopy.gameObject.SetActive(true);

        var scene_Booth = SceneLogic.instance as Scene_Room_Exposition_Booth;
        scene_Booth.GetBoothDetail(() => RefreshUI(scene_Booth.GetBooth()));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        source.Cancel();

        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_ROOM_INFO, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_ROOM_INFO, this);

        // 팝업이 비활성화 시간을 기록
        disableTime = Time.fixedTime;
    }

    /// <summary>
    /// 실시간데이터 셋업
    /// </summary>
    private async void RefreshUI(S_OFFICE_GET_ROOM_INFO roominfo)
    {
        // 룸 진행 시간 갱신
        txtmp_RoomTime.gameObject.SetActive(true);

        if (co_processTime != null)
            StopCoroutine(co_processTime);

        float remain_sec = roominfo.PassedTime * 60;
        if (processTime < remain_sec)
		{
            processTime = roominfo.PassedTime * 60;
        }
        txtmp_RoomTime.text = $"{processTime / 60:00}:{processTime % 60:00} / {roominfo.RunningTime:00}:{0:00}";

        co_processTime = StartCoroutine(Co_ProcessTime());

        // 방 이름
        txtmp_RoomName.text = roominfo.RoomName;

        // 방 코드
        txtmp_RoomCode.text = roominfo.Roomcode;

        // 토픽 이름
        Util.SetMasterLocalizing(txtmp_Topic, new MasterLocalData(
            Single.MasterData.dataOfficeTopicType.GetData(roominfo.TopicType).name));

        // 방 설명
        txtmp_RoomDesc.text = roominfo.Description;

        // 썸네일(공간)
        string spaceInfoId = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(roominfo.SpaceInfoId)).thumbnail;
        img_Thumbnail.sprite = Single.Resources.Load<Sprite>(Cons.Path_OfficeThumbnail + spaceInfoId);


        #region 썸네일 (프리뷰)
        img_ThumbnailPreview.sprite = null;
        Sprite thumbnail = await Util.UtilOffice.GetThumbnail_Office(roominfo.Roomcode, roominfo.Thumbnail);

        if (thumbnail != null)
        {
            img_ThumbnailPreview.sprite = thumbnail;
        }
        #endregion




        // 대기실
        togplus_IsWaitingRoom.SetToggleIsOn(roominfo.IsWaitingRoom);

        // 홍보노출
        togplus_IsAdvertising.SetToggleIsOn(roominfo.IsAdvertising);

        // 폐쇄
        togplus_IsShutDown.SetToggleIsOn(roominfo.IsShutdown);

        callbackPlayTime = Util.Min2HourMin(roominfo.RunningTime);

        Callback_PlayerCount();
        Callback_PlayTime();

        // 비밀번호
        togplus_PasswordLock.gameObject.SetActive(false);
        input_Password.gameObject.SetActive(false);
        txtmp_Password.gameObject.SetActive(false);

        btn_PlayerCount.gameObject.SetActive(false);
        btn_PlayerCount.interactable = false;
        txtmp_PlayerCount.gameObject.SetActive(false);

        btn_PlayTime.gameObject.SetActive(false);
        btn_PlayTime.interactable = false;
        txtmp_PlayTime.gameObject.SetActive(false);

        go_ExitSave.SetActive(false);

        switch ((OfficeAuthority)sceneOfficeRoom.myPermission.Authority)
        {
        case OfficeAuthority.관리자:
            {
                togplus_PasswordLock.gameObject.SetActive(true);
                input_Password.text = roominfo.Password;
                bool isPasswordIsOn = roominfo.Password == string.Empty ? false : true;
                togplus_PasswordLock.SetToggleIsOn(isPasswordIsOn);
                if (isPasswordIsOn)
				{
                    input_Password.gameObject.SetActive(true);
                }
                else
				{
                    input_Password.gameObject.SetActive(false);
                }
                txtmp_Password.gameObject.SetActive(false);

                btn_PlayerCount.gameObject.SetActive(true);
                btn_PlayerCount.interactable = true;
                txtmp_PlayerCount.gameObject.SetActive(false);

                btn_PlayTime.gameObject.SetActive(true);
                btn_PlayTime.interactable = true;
                txtmp_PlayTime.gameObject.SetActive(false);

                go_Advenced.SetActive(true);

                // 관전자 수
                if (getRoomInfo.Observer > 0)
                {
                    togplus_Observer.SetToggleIsOn(true);
                    go_ObserverTog.SetActive(true);
                    input_ObserverCount.text = getRoomInfo.Observer.ToString();
                }
                else
				{
                    togplus_Observer.SetToggleIsOn(false);
                    go_ObserverTog.SetActive(false);
                    input_ObserverCount.text = "0";
                }

                if ((eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Pro)
                {
                    go_UpgradeAdvenced.SetActive(false);
                }
                else
                {
                    go_UpgradeAdvenced.SetActive(true);
                }

                go_ExitSave.SetActive(true);
            }
            break;

        default:
            {   
                togplus_PasswordLock.gameObject.SetActive(false);
                input_Password.gameObject.SetActive(false);
                txtmp_Password.text = roominfo.Password;
                txtmp_Password.gameObject.SetActive(true);

                btn_PlayerCount.gameObject.SetActive(false);
                btn_PlayerCount.interactable = false;
                txtmp_PlayerCount.gameObject.SetActive(true);

                btn_PlayTime.gameObject.SetActive(false);
                btn_PlayTime.interactable = false;
                txtmp_PlayTime.gameObject.SetActive(true);

                go_Advenced.SetActive(false);
                go_UpgradeAdvenced.SetActive(true);
                go_ExitSave.SetActive(false);
            }
            break;
        }

        // 내 오피스 등급 표시
        string strGrade = string.Empty;
        eOfficeGradeType officegrade = (eOfficeGradeType)LocalPlayerData.OfficeGradeType;
        switch (officegrade)
        {
        case eOfficeGradeType.Basic:
            {
                go_UpgradeAdvenced.SetActive(true);
                strGrade = Util.GetMasterLocalizing("office_grade_basic");
                strGrade = Util.GetMasterLocalizing("office_mygrade", strGrade);
            }
            break;

        case eOfficeGradeType.Pro:
            {
                go_UpgradeAdvenced.SetActive(false);
                strGrade = Util.GetMasterLocalizing("office_grade_pro");
                strGrade = Util.GetMasterLocalizing("office_mygrade", strGrade);
            }
            break;

        default:
            strGrade = string.Empty;
            break;
        }
        txtmp_MyOfficeGrade.text = strGrade;

        Util.RefreshScrollView(gameObject, "sview_OfficeRoomSave");
        Util.RefreshLayout(gameObject, "go_Content");
    }

    #region 유학박람회 초기화
    public void RefreshUI(Booth boothInfo)
    {
        if(txtmp_RoomName != null)
        {
            txtmp_RoomName.text = boothInfo.name;
        }
        if(txtmp_RoomCode != null)
        {
            txtmp_RoomCode.text = boothInfo.roomCode;
        }
        if(txtmp_RoomDesc != null)
        {
            txtmp_RoomDesc.text = boothInfo.description;
        }
    }
    #endregion

    #endregion

    private void OnClick_Exit()
    {
        go_ThumbnailPreview.SetActive(true);

        SceneLogic.instance.Back();
    }

    #region 온클릭
    /// <summary>
    /// 룸코드 공유
    /// </summary>
    private void OnClick_ShareLink()
    {
        shareLinkInfo.roomType = SHARELINK_TYPE.OFFICE_ENTER;
        shareLinkInfo.roomId = LocalContentsData.roomId;
        shareLinkInfo.nickName = getRoomInfo.HostNickname;
        shareLinkInfo.roomName = getRoomInfo.RoomName;
        shareLinkInfo.spaceInfoId = getRoomInfo.SpaceInfoId;
        shareLinkInfo.description = getRoomInfo.Description;
        shareLinkInfo.roomCode = getRoomInfo.Roomcode;
        shareLinkInfo.password = getRoomInfo.Password;  // 현재 접속한 방의 비밀번호
        shareLinkInfo.topicType = getRoomInfo.TopicType;
        shareLinkInfo.startTime = DateTime.Now.ToString(("yyyy.MM.dd tt hh:mm"));
        if (!string.IsNullOrEmpty(getRoomInfo.Password))
        {
            shareLinkInfo.isPassword = 1;
        }
        else shareLinkInfo.isPassword = 0;

        CreateShareLink.CreateLink(shareLinkInfo);
    }

    /// <summary>
    /// 진행시간 버튼 : 00:00 분, 회의를 진행하는 시간
    /// </summary>
    private void OnClick_PlayTime()
    {
        ChangeView(Cons.View_NumberInput);

        view_NumberInput.SetData(
            Callback_PlayTime,
            new MinNumMax(playTime.hour, 0, 99, "00", (hour) => callbackPlayTime.hour = hour),
            new MinNumMax(playTime.min, 0, 59, "00", (min) => callbackPlayTime.min = min)
            );
    }

    /// <summary>
    /// 진행시간 콜백
    /// </summary>
    private void Callback_PlayTime()
    {
        playTime = callbackPlayTime;

        // TODO: BLUEKID - 시간 세팅을 잘못했다는 메시지 노출 시켜야 함
        if (Util.HourMin2Min(playTime.hour, playTime.min) < (processTime / 60))
        {
            playTime = Util.Min2HourMin(officeGradeAuthority.timeLimit);
        }

        if (Util.HourMin2Min(playTime.hour, playTime.min) > officeGradeAuthority.timeLimit)
        {
            if ((eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic)
            {
                playTime = Util.Min2HourMin(officeGradeAuthority.timeLimit);
                OnClick_Upgrade(eOfficeGradeType.Basic);
            }
        }

        Util.SetMasterLocalizing(txtmp_PlayTime, new MasterLocalData("office_room_time_set", playTime.hour.ToString("00"), playTime.min.ToString("00")));
        Util.SetMasterLocalizing(txtmp_PlayTimeChange, new MasterLocalData("office_room_time_set", playTime.hour.ToString("00"), playTime.min.ToString("00")));
    }

    private void OnClick_Upgrade(eOfficeGradeType eOfficeGradeType)
    {
        GetPopup<Popup_OfficeGradeUpgrade>().SetData(eOfficeGradeType);
        PushPopup<Popup_OfficeGradeUpgrade>();
    }

    /// <summary>
    /// 총인원 버튼
    /// </summary>
    private void OnClick_PlayerCount()
    {
        OfficeSpaceInfo info = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(getRoomInfo.SpaceInfoId));
        if (info == null)
        {
            view_NumberInput.SetData(
            Callback_PlayerCount,
            new MinNumMax(getRoomInfo.Personnel, 1, 999, "0", (playerCount) =>
           {
               getRoomInfo.Personnel = playerCount;
           }));
        }
        else
        {
            view_NumberInput.SetData(
            Callback_PlayerCount,
            new MinNumMax(getRoomInfo.Personnel, 1, info.sitCapacity, "0", (playerCount) =>
           {
               getRoomInfo.Personnel = playerCount;
           }));
        }

        ChangeView(Cons.View_NumberInput);
    }

    /// <summary>
    /// 총인원 콜백
    /// </summary>
    private void Callback_PlayerCount()
    {
        if (getRoomInfo.Personnel < masterOfficeRoomInfo.minCapacity)
        {
            getRoomInfo.Personnel = masterOfficeRoomInfo.minCapacity;
        }
        else if (getRoomInfo.Personnel > masterOfficeRoomInfo.maxCapacity)
        {
            getRoomInfo.Personnel = masterOfficeRoomInfo.maxCapacity;
        }
        txtmp_PlayerCount.text = txtmp_PlayerCountChange.text = getRoomInfo.Personnel.ToString("0");
    }

    /// <summary>
    /// 방 파괴
    /// </summary>
    private void OnClick_ExitRoom()
    {
        ((Scene_OfficeRoom)SceneLogic.instance).OnClick_Back();
    }

    /// <summary>
    /// 썸네일 추가
    /// </summary>
    private async void OnClick_ThumbnailLoad()
    {
        thumbnailPath = await Util.Co_FindLocalTexPath();

        if (string.IsNullOrEmpty(thumbnailPath))
        {
            return;
        }

        img_ThumbnailPreview.sprite = await Util.Co_LoadLocalAsyncSprite(thumbnailPath);

        go_ThumbnailPreview.SetActive(true);
    }

    /// <summary>
    /// 썸네일 비우기
    /// </summary>
    private void OnClick_ResetThumbnail()
    {
        thumbnailPath = "";

        go_ThumbnailPreview.SetActive(false);
    }

    private OfficeReservationInfo GetOfficeReservReqData()
    {
        Office_CreateOfficeReservReq Office_ReservData = new Office_CreateOfficeReservReq();


        Dictionary<string, string> dummyDic = new Dictionary<string, string>();

        dummyDic.Add("password", !togplus_PasswordLock.GetToggleIsOn() ? "" : input_Password.text);
        dummyDic.Add("personnel", getRoomInfo.Personnel.ToString());
        dummyDic.Add("isWaitingRoom", (getRoomInfo.IsWaitingRoom ? 1 : 0).ToString());
        dummyDic.Add("isAdvertising", (getRoomInfo.IsAdvertising ? 1 : 0).ToString());


        return null;
    }


    /// <summary>
    /// 방 저장
    /// </summary>
    private void OnClick_SaveRoom()
    {
        Single.RealTime.Send(new C_OFFICE_SET_ROOM_INFO
        {
            Personnel = getRoomInfo.Personnel,
            Password = getRoomInfo.Password,
            IsShutdown = getRoomInfo.IsShutdown,
            IsAdvertising = getRoomInfo.IsAdvertising,
            IsWaitingRoom = getRoomInfo.IsWaitingRoom,
            RunningTime = Util.HourMin2Min(playTime.hour, playTime.min),
            Thumbnail = string.Empty,
            Observer = getRoomInfo.Observer,
        });
	}

    #endregion

    #region 실시간패킷
    private void S_OFFICE_GET_ROOM_INFO(PacketSession session, IMessage packet)
    {
        S_OFFICE_GET_ROOM_INFO message = packet as S_OFFICE_GET_ROOM_INFO;

        getRoomInfo = message;

        RefreshUI(getRoomInfo);
    }

    private void S_OFFICE_SET_ROOM_INFO(PacketSession session, IMessage packet)
    {
        S_OFFICE_SET_ROOM_INFO setRoomInfo = packet as S_OFFICE_SET_ROOM_INFO;
        if (setRoomInfo.Success)
        {
            // 룸정보 다시 받아오기
            C_OFFICE_GET_ROOM_INFO getRoomInfo = new C_OFFICE_GET_ROOM_INFO();
            Single.RealTime.Send(getRoomInfo);
            //SceneLogic.instance.PopPopup();
        }
    }
    #endregion

    #region 기타
    private IEnumerator Co_ProcessTime()
    {
        if (getRoomInfo == null)
            yield break;

        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            processTime += 1.0f;
            if (processTime > getRoomInfo.RunningTime * 60)
			{
                processTime = getRoomInfo.RunningTime * 60;
                break;
            }

            txtmp_RoomTime.text = $"{processTime / 60:00}:{processTime % 60:00} / {getRoomInfo.RunningTime:00}:{0:00}";
        }

        txtmp_RoomTime.text = $"{processTime / 60:00}:{processTime % 60:00} / {getRoomInfo.RunningTime:00}:{0:00}";
    }

    #endregion
}
