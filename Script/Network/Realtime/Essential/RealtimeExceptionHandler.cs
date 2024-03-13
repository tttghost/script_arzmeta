using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using MEC;
using UnityEngine.Events;
using System;

namespace FrameWork.Network
{
	public enum PacketError
	{
		SERVER_CHANGE = -1,
		ROOM_IS_FULL = 5006,
		ROON_IS_SHUTDOWN = 216,
		DUPLICATE = 3049,
		WRONG_ROOM_ID = 80000,
		PASSWORD_FAIL = 60007,
		KICKED = 1178,
		WAITING_REJECTED = -100,
		CLOSING = 1161,
	}

	public enum SocketError
	{
		NETWORK_ERROR = 80000,
		NETWORK_RECONNECT = 80001,
		SERVER_ERROR = 80002,
	}

	public class RealtimeExceptionHandler
	{
		#region Members

		public static bool isReconnect = false;

		public static UnityAction callback_error;
		public static UnityAction callback_confirm;
		public static UnityAction callback_cancel;

		public static Queue<System.Net.Sockets.SocketError> socketError = new Queue<System.Net.Sockets.SocketError>();

		#endregion



		#region Core Methods

		public static void AddSocketError(System.Net.Sockets.SocketError _error)
		{
			SocketErrorException(_error);

			Single.RealTime.connection.current.connectState = eConnectState.ERROR;

			DEBUG.LOG($"socketError : {_error}", eColorManager.REALTIME);
		}

		public static void AddPacketError(string _error)
		{
			var error = (PacketError)Enum.Parse(typeof(PacketError), _error);

			ErrorPopup(error);

			DEBUG.LOG($"PacketError : {_error}", eColorManager.REALTIME);
		}

