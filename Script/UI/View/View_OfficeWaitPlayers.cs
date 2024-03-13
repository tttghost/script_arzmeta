using System.Collections.Generic;
using MEC;
using Google.Protobuf;
using Protocol;
using TMPro;
using UnityEngine.UI;
using FrameWork.Network;

public class View_OfficeWaitPlayers : View_OfficeRealTime
{
    private ScrollRect sview_Content;
    private TMP_Text txtmp_Caution;

    private Button btn_AcceptAll;
    private Button btn_RejectAll;

    private Dictionary<string, Item_OfficeUser> dicScrollitem = new Dictionary<string, Item_OfficeUser>();

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        sview_Content = GetUI<ScrollRect>(nameof(sview_Content));

        btn_AcceptAll = GetUI_Button(nameof(btn_AcceptAll), OnClickAcceptAll);
        btn_RejectAll = GetUI_Button(nameof(btn_RejectAll), OnClickRejectAll);

        GetUI_TxtmpMasterLocalizing("txtmp_AcceptAll", new MasterLocalData("common_bulk_accept"));
        GetUI_TxtmpMasterLocalizing("txtmp_RejectAll", new MasterLocalData("common_bulk_reject"));

        txtmp_Caution = GetUI_TxtmpMasterLocalizing(
            "txtmp_Caution", new MasterLocalData("common_state_empty"));
    }

    #region 실시간 서버 패킷 처리 함수
    protected override void AddHandler()
    {
        base.AddHandler();

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_ADD_WAITING_CLIENT, this, S_OFFICE_ADD_WAITING_CLIENT);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_REMOVE_WAITING_CLIENT, this, S_OFFICE_REMOVE_WAITING_CLIENT);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this, S_OFFICE_KICK);
    }

    protected override void RemoveHandler()
    {
        base.RemoveHandler();

        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_ADD_WAITING_CLIENT, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_REMOVE_WAITING_CLIENT, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this);
    }

    // 대기자 전체 리스트 요청
    public override void REQUEST_NFO()
    {
        Util.KillCoroutine("buttontween");

        foreach(KeyValuePair<string, Item_OfficeUser> pair in dicScrollitem)
        {
            pair.Value.StopButtonTweenComponent();
            Destroy(pair.Value.gameObject);
        }

        dicScrollitem.Clear();

        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());
        Single.RealTime.Send(new C_OFFICE_GET_WAITING_LIST());
    }

    private void S_OFFICE_ADD_WAITING_CLIENT(PacketSession session, IMessage packet)
    {
        S_OFFICE_ADD_WAITING_CLIENT message = packet as S_OFFICE_ADD_WAITING_CLIENT;

        // 내가 관리자 일 때, 모드 수락, 모두 거절 버튼 활성화
        if (sceneOfficeRoom.IsAdmin())
		{
            btn_AcceptAll.interactable = true;
            btn_RejectAll.interactable = true;
        }
        else
		{
            btn_AcceptAll.interactable = false;
            btn_RejectAll.interactable = false;
        }

        if (message.Clients.Count == 0)
            txtmp_Caution.gameObject.SetActive(true);
        else
            txtmp_Caution.gameObject.SetActive(false);

        foreach (S_OFFICE_ADD_WAITING_CLIENT.Types.WaitingClient waitclient in message.Clients)
        {
            // 이미 ScrollItem 이 있다면, Add 가 아니고, 데이터만 갱신
            if (dicScrollitem.ContainsKey(waitclient.ClientId))
            {
                Item_OfficeUser item_officeuser;

                if (dicScrollitem.TryGetValue(waitclient.ClientId, out item_officeuser))
                    item_officeuser.SetData_OfficeUserWaiting(waitclient.ClientId, waitclient.Nickname);

                continue;
            }

            Item_OfficeUser scrollitem =
                        Single.Resources.Instantiate<Item_OfficeUser>(
                            Cons.Path_Prefab_Item + "Item_OfficeUser",
                            sview_Content.content.transform);

            scrollitem.SetData_OfficeUserWaiting(waitclient.ClientId, waitclient.Nickname);
            scrollitem.gameObject.SetActive(true);
            dicScrollitem.Add(waitclient.ClientId, scrollitem);
        }

        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());
    }

    private void S_OFFICE_REMOVE_WAITING_CLIENT(PacketSession session, IMessage packet)
    {
        //Timing.KillCoroutines();

        S_OFFICE_REMOVE_WAITING_CLIENT message = packet as S_OFFICE_REMOVE_WAITING_CLIENT;
        foreach(S_OFFICE_REMOVE_WAITING_CLIENT.Types.WaitingClient waitclient in message.Clients)
        {
            Item_OfficeUser itemofficeuser;
            if(dicScrollitem.TryGetValue(waitclient.ClientId, out itemofficeuser))
            {
                itemofficeuser.StopButtonTweenComponent();
                Destroy(itemofficeuser.gameObject);
                dicScrollitem.Remove(waitclient.ClientId);
            }
        }

        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());
    }

    private void S_OFFICE_KICK(PacketSession session, IMessage packet)
    {
        S_OFFICE_KICK message = packet as S_OFFICE_KICK;
        if(message.Success == false)
        {
            DEBUG.LOG("S_OFFICE_KICK : 사용자 KICK 실패", eColorManager.REALTIME);
        }

        REQUEST_NFO();
    }
    #endregion

    #region 버튼 콜백
    private void OnClickAcceptAll()
    {
        if(dicScrollitem.Count == 0)
            return;

        C_OFFICE_ACCEPT_WAIT message = new C_OFFICE_ACCEPT_WAIT();

        foreach(KeyValuePair<string, Item_OfficeUser> pair in dicScrollitem)
        {
            message.ClientId.Add(pair.Value.waitingUserClientId);
        }
        message.IsAccepted = true;

        Single.RealTime.Send(message);

        REQUEST_NFO();
    }

    private void OnClickRejectAll()
    {
        if(dicScrollitem.Count == 0)
            return;

        C_OFFICE_ACCEPT_WAIT message = new C_OFFICE_ACCEPT_WAIT();

        foreach(KeyValuePair<string, Item_OfficeUser> pair in dicScrollitem)
        {
            message.ClientId.Add(pair.Value.waitingUserClientId);
        }
        message.IsAccepted = false;

        Single.RealTime.Send(message);

        REQUEST_NFO();
    }
    #endregion
}