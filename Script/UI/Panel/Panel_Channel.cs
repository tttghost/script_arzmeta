using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UI;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

/// <summary>
/// 채널 선택 패널 사용 안함 
/// </summary>
public class Panel_Channel : PanelBase
{
    public List<RoomInfoRes> infos = new List<RoomInfoRes>();

    private TMP_Text txtmp_NickName = null;

    private List<View_ChannelItem> view_channels = new List<View_ChannelItem>();
    private GameObject go_Content = null;

    private AvatarPartsController selector;
    private EventTrigger eventTrigger;
    private Animator anim_Avatar;

    private int preRandom = 0;
    private IEnumerator SetCoroutine = null;

    [SerializeField, Range(0, 50)] private float waitTime = 30f; // 시간초 조절할 수 있도록 

    public string type = "arzmeta";
    public bool isCloud;

    #region 초기화

    /// <summary>
    /// Awake에서 오브젝트 찾고 방 정보 얻음
    /// </summary>
    protected override void SetMemberUI()
    {
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("5002"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sub", new MasterLocalData("5003"));
        txtmp_NickName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_NickName));

        // Object
        go_Content = GetChildGObject(nameof(go_Content));

        // Button
        GetUI_Button("btn_Back", () =>
        {
            SetCloseEndCallback(() => anim_Avatar.Rebind());
            SceneLogic.instance.Back();
        });

        GameObject oriAvatarTr = GameObject.Find("go_AvatarView");
        selector = oriAvatarTr.GetComponent<AvatarPartsController>();
        anim_Avatar = Util.Search<Animator>(oriAvatarTr, "AvatarParts");

        eventTrigger = GetChildGObject("go_AvatarView2").GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry entry_PointerClick = new EventTrigger.Entry();
            entry_PointerClick.eventID = EventTriggerType.PointerClick;
            entry_PointerClick.callback.AddListener((data) =>
            {
                Single.Sound.PlayEffect(Cons.click);
                OnPointerClick((PointerEventData)data);
            });