		public static void SocketErrorException(System.Net.Sockets.SocketError _error)
		{
			switch (_error)
			{
				case System.Net.Sockets.SocketError.Success:
                case System.Net.Sockets.SocketError.Disconnecting:
                    break;

                case System.Net.Sockets.SocketError.Shutdown:

					//중복실행 방지
					if (Single.Scene.isSocketLock) return;
					Single.Scene.isSocketLock = true;

					//씬 전환중이면 다음씬에서!
					if (Single.Scene.isSceneLock) return;
                    UnityAction confirm = () => Util.ReturnToLogin();
                    ErrorPopup(SocketError.SERVER_ERROR, confirm);

                    break;

                case System.Net.Sockets.SocketError.ConnectionRefused:
                    ErrorPopup(SocketError.SERVER_ERROR);
                    break;

                case System.Net.Sockets.SocketError.SocketError:
                    Reconnect();
                    break;

                case System.Net.Sockets.SocketError.AccessDenied:                              // 지정된 액세스 권한에서 허용하지 않는 방식으로 Socket에 액세스하려고 시도했습니다.
                case System.Net.Sockets.SocketError.AddressAlreadyInUse:                       // 일반적으로 같은 주소는 한 번만 사용할 수 있습니다.
				case System.Net.Sockets.SocketError.AddressFamilyNotSupported:                 // 지정된 주소 패밀리가 지원되지 않습니다.
				case System.Net.Sockets.SocketError.AlreadyInProgress:                         // 비블로킹 Socket 작업이 이미 진행 중입니다.
				case System.Net.Sockets.SocketError.ConnectionAborted:                         // .NET 또는 내부 소켓 공급자에 의해 연결이 끊어졌습니다.
				case System.Net.Sockets.SocketError.AddressNotAvailable:                       // 선택한 IP 주소가 이 컨텍스트에서 유효하지 않습니다. // 포트가 0일 때,  
				case System.Net.Sockets.SocketError.NetworkUnreachable:                        // 원격 호스트의 경로가 존재하지 않습니다.
				case System.Net.Sockets.SocketError.TimedOut:                                  // 연결 시도 제한 시간이 초과되었거나 연결된 호스트에서 응답하지 않습니다.
				case System.Net.Sockets.SocketError.DestinationAddressRequired:                // Socket 작업에 필수 주소가 누락되었습니다.
				case System.Net.Sockets.SocketError.Fault:                                     // 내부 소켓 공급자에서 잘못된 포인터 주소를 발견했습니다.
				case System.Net.Sockets.SocketError.HostDown:                                  // 원격 호스트가 다운되어 작업이 실패했습니다.
				case System.Net.Sockets.SocketError.HostNotFound:                              // 호스트를 확인할 수 없습니다. 이름이 공식 호스트 이름 또는 별칭이 아닙니다.
				case System.Net.Sockets.SocketError.HostUnreachable:                           // 지정된 호스트에 대한 네트워크 경로가 존재하지 않습니다.
				case System.Net.Sockets.SocketError.InProgress:                                // 블로킹 작업이 진행 중입니다.
				case System.Net.Sockets.SocketError.Interrupted:                               // 블로킹 Socket 호출이 취소되었습니다.
				case System.Net.Sockets.SocketError.InvalidArgument:                           // Socket 멤버에 잘못된 인수를 지정했습니다.
				case System.Net.Sockets.SocketError.IOPending:                                 // 애플리케이션에서 즉시 완료할 수 없는 겹쳐진 작업을 시작했습니다.
				case System.Net.Sockets.SocketError.IsConnected:                               // Socket이 이미 연결되어 있습니다.
				case System.Net.Sockets.SocketError.MessageSize:                               // 데이터그램이 너무 깁니다.
				case System.Net.Sockets.SocketError.NetworkDown:                               // 네트워크를 사용할 수 없는 경우
				case System.Net.Sockets.SocketError.NetworkReset:                              // 애플리케이션에서 시간이 초과된 연결에 KeepAlive를 설정하려고 했습니다.
				case System.Net.Sockets.SocketError.NoBufferSpaceAvailable:                    // Socket 작업에 사용할 수 있는 여유 버퍼 공간이 없습니다.
				case System.Net.Sockets.SocketError.NoData:                                    // 요청된 이름 또는 IP 주소를 이름 서버에서 찾을 수 없습니다.
				case System.Net.Sockets.SocketError.NoRecovery:                                // 오류를 복구할 수 없거나 요청된 데이터베이스를 찾을 수 없습니다.
				case System.Net.Sockets.SocketError.NotConnected:                              // Socket이 연결되지 않은 상태로 애플리케이션에서 데이터를 보내고 받으려고 했습니다.
				case System.Net.Sockets.SocketError.NotInitialized:                            // 내부 소켓 공급자가 초기화되지 않았습니다.
				case System.Net.Sockets.SocketError.NotSocket:                                 // 소켓이 아닌 위치에서 Socket 작업을 시도했습니다.
				case System.Net.Sockets.SocketError.OperationAborted:                          // Socket을 닫아서 겹쳐진 작업이 중단되었습니다.
				case System.Net.Sockets.SocketError.OperationNotSupported:                     // 주소 패밀리가 프로토콜 패밀리에서 지원되지 않습니다.
				case System.Net.Sockets.SocketError.ProcessLimit:                              // 내부 소켓 공급자를 사용하는 프로세스가 너무 많습니다.
				case System.Net.Sockets.SocketError.ProtocolFamilyNotSupported:                // 프로토콜 패밀리가 구현되지 않거나 구성되지 않았습니다.
				case System.Net.Sockets.SocketError.ProtocolNotSupported:                      // 프로토콜이 구현되지 않거나 구성되지 않았습니다.
				case System.Net.Sockets.SocketError.ProtocolOption:                            // 알 수 없거나, 잘못되거나, 지원되지 않는 옵션 또는 수준을 Socket에 사용했습니다.
				case System.Net.Sockets.SocketError.ProtocolType:                              // 이 Socket의 프로토콜 형식이 잘못되었습니다.
				case System.Net.Sockets.SocketError.SocketNotSupported:                        // 이 주소 패밀리에서는 지정된 소켓 형식이 지원되지 않습니다.
				case System.Net.Sockets.SocketError.SystemNotReady:                            // 네트워크 하위 시스템을 사용할 수 없습니다.
				case System.Net.Sockets.SocketError.TooManyOpenSockets:                        // 내부 소켓 공급자에 열려 있는 소켓이 너무 많습니다.
				case System.Net.Sockets.SocketError.TryAgain:                                  // 호스트 이름을 확인할 수 없습니다.나중에 다시 시도하십시오.
				case System.Net.Sockets.SocketError.TypeNotFound:                              // 지정된 클래스를 찾을 수 없습니다.
				case System.Net.Sockets.SocketError.VersionNotSupported:                       // 내부 소켓 공급자의 버전이 범위를 벗어났습니다.
				case System.Net.Sockets.SocketError.WouldBlock:                                // 비블로킹 소켓에 대한 작업을 즉시 완료할 수 없습니다.
				case System.Net.Sockets.SocketError.ConnectionReset:                           // 원격 피어가 연결을 다시 설정했습니다.
					break;
			}
		}



