using FrameWork.UI;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lobby씬에서 만들어지는 버튼, 현재 사용 X 
/// </summary>
public class View_ChannelItem : UIBase
{
    [SerializeField] protected string ip;
    [SerializeField] protected int port, playerNumber = 0;
    protected string channelName => txtmp_channelName.text;
    protected RoomInfoRes serverInfo = new RoomInfoRes();
    protected Button btn_channel = null;
    protected TMP_Text txtmp_channelName = null;

    private Image img_iconStatus = null;
    private TMP_Text txtmp_status = null;
    protected int maxEnterCount = 60;

    private MainModuleData _mainModuleData = new MainModuleData();

    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        txtmp_channelName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_channelName));
        img_iconStatus = GetUI<Image>(nameof(img_iconStatus));
        txtmp_status = GetUI_TxtmpMasterLocalizing(nameof(txtmp_status));
    }

    protected override void Start()
    {
        base.Start();
        btn_channel = GetUI_Button(nameof(btn_channel), OnClickConnectCheck);
    }

    /// <summary>
    /// 버튼 UI 초기화
    /// </summary>
    /// <param name="info"></param>
    protected internal void Init(RoomInfoRes info)
    {
        serverInfo = info;

    }

    /// <summary>
    /// 서버 접속 되어있는 플레이어 수 표시 
    /// </summary>
    /// <param name="number"></param>
    private void ShowPlayerCounter(int number)
    {
        playerNumber = number;
        string entry = string.Empty;
        Color color = default;
        if (number >= 50) //포화
        {
            entry = "5007";
            color = Cons.Color_Red;
        }
        else if (number >= 30) //혼잡
        {
            entry = "5008";
            color = Cons.Color_Yellow;
        }
        else if (number >= 15) //보통
        {
            entry = "5009";
            color = Cons.Color_White;
        }
        else //원활
        {
            entry = "5010";
            color = Cons.Color_Green;
        }

        img_iconStatus.color = color;
        txtmp_status.color = color;
        Util.SetMasterLocalizing(txtmp_status, entry);
    }

    #endregion

    /// <summary>
    /// 버튼 클릭 후 네트워크 문제가 있으면 Popup 띄움, 네트워크 정상일 시 채널 입장 팝업 활성화 
    /// </summary>
    private void OnClickConnectCheck()
    {
        // 2회 체크
        int checkCount = 2;
        for (int i = 0; i < checkCount; i++)
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable: // 인터넷이 연결되어 있지 않을 때
                    if (checkCount == i + 1) // 한번만 실행하도록
                    {
                        PushPopup<Popup_Basic>()
                            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("80000")));
                        return;
                    }
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork: // 4g, 통신사 데이터를 사용 중일 때 
                case NetworkReachability.ReachableViaLocalAreaNetwork: // 로컬 데이터(wifi)를 사용 중일 때 확인
                    PopupOpen();
                    break;
            }
        }
    }

    /// <summary>
    /// 버튼 눌렀을 때 팝업 활성화
    /// </summary>
    protected virtual void PopupOpen()
    {
        // 인원 수 초과했을 때 
        if (playerNumber >= maxEnterCount)
        {
            MasterLocalData localData2 = new MasterLocalData(Cons.Local_Arzmeta, "5006");
            PushPopup<Popup_Basic>()
                 .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: localData2));
        }
        else
        {
            // 웹서버에서 주는 방 이름은 Ch.1
            string[] roomNumber = _mainModuleData.roomName.Split('.');
            MasterLocalData localData = new MasterLocalData("5005", roomNumber[1]);

            // UI/UX 피드백 적용한 부분
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: localData))
                .ChainPopupAction(new PopupAction(() => Util.RunCoroutine(ServerConnect())));
        }
    }

    protected virtual IEnumerator<float> ServerConnect()
    {
        Single.Scene.SetDimOn();

        yield return Timing.WaitUntilTrue(() => !SceneLogic.instance.isUILock);

        // RealtimeUtils.NewConnectServer(type, Info, Cons.Scene_World_Arzmeta, SuccessCallback, 5f); // 사용 X
    }

    //private void SuccessCallback()
    //{
    //    MainModuleData mainModuleData = (MainModuleData)RealtimeUtils.GetModuleData(Info, Cons.ModuleType_Main);
    //    LocalContentsData.curChannel = mainModuleData.roomName;
    //}

}
