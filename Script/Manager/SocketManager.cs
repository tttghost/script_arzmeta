using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using CryptoWebRequestSample;
using Cysharp.Threading.Tasks;
using FrameWork.Network;
using FrameWork.UI;
using Gpm.Ui.Sample;
using Newtonsoft.Json;
using System;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using System.Linq;

namespace FrameWork.Socket
{
    public class SocketManager : Singleton<SocketManager>
    {
        public BestHTTP.SocketIO3.SocketManager socketManager { get; private set; }
        private bool isConnected = false;
        private string tempScreen;
        private string tempBanner;

        /// <summary>
        /// 최초 로그인 시, 1회 연결
        /// </summary>
        public async void SocketIO3Connect()
        {
            await ConnectSocket();

            AddListener();
        }

        private async UniTask ConnectSocket()
        {
            SocketOptions socketOptions = new SocketOptions();
            socketOptions.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
            socketOptions.AutoConnect = false;

            socketOptions.Auth = (manager, socket) => new
            {
                jwtAccessToken = ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken),
                sessionId = ClsCrypto.EncryptByAES(LocalPlayerData.SessionID),
            };

            socketManager = new BestHTTP.SocketIO3.SocketManager(new Uri(Single.Web.WebSocketUrl), socketOptions);

            await UniTask.SwitchToThreadPool();
            if (socketManager.State == BestHTTP.SocketIO3.SocketManager.States.Open)
                socketManager.Close();

            socketManager.Open();
            await UniTask.SwitchToMainThread();
        }

        private void AddListener()
        {
            #region Basic
            socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
            socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Disconnect, Disconnected);
            socketManager.Socket.On<string>("S_PlayerConnected", (userInfo) => S_PlayerConnected(userInfo));
            socketManager.Socket.On<string>("S_DropPlayer", (message) => S_DropPlayer(message));
            #endregion

            #region Chatting
            socketManager.GetSocket("/chatting").On<string>("S_ChatMessage", (message) => S_ChatMessage(message));
            socketManager.GetSocket("/chatting").On<string>("S_ChatDirectMessage", (message) => S_ChatDM(message));
            socketManager.GetSocket("/chatting").On<string>("S_SystemMessage", (message) => S_ChatSystem(message));
            #endregion

            #region Screen & Banner
            socketManager.Socket.On<string>("S_ScreenList", (msg) =>
            {
                if (msg == tempScreen)
                {
                    return;
                }
                //DEBUG.LOG("screenList : " + msg, eColorManager.MediaPlayer);
                tempScreen = msg;
                LocalPlayerData.ScreenInfo = JsonConvert.DeserializeObject<_ScreenInfo[]>(msg, new CustomJsonConverter_Screen());
                switch (SceneLogic.instance.GetSceneType())
                {
                    case SceneName.Scene_Land_Arz:
                    case SceneName.Scene_Land_Busan:
                    case SceneName.Scene_Zone_Office:
                    case SceneName.Scene_Zone_Store:
                    case SceneName.Scene_Zone_Conference:
                    case SceneName.Scene_Zone_Festival:
                    case SceneName.Scene_Zone_Exposition:
                        Test_Screen(LocalPlayerData.ScreenInfo);
                        break;
                }
            });

            socketManager.Socket.On<string>("S_BannerList", (msg) =>
            {
                if (msg == tempBanner)
                {
                    return;
                }
                //DEBUG.LOG("bannerList : " + msg);
                tempBanner = msg;
                LocalPlayerData.BannerInfo = JsonConvert.DeserializeObject<_BannerInfo[]>(msg, new CustomJsonConverter_Banner());
                switch (SceneLogic.instance.GetSceneType())
                {
                    case SceneName.Scene_Land_Arz:
                    case SceneName.Scene_Zone_Festival:
                    case SceneName.Scene_Zone_Exposition:
                        Test_Banner(LocalPlayerData.BannerInfo);
                        break;
                }
            });
            #endregion

