using System;
using FrameWork.Network;
using System.Collections.Generic;
using Google.Protobuf;
using Cysharp.Threading.Tasks;
using System.Net;
using MEC;
using FrameWork.UI;

public class RealtimeUtils
{
	#region Packet Handler

	public static void AddHandler(RealtimePacket.MsgId _packetId, object _object, Action<PacketSession, IMessage> _handler)
	{
		Single.RealTime.connection.current.packetManager.AddHandler(_packetId, _object, _handler);
	}

	public static void RemoveHandler(RealtimePacket.MsgId _packetId, object _object)
	{
		try
		{
			Single.RealTime.connection.current.packetManager.RemoveHandler(_packetId, _object);
		}
		catch { }
	}

	#endregion



	#region Getter

	public static Connection GetConnection()
	{
		if (Single.RealTime.connection == null) return null;

		return Single.RealTime.connection.current;
	}

	public static eConnectState GetConnectState()
	{
		Connection connection = GetConnection();

		if (connection != null)
		{
			return connection.connectState;
		}

		return eConnectState.DISCONNECTED;
	}

	public static string GetMemberCode()
	{
		return LocalPlayerData.MemberCode;
	}

	public static RoomInfoRes GetRoomInfo(RoomInfoRes[] _roomInfos)
	{
		if (_roomInfos.Length > 0)
		{
			return _roomInfos[UnityEngine.Random.Range(0, _roomInfos.Length)];
		}

		return null;
	}

	#endregion



	#region Core Methods

	public static void Connect(Connection _connection) 
	{
		var endPoint = new IPEndPoint(IPAddress.Parse(_connection.ip), _connection.port);
		var connector = new Connector();

		connector.Connect(endPoint, () => _connection.session);
	}

	#endregion



	#region Utils

	public static Connection MakeConnection(Connect _connect)
	{
		Connection connection = new Connection();

		connection.ip = Define.IP;
		connection.port = Define.PORT;
		connection.roomId = _connect.roomId;
		connection.password = _connect.password;
		connection.connectState = eConnectState.CONNECTING;

		return connection;
	}

	public static void EnterToServer()
	{
		var connection = Single.RealTime.connection.target;

		if (RealtimeExceptionHandler.isReconnect)
		{
			ReEnter(connection);

			RealtimeExceptionHandler.isReconnect = false;
		}

		else Enter(connection);
	}

	private static void Enter(Connection _connection)
	{
		_connection.Enter();
	}

	private static void ReEnter(Connection _connection)
	{
		_connection.ReEnter();
	}

	public static RoomData MakeRoomData(RoomType _roomType, string _memberCode = null)
	{
		RoomData data = new RoomData();

		switch(_roomType)
		{
			case RoomType.Meeting:
			case RoomType.Lecture:
			case RoomType.Consulting:
				data = RealtimeWebManager.RoomData as OfficeRoomData;
				break;
			case RoomType.Exposition:
			case RoomType.Exposition_Booth:
				data.roomCode = LocalPlayerData.MemberCode; // 임시 23/09/12
				break;
			case RoomType.JumpingMatching:
			case RoomType.OXQuiz:
				data = RealtimeWebManager.RoomData as GameRoomData;
				break;
		}

		data.ownerId = _memberCode;
		data.roomType = GetRoomType(_roomType);
		data.sceneName = GetSceneName(_roomType);

		return data;
	}

	public static string GetSceneName(RoomType _roomType)
	{
		string value = "Scene_";

		switch (_roomType)
		{
			case RoomType.None:
			case RoomType.Arz:
			case RoomType.Busan:
				value += "Land_";
				break;

			case RoomType.Conference:
			case RoomType.Game:
			case RoomType.Office:
			case RoomType.Hospital:
			case RoomType.Store:
			case RoomType.Vote:
			case RoomType.Festival:
			case RoomType.Exposition:
				value += "Zone_";
				break;

			case RoomType.JumpingMatching:
			case RoomType.OXQuiz:
			case RoomType.MyRoom:
			case RoomType.Meeting:
			case RoomType.Lecture:
			case RoomType.Consulting:
			case RoomType.Exposition_Booth:
				value += "Room_";
				break;
		}

		return value + _roomType.ToString();
	}

