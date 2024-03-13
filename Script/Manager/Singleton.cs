using Assets.DeepLinkingForWindows;
using FrameWork.Network;
using FrameWork.Scene;
using FrameWork.Socket;
using FrameWork.UI;
using UnityEngine;

public static class Single
{
    public static ResourcesManager Resources => ResourcesManager.Instance;
    public static WebManager Web => WebManager.Instance;
    public static RealtimeManager RealTime => RealtimeManager.Instance;
    public static SceneManager Scene => SceneManager.Instance;
    public static SoundManager Sound => SoundManager.Instance;
    public static MasterDataManager MasterData => MasterDataManager.Instance;
    public static ItemDataManager ItemData => ItemDataManager.Instance;
    public static WebViewManager WebView => WebViewManager.Instance;
#if UNITY_ANDROID || UNITY_IOS
    public static GamePotManager Gamepot=> GamePotManager.Instance;
#endif
    public static StorageManager Storage=> StorageManager.Instance;
    public static DynamicLinkSetting DynamicLink => DynamicLinkSetting.Instance;
    public static ScreenManager Screen => ScreenManager.Instance;
    public static JoinShareLink JoinShareLink => JoinShareLink.Instance;
    public static AgoraManager Agora => AgoraManager.Instance;
    public static SocketManager Socket => SocketManager.Instance;
}

/// <summary>
/// 싱글턴 추상클래스
/// </summary>
/// <typeparam name="T">해당클래스</typeparam>
public abstract class Singleton<T> : MonoBase where T : Singleton<T>
{
    private static T instance = null; //진짜 싱글톤 인스턴스

    private static object objectLock = new object();
    private static bool isTerminated;

    public static T Instance => GetInstance();

    public static T GetInstance()
    {
        if (isTerminated)
        {
            return null;
        }

        lock(objectLock)
		{
            if (instance == null) // 찐 인스턴스는 없는데,
            {
                if (FindObjectOfType<T>() == null) // 씬에 존재하는 인스턴스도 없다? (그럼 생성하여 찐 인스턴스에 넣어줌)
                {
                    instance = CreateEmptyObject(typeof(T).ToString()).AddComponent<T>();
#if UNITY_EDITOR
                    instance.gameObject.AddComponent<GameObjectStealthMode>();
#endif
                }
                else // 씬에 존재하는 인스턴스가 있다 ?
                {
                    instance = FindObjectOfType<T>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        return instance;
    }



    /// <summary>
    /// 빈오브젝트 생성
    /// </summary>
    public static GameObject CreateEmptyObject(string strName) //2
    {
        DEBUG.LOG("Create Singleton Manager : " + strName, eColorManager.SINGLE);
        return new GameObject(strName);
    }


    /// <summary>
    /// 유틸 : 객체 중복체크 -> 인스턴스ID 다른놈 추출, 다른놈 없으면 null
    /// </summary>
#pragma warning disable 693
    public static T GetDuplication<T>(T pInstance) where T : Object //4
#pragma warning restore 693
    {
        T[] pList = FindObjectsOfType<T>();
        if (pList == null) //찾은 오브젝트가 없으면 null
        {
            return null;
        }

        for (int iLoop = 0; iLoop < pList.Length; iLoop++) //찾은 오브젝트가 1개 이상이면
        {
            if (pInstance.GetInstanceID() != pList[iLoop].GetInstanceID()) //내 오브젝트와 찾은 오브젝트의 인스턴스ID비교하여 다르면
            {
                return pList[iLoop];
            }
        }
        return null;
    }

    public static void DuplicationDelete(T pInstance) //4
    {
        T[] pList = FindObjectsOfType<T>();
        for (int iLoop = 0; iLoop < pList.Length; iLoop++) //찾은 오브젝트가 1개 이상이면
        {
            if (pInstance.GetInstanceID() != pList[iLoop].GetInstanceID()) //내 오브젝트와 찾은 오브젝트의 인스턴스ID비교하여 다르면
            {
                DestoryObject(pList[iLoop].gameObject);
            }
        }
    }


    /// <summary>
    /// 오브젝트 제거
    /// </summary>
    public static void DestoryObject(Object pObject)
    {
        if (pObject == null)
            return;

        DestroyImmediate(pObject);
    }


    protected override void START()
    {
        base.START();
        if (instance != null)
        {
            DuplicationDelete(instance);
        }
    }

    /// <summary>
    /// 메모리릭 누수 방지.. 추측.. 프로세스터지는 현상..
    /// </summary>
    protected virtual void OnDestroy()
    {
        instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        isTerminated = true;
    }

    protected virtual void OnApplicationPause(bool _isPaused)
    {

    }
}