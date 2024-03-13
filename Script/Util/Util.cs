using BKK;
using MEC;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;
using FrameWork.UI;
using static EasingFunction;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using SimpleFileBrowser;
using Newtonsoft.Json.Linq;

public static partial class Util
{
    //private static DateTime lastCheckedDate;
    //private const string LastCheckedDate = "LastCheckedDate";

    //public static void StartCheckDate(Action act)
    //{
    //    // 게임 시작 시 저장된 날짜 불러오기 (첫 실행 시 저장된 값이 없으면 현재 날짜로 초기화)
    //    string savedDate = PlayerPrefs.GetString(LastCheckedDate);

    //    if(savedDate == string.Empty)
    //    {
    //        PlayerPrefs.SetString(LastCheckedDate, savedDate = DateTime.Now.ToString());
    //        PlayerPrefs.Save();

    //        act?.Invoke();
    //    }
    //    else
    //    {
    //        lastCheckedDate = DateTime.Parse(savedDate);

    //        // 현재 날짜와 마지막 체크한 날짜 비교
    //        if (DateTime.Now.Date > lastCheckedDate.Date)
    //        {

    //            // 마지막 체크한 날짜 업데이트 및 저장
    //            lastCheckedDate = DateTime.Now;
    //            PlayerPrefs.SetString(LastCheckedDate, lastCheckedDate.ToString());
    //            PlayerPrefs.Save();

    //            // 하루가 지났으므로 체크할 작업 수행
    //            act?.Invoke();
    //        }
    //    }
    //}

    /// <summary>
    /// 입장공지 체크용 멤버 구하기
    /// </summary>
    /// <param name="noticeExposureType"></param>
    /// <returns></returns>
    public static NoticeMember GetNoticeMember()
    {
        NoticeInfo enterNoticeInfo = LocalPlayerData.noticeInfo.ToList().FirstOrDefault(x => (eNoticeType)x.noticeType == eNoticeType.입장공지이벤트);
        if (enterNoticeInfo == default)
        {
            return null;
        }
        int noticeExposureType = enterNoticeInfo.noticeExposureType;

        NoticeMember noticeMember;
        string noticeMemberCode = $"Notice_{LocalPlayerData.MemberCode}";
        string savedDate = PlayerPrefs.GetString(noticeMemberCode, string.Empty);
        if (savedDate == string.Empty)
        {
            noticeMember = new NoticeMember(noticeExposureType);
        }
        else
        {
            noticeMember = JsonConvert.DeserializeObject<NoticeMember>(savedDate);
            if(noticeMember.noticeExposureType != (eNoticeExposureType)enterNoticeInfo.noticeExposureType)
            {
                noticeMember.noticeExposureType = (eNoticeExposureType)enterNoticeInfo.noticeExposureType;
            }
        }

        return noticeMember;
    }

    /// <summary>
    /// 입장공지 체크용 멤버 저장하기
    /// </summary>
    /// <param name="noticeMember"></param>
    public static void SetNoticeMember(NoticeMember noticeMember)
    {
        string noticeMemberCode = $"Notice_{LocalPlayerData.MemberCode}";
        string json = JsonConvert.SerializeObject(noticeMember);
        PlayerPrefs.SetString(noticeMemberCode, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 앱 실행시 최초
    /// </summary>
    public static void InitNoticeMember()
    {
        NoticeMember noticeMember = GetNoticeMember();

        if(noticeMember == null)
        {
            return;
        }

        //시간 체크해서 다음날이면 초기화
        if (CheckNextDay(noticeMember.lastCheckedDate))
        {
            noticeMember.lastCheckedDate = DateTime.Now;
            noticeMember.noticeExposureState = eNoticeExposureState.reset;
        }
        else
        {
            if ((noticeMember.noticeExposureType == eNoticeExposureType.일일일회노출 && noticeMember.noticeExposureState == eNoticeExposureState.done)
                || (noticeMember.noticeExposureType == eNoticeExposureType.선택노출 && noticeMember.noticeExposureState == eNoticeExposureState.done))
            {
                
            }
            else
            {
                noticeMember.noticeExposureState = eNoticeExposureState.reset;
            }
        }
        SetNoticeMember(noticeMember);
    }

    /// <summary>
    /// 공지 열기
    /// </summary>
    public static void OpenNotice()
    {
        NoticeMember noticeMember = GetNoticeMember();

        if (noticeMember == null)
        {
            //SceneLogic.instance.OpenToast<Toast_Basic>()
            //    .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("000 공지가없다")));
            return;
        }

       

        if (noticeMember.noticeExposureState == eNoticeExposureState.reset)
        {
            noticeMember.noticeExposureState = eNoticeExposureState.done;
            if (noticeMember.noticeExposureType == eNoticeExposureType.선택노출)
            {
                noticeMember.noticeExposureState = eNoticeExposureState.once;
                Single.WebView.togplus_Today.gameObject.SetActive(true);
            }
            SetNoticeMember(noticeMember);

            Single.WebView.OnBackBtnCallback += () =>
            {
                NoticeMember noticeMember = GetNoticeMember();
                if (noticeMember.noticeExposureType == eNoticeExposureType.선택노출)
                {
                    noticeMember.noticeExposureState = Single.WebView.togplus_Today.GetToggleIsOn() ? eNoticeExposureState.done : eNoticeExposureState.once;
                    SetNoticeMember(noticeMember);

                    Single.WebView.togplus_Today.gameObject.SetActive(false);
                }
            };
            LocalPlayerData.Method.OpenNoticeOrEvent(eNoticeType.입장공지이벤트);
        }
    }

    /// <summary>
    /// 다음날인지 여부 체크
    /// </summary>
    /// <param name="noticeMember"></param>
    /// <returns></returns>
    private static bool CheckNextDay(DateTime curDateTime)
    {
        // 현재 날짜와 마지막 체크한 날짜 비교
        return DateTime.Now.Date > curDateTime;
    }



    #region 시스템

    public enum Orientation
    {
        Landscape,
        Portrait,
    }

    /// <summary>
    /// 화면 가로세로모드 변경
    /// </summary>
    /// <param name="orientation">가로세로모드 설정</param>
    public static void SetScreenOrientation(Orientation orientation)
    {
        Debug.Log("!!!!!!!!!!!!! 가로세로변경 : " + orientation + " !!!!!!!!!!!!!");
        //if (orientation == Orientation.Landscape)
        //{
        //    Screen.orientation = ScreenOrientation.AutoRotation;
        //    Screen.autorotateToPortrait = false;
        //    Screen.autorotateToPortraitUpsideDown = false;
        //    Screen.autorotateToLandscapeLeft = true;
        //    Screen.autorotateToLandscapeRight = true;
        //}
        //else
        //{
        //    Screen.orientation = ScreenOrientation.Portrait;
        //}
    }

    /// <summary>
    /// 퀄리티 세팅 가져오기 및 변경
    /// </summary>
    public static QUALITY_LEVEL Quality
    {
        get
        {
            return GetQualitySetting();
        }
        set
        {
            PlayerPrefs.SetString(Cons.Setting_Quality, value.ToString());
            QualitySettingInitialize();
        }
    }

    /// <summary>
    /// 저장된 퀄리티 세팅을 불러와 현재 퀄리티 세팅에 적용
    /// </summary>
    public static void QualitySettingInitialize()
    {
        QualitySettings.SetQualityLevel((int)GetQualitySetting());
    }

    /// <summary>
    /// 현재 그래픽 퀄리티 세팅을 eQuality enum 형태로 가져오기
    /// </summary>
    public static QUALITY_LEVEL GetQualitySetting()
    {
        if (!PlayerPrefs.HasKey(Cons.Setting_Quality))
        {
            PlayerPrefs.SetString(Cons.Setting_Quality, ((QUALITY_LEVEL)QualitySettings.GetQualityLevel()).ToString());
        }

        return String2Enum<QUALITY_LEVEL>(PlayerPrefs.GetString(Cons.Setting_Quality));
    }

    /// <summary>
    /// 저장된 퀄리티 세팅을 불러와 현재 퀄리티 세팅에 적용
    /// </summary>
    //public static IEnumerator<float> SoundSettingInitialize()
    //{
    //    yield return Timing.WaitUntilTrue(() => AppGlobalSettings.Instance);

    //    AppGlobalSettings.Instance.LoadData();

    //    // yield return Timing.WaitUntilTrue(()=> Single.Sound);
    //    //
    //    // Single.Sound.SetVolume("BGM", AppGlobalSettings.Instance.volumeBGM);
    //    // Single.Sound.SetVolume("Effect", AppGlobalSettings.Instance.volumeEffect);
    //    // Single.Sound.SetMediaSliderValue(AppGlobalSettings.Instance.volumeMedia);
    //}
    #endregion



    #region 기본

    /// <summary>
    /// 해당 게임오브젝트가 카메라 영역 안에 있는지 체크(포지션 체크 방식)
    /// </summary>
    /// <param name="go">대상 게임오브젝트</param>
    /// <param name="cam">대상 카메라</param>
    /// <returns></returns>
    public static bool IsInCamera(this GameObject go, Camera cam)
    {
        var targetScreenPos = cam.WorldToViewportPoint(go.transform.position);

        return targetScreenPos.x < 1 && targetScreenPos.x > 0 &&
               targetScreenPos.y < 1 && targetScreenPos.y > 0 &&
               targetScreenPos.z > 0;
    }

    /// <summary>
    /// 해당 게임오브젝트가 카메라 영역 안에 있는지 체크(바운드 체크 방식)
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static bool IsInCamera(this Renderer renderer, Camera cam)
    {
        return renderer.bounds.IsInCamera(cam);
    }

    /// <summary>
    /// 해당 게임오브젝트가 카메라 영역 안에 있는지 체크(바운드 체크 방식)
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static bool IsInCamera(this Collider collider, Camera cam)
    {
        return collider.bounds.IsInCamera(cam);
    }

    /// <summary>
    /// 해당 게임오브젝트가 카메라 영역 안에 있는지 체크(바운드 체크 방식)
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="cam"></param>
    /// <returns></returns>
    public static bool IsInCamera(this Bounds bounds, Camera cam)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);

        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    /// <summary>
    /// GetComponentsInChildren 강화버전 (비활성화 상태도 호출가능)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_target"></param>
    /// <returns></returns>
    public static List<T> GetComponentsInChildrenPlus<T>(this Transform _target) where T : Component
    {
        List<T> list = new List<T>();
        GetComponentsInChildrenPlus(_target, ref list);
        return list;
    }
    private static void GetComponentsInChildrenPlus<T>(this Transform _target, ref List<T> list) where T : Component
    {
        for (int i = 0; i < _target.childCount; ++i)
        {
            Transform childTr = _target.GetChild(i);
            if (childTr.TryGetComponent(out T t))
            {
                list.Add(t);
            }
            GetComponentsInChildrenPlus(childTr, ref list);
        }
    }

    /// <summary>
    /// 딕셔너리의 벨류의 리스트에 데이터 추가
    /// </summary>
    /// <typeparam name="T1">타입1</typeparam>
    /// <typeparam name="T2">타입2</typeparam>
    /// <param name="dic">딕셔너리</param>
    /// <param name="data">추가할 데이터</param>
    /// <param name="key">키값</param>
    public static void DicList<T1, T2>(ref Dictionary<T1, List<T2>> dic, T1 key, T2 data)
    {
        if (!dic.ContainsKey(key))
        {
            dic.Add(key, new List<T2>());
        }
        dic[key].Add(data);
    }
    public static void DicQueue<T1, T2>(ref Dictionary<T1, Queue<T2>> dic, T1 key, T2 data)
    {
        if (!dic.ContainsKey(key))
        {
            dic.Add(key, new Queue<T2>());
        }
        dic[key].Enqueue(data);
    }



    /// <summary>
    /// 해당 오브젝트 하위에서 특정 이름의 컴포넌트 서치
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_gameObject"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static T Search<T>(this GameObject _gameObject, string _name) where T : UnityEngine.Object
    {
        return _gameObject.transform.Search<T>(_name);
    }

    /// <summary>
    /// 해당 오브젝트 하위에서 특정 이름의 컴포넌트 서치
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_obj"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static T Search<T>(this Transform _transform, string _name) where T : UnityEngine.Object
    {
        var go = Search(_transform, _name);

        if (go == null)
        {
            return null;
        }

        if (typeof(T) == typeof(GameObject))
        {
            return go as T;
        }
        else
        {
            return go.GetComponent<T>();
        }
    }



    /// <summary>
    /// 해당 오브젝트 하위에서 특정 이름의 트랜스폼 서치
    /// </summary>
    /// <param name="_obj"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static Transform Search(this GameObject _obj, string _name)
    {
        return Search(_obj.transform, _name);
    }

    /// <summary>
    /// 자식 트랜스폼 찾기
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static Transform Search(this Transform _target, string _name)
    {
        if (_target.name == _name) return _target;

        for (int i = 0; i < _target.childCount; ++i)
        {
            var result = Search(_target.GetChild(i), _name);

            if (result != null) return result;
        }

        return null;
    }

    /// <summary>
    /// 부모 트랜스폼 찾기
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_name"></param>
    /// <returns></returns>
    public static Transform SearchParent(this Transform _target, string _name)
    {
        if (_target.name == _name) return _target;

        if (_target.parent == null) return null;

        if (_target.parent.name == _name)
        {
            return _target.parent;
        }
        else
        {
            return SearchParent(_target.parent, _name);
        }

        return null;
    }

    public static void InsertIfNull<TResult>(ref TResult targetProperty, Func<TResult> func)
    {
        if (targetProperty == null)
        {
            targetProperty = func.Invoke();
        }
    }

    public static void AddButtonListener(ref UnityEngine.UI.Button button, Action onClick)
    {
        if (button != null)
        {
            button.onClick.AddListener(onClick.Invoke);
        }
    }

    public static void AddToggleListener(ref UnityEngine.UI.Toggle toggle, Action<bool> onValueChanged)
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(onValueChanged.Invoke);
        }
    }

    public static void AddInputFieldListener(ref UnityEngine.UI.InputField inputField, Action<string> onValueChanged = null, Action<string> onEndEdit = null)
    {
        if (inputField != null)
        {
            if (onValueChanged != null) inputField.onValueChanged.AddListener(onValueChanged.Invoke);
            if (onEndEdit != null) inputField.onEndEdit.AddListener(onEndEdit.Invoke);
        }
    }

    public static void AddTMPInputFieldListener(ref TMPro.TMP_InputField inputField, Action<string> onValueChanged = null, Action<string> onSubmit = null, Action<string> onEndEdit = null)
    {
        if (inputField != null)
        {
            if (onValueChanged != null) inputField.onValueChanged.AddListener(onValueChanged.Invoke);
            if (onSubmit != null) inputField.onSubmit.AddListener(onSubmit.Invoke);
            if (onEndEdit != null) inputField.onEndEdit.AddListener(onEndEdit.Invoke);
        }
    }

    #endregion



    #region enum 관련

    /// <summary>
    /// enum을 list로 변환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> Enum2List<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    /// <summary>
    /// enum을 stringArray로 변환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string[] Enum2StringArray<T>()
    {
        return Enum.GetNames(typeof(T));
    }

    /// <summary>
    /// enum원형의 길이 구하기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int EnumLength<T>()
    {
        return Enum.GetNames(typeof(T)).Length;
    }

    /// <summary>
    /// string을 enum으로 변환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_str"></param>
    /// <returns></returns>
    public static T String2Enum<T>(string _str)
    {
        try { return (T)Enum.Parse(typeof(T), _str); }
        catch { return (T)Enum.Parse(typeof(T), "none"); }
    }

    /// <summary>
    /// enum을 string으로 변환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_enum"></param>
    /// <returns></returns>
    public static string Enum2String<T>(T _enum) where T : Enum
    {
        try { return Enum.GetName(typeof(T), _enum); }
        catch { return String.Empty; }
    }

    /// <summary>
    /// enum의 숫자를 string으로 변환
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string EnumInt2String(object obj)
    {
        return ((int)obj).ToString();
    }
    #endregion



    #region 기타...
    /// <summary>
    /// 주석달기...
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="show"></param>
    /// <param name="max"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private static IEnumerator CoSetTransition(Material mat, bool show, float max = 1f, float duration = 0.3f)
    {
        float curTime;

        if (show == true)
        {
            curTime = 0f;
            while (curTime < 1f)
            {
                curTime += Time.deltaTime / duration;
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, curTime * max);
                yield return null;
            }
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, max);
        }

        else
        {
            curTime = 1f;
            while (curTime > 0f)
            {
                curTime -= Time.deltaTime / duration;
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, curTime * max);
                yield return null;

            }
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0f);

        }

    }

    public static string LocalizeString(string table, string entry)
    {
        return new LocalizedString(table, entry).GetLocalizedString();
    }

    /// <summary>
    /// 리치 텍스트를 제외한 텍스트의 길이 반환
    /// </summary>
    /// <param name="richText"></param>
    /// <returns></returns>
    public static int TextLengthWithoutRichText(string richText)
    {
        int len = 0;
        bool inTag = false;

        foreach (var ch in richText)
        {
            if (ch == '<')
            {
                inTag = true;
                continue;
            }
            else if (ch == '>')
            {
                inTag = false;
            }
            else if (!inTag)
            {
                len++;
            }
        }

        return len;
    }

    /// <summary>
    /// 해시태그
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public static string CreateHashTag(string contents, string colorHEX)
    {
        // 띄어쓰기 스플릿
        string str = contents;
        string[] strArr = str.Split(' ');
        int strArrCount = strArr.Length;
        if (strArrCount > 0)
        {
            List<string> hashtagList = new List<string>();
            for (int i = 0; i < strArrCount; i++)
            {
                // # 들어가 있는 거 찾기
                if (strArr[i].Contains("#"))
                {
                    int index = strArr[i].IndexOf("#");
                    string subStr = strArr[i].Substring(index);
                    hashtagList.Add(subStr);
                }
            }
            hashtagList = hashtagList.Distinct().ToList();

            int hashtagArrCount = hashtagList.Count;
            if (hashtagArrCount > 0)
            {
                for (int i = 0; i < hashtagArrCount; i++)
                {
                    // # 앞뒤로 리치텍스트
                    string richStr = string.Format("<link=\"{0}\"><color={1}><b>{0}</b></color></link>", hashtagList[i], colorHEX);
                    // 본 문장에서 치환
                    str = str.Replace(hashtagList[i], richStr);
                }
            }
        }
        return str;
    }

    /// <summary>
    /// 키보드 높이 구하기
    /// </summary>
    /// <returns></returns>
    public static float GetKeyboardSize()
    {
#if UNITY_ANDROID
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject Rct = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", Rct);
                return Rct.Call<int>("height");
            }
        }