            #region Friend
            socketManager.GetSocket("/friend").On<string>("S_FriendList", S_FriendList);
            socketManager.GetSocket("/friend").On<string>("S_FriendFollow", S_FriendFollow);
            socketManager.GetSocket("/friend").On<string>("S_FriendBring", S_FriendBring);
            #endregion

            #region NFTAvatar
            socketManager.GetSocket("/blockchain").On<string>("S_AvatarDataRefresh", S_AvataInfoJson);
            #endregion
        }
        public float bannerRollingDuration = 1f;
        private void Awake()
        {
            BestHTTP.HTTPManager.Setup();

            // 씬 로드 되기 전에 받은 채팅도 보여주기 위한 핸들러 이벤트 등록
            item_S_ChatDM_Handler += ChatDataHolder;
        }

        private void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.Comma))
            //    socketManager.GetSocket("/chatting").Emit("C_TEST_SYSTEM"); 

            //if (Input.GetKeyDown(KeyCode.Slash))
            //    socketManager.Socket.Emit("C_TEST");
#endif
        }

        #region Basic
        private void OnConnected(ConnectResponse obj)
        {
            Debug.Log("OnConnected");

            isConnected = true;

            // 웹 소켓이 재연결됐을 때 재입장
            if (SceneLogic.instance.isChatEnabled)
            {
                C_EnterChatRoom("C_EnterChatRoom");
            }

        }
        private void Disconnected(ConnectResponse obj)
        {
            Debug.Log("Disconnected");

            isConnected = false;

            Util.RunCoroutine(Co_CheckConnect(), nameof(Co_CheckConnect));
        }

        /// <summary>
        /// Disconnect 된 후, 5초 안에 Reconnect가 안될 경우, 에러 팝업 ON & title 씬으로 이동
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> Co_CheckConnect()
        {
            float timer = 0f;

            while (!isConnected && timer < 10f)
            {
                yield return Timing.WaitForOneFrame;
                timer += Time.deltaTime;
            }

            // 아웃월드에서는 팝업안뜨게
            bool isPass = SceneLogic.instance.GetSceneType().ToString().StartsWith("Scene_Base_");
            if (!isConnected && !isPass)
            {
                // 에러코드 100000 : 클라이언트 재연결 시도 후에도 연결이 안될때
                SceneLogic.instance.GetPopup<Popup_Basic>().CheckWebSocketResponseError("100000");
            }
        }

        public void KillConnect()
        {
            Util.KillCoroutine(nameof(Co_CheckConnect));
        }

        /// <summary>
        /// 서버에 최초 접속 시, 정상적으로 접속이 되었을 때 서버에서 클라이언트로 전달해주는 패킷
        /// </summary>
        /// <param name="userInfo"></param>
        private void S_PlayerConnected(string userInfo)
        {
            Debug.Log("Player Connected!!");
            Debug.Log("유저 정보 : " + userInfo);
        }
        /// <summary>
        /// 서버에서 클라이언트로 연결 해제 요청 전달해주는 패킷
        /// </summary>
        /// <param name="message"></param>
        private void S_DropPlayer(string message)
        {
            Debug.Log("S_DropPlayer : " + message);

            // 중복실행 방지
            if (Single.Scene.isSocketLock) return;

            // 씬 전환중이면 다음씬에서!
            if (Single.Scene.isSceneLock) return;

            Util.RunCoroutine(Co_PushPopUp(message), nameof(Co_PushPopUp));
        }

        private IEnumerator<float> Co_PushPopUp(string message)
        {
            Single.Scene.isSocketLock = true;

            yield return Timing.WaitUntilTrue(() => !Single.Scene.isSceneLock);

            SceneLogic.instance.GetPopup<Popup_Basic>().CheckWebSocketResponseError(message);
        }

        /// <summary>
        /// 닉네임 변경되었을 때 웹서버에 알려주기 위한 패킷
        /// </summary>
        public void C_ChangeNickname()
        {
            socketManager.Socket.Emit("C_ChangeNickname");
        }
        #endregion

        #region Chatting

        // 월드 채팅
        public delegate void Item_S_ChatMessage_Handler(Item_S_ChatMessage item_S_ChatMessage);
        public event Item_S_ChatMessage_Handler item_S_ChatMessage_Handler;
        public void C_ChatMessage(Item_C_ChatMessage _message)
        {
            socketManager.GetSocket("/chatting").Emit("C_ChatMessage", _message);
        }
        private void S_ChatMessage(string _message)
        {
            Item_S_ChatMessage Item_S_ChatMessage = JsonConvert.DeserializeObject<Item_S_ChatMessage>(_message);

            //Debug.Log("S_ChatMessage : " + Item_S_ChatMessage.message);

            item_S_ChatMessage_Handler?.Invoke(Item_S_ChatMessage);
        }

        // 1:1 채팅
        public delegate void Item_S_ChatDM_Handler(Item_S_ChatDM item_S_ChatDM);
        public Item_S_ChatDM_Handler item_S_ChatDM_Handler;
        public void C_ChatDM(Item_C_ChatDM _item)
        {
            socketManager.GetSocket("/chatting").Emit("C_ChatDirectMessage", _item);
        }
        private void S_ChatDM(string _message)
        {
            Item_S_ChatDM Item_S_ChatDM = JsonConvert.DeserializeObject<Item_S_ChatDM>(_message);

            //Debug.Log("S_ChatDM : " + Item_S_ChatDM.message);

            item_S_ChatDM_Handler?.Invoke(Item_S_ChatDM);
        }

        /// <summary>
        /// 씬 로드 되기 전에 받은 채팅 저장
        /// </summary>
        /// <param name="item_S_ChatDM"></param>
        public void ChatDataHolder(Item_S_ChatDM item_S_ChatDM)
        {
            LocalPlayerData.Method.allChat.Add(item_S_ChatDM);
            LocalPlayerData.Method.dmChat.Add(item_S_ChatDM);
        }

        // 시스템 채팅
        public delegate void Item_S_ChatSystem_Handler(Item_S_ChatSystem item_S_ChatSystem);
        public event Item_S_ChatSystem_Handler item_S_ChatSystem_Handler;
        private void S_ChatSystem(string _message)
        {
            //Debug.Log("S_SystemMessage : " + _message);

            Item_S_ChatSystem item_S_ChatSystem = new Item_S_ChatSystem() { message = _message, color = Cons.ChatColor_Green, };
            item_S_ChatSystem_Handler?.Invoke(item_S_ChatSystem);
        }

        // 방 입장, 퇴장
        public void C_EnterChatRoom(string connectType)
        {
            RoomInfo roomInfo = new RoomInfo()
            {
                roomId = LocalContentsData.roomId,
                sceneName = SceneLogic.instance.GetSceneType().ToString(),
                roomCode = LocalContentsData.roomCode,
                roomName = LocalContentsData.roomName,
            };

            socketManager.GetSocket("/chatting").Emit(connectType, roomInfo);
            LocalContentsData.roomName = string.Empty;
        }
        public void C_ExitChatRoom()
        {
            socketManager.GetSocket("/chatting").Emit("C_ExitChatRoom");
        }
        #endregion

        #region Screen & Banner
        public void Test_Screen(_ScreenInfo[] screenInfos)
        {
            ScreenComponent[] screenComponents = FindObjectsOfType<ScreenComponent>();
            foreach (var screenComponent in screenComponents)
            {
                screenComponent.SetData(screenInfos);
            }
        }
        public void Test_Banner(_BannerInfo[] bannerInfos)
        {
            BannerComponent[] bannerComponents = FindObjectsOfType<BannerComponent>();
            foreach (var bannerComponent in bannerComponents)
            {
                bannerComponent.SetData(bannerInfos);
            }
        }
        #endregion

        #region Friend
        private FriendFollowInfo targetFriendData;

        #region 친구 목록 가져오기
        public delegate void Item_S_FriendList_Handler(FriendWebSocket[] item_S_FriendList);
        public event Item_S_FriendList_Handler item_S_FriendList_Handler;

        public void C_FriendList()
        {
            socketManager.GetSocket("/friend").Emit("C_FriendList");
        }

        private void S_FriendList(string _message)
        {
            DEBUG.LOG("S_FriendList : " + _message, eColorManager.Web_Response);

            FriendWebSocket[] item_S_FriendList = JsonConvert.DeserializeObject<FriendWebSocket[]>(_message);
            item_S_FriendList_Handler?.Invoke(item_S_FriendList);
        }
        #endregion

        #region 친구 따라가기
        public void C_FriendFollow(string friendMemberId)
        {
            // 차후 따라가기 확인 팝업 추가
            socketManager.GetSocket("/friend").Emit("C_FriendFollow", friendMemberId);
        }

        private void S_FriendFollow(string _message)
        {
            DEBUG.LOG("S_FriendFollow : " + _message, eColorManager.Web_Response);

            try
            {
                targetFriendData = JsonConvert.DeserializeObject<FriendFollowInfo>(_message);

                if (SceneLogic.instance.GetPopup<Popup_Basic>().CheckWebSocketResponseError(targetFriendData.code.ToString())) return;

                CheckRoomIdEnterRoom();
            }
            catch
            {
                OpenErrorPopup_UnableFollow();
            }
        }
        #endregion

        #region 친구 불러오기
        public void C_FriendBring(string friendMemberId, string friendNickname)
        {
            if (!IsCurScene(SceneLogic.instance.GetSceneType(), (MYROOMSTATE_TYPE)LocalPlayerData.MyRoomStateType))
            {
                OpenErrorPopup_UnableBring(friendNickname);
                return;
            }

            OpenToast_FriendCall(friendNickname);
            socketManager.GetSocket("/friend").Emit("C_FriendBring", friendMemberId);
        }

        private void S_FriendBring(string _message)
        {
            DEBUG.LOG("S_FriendBring : " + _message, eColorManager.Web_Response);

            try
            {
                targetFriendData = JsonConvert.DeserializeObject<FriendFollowInfo>(_message);

                if (SceneLogic.instance.GetPopup<Popup_Basic>().CheckWebSocketResponseError(targetFriendData.code.ToString())) return;

                OpenToast_FriendBring();
            }
            catch
            {
                Debug.Log(this.name + " :: S_FriendBring 불러오기 웹소켓 패킷 오류");
            }
        }

        private void OpenToast_FriendCall(string nickname)
        {
            SceneLogic.instance.OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData("friend_notice_call_succeed", nickname)));
        }

        private void OpenToast_FriendBring()
        {
            SceneLogic.instance.OpenToast<Toast_TwoBtn>()
            .ChainToastData(
                new ToastData_TwoBtn(TOASTICON.None, new MasterLocalData("friend_notice_calling", targetFriendData.nickname), 10f,
                new ToastAction(() => SceneLogic.instance.OpenToast<Toast_TwoBtn>().SetCloseEndCallback(() => CheckRoomIdEnterRoom()))));
        }
        #endregion

        #region EnterRoom

        private void CheckRoomIdEnterRoom()
        {
            string curRoomId = targetFriendData.GetRoomId();

            if (!string.IsNullOrEmpty(curRoomId))
            {
                EnterRoom(curRoomId);
            }
            else
            {
                OpenErrorPopup_UnableFollow();
            }
        }

        /// <summary>
        /// RoomId로 서버 이동
        /// </summary>
        /// <param name="_roomId"></param>
        private void EnterRoom(string roomId)
        {
            RealtimeWebManager.AddQuery(Query.roomId, roomId);

            RealtimeWebManager.GetRoom();

            RealtimeWebManager.Run<RoomInfoRes>(JoinRoom);
        }

        /// <summary>
        /// 이동 시 예외처리
        /// </summary>
        /// <param name="_roomInfo"></param>
        private void JoinRoom(RoomInfoRes _roomInfo)
        {
            SceneName curSceneName = Util.String2Enum<SceneName>(_roomInfo.sceneName);

            if (IsSameRoom(curSceneName)) return;

            if (!IsCurScene(curSceneName, (MYROOMSTATE_TYPE)targetFriendData.myRoomStateType))
            {
                OpenErrorPopup_UnableFollow();
                return;
            }
            else
            {
                if (curSceneName == SceneName.Scene_Room_MyRoom)
                {
                    Single.RealTime.EnterRoom(RoomType.MyRoom, targetFriendData.memberCode);
                    SetFollow();
                    return;
                }
            }

            SetFollow();

            Single.RealTime.roomType.target = Util.String2Enum<RoomType>(_roomInfo.roomType);
            Single.RealTime.JoinRoom(_roomInfo);
        }

        /// <summary>
        /// 같은 씬에 있을 시 텔레포트만 실행
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool IsSameRoom(SceneName sceneName)
        {
            if (SceneLogic.instance.GetSceneType() == sceneName)
            {
                // 마이룸에 있을 시 텔레포트 막음
                if (sceneName == SceneName.Scene_Room_MyRoom)
                {
                    SceneLogic.instance.PopPanel(2);
                    return true;
                }

                if (FindObjectOfType<NetworkHandler>() is NetworkHandler networkHandler)
                {
                    if (networkHandler.Players.TryGetValue(targetFriendData.memberCode, out GameObject targetPlayer))
                    {
                        Transform targetTr = targetPlayer.transform;
                        Vector3 pos = Util.RandomSpawn(targetTr, 2f).position;
                        Quaternion angle = Quaternion.Euler(Util.RandomSpawn(targetTr, 2f).eulerAngle);
                        MyPlayer.instance.Teleport_New(pos, angle);

                        ExitTeleport();
                        return true;
                    }
                }
            }
            return false;
        }

        #region Teleport
        /// <summary>
        /// 텔레포트 종료 시 처리
        /// </summary>
        private void ExitTeleport()
        {
            SceneLogic.instance.PopPanel(2);

            Util.RunCoroutine(Co_EnableInput().Delay(1f));
        }

        IEnumerator<float> Co_EnableInput()
        {
            yield return Timing.WaitUntilTrue(() => MyPlayer.instance);

            var interactionHandler = FindObjectOfType<InteractionHandler>();
            if (interactionHandler != null)
            {
                interactionHandler.C_INTERACTION_REMOVE_ITEM(interactionHandler.interactionId);
            }

            ArzMetaManager.Instance.PhoneController.EndInteraction();

            MyPlayer.instance.EnableInput(true);

            SceneLogic.instance.GetPanel<Panel_HUD>().Show(true);
            SceneLogic.instance.GetPanel<Panel_HUD>().Joystick.ShowItem(true);
            SceneLogic.instance.GetPanel<Panel_HUD>().Joystick.left.SetActive(true);

            yield return Timing.WaitUntilTrue(() => SceneLogic.instance.GetPanel<Panel_HUD>().canvasGroup.alpha >= .99f);

            SceneLogic.instance.GetPanel<Panel_HUD>().Joystick.virtualJoystick.GetComponent<UIVirtualJoystick>().enabled = true;
        }
        #endregion

        /// <summary>
        /// 따라가기 및 불려가는 상황 여부 및 멤버코드 저장
        /// </summary>
        private void SetFollow()
        {
            SceneLogic.instance.spawnType = SceneLogic.SpawnType.Follow;
            LocalPlayerData.Method.followPlayerMemberCode = targetFriendData.memberCode;
        }
        #endregion

        /// <summary>
        /// 따라가기 불가 팝업
        /// </summary>
        public void OpenErrorPopup_UnableFollow() => OpenErrorPopup("friend_reception_unable_follow", targetFriendData.nickname);

        /// <summary>
        /// 불러오기 불가 팝업
        /// </summary>
        private void OpenErrorPopup_UnableBring(string nickname) => OpenErrorPopup("friend_notice_call_failure", nickname);

        private void OpenErrorPopup(string local, params object[] args)
        {
            SceneLogic.instance.isUILock = false;

            SceneLogic.instance.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData(local, args)));
        }

        private bool IsCurScene(SceneName sceneName, MYROOMSTATE_TYPE myroomType)
        {
            switch (sceneName)
            {
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                case SceneName.Scene_Room_Meeting:
                case SceneName.Scene_Room_Meeting_22Christmas:
                case SceneName.Scene_Room_Meeting_Office:
                case SceneName.Scene_Room_Consulting:
                    return false;
                case SceneName.Scene_Room_MyRoom:
                    switch (myroomType)
                    {
                        case MYROOMSTATE_TYPE.myroom_condition_anyone: return true;
                        default: return false;
                    }
                default: return true;
            }
        }
        #endregion

        #region NFTAvatar
        public delegate void Item_S_AvataInfoJson_Handler();
        public event Item_S_AvataInfoJson_Handler item_S_ChangeInven_Handler;
        public event Item_S_AvataInfoJson_Handler item_S_ChangeAvatarInfo_Handler;

        /// <summary>
        /// 블록체인 데이터(인벤, 코스튬) 변경 시 받음
        /// </summary>
        /// <param name="_message"></param>
        private void S_AvataInfoJson(string _message)
        {
            DEBUG.LOG("S_AvataInfoJson : " + _message, eColorManager.Web_Response);

            try
            {
                Dictionary<string, string> item_S_AvataInfoJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(_message);

                if (item_S_AvataInfoJson.TryGetValue("memberAvatarPartsItemInven", out string _invenData))
                {
                    var inven = JsonConvert.DeserializeObject<MemberAvatarPartsItemInven[]>(_invenData);
                    if (inven != null)
                    {
                        LocalPlayerData.AvatarPartsInvens = inven.Select(x => new AvatarPartsInvens { itemId = x.itemId }).ToArray();

                        item_S_ChangeInven_Handler?.Invoke();
                    }
                }

                if (item_S_AvataInfoJson.TryGetValue("memberAvatarInfo", out string _avatarData))
                {
                    if (!string.IsNullOrEmpty(_avatarData))
                    {
                        var info = JsonConvert.DeserializeObject<MemberAvatarInfo[]>(_avatarData);
                        if (info != null
                            && info.Length > 0)
                        {
                            var infoDic = info.ToDictionary(x => x.avatarPartsType.ToString(), x => x.itemId);

                            // 서버와 데이터가 같은지 검증
                            var originAvatarDataDic = LocalPlayerData.AvatarInfo.OrderBy(x => x.Value);
                            var changeAvatarDataDic = infoDic.OrderBy(x => x.Value);

                            string originAvatarData = JsonConvert.SerializeObject(originAvatarDataDic);
                            string changeAvatarData = JsonConvert.SerializeObject(changeAvatarDataDic);

                            // 데이터가 다를 시 서버측 데이터로 싱크
                            if (!originAvatarData.Equals(changeAvatarData))
                            {
                                SetAvatar(infoDic);
                            }
                            return;
                        }
                    }

                    SetAvatar(Single.ItemData.GetAvatarResetInfo());
                }
            }
            catch
            {
                Debug.Log("S_AvataInfoJson error ::");
            }
        }

        private void SetAvatar(Dictionary<string, int> data)
        {
            Single.Web.member.Avatar(data, (res) =>
            {
                LocalPlayerData.AvatarInfo = res.avatarInfos.ToDictionary(x => x.Key, x => x.Value);
                Util.SendAvatarInfoRealTime(LocalPlayerData.AvatarInfo);
                SceneLogic.instance.GetPanel<Panel_HUD>().viewHudTopLeft.ViewSetting();

                item_S_ChangeAvatarInfo_Handler?.Invoke();
            });
        }
        #endregion
    }

    public class RoomInfo
    {
        public string roomId;
        public string sceneName;
        public string roomCode;
        public string roomName;
    }

    public class FriendFollowInfo : RoomInfo
    {
        public int code;
        public string memberCode;
        public int myRoomStateType;
        public string nickname;

        public string GetRoomId()
        {
            if (!string.IsNullOrEmpty(roomId))
            {
                return roomId.Split(':')[1];
            }
            return null;
        }
    }

    public class MemberAvatarPartsItemInven
    {
        public string memberId;
        public int itemId;
        public int itemType;
        public string createdAt;
        public string updatedAt;
    }

    public class MemberAvatarInfo
    {
        public string memberId;
        public int avatarPartsType;
        public int itemId;
        public string createdAt;
        public string updatedAt;
    }
}
