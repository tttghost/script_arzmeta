/****************************************************************************************************
 * 
 *					SceneLogic.cs
 *						- 모든 Scene 의 시작, 종료(Scene 전환) 로직을 담당
 *						- SceneLogic
 *							-> Panel, Popup 관리
 *											-> Child UI(Button, Text...etc)
 * 
 ****************************************************************************************************/

using MEC;
using System;
using System.Collections.Generic;
using Unity.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using FrameWork.Network;
using Debug = UnityEngine.Debug;
using System.Linq;
using CustomPrimitiveColliders;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections;
using FrameWork.Cache;

namespace FrameWork.UI
{
    public partial class SceneLogic : MonoBehaviour
    {
        #region 변수
        public NetworkHandler networkHandler { get; private set; }
        public enum SpawnType
        {
            WayPoint,   //포탈이동
            Follow,     //따라가기
        }
        public SpawnType spawnType { get; set; }

        #endregion


        #region 함수

        /// <summary>
        /// 
        /// </summary>
        private void Network_Awake()
        {
            InitNetworkHandler();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void Network_Start()
        {
            InitChatSocket();
            InitScreenBannerSocket();

            CreatePlayer();
        }

        private void InitNetworkHandler()
        {

            //네트워크핸들러 캐싱
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:

                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                case SceneName.Scene_Room_Meeting:
                case SceneName.Scene_Room_Meeting_22Christmas:
                case SceneName.Scene_Room_Meeting_Office:
                case SceneName.Scene_Room_Consulting:
                case SceneName.Scene_Room_Exposition_Booth:

                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Exposition:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Hospital:
                    networkHandler = FindObjectOfType<NetworkHandler>();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 플레이어 생성
        /// </summary>
        private void CreatePlayer()
        {

            bool isRandomSpawn = default;
            CameraView cameraView = default;

            //랜덤스폰
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                    isRandomSpawn = true;
                    break;
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Exposition:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Hospital:
                    isRandomSpawn = false;
                    break;
            }

            //카메라 정면/후면
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Hospital:
                case SceneName.Scene_Room_Exposition_Booth:
                    cameraView = CameraView.Back;
                    break;
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Exposition:
                case SceneName.Scene_Zone_Conference:
                    cameraView = CameraView.Front;
                    break;
            }

            //플레이어 생성
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Exposition:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Room_Exposition_Booth:
                    Co_CreatePlayer(isRandomSpawn, cameraView).RunCoroutine();
                    break;
                default: //오피스류는 퍼미션 받은 이후 처리해야 하기때문에 생성하지 않음
                    break;
            }

        }

        /// <summary>
        /// 플레이어 생성
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> Co_CreatePlayer(bool isRandomSpawn, CameraView cameraView)
        {
            yield return Timing.WaitForOneFrame;

            networkHandler.GetClient();
            networkHandler.GetObject();

            if (spawnType == SpawnType.Follow)
            {
                cameraView = CameraView.Back;
                //LocalPlayerData.IsPlayerFollow = false;

                yield return Timing.WaitUntilTrue(() => networkHandler.Players.Count != 0);
            }

            Transform playerTarget = GetPlayerTarget();

            PlayerData playerData = new PlayerData();
            playerData = CreatePlayerData(playerTarget.position, playerTarget.eulerAngles, isRandomSpawn, cameraView);

            NetworkPlayer networkPlayer = new NetworkPlayer(playerData); //캐릭터 생성

            yield return Timing.WaitUntilTrue(() => MyPlayer.instance != null);

            OnCompleteCreatePlayer();

            Single.Scene.FadeIn(.5f, _action: Single.Scene.ResetFadeColor);
        }

        /// <summary>
        /// 플레이어타겟(트랜스폼) 가져오기
        /// </summary>
        /// <param name="isRandomSpawn"></param>
        /// <param name="cameraView"></param>
        /// <returns></returns>
        private Transform GetPlayerTarget()
        {
            Transform target = default;

            switch (spawnType)
            {
                case SpawnType.WayPoint:
                    {
                        target = GetWayPoint(LocalContentsData.scenePortal);
                        LocalContentsData.scenePortal = ScenePortal.None;
                    }
                    break;
                case SpawnType.Follow:
                    {
                        if (networkHandler.Players.Count > 0 && networkHandler.Players.ContainsKey(LocalPlayerData.Method.followPlayerMemberCode))
                        {
                            target = networkHandler.Players[LocalPlayerData.Method.followPlayerMemberCode].transform;
                        }
                    }
                    break;
            }
            spawnType = SpawnType.WayPoint;

            if (target == null)
            {
                DEBUG.LOG("타겟 없음 에러!!");
            }

            return target;
        }

        /// <summary>
        /// 웨이포인트 가져오기
        /// </summary>
        /// <param name="scenePortal"></param>
        /// <returns></returns>
        public Transform GetWayPoint(ScenePortal scenePortal)
        {
            WayPoint[] wayPoints = FindObjectsOfType<WayPoint>();
            return wayPoints.SingleOrDefault(x => x.scenePortal == scenePortal).transform;
        }


        /// <summary>
        /// 플레이어데이터 생성
        /// </summary>
        /// <param name="position"></param>
        /// <param name="eulerAngle"></param>
        /// <param name="isRandomSpawn"></param>
        /// <param name="cameraView"></param>
        /// <returns></returns>
        private PlayerData CreatePlayerData(Vector3 position, Vector3 eulerAngle, bool isRandomSpawn, CameraView cameraView)
        {
            PlayerData playerData = new PlayerData
            { 
                position = position,
                eulerAngle = eulerAngle,
                isRandomSpawn = isRandomSpawn,
                cameraView = cameraView,
            };
            return playerData;
        }

        /// <summary>
        /// 플레이어 생성 콜백 (코루틴 마지막부분 함수)
        /// </summary>
        protected virtual void OnCompleteCreatePlayer() { }





        protected void InitChatSocket() => StartCoroutine(Co_InitChatSocket());

        /// <summary>
        /// 웹채팅소켓 초기화 및 방입장
        /// </summary>
        /// <returns></returns>
        private IEnumerator Co_InitChatSocket()
        {
            yield return new WaitUntil(() => Single.Socket);

            // 채팅이 가능한 곳이면 씬 이동될 때, 월드 채팅방 입장
            if (isChatEnabled)
            {
                string chatConnectType = string.Empty;
                if (LocalPlayerData.Method.IsFirst)
                {
                    chatConnectType = "C_EnterChatRoom";
                }
                else
                {
                    chatConnectType = "C_ExitAndEnterChatRoom";
                }
                Single.Socket.C_EnterChatRoom(chatConnectType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected async void InitScreenBannerSocket()
        {
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Exposition:
                    await Task.Delay(500); //배너 검정화면 나오는 이유? 때문에 1초 딜레이...
                    Single.Socket.Test_Banner(LocalPlayerData.BannerInfo);
                    Single.Socket.Test_Screen(LocalPlayerData.ScreenInfo);
                    break;
            }
        }

        
        
        
        
        #endregion


    }
}