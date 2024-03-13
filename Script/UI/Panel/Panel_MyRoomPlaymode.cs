using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using MEC;
using Protocol;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MyRoomPlaymode : PanelBase
{
    public Button btn_EnterEditmode { get; private set; }
    private string ownerNickname;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_EnterEditmode = GetUI_Button(nameof(btn_EnterEditmode), OnClick_EnterEditmode);

        Util.RunCoroutine(Co_SetOwnerName());
    }
    protected override void Start()
    {
        base.Start();

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_GET_ROOMINFO, this, S_MYROOM_GET_ROOMINFO);

        C_MYROOM_GET_ROOMINFO();

    }
    private void OnDestroy()
	{
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_GET_ROOMINFO, this);
    }
        
    private void OnClick_EnterEditmode()
    {
        PushPopup<Popup_MyRoomSetting>();
    }

    IEnumerator<float> Co_SetOwnerName()
    {
        yield return Timing.WaitUntilTrue(()=> MyPlayer.instance != null);

        MyPlayer.instance.changeNickName += SetOwnerName;

    }

    /// <summary>
    /// 마이룸 Onwer 닉네임 표시
    /// </summary>
    private void SetOwnerName()
    {
        GetUI_TxtmpMasterLocalizing("txtmp_RoomOwnerName", new MasterLocalData("myroom_title", ownerNickname));
    }

    private void C_MYROOM_GET_ROOMINFO()
	{
        C_MYROOM_GET_ROOMINFO packet = new C_MYROOM_GET_ROOMINFO();
 
        Single.RealTime.Send(packet);
    }

    public void S_MYROOM_GET_ROOMINFO(PacketSession _session, IMessage _packet)
    {
        S_MYROOM_GET_ROOMINFO packet = _packet as S_MYROOM_GET_ROOMINFO;
        
        LocalContentsData.prevMemberCode = packet.OwnerId;
        if (LocalPlayerData.MemberCode != packet.OwnerId) //들어온방이 상대방 방이라면?
        {
            LocalContentsData.prevMemberCode = LocalPlayerData.MemberCode;
        }

        ownerNickname = packet.OwnerNickname;
        SetOwnerName();
	}
}