#elif UNITY_IOS
        Debug.Log("IOS 환경");
        return TouchScreenKeyboard.area.height;
#else
		return 0;
#endif
    }

    static float height, width, keyboardSize, curRatio;

    /// <summary>
    /// 키보드 높이에 따른 패널 사이즈 구하기
    /// </summary>
    /// <returns></returns>
    public static Vector2 GetCurrentPanelSize(Vector2 oriSize)
    {
        float screenSize = Screen.height;
        curRatio = oriSize.y / screenSize;
#if UNITY_EDITOR
        height = oriSize.y;
#elif UNITY_ANDROID
        keyboardSize = screenSize - GetKeyboardSize();
        height = keyboardSize == 0 ? oriSize.y : oriSize.y - (keyboardSize * (oriSize.y / screenSize));
        //keyboardSize = (screenSize - GetKeyboardSize()) * 10 * Mathf.Pow(curRatio, 2);
        //height = keyboardSize == 0 ? oriSize.y : oriSize.y - keyboardSize;
        Debug.Log("screenSize : " + screenSize + " / GetKeyboardSize() : " + GetKeyboardSize() + " / keyboardSize : " + keyboardSize + " / oriSize.y : " + oriSize.y + " / height : " + height);
#elif UNITY_IOS
        keyboardSize = GetKeyboardSize();
        height = keyboardSize == 0 ? oriSize.y : oriSize.y - (keyboardSize * (oriSize.y / screenSize));
        //keyboardSize = GetKeyboardSize();
        //height = keyboardSize == 0 ? oriSize.y : oriSize.y - (keyboardSize * curRatio);
#endif
        width = oriSize.x / Screen.width;
        return new Vector2(width, height);
    }

    /// <summary>
    /// 맨 처음 문자열 확인하고 지움
    /// </summary>
    /// <param name="oriStr"></param>
    /// <param name="subStr"></param>
    /// <returns></returns>
    public static string StartWithSubString(string oriStr, string subStr)
    {
        if (oriStr.StartsWith(subStr))
        {
            int subStrCount = subStr.Length;
            return oriStr.Substring(subStrCount);
        }
        return string.Empty;
    }

    /// <summary>
    /// 명함 프리팹명 가져오기
    /// </summary>
    /// <param name="templateId"></param>
    /// <returns></returns>
    public static string GetBizPrefabName(int templateId, BIZTEMPLATE_TYPE type)
    {
        string prefabName = null;

        switch ((BIZCARD_TYPE)templateId)
        {
            case BIZCARD_TYPE.BIZCARD_A:
                {
                    switch (type)
                    {
                        case BIZTEMPLATE_TYPE.NOMAL: prefabName = Cons.View_BizCard_A; break;
                        case BIZTEMPLATE_TYPE.MINI: prefabName = Cons.View_BizCard_A_Mini; break;
                        case BIZTEMPLATE_TYPE.EDIT: prefabName = Cons.View_BizCard_A_Edit; break;
                    }
                }
                break;
            default: break;
        }
        return prefabName;
    }


    /// <summary>
    /// 로그아웃 후 로그인 창으로 돌아감
    /// </summary>
    public static void ReturnToLogin()
    {
        Single.Scene.FadeOut(1f, () =>
        {
            DisconnectSocket();

            //기본정보 리셋
            LocalPlayerData.ResetData();

            //타이틀씬 이동
            Single.Scene.LoadScene(SceneName.Scene_Base_Title);

            Single.Scene.isSocketLock = false;
        });
    }

    /// <summary>
    /// 어플리케이션 강제 종료
    /// </summary>
    public static void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); // 강종
#endif
    }

    public static void ReturnToLogo()
    {
        Single.Scene.FadeOut(1f, () =>
        {
            DisconnectSocket();
            LocalPlayerData.ResetChat();
            LocalPlayerData.StopHeartBeat();

            Single.Scene.LoadScene(SceneName.Scene_Base_Logo);

            Single.Scene.isSocketLock = false;
        });
    }

    private static void DisconnectSocket()
    {
        //웹소켓 끊어줌
        if (Single.Socket.socketManager != null)
            Single.Socket.socketManager.Close();

        //실시간소켓 끊어줌
        RealtimeUtils.Leave();
        RealtimeUtils.Disconnect();
    }

    #endregion




    #region Material 관련

    /// <summary>
    /// 메테리얼에 색상넣기!
    /// </summary>
    /// <param name="_meshRd"></param>
    /// <param name="_color"></param>
    /// <param name="_texture"></param>
    public static void SetColor(MeshRenderer _meshRd, Color _color, Texture[] _texture)
    {
        for (int i = 0; i < _meshRd.materials.Length; i++)
        {
            _meshRd.materials[i].color = _color;
            _meshRd.materials[i].mainTexture = _texture[i];
        }
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    /// <summary>
    /// 렌더모드 바꾸기
    /// </summary>
    /// <param name="standardShaderMaterial"></param>
    /// <param name="blendMode"></param>
    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }
    #endregion



    #region 이미지 관련

    /*
     * 이미지는 탐색, 로드, 변환, 저장 기능으로 분류할 수 있다.
     * 
     * 탐색 : AOS/IOS : 앨범에서 텍스쳐를 찾는다. PC : 탐색기로 찾는다.
     * 로드 : AOS/IOS : 앨범에서 텍스쳐를 로드한다. PC : 패스로 직접 로드한다.
     * 변환 : 텍스쳐or스프라이트를 다른 형태로 변환, 역변환이 가능하다.
     * 저장 : 텍스쳐or스프라이트를 JPG나 PNG로 저장한다. 
     */

    /* 텍스쳐 탐색 */
    /// <summary>
    /// 로컬파일에서 텍스쳐 탐색 (AOS, IOS, PC 모두 가능)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<Texture2D> Co_FindLocalTex()
    {
        string path = await Co_FindLocalTexPath();

        return await Co_LoadRemoteAsyncTex(path);
    }

    /// <summary>
    /// 로컬파일에서 스프라이트 탐색 (AOS, IOS, PC 모두 가능)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<Sprite> Co_FindLocalSprite()
    {
        return Tex2Sprite(await Co_FindLocalTex());
    }

    /// <summary>
    /// 로컬파일에서 텍스쳐 패스 탐색 (UnityEditor, PC, AOS, IOS)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<string> Co_FindLocalTexPath()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //return await Co_FindLocalTexPath_PC(); // 이걸 쓸 수도 있음
        return await SceneLogic.instance.PushPopup<Popup_FileBrowser>().OpenFileBrowser_ShowLoadDialog();
