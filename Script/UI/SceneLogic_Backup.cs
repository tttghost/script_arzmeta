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

    [DefaultExecutionOrder(-1)] //UIBase 보다 빨리 실행되기 위함
    public class SceneLogic_Backup : MonoBehaviour
    {
        #region 변수


        //싱글톤
        public static SceneLogic_Backup instance = null;

        // 빌드설정 데이터를 보관하고 있는 asset Data


        // 빌드 버전, 코드 등등
        public static CustomBuildSetting projSetting { get; private set; }


        //Panel, Popup, Toast 캐싱
        private Dictionary<string, PanelBase> _dicPanels = new Dictionary<string, PanelBase>();
        private Dictionary<string, PopupBase> _dicPopups = new Dictionary<string, PopupBase>();
        private Dictionary<string, ToastBase> _dicToasts = new Dictionary<string, ToastBase>();


        //Panel, Popup 스택용
        public Stack<PanelBase> _stackPanels = new Stack<PanelBase>();
        public Stack<bool> _stackPanelsAni = new Stack<bool>();
        public Stack<PopupBase> _stackPopups = new Stack<PopupBase>();
        public GameObject go_UI { get; private set; }
        public Canvas canvas { get; private set; }
        public GameObject go_Panel { get; private set; }
        public GameObject go_Popup { get; private set; }
        public Transform _panelRoot { get; private set; }
        public Transform _popupRoot { get; private set; }



        //UI 변동사항 있을때 조작못하게 하는 bool값
        public bool isUILock { get; set; } = false;



        //채팅 기능을 사용할 수 있는 곳인지 아닌지
        public bool isChatEnabled { get; set; } = false;



        //실 스크린 가로/세로 길이 추출
        public float screenWidth { get; private set; }
        public float screenHeight { get; private set; }



        //초기스폰위치
        public Transform CreateSpot { get; private set; }

        // PopPanel 후 호출. 패널 닫으면서 다른 패널 오픈하고자 할때 사용
        public UnityEvent FinishPopPanelEvent = new UnityEvent();
        #endregion


        #region 함수
        #region Awake - Panel, Popup, Toast 찾아서 초기화
        /// <summary>
        /// Scene 시작부
        /// </summary>
        protected virtual void Awake()
        {
            GPresto.Protector.Engine.GPrestoEngine.Start();

            instance = this;

            DEBUG.LOG($"┏━━━━━ {SceneManager.GetActiveScene().name} scene loaded ! ! ! ━━━━━┓", eColorManager.SCENE);

            // 코드게이트, 아즈메타 프로젝트 스위칭
            if (projSetting == null)
            {
                projSetting = CustomBuildSetting.Load();
            }

            InitCache();

            InitSpawnPosition();

            // Panel, Popup 초기화
            Initialize();

            // Panel, Popup 등을 캐싱하기 위해 늦은 타이밍 이니셜라이즈
            InitializeLate();
        }

        /// <summary>
        /// 최초생성포지션
        /// </summary>
        private void InitSpawnPosition()
        {
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Room_Consulting:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Room_Meeting:
                case SceneName.Scene_Room_Meeting_Office:
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Room_Meeting_22Christmas:
                case SceneName.Scene_Room_Lecture_22Christmas:
                case SceneName.Scene_Zone_Office:
                    CreateSpot = gameObject.Child(nameof(CreateSpot))?.transform;
                    break;
                default:
                    break;
            }
        }

        private void InitCache()
        {
            switch (GetSceneType())
            {
                case SceneName.Scene_Land_Arz:
                    //CacheManager.instance.SetCache<NetworkHandler>();
                    //CacheManager.instance.SetCache<InteractionHandler>();
                    //CacheManager.instance.SetCache<ChatModuleHandler>();


                    //CacheManager.instance.SetCache<Cache_RTView>();
                    //CacheManager.instance.GetCache<Cache_RTView>();
                    break;
                case SceneName.Scene_Land_Busan:
                    break;
                case SceneName.Scene_Room_JumpingMatching:
                    break;
                case SceneName.Scene_Room_OXQuiz:
                    break;
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                    CacheManager.instance.SetCache<OfficeVirtualCamera>();
                    break;
                case SceneName.Scene_Room_Meeting:
                    break;
                case SceneName.Scene_Room_Meeting_22Christmas:
                    break;
                case SceneName.Scene_Room_Meeting_Office:
                    break;
                case SceneName.Scene_Room_Consulting:
                    break;
                case SceneName.Scene_Room_MyRoom:
                    break;
                case SceneName.Scene_Zone_Conference:
                    break;
                case SceneName.Scene_Zone_Game:
                    break;
                case SceneName.Scene_Zone_Office:
                    break;
                case SceneName.Scene_Zone_Store:
                    break;
                case SceneName.Scene_Zone_Vote:
                    break;
                case SceneName.Scene_Zone_Festival:
                    break;
                case SceneName.Scene_Zone_Hospital:
                    break;
                default:
                    break;
            }
        }
        private void OnEnable()
        {
            //AppGlobalSettings.Instance.LoadData();
        }
        private void OnDisable()
        {
            //AppGlobalSettings.Instance.SaveData();
        }
        private void OnApplicationPause(bool pauseStatus)
        {
            //if (pauseStatus) AppGlobalSettings.Instance.SaveData();
        }
        private void OnApplicationQuit()
        {
            AppGlobalSettings.Instance.SaveData();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeLate()
        {
            InitializeLate(_dicPanels);
            InitializeLate(_dicPopups);
            InitializeLate(_dicToasts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        private void InitializeLate<T>(Dictionary<string, T> dic) where T : UIBase
        {
            foreach (var item in dic)
            {
                item.Value.InitializeLate();
            }
        }

        /// <summary>
        /// Heierachy 에서, Panel, Popup 을 찾아서 초기화 
        /// </summary>
        private void Initialize()
        {
            go_UI = GameObject.Find("UI_Canvas");
            if (go_UI == null)
            {
                Debug.LogError("SceneLogic.cs> Initialize() - 하이라키에서 UI를 찾을수 없어");
                return;
            }
            canvas = go_UI.GetComponent<Canvas>();
            screenTouchParticle = go_UI.AddComponent<ScreenTouchParticle>();

            _panelRoot = go_UI.transform.Find("Panel");
            go_Panel = _panelRoot.gameObject;
            _popupRoot = go_UI.transform.Find("Popup");
            go_Popup = _popupRoot.gameObject;


            if (_panelRoot == null)
            {
                Debug.LogError("SceneLogic.cs> Initialize() _panelRoot 가 널이야");
                return;
            }

            if (_popupRoot == null)
            {
                Debug.LogError("SceneLogic.cs> Initialize() _popupRoot 가 널이야");
                return;
            }

            Transform child;

            for (int i = 0; i < _panelRoot.childCount; ++i)
            {
                child = _panelRoot.GetChild(i);
                PanelBase panel = child.GetComponent<PanelBase>();
                if (panel != null)
                {
                    //DebugManager.LogManager("panel searched : " + panel.name, eColorManager.UI);
                    if (_dicPanels.ContainsKey(child.name))
                    {
                        DEBUG.LOG("이미 추가된 Panel 임 - " + child.name, eColorManager.UI);
                    }
                    else
                    {
                        DEBUG.LOG(child.name + " Panel ADD", eColorManager.STACK);
                        _dicPanels.Add(child.name, panel);
                    }
                    panel.Initialize();
                }
            }

            for (int i = 0; i < _popupRoot.childCount; ++i)
            {
                child = _popupRoot.GetChild(i);

                // Popup 찾기
                PopupBase popup = child.GetComponent<PopupBase>();
                if (popup != null)
                {
                    //DEBUG.LOG("popup searched : " + popup.name, eColorManager.UI);
                    popup.Initialize();

                    if (_dicPopups.ContainsKey(child.name))
                    {
                        Debug.LogError("SceneLogic.cs> Initialize() : 이미 추가된 Popup 임 - " + child.name);
                    }
                    else
                    {
                        DEBUG.LOG(child.name + " Popup ADD", eColorManager.STACK);
                        _dicPopups.Add(child.name, popup);
                    }
                }

                // Toast 찾기 - 한효주 추가
                ToastBase toast = child.GetComponent<ToastBase>();
                if (toast != null)
                {
                    toast.Initialize();

                    if (_dicToasts.ContainsKey(child.name))
                    {
                        Debug.LogError("SceneLogic.cs> Initialize() : 이미 추가된 Toast 임 - " + child.name);
                    }
                    else
                    {
                        DEBUG.LOG(child.name + " Toast ADD", eColorManager.STACK);
                        _dicToasts.Add(child.name, toast);
                    }
                }
            }
        }
        #endregion



        #region Start - Awake후 초기화 할 것들 (BGM, Chat, NPC...)
        protected virtual void Start()
        {
            screenWidth = go_UI.GetComponent<RectTransform>().sizeDelta.x;
            screenHeight = go_UI.GetComponent<RectTransform>().sizeDelta.y;

            InitBGM();
            InitChat();
            InitHUD();
            InitThumbnailAvatar();

            InitEditmode();
            InitEventMode();
            InitScreenBanner();

            StartCoroutine(Co_InitChatSocket());

            GamePotManager.ShowEvent_Scene();
        }

        /// <summary>
        /// 웹채팅소켓 초기화 및 방입장
        /// </summary>
        /// <returns></returns>
        IEnumerator Co_InitChatSocket()
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
        private async void InitScreenBanner()
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


        public AvatarPartsController thumbnailAvatar { get; private set; }

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
        /// 씬별 HUD 초기화
        /// </summary>
        private void InitHUD()
        {
            var sceneType = GetSceneType();

            switch (sceneType)
            {
                //게임룸만..
                case SceneName.Scene_Room_JumpingMatching:
                case SceneName.Scene_Room_OXQuiz:
                    OpenPanel<Panel_HUD>();
                    OpenPanel<Panel_Game>();
                    break;
                //게임룸을 제외한 랜드, 룸, 존...
                case SceneName.Scene_Land_Arz:
                case SceneName.Scene_Land_Busan:
                case SceneName.Scene_Room_Consulting:
                case SceneName.Scene_Room_Lecture:
                case SceneName.Scene_Room_Lecture_22Christmas:
                case SceneName.Scene_Room_Meeting:
                case SceneName.Scene_Room_Meeting_22Christmas:
                case SceneName.Scene_Room_Meeting_Office:
                case SceneName.Scene_Room_MyRoom:
                case SceneName.Scene_Zone_Game:
                case SceneName.Scene_Zone_Store:
                case SceneName.Scene_Zone_Vote:
                case SceneName.Scene_Zone_Conference:
                case SceneName.Scene_Zone_Office:
                case SceneName.Scene_Zone_Hospital:
                case SceneName.Scene_Zone_Festival:
                case SceneName.Scene_Zone_Exposition:
                case SceneName.Scene_Room_Exposition_Booth:
                    OpenPanel<Panel_HUD>();
                    PushPanel<Panel_Empty>(false);
                    if (GetSceneType() == SceneName.Scene_Room_MyRoom)
                    {
                        OpenPanel<Panel_GetJuri>();
                    }
                    break;
                //아웃월드
                default:
                    break;
            }
        }



        /// <summary>
        /// 씬별 채팅 초기화
        /// </summary>
        private void InitChat()
        {
            var sceneType = GetSceneType();

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



        /// <summary>
        /// 씬타입 추출
        /// </summary>
        /// <returns></returns>
        public SceneName GetSceneType()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            return Util.String2Enum<SceneName>(sceneName);
        }

        #endregion



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



        #region Update - Back키 액션, 언어 변경 등...
        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Back();
            }
        }

        /// <summary>
        /// Back키
        ///같은 타입의 팝업이 있을 경우는 기존
        ///딤패널시에도 Back키 안먹게...
        ///뒤로가기 키 눌렀을때 행동, PC : Esc, Android : Back키
        ///팝업부터 닫고 팝없 없으면 이전 패널로 이동
        /// </summary>
        /// <param name="cnt"> 몇단계 돌아갈지</param>
        public void Back()
        {
            Back(1);
        }
        public void Back(int cnt)
        {
            if (isUILock || Single.Scene.isDim) //패널,팝업 전환 간 락 기능
            {
                return;
            }

            Single.Sound.PlayEffect(Cons.click);

            if (_stackPopups.Count > 0) //팝업이 1개이상 있으면
            {
                PopupBase curPopup = _stackPopups.Peek();
                curPopup.Back(cnt);
            }
            else if (_stackPanels.Count > 1) //패널이 2개이상 있으면
            {
                PanelBase curPanel = _stackPanels.Peek();
                curPanel.Back(cnt);
            }
            else //아무 스택도 없을 때..
            {
                OnClick_Back();
            }
        }

        /// <summary>
        /// 백키 기본종료 액션
        /// </summary>
        public virtual void OnClick_Back()
        {

            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("3012")))
                .ChainPopupAction(new PopupAction(() =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.Exit(0);
#else
                        Application.Quit();
#endif
                }));
        }

        /// <summary>
        /// 언어변경
        /// </summary>
        public void ChangeLanguage()
        {
            AppGlobalSettings.Instance.language = AppGlobalSettings.Instance.language == Language.English ? Language.Korean : Language.English;
        }
        #endregion



        #region Function - 스크린터치 파티클, ...
        #region 스크린터치 파티클
        protected ScreenTouchParticle screenTouchParticle;
        /// <summary>
        /// 스크린터치파티클 활성화/비활성화
        /// </summary>
        /// <param name="enable"></param>
        public void SetScreenTouchParticle(bool enable)
        {
            screenTouchParticle.enabled = enable;
        }
        #endregion
        #endregion



        #region Destroy - 메모리해제
        protected virtual void OnDestroy()
        {
            // 스택을 클리어함으로서 메모리에서 없앤다. - BKK -
            _stackPanels.Clear();
            _stackPanelsAni.Clear();
            _stackPopups.Clear();

            FinishPopPanelEvent = null;
        }
        #endregion
        #endregion


        #region UI
        #region UI Action : UI의 상태 변화에 따른 액션 정의
        private Action curOpenStartAct;  // 현재 UI가 열기 시작할 때
        private Action curOpenEndAct;    // 현재 UI가 열기 종료할 때
        private Action curCloseStartAct; // 현재 UI가 닫기 시작할 때
        private Action curCloseEndAct;   // 현재 UI가 닫기 종료할 때

        //액션 셋팅
        /// <summary>
        /// 오픈을 시작할 때 액션 부여
        /// </summary>
        /// <param name="callback"></param>
        public void SetCurOpenStartCallback(Action callback)
        {
            curOpenStartAct = callback;
        }

        /// <summary>
        /// 오픈을 끝낼 때 액션 부여
        /// </summary>
        /// <param name="callback"></param>
        public void SetCurOpenEndCallback(Action callback)
        {
            curOpenEndAct = callback;
        }

        /// <summary>
        /// 클로즈를 시작할 때 액션 부여
        /// </summary>
        /// <param name="callback"></param>
        public void SetCurCloseStartCallback(Action callback)
        {
            curCloseStartAct = callback;
        }

        /// <summary>
        /// 클로즈를 끝낼 때 액션 부여
        /// </summary>
        /// <param name="callback"></param>
        public void SetCurCloseEndCallback(Action callback)
        {
            curCloseEndAct = callback;
        }

        //액션 실행
        /// <summary>
        /// 오픈을 시작할 때 액션 실행
        /// </summary>
        public void OnCurOpenStartAct()
        {
            curOpenStartAct?.Invoke();
            curOpenStartAct = null;
        }

        /// <summary>
        /// 오픈을 끝낼 때 액션 실행
        /// </summary>
        public void OnCurOpenEndAct()
        {
            curOpenEndAct?.Invoke();
            curOpenEndAct = null;
        }

        /// <summary>
        /// 클로즈를 시작할 때 액션 실행
        /// </summary>
        public void OnCurCloseStartAct()
        {
            curCloseStartAct?.Invoke();
            curCloseStartAct = null;
        }

        /// <summary>
        /// 클로즈를 끝낼 때 액션 실행
        /// </summary>
        public void OnCurCloseEndAct()
        {
            curCloseEndAct?.Invoke();
            curCloseEndAct = null;
        }
        #endregion



        #region Panel : 1. 패널은 항상 1개만 떠 있어야 한다. (특수한 경우 예외상황)
        #region Peek Panel :    패널 가져오기(현재열려있는패널)
        /// <summary>
        /// 현재패널 가져옴
        /// </summary>
        /// <returns></returns>
        public PanelBase PeekPanel()
        {
            return _stackPanels.Peek();
        }
        #endregion

        #region Get Panel :     패널 가져오기(이름, 타입명, 이름+타입명)

        /// <summary>
        /// 이름, 타입명으로 패널 찾기
        /// </summary>
        /// <param name="panelName"> 하이라키 이름</param>
        /// <returns></returns>
        public T GetPanel<T>() where T : PanelBase
        {
            string panelName = typeof(T).Name;
            try
            {
                if (_dicPanels.ContainsKey(panelName) == false)
                {
                    Debug.LogWarning("PanelManager.cs> GetPanel() is null - panelName = " + panelName);
                    return null;
                }

                return (T)_dicPanels[panelName];
            }
            catch
            {
                Debug.LogWarning("GetPanel Exception, PanelName : " + panelName);
                return null;
            }
        }

        /// <summary>
        /// 이전 패널 가져오기
        /// </summary>
        public PanelBase GetPrevPanel()
        {
            if (_stackPanels.Count < 2)
            {
                Debug.LogWarning("Panel Stack이 2개 미만입니다.");
                return null;
            }

            return _stackPanels.ToArray()[_stackPanels.Count - 2];
        }
        #endregion

        #region Push Panel :    패널 열기(스택+)

        /// <summary>
        /// 타입으로 패널 푸쉬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        public T PushPanel<T>(bool isShowAnimation = true) where T : PanelBase
        {
            if (!isUILock)
            {
                Co_PushPanel<T>(isShowAnimation).RunCoroutine();
            }
            return GetPanel<T>();
        }

        /// <summary>
        /// 패널로 코루틴 푸쉬패널
        /// </summary>
        /// <param name="panelBase"></param>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        public IEnumerator<float> Co_PushPanel<T>(bool isShowAnimation = true) where T : PanelBase
        {
            DEBUG.LOG("PanelBase : " + typeof(T).Name, eColorManager.UI);

            isUILock = true;

            if (_stackPanels.Count > 0) //애니메이션
            {
                yield return _stackPanels.Peek().Co_ClosePanel(isShowAnimation).WaitUntilDone();
            }

            StackPanel<T>(isShowAnimation); //스택

            yield return _stackPanels.Peek().Co_OpenPanel(isShowAnimation).WaitUntilDone(); //애니메이션

            isUILock = false;
        }

        /// <summary>
        /// 푸쉬패널 디버그 출력
        /// </summary>
        private void Debug_PushPanel()
        {
            string panelName = _stackPanels.Peek().name;
            int cnt = _stackPanels.Count;
            DEBUG.LOG($"Push Panel : {panelName}, Panel Stack : {cnt - 1} -> {cnt}", eColorManager.STACK);
        }
        #endregion

        #region Pop Panel :     패널 닫기(스택-)
        /// <summary>
        /// 팝 패널
        /// </summary>
        /// <param name="isShowAnimation"></param>
        public void PopPanel(int cnt = 1)
        {
            if (isUILock) return;
            Co_PopPanel(cnt).RunCoroutine();
        }

        /// <summary>
        /// 팝패널 코루틴
        /// </summary>
        /// <returns></returns>
        public IEnumerator<float> Co_PopPanel(int cnt = 1)
        {
            isUILock = true;

            bool isShowAnimation = _stackPanelsAni.Peek();

            yield return _stackPanels.Peek().Co_ClosePanel(isShowAnimation).WaitUntilDone(); //애니메이션

            for (int i = 0; i < cnt; i++)
            {
                UnStackPanel(true); //언스택
            }

            if (_stackPanels.Count > 0)
            {
                yield return _stackPanels.Peek().Co_OpenPanel(isShowAnimation).WaitUntilDone(); //애니메이션
            }

            isUILock = false;

            if (FinishPopPanelEvent != null)
            {
                FinishPopPanelEvent.Invoke();
                FinishPopPanelEvent.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 팝패널 디버그 출력
        /// </summary>
        private void Debug_PopPanel()
        {
            string panelName = _stackPanels.Count > 0 ? _stackPanels.Peek().name : "Panel Empty";
            int cnt = _stackPanels.Count;
            DEBUG.LOG($"Pop Panel : {panelName}, Panel Stack : {cnt + 1} -> {cnt}", eColorManager.STACK);
        }
        #endregion

        #region Stack Panel :   스택 증가 (스택+, UI연출 선택사항)

        /// <summary>
        /// 패널로 스택쌓기
        /// </summary>
        /// <param name="newPanelBase">추가패널</param>
        /// <param name="isShowAnimationNewPanel">추가패널 애니메이션 여부 - 기본 true</param>
        /// <param name="curPanelShow">기존패널 열어줄지 닫아줄지 - 기본 false</param>
        /// <param name="newPanelShow">추가패널 열어줄지 닫아줄지 - 기본 true</param>
        public void StackPanel<T>(bool isShowAnimationNewPanel = true, bool curPanelShow = false, bool newPanelShow = true) where T : PanelBase
        {
            PanelBase newPanelBase = GetPanel<T>();

            DEBUG.LOG("_stackPanels.Count : " + _stackPanels.Count, eColorManager.STACK);
            if (_stackPanels.Count > 0)
            {
                _stackPanels.Peek().gameObject.SetActive(curPanelShow);
            }

            _stackPanels.Push(newPanelBase);
            _stackPanelsAni.Push(isShowAnimationNewPanel);
            DEBUG.LOG("_stackPanels.Peek().gameObject : " + _stackPanels.Peek().gameObject.name, eColorManager.STACK);
            _stackPanels.Peek().gameObject.SetActive(newPanelShow);

            Debug_PushPanel();
        }


        #endregion

        #region UnStack Panel : 스택 감소(스택-, UI연출 선택사항)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldPanel"></param>
        /// <param name="curPanel"></param>
        private void UnStackPanel(bool curPanel = true)
        {
            if (!_stackPanels.Peek().dontSetActiveFalse) _stackPanels.Peek().gameObject.SetActive(false);

            _stackPanels.Pop();
            _stackPanelsAni.Pop();

            if (_stackPanels.Count > 0)
            {
                _stackPanels.Peek().gameObject.SetActive(curPanel); //기존패널 열어줄지 닫아줄지
            }

            Debug_PopPanel();
        }
        #endregion

        #region Open Panel :    패널 열기(스택 그대로)
        public T OpenPanel<T>() where T : PanelBase
        {
            T panelBase = GetPanel<T>();
            if (panelBase) panelBase.gameObject.SetActive(true);
            return panelBase;
        }
        public T ShowPanel<T>() where T : PanelBase
        {
            T panelBase = GetPanel<T>();
            panelBase.Show(true);
            return panelBase;
        }
        public T HidePanel<T>() where T : PanelBase
        {
            T panelBase = GetPanel<T>();

            panelBase.Show(false);
            return panelBase;
        }
        #endregion

        #region Close Panel :   패널 닫기(스택 그대로)
        public T ClosePanel<T>() where T : PanelBase
        {
            T panelBase = GetPanel<T>();
            if (!panelBase.dontSetActiveFalse) panelBase.gameObject.SetActive(false);
            return panelBase;
        }
        #endregion

        #region Swap Panel :    현재패널 닫고 추가패널 열기(스택 그대로)	
        /// <summary>
        /// 현재 떠있는 패널을 닫으면서 새로운 패널 열어줌
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="isShowAnimation"></param>
        public T SwapPanel<T>(bool isShowAnimation = true) where T : PanelBase
        {
            if (!isUILock)
            {
                Co_SwapPanel<T>(isShowAnimation).RunCoroutine();
            }
            return GetPanel<T>();
        }

        /// <summary>
        /// 패널 스왑
        /// </summary>
        /// <param name="panelBase"></param>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        private IEnumerator<float> Co_SwapPanel<T>(bool isShowAnimation = true) where T : PanelBase
        {
            isUILock = true;

            if (_stackPanels.Count > 0)
            {
                yield return _stackPanels.Peek().Co_ClosePanel(isShowAnimation).WaitUntilDone(); //애니메이션
            }

            UnStackPanel(false); //언스택
            StackPanel<T>(isShowAnimation); //스택

            yield return _stackPanels.Peek().Co_OpenPanel(isShowAnimation).WaitUntilDone(); //애니메이션

            isUILock = false;
        }
        #endregion
        #endregion



        #region Popup : 1. 팝업은 여러개 뜰 수 있다.  /  2. 팝업위에 패널이 뜰 수 없다.  /  3. 같은팝업 여러개 띄우기 작업하자.

        #region PopupBase : 모든 팝업의 부모 팝업

        #region GetPopup


        /// <summary>
        /// 이름, 타입명으로 팝업 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public T GetPopup<T>() where T : PopupBase
        {
            T popupBase;
            string popupName = typeof(T).Name;
            if (_dicPopups.ContainsKey(popupName))
            {
                popupBase = (T)_dicPopups[popupName];
            }
            else
            {
                popupBase = Single.Resources.Instantiate<T>(Cons.Path_Prefab_UI_Popup + popupName, _popupRoot);
                if (popupBase != null)
                {
                    popupBase.Initialize();
                    popupBase.InitializeLate();

                    _dicPopups.Add(popupName, popupBase);
                }
                else
                {
                    Debug.LogWarning("GetPopup() is null - popupName = " + popupName);
                    return null;
                }
            }
            return popupBase;
        }
        #endregion

        #region PushPopup

        /// <summary>
        /// 이름 타입명으로 팝업 푸쉬
        /// </summary>
        /// <param name="popupName">팝업이름</param>
        /// <param name="popupData">팝업데이터(텍스트...)</param>
        /// <param name="popupAction">팝업액션</param>
        public T PushPopup<T>() where T : PopupBase
        {
            PopupBase popupBase = GetPopup<T>();
            if (!isUILock)
            {
                Util.RunCoroutine(Co_PushPopup<T>());
            }
            return (T)popupBase;
        }

        /// <summary>
        /// 팝업 푸쉬 - 코루틴
        /// </summary>
        /// <param name="popupBase"></param>
        /// <returns></returns>
        private IEnumerator<float> Co_PushPopup<T>() where T : PopupBase
        {
            PopupBase popupBase = GetPopup<T>();
            if (_stackPopups.Count > 0 && popupBase == _stackPopups.Peek())
            {
                yield break;
            }

            isUILock = true;
            _stackPopups.Push(popupBase); //푸쉬해서
            yield return _stackPopups.Peek().Co_OpenPopup().WaitUntilDone(); //오픈

            string popupName = _stackPopups.Peek().name;
            int cnt = _stackPopups.Count;
            DEBUG.LOG($"Push Popup : {popupName}, Cnt : {cnt - 1} -> {cnt}", eColorManager.STACK);
            isUILock = false;
        }
        #endregion

        #region PopPopup : 팝업 닫기

        /// <summary>
        /// 팝 팝업
        /// </summary>
        public void PopPopup()
        {
            if (isUILock) return;
            Util.RunCoroutine(Co_PopPopup());
        }
        private IEnumerator<float> Co_PopPopup()
        {
            isUILock = true;

            PopupBase popupBase = _stackPopups.Pop(); //팝해서
            yield return popupBase.Co_ClosePopup().WaitUntilDone(); //클로즈

            string popupName = _stackPopups.Count > 0 ? _stackPopups.Peek().name : "Popup Empty";
            int cnt = _stackPopups.Count;
            DEBUG.LOG($"Pop Popup : {popupName}, Cnt : {cnt + 1} -> {cnt}", eColorManager.STACK);

            isUILock = false;
        }

        #endregion

        #endregion


        #region ToastPopup : 토스트 팝업 - 스텍에 안 쌓임, Toast_Basic 외에는 데이터 알아서 넣어야함

        #region GetToast
        /// <summary>
        /// 타입명으로 토스트 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenToast<T>() where T : ToastBase
        {
            T toastBase;
            string toastName = typeof(T).Name;
            if (_dicToasts.ContainsKey(toastName))
            {
                toastBase = (T)_dicToasts[toastName];
            }
            else
            {
                toastBase = Single.Resources.Instantiate<T>(Cons.Path_Prefab_UI_Toast + toastName, _popupRoot);
                if (toastBase != null)
                {
                    _dicToasts.Add(toastName, toastBase);
                }
                else
                {
                    Debug.LogWarning("GetToast() is null - toastName = " + toastName);
                    return null;
                }
            }
            return toastBase;
        }
        #endregion
        #endregion
        #endregion
        #endregion


        #region 더미파일
        /// <summary>
        /// 씬 하이라키에 없는 별도의 Panel 생성 후 추가히기
        /// </summary>
        /// <param name="panel"></param>
        //      public void AddPanel(PanelBase panel)
        //{
        //	panel.transform.parent = _panelRoot;

        //	panel.Initialize();
        //	panel.gameObject.SetActive(false);

        //	_dicPanels.Add(panel.name, panel);
        //}
        /// <summary>
        /// 메모리언로드
        /// </summary>
        //public void Unload()
        //{
        //    foreach (PanelBase panel in _dicPanels.Values)
        //    {
        //        panel.Unload();
        //    }
        //    _dicPanels.Clear();

        //    foreach (PanelBase panel in _dicPopups.Values)
        //    {
        //        panel.Unload();
        //    }
        //    _dicPopups.Clear();
        //}
        #endregion
    }
}