	public static string GetOfficeSceneName(string _spaceInfoId)
	{
		var sceneName = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(_spaceInfoId)).sceneName;
		DEBUG.LOG(_spaceInfoId + " "  + sceneName);
		return sceneName;
	}

	public static string MakeQuery(Dictionary<string, string> _query)
	{
		if (_query.Count < 0) return string.Empty;

		else
		{
			string value = "?";

			foreach (var element in _query)
			{
				value += element.Key + "=" + element.Value + "&";
			}

			return value.Remove(value.Length - 1);
		}
	}

	public static string GetRoomType(RoomType _roomType)
	{
		return _roomType.ToString();
	}

	public static string GetPassword(string _roomType)
	{
		var roomType = (RoomType)Enum.Parse(typeof(RoomType), _roomType);

		switch (roomType)
		{
			case RoomType.Meeting:
			case RoomType.Lecture:
			case RoomType.Consulting:
				var roomData = RealtimeWebManager.RoomData as OfficeRoomData;

				if (roomData == null) break;
				return roomData.password;
		}

		return string.Empty;
	}

	public static void RunDim(string tag, float _delay = 0)
	{
		Timing.RunCoroutine(Co_DimOn().Delay(_delay), tag);

		Timing.RunCoroutine(Co_DimOff(10f, LoginFail), tag);
	}

	private static IEnumerator<float> Co_DimOn(Action _action = null)
	{
		if (Single.Scene.isDim) yield break;

		Single.Scene.SetDimOn();

		_action?.Invoke();
	}

	private static IEnumerator<float> Co_DimOff(float _time, Action _action = null)
	{
		yield return Timing.WaitForSeconds(_time);

		Single.Scene.SetDimOff();

		_action?.Invoke();
	}

	public static void KillDim(string tag)
	{
		Timing.KillCoroutines(tag);
	}

	private static void LoginFail()
	{
		var sceneName = Single.Scene.GetSceneId();

		if (sceneName != SceneName.Scene_Base_Title.ToString()) return;

		Single.RealTime.KillConnect();

		SceneLogic.instance.GetPanel<Panel_Title>().RefreshUI();

		RealtimeExceptionHandler.ErrorPopup(SocketError.NETWORK_ERROR);
	}


	private static DateTime pauseTime;
	private static DateTime restartTime;
	private static TimeSpan elapsedTime;
	private static bool isPaused = false;

	public static void GamePaused(bool _isPaused)
	{
		if (_isPaused)
		{
			pauseTime = DateTime.Now;

			isPaused = true;
		}

		else
		{
			if (!isPaused) return;

			restartTime = DateTime.Now;

			elapsedTime = restartTime - pauseTime;

			if (elapsedTime.TotalMinutes > Cons.TIMEOUT)
			{
				//LocalPlayerData.ResetData();

				//Single.Scene.LoadScene(SceneName.Scene_Base_Logo);

				isPaused = false;
			}
		}
	}

	private static ServerSession session;

	public static void SetSession(ServerSession _session)
	{
		session = _session;
	}

	public static void Disconnect()
	{
		if (session != null)
		{
			session.Disconnect();
		}
	}

	public static void Leave()
	{
		if (Single.RealTime.connection != null)
		{
			if (Single.RealTime.connection.current != null) Single.RealTime.connection.current.Leave();

			if (Single.RealTime.connection.target != null) Single.RealTime.connection.target.Leave();
		}
	}

	public static IEnumerator<float> Co_SetOfficeUserInfo()
	{
		if (LocalContentsData.isOfficeEnter)
		{
			Single.Scene.SetDimOff();

			DEBUG.LOG("OFFICE ENTER SETTING.", eColorManager.REALTIME);

			 SceneLogic.instance.PushPopup<Popup_OfficeEnterSetting>();
		}

		yield return Timing.WaitUntilTrue(() => !LocalContentsData.isOfficeEnter);
	}

	public static bool IsEnterSuccess()
	{
		return Single.RealTime.connection.target.enterState == eEnterState.SUCCESS;
	}

	public static string GetErrorMasterId(SocketError _message)
	{
		string masterId = string.Empty;

		switch (_message)
		{
			case SocketError.NETWORK_ERROR:
				masterId = "common_error_network_01";
				break;
			case SocketError.NETWORK_RECONNECT:
				masterId = "common_error_network_02";
				break;
			case SocketError.SERVER_ERROR:
				masterId = "common_error_network_03";
				break;
		}

		return masterId;
	}

	#endregion
}