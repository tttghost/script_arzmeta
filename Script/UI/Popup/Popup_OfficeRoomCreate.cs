

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using db;
using FrameWork.UI;
using MEC;
using Office;
using TMPro;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class Popup_OfficeRoomCreate : Popup_BaseRoomCreate
{

    #region 변수
    protected string roomCode;

    //go
    protected GameObject go_Reservation;
    protected GameObject go_ResvDate;
    protected GameObject go_RepeatResvContent;
    protected GameObject go_UpgradeAdvenced;

    protected GameObject go_Observer;
    protected GameObject go_ObserverTog;


    //txtmp

    //진행시간
    protected TMP_Text txtmp_PlayTimeChange;

    //예약    
    protected TMP_Text txtmp_SingleResv;

    //시작시간
    protected TMP_Text txtmp_StartTimeChange;

    //업그레이드
    protected TMP_Text txtmp_UpgradeAdvencedDesc2;

    protected TMP_InputField input_Password;
    protected TMP_InputField input_PlayerCount;
    protected TMP_InputField input_ObserverCount;


    /// <summary>
    /// formdata에 맞게 다시 캐싱
    /// </summary>
    /// <param name="office_CreateOfficeReservReq"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetOfficeFormData(Office_CreateOfficeReservReq office_CreateOfficeReservReq)
    {
        Dictionary<string, string> dummyDic = new Dictionary<string, string>();
        dummyDic.Add("modeType", office_CreateOfficeReservReq.modeType.ToString());
        dummyDic.Add("name", office_CreateOfficeReservReq.name.ToString());
        dummyDic.Add("topicType", office_CreateOfficeReservReq.topicType.ToString());
        dummyDic.Add("alarmType", office_CreateOfficeReservReq.alarmType.ToString());
        dummyDic.Add("description", office_CreateOfficeReservReq.description.ToString());
        
        //비밀번호 예외처리
        if (office_CreateOfficeReservReq.password != "") dummyDic.Add("password", office_CreateOfficeReservReq.password.ToString());
        dummyDic.Add("spaceInfoId", office_CreateOfficeReservReq.spaceInfoId.ToString());
        dummyDic.Add("runningTime", office_CreateOfficeReservReq.runningTime.ToString());
        dummyDic.Add("personnel", office_CreateOfficeReservReq.personnel.ToString());
        dummyDic.Add("observer", office_CreateOfficeReservReq.observer.ToString());
        
        //예약날짜 예외처리
        if (office_CreateOfficeReservReq.reservationAt != "") dummyDic.Add("reservationAt", office_CreateOfficeReservReq.reservationAt.ToString());
        dummyDic.Add("startTime", office_CreateOfficeReservReq.startTime.ToString());
        dummyDic.Add("repeatDay", office_CreateOfficeReservReq.repeatDay.ToString());
        dummyDic.Add("isAdvertising", office_CreateOfficeReservReq.isAdvertising.ToString());
        dummyDic.Add("isWaitingRoom", office_CreateOfficeReservReq.isWaitingRoom.ToString());
        
        //룸수정일때 썸네일 예외처리
        if (!string.IsNullOrEmpty(roomCode)) //룸코드있음 = 룸수정 -> 썸네일 유지할지 지울지 업데이트할지
        {
            int isDelete;
            if (thumbnailPath == string.Empty) //지움
            {
                isDelete = 1;
            }
            else if (thumbnailPath == Cons.EMPTY) //그대로
            {
                thumbnailPath = "";
                isDelete = 0;
            }
            else //변경
            {
                isDelete = 0;
            }
            dummyDic.Add("isDelete", isDelete.ToString());
        }

        return dummyDic;
    }



    //btn
    //비밀번호 랜덤
    protected Button btn_PasswordRandom;
    //인원 설정 버튼
    protected Button btn_SubPlayerCount;
    protected Button btn_AddPlayerCount;
    //진행시간
    protected Button btn_PlayTime;
    //단일예약
    protected Button btn_SingleResv;
    //시작시간(예약시간)
    protected Button btn_StartTime;

    //업그레이드
    protected Button btn_SubObserverCount;
    protected Button btn_AddObserverCount;
    protected Button btn_UpgradeAdvenced;

    //tog&togg
    //비밀번호
    protected TogglePlus togplus_PasswordLock;
    //공간
    protected ToggleGroup togg_Space;
    //단일/반복 예약 토글스위칭
    protected ToggleGroup togg_ResvDate;
    protected TogglePlus togplus_SingleResv;
    protected TogglePlus togplus_RepeatResv;
    protected List<TogglePlus> togplus_DayOfWeekList = new List<TogglePlus>();
    //홍보노출
    protected TogglePlus togplus_Promotion;
    //대기실
    protected TogglePlus togplus_WaitingRoom;
    //관전자
    protected TogglePlus togplus_Observer;



    //dropdown
    protected TMP_Dropdown dropdown_Topic;
    protected TMP_Dropdown dropdown_Alarm;



    //view
    protected View_OfficeSpaceSelect view_OfficeSpaceSelect;
    protected View_NumberInput view_NumberInput;


    //기타
    //public eOfficeGradeType eOfficeGradeType; //오피스등급 결제전 임시적으로 사용할 값
    //protected OfficeMode officeMode;
    public List<OfficeSpaceInfo> officeSpaceInfoList { get; private set; }
    public OfficeGradeAuthority officeGradeAuthority => Single.MasterData.dataOfficeGradeAuthority.GetData(LocalPlayerData.OfficeGradeType);

    protected bool isPromotion;
    protected bool isWaitingRoom;

    //인원 인풋
    protected int curCapacity;
    protected int minCapacity;
    protected int maxCapacity;

    protected (int hour, int min) playTime;
    protected (int hour, int min) callbackPlayTime;
    protected (int hour, int min) startTime;
    protected (int hour, int min) callbackStartTime;

    //관전자 인풋
    protected int curObserver;
    protected int minObserver;
    protected int maxObserver;

    protected int dayOfWeekTotal = 0; //반복예약 요일 총합

    protected DateTime reservationDate;
    protected DateTime callbackReservationDate;
    public int selectOfficeSpaceIdx { get; protected set; } //선택한 오피스공간 인덱스, Choose는 고른것, Select는 선택한것 즉, 골라서 선택한다, Choose를 한다음 Select를 한다.
    protected List<Item_OfficeSpace> item_OfficeSpaceList = new List<Item_OfficeSpace>();

    /// 상담실 생성
    protected Panel_Consulting panelMedicine;
    protected MakeMeetingRoomData roomData;
    protected eOpenType eOfficeOpenType;
    //protected string roomCode;
    //protected string thumbnailPath;

    public OfficeModeType modeType = OfficeModeType.Meeting;
    #endregion

  
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        //gameObject             
        go_Reservation = GetChildGObject(nameof(go_Reservation));
        go_ResvDate = GetChildGObject(nameof(go_ResvDate));
        go_UpgradeAdvenced = GetChildGObject(nameof(go_UpgradeAdvenced));
        go_Observer = GetChildGObject(nameof(go_Observer));
        go_ObserverTog = GetChildGObject(nameof(go_ObserverTog));

        //txtmp
        //타이틀
        txtmp_RoomTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomTitle));
        txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
        txtmp_RoomDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomDesc));

        //진행시간                                                                 
        txtmp_PlayTimeChange = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayTimeChange));

        //단일예약                                                                       
        txtmp_SingleResv = GetUI_TxtmpMasterLocalizing(nameof(txtmp_SingleResv));

        //시작시간                                                                 
        txtmp_StartTimeChange = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StartTimeChange));

        //업그레이드
        txtmp_UpgradeAdvencedDesc2 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_UpgradeAdvencedDesc2));

        //개설or예약버튼
        txtmp_CreateRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateRoom));


        //input
        input_Password = GetUI<TMP_InputField>(nameof(input_Password));
        input_Password.characterLimit = 10;
        input_PlayerCount = GetUI<TMP_InputField>(nameof(input_PlayerCount));
        input_ObserverCount = GetUI<TMP_InputField>(nameof(input_ObserverCount));


        //btn
        //비밀번호
        btn_PasswordRandom = GetUI_Button(nameof(btn_PasswordRandom), () => input_Password.text = Util.MakeRandomPassword());
        //인원
        btn_SubPlayerCount = GetUI_Button(nameof(btn_SubPlayerCount), () =>
        {
            if (curCapacity > minCapacity)
            {
                curCapacity--;
                input_PlayerCount.text = curCapacity.ToString();
            }
        });
        btn_AddPlayerCount = GetUI_Button(nameof(btn_AddPlayerCount), () =>
        {
            if (curCapacity < maxCapacity)
            {
                curCapacity++;
                input_PlayerCount.text = curCapacity.ToString();
            }
            else if ((eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic)
            {
                OnClick_Upgrade(eOfficeGradeType.Basic);
            }
        });

        //진행시간 : 시시:분분
        btn_PlayTime = GetUI_Button(nameof(btn_PlayTime), OnClick_PlayTime);
        //단일예약 : 년.월.일 요일
        btn_SingleResv = GetUI_Button(nameof(btn_SingleResv), OnClick_SingleResv);
        //시작시간
        btn_StartTime = GetUI_Button(nameof(btn_StartTime), OnClick_StartTime);
        //관전자인원
        btn_SubObserverCount = GetUI_Button(nameof(btn_SubObserverCount), () =>
        {
            if (curObserver > 0)
            {
                curObserver--;
                input_ObserverCount.text = curObserver.ToString();
            }
        });
        btn_AddObserverCount = GetUI_Button(nameof(btn_AddObserverCount), () =>
        {
            if (curObserver < maxObserver)
            {
                curObserver++;
                input_ObserverCount.text = curObserver.ToString();
            }
        });
        //업그레이드
        btn_UpgradeAdvenced = GetUI_Button(nameof(btn_UpgradeAdvenced), () => OnClick_Upgrade(eOfficeGradeType.Basic));






        //tog, togg
        togplus_PasswordLock = GetUI<TogglePlus>(nameof(togplus_PasswordLock)); //패스워드
        togplus_PasswordLock.SetToggleAction((b) =>
        {
            input_Password.text = !b ? "" : input_Password.text;
            input_Password.gameObject.SetActive(b);
        });
        togg_Space = GetUI<ToggleGroup>(nameof(togg_Space)); //공간

        InitToggle_ResvGroup();//예약
        InitToggle_ResvDayOfWeek();//요일

        togplus_Promotion = GetUI<TogglePlus>(nameof(togplus_Promotion));
        togplus_Promotion.SetToggleAction((b) => isPromotion = b);
        togplus_WaitingRoom = GetUI<TogglePlus>(nameof(togplus_WaitingRoom));
        togplus_WaitingRoom.SetToggleAction((b) => isWaitingRoom = b);
        togplus_Observer = GetUI<TogglePlus>(nameof(togplus_Observer));
        togplus_Observer.SetToggleAction((isOn) =>
        {
            if (!isOn)
            {
                curObserver = 0;
                input_ObserverCount.text = curObserver.ToString();
            }
            go_ObserverTog.SetActive(isOn);
        });

        //view
        view_OfficeSpaceSelect = (View_OfficeSpaceSelect)GetUI<UIBase>(Cons.View_OfficeSpaceSelect);
        view_NumberInput = (View_NumberInput)GetUI<UIBase>(Cons.View_NumberInput);

        img_ThumbnailPreview = GetUI_Img(nameof(img_ThumbnailPreview));

        //기타
        officeSpaceInfoList = new List<OfficeSpaceInfo>();

    }


    /// <summary>
    /// 오피스 수정
    /// </summary>
    /// <param name="officeReservationInfo"></param>
    public void Modify_OfficeRoom(OfficeReservationInfo officeReservationInfo)
    {
        this.modeType = (OfficeModeType)officeReservationInfo.modeType;
        this.eOfficeOpenType = eOpenType.Modify;
        roomCode = officeReservationInfo.roomCode;
        SetData_OfficeRoom(officeReservationInfo);
    }

    /// <summary>
    /// 오피스 개설, 예약
    /// </summary>
    /// <param name="eOfficeModeType"></param>
    /// <param name="eOfficeOpenType"></param>
    public void Create_OfficeRoom(OfficeModeType eOfficeModeType, eOpenType eOfficeOpenType)
    {
        var reservation = new OfficeReservationInfo(eOfficeModeType);

        this.modeType = eOfficeModeType;
        this.eOfficeOpenType = eOfficeOpenType;

        roomCode = string.Empty;

        SetData_OfficeRoom(reservation);
    }


    /// <summary>
    /// 회의, 강의 눌러서 들어올 때, 팝업 진입 시 최초 1회
    /// 회의 강의 컨퍼런스
    /// 예약개설, 즉시개설
    /// </summary>
    /// <param name="eOfficeModeType"></param>
    /// <param name="eOfficeOpenType"></param>
    private async void SetData_OfficeRoom(OfficeReservationInfo officeReservationInfo)
    {
        ChangeView("");
        InitDropDown_Topic();
        InitDropDown_Alarm();

        Util.SetMasterLocalizing(txtmp_UpgradeAdvencedDesc2, new MasterLocalData("office_mygrade", Util.GetMasterLocalizing(Single.MasterData.dataOfficeGradeType.GetData(LocalPlayerData.OfficeGradeType).name)));

        //공간 선택
        officeSpaceInfoList = Single.MasterData.GetOfficeSpaceInfoDatas(modeType); //오피스 모드에 따른 룸정보(공간) 리스트

        OfficeMode officeMode = Single.MasterData.dataOfficeMode.GetData((int)modeType);

        //타이틀
        Util.SetMasterLocalizing(txtmp_RoomTitle, new MasterLocalData("office_room_set", Util.GetMasterLocalizing(Single.MasterData.dataOfficeModeType.GetData((int)modeType).name)));
        //이름 플레이스
        input_RoomName.MasterLocalInputFieldPlaceHolder(officeMode.roomName, officeReservationInfo.nickName);
        input_RoomName.text = officeReservationInfo.roomName;

        //토픽
        dropdown_Topic.value = officeReservationInfo.topicType - 1;

        //설명 플레이스
        input_RoomDesc.MasterLocalInputFieldPlaceHolder(officeMode.roomDesc, officeReservationInfo.nickName);
        input_RoomDesc.text = officeReservationInfo.description;

        //비밀번호
        input_Password.text = officeReservationInfo.password;
        if (input_Password.text == "") //비번이 없었으면?
        {
            togplus_PasswordLock.SetToggleIsOn(false);
        }

        //인원
        //curCapacity = officeReservationInfo.personnel; //룸 선택시 인원으로 되게끔 변경 20230714김지수
        curObserver = officeReservationInfo.observer;

        //룸
        togg_Space.gameObject.Children().Destroy();

        item_OfficeSpaceList.Clear();

        // officeSpaceInfoList 를 오피스룸 표시 순서에 따라 sorting 해 주기
        officeSpaceInfoList.Sort(new OfficeSpaceInfoComparer());

        for (int i = 0; i < officeSpaceInfoList.Count; i++)
        {
            int capture = i;
            OfficeSpaceInfo officeSpaceInfo = officeSpaceInfoList[capture];
            Item_OfficeSpace item_OfficeSpace = Single.Resources.Instantiate<Item_OfficeSpace>(Cons.Path_Prefab_Item + nameof(item_OfficeSpace), togg_Space.transform);
            item_OfficeSpaceList.Add(item_OfficeSpace);
            item_OfficeSpace.SetData(togg_Space, officeSpaceInfo);
            item_OfficeSpace.btn_OfficeSpace.onClick.AddListener(() =>
            {
                Single.Sound.PlayEffect(Cons.click);
                ChangeView(Cons.View_OfficeSpaceSelect);
                view_OfficeSpaceSelect.OnClick_PreviewOfficeSpace(capture);
            });
        }
        int idx = officeSpaceInfoList.FindIndex(x => x.id == officeReservationInfo.spaceInfoId);
        OnValueChanged_Space(idx == -1 ? 0 : idx);

        //인원 여기에서...
        curCapacity = officeReservationInfo.personnel;

        //오피스 등급에 따른 패널 갱신, 유저등급에 따른 정보 노출 정도, 고급기능 등급에따른 활성화
        RefreshOfficeGrade();

        //진행시간
        callbackPlayTime = Util.Min2HourMin(officeReservationInfo.runningTime);
        Callback_PlayTime();

        //예약/시간 초기화
        switch (eOfficeOpenType)
        {
            case eOpenType.Instant:
                go_Reservation.SetActive(false);
                break;
            case eOpenType.Reservation:
            case eOpenType.Modify:
                go_Reservation.SetActive(true);
                break;
            default:
                break;
        }

        await Task.Delay(1);

        //예약토글
        int repeatDay;
        if (officeReservationInfo.repeatDay == 0)
        {
            //단일예약
            togplus_SingleResv.SetToggleIsOn(true, false);
            callbackReservationDate = Convert.ToDateTime(officeReservationInfo.reservationAt);

            togplus_RepeatResv.SetToggleIsOn(false, false);
            repeatDay = 1 << (int)DateTime.Now.DayOfWeek;
        }
        else
        {
            togplus_SingleResv.SetToggleIsOn(false, false);
            callbackReservationDate = DateTime.Today;

            //반복예약
            togplus_RepeatResv.SetToggleIsOn(true, false);
            repeatDay = officeReservationInfo.repeatDay;
        }
        Callback_SingleResv();
        SetDayOfWeek(repeatDay);

        //시작시간
        callbackStartTime = Util.Min2HourMin(officeReservationInfo.startTime);
        Callback_StartTime();

        //알림
        dropdown_Alarm.value = officeReservationInfo.alarmType;



        #region 썸네일		

        thumbnailPath = officeReservationInfo.thumbnail;
        Sprite thumbnail = await Util.UtilOffice.GetThumbnail_Office(officeReservationInfo.roomCode, officeReservationInfo.thumbnail);

        if (thumbnail != null)
        {
            thumbnailPath = Cons.EMPTY; //같은 파일인지 체크 용, WebManager의 Office_CreateOfficeReservReq에서 비교함
            spriteThumbnail = thumbnail;
        }
        else
        {
            OnClick_ThumbnailReset();
        }
        #endregion







        //홍보노출
        togplus_Promotion.SetToggleIsOn(Convert.ToBoolean(officeReservationInfo.isAdvertising));

        //대기실
        togplus_WaitingRoom.SetToggleIsOn(Convert.ToBoolean(officeReservationInfo.isWaitingRoom));

        //관전자
        go_Observer.SetActive(modeType == OfficeModeType.Lecture);

        togplus_Observer.SetToggleIsOn(curObserver > 0);

        //삭제 버튼
        btn_RemoveRoom.gameObject.SetActive(roomCode != string.Empty);

        //개설or예약 버튼
        string masterId = "";


        switch (eOfficeOpenType)
        {
            case eOpenType.Instant:
                masterId = "office_instant_creation";
                break;
            case eOpenType.Reservation:
                masterId = "office_reservation_creation";
                break;
            case eOpenType.Modify:
                masterId = "office_reservation_modify";
                break;
        }

        Util.SetMasterLocalizing(txtmp_CreateRoom, new MasterLocalData(masterId));

        Util.RefreshScrollView(gameObject, "sview_OfficeRoomCreate");
        Util.RefreshLayout(gameObject, "go_Content");
        Util.RefreshLayout(gameObject, "go_Content");
    }



    /// <summary>
    /// 공간 선택 토글
    /// </summary>
    /// <param name="chooseOfficeSpaceIdx">고른 오피스공간 인덱스</param>
    public async void OnValueChanged_Space(int chooseOfficeSpaceIdx)
    {
        selectOfficeSpaceIdx = chooseOfficeSpaceIdx;

        //오피스 공간정보 추출
        OfficeSpaceInfo officeRoomInfo = officeSpaceInfoList[selectOfficeSpaceIdx];

        await UniTask.Delay(1);
        //공간선택
        item_OfficeSpaceList[selectOfficeSpaceIdx].togglePlus.SetToggleIsOn(true);


        //인원
        curCapacity = (((eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic) && officeRoomInfo.defaultCapacity > 6) ? 6 : officeRoomInfo.defaultCapacity;

        minCapacity = officeRoomInfo.minCapacity;
        maxCapacity = (eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic ? Mathf.Min(officeGradeAuthority.capacityLimit, officeRoomInfo.maxCapacity) : officeRoomInfo.maxCapacity; //일반등급 : 일반등급 인원 사용, 그외 : 룸인원 사용
                                                                                                                                                                                                            //관전자 
        minObserver = 0;
        maxObserver = officeRoomInfo.maxObserver;

        //인원
        input_PlayerCount.onEndEdit.RemoveAllListeners();
        input_PlayerCount.onEndEdit.AddListener(((str) =>
        {
            if (int.Parse(str) > maxCapacity && (eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic)
            {
                OnClick_Upgrade(eOfficeGradeType.Basic);
            }
            curCapacity = Mathf.Clamp(int.Parse(str), minCapacity, maxCapacity);
            input_PlayerCount.text = curCapacity.ToString();

        }));
        input_PlayerCount.onEndEdit.Invoke(curCapacity.ToString());

        //관전자 
        input_ObserverCount.onEndEdit.RemoveAllListeners();
        input_ObserverCount.onEndEdit.AddListener((str) =>
        {
            curObserver = Mathf.Clamp(int.Parse(str), minObserver, maxObserver);
            input_ObserverCount.text = curObserver.ToString();
        });
        input_ObserverCount.onEndEdit.Invoke(curObserver.ToString());
    }

    /// <summary>
    /// 오피스등급 갱신
    /// </summary>
    public void RefreshOfficeGrade()
    {
        switch ((eOfficeGradeType)LocalPlayerData.OfficeGradeType)
        {
            case eOfficeGradeType.Basic:
                go_UpgradeAdvenced.SetActive(true);
                maxCapacity = officeGradeAuthority.capacityLimit;
                break;
            case eOfficeGradeType.Pro:
                go_UpgradeAdvenced.SetActive(false);
                maxCapacity = Mathf.Max(officeSpaceInfoList[selectOfficeSpaceIdx].maxCapacity, 6);
                break;
            default:
                break;
        }
    }




    /// <summary>
    /// 예약토글 셋팅
    /// </summary>
    private void InitToggle_ResvGroup()
    {
        togg_ResvDate = go_ResvDate.transform.GetComponent<ToggleGroup>();

        togplus_SingleResv = GetUI<TogglePlus>(nameof(togplus_SingleResv));
        togplus_SingleResv.SetToggleGroup(togg_ResvDate);
        togplus_SingleResv.SetToggleAction((b) =>
        {
            btn_SingleResv.GetComponent<CanvasGroup>().interactable = b;
            Util.RefreshLayout(gameObject, nameof(go_Reservation));
        });

        togplus_RepeatResv = GetUI<TogglePlus>(nameof(togplus_RepeatResv));
        togplus_RepeatResv.SetToggleGroup(togg_ResvDate);
        togplus_RepeatResv.SetToggleAction((b) =>
        {
            CanvasGroup canvasGroup = go_RepeatResvContent.GetComponent<CanvasGroup>();
            canvasGroup.interactable = b;
            canvasGroup.alpha = b ? 1f : 0.5f;
            Util.RefreshLayout(gameObject, nameof(go_Reservation));
        });
    }

    /// <summary>
    /// 반복예약 요일 토글 셋팅
    /// </summary>
    private void InitToggle_ResvDayOfWeek()
    {
        go_RepeatResvContent = GetChildGObject(nameof(go_RepeatResvContent));
        togplus_DayOfWeekList.Clear();
        for (int i = 0; i < Util.EnumLength<DayOfWeek>(); i++)
        {
            int capture = i;
            GameObject item_DayOfWeek = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_Item + nameof(item_DayOfWeek), go_RepeatResvContent.transform); //프리팹생성
            TogglePlus togplus_DayOfWeek = item_DayOfWeek.GetComponentInChildren<TogglePlus>();
            togplus_DayOfWeek.SetToggleAction(
                () => dayOfWeekTotal += (1 << capture),
                () =>
                {
                    dayOfWeekTotal -= (1 << capture);
                    if (dayOfWeekTotal == 0)
                    {
                        togplus_DayOfWeek.SetToggleIsOn(true);
                    }
                });

            togplus_DayOfWeekList.Add(togplus_DayOfWeek);

            TMP_Text txtmp_DayOfWeek = Util.Search<TMP_Text>(item_DayOfWeek.gameObject, nameof(txtmp_DayOfWeek));
            txtmp_DayOfWeek.text = Util.GetDayOfWeek((DayOfWeek)i);
        }
    }

    /// <summary>
    /// 반복예약 요일 토글 초기화
    /// </summary>
    private void SetDayOfWeek(int inputDayOfWeek)
    {
        for (int i = 0; i < togplus_DayOfWeekList.Count; i++)
        {
            TogglePlus item_DayOfWeek = togplus_DayOfWeekList[i];
            item_DayOfWeek.SetToggleIsOn(false);
        }
        dayOfWeekTotal = 0;

        for (int i = 0; i < togplus_DayOfWeekList.Count; i++)
        {
            TogglePlus item_DayOfWeek = togplus_DayOfWeekList[i];
            int check = inputDayOfWeek & (1 << i);
            if (check != 0)
            {
                item_DayOfWeek.SetToggleIsOn(true); //오늘요일 토글
            }
        }

    }





    /// <summary>
    /// 업그레이드
    /// </summary>
    /// <param name="eOfficeGradeType"></param>
    private void OnClick_Upgrade(eOfficeGradeType eOfficeGradeType)
    {
        GetPopup<Popup_OfficeGradeUpgrade>().SetData(eOfficeGradeType);
        PushPopup<Popup_OfficeGradeUpgrade>();
    }






    /// <summary>
    /// 단일예약 버튼 : 년.월.일 요일
    /// </summary>
    private void OnClick_SingleResv()
    {
        ChangeView(Cons.View_NumberInput);
        view_NumberInput.SetData(
            Callback_SingleResv,

            new MinNumMax(reservationDate.Year, DateTime.Today.Year, DateTime.Today.Year + 10, "0000",
            (year) =>
            {
                int resvDay = callbackReservationDate.Day; //예약 day
                int lastDay = DateTime.DaysInMonth(year, callbackReservationDate.Month); //갱신한 월의 마지막 day
                view_NumberInput.item_NumberInputList[2].SetData( //일 최대치 적용
                        new MinNumMax(Mathf.Min(resvDay, lastDay), 1, lastDay, "00",
                        (day) => callbackReservationDate = Util.ChangeTime(callbackReservationDate, Day: day)));

                callbackReservationDate = Util.ChangeTime(callbackReservationDate, Year: year);
            }),

            new MinNumMax(reservationDate.Month, 1, 12, "00",
            (month) =>
            {
                int resvDay = callbackReservationDate.Day; //예약 day
                int lastDay = DateTime.DaysInMonth(callbackReservationDate.Year, month); //갱신한 월의 마지막 day
                view_NumberInput.item_NumberInputList[2].SetData( //일 최대치 적용
                        new MinNumMax(Mathf.Min(resvDay, lastDay), 1, lastDay, "00",
                        (day) => callbackReservationDate = Util.ChangeTime(callbackReservationDate, Day: day)));

                callbackReservationDate = Util.ChangeTime(callbackReservationDate, Month: month, Day: Mathf.Min(resvDay, lastDay)); //월 갱신
            }),

            new MinNumMax(reservationDate.Day, 1, DateTime.DaysInMonth(reservationDate.Year, reservationDate.Month), "00",
            (day) =>
            {
                callbackReservationDate = Util.ChangeTime(callbackReservationDate, Day: day);
            })
            );
    }

    /// <summary>
    /// 단일예약 콜백 : 년.월.일 요일
    /// </summary>
    private void Callback_SingleResv()
    {
        //callbackReservationDate
        //SetSingleResv(reservationDate);
        reservationDate = callbackReservationDate;
        txtmp_SingleResv.text = $"{this.reservationDate.ToString("yyyy.MM.dd")} {Util.GetDayOfWeek(this.reservationDate.DayOfWeek)}";
    }






    /// <summary>
    /// 진행시간 버튼 : 시시:분분 분, 회의를 진행하는 시간
    /// </summary>
    private void OnClick_PlayTime()
    {
        ChangeView(Cons.View_NumberInput);
        view_NumberInput.SetData(
            Callback_PlayTime,
            new MinNumMax(playTime.hour, 0, 99, "00", (hour) =>
            {
                callbackPlayTime.hour = hour;
            }),
            new MinNumMax(playTime.min, 0, 59, "00", (min) =>
            {
                callbackPlayTime.min = min;
            }));
    }

    /// <summary>
    /// 진행시간 콜백
    /// </summary>
    private void Callback_PlayTime()
    {
        playTime = callbackPlayTime;
        if (Util.HourMin2Min(playTime.hour, playTime.min) > officeGradeAuthority.timeLimit)
        {
            playTime = Util.Min2HourMin(officeGradeAuthority.timeLimit);
            if ((eOfficeGradeType)LocalPlayerData.OfficeGradeType == eOfficeGradeType.Basic)
            {
                OnClick_Upgrade(eOfficeGradeType.Basic);
            }
        }
        Util.SetMasterLocalizing(txtmp_PlayTimeChange, new MasterLocalData("office_room_time_set", playTime.hour.ToString("00"), playTime.min.ToString("00")));
    }







    /// <summary>
    /// 시작시간 버튼 : 시시:분분 오전 예약시 사용
    /// </summary>
    private void OnClick_StartTime()
    {
        ChangeView(Cons.View_NumberInput);
        view_NumberInput.SetData(
            Callback_StartTime,
            new MinNumMax(startTime.hour, 0, 23, "00", (hour) =>
            {
                callbackStartTime.hour = hour;
            }),
            new MinNumMax(startTime.min, 0, 59, "00", (min) =>
            {
                callbackStartTime.min = min;
            }));
    }

    /// <summary>
    /// 시작시간 콜백
    /// </summary>
    private void Callback_StartTime()
    {
        startTime = callbackStartTime;
        string 오전오후 = (startTime.hour / 12) == 0 ? "common_am_set" : "common_pm_set";
        Util.SetMasterLocalizing(txtmp_StartTimeChange, new MasterLocalData(오전오후, (startTime.hour % 12).ToString("00"), startTime.min.ToString("00")));
    }





    /// <summary>
    /// 오피스 토픽 드롭다운 초기화
    /// </summary>
    private void InitDropDown_Topic()
    {
        //OfficeTopicType
        dropdown_Topic = GetUI<TMP_Dropdown>(nameof(dropdown_Topic));
        dropdown_Topic.options.Clear();
        List<OfficeTopicType> officeTopicTypeList = Single.MasterData.dataOfficeTopicType.GetList().OrderBy(x => x.type).ToList();

        for (int i = 0; i < officeTopicTypeList.Count; i++)
        {
            OfficeTopicType officeTopicType = officeTopicTypeList[i];
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(Util.GetMasterLocalizing(officeTopicType.name));
            dropdown_Topic.options.Add(optionData);
        }
        dropdown_Topic.onValueChanged.Invoke(0);
        dropdown_Topic.value = 0;
    }

    /// <summary>
    /// 오피스 알람 드롭다운 초기화
    /// </summary>
    private void InitDropDown_Alarm()
    {
        dropdown_Alarm = GetUI<TMP_Dropdown>(nameof(dropdown_Alarm));
        dropdown_Alarm.options.Clear();
        List<OfficeAlarmType> officeAlarmTypeList = Single.MasterData.dataOfficeAlarmType.GetList();

        for (int i = 0; i < officeAlarmTypeList.Count; i++)
        {
            OfficeAlarmType officeAlarmType = officeAlarmTypeList[i];
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(Util.GetMasterLocalizing(officeAlarmType.name));
            dropdown_Alarm.options.Add(optionData);

        }
        dropdown_Alarm.onValueChanged.Invoke(0);
        dropdown_Alarm.value = 0;
    }






    /// <summary>
    /// 방 만들기 요청
    /// </summary>
    protected override void OnClick_CreateRoom()
    {
        base.OnClick_CreateRoom();

        switch (eOfficeOpenType)
        {
            case eOpenType.Instant:
                {
                    if (this.modeType == OfficeModeType.Consulting)
                    {
                        CreateRoom_Consulting();
                    }
                    else
                    {
                        CreateRoom();
                    }
                }
                break;
            case eOpenType.Reservation:
            case eOpenType.Modify:
                {
                    ReservOrModifyRoom();
                }
                break;
            default:
                break;
        }
    }





    /// <summary>
    /// 오피스 나의예약 취소
    /// </summary>
    protected override void OnClick_RemoveRoom()
    {
        base.OnClick_RemoveRoom();
        Single.Web.office.Office_CancelReservation(roomCode, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
            {
                Co_RemoveRoom().RunCoroutine();

                ArzMetaReservationController.Instance.Remove();
            }
        });
    }

    IEnumerator<float> Co_RemoveRoom()
    {
        Single.Scene.SetDimOn();

        SceneLogic.instance.PopPopup();

        if (SceneLogic.instance._stackPopups.Count > 0)
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PopPopup();
        }

        yield return Timing.WaitForSeconds(.4f);

        Single.Scene.SetDimOff();
    }







    /// <summary>
    /// 오피스예약리퀘스트 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    private Office_CreateOfficeReservReq GetOfficeReservReqData()
    {
        Office_CreateOfficeReservReq Office_ReservData = new Office_CreateOfficeReservReq();

        //추출데이터
        Office_ReservData.modeType = (int)modeType;//공간이름,설명,토픽,패스워드
        Office_ReservData.name = input_RoomName.text != "" ? input_RoomName.text : input_RoomName.placeholder.GetComponent<TMP_Text>().text;//string input_RoomName

        Office_ReservData.topicType = dropdown_Topic.value + 1;
        Office_ReservData.description = input_RoomDesc.text != "" ? input_RoomDesc.text : input_RoomDesc.placeholder.GetComponent<TMP_Text>().text;//string input_RoomDesc

        //Office_ReservData.password = !togplus_PasswordLock.GetToggleIsOn() || input_Password.text == string.Empty ? "" : input_Password.text;//string input_Password
        Office_ReservData.password = input_Password.text;//string input_Password

        Office_ReservData.spaceInfoId = officeSpaceInfoList[selectOfficeSpaceIdx].id; //string sceneName -> 공간이름 int RoomInfoId

        //플레이어수,시작시간
        Office_ReservData.personnel = int.Parse(input_PlayerCount.text); //int input_PlayerCount personnel
        Office_ReservData.runningTime = Util.HourMin2Min(playTime.hour, playTime.min); //int txtmp_PlayTimeChange runningTime

        //단일or반복(실시간에선 안씀)
        //string txtmp_SingleResv(시작날짜) "2023-02-03" or  ""
        //Office_ReservData.reservationAt = "";
        Office_ReservData.repeatDay = 0;
        if (togplus_SingleResv.GetToggleIsOn())
        {
            Office_ReservData.reservationAt = reservationDate.ToString("yyyy-MM-dd");
        }
        else
        {
            Office_ReservData.reservationAt = reservationDate.AddYears(100).ToString("yyyy-MM-dd");
            Office_ReservData.repeatDay = dayOfWeekTotal;
        }
        Office_ReservData.startTime = Util.HourMin2Min(startTime.hour, startTime.min);  //int txtmp_StartTimeChange(시작시간)
        Office_ReservData.alarmType = Single.MasterData.dataOfficeAlarmType.GetList()[dropdown_Alarm.value].type;   //int dropdown_Alarm
                                                                                                                    //고급기능
        Office_ReservData.isAdvertising = Convert.ToInt32(isPromotion);  //int promotion
        Office_ReservData.isWaitingRoom = Convert.ToInt32(isWaitingRoom);  //int waitroom -> 클라쪽에선 받을 이유가 없을듯..? 실제 들어갈때 실시간에서 Callback로 확인
        Office_ReservData.observer = int.Parse(input_ObserverCount.text); //int observer

        return Office_ReservData;
    }

    /// <summary>
    /// 오피스썸네일 저장
    /// </summary>
    /// <param name="roomCode"></param>
    /// <param name="thumbnail"></param>
    private async void SaveOfficeThumbnail(string roomCode, string thumbnail)
    {
        #region 썸네일 저장
        if (string.IsNullOrEmpty(thumbnail))
        {
            return;
        }
        string commonPath = Path.Combine("office", roomCode, thumbnail);
        string remotePath = Path.Combine(Single.Web.StorageUrl, commonPath);
        string localPath = Path.Combine(Application.persistentDataPath, commonPath);
        if (!string.IsNullOrEmpty(roomCode)) //수정
        {
            if (!File.Exists(localPath)) //기존파일삭제
            {
                Util.DeleteFiles(new FileInfo(localPath).DirectoryName);
            }
        }
        //무조껀 스토리지에서 이미지 받기
        Texture2D tex = await Util.Co_LoadRemoteAsyncTex(remotePath);
        Util.Tex2Image(localPath, tex);
        #endregion
    }

    /// <summary>
    /// 오피스예약/수정 리퀘스트
    /// </summary>
    protected override void ReservOrModifyRoom()
    {
        base.ReservOrModifyRoom();
        Single.Scene.SetDimOn();

        Office_CreateOfficeReservReq office_CreateOfficeReservReq = GetOfficeReservReqData();
        Dictionary<string, string> dummyDic = GetOfficeFormData(office_CreateOfficeReservReq);

        Single.Web.office.Office_CreateOfficeReservReq(dummyDic, thumbnailPath, roomCode, tempTex, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
            {
                SaveOfficeThumbnail(res.memberReservationInfo.roomCode, res.memberReservationInfo.thumbnail);


                if (roomCode != string.Empty) //룸 예약정보 변경
                {
                    ArzMetaReservationController.Instance.Modify(res.memberReservationInfo);

                    SceneLogic.instance.PopPopup();
                    //SceneLogic.instance.Back();
                }
                else //룸 예약생성 (입장하진 않음)
                {
                    SceneLogic.instance.PopPopup();
                    SceneLogic.instance.isUILock = false;
                    string desc = $"{Util.GetMasterLocalizing("office_reception_myreservation_add")}\n{Util.GetMasterLocalizing("office_confirm_move_myreservation")}";

                    PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData(desc)))
                    .ChainPopupAction(new PopupAction(() => Timing.RunCoroutine(ChangeView())));
                }

                Single.Scene.SetDimOff();
            }
        });
    }

    IEnumerator<float> ChangeView()
    {
        Single.Scene.SetDimOn();

        yield return Timing.WaitForSeconds(.5f);

        var panel = SceneLogic.instance.GetPanel<Panel_Office>();

        panel.ChangeView(nameof(View_Office_Reservation), Cons.MASTER_OFFICE_RESERVATION);

        panel.GetView<View_Office_Reservation>().ShowContent(Define.RESERVATION, true);

        Single.Scene.SetDimOff();
    }
}


#region OfficeSpaceInfo 를 sorting 하기 위한 IComparer 인터페이스
class OfficeSpaceInfoComparer : IComparer<OfficeSpaceInfo>
{
    public int Compare(OfficeSpaceInfo A, OfficeSpaceInfo B)
    {
        return A.exposureOrder.CompareTo(B.exposureOrder);
    }
}
#endregion
