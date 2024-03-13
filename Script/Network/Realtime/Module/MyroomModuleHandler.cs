using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyroomModuleHandler : MonoBehaviour
{
	//public string kickId;
	//public bool shutdown;
	private void OnEnable()
	{

		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_START_EDIT, this, S_MYROOM_START_EDIT);
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_END_EDIT, this, S_MYROOM_END_EDIT);
		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_KICK, this, S_MYROOM_KICK);
		//RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_SHUTDOWN, this, S_MYROOM_SHUTDOWN);

		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_DISCONNECT, this, S_DISCONNECT);// 연결 해제시 호출

		//RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_ADD_CLIENT, this, S_ADD_CLIENT);    // 사람 정보 들어올 때 Update
		//RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_REMOVE_CLIENT, this, S_REMOVE_CLIENT); // 사람이 나갈 때 마다 Update

		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA_NOTICE, this, S_BASE_SET_OBJECT_DATA_NOTICE);

		RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this, S_SET_NICKNAME_NOTICE);
	}
	private void OnDisable()
	{

		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_START_EDIT, this);
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_END_EDIT, this);
		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_KICK, this);
		//RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_SHUTDOWN, this); // 230912 주석했음, 위에도 주석되어있어서 같이 주석함

		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_DISCONNECT, this);// 연결 해제시 호출

		//RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_ADD_CLIENT, this);    // 사람 정보 들어올 때 Update
		//RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_REMOVE_CLIENT, this); // 사람이 나갈 때 마다 Update

		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_SET_OBJECT_DATA_NOTICE, this);

		RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_SET_NICKNAME_NOTICE, this);
	}


	/// <summary>
	/// 유저데이터 갱신 콜백
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	private void S_BASE_SET_OBJECT_DATA_NOTICE(PacketSession arg1, IMessage arg2)
	{
		S_BASE_SET_OBJECT_DATA_NOTICE packet = arg2 as S_BASE_SET_OBJECT_DATA_NOTICE;

		//네트워크핸들러 가져옴
		//Scene_MyRoom scene_MyRoom = MyRoomManager.Instance.GetSceneMyRoom();
		//NetworkHandler networkHandler = scene_MyRoom.networkHandler;

		//유저데이터 오브젝터 변경
		//UserData userData = networkHandler.UserDatas[packet.ObjectId];
		UserData userData = SceneLogic.instance.networkHandler.UserDatas[packet.ObjectId];
		userData.ObjectData = packet.ObjectData;

		//핸들러 갱신
		SceneLogic.instance.GetPopup<Popup_MyRoomSetting>().userDataHandler?.Invoke(userData);

		//if (packet.ObjectId != objectId) return;

		//avatarPartsController.SetAvatarParts(packet.ObjectData,
		//() =>
		//{
		//    //if (outline) outline.Initialize();
		//});
	}


	/// <summary>
	/// 마이룸 닉네임 변경시 마이룸셋팅 유저정보 변경
	/// </summary>
	/// <param name="_session"></param>
	/// <param name="_packet"></param>
	private void S_SET_NICKNAME_NOTICE(PacketSession _session, IMessage _packet)
    {
        S_SET_NICKNAME_NOTICE packet = _packet as S_SET_NICKNAME_NOTICE;

        var networkHandler = FindObjectOfType<NetworkHandler>();

        //유저데이터 오브젝터 변경
        UserData userData = networkHandler.UserDatas.Single(x => x.Value.OwnerId == packet.ClientId).Value;
        userData.Nickname = packet.Nickname;

        //핸들러 갱신
        SceneLogic.instance.GetPopup<Popup_MyRoomSetting>().userDataHandler?.Invoke(userData);

    }

	/// <summary>
	/// 마이룸 편집 시작 호출
	/// </summary>
	public void C_MYROOM_START_EDIT()
	{
		C_MYROOM_START_EDIT C_MYROOM_START_EDIT = new C_MYROOM_START_EDIT();
		Single.RealTime.Send(C_MYROOM_START_EDIT);
	}

	/// <summary>
	/// 마이룸 편집 시작 콜백
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	private void S_MYROOM_START_EDIT(PacketSession arg1, IMessage arg2)
	{
		S_MYROOM_START_EDIT packet = arg2 as S_MYROOM_START_EDIT;
		if (LocalPlayerData.Method.roomOwnerName == LocalPlayerData.NickName)
		{
			Debug.Log("내방");
		}
		else
		{
			Debug.Log("남에방");
		}
	}

	/// <summary>
	/// 마이룸 편집 종료 호출
	/// </summary>
	public void C_MYROOM_END_EDIT(bool isChanged)
	{
		C_MYROOM_END_EDIT C_MYROOM_END_EDIT = new C_MYROOM_END_EDIT();
		C_MYROOM_END_EDIT.IsChanged = isChanged;
		Single.RealTime.Send(C_MYROOM_END_EDIT);
	}

	/// <summary>
	/// 마이룸 편집 종료 콜백
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	private void S_MYROOM_END_EDIT(PacketSession arg1, IMessage arg2)
	{
		S_MYROOM_END_EDIT packet = arg2 as S_MYROOM_END_EDIT;

		if (packet.IsChanged)
		{
			Debug.Log("실시간서버 방정보가 바꼇어요");

			MyRoomManager.Instance.gridSystem.MyRoomOthersRoomList_Req();
			InteractionCustomManager.Instance.EndInteraction();

			MyPlayer.instance.Teleport(SceneLogic.instance.GetWayPoint(ScenePortal.MyRoom), null, false, true);
		}
		else
		{
			Debug.Log("실시간서버 방정보 그대로에요");

			MyRoomManager.Instance.gridSystem.MyRoomOthersRoomList_Req();
		}
	}

	/// <summary>
	/// 마이룸 단일 강퇴 호출
	/// </summary>
	/// <param name="sessionId"></param>
	public void C_MYROOM_KICK(string sessionId)
	{
		C_MYROOM_KICK C_MYROOM_KICK = new C_MYROOM_KICK();
		C_MYROOM_KICK.ClientId = sessionId;
		Single.RealTime.Send(C_MYROOM_KICK);
	}

	/// <summary>
	/// 마이룸 단일 강퇴 콜백
	/// </summary>
	/// <param name="arg1"></param>
	/// <param name="arg2"></param>
	private void S_MYROOM_KICK(PacketSession arg1, IMessage arg2)
	{
		S_MYROOM_KICK packet = arg2 as S_MYROOM_KICK;
		Debug.Log("Kicked");
		//packet.Success;
	}

    /// <summary>
    /// 마이룸 폐쇄 호출 -> S_DISCONNECT
    /// </summary>
    /// <param name="isShutdown"></param>
    public void C_MYROOM_SHUTDOWN(bool isShutdown)
    {
        C_MYROOM_SHUTDOWN C_MYROOM_SHUTDOWN = new C_MYROOM_SHUTDOWN();
        C_MYROOM_SHUTDOWN.IsShutdown = isShutdown;
        Single.RealTime.Send(C_MYROOM_SHUTDOWN);
    }

    /// <summary>
    /// 끊겼을때 콜백
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void S_DISCONNECT(PacketSession arg1, IMessage arg2)
	{
		S_DISCONNECT packet = arg2 as S_DISCONNECT;
		DISCONNECT_TYPE DISCONNECT_TYPE = Util.String2Enum<DISCONNECT_TYPE>(packet.Code);

		switch (DISCONNECT_TYPE)
		{
			case DISCONNECT_TYPE.CLOSING:
				break;
			case DISCONNECT_TYPE.KICKED:
				{
					Debug.Log("강퇴 당함");
					Single.RealTime.EnterRoom(Single.RealTime.roomType.prev, LocalContentsData.prevMemberCode);
				}
				break;
			case DISCONNECT_TYPE.SERVER_CHANGE:
				{
					Debug.Log("씬 이동");
				}
				break;
			default:
				Debug.LogError($"{DISCONNECT_TYPE} 끊기는 케이스 정의안함. 정의해야함");
				break;
		}
	}

	
	//private List<ClientData> clientDataList = new List<ClientData>();
	///// <summary>
	///// 마이룸 인원추가 콜백
	///// </summary>
	///// <param name="arg1"></param>
	///// <param name="arg2"></param>
	//private void S_ADD_CLIENT(PacketSession arg1, IMessage arg2)
	//{
	//	S_ADD_CLIENT packet = arg2 as S_ADD_CLIENT;
	//	for (int i = 0; i < packet.ClientInfos.Count; i++)
	//	{
	//		S_ADD_CLIENT.Types.ClientInfo clientInfo = packet.ClientInfos[i];
	//		//ClientData clientData = new ClientData(clientInfo.ClientId, clientInfo.Nickname);
	//		ClientData clientData = new ClientData(clientInfo);
	//		clientDataList.Add(clientData);
	//	}
	//}

	///// <summary>
	///// 마이룸 인원감소 콜백
	///// </summary>
	///// <param name="arg1"></param>
	///// <param name="arg2"></param>
	//public void S_REMOVE_CLIENT(PacketSession _session, IMessage _packet)
	//{
	//	S_REMOVE_CLIENT packet = _packet as S_REMOVE_CLIENT;

	//	for (int i = 0; i < packet.ClientIds.Count; i++)
	//	{
	//		string cliendId = packet.ClientIds[i];
	//		Debug.Log("cliendId  : " + cliendId);
	//	}
	//}
}
