
/****************************************************************************************************
 * 
 *					SceneLogic.cs
 *						- 모든 Scene 의 시작, 종료(Scene 전환) 로직을 담당
 *						- SceneLogic 
 *						    Base : 공통되는 기본 함수들 호출
 *						    UI : UI 캐싱, 푸쉬, 팝 연출
 *						    Network : 캐릭터 생성, 웹소켓연결 등
 *											                        * Update이력
 *											                        - 2023-09-07 Scenelogic Partial화
 * 
 ****************************************************************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrameWork.UI
{
    public static class Scene
    {
        public static Scene_ConferenceZone ConferenceZone => SceneLogic.instance as Scene_ConferenceZone;
        public static Scene_Consulting Consulting => SceneLogic.instance as Scene_Consulting;
        public static Scene_FestivalZone FestivalZone => SceneLogic.instance as Scene_FestivalZone;
        public static Scene_GameZone GameZone => SceneLogic.instance as Scene_GameZone;
        public static Scene_JumpingMatching JumpingMatching => SceneLogic.instance as Scene_JumpingMatching;
        public static Scene_Land_Arz Arz => SceneLogic.instance as Scene_Land_Arz;
        public static Scene_Land_Busan Busan => SceneLogic.instance as Scene_Land_Busan;
        public static Scene_Lobby Lobby => SceneLogic.instance as Scene_Lobby;
        public static Scene_Logo Logo => SceneLogic.instance as Scene_Logo;
        public static Scene_MyRoom MyRoom => SceneLogic.instance as Scene_MyRoom;
        public static Scene_OfficeRoom OfficeRoom => SceneLogic.instance as Scene_OfficeRoom;
        public static Scene_OfficeZone OfficeZone => SceneLogic.instance as Scene_OfficeZone;
        public static Scene_OXQuiz OXQuiz => SceneLogic.instance as Scene_OXQuiz;
        public static Scene_Patch Patch => SceneLogic.instance as Scene_Patch;
        public static Scene_Room_Exposition_Booth Exposition_Booth => SceneLogic.instance as Scene_Room_Exposition_Booth;
        public static Scene_StoreZone StoreZone => SceneLogic.instance as Scene_StoreZone;
        public static Scene_Title Title => SceneLogic.instance as Scene_Title;
        public static Scene_VoteZone VoteZone => SceneLogic.instance as Scene_VoteZone;
        public static Scene_Zone_Exposition Exposition => SceneLogic.instance as Scene_Zone_Exposition;
    }


    [DefaultExecutionOrder(-1)] //UIBase 보다 빨리 실행되기 위함
    public partial class SceneLogic : MonoBehaviour
    {
        //싱글톤
        public static SceneLogic instance = null;

        protected virtual void Awake()
        {
            Base_Awake();
            UI_Awake();
            Network_Awake();
        }
        protected virtual void Start()
        {
            Base_Start();
            UI_Start();
            Network_Start();
        }
        protected virtual void Update()
        {
            UI_Update();
        }
        protected virtual void OnApplicationQuit()
        {
            Base_OnApplicationQuit();
        }

        protected virtual void OnApplicationPause(bool isPause)
        {
            Base_OnApplicationPause(isPause);
        }

        protected virtual void OnDestroy()
        {
            UI_OnDestroy();
        }

        /// <summary>
        /// 씬타입 추출
        /// </summary>
        /// <returns></returns>
        public SceneName GetSceneType()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            return Util.String2Enum<SceneName>(sceneName);
        }

    }
}