		public static void Reconnect() => Timing.RunCoroutine(Co_Reconnect());
		
		private static IEnumerator<float> Co_Reconnect()
		{
			float time = 15f;

			while (time >= 0f)
			{
				DEBUG.LOG(Application.internetReachability.ToString(), eColorManager.REALTIME);

				switch (Application.internetReachability)
				{
					case NetworkReachability.ReachableViaCarrierDataNetwork:
					case NetworkReachability.ReachableViaLocalAreaNetwork:
						DEBUG.LOG(Application.internetReachability.ToString(), eColorManager.REALTIME);
	
						ErrorPopup(SocketError.SERVER_ERROR, () =>
						{
							Single.Scene.FadeOut(1f, _action: () =>
							{
								Single.Socket.socketManager.Close();

								RealtimeUtils.Leave();

								RealtimeUtils.Disconnect();

								Single.Scene.LoadScene(SceneName.Scene_Base_Title);
							});

							Single.Web.member.StopHeartBeat();
						});

						yield break;

					case NetworkReachability.NotReachable:
						break;
				}

				time -= Time.deltaTime;

				yield return Timing.WaitForOneFrame;
			}

			Single.Scene.SetDimOff();
		}

		#endregion



		#region Methods

		public static void ErrorPopup(PacketError _message, UnityAction _confirm = null, UnityAction _cancel = null)
		{
			if (Single.RealTime.EventSystem != null)
			{
				Single.RealTime.EventSystem.SetActive(true);
			}

			var masterId = ((int)_message).ToString();

			if (_message == PacketError.SERVER_CHANGE) return;

			else if (_message == PacketError.WRONG_ROOM_ID)
			{
				var panel = SceneLogic.instance.GetPanel<Panel_Title>();

				if (panel != null) panel.RefreshUI();
			}

			else if (_message == PacketError.WAITING_REJECTED)
			{
				SceneLogic.instance.GetPanel<Panel_Office>().GetView<View_Office_EnterRoom>().ClearRoomCode();

				SceneLogic.instance.PopPanel();

				Single.RealTime.KillConnect();

				masterId = "office_error_entrance_reject";
			}

			Single.Scene.SetDimOff();

			SceneLogic.instance.PushPopup<Popup_Basic>()
				.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData(masterId)))
				.ChainPopupAction(new PopupAction(_confirm, _cancel));

			callback_error?.Invoke();
		}

		public static void ErrorPopup(SocketError _message, UnityAction _confirm = null, UnityAction _cancel = null)
		{
			if (Single.RealTime.EventSystem != null)
			{
				Single.RealTime.EventSystem.SetActive(true);
			}

			Single.Scene.SetDimOff();

			SceneLogic.instance.PushPopup<Popup_Basic>()
				.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData(RealtimeUtils.GetErrorMasterId(_message), (int)_message)))
				.ChainPopupAction(new PopupAction(_confirm, _cancel));

			callback_error?.Invoke();
		}

		#endregion
	}
}