            eventTrigger.triggers.Add(entry_PointerClick);
        }
    }
    #endregion

    //#region 네트워크 부하 테스트 씬 이동 기능
    //private IEnumerator TestScene()
    //{
    //    Single.Scene.SetDimOn(2f);

    //    eSessionType type = eSessionType.MAIN;

    //    //CONNECT 시도
    //    Task task = FrontisNetwork.Connect(type, "192.168.0.47", 7777);

    //    yield return new WaitUntil(() =>
    //        Single.RealTime.GetIsConnectState(type) != eServerConnect.CONNECTING);

    //    eServerConnect getIsConnectedState = Single.RealTime.GetIsConnectState(type);

    //    List<string> modules;
    //    switch (getIsConnectedState)
    //    {
    //        case eServerConnect.CONNECTED:
    //        {
    //            modules = new List<string>();
    //            modules.Add("Base");

    //            //ENTER 후 REGISTER 시도
    //            Single.RealTime.CreateModuleData(type,modules);

    //            // 중복로그인 패킷 보냄
    //            FrontisNetwork.connections[type].SendCheckDuplicate("test");

    //            //REGISTER 완료 될 때까지 대기
    //            yield return new WaitUntil(() => Single.RealTime.GetIsModuleReady(type));

    //            SceneManager.LoadScene("804_Scene_NetworkTest");
    //        }
    //            break;
    //    }
    //}
    //#endregion

    /// <summary>
    /// 로비 아바타 터치 시 랜덤 애니메이션 실행
    /// </summary>
    /// <param name="data"></param>
    void OnPointerClick(PointerEventData data)
    {
        int random = Random.Range(1, 4);
        if (random == preRandom)
        {
            OnPointerClick(null);
            return;
        }

        preRandom = random;
        string state = "Emote0" + random;
        if (anim_Avatar != null)
        {
            //anim_Avatar.Rebind();
            //anim_Avatar.Play(state, -1, 0);
            if (!anim_Avatar.IsInTransition(0)) anim_Avatar.CrossFade(state, 0.1f);

        }
    }


    /// <summary>
    /// 활성화 시 Request 요청 및 Error 처리 
    /// </summary>
    private void OnEnable()
    {
        if (txtmp_NickName != null)  Util.SetMasterLocalizing(txtmp_NickName, LocalPlayerData.NickName);
        if (selector != null) selector.SetAvatarParts(LocalPlayerData.AvatarInfo);

        // 처음 활성화 할 때 버튼 생성 후 임시 데이터 넣어줘야됨
        if (view_channels.Count == 0)
        {
            MakeChannelButton(); // 버튼 생성 
            StartCoroutine(InitButton()); // 웹서버한테 정보 요청 전에 미리 데이터 넣기, 데이터 요청 실패 후 데이터를 넣으면 버튼 데이터 없이 보이다가 넣어짐
        }

        StartCoroutine(CorNetworkChecking());

    }

    protected override void Start()
    {
        base.Start();

        #region 부하 테스트 씬 이동 버튼 초기화

        //GetUI_Button("btn_TestScene", () => { StartCoroutine(TestScene()); });

        #endregion
    }

    // 버튼 생성 
    private void MakeChannelButton()
    {
        view_channels.Clear();
        // 버튼 만들기
        for (int i = 0; i < 8; i++)
        {
            View_ChannelItem viewChannelItem = Single.Resources.Instantiate<View_ChannelItem>(Cons.Path_Prefab_UI_View + "View_ChannelItem", go_Content.transform);
            view_channels.Add(viewChannelItem);
        }
    }

    /// <summary>
    ///  버튼 초기화 
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitButton()
    {
        yield return Cons.Time_WaitForEndOfFrame;

        TempButtonData();

        for (int i = 0; i < view_channels.Count; i++)
        {
            view_channels[i].Init(infos[i]);
            view_channels[i].gameObject.SetActive(true);
        }
    }

    // 임시로 데이터 넣어주고 버튼
    private void TempButtonData()
    {
        infos.Clear();
        for (int i = 0; i < 8; i++)
        {
            RoomInfoRes info = new RoomInfoRes();
            //info.modules = new List<ModuleData>();

            BaseModuleData baseModuleData = new BaseModuleData();
            baseModuleData.type = "Base";

            MainModuleData mainModuleData = new MainModuleData();
            mainModuleData.type = "Main";
            mainModuleData.roomName = "Ch." + (i + 1);
            mainModuleData.currentPlayerNumber = 0;
            mainModuleData.maxPlayerNumber = 60;

            ChatModuleData chatModuleData = new ChatModuleData();
            chatModuleData.type = "Chat";

            //info.modules.Add(mainModuleData);
            //info.modules.Add(baseModuleData);
            //info.modules.Add(chatModuleData);

            //info.RoomId = info.RoomId;

            infos.Add(info);
        }
    }

    /// <summary>
    /// 30초 동안 3초마다 네트워크 확인
    /// 네트워크 오류가 있다면 30초동안 확인, 30초가 지나면 로그인 화면으로 이동
    /// </summary>
    /// <returns></returns>
    IEnumerator CorNetworkChecking()
    {
        Single.Scene.SetDimOn(2f);
        while (true)
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable: // 인터넷이 연결되어 있지 않을 때
                    if (waitTime <= 0)
                    {
                        Single.Scene.SetDimOff();
                        PushPopup<Popup_Basic>()
                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("80000")))
                            .ChainPopupAction(new PopupAction(() =>
                            {
                                Single.Scene.LoadScene(SceneName.Scene_Base_Title);
                                LocalPlayerData.ResetData();
                            }));

                        yield break; // 코루틴 중지 break; 
                    }
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork: // 4g, 통신사 데이터를 사용 중일 때
                case NetworkReachability.ReachableViaLocalAreaNetwork: // 로컬 데이터(wifi)를 사용 중일 때 확인
                    RequestMainServer();
                    yield break;
            }


            yield return Cons.Time_Sec_3;
            waitTime -= 3;

            yield return null;

        }
    }

    /// <summary>
    /// 메인서버 데이터 요청(클라우드/내부)
    /// </summary>
    private void RequestMainServer()
    {
        //RealtimeUtils.GetServerInfo(type, 
        //(serverInfos) =>
        //{ 
        //    Util.RunCoroutine(UIReset(serverInfos));
        //}, 
        //ErrorHandler,
        //_isCloud : isCloud);
    }

    /// <summary>
    /// 웹서버로 받은 정보를 버튼에 초기화 
    /// </summary>
    /// <param name="infos"></param>
    public IEnumerator<float> UIReset(List<RoomInfoRes> infos)
    {
        Single.Scene.SetDimOff();

        yield return Timing.WaitForOneFrame;

        this.infos.Clear();

        for (int i = 0; i < infos.Count; i++)
        {
            //if (RealtimeUtils.GetModuleData(infos[i], "Main") != null)
            //{
            //    this.infos.Add(infos[i]);
            //}
        }

        this.infos = this.infos.OrderBy(x => x.roomId).ToList();

        if (this.infos.Count > 0)
        {
            for (int i = 0; i < view_channels.Count; i++)
            {
                view_channels[i].Init(this.infos[i]);
            }
        }
        else
        {
            Debug.Log("데이터가 없음");
        }

    }

}