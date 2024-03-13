using Protocol;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using UnityEngine;
using FrameWork.Socket;

namespace FrameWork.Network
{
	public class Connection
	{
		#region Members

		public ServerSession session;

		public string ip;
		public int port;
		public string roomId;
		public string password;

		public int sessionId;
		public string clientId = LocalPlayerData.MemberCode;

		public string result = string.Empty;
		public bool isConnected; 
		bool disconnected = false;

		public eConnectState connectState = eConnectState.NONE;
		public eEnterState enterState = eEnterState.NONE;

		public RealtimePacket packetManager = new RealtimePacket();
		public PacketQueue packetQueue = new PacketQueue();

		public Action callback_connect;
		public Action callback_enterFailed;
		public Action<object> callback_disconnect;

		#endregion



		#region Connection

		public Connection()
		{
			this.session = new ServerSession(this);
			this.enterState = eEnterState.NONE;
			this.connectState = eConnectState.DISCONNECTED;
			this.isConnected = false;

			#region 멤버코드 마이룸 예외처리 230410
			this.clientId = RealtimeUtils.GetMemberCode();
			#endregion

			AddListener();
			AddHandler();

			AsyncPacketUpdate().Forget();
		}

		protected void AddListener()
		{
			packetManager.CustomHandler =
			(packetSession, message, i) =>
			{
				packetQueue.Push(packetSession, i, message);
			};

			session.callback_received +=
			(session, buffer) =>
			{
				packetManager.OnRecvPacket(session, buffer);
			};

			callback_connect += ConnectHandler;
			callback_disconnect += DisconnectHandler;
		}


		protected virtual void AddHandler()
		{
			packetManager.AddHandler(RealtimePacket.MsgId.PKT_S_ENTER, this, S_ENTER);
			packetManager.AddHandler(RealtimePacket.MsgId.PKT_S_DISCONNECT, this, S_DISCONNECT);
		}

		protected virtual void RemoveHandler()
		{
			packetManager.RemoveHandler(RealtimePacket.MsgId.PKT_S_ENTER, this);
			packetManager.RemoveHandler(RealtimePacket.MsgId.PKT_S_DISCONNECT, this);
		}


		private void ConnectHandler()
		{
			connectState = eConnectState.CONNECTED;

			isConnected = true;
		}

		private void DisconnectHandler(object _endPoint)
		{
			if (_endPoint == null)
			{
				if (disconnected) return;

				RealtimeExceptionHandler.AddSocketError(System.Net.Sockets.SocketError.SocketError);

				DEBUG.LOG($"Disconnect Error : {_endPoint}", eColorManager.REALTIME);
			}

			else
			{
				isConnected = false;

				DEBUG.LOG($"Disconnected : {_endPoint}", eColorManager.REALTIME);
			}
		}


		public async UniTaskVoid AsyncPacketUpdate()
		{
			while (true)
			{
				try
				{
					var packets = packetQueue.PopAll();

					for (var i = 0; i < packets.Count; i++)
					{
						var handlers = packetManager.GetPacketHandler(packets[i].Id);
						
						if (handlers == null) continue;

						var values = new List<Action<PacketSession, IMessage>>(handlers.Values);

						for (var j = 0; j < values.Count; j++)
						{
							values[j].Invoke(packets[i].Session, packets[i].Message);
						}
					}
				}

				catch (Exception ex)
				{
					DEBUG.LOG($"AsyncPacketUpdate error : {ex}");
				}

				await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
			}
		}

		#endregion



		#region Enter

		public void Enter() => C_ENTER();

		protected virtual void C_ENTER()
		{
			C_ENTER packet = new C_ENTER();

			packet.ClientId = LocalPlayerData.MemberCode;
			packet.RoomId = roomId;
			packet.Password = password;
			packet.SessionId = sessionId;

			//packet.Nickname = "";
			//packet.IsExpose = true;
			packet.IsObserver = LocalContentsData.isObserver;

			session.Send(packet);

			DEBUG.LOG($"Enter to Server.", eColorManager.REALTIME);
		}

		protected virtual void S_ENTER(PacketSession _session, IMessage _packet)
		{
			S_ENTER packet = _packet as S_ENTER;

			switch (packet.Result)
			{
				case Cons.SUCCESS:
					enterState = eEnterState.SUCCESS;
					break;

				case Cons.ROOM_IS_SHUTDOWN:
					SceneLogic.instance.PushPopup<Popup_Basic>().ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_state_unaccessible")));
					result = packet.Result;
					enterState = eEnterState.FAIL;
					break;
				case Cons.ROOM_IS_FULL:
				case Cons.DUPLICATE:					
				case Cons.PASSWORD_FAIL:
				case Cons.KICKED:
					result = packet.Result;
					enterState = eEnterState.FAIL;
					callback_enterFailed?.Invoke();
					break;

				case Cons.WAITING:
					enterState = eEnterState.WAITING;
					SceneLogic.instance.GetPanel<Panel_Office>().WaitRoom();
					break;

				case Cons.WRONG_ROOM_ID:
					Single.RealTime.KillConnect();
					Single.RealTime.EnterRoom(RoomType.MyRoom, LocalPlayerData.MemberCode);
					break;
			}
			
			DEBUG.LOG($"Entered. Result {packet.Result} ", eColorManager.REALTIME);
		}


		public void ReEnter() => C_REENTER();

		protected virtual void C_REENTER()
		{
			C_REENTER packet = new C_REENTER();

			packet.ClientId = LocalPlayerData.MemberCode;

			session.Send(packet);

			DEBUG.LOG($"ReEnter to Server.", eColorManager.REALTIME);
		}


		#endregion



		#region Disconnect

		protected void S_DISCONNECT(PacketSession _session, IMessage _packet)
		{
			S_DISCONNECT packet = _packet as S_DISCONNECT;

			var reason = Util.String2Enum<DISCONNECT_TYPE>(packet.Code);

			switch (reason)
			{
				case DISCONNECT_TYPE.WAITING_REJECTED:
					SceneLogic.instance.PushPopup<Popup_Basic>()
					.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("office_error_entrance_reject")))
					.ChainPopupAction(new PopupAction(() => SceneLogic.instance.GetPanel<Panel_OfficeWaitRoom>().OnClick_Back()));
					break;

				case DISCONNECT_TYPE.DUPLICATED:
					SceneLogic.instance.PushPopup<Popup_Basic>()
					.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("common_error_concurrent_access")))
					.ChainPopupAction(new PopupAction(() => {
#if UNITY_EDITOR
						Single.Scene.LoadScene(SceneName.Scene_Base_Title);
#endif

						Application.Quit();
					}));

					SocketManager.Instance.KillConnect();
					break;
			}

			disconnected = true;
		}

		public void Clear()
		{
			connectState = eConnectState.DISCONNECTED;

			session.Disconnect();
			packetManager.Clear();

			RemoveHandler();
		}

		#endregion



		#region Leave

		public void Leave()
		{
			C_LEAVE packet = new C_LEAVE();

			Single.RealTime.Send(packet);
		}

		#endregion
	}
}
