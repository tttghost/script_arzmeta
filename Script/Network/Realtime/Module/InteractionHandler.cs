using Google.Protobuf;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인터렉션 핸들러
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    #region 변수
    //[SerializeField] public string id;
    //[SerializeField] public string state;

    public delegate void	S_INTERACTION_SET_ITEM_HANDLER(S_INTERACTION_SET_ITEM S_INTERACTION_SET_ITEM);
	public event			S_INTERACTION_SET_ITEM_HANDLER INTERACTION_SET_ITEM_HANDLER;

	public delegate void	S_INTERACTION_SET_ITEM_NOTICE_HANDLER(S_INTERACTION_SET_ITEM_NOTICE S_INTERACTION_SET_ITEM_NOTICE);
	public event			S_INTERACTION_SET_ITEM_NOTICE_HANDLER INTERACTION_SET_ITEM_NOTICE_HANDLER;

	public delegate void	S_INTERACTION_REMOVE_ITEM_HANDLER(S_INTERACTION_REMOVE_ITEM S_INTERACTION_REMOVE_ITEM);
	public event			S_INTERACTION_REMOVE_ITEM_HANDLER INTERACTION_REMOVE_ITEM_HANDLER;

	public delegate void	S_INTERACTION_REMOVE_ITEM_NOTICE_HANDLER(S_INTERACTION_REMOVE_ITEM_NOTICE s_INTERACTION_GET_ITEMS);
	public event			S_INTERACTION_REMOVE_ITEM_NOTICE_HANDLER INTERACTION_REMOVE_ITEM_NOTICE_HANDLER;

	public delegate void	S_INTERACTION_GET_ITEMS_HANDLER(S_INTERACTION_GET_ITEMS s_INTERACTION_GET_ITEMS);
	public event			S_INTERACTION_GET_ITEMS_HANDLER INTERACTION_GET_ITEMS_HANDLER;

	public S_INTERACTION_GET_ITEMS getItems { get; private set; }
	public string interactionId { get; set; } = string.Empty;

    #endregion



    #region 함수



    #region 핸들러 등록 / 삭제
    private void OnEnable()
	{
		AddHandler();
        if (Single.RealTime.roomType.current != RoomType.Lecture)
        {
            C_INTERACTION_GET_ITEMS();
        }
    }
	//private void Update()
	//{
	//    if (Input.GetKeyDown(KeyCode.Alpha1))
	//    {
	//        C_INTERACTION_SET_ITEM(id);
	//    }
	//    if (Input.GetKeyDown(KeyCode.Alpha2))
	//    {
	//        C_INTERACTION_REMOVE_ITEM(id);
	//    }
	//    if (Input.GetKeyDown(KeyCode.Alpha3))
	//    {
	//        C_INTERACTION_GET_ITEMS();
	//    }
	//}
	private void AddHandler()
	{
		//인터렉션 추가
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM, this, S_INTERACTION_SET_ITEM);
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM_NOTICE, this, S_INTERACTION_SET_ITEM_NOTICE);

		//인터렉션 제거
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_REMOVE_ITEM, this, S_INTERACTION_REMOVE_ITEM);
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_REMOVE_ITEM_NOTICE, this, S_INTERACTION_REMOVE_ITEM_NOTICE);

		//인터렉션 확인
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_GET_ITEMS, this, S_INTERACTION_GET_ITEMS);
	}
	private void OnDisable()
	{
		RemoveHandler();
	}
	private void RemoveHandler()
	{
		//인터렉션 추가
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM, this);
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_SET_ITEM_NOTICE, this);

		//인터렉션 제거
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_REMOVE_ITEM, this);
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_REMOVE_ITEM_NOTICE, this);

		//인터렉션 확인
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_INTERACTION_GET_ITEMS, this);
	}

    #endregion



    #region 실시간소켓 

    //인터렉션 추가
    public void C_INTERACTION_SET_ITEM(string id)
	{
		C_INTERACTION_SET_ITEM packet = new C_INTERACTION_SET_ITEM() { Id = id, State = string.Empty, };
		Single.RealTime.Send(packet);
	}
	private void S_INTERACTION_SET_ITEM(PacketSession arg1, IMessage arg2)
	{
		S_INTERACTION_SET_ITEM packet = arg2 as S_INTERACTION_SET_ITEM;
		//Debug.Log("S_INTERACTION_SET_ITEM : " + packet.Success);

		INTERACTION_SET_ITEM_HANDLER?.Invoke(packet);
	}
	public void S_INTERACTION_SET_ITEM_NOTICE(PacketSession arg1, IMessage arg2)
	{
		S_INTERACTION_SET_ITEM_NOTICE packet = arg2 as S_INTERACTION_SET_ITEM_NOTICE;
		//Debug.Log("S_INTERACTION_SET_ITEM_NOTICE : " + packet.Id);

		INTERACTION_SET_ITEM_NOTICE_HANDLER?.Invoke(packet);
	}



	// 인터렉션 제거
	public void C_INTERACTION_REMOVE_ITEM(string id)
	{
		C_INTERACTION_REMOVE_ITEM packet = new C_INTERACTION_REMOVE_ITEM() { Id = id };
		Single.RealTime.Send(packet);
	}
	public void S_INTERACTION_REMOVE_ITEM(PacketSession arg1, IMessage arg2)
	{
		S_INTERACTION_REMOVE_ITEM packet = arg2 as S_INTERACTION_REMOVE_ITEM;
		//Debug.Log("S_INTERACTION_REMOVE_ITEM : " + packet.Success);

		INTERACTION_REMOVE_ITEM_HANDLER?.Invoke(packet);
	}
	public void S_INTERACTION_REMOVE_ITEM_NOTICE(PacketSession arg1, IMessage arg2)
	{
		S_INTERACTION_REMOVE_ITEM_NOTICE packet = arg2 as S_INTERACTION_REMOVE_ITEM_NOTICE;
		//Debug.Log("S_INTERACTION_REMOVE_ITEM_NOTICE : " + packet.Id);

		INTERACTION_REMOVE_ITEM_NOTICE_HANDLER?.Invoke(packet);
	}


    //인터렉션 확인
    public void C_INTERACTION_GET_ITEMS()
    {
        C_INTERACTION_GET_ITEMS packet = new C_INTERACTION_GET_ITEMS();
		Single.RealTime.Send(packet);
	}
	public void S_INTERACTION_GET_ITEMS(PacketSession arg1, IMessage arg2)
	{
		getItems = arg2 as S_INTERACTION_GET_ITEMS;
		//Debug.Log("S_INTERACTION_GET_ITEMS Count: " + getItems.Items.Count);
		//for (int i = 0; i < getItems.Items.Count; i++)
		//{
		//	DEBUG.LOG($"packet.Items[{i}].Id : " + getItems.Items[i].Id);
		//}
		INTERACTION_GET_ITEMS_HANDLER?.Invoke(getItems);
	}

    #endregion

    #endregion
}
