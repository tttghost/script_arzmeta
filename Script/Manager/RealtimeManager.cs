
using System.Collections.Generic;
using BestHTTP.PlatformSupport.IL2CPP;
using FrameWork.UI;
using Google.Protobuf;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace FrameWork.Network
{
	[RequireComponent(typeof(RealtimeManager_InputController))]
	public class RealtimeManager : Singleton<RealtimeManager>
	{
		#region Members

		public RealtimeSwitcher<Connection> connection = new RealtimeSwitcher<Connection>();
		public RealtimeSwitcher<RoomType> roomType = new RealtimeSwitcher<RoomType>();
		public RealtimeSwitcher<SceneName> sceneName = new RealtimeSwitcher<SceneName>();

		private Connect connect;
		public string memberCode;
		public int sessionId;

		public GameObject EventSystem { get; set; }

		#endregion



		#region Initialize

		protected override void OnDestroy()
		{
			RealtimeWebManager.Clear();
			LocalContentsData.Clear();

			Timing.KillCoroutines(nameof(Co_SocketError));
		}

		protected override void OnApplicationPause(bool _isPaused)
		{
			base.OnApplicationPause(_isPaused);

			RealtimeUtils.GamePaused(_isPaused);
		}

		protected override void OnApplicationQuit()
		{
			RealtimeUtils.Leave();

			RealtimeUtils.Disconnect();

			Timing.KillCoroutines(nameof(Co_SocketError));
		}
				
		protected override void START()
		{
			base.START();

			roomType.current = RoomType.Arz;

			Timing.RunCoroutine(Co_SocketError(), nameof(Co_SocketError));
		}

		private IEnumerator<float> Co_SocketError()
		{
			while(true)
			{
				if (RealtimeExceptionHandler.socketError.Count > 0)
				{
					Single.RealTime.EventSystem.SetActive(true);

					var error = RealtimeExceptionHandler.socketError.Dequeue();

					RealtimeExceptionHandler.AddSocketError(error);
				}

				yield return Timing.WaitForSeconds(.2f);
			}
		}

		#endregion



		#region Core Methods

		/// <summary>
		/// 실시간 서버에 패킷을 전송합니다.
		/// </summary>
		public void Send(IMessage _packet)
		{
			if (connection.current == null) return;

			connection.current.session.Send(_packet);
		}


		/// <summary>
		/// 포탈이 Default일때 갈 위치 지정
		/// </summary>
		/// <param name="_roomType"></param>
		private void GetDefaultPortal(RoomType _roomType)
        {
			if (LocalContentsData.scenePortal == ScenePortal.None)
			{
				switch (_roomType)
				{
					case RoomType.Game:
						LocalContentsData.scenePortal = ScenePortal.Game1;
						break;
					default:
						LocalContentsData.scenePortal = Util.String2Enum<ScenePortal>(_roomType.ToString());
						break;
				}
			}
		}

		/// <summary>
		/// 랜드(Land) / 존(Zone) / 방(Room)에 입장합니다.
		/// </summary>
		public void EnterRoom(RoomType _roomType, string _memberCode = null)
		{
			GetDefaultPortal(_roomType);

			if (EventSystem != null)
			{
				EventSystem.SetActive(false);				
			}

			else
			{
				EventSystem = FindObjectOfType<EventSystem>().gameObject;

				EventSystem.SetActive(false);
			}

			roomType.target = _roomType;

			memberCode = _memberCode;

			RealtimeWebManager.SetQuery(_roomType, _memberCode);

			RealtimeWebManager.GetRoom();

			RealtimeWebManager.Run<RoomInfoRes[]>(CreateOrJoin);
		}



		/// <summary>
		/// RoomId로 랜드(Land) / 존(Zone) 에 입장합니다.
		/// </summary>
		public void EnterRoom(string _roomId)
		{
			RealtimeWebManager.AddQuery(Query.roomId, _roomId);

			RealtimeWebManager.GetRoom();

			RealtimeWebManager.Run<RoomInfoRes>(JoinRoom);
		}


		/// <summary>
		/// 랜드(Land) / 존(Zone) / 방(Room)에 입장합니다.
		/// </summary>
		public void EnterRoom(RoomType _roomType, int _roomId)
		{
			GetDefaultPortal(_roomType);

			if (EventSystem != null)
			{
				EventSystem.SetActive(false);
			}

			else
			{
				EventSystem = FindObjectOfType<EventSystem>().gameObject;

				EventSystem.SetActive(false);
			}

			roomType.target = _roomType;

			RealtimeWebManager.SetQuery(_roomType);
			RealtimeWebManager.AddQuery(Query.roomCode, _roomId.ToString());

			RealtimeWebManager.GetRoom();

			RealtimeWebManager.Run<RoomInfoRes[]>(CreateOrJoin);
		}



		/// <summary>
		/// 방이 있으면 입장하고 없다면 만들고 입장합니다.
		/// </summary>
		public void CreateOrJoin(RoomInfoRes[] _roomInfos) => Timing.RunCoroutine(Co_CreateOrJoin(_roomInfos));

		private IEnumerator<float> Co_CreateOrJoin(RoomInfoRes[] _roomInfos)
		{
			var roomInfo = RealtimeUtils.GetRoomInfo(_roomInfos);

			if (roomInfo != null)
			{
				JoinRoom(roomInfo);
			}

			else
			{
				yield return Timing.WaitUntilTrue(() => !RealtimeWebManager.IsRecieved());

				var roomData = RealtimeUtils.MakeRoomData(this.roomType.target, this.memberCode);

				RealtimeWebManager.CreateRoom(roomData);

				RealtimeWebManager.Run<RoomInfoRes>(JoinRoom);
			}
		}



		/// <summary>
		/// 방에 입장합니다.
		/// </summary>
		public void JoinRoom(RoomInfoRes _roomInfo)
		{
			LocalPlayerData.Method.roomCode = memberCode;

			var roomId = _roomInfo.roomId;
			var roomType = _roomInfo.roomType != null ? _roomInfo.roomType : this.roomType.target.ToString();
			var sceneName = _roomInfo.sceneName;
			var password = RealtimeUtils.GetPassword(roomType);
			
			connect = new Connect(roomId, roomType, sceneName, password);

			Connect(connect);
		}

		private void Connect(Connect _connect) => Timing.RunCoroutine(Co_Connect(_connect), "Co_Connect");

		private IEnumerator<float> Co_Connect(Connect _connect)
		{
			DEBUG.LOG("Connect to server.", eColorManager.REALTIME);

			RealtimeUtils.RunDim(nameof(RealtimeManager));

			connection.target = RealtimeUtils.MakeConnection(_connect);
			connection.target.sessionId = sessionId;

			RealtimeUtils.Connect(connection.target);


			yield return Timing.WaitUntilTrue(() => connection.target.isConnected);

			DEBUG.LOG("Connected.", eColorManager.REALTIME);

			switch (connection.target.connectState)
			{
				case eConnectState.CONNECTED:
					RealtimeUtils.EnterToServer();

					yield return Timing.WaitUntilTrue(() => connection.target.enterState != eEnterState.NONE);

					switch (connection.target.enterState)
					{
						case eEnterState.SUCCESS:
							yield return Timing.WaitUntilDone(RealtimeUtils.Co_SetOfficeUserInfo());

							DEBUG.LOG($"Enter Successed..! Now loading {_connect.sceneName}. Please Wait...", eColorManager.REALTIME);
														
							LocalContentsData.roomId = connect.roomId;

							if (Single.RealTime.connection.current != null) Single.RealTime.connection.current.Leave();

							connection.Swap(connection.target);
							roomType.Swap(Util.String2Enum<RoomType>(_connect.roomType));
							sceneName.Swap(Util.String2Enum<SceneName>(_connect.sceneName));
							
							RealtimeUtils.SetSession(connection.current.session);						

							Single.Scene.FadeOut(_action: () => Single.Scene.LoadScene(sceneName.current));

                            break; 

						case eEnterState.WAITING:
							DEBUG.LOG("WAITING..", eColorManager.REALTIME);
	
							Single.Scene.SetDimOff();

							yield return Timing.WaitUntilTrue(RealtimeUtils.IsEnterSuccess);
							yield return Timing.WaitUntilDone(RealtimeUtils.Co_SetOfficeUserInfo());

							DEBUG.LOG($"Enter Successed..! Now loading {_connect.sceneName}. Please Wait...", eColorManager.REALTIME);

							Single.Scene.SetDimOn();

							LocalContentsData.roomId = connect.roomId;

							connection.Swap(connection.target);
							roomType.Swap(Util.String2Enum<RoomType>(_connect.roomType));
							sceneName.Swap(Util.String2Enum<SceneName>(_connect.sceneName));

							RealtimeUtils.SetSession(connection.current.session);

							Single.Scene.FadeOut(_action: () => Single.Scene.LoadScene(sceneName.current));

							break;

						case eEnterState.FAIL:
							DEBUG.LOG("Enter Failed..", eColorManager.REALTIME);

							LocalContentsData.isObserver = false;
							RealtimeExceptionHandler.AddPacketError(connection.target.result);
							break;

						case eEnterState.DISCONNECTED:
							DEBUG.LOG("Disconnected..", eColorManager.REALTIME);

							LocalContentsData.isObserver = false;
							RealtimeExceptionHandler.AddSocketError(System.Net.Sockets.SocketError.SocketError);
							break;
					}
					break;

				case eConnectState.ERROR:
					RealtimeExceptionHandler.AddSocketError(System.Net.Sockets.SocketError.ConnectionRefused);
					break;

				case eConnectState.DISCONNECTED:
					break;
			}

			RealtimeUtils.KillDim(nameof(RealtimeManager));
		}



		public void Reconnect()
		{
			Connect(connect);
		}

		#endregion



		#region Methods

		public void SetRoom(RoomData _data) => RealtimeWebManager.SetRoomData(_data);

		public void KillConnect()
		{
			Timing.KillCoroutines("Co_Connect");
			LocalContentsData.roomCode = string.Empty;
		}

		#endregion
	}
}

