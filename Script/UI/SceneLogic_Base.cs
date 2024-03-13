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


        // 빌드 버전, 코드 등등
        public static CustomBuildSetting projSetting { get; private set; }


        //채팅 기능을 사용할 수 있는 곳인지 아닌지
        public bool isChatEnabled { get; set; } = false;


        //실 스크린 가로/세로 길이 추출
        public float screenWidth { get; private set; }
        public float screenHeight { get; private set; }


        //아바타 썸네일
        public AvatarPartsController thumbnailAvatar { get; private set; }

        #endregion


        #region 함수
 
        #region 마스터테이블 로컬라이징
        public delegate void LocalizingEventHandler();

        public LocalizingEventHandler localizingEventHandler;

        /// <summary>
        /// 언어설정 변경시 갱신
        /// </summary>
        public void RefreshMasterLocalizing()
        {
            localizingEventHandler?.Invoke();
        }
        #endregion

        #region Awake
        private void Base_Awake()
        {
            DEBUG.LOG($"┏━━━━━ {SceneManager.GetActiveScene().name} scene loaded ! ! ! ━━━━━┓", eColorManager.SCENE);

            instance = this;

            GPresto.Protector.Engine.GPrestoEngine.Start();

            // 코드게이트, 아즈메타 프로젝트 스위칭
            if (projSetting == null)
            {
                projSetting = CustomBuildSetting.Load();
            }

        }

        #endregion

        #region Start
        private void Base_Start()
        {
            InitScreenSize();
            InitBGM();
            InitChat();
            InitThumbnailAvatar();
            InitEditmode();
            InitEventMode();

            GamePotManager.ShowEvent_Scene();
        }



        /// <summary>
        /// 스크린사이즈 구하기
        /// </summary>
        private void InitScreenSize()
        {
            RectTransform canvasRectTransform = go_UI.GetComponent<RectTransform>();
            screenWidth = canvasRectTransform.sizeDelta.x;
            screenHeight = canvasRectTransform.sizeDelta.y;
        }



        /// <summary>
        /// 씬별 BGM 초기화
        /// </summary>
        private void InitBGM()
        {

            string bgmName = string.Empty;
            float bgmVolume = 0.8f;

            switch (GetSceneType())
            {
                case SceneName.Scene_Base_Patch:
                case SceneName.Scene_Base_Title:
                case SceneName.Scene_Base_Lobby:
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                    bgmName = "BGM_world_00";
                    break;
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                case SceneName.Scene_Zone_Game:
                    bgmName = "bgm_game_0";
                    bgmVolume = 1f;
                    break;
                case SceneName.Scene_Zone_Store:
                    bgmName = "BGM_StoreConnecity_00";
                    break;
                case SceneName.Scene_Room_MyRoom:
                    bgmName = "bgm_myroom";
                    break;
                case SceneName.Scene_Zone_Vote:
                    bgmName = "bgm_votehall_0";
                    break;
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Exposition: //임시로 페스티벌 BGM 사용
                    bgmName = "bgm_festivalzone_01_FeelGood";
                    break;
            }

            if (bgmName != string.Empty)
            {
                Single.Sound.PlayBGM(bgmName, bgmVolume);
            }
            else
            {
                Single.Sound.StopBGM();
            }
        }



        /// <summary>
        /// 씬별 채팅 초기화
        /// </summary>
        private void InitChat()
        {
            var sceneType = GetSceneType();

            // TODO : 필요시 씬 추가해야 함
            switch (sceneType)
            {
                case SceneName.Scene_Base_Loading:
                case SceneName.Scene_Base_Lobby:
                case SceneName.Scene_Base_Logo:
                case SceneName.Scene_Base_Patch:
                case SceneName.Scene_Base_Title:
                    isChatEnabled = false;
                    break;
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Room_Consulting:
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                case SceneName.Scene_Room_Meeting:
                case SceneName.Scene_Room_Meeting_22Christmas:
                case SceneName.Scene_Room_Meeting_Office:
                case SceneName.Scene_Room_Exposition_Booth:
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Exposition:
                // 채팅 기능을 사용하지는 않지만 웹서버에 현재 접속 씬이름을 넘기기 위해 -> hud에서 채팅 UI 자체가 안뜸
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                default:
                    isChatEnabled = true;
                    break;
            }
        }



        /// <summary>
        /// 씬별 썸네일아바타 초기화
        /// </summary>
        private void InitThumbnailAvatar()
        {
            var sceneType = GetSceneType();

            switch (sceneType)
            {
                case SceneName.Scene_Base_Logo:
                case SceneName.Scene_Base_Patch:
                case SceneName.Scene_Base_Title:
                case SceneName.Scene_Base_Loading:
                case SceneName.Scene_Base_Lobby:
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                    thumbnailAvatar = null;
                    break;
                default:
                    GameObject go_ThumbnailAvatar = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_GameObject + nameof(go_ThumbnailAvatar));
                    thumbnailAvatar = go_ThumbnailAvatar.GetComponent<AvatarPartsController>();
                    break;
            }
        }



        /// <summary>
        /// 씬별 에딧모드 초기화
        /// </summary>
        private void InitEditmode()
        {
#if UNITY_EDITOR
            Panel_Editmode panel_Editmode = GetPanel<Panel_Editmode>();
            if (panel_Editmode == null)
            {
                panel_Editmode = Single.Resources.Instantiate<Panel_Editmode>(Cons.Path_Prefab_UI_Panel + typeof(Panel_Editmode).Name, _panelRoot);
            }
            panel_Editmode.OpenPanel<Panel_Editmode>();
            panel_Editmode.transform.SetAsFirstSibling();
#endif
        }



        #region Init Event
        /// <summary>
        /// 이벤트모드 (ex 앤디워홀...)
        /// </summary>
        private void InitEventMode()
        {
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                    Event_Andywarhol();
                    Event_Taekwondo();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 앤디워홀 이벤트
        /// </summary>
        private async void Event_Andywarhol()
        {
            var v = LocalPlayerData.OnfContentsInfos.First(x => (ONFCONTENTS_TYPE)x.onfContentsType == ONFCONTENTS_TYPE.AndyWarhol);

            GameObject[] gos = FindObjectsOfType<GameObject>().Where(x => x.name.Contains("BG_KJ_Banner_AndyWarhol_02")).ToArray();
            foreach (var go in gos)
            {
                if (!Convert.ToBoolean(v.isOn))
                {
                    go.SetActive(false);
                    continue;
                }
                //touchInteractable
                TouchInteractable touchInteractable = go.GetComponent<TouchInteractable>();
                touchInteractable.AddEvent(() =>
                {
                    PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("event_confirm_move_andywarholgallery")))
                    .ChainPopupAction(new PopupAction(() =>
                    {
                        isUILock = false;
                        PushPanel<Panel_Empty>().SetOpenStartCallback(() =>
                        {
                            Overlay_Andywarhol overlay_Andywarhol = Single.Resources.Instantiate<Overlay_Andywarhol>(Cons.Path_Prefab_UI + $"Overlay/{typeof(Overlay_Andywarhol).Name}");
                            overlay_Andywarhol.InitialUrl = "https://manos.kr/andywarhol/";

                            GetPanel<Panel_Empty>()
                            .SetCloseEndCallback(() => Destroy(overlay_Andywarhol.gameObject))
                            .BackAction_Custom = (() => PopPanel());
                        });
                    }));
                });

                // outline test. cwj
                go.GetComponent<OutlineInteracter>().AddTouchEvent(() =>
                {
                    PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("event_confirm_move_andywarholgallery")))
                    .ChainPopupAction(new PopupAction(() =>
                    {
                        PushPanel<Panel_Empty>().SetOpenStartCallback(() =>
                        {
                            Overlay_Andywarhol overlay_Andywarhol = Single.Resources.Instantiate<Overlay_Andywarhol>(Cons.Path_Prefab_UI + $"Overlay/{typeof(Overlay_Andywarhol).Name}");
                            overlay_Andywarhol.InitialUrl = "https://manos.kr/andywarhol/";
                            GetPanel<Panel_Empty>()
                            .SetCloseEndCallback(() => Destroy(overlay_Andywarhol.gameObject))
                            .BackAction_Custom = (() => PopPanel());
                        });
                    }));
                });

                //interactionArea
                GameObject dummy = new GameObject("InteractionArea");
                dummy.transform.SetParent(go.transform, false);
                FanCylinderCollider fanCylinderCollider = dummy.AddComponent<FanCylinderCollider>();
                fanCylinderCollider.ReCreate(10f, 4f, 360);

                await UniTask.WaitUntil(() => dummy.transform.GetComponent<MeshCollider>() != null);
                MeshCollider meshCollider = dummy.transform.GetComponent<MeshCollider>();
                meshCollider.convex = true;
                meshCollider.isTrigger = true;

                InteractionArea interactionArea = dummy.AddComponent<InteractionArea>();
            }
        }

        /// <summary>
        /// 태권도 이벤트
        /// </summary>
        private void Event_Taekwondo()
        {
            var v = LocalPlayerData.OnfContentsInfos.FirstOrDefault(x => (ONFCONTENTS_TYPE)x.onfContentsType == ONFCONTENTS_TYPE.Taekwondo);

            if (v == null) return;

            GameObject go = FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name == "FV_dy_TKstage");
            if (go != null)
            {
                go.SetActive(Convert.ToBoolean(v.isOn));
            }
        }
        #endregion






        #endregion

        private void Base_OnApplicationQuit()
        {
            AppGlobalSettings.Instance.SaveData();
        }

        private void Base_OnApplicationPause(bool isPause)
        {
            if (isPause)
            {
                AppGlobalSettings.Instance.SaveData();
            }
        }
        #endregion
    }
}
///// <summary>
        ///// 씬별 NPC 초기화
        ///// </summary>
        //private void InitNPC()
        //{
        //    var sceneType = GetSceneType();

        //    switch (sceneType)
        //    {
        //        case SceneName.Scene_Zone_Conference:
        //        case SceneName.Scene_Zone_Game:
        //        case SceneName.Scene_Land_Arz:
        //            CreateNPC();
        //            break;
        //        default:
        //            break;
        //    }
        //}



        ///// <summary>
        ///// 엔피씨 생성
        ///// </summary>
        //private void CreateNPC()
        //{
        //    GameObject go = new GameObject();
        //    NPCManager nPCManager = go.AddComponent<NPCManager>();
        //    nPCManager.name = typeof(NPCManager).Name;
        //}


        //private void InitCache()
        //{
        //    switch (GetSceneType())
        //    {
        //        case SceneName.Scene_Land_Arz:
        //            //CacheManager.instance.SetCache<NetworkHandler>();
        //            //CacheManager.instance.SetCache<InteractionHandler>();
        //            //CacheManager.instance.SetCache<ChatModuleHandler>();


        //            //CacheManager.instance.SetCache<Cache_RTView>();
        //            //CacheManager.instance.GetCache<Cache_RTView>();
        //            break;
        //        case SceneName.Scene_Land_Busan:
        //            break;
        //        case SceneName.Scene_Room_JumpingMatching:
        //            break;
        //        case SceneName.Scene_Room_OXQuiz:
        //            break;
        //        case SceneName.Scene_Room_Lecture:
        //        case SceneName.Scene_Room_Lecture_22Christmas:
        //            CacheManager.instance.SetCache<OfficeVirtualCamera>();
        //            break;
        //        case SceneName.Scene_Room_Meeting:
        //            break;
        //        case SceneName.Scene_Room_Meeting_22Christmas:
        //            break;
        //        case SceneName.Scene_Room_Meeting_Office:
        //            break;
        //        case SceneName.Scene_Room_Consulting:
        //            break;
        //        case SceneName.Scene_Room_MyRoom:
        //            break;
        //        case SceneName.Scene_Zone_Conference:
        //            break;
        //        case SceneName.Scene_Zone_Game:
        //            break;
        //        case SceneName.Scene_Zone_Office:
        //            break;
        //        case SceneName.Scene_Zone_Store:
        //            break;
        //        case SceneName.Scene_Zone_Vote:
        //            break;
        //        case SceneName.Scene_Zone_Festival:
        //            break;
        //        case SceneName.Scene_Zone_Hospital:
        //            break;
        //        default:
        //            break;
        //    }
        //}