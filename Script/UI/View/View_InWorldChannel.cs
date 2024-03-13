using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UI;
using MEC;
using TMPro;
using UnityEngine;


public class View_InWorldChannel : UIBase
{
    public List<RoomInfoRes> infos = new List<RoomInfoRes>();
    public bool isCloud = true;
    public int dataCount = 4; // 마스터데이터로 받아오는게 좋아보임
    public string curChannel;


    private List<View_InWorldChannelItem> viewItems = new List<View_InWorldChannelItem>();
    private GameObject go_Content = null;
    private TMP_Text txtmp_curChannel = null;
    private string serverType = "arzmeta";

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_Content = GetChildGObject("go_Content");
        
        GetUI_TxtmpMasterLocalizing("txtmp_title", new MasterLocalData("9300"));
        txtmp_curChannel = GetUI_TxtmpMasterLocalizing(nameof(txtmp_curChannel));
        
        viewItems.Clear();
    }


    protected override void OnEnable()
    {
        curChannel = LocalContentsData.channel;
        Util.SetMasterLocalizing(txtmp_curChannel, new MasterLocalData("9301", curChannel));

        if (viewItems.Count == 0)
        {
            MakeChannelButton();
            Util.RunCoroutine(SetInitButtonData());
        }
        
        GetServer(( infos ) =>
        {
            Util.RunCoroutine(UpdateChannelButton(infos));
        }, ErrorCallback);
        

    }
    
    protected override void OnDisable()
    {
        Util.KillCoroutine("RequestServerData");
    }

    private void GetServer( Action<List<RoomInfoRes>> callback, Action<DefaultPacketRes> errorCallback = null )
    {
        //RealtimeUtils.GetServerInfo(serverType, callback, errorCallback, _isCloud: isCloud);
    }


    /// <summary>
    /// 버튼 생성
    /// </summary>
    private void MakeChannelButton()
    {
        viewItems.Clear();
        for (int i = 0; i < dataCount; i++)
        {
            View_InWorldChannelItem viewInWorld = Single.Resources.Instantiate<View_InWorldChannelItem>(Cons.Path_Prefab_UI_View + "View_InWorldChanneItem", go_Content.transform);
            viewInWorld.viewInWorldChannel = this;
            viewItems.Add(viewInWorld);
        }
    }
    
    private IEnumerator<float> SetInitButtonData()
    {
        yield return Timing.WaitForOneFrame;
        
        InitButtonData();
        
        for (int i = 0; i < viewItems.Count; i++)
        {
            viewItems[i].Init(infos[i]);
            viewItems[i].gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void InitButtonData()
    {
        infos.Clear();
        for (int i = 0; i < dataCount; i++)
        {
            RoomInfoRes info = new RoomInfoRes();
            //info.modules = new List<ModuleData>();
            
            BaseModuleData baseModuleData = new BaseModuleData();
            baseModuleData.type = "Base";

            MainModuleData mainModuleData = new MainModuleData();
            mainModuleData.type = "Main";
            mainModuleData.roomName = "Ch."+(i + 1);
            mainModuleData.currentPlayerNumber = 0;
            mainModuleData.maxPlayerNumber = 60;

            ChatModuleData chatModuleData = new ChatModuleData();
            chatModuleData.type = "Chat";

            //info.modules.Add(mainModuleData);
            //info.modules.Add(baseModuleData);
            //info.modules.Add(chatModuleData);
            
            info.roomId = i.ToString();
            
            infos.Add(info);
        }
    }

    
    private void ErrorCallback(DefaultPacketRes res)
    {
        Debug.Log(res.errorMessage);
        
        Debug.Log("View_Channel 웹서버 데이터 받아오기 실패");

        PushPopup<Popup_Basic>()
            .ChainPopupData( new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("80000")));

        Util.RunCoroutine(RequestServerData(), "RequestServerData");
    }

    /// <summary>
    /// 30초 동안 3초마다 서버 정보 Request 날림
    /// 30초 이후 정보를 받지 못하면 요청 중지
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> RequestServerData()
    {
        float limitTime = 30;

        yield return Timing.WaitForOneFrame;
        while (true)
        {
            // Debug.Log(limitTime);
            if (limitTime <= 0)
            {
                yield break;
            }
            
            yield return Timing.WaitForSeconds(3f);
            limitTime -= 3;
            GetServer((infos)=> {
                Util.RunCoroutine(UpdateChannelButton(infos));
            });

            yield return Timing.WaitForOneFrame;
            
        }
    }
    
    /// <summary>
    /// 요청 후 버튼 상태 업데이트 
    /// </summary>
    public void RepeatingInfo()
    {
        int checkCount = 2;
        for (int i = 0; i < checkCount; i++)
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable: // 인터넷이 연결되어 있지 않을 때
                    if (checkCount == i + 1)
                    {
                        PushPopup<Popup_Basic>()
                            .ChainPopupData( new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm,masterLocalDesc: new MasterLocalData("80000")));
                    }
                    break;
                
                case NetworkReachability.ReachableViaCarrierDataNetwork: // 4g, 통신사 데이터를 사용 중일 때 
                case NetworkReachability.ReachableViaLocalAreaNetwork: // 로컬 데이터(wifi)를 사용 중일 때 확인
                    GetServer((infos) => { Util.RunCoroutine(UpdateChannelButton(infos)); });
                    break;
            }
        }
    }
    
    /// <summary>
    /// 생성되어있는 버튼 데이터 업데이트 
    /// </summary>
    /// <param name="infos"></param>
    private IEnumerator<float> UpdateChannelButton(List<RoomInfoRes> infos)
    {
        Util.KillCoroutine("RequestServerData");

        yield return Timing.WaitForOneFrame;

        this.infos.Clear();
        this.infos = infos.OrderBy(x => x.roomId).ToList();

        for (int i = 0; i < viewItems.Count; i++)
        {
            viewItems[i].Init(this.infos[i]);
        }
    }

}