public class Connect
{
	public string roomId;
	public string roomType;
	public string sceneName;
	public string password;

	public Connect(string _roomId, string _roomType, string _sceneName = null, string _password = null)
	{
		this.roomId = _roomId;
		this.roomType = _roomType;
		this.sceneName = _sceneName;
		this.password = _password;
	}
}

[System.Serializable, Preserve]
public class RoomData
{
	public string ownerId;
	public string roomCode;
	public string roomType;

	public string sceneName;
	public string roomName;
}


[System.Serializable, Preserve]
public class OfficeRoomData : RoomData
{
	public string description;
	public string spaceInfoId;
	public string thumbnail;

	public int modeType;
	public int topicType;
	public string creatorId;
	public string password;
	public int personnel;

	public int observer;
	public bool isWaitingRoom;
	public bool isAdvertising;
	public int runningTime;
}


[System.Serializable, Preserve]
public class GameRoomData : RoomData
{
	public bool isPlaying;
	public int maxPlayerNumber;
	public int currentPlayerNumber;
}

public class RealtimeSwitcher<T>
{
	public T target;
	public T current;
	public T prev;

	public void Swap(T _target)
	{
		prev = current;
		current = _target;
	}

	public void ShowLog()
	{
		DEBUG.LOG($"prev scene is {prev}, current scene is : {current}", eColorManager.REALTIME);
	}
}