#else
        return await Co_FindLocalTexPath_Mobile();
#endif
    }

    /// <summary>
    /// 로컬파일에서 텍스쳐 패스 탐색 (UnityEditor, AOS, IOS)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<string> Co_FindLocalTexPath_Mobile()
    {
        string resultPath = string.Empty;
        bool completed = false;

        NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                resultPath = path;
            }
            completed = true;
        });

        await UniTask.WaitUntil(() => completed);

        return resultPath;
    }


    /// <summary>
    /// 로컬파일에서 텍스쳐 패스 탐색 (UnityEditor, PC)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<string> Co_ShowLoadDialog(FileBrowser.OnSuccess OnSuccess = null, FileBrowser.OnCancel OnCancel = null, FILEBROWSER_SETFILTER fILEBROWSER_FILTER = FILEBROWSER_SETFILTER.IMAGE)
    {
        string resultPath = string.Empty;
        bool completed = false;

        OnSuccess += (paths) =>
        {
            resultPath = paths[0];
            completed = true;
        };
        OnCancel += () => completed = true;
        SetFileFilter(fILEBROWSER_FILTER);
        FileBrowser.ShowLoadDialog(OnSuccess, OnCancel, FileBrowser.PickMode.Files);

        await UniTask.WaitUntil(() => completed);

        return resultPath;
    }

    private static void SetFileFilter(FILEBROWSER_SETFILTER fILEBROWSER_FILTER)
    {
        bool allFileFilter = false;
        switch (fILEBROWSER_FILTER)
        {
            case FILEBROWSER_SETFILTER.IMAGE:
                FileBrowser.SetFilters(allFileFilter, Cons.filterImage);
                break;
            case FILEBROWSER_SETFILTER.PDF:
                FileBrowser.SetFilters(allFileFilter, Cons.filterPDF);
                break;
            default:
                break;
        }
    }

    public static async UniTask<string> Co_ShowSaveDialog(FileBrowser.OnSuccess OnSuccess = null, FileBrowser.OnCancel OnCancel = null, FILEBROWSER_SETFILTER fILEBROWSER_FILTER = FILEBROWSER_SETFILTER.IMAGE)
    {
        string resultPath = string.Empty;
        bool completed = false;

        OnSuccess += (paths) =>
        {
            resultPath = paths[0];
            completed = true;
        };
        OnCancel += () => completed = true;

        FileBrowser.ShowSaveDialog(OnSuccess, OnCancel, FileBrowser.PickMode.Files);

        await UniTask.WaitUntil(() => completed);

        return resultPath;
    }



    /// <summary>
    /// 로컬파일에서 비디오 패스 탐색 (AOS, IOS, PC 모두 가능)
    /// </summary>
    /// <returns></returns>
    public static async UniTask<string> Co_FindLocalVideoPath()
    {
        string resultPath = "";
        bool completed = false;

        NativeGallery.GetVideoFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                resultPath = path;
            }
            completed = true;
        });

        await UniTask.WaitUntil(() => completed);

        return resultPath;
    }

    /* 텍스쳐 로드 */
    /// <summary>
    /// 비동기 텍스쳐 로드, 내부경로 텍스쳐 로드
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> Co_LoadLocalAsyncTex(string _path)
    {
        if (_path == "") return null;
        return await NativeGallery.LoadImageAtPathAsync(_path, maxSize: 1024, markTextureNonReadable: false);
    }

    /// <summary>
    /// 비동기 텍스쳐 로드, storage or url 텍스쳐 로드
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> Co_LoadRemoteAsyncTex(string _path)
    {
        try
        {
            if (_path == "") return null;
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_path);

            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("uwr.result Error: " + uwr.result);
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
            uwr.Dispose();
            return tex;
        }
        catch
        {
            return null;
        }
    }

    public static async UniTask<Sprite> Co_LoadLocalAsyncSprite(string _url)
    {
        return Tex2Sprite(await Co_LoadLocalAsyncTex(_url));
    }
    public static async UniTask<Sprite> Co_LoadRemoteAsyncSprite(string _url)
    {
        return Tex2Sprite(await Co_LoadRemoteAsyncTex(_url));
    }





    /* 텍스쳐 변환 */
    /// <summary>
    ///  RenderTexture를 텍스쳐로 변환
    /// </summary>
    /// <param name="_renderTex"></param>
    /// <returns></returns>
    public static Texture2D RenderTex2Tex(RenderTexture _renderTex)
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = _renderTex;

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D tex = new Texture2D(_renderTex.width, _renderTex.height, TextureFormat.RGBAFloat, false);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentActiveRT;
        return tex;
    }

    /// <summary>
    /// 텍스쳐를 스프라이트로 변환
    /// </summary>
    /// <param name="_tex"></param>
    /// <returns></returns>
    public static Sprite Tex2Sprite(Texture2D _tex) => _tex ? Sprite.Create(_tex, new Rect(0, 0, _tex.width, _tex.height), new Vector2(0.5f, 0.5f)) : null;

    /// <summary>
    /// 스프라이트를 텍스쳐로 변환
    /// </summary>
    /// <param name="_sprite"></param>
    /// <returns></returns>
    public static Texture2D Sprite2Tex(Sprite _sprite) => _sprite ? _sprite.texture : null;


    /* 텍스쳐 저장 ( Sprite -> Texture -> Image(PNG, JPG) ) */
    //Sprite
    /// <summary>
    /// 스프라이트를 파일확장자에 맞게 이미지로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_sprite"></param>
    public static void Sprite2Image(string _path, Sprite _sprite) => Tex2Image(_path, _sprite.texture);

    /// <summary>
    /// 스프라이트를 PNG로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_sprite"></param>
    public static void Sprite2PNG(string _path, Sprite _sprite) => Tex2PNG(_path, Sprite2Tex(_sprite));



    /// <summary>
    /// 스프라이트를 JPG로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_sprite"></param>
    public static void Sprite2JPG(string _path, Sprite _sprite) => Tex2JPG(_path, Sprite2Tex(_sprite));
    //Texture
    /// <summary>
    /// 텍스쳐를 파일확장자에 맞게 이미지로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_tex"></param>
    public static void Tex2Image(string _path, Texture2D _tex)
    {
        string fileExtension = _path.Split('.').Last();
        switch (fileExtension)
        {
            case "png":
                Tex2PNG(_path, _tex);
                break;
            case "jpg":
            case "jpeg":
                Tex2JPG(_path, _tex);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 텍스쳐를 PNG로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_tex"></param>
    public static void Tex2PNG(string _path, Texture2D _tex) => CreateFile(_path, _tex.EncodeToPNG());

    /// <summary>
    /// 텍스쳐를 JPG로 저장
    /// </summary>
    /// <param name="_path"></param>
    /// <param name="_tex"></param>
    public static void Tex2JPG(string _path, Texture2D _tex) => CreateFile(_path, _tex.EncodeToJPG());


    /* 텍스쳐 유틸 */
    /// <summary>
    /// 파일 용량 가져오기
    /// </summary>
    /// <param name="_maxMB"></param>
    /// <param name="_url"></param>
    /// <returns></returns>
    public static async UniTask<float> Co_GetFileSize(string _url, eDisk _disk = eDisk.MB)
    {
        try
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url);

            await uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("uwr.result Error: " + uwr.result);
            }
            byte[] data = uwr.downloadHandler.data;
            int sizeInBytes = data.Length;
            float size = sizeInBytes / (Mathf.Pow(1024, (int)_disk));
            uwr.Dispose();
            return size;
        }
        catch
        {
            return -1f;
        }
    }

    /// <summary>
    /// 이미지 가져오기
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> Co_LoadImage(string path, string fileName)
    {
        Texture2D tex = null;
        string path_ServerSaveAlbumFile = Path.Combine(Single.Web.StorageUrl, path, fileName);
        string path_LocalSaveAlbumFile = Path.Combine(Application.persistentDataPath, path, fileName);


        //내부 파일이 존재
        if (File.Exists(path_LocalSaveAlbumFile))
        {
            tex = await Co_LoadLocalAsyncTex(path_LocalSaveAlbumFile);
        }
        else //내부 파일에 존재하지 않음 -> 서버로부터 가져오기 (Download) or 로컬에서 로드 -> 저장(Save)
        {
            tex = await Co_LoadRemoteAsyncTex(path_ServerSaveAlbumFile);
            if (tex != null)
            {
                Tex2Image(path_LocalSaveAlbumFile, tex);
            }
        }

        return tex;
    }

    private const string path_Album = "frameImage";
    /// <summary>
    /// 마이룸 앨범 전용, 파일이 있으면 로드, 없으면 다운로드 로드
    /// </summary>
    /// <returns>로컬에 저장여부</returns>
    public static async UniTask<Texture2D> Co_LoadMyRoomFrame(MyRoomFrameImage myRoomFrameImage, bool isSave)
    {
        string memberCode = LocalPlayerData.Method.roomCode;
        string path_ServerSaveAlbumFile = string.Empty;
        string path_LocalSaveAlbumFile = string.Empty;

        //패스 구하기
        switch ((FRAMEIMAGEAPPEND_TYPE)myRoomFrameImage.uploadType)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                {
                    string tempPath = Path.Combine(path_Album, memberCode, myRoomFrameImage.num.ToString(), Path.GetFileName(myRoomFrameImage.imageName));
                    path_ServerSaveAlbumFile = Path.Combine(Single.Web.StorageUrl, tempPath);
                    path_LocalSaveAlbumFile = Path.Combine(Application.persistentDataPath, tempPath);
                }
                break;
            case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                {
                    string tempPath = Uri.EscapeDataString(myRoomFrameImage.imageName);
                    tempPath = (tempPath.Length > 30 ? tempPath.Substring(tempPath.Length - 30, 30) : tempPath) + ".jpg";
                    path_ServerSaveAlbumFile = myRoomFrameImage.imageName;
                    path_LocalSaveAlbumFile = Path.Combine(Application.persistentDataPath, path_Album, memberCode, "url", tempPath);
                }
                break;
        }

        //텍스쳐 구하기
        Texture2D tex = null;

        //내부 파일이 존재
        if (File.Exists(path_LocalSaveAlbumFile))
        {
            tex = await Co_LoadLocalAsyncTex(path_LocalSaveAlbumFile);
        }
        else //내부 파일에 존재하지 않음 -> 서버로부터 가져오기 (Download) or 로컬에서 로드 -> 저장(Save)
        {
            FRAMEIMAGEAPPEND_TYPE FRAMEIMAGEAPPEND_TYPE = (FRAMEIMAGEAPPEND_TYPE)myRoomFrameImage.uploadType;
            //텍스쳐 임시로드
            switch (FRAMEIMAGEAPPEND_TYPE)
            {
                case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                    {
                        if (isSave) //스토리지에 있음 -> 스토리지에서 가져와 임시로드->저장
                        {
                            tex = await Co_LoadRemoteAsyncTex(path_ServerSaveAlbumFile);
                        }
                        else //스토리지에 없음 -> 해당경로에서 가져와서 임시로드
                        {
                            tex = await Co_LoadLocalAsyncTex(myRoomFrameImage.imageName);
                        }
                    }
                    break;
                case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                    {
                        tex = await Co_LoadRemoteAsyncTex(path_ServerSaveAlbumFile);
                    }
                    break;
            }

            if (tex == null)
            {
                return null;
            }

            //텍스쳐 저장
            if (isSave)
            {
                if ((FRAMEIMAGEAPPEND_TYPE)myRoomFrameImage.uploadType == FRAMEIMAGEAPPEND_TYPE.로컬이미지)
                {
                    DeleteFiles(Path.Combine(Application.persistentDataPath, path_Album, memberCode, myRoomFrameImage.num.ToString()));
                }
                Tex2Image(path_LocalSaveAlbumFile, tex); //텍스쳐 저장
            }
        }

        return tex;
    }


    public const string path_Banner = "boothBanner";
    /// <summary>
    /// 박람회 부스 전용
    /// </summary>
    /// <param name="bannerInfo"></param>
    /// <param name="isSave"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> Co_LoadExpositionFrame(BannerInfo bannerInfo, bool isSave)
    {
        string path_ServerSaveAlbumFile = string.Empty;
        string path_LocalSaveAlbumFile = string.Empty;

        //패스 구하기
        switch ((FRAMEIMAGEAPPEND_TYPE)bannerInfo.uploadType)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                {
                    string tempPath = Path.Combine(path_Banner, bannerInfo.boothId.ToString(), bannerInfo.bannerId.ToString(), Path.GetFileName(bannerInfo.uploadValue));
                    path_ServerSaveAlbumFile = Path.Combine(Single.Web.StorageUrl, tempPath);
                    path_LocalSaveAlbumFile = Path.Combine(Application.persistentDataPath, tempPath);
                }
                break;
            case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                {
                    string tempPath = Uri.EscapeDataString(bannerInfo.uploadValue);
                    tempPath = (tempPath.Length > 30 ? tempPath.Substring(tempPath.Length - 30, 30) : tempPath) + ".jpg";
                    path_ServerSaveAlbumFile = bannerInfo.uploadValue; //url은 해당경로 그 자체
                    path_LocalSaveAlbumFile = Path.Combine(Application.persistentDataPath, path_Banner, bannerInfo.boothId.ToString(), "url", tempPath);
                }
                break;
        }

        //텍스쳐 구하기
        Texture2D tex = null;

        //내부 파일이 존재
        if (File.Exists(path_LocalSaveAlbumFile))
        {
            tex = await Co_LoadLocalAsyncTex(path_LocalSaveAlbumFile);
        }
        else //내부 파일에 존재하지 않음 -> 서버로부터 가져오기 (Download) or 로컬에서 로드 -> 저장(Save)
        {
            FRAMEIMAGEAPPEND_TYPE FRAMEIMAGEAPPEND_TYPE = (FRAMEIMAGEAPPEND_TYPE)bannerInfo.uploadType;
            //텍스쳐 임시로드
            switch (FRAMEIMAGEAPPEND_TYPE)
            {
                case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                    {
                        if (isSave) //스토리지에 있음 -> 스토리지에서 가져와서 임시로드 -> 저장
                        {
                            tex = await Co_LoadRemoteAsyncTex(path_ServerSaveAlbumFile);
                        }
                        else        //스토리지에 없음 -> 해당경로에서 가져와서 임시로드
                        {
                            tex = await Co_LoadLocalAsyncTex(bannerInfo.uploadValue);
                        }
                    }
                    break;
                case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                    {
                        tex = await Co_LoadRemoteAsyncTex(path_ServerSaveAlbumFile);
                    }
                    break;
            }

            if (tex == null)
            {
                return null;
            }

            //텍스쳐 저장
            if (isSave)
            {
                if ((FRAMEIMAGEAPPEND_TYPE)bannerInfo.uploadType == FRAMEIMAGEAPPEND_TYPE.로컬이미지)
                {
                    DeleteFiles(Path.Combine(Application.persistentDataPath, path_Banner, bannerInfo.boothId.ToString(), bannerInfo.bannerId.ToString()));
                }
                Tex2Image(path_LocalSaveAlbumFile, tex); //텍스쳐 저장
            }
        }

        return tex;
    }



    /// <summary>
    /// 이미지 가로세로 길이제한 확인
    /// </summary>
    /// <param name="maxMegabyte"></param>
    /// <param name="_tex"></param>
    /// <returns></returns>
    public static bool CheckTexWidthHeight(int _widthHeight, Texture2D _tex)
    {
        return (_tex.width > _widthHeight || _tex.height > _widthHeight);
    }

    /// <summary>
    /// 텍스쳐의 특정영역만 잘라서 반환
    /// </summary>
    /// <param name="_rect"></param>
    /// <param name="_tex"></param>
    /// <returns></returns>
    public static Texture2D CropTex(Rect _rect, Texture2D _tex)
    {
        int x = Mathf.FloorToInt(_rect.x);
        int y = Mathf.FloorToInt(_rect.y);
        int w = Mathf.FloorToInt(_rect.width);
        int h = Mathf.FloorToInt(_rect.height);

        Color[] pixels = _tex.GetPixels(x, y, w, h);
        Texture2D croppedTexture = new Texture2D(w, h);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        return croppedTexture;
    }

    /// <summary>
    /// 꽉차게 크롭 줌
    /// </summary>
    public static void ZoomImage_Crop(Image _img)
    {
        if (_img.sprite == null) return;
        _img.SetNativeSize();
        Image parent = _img.transform.parent.GetComponent<Image>();
        float parentWidth = parent.rectTransform.rect.width;
        float parentHeight = parent.rectTransform.rect.height;

        float childWidth = _img.rectTransform.rect.width;
        float childHeight = _img.rectTransform.rect.height;

        float parentAspectRatio = parentWidth / parentHeight;
        float childAspectRatio = childWidth / childHeight;

        float scaleFactor = 1f;

        if (childAspectRatio > parentAspectRatio)
        {
            scaleFactor = parentHeight / childHeight;
        }
        else
        {
            scaleFactor = parentWidth / childWidth;
        }

        _img.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }


    //public static void ZoomImage_Crop(Image _img)
    //{
    //    if (_img.sprite == null) return;
    //    _img.SetNativeSize();
    //    Image parent = _img.transform.parent.GetComponent<Image>();
    //    float parentWidth = parent.rectTransform.rect.width;
    //    float parentHeight = parent.rectTransform.rect.height;

    //    float childWidth = _img.rectTransform.rect.width;
    //    float childHeight = _img.rectTransform.rect.height;

    //    float scaleFactor;
    //    float scaleFactorWidth;
    //    float scaleFactorHeight;
    //    bool isLarge = true;
    //    if (childWidth > parentWidth)
    //    {
    //        scaleFactorWidth = parentWidth / childWidth;
    //    }
    //    else
    //    {
    //        isLarge = false;
    //        scaleFactorWidth = childWidth / parentWidth;
    //    }

    //    if (childHeight > parentHeight)
    //    {
    //        scaleFactorHeight = parentHeight / childHeight;
    //    }
    //    else
    //    {
    //        isLarge = false;
    //        scaleFactorHeight = childHeight / parentHeight;
    //    }

    //    scaleFactor = Mathf.Max(scaleFactorWidth, scaleFactorHeight);

    //    scaleFactor = isLarge ? scaleFactor : 1 / scaleFactor;

    //    _img.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    //}




    /// <summary>
    /// 꽉차게 논크롭 줌
    /// </summary>
    public static void ZoomImage_NonCrop(Image _img)
    {
        _img.SetNativeSize();
        Image parent = _img.transform.parent.GetComponent<Image>();
        float parentWidth = parent.rectTransform.rect.width;
        float parentHeight = parent.rectTransform.rect.height;

        float childWidth = _img.rectTransform.rect.width;
        float childHeight = _img.rectTransform.rect.height;

        float parentAspectRatio = parentWidth / parentHeight;
        float childAspectRatio = childWidth / childHeight;

        float scaleFactor = 1f;

        if (childAspectRatio < parentAspectRatio)
        {
            scaleFactor = parentHeight / childHeight;
        }
        else
        {
            scaleFactor = parentWidth / childWidth;
        }

        _img.rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }



    // 파일 생성 & 삭제
    /// <summary>
    /// 파일 생성
    /// </summary>
    /// <param name="_filePath"></param>
    /// <param name="_bytes"></param>
    public static void CreateFile(string _filePath, byte[] _bytes)
    {
        string directory = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        try
        {
            File.WriteAllBytes(_filePath, _bytes);
        }
        catch (Exception e)
        {
            
            Debug.LogError("파일생성 에러 : " + e.Message);
            throw;
        }

    }

    /// <summary>
    /// 파일 삭제 (단일)
    /// </summary>
    /// <param name="_filePath"></param>
    public static void DeleteFile(string _filePath)
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    /// <summary>
    /// 파일 삭제 (다중, 디렉토리 내 파일들 삭제)
    /// </summary>
    /// <param name="_directoryPath"></param>
    public static void DeleteFiles(string _directoryPath)
    {
        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
        DirectoryInfo directory = new DirectoryInfo(_directoryPath);
        foreach (FileInfo file in directory.GetFiles()) file.Delete();
        foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
    }


    private static string[] extensionsToReplace = { "heic", "heif", "heix", "hevc" };
    private static string replacementExtension = "png";

    /// <summary>
    /// ios이미지 확장자 컨버터
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string ConvertIOSExtension(string fileName)
    {
        foreach (string extension in extensionsToReplace)
        {
            if (fileName.Contains(extension, StringComparison.OrdinalIgnoreCase))
            {
                fileName = Regex.Replace(fileName, extension, replacementExtension, RegexOptions.IgnoreCase);
                break;
            }
        }
        return fileName;
    }

    #endregion



    #region 금칙어 관련
    /// <summary>
    /// 띄어쓰기 제한
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularTrimExpression(string str)
    {
        if (Regex.IsMatch(str, @"^\S$") == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 숫자 외 제한
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularNumberExpression(string str)
    {
        if (Regex.IsMatch(str, @"^[0-9]+\S$") == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 영어 대소문자, 숫자 외 제한
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularIDExpression(string str)
    {
        if (Regex.IsMatch(str, @"^[a-zA-Z0-9]+\S$") == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 영어 대소문자, 숫자, 기호 외 제한
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularPasswordExpression(string str)
    {
        if (Regex.IsMatch(str, @"^[a-zA-Z0-9!-/:-@'-~]+\S$") == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 영어 대소문자, 한글 음절 및 초성, 숫자 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularNickNameExpression(string str)
    {
        if (Regex.IsMatch(str, @"^[0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ]*$") == false)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 영어 대소문자, 숫자, 기호,한글 음절 및 초성 허용
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularEmailExpression(string str)
    {
        return Regex.IsMatch(str, @"^[0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ!-/:-@'-~]*$");
    }

    /// <summary>
    /// 영어 대소문자, 숫자, 기호,한글 음절 및 초성, 공백 허용
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularJobExpression(string str)
    {
        return Regex.IsMatch(str, @"^[0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ!-/:-@'-~\s]*$");
    }

    /// <summary>
    /// 영어 대소문자, 한글 음절 허용
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool RegularLanguageExpression(string str)
    {
        if (Regex.IsMatch(str, @"^[a-zA-Z가-힣]*$") == false)
        {
            return false;
        }
        return true;
    }

    public static bool RegularRoomNameExpression(string str)
    {
        return Regex.IsMatch(str, @"^[0-9a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ\s]*$");
    }

    /// <summary>
    /// 한글만 지움
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RegularReplaceWithoutKorean(string str)
    {
        string replaceStr = Regex.Replace(str, @"[^a-zA-Z0-9`!@#$%^&*()_+|\-=\\{}\[\]:"";'<>?,./]", "");
        return replaceStr;
    }

    /// <summary>
    /// 숫자 외에 다 지움
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string RegularReplaceOnlyNumber(string num)
    {
        string replaceStr = Regex.Replace(num, @"[^0-9]", "");
        return replaceStr;
    }

    /// <summary>
    /// 숫자를 핸드폰 번호로 만들기
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public static string RegularAddHyphenPhonNumber(string phoneNumber)
    {
        string replaceStr1 = Regex.Replace(phoneNumber, @"[^0-9]", "");
        string replaceStr2 = Regex.Replace(replaceStr1, @"(^02|^0505|^1[0-9]{3}|^0[0-9]{2})([0-9]+)?([0-9]{4})$", "$1-$2-$3");
        string replaceStr3 = replaceStr2.Replace("--", "-");
        return replaceStr3;
    }

    /// <summary>
    /// 한글 영어 외에 다 지움
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RegularReplaceWithoutKorNEng(string str)
    {
        string replaceStr = Regex.Replace(str, @"[^a-zA-Z가-힣ㄱ-ㅎㅏ-ㅣ]", "");
        return replaceStr;
    }

    /// <summary>
    /// 금칙어를 *로 변경
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ChangeForbiddenWordsToStar(string str)
    {
        string curStr = str;
        var forbiddenWords = Single.MasterData.dataForbiddenWords.GetList();
        int count = forbiddenWords.Count;
        for (int i = 0; i < count; i++)
        {

            if (curStr.Contains(forbiddenWords[i].text))
            {
                string forbiddenWord = forbiddenWords[i].text;
                int length = forbiddenWord.Length;
                string star = default;
                for (int j = 0; j < length; j++)
                {
                    star += '*';
                }
                curStr = curStr.Replace(forbiddenWord, star);
            }
        }
        return curStr;
    }

    /// <summary>
    /// 금칙어 포함 여부
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool CheckForbiddenWords(string str)
    {
        string curStr = str;
        var forbiddenWords = Single.MasterData.dataForbiddenWords.GetList();
        int count = forbiddenWords.Count;
        for (int i = 0; i < count; i++)
        {
            if (curStr.Contains(forbiddenWords[i].text))
            {
                return true;
            }
        }
        return false;
    }
    #endregion



    #region Disk용량 관련
    private const float disk = 0.001f;
    public static float KB(this int size)
    {
        return KB((float)size);
    }
    public static float KB(this float f)
    {
        for (int i = 0; i < (int)eDisk.KB; i++)
        {
            f *= disk;
        }
        return f;
    }
    public static float MB(this int size)
    {
        return MB((float)size);
    }
    public static float MB(this float f)
    {
        for (int i = 0; i < (int)eDisk.MB; i++)
        {
            f *= disk;
        }
        return f;
    }
    public static float GB(this int size)
    {
        return GB((float)size);
    }
    public static float GB(this float f)
    {
        for (int i = 0; i < (int)eDisk.GB; i++)
        {
            f *= disk;
        }
        return f;
    }
    public static float TB(this int size)
    {
        return TB((float)size);
    }
    public static float TB(this float f)
    {
        for (int i = 0; i < (int)eDisk.TB; i++)
        {
            f *= disk;
        }
        return f;
    }

    public static double ConvertSize(this double bytes, SizeUnit unit)
    {
        try
        {
            switch (unit)
            {
                case SizeUnit.B:
                    return bytes;
                case SizeUnit.KB:
                    return (bytes / 1024);
                case SizeUnit.MB:
                    return (bytes / Math.Pow(1024, 2));
                case SizeUnit.GB:
                    return (bytes / Math.Pow(1024, 3));
                case SizeUnit.TB:
                    return (bytes / Math.Pow(1024, 4));
                case SizeUnit.PB:
                    return (bytes / Math.Pow(1024, 5));
                case SizeUnit.EB:
                    return (bytes / Math.Pow(1024, 6));
                case SizeUnit.ZB:
                    return (bytes / Math.Pow(1024, 7));
                case SizeUnit.YB:
                    return (bytes / Math.Pow(1024, 8));
                default:
                    return bytes;
            }
        }
        catch
        {
            return -1;
        }
    }

    public static string ConvertSize(this string bytes, out SizeUnit unit)
    {
        var parsed = double.Parse(bytes);

        SizeUnit sizeUnit = SizeUnit.B;

        if (bytes.Length >= 4 && bytes.Length < 7)
        {
            sizeUnit = SizeUnit.KB;
        }
        else if (bytes.Length >= 7 && bytes.Length < 10)
        {
            sizeUnit = SizeUnit.MB;
        }
        else
        {
            sizeUnit = SizeUnit.GB;
        }

        unit = sizeUnit;

        var converted = parsed.ConvertSize(sizeUnit);
        var rounded = Math.Round(converted, 2);

        return rounded.ToString();
    }

    #endregion

    #region 인터넷 시간 컨버팅 
    public static string ConvertDate(string date)
    {
        DateTime str = DateTime.Parse(date);
        string convertStr = str.ToString("yy-MM-dd HH:mm");
        return convertStr;

        //string dummyTime = "2022 - 04 - 05T11: 45:20.000Z";
        //DateTime time = DateTime.Parse(dummyTime);

        //DateTime ToLocalTime = time.ToLocalTime(); // 날짜 오전오후 시간 풀버전          2022-04-05 PM 8:45:20
        //string ToLongDateString = time.ToLongDateString(); // 날짜 년월요일 풀버전       2022년 4월 5일 화요일
        //string ToLongTimeString = time.ToLongTimeString(); // 오전오후 시간 풀버전       PM 8:45:20
        //string ToShortDateString = time.ToShortDateString(); // 날짜 간략버전(구분자 -)  2022-04-05
        //string ToShortTimeString = time.ToShortTimeString(); // 오전오후 시간 간략버전   PM 8:45
        //string ToString = time.ToString("yyyy-MM-dd HH:mm:ss"); // 날짜 24시간 풀버전    2022-04-05 20:45:20
        //string ToStringDate = time.ToString("yyyy-MM-dd"); // 날짜 간략버전(구분자 -)    2022-04-05
        //string ToStringTime = time.ToString("HH:mm:ss"); // 24시간                      20:45:20
    }


    // 스트링 00(시간) : 00(분)을 넣었을 때 분으로 변환
    public static int ConvertTimeStringToMinute(string timestr)
    {
        string[] times = new string[2];
        times = timestr.Split(':');

        int result = int.Parse(times[0]) * 60 + int.Parse(times[1]);
        return result;
    }

    public static string ConvertMinuteToTimeString(int minute)
    {
        TimeSpan span = TimeSpan.FromMinutes(minute);
        string str = string.Format("{0:00}:{1:00}", span.Hours, span.Minutes);
        return str;
    }

    #endregion



    #region 그래픽
    public static T ChangeAlpha<T>(this T g, float newAlpha) where T : Graphic
    {
        var color = g.color;
        color.a = newAlpha;
        g.color = color;
        return g;
    }
    #endregion



    #region 애니메이션

    public static AnimatorControllerParameterType GetAnimatorParameterType(this Animator animator, string parameterName)
    {
        for (var index = 0; index < animator.parameters.Length; index++)
        {
            var param = animator.GetParameter(index);

            if (param.name == parameterName)
            {
                return param.type;
            }
        }
        return (AnimatorControllerParameterType)(-1);
    }

    public static bool CheckParameter(this Animator animator, string paramName)
    {
        return animator.parameters.Any(param => param.name == paramName);
    }

    #endregion



    #region 이펙트 실행 (오브젝트 풀링)

    public static GameObject GetPooledObject(string _name, Transform _parent)
    {
        if (ObjectPooler.instance == null) return null;

        GameObject effect = ObjectPooler.instance.GetPooledObject(_name);

        effect.transform.SetParent(_parent);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localRotation = Quaternion.identity;
        effect.transform.localScale = Vector3.one;

        effect.SetActive(true);

        return effect;
    }

    public static GameObject GetPooledObject(string _name, Vector3 _position, Quaternion _rotation)
    {
        if (ObjectPooler.instance == null) return null;

        GameObject effect = ObjectPooler.instance.GetPooledObject(_name);

        effect.transform.localPosition = _position;
        effect.transform.localRotation = _rotation;
        effect.transform.localScale = Vector3.one;

        effect.SetActive(true);

        return effect;
    }

    public static GameObject GetPooledObject(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        if (ObjectPooler.instance == null) return null;

        GameObject effect = ObjectPooler.instance.GetPooledObject(_name);

        effect.transform.localPosition = _position;
        effect.transform.localRotation = _rotation;
        effect.transform.localScale = _scale;

        effect.SetActive(true);

        return effect;
    }

    public static void Pool(string _name, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        if (ObjectPooler.instance == null) return;

        GameObject effect = ObjectPooler.instance.GetPooledObject(_name);

        effect.transform.localPosition = _position;
        effect.transform.localRotation = _rotation;
        effect.transform.localScale = _scale;

        effect.SetActive(true);
    }

    public static void Pool(string _name, Vector3 _position, Quaternion _rotation)
    {
        if (ObjectPooler.instance == null) return;

        GameObject effect = ObjectPooler.instance.GetPooledObject(_name);

        effect.transform.localPosition = _position;
        effect.transform.localRotation = _rotation;
        effect.transform.localScale = Vector3.one;

        effect.SetActive(true);
    }

    public static void Pool(string _name, Transform _parent)
    {
        if (ObjectPooler.instance == null) return;

        GameObject target = ObjectPooler.instance.GetPooledObject(_name);

        target.transform.SetParent(_parent);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;

        target.SetActive(true);
    }

    public static void Pool(string _name, Transform _parent, Vector3 _position)
    {
        if (ObjectPooler.instance == null) return;

        GameObject target = ObjectPooler.instance.GetPooledObject(_name);

        target.transform.SetParent(_parent);
        target.transform.localPosition = _position;
        target.transform.localRotation = Quaternion.identity;
        target.transform.localScale = Vector3.one;

        target.SetActive(true);
    }

    public static void Pool(string _name, Transform _parent, Vector3 _position, Quaternion _rotation)
    {
        if (ObjectPooler.instance == null) return;

        GameObject target = ObjectPooler.instance.GetPooledObject(_name);

        target.transform.SetParent(_parent);
        target.transform.localPosition = _position;
        target.transform.localRotation = _rotation;
        target.transform.localScale = Vector3.one;

        target.SetActive(true);
    }

    public static void Pool(string _name, Transform _parent, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        if (ObjectPooler.instance == null) return;

        GameObject target = ObjectPooler.instance.GetPooledObject(_name);

        if (!target)
        {
            DEBUG.LOGWARNING("풀링되어 있는 오브젝트가 존재하지 않습니다.");
            return;
        }

        target.transform.SetParent(_parent);
        target.transform.localPosition = _position;
        target.transform.localRotation = _rotation;
        target.transform.localScale = _scale;

        target.SetActive(true);
    }

    #endregion



    #region UI관련 (Toggle ...)

    public static void SetEventTrigger(this EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        var entry = new EventTrigger.Entry
        {
            eventID = type
        };

        entry.callback.AddListener(delegate (BaseEventData eventData) { action.Invoke(); });

        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// BKK의 UI체크함수
    /// </summary>
    /// <param name="point"></param>
    /// <param name="graphicRaycaster"></param>
    /// <returns></returns>
    public static bool UIExist(this Vector2 point, GraphicRaycaster graphicRaycaster)
    {
        var eventData = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();

        eventData.position = point;

        graphicRaycaster.Raycast(eventData, results);

        var uiExists = results.Count > 0;

        return uiExists;
    }

    /// <summary>
    /// 고선생의 UI체크함수
    /// </summary>
    /// <param name="graphicRaycaster"></param>
    /// <returns></returns>
    public static bool UIExist(GraphicRaycaster graphicRaycaster)
    {
        return UIExist(Input.mousePosition, graphicRaycaster);
    }

    /// <summary>
    /// 토글 온 하기 위한 유틸함수
    /// 참고자료 : https://srdeveloper.tistory.com/117?category=1069670
    /// </summary>
    /// <param name="tog"></param>
    public static void ToggleIsOn(ToggleGroup togg, Toggle tog)
    {
        togg.allowSwitchOff = true;
        tog.isOn = false;
        tog.isOn = true;
        togg.allowSwitchOff = false;
    }

    public static void TogglePP(Toggle tog)
    {
        tog.SetIsOnWithoutNotify(true);
    }

    /// <summary>
    /// UI 경고문구 상태 변경
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="entry"></param>
    /// <param name="go"></param>
    /// <param name="b"></param>

    public static void SetActive_Warning(TMP_Text txt, string id = null)
    {
        if (txt != null)
        {
            if (string.IsNullOrEmpty(id))
            {
                SetMasterLocalizing(txt, string.Empty);
                txt.gameObject.SetActive(false);
            }
            else
            {
                SetMasterLocalizing(txt, new MasterLocalData(id));
                txt.gameObject.SetActive(true);
            }
        }
    }

    #region 버튼 누르는 동안 패스워드 보여주기
    /// <summary>
    /// 버튼 누르는 동안 패스워드 보여주기
    /// </summary>
    /// <param name="eventTrigger"></param>
    /// <param name="inputField"></param>
    public static void AddTriggerEntry(EventTrigger eventTrigger, TMP_InputField inputField)
    {
        if (eventTrigger == null || inputField == null) return;

        eventTrigger.SetEventTrigger(EventTriggerType.PointerDown, () => OnPointer(inputField, TMP_InputField.ContentType.Standard));
        eventTrigger.SetEventTrigger(EventTriggerType.PointerUp, () => OnPointer(inputField, TMP_InputField.ContentType.Password));
    }

    private static void OnPointer(TMP_InputField inputField, TMP_InputField.ContentType type)
    {
        inputField.contentType = type;
        inputField.ForceLabelUpdate();
    }
    #endregion

    #region Select

    public static void SelectDown<T>() where T : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectDown<T>(currentSelected);
    }

    public static void SelectUp<T>() where T : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectUp<T>(currentSelected);
    }

    public static void SelectDown<T1, T2>()
        where T1 : Selectable
        where T2 : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectDown<T1, T2>(currentSelected);
    }

    public static void SelectUp<T1, T2>()
        where T1 : Selectable
        where T2 : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectUp<T1, T2>(currentSelected);
    }

    public static void SelectDown<T1, T2, T3>()
        where T1 : Selectable
        where T2 : Selectable
        where T3 : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectDown<T1, T2, T3>(currentSelected);
    }

    public static void SelectUp<T1, T2, T3>()
        where T1 : Selectable
        where T2 : Selectable
        where T3 : Selectable
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        SelectUp<T1, T2, T3>(currentSelected);
    }

    public static void SelectDown<T>(Selectable selectable) where T : Selectable
    {
        var next = selectable.FindSelectableOnDown();
        var isValid = next is T;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    public static void SelectUp<T>(Selectable selectable) where T : Selectable
    {
        var next = selectable.FindSelectableOnUp();
        var isValid = next is T;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    public static void SelectDown<T1, T2>(Selectable selectable)
        where T1 : Selectable
        where T2 : Selectable
    {
        var next = selectable.FindSelectableOnDown();
        var isValid = next is T1 || next is T2;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    public static void SelectUp<T1, T2>(Selectable selectable)
        where T1 : Selectable
        where T2 : Selectable
    {
        var next = selectable.FindSelectableOnUp();
        var isValid = next is T1 || next is T2;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    public static void SelectDown<T1, T2, T3>(Selectable selectable)
        where T1 : Selectable
        where T2 : Selectable
        where T3 : Selectable
    {
        var next = selectable.FindSelectableOnDown();
        var isValid = next is T1 || next is T2 || next is T3;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    public static void SelectUp<T1, T2, T3>(Selectable selectable)
        where T1 : Selectable
        where T2 : Selectable
        where T3 : Selectable
    {
        var next = selectable.FindSelectableOnUp();
        var isValid = next is T1 || next is T2 || next is T3;

        if (next != null && isValid)
        {
            next.Select();
        }
    }

    #endregion

    /// <summary>
    /// 프리팹 부모 설정 및 위치 초기화
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parent"></param>
    public static void SetParentPosition(GameObject obj, Transform parent)
    {
        obj.transform.SetParent(parent);
        obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        obj.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    /// <summary>
    /// UI 버튼에서 성공 시 상태 변경해줌
    /// </summary>
    private static Sprite selectSprite;
    public static Sprite GetBtnSelectSprite()
    {
        if (selectSprite == null)
        {
            selectSprite = Single.Resources.Load<Sprite>(Cons.Path_Image + "btn_bg_01_r30_03");
        }
        return selectSprite;
    }

    /// <summary>
    /// 로드 및 스토리지 다운 실패 시 임시로 보여줄 이미지
    /// </summary>
    private static Sprite dummyThumbnail = null;
    public static Sprite GetDummySprite()
    {
        if (dummyThumbnail == null)
        {
            try
            {
                dummyThumbnail = Resources.Load<Sprite>(Cons.Path_Image + "Icon_emoji");
            }
            catch
            {
                Debug.Log("Util.cs dummyThumbnail 로드 실패");
            }
        }
        return dummyThumbnail;
    }

    private static Sprite dummyLogo = null;
    public static Sprite GetLogoSprite()
    {
        if (dummyLogo == null)
        {
            try
            {
                
                dummyLogo = Resources.Load<Sprite>(Cons.Path_OfficeThumbnail + "thumb_exhibition001");
            }
            catch
            {
                Debug.Log("Util.cs dummyLogo 로드 실패");
            }
        }
        return dummyLogo;
    }


    #endregion



    #region 스톱워치(스크립트 시간 체크용)
    private static System.Diagnostics.Stopwatch sw;
    public static void StartStopwatch()
    {
        sw = new System.Diagnostics.Stopwatch();
        sw.Start(); // 스톱워치 시작.
    }
    public static void StopStopwatch()
    {
        sw.Stop(); // 스톱워치 끝.
        //Debug.Log("End Time: " + sw.ElapsedMilliseconds + "msec"); // 동작 시간을 밀리초로 출력.
        Debug.Log("End Time: " + sw.Elapsed); // 동작 시간을 출력.
    }
    #endregion
    public static void CreateEffect(string particleName, Vector3 pos)
    {
        GameObject go = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_Particle + particleName);
        go.transform.position = pos;
        UnityEngine.Object.Destroy(go, 2f);
    }

    #region 레이어 마스크 [임시, CWJ, 221019]

    public static void ChangeLayerMask(GameObject _gameObject, string _layer = "Default")
    {
        _gameObject.layer = LayerMask.NameToLayer(_layer);

        foreach (Transform child in _gameObject.transform)
        {
            if (null == child) continue;

            SetLayerRecursively(child.gameObject, LayerMask.NameToLayer(_layer));
        }
    }

    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    #endregion


    #region MEC

    public enum SceneCoroutine
    {
        None,
        Player,
        Manager,
        World,
        Game,
        Store,
    }

    public static CoroutineHandle RunCoroutine(IEnumerator<float> _body, string _tag = null, SceneCoroutine _scene = SceneCoroutine.None)
    {
        if (!string.IsNullOrEmpty(_tag)) Timing.KillCoroutines(_tag);
        return Timing.RunCoroutine(_body, (int)_scene, _tag);
    }

    public static CoroutineHandle RunCoroutineLayer(IEnumerator<float> _body, SceneCoroutine _scene = SceneCoroutine.None)
    {
        return Timing.RunCoroutine(_body, (int)_scene);
    }

    public static CoroutineHandle RunCoroutineTag(IEnumerator<float> _body, string _tag = null)
    {
        return Timing.RunCoroutine(_body, _tag);
    }

    public static void KillCoroutine(string _tag)
    {
        Timing.KillCoroutines(_tag);
    }

    public static void KillCoroutine(int _layer)
    {
        Timing.KillCoroutines(_layer);
    }

    public static void KillCoroutine(SceneCoroutine _scene)
    {
        Timing.KillCoroutines((int)_scene);
    }

    public static void KillCoroutineAfterSeconds(string _tag, float _delay, Action _action = null)
    {
        RunCoroutine(KillAfterDelay(_tag, _delay, _action));
    }

    public static IEnumerator<float> KillAfterDelay(string _tag, float _delay, Action _action = null)
    {
        yield return Timing.WaitForSeconds(_delay);

        KillCoroutine(_tag);

        _action?.Invoke();
    }

    public static IEnumerator<float> WaitUntilTrue(bool b)
    {
        yield return Timing.WaitUntilTrue(() => b);
    }

    public static IEnumerator<float> WaitUntilFalse(bool b)
    {
        yield return Timing.WaitUntilFalse(() => b);
    }

    #endregion

    #region ButtonTween

    private static Dictionary<int, Vector3> btnOriScaleDic = new Dictionary<int, Vector3>();
    public static IEnumerator<float> Co_ButtonTween(ButtonTweenComponent buttonTween, eButtonTweenState buttonTweenState)
    {
        eButtonTweenType buttonTweenType = buttonTween.buttonTweenType;
        GameObject[] gos = buttonTween.gos;
        Image img = buttonTween.selectable.image;
        SO_ButtonTween so_ButtonTween = Single.Resources.Load<SO_ButtonTween>(Path.Combine(Cons.Path_ScriptableObject, nameof(SO_ButtonTween)));
        ButtonTween buttonTweenContainer = so_ButtonTween.buttonTweens.FirstOrDefault(x => x.buttonTweenType == buttonTweenType);

        //버튼타입에 맞지 않으면 탈출
        if (buttonTweenContainer == null)
        {
            yield break;
        }

        for (int i = 0; i < gos.Length; i++)
        {
            Vector3 oriScale = gos[i].transform.localScale;
            int instanceId = gos[i].GetInstanceID();
            if (!btnOriScaleDic.ContainsKey(instanceId))
            {
                btnOriScaleDic.Add(instanceId, oriScale);
            }
        }

        float easeTime = 0f;
        Ease ease = Ease.EaseInBack;
        float fromScale = 0f;
        float toScale = 0f;
        img.raycastPadding = Vector4.zero;

        switch (buttonTweenState)
        {
            case eButtonTweenState.oriDown:
                {
                    easeTime = buttonTweenContainer.downTime;
                    ease = buttonTweenContainer.downEase;
                    fromScale = 1f;
                    toScale = buttonTweenContainer.downScale;
                    img.raycastPadding = Vector4.one * -50f;
                }
                break;
            case eButtonTweenState.downOri:
                {
                    easeTime = buttonTweenContainer.downTime;
                    ease = buttonTweenContainer.downEase;
                    fromScale = buttonTweenContainer.downScale;
                    toScale = 1f;
                }
                break;
            case eButtonTweenState.downUp:
                {
                    easeTime = buttonTweenContainer.upTime;
                    ease = buttonTweenContainer.upEase;
                    fromScale = buttonTweenContainer.downScale;
                    toScale = buttonTweenContainer.upScale;
                }
                break;
            case eButtonTweenState.upOri:
                {
                    easeTime = buttonTweenContainer.oriTime;
                    ease = buttonTweenContainer.oriEase;
                    fromScale = buttonTweenContainer.upScale;
                    toScale = 1f;
                }
                break;
        }

        //스케일전환부
        float curTime = 0f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / easeTime;
            Function function = GetEasingFunction(ease);
            float f2 = Mathf.Lerp(fromScale, toScale, function(0f, 1f, curTime));
            for (int i = 0; i < gos.Length; i++)
            {
                if (gos[i] == null) continue;

                gos[i].transform.localScale = btnOriScaleDic[gos[i].GetInstanceID()] * f2;
            }
            yield return Timing.WaitForOneFrame;
        }
    }






    #endregion

    #region Youtube

    /// <summary>
    /// 유튜브 링크 원 동영상 주소 컨버트
    /// </summary>
    /// <param name="youtubeUrl"></param>
    /// <param name="resolution"></param>
    /// <returns></returns>
    public static string ConvertPlayableYoutubeLink(string youtubeUrl, int resolution = 360)
    {
        if (!IsYoutubeVideo(youtubeUrl)) return string.Empty;

        string youtubeCoreUrl = string.Empty;

        if (youtubeUrl.Contains("youtu.be/")) // 공유용 축약 링크일 경우
        {
            string temp = youtubeUrl.Split(new string[] { ".be/" }, StringSplitOptions.None)[1];
            youtubeCoreUrl = temp.Split(new string[] { "?" }, StringSplitOptions.None)[0];
        }
        else if (youtubeUrl.Contains("youtube.com/")) // 일반 링크일 경우
        {
            //NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(new Uri(youtubeUrl).Query);
            //id = queryString["v"];
            youtubeCoreUrl = youtubeUrl.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
        }

        //string preUrl = "http://20.214.167.193:2023/watch";
        string preUrl = $"{Single.Web.YoutubeDlUrl}/watch";

        string query = 
            AddQuery("v", youtubeCoreUrl)
            .AddQuery("cli", "yt-dlp")
            .AddQuery("bestvideo[height<", $"?{resolution}]")
            .MakeQuery();
        

        string url = youtubeCoreUrl == string.Empty ? string.Empty : preUrl + query;

        return url;


        //: $"{Single.Web.YoutubeDlUrl}/watch?v={id}&cli=yt-dlp&bestvideo[height<=?{resolution}]";
    }

    /// <summary>
    /// 쿼리 만들기, 최초 만드는 부분
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, string> AddQuery(string key, string value)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        return dic.AddQuery(key, value);
    }
    
    /// <summary>
    /// 쿼리 만들기, 중간 부분
    /// </summary>
    /// <param name="query"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, string> AddQuery(this Dictionary<string, string> query, string key, string value)
    {
        query.Add(key, value);
        return query;
    }

    /// <summary>
    /// 쿼리 만들기, 마지막 부분
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string MakeQuery(this Dictionary<string, string> query)
    {
        if (query.Count < 0) return string.Empty;

        else
        {
            string value = "?";

            foreach (var element in query)
            {
                value += element.Key + "=" + element.Value + "&";
            }

            return value.Remove(value.Length - 1);
        }
        //query
    }
    public static bool IsYoutubeVideo(this string url)
    {
        return url.Contains("youtube.com/watch?") || url.Contains("youtu.be/");
    }

    #endregion
    
    /// <summary>
    /// 러프 인벌스러프
    /// </summary>
    /// <param name="fromA"></param>
    /// <param name="fromB"></param>
    /// <param name="toA"></param>
    /// <param name="toB"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static float LerpInverseLerp(float fromA, float fromB, float toA, float toB, float val)
    {
        return Mathf.Lerp(toA, toB, Mathf.InverseLerp(fromA, fromB, val));
    }

    /// <summary>
    /// 안드로이드일때와 에디터모드일때 패스 구하기
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadAllText(string path)
    {
        string combinePath = Path.Combine(Application.streamingAssetsPath, path);
        string convertPath = AndroidPersistentDataPath(combinePath);
        return File.ReadAllText(convertPath);
    }
    public static byte[] ReadAllByte(string path)
    {
        string combinePath = Path.Combine(Application.streamingAssetsPath, path);
        string convertPath = AndroidPersistentDataPath(combinePath);
        return File.ReadAllBytes(convertPath);
    }

    /// <summary>
    /// 안드로이드패스 컨버팅용
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string AndroidPersistentDataPath(string path)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        WWW reader = new WWW(path);
        while (!reader.isDone) { }
        string androidPath = Application.persistentDataPath + "/db";
        File.WriteAllBytes(androidPath, reader.bytes);

        path = File.ReadAllText(androidPath);
#endif
        return path;
    }
    public static void WriteAllText(string path, string data)
    {
        string combinePath = Path.Combine(Application.streamingAssetsPath, path);
        string convertPath = AndroidPersistentDataPath(combinePath);
        File.WriteAllText(convertPath, data);

    }


    /// <summary>
    /// 스크롤뷰 리프레쉬
    /// </summary>
    /// <param name="go"></param>
    /// <param name="objName"></param>
    public static void RefreshScrollView(GameObject go, string objName)
    {
        ScrollRect scrollRect = Search<ScrollRect>(go, objName);
        scrollRect.content.anchoredPosition = Vector3.zero;
    }

    /// <summary>
    /// 컨텐츠사이즈피터 버그가 있어 레이아웃 갱신하는 함수
    /// </summary>
    /// <param name="go"></param>
    /// <param name="objName"></param>
    public static async void RefreshLayout(GameObject go, string objName)
    {
        bool exception = await UniTask.WaitUntil(() => go.activeSelf).TimeoutWithoutException(TimeSpan.FromSeconds(3f)); //3초안에 켜지지 않는 상황이면 패스
        
        if (exception) return;

        RectTransform rectTr = Search<RectTransform>(go, objName);
        if (!rectTr)
        {
            Debug.Log("리프레쉬할게없음");
            return;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTr);
    }

    public static Dictionary<string, int> ConvertJsonToDic(string json)
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic = JsonConvert.DeserializeObject<Dictionary<string, int>>(json).ToDictionary(x => x.Key, x => x.Value);
        return dic;
    }

    /// <summary>
    /// 요일 구하기
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static string GetDayOfWeek(DayOfWeek _dayOfWeek)
    {
        return GetMasterLocalizing(dayOfWeek[(int)_dayOfWeek]);
    }

    public static int GetDayOfWeekIndex(DayOfWeek dayOfWeek)
    {
        int result = -1;

        switch (dayOfWeek)
        {
            case DayOfWeek.Sunday: result = 0; break;
            case DayOfWeek.Monday: result = 1; break;
            case DayOfWeek.Tuesday: result = 2; break;
            case DayOfWeek.Wednesday: result = 3; break;
            case DayOfWeek.Thursday: result = 4; break;
            case DayOfWeek.Friday: result = 5; break;
            case DayOfWeek.Saturday: result = 6; break;
        }

        return result;
    }

    /// <summary>
    /// 시분을 분으로 변경
    /// </summary>
    public static int HourMin2Min(int _hour, int _min)
    {
        int min = _hour * 60;
        min += _min;
        return min;
    }

    /// <summary>
    /// 분을 시분으로 변경
    /// </summary>
    public static (int hour, int min) Min2HourMin(int _min)
    {
        int hour = _min / 60;
        int min = _min % 60;
        return (hour, min);
    }

    /// <summary>
    /// 데이트타임 변경
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="Year"></param>
    /// <param name="Month"></param>
    /// <param name="Day"></param>
    /// <returns></returns>
    public static DateTime ChangeTime(DateTime dateTime, int Year = 0, int Month = 0, int Day = 0, int Hour = 0, int Min = 0, int Second = 0)
    {
        return new DateTime(
            Year != 0 ? Year : dateTime.Year,
            Month != 0 ? Month : dateTime.Month,
            Day != 0 ? Day : dateTime.Day,
            Hour != 0 ? Hour : dateTime.Hour,
            Min != 0 ? Min : dateTime.Minute,
            Second != 0 ? Second : dateTime.Second,
            dateTime.Millisecond,
            dateTime.Kind);
    }

    public static DateTime String2DateTime(string _reservationAt, int _startTime)
    {
        int hour = Min2HourMin(_startTime).hour;
        int minute = Min2HourMin(_startTime).min;
        int second = 0;

        string time = string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

        DateTime startTime = Convert.ToDateTime(time);

        DateTime reservationTime = Util.ChangeTime(System.DateTime.Parse(_reservationAt), 0, 0, 0, startTime.Hour, startTime.Minute, startTime.Second);

        return reservationTime;
    }

    public static string DateTime2String(DateTime _time)
    {
        string date = string.Format("{0:yyyy/MM/dd}", _time);
        string time = "AM";

        int hour = _time.Hour;
        int minute = _time.Minute;
        int second = _time.Second;

        if (hour > 12)
        {
            hour = hour - 12;
            time = time.Replace("AM", "PM");
        }

        time += " " + string.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);

        return date + " " + time;
    }

    public static string Int2StringTime(int _time)
    {
        string time = "AM";

        int hour = Min2HourMin(_time).hour;
        int minute = Min2HourMin(_time).min;

        if (hour > 12)
        {
            hour = hour - 12;
            time = time.Replace("AM", "PM");
        }

        return time + string.Format(" {0:D2}:{1:D2}:{2:D2}", hour, minute, 0);
    }

    private static string[] dayOfWeek =
        {
        "office_reservation_sun",
        "office_reservation_mon",
        "office_reservation_tue",
        "office_reservation_wed",
        "office_reservation_thu",
        "office_reservation_fri",
        "office_reservation_sat",
    };

    public static string Int2DayOfWeek(int _repeatDay)
    {
        List<bool> days = CheckDayOfWeek(_repeatDay);
        string repeatDate = String.Empty;

        for (int i = 0; i < days.Count; i++)
        {
            if (!days[i]) continue;

            repeatDate += GetMasterLocalizing(dayOfWeek[i]);
        }

        return repeatDate;
    }

    /// <summary>
    /// 일~토 까지 예약날짜 true
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<bool> CheckDayOfWeek(int value)
    {
        List<bool> dayOfWeekList = new List<bool>();
        for (int i = 0; i < EnumLength<DayOfWeek>(); i++)
        {
            int result = value & (1 << i);
            dayOfWeekList.Add(result != 0 ? true : false);
        }
        return dayOfWeekList;
    }


    /// <summary>
    /// 앞자리 2개 영어/ 뒷자리 4개 숫자 
    /// </summary>
    /// <returns></returns>
    public static string MakeRandomPassword()
    {
        System.Random random = new System.Random();
        const string englishChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChar = "0123456789";
        var str1 = new string(Enumerable.Repeat(englishChar, 2).Select(s => s[random.Next(s.Length)]).ToArray());
        var str2 = new string(Enumerable.Repeat(numberChar, 4).Select(s => s[random.Next(s.Length)]).ToArray());
        return str1 + str2;
    }

    /// <summary>
    /// 클립보드로 복사
    /// </summary>
    /// <param name="str"></param>
    public static void CopyToClipboard(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }

    /// <summary>
    /// 클립보드에서 가져오기
    /// </summary>
    /// <returns></returns>
    public static string CopyFromClipboard()
    {
        return GUIUtility.systemCopyBuffer;
    }

    #region 로컬라이징

    /// <summary>
    /// 마스터로컬라이징 셋팅
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="masterLocalData"></param>    
    public static void SetMasterLocalizing(MonoBehaviour mono, string str)
    {
        if (mono == null)
        {
            return;
        }

        if (!mono.TryGetComponent(out LocalizingEvent localizingEvent))
        {
            localizingEvent = mono.gameObject.AddComponent<LocalizingEvent>();
        }
        localizingEvent.SetString(str);
    }

    public static void SetMasterLocalizing(MonoBehaviour mono, MasterLocalData masterLocalData)
    {
        if (mono == null)
        {
            return;
        }

        if (!mono.TryGetComponent(out LocalizingEvent localizingEvent))
        {
            localizingEvent = mono.gameObject.AddComponent<LocalizingEvent>();
        }
        localizingEvent.SetMasterLocalizing(masterLocalData);
    }

    /// <summary>
    /// 마스터로컬스트링 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string GetMasterLocalizing(MasterLocalData masterLocalData)
    {
        return GetMasterLocalizing(masterLocalData.id, masterLocalData.args);
    }
    public static string GetMasterLocalizing(string id, params object[] args)
    {
        if (string.IsNullOrEmpty(id)) return "";
        var localizeData = Single.MasterData.GetLocalizeData(id);

        string result;
        string language = string.Empty;
        try
        {
            switch (AppGlobalSettings.
                Instance.language)
            {
                case Language.Korean: language = localizeData.kor; break;
                case Language.English: language = localizeData.eng; break;
                default: language = localizeData.eng; break;
            };
            result = args == null || args.Length == 0 ? language : string.Format(language, args);
        }
        catch
        {
            //result = $"로컬라이징필요 - {id}";
            result = id;
        }
        return result;
    }



    #endregion


    #region 렉트 트랜스폼(Recttransform)

    // 복사 버튼을 눌렀을 때 실행되는 함수
    public static void CopyRectTransform(RectTransform ori, RectTransform set)
    {
        ori.anchorMin = set.anchorMin;
        ori.anchorMax = set.anchorMax;
        ori.pivot = set.pivot;
        ori.anchoredPosition = set.anchoredPosition;
        ori.sizeDelta = set.sizeDelta;
        ori.localEulerAngles = set.localEulerAngles;
    }
    #endregion

    #region 비교
    public static bool DictionaryEqual<TKey, TValue>(Dictionary<TKey, TValue> a, Dictionary<TKey, TValue> b)
    {
        // 사전의 크기가 같은지 확인
        if (a.Count != b.Count)
            return false;

        // 각 키/값 쌍이 같은지 확인
        foreach (KeyValuePair<TKey, TValue> pair in a)
        {
            TValue value;
            if (b.TryGetValue(pair.Key, out value))
            {
                // 키가 존재하고 값을 비교
                if (!EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                    return false;
            }
            else
            {
                // 키가 존재하지 않음
                return false;
            }
        }
        return true;
    }
    #endregion

    #region 이벤트 Queue 직렬실행(프로세스화)
    private static Dictionary<string, Queue<Action>> actionQueue = new Dictionary<string, Queue<Action>>();
    private static Dictionary<string, bool> isActionQueue = new Dictionary<string, bool>();

    /// <summary>
    /// 코루틴
    /// </summary>
    /// <param name="action"></param>
    public static void ProcessQueue(string key, Action action, float delay = 0.1f)
    {
        DicQueue(ref actionQueue, key, action);

        if (!isActionQueue.ContainsKey(key))
        {
            isActionQueue.Add(key, false);
        }
        if (!isActionQueue[key])
        {
            RunCoroutine(CoProcessQueue(key, delay), key);
        }
    }

    private static IEnumerator<float> CoProcessQueue(string key, float delay = 0.1f)
    {
        isActionQueue[key] = true;

        while (actionQueue[key].Count > 0)
        {
            yield return Timing.WaitForSeconds(delay);
            Action action = actionQueue[key].Dequeue();
            action.Invoke();
        }

        isActionQueue[key] = false;
    }

    public static void ClearProcessQueue(string key)
    {
        KillCoroutine(key);
        if (isActionQueue.ContainsKey(key))
            isActionQueue[key] = false;
        if (actionQueue.ContainsKey(key))
            actionQueue[key].Clear();
    }
    #endregion

    #region 코루틴 직렬 실행
    private static Dictionary<string, Queue<IEnumerator<float>>> coroutineQueue = new Dictionary<string, Queue<IEnumerator<float>>>();
    private static Dictionary<string, bool> isCoroutineQueue = new Dictionary<string, bool>();

    public static void ProcessQueue_Coroutine(string key, IEnumerator<float> coroutine, string tag = null, Action<bool> actStartDone = null)
    {
        DicQueue(ref coroutineQueue, key, coroutine);

        if (!isCoroutineQueue.ContainsKey(key))
        {
            isCoroutineQueue.Add(key, false);
        }
        if (!isCoroutineQueue[key])
        {
            CoProcessQueue_Coroutine(key, tag, actStartDone).RunCoroutine("CoProcessQueue_Coroutine");
        }
    }

    private static IEnumerator<float> CoProcessQueue_Coroutine(string key, string tag = null, Action<bool> actStartDone = null)
    {
        isCoroutineQueue[key] = true;
        actStartDone?.Invoke(true);

        while (coroutineQueue[key].Count > 0)
        {
            yield return Timing.WaitUntilDone(coroutineQueue[key].Dequeue(), tag);
        }

        isCoroutineQueue[key] = false;
        actStartDone?.Invoke(false);
    }

    public static void ClearQueue_Coroutine(string key, string tag = null, Action actClear = null)
    {
        if (!isCoroutineQueue.ContainsKey(key) || !isCoroutineQueue[key]) return;

        if (!string.IsNullOrEmpty(tag))
        {
            KillCoroutine(tag);
        }

        KillCoroutine("CoProcessQueue_Coroutine");

        actClear?.Invoke();

        coroutineQueue[key].Clear();
        isCoroutineQueue[key] = false;
    }
    #endregion

    #region 비밀번호 제한
    /// <summary>
    /// 비밀번호 제한
    /// 8~20 자 영문,숫자. 특수문자 입력 가능
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool PasswordRestriction(string password)
    {
        return password.Length >= 8 && password.Length <= 20 && RegularPasswordExpression(password); ;
    }

    /// <summary>
    /// 이메일 제한
    /// 0~64 자 영어 대소문자, 숫자, 기호,한글 음절 및 초성 입력 가능, @ 포함 필수
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool EmailRestriction(string email)
    {
        return email.Length <= 64 && RegularEmailExpression(email) && email.Contains("@");
    }

    /// <summary>
    /// 닉네임 제한
    /// 2~12 자 영어 대소문자, 한글 음절 및 초성, 숫자 입력 가능
    /// </summary>
    /// <param name="nickname"></param>
    /// <returns></returns>
    public static bool NicknameRestriction(string nickname)
    {
        return nickname.Length >= 2 && nickname.Length <= 12 && RegularNickNameExpression(nickname);
    }

    /// <summary>
    /// 상태메시지 제한
    /// 120자. 제한 없음
    /// </summary>
    /// <param name="stateMessage"></param>
    /// <returns></returns>
    public static bool StateMessageRestriction(string stateMessage)
    {
        return stateMessage.Length <= 120;
    }

    /// <summary>
    /// 인증코드 제한
    /// 1~4 자 숫자 입력 가능
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static bool AuthCodeRestriction(string code)
    {
        return code.Length >= 1 && code.Length <= 4 && RegularNumberExpression(code);
    }

    /// <summary>
    /// 202309 유학박람회 파일 이름 제한
    /// 한글, 영문, 숫자, 띄어쓰기 포함 20자 입력 가능
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static bool FileNameRestriction(string fileName)
    {
        return fileName.Length >= 1 && fileName.Length <= 20 && RegularJobExpression(fileName);
    }


    // 명함===========

    /// <summary>
    /// 명함 이름 제한
    /// 2~20 자 한글, 영문 입력 가능
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool BizNameRestriction(string name)
    {
        return name.Length >= 2 && name.Length <= 20 && RegularLanguageExpression(name);
    }

    /// <summary>
    /// 명함 직업 제한
    /// 2~20 자 한글, 영문, 숫자, 특수문자 입력 가능
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public static bool BizJobRestriction(string job)
    {
        return job.Length >= 2 && job.Length <= 20 && RegularJobExpression(job);
    }

    /// <summary>
    /// 명함 핸드폰 번호 제한
    /// 2~12 자 숫자 입력 가능
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static bool BizPhoneNumberRestriction(string number)
    {
        return number.Length >= 2 && number.Length <= 12 && RegularNumberExpression(number);
    }

    /// <summary>
    /// 이메일 제한
    /// 2~64 자 한글, 영문, 숫자, 특수문자 입력 가능
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static bool BizEmailRestriction(string email)
    {
        return email.Length >= 2 && email.Length <= 64 && RegularEmailExpression(email);
    }

    #endregion

    #region 캡쳐
    public static string capturePath
    {
        get
        {
            string folderName = "capture";
            string folderPath = Path.Combine(Application.dataPath, "..", folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }
    }
    public static string CaptureName() => "Screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
    public static string captureFullPath() => Path.Combine(capturePath, CaptureName());
    #endregion

    #region 아바타, 썸네일
    public static void SetAvatarParts(GameObject player, string _avatarData)
    {
        var avatarPartsController = player.GetComponentInChildren<AvatarPartsController>();

        if (avatarPartsController != null)
        {
            avatarPartsController.SetAvatarParts(_avatarData);
        }
    }

    /// <summary>
    /// 실시간 아바타 코스튬 정보 변경
    /// </summary>
    /// <param name="info"></param>
    public static void SendAvatarInfoRealTime(Dictionary<string, int> info)
    {
        if (MyPlayer.instance != null)
        {
            var data = new Protocol.C_BASE_SET_OBJECT_DATA
            {
                ObjectId = MyPlayer.instance.NetworkObserver.objectId,
                ObjectData = JsonConvert.SerializeObject(info)
            };
            Single.RealTime.Send(data);
        }
    }
    #endregion

    #region 랜덤 트랜스폼 계산
    public static (Vector3 position, Vector3 eulerAngle) RandomSpawn(Transform targetPoint, float range = 2f)
    {
        Vector3 pos = targetPoint.position;
        Vector3 angle = targetPoint.eulerAngles;

        pos += PositionOffset(range);
        angle += EulerAngleOffset();

        return (pos, angle);
    }

    public static Vector3 PositionOffset(float range)
    {
        return Vector3.right * UnityEngine.Random.Range(-range, range) + Vector3.forward * UnityEngine.Random.Range(-range, range);
    }

    public static Vector3 EulerAngleOffset()
    {
        return Vector3.up * UnityEngine.Random.Range(-360f, 360f);
    }
    #endregion  

    public static List<T> DeepCopy<T>(List<T> source) where T : ICloneable
    {
        if (source == null)
            return null;

        List<T> copy = new List<T>(source.Count);
        foreach (T item in source)
        {
            if (item is ICloneable cloneableItem)
                copy.Add((T)cloneableItem.Clone());
            else
                copy.Add(item);
        }

        return copy;
    }


    public static bool EqualMyRoomList<T>(T t1, T t2)
    {
        string jsonA = JsonConvert.SerializeObject(t1); //에디터모드 저장 데이터
        string jsonB = JsonConvert.SerializeObject(t2); //에디터모드 저장 데이터
        return jsonA.Equals(jsonB);
    }
    public static bool EqualMyRoomList<T>(T[] t1, T[] t2) where T : IComparable<T>
    {
        string jsonA = JsonConvert.SerializeObject(t1.OrderBy(x => x).ToArray()); //에디터모드 저장 데이터
        string jsonB = JsonConvert.SerializeObject(t2.OrderBy(x => x).ToArray()); //에디터모드 저장 데이터
        return jsonA.Equals(jsonB);
    }


    /// <summary>
    /// 이미지데이터 해쉬값 추출
    /// </summary>
    /// <param name="imageData"></param>
    /// <returns></returns>
    public static string CalculateMD5Hash(byte[] imageData)
    {
        using (var md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(imageData);
            string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hash;
        }
    }

    public static GameObject LoadInteriorPrefab(int itemId)
    {
        try
        {
            db.Item item = Single.MasterData.GetItem(itemId);

            //기존폴더
            int layerType = Single.MasterData.dataInteriorInstallInfo.GetData(item.id).layerType;
            string layerName = Single.MasterData.dataLayerType.GetData(layerType).name;
            string path = Path.Combine(Cons.Path_MyRoom, layerName, item.prefab);

            //새로운폴더
            //string categoryType = MasterDataManager.Instance.dataCategoryType.GetData(item.categoryType).name;
            //categoryType  = categoryType.Split('_').Last();
            //string path = Path.Combine(Cons.Path_MyRoom, categoryType, item.prefab);

            return Resources.Load<GameObject>(path);
        }
        catch
        {
            return null;
        }
    }


    public static void AdjustOffsets(RectTransform rect, Vector2 originalOffsetMin, Vector2 originalOffsetMax, float amount)
    {
        // 현재 offset 값을 가져오기
        Vector2 currentOffsetMin = originalOffsetMin;
        Vector2 currentOffsetMax = originalOffsetMax;

        // offset 값을 조정하기 위해 amount를 더해주거나 빼줌
        currentOffsetMin += new Vector2(-amount, -amount);
        currentOffsetMax += new Vector2(amount, amount);

        // 수정된 offset 값을 적용하여 RectTransform 업데이트
        rect.offsetMin = currentOffsetMin;
        rect.offsetMax = currentOffsetMax;
    }

    public enum MouseScrollWheelState
    {
        Normal,
        Up,
        Down,
    }

    /// <summary>
    /// 마우스 스크롤 휠
    /// </summary>
    /// <returns></returns>
    public static MouseScrollWheelState MouseScrollWheel()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        MouseScrollWheelState mouseScrollWheelState = MouseScrollWheelState.Normal;
        if (mouseWheel > 0f)
        {
            // 마우스 휠을 위로 스크롤했을 때 실행할 코드
            //Debug.Log("마우스 휠을 위로 스크롤했습니다.");
            mouseScrollWheelState = MouseScrollWheelState.Up;
        }
        else if (mouseWheel < 0f)
        {
            // 마우스 휠을 아래로 스크롤했을 때 실행할 코드
            //Debug.Log("마우스 휠을 아래로 스크롤했습니다.");
            mouseScrollWheelState = MouseScrollWheelState.Down;
        }
        return mouseScrollWheelState;
    }

    /// <summary>
    /// json Beautify(줄맞춤)
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static string Beautify(string jsonString)
    {
        try
        {
            string beautifiedJson = JValue.Parse(jsonString).ToString(Formatting.Indented);
            return beautifiedJson;
        }
        catch
        {
            return jsonString;
        }
    }


    /// <summary>
    /// 가져오던지 추가하던지
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : MonoBehaviour
    {
        if (!go.TryGetComponent(out T t))
        {
            t = go.AddComponent<T>();
        }
        return t;
    }
}
