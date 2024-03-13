using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using GamePotUnity;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using UnityEngine.UI;
using CryptoWebRequestSample;
using Vuplex.WebView;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using Cysharp.Threading.Tasks;

public class View_Account : UIBase
{
    #region 변수
    public TogglePlus togplus_Social_arzMETA { get; private set; }
    public TogglePlus togplus_Social_naver { get; private set; }
    public TogglePlus togplus_Social_google { get; private set; }
    public TogglePlus togplus_Social_apple { get; private set; }

    private TMP_Text txtmp_arzMETA_On;
    private TMP_Text txtmp_Naver_On;
    private TMP_Text txtmp_Google_On;
    private TMP_Text txtmp_Apple_On;
    private TMP_Text txtmp_arzMETA_Off;
    private TMP_Text txtmp_Naver_Off;
    private TMP_Text txtmp_Google_Off;
    private TMP_Text txtmp_Apple_Off;
    private TMP_Text txtmp_AccountType;
    private Button btn_ChangePassword;

    public int linkProviderType = 0;
    private string pwd = string.Empty;

    // 로그아웃을 하거나 앱을 종료하지 않았을 때, 한번 로그인 했던 소셜 로그인이 자동 로그인이 되는 경우를 방지하기 위한 flag
    public bool firstLogin = true;
    #endregion

    #region 초기화
    protected override void Awake()
    {
        base.Awake();

        // GamePotManager 콜백 등록 하기
        GamePotManager.cbLoginCancel = onLoginCancel;
        GamePotManager.cbLoginSuccess = onLoginSuccess;
        GamePotManager.cbLogoutFailure = onLogoutFailure;
        GamePotManager.cbLogoutSuccess = onLogoutSuccess;
        GamePotManager.cbMemberDelete = onDeleteMemberSuccess;
    }

    protected override void Start()
    {
        base.Start();

        // LinkedAccount
        // 계정 연동 Toggle on/off Action 추가
        togplus_Social_arzMETA.SetToggleOnAction(() => OnClick_LoginSDKForLink(LOGIN_PROVIDER_TYPE.ARZMETA));
        togplus_Social_naver.SetToggleOnAction(() => OnClick_LoginSDKForLink(LOGIN_PROVIDER_TYPE.NAVER));
        togplus_Social_google.SetToggleOnAction(() => OnClick_LoginSDKForLink(LOGIN_PROVIDER_TYPE.GOOGLE));
        togplus_Social_apple.SetToggleOnAction(() => OnClick_LoginSDKForLink(LOGIN_PROVIDER_TYPE.APPLE));

        // RealeseLinkedAccount
        togplus_Social_arzMETA.SetToggleOffAction(() => IsLinkAccount(LOGIN_PROVIDER_TYPE.ARZMETA));
        togplus_Social_naver.SetToggleOffAction(() => IsLinkAccount(LOGIN_PROVIDER_TYPE.NAVER));
        togplus_Social_google.SetToggleOffAction(() => IsLinkAccount(LOGIN_PROVIDER_TYPE.GOOGLE));
        togplus_Social_apple.SetToggleOffAction(() => IsLinkAccount(LOGIN_PROVIDER_TYPE.APPLE));
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ConnectPanelUpdate();
    }

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_arzMETA_On = GetUI_TxtmpMasterLocalizing(nameof(txtmp_arzMETA_On), new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.ARZMETA));
        txtmp_Naver_On = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Naver_On), new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.NAVER));
        txtmp_Google_On = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Google_On), new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.GOOGLE));
        txtmp_Apple_On = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Apple_On), new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.APPLE));
        txtmp_arzMETA_Off = GetUI_TxtmpMasterLocalizing(nameof(txtmp_arzMETA_Off), new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.ARZMETA));
        txtmp_Naver_Off = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Naver_Off), new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.NAVER));
        txtmp_Google_Off = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Google_Off), new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.GOOGLE));
        txtmp_Apple_Off = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Apple_Off), new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.APPLE));
        txtmp_AccountType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AccountType));

        GetUI_TxtmpMasterLocalizing("txtmp_ConnectTitle", new MasterLocalData("10211"));
        GetUI_TxtmpMasterLocalizing("txtmp_InfoTitle", new MasterLocalData("setting_account_info"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCodeTitle", new MasterLocalData("014"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCode", LocalPlayerData.MemberCode);
        GetUI_TxtmpMasterLocalizing("txtmp_Copy", new MasterLocalData("009"));
        GetUI_TxtmpMasterLocalizing("txtmp_Logout", new MasterLocalData("9101"));
        GetUI_TxtmpMasterLocalizing("txtmp_Center", new MasterLocalData("setting_cs"));
        #endregion

        #region Toggle
        togplus_Social_arzMETA = GetUI<TogglePlus>(nameof(togplus_Social_arzMETA));
        togplus_Social_naver = GetUI<TogglePlus>(nameof(togplus_Social_naver));
        togplus_Social_google = GetUI<TogglePlus>(nameof(togplus_Social_google));
        togplus_Social_apple = GetUI<TogglePlus>(nameof(togplus_Social_apple));
        #endregion

        #region Button
        btn_ChangePassword = GetUI_Button(nameof(btn_ChangePassword), () => PushPopup<Popup_AccountManagement>());

        GetUI_Button("btn_Copy", () =>
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData(Util.GetMasterLocalizing("014") + $" {LocalPlayerData.MemberCode} " + Util.GetMasterLocalizing("009"))));
            Util.CopyToClipboard(LocalPlayerData.MemberCode);
        });
        GetUI_Button("btn_Center", OnClick_Center);
        GetUI_Button("btn_Logout", OnClick_Logout);
        GetUI_Button("btn_Notice", OnClick_GamePotNotice);
        #endregion
    }
    #endregion

    #region Method
    /// <summary>
    /// 아즈메타 계정 비밀번호 변경 버튼 비/활성화
    /// </summary>
    private void SetActiveChangePassword(bool active)
    {
        if (btn_ChangePassword != null)
        {
            btn_ChangePassword.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// 고객센터 웹뷰 열기
    /// </summary>
    private void OnClick_Center()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken)},
                { "type", Util.EnumInt2String(ARZMETA_HOMEPAGE_TYPE.Center) },
                { "appVersion",  Application.version},
                { "deviceModel",  SystemInfo.deviceModel},
                { "deviceOs", SystemInfo.operatingSystem}
            };

        CallBack();

        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, Single.Web.WebviewUrl + "/login", dic)));
    }

    private void CallBack()
    {
        Single.WebView.OnReceiveCallback = (str) =>
        {
            if (str.Url == "arzmeta://DeleteDone")
            {
                Single.WebView.CloseWebview();

                Single.Scene.FadeOut(1f, () =>
                {
                    RealtimeUtils.Disconnect();

                    LocalPlayerData.ResetData();
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                   GamePot.deleteMember();
#endif
                   Single.Scene.LoadScene(SceneName.Scene_Base_Title);
                });
            }
        };
    }

    /// <summary>
    /// 로그아웃 
    /// </summary>
    private void OnClick_Logout()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("4014")))
            .ChainPopupAction(new PopupAction(() =>
            {
                Single.Scene.FadeOut(1f, () =>
                {
                    RealtimeUtils.Disconnect();

                    LocalPlayerData.ResetData();
                    Single.Scene.LoadScene(SceneName.Scene_Base_Title);
                });
            }));
    }

    /// <summary>
    /// GAMEPOT 공지사항 보여주기
    /// </summary>
    private void OnClick_GamePotNotice()
    {
        GamePotManager.ShowNotice(true);
    }
    #endregion

    #region 계정 연동
    /// <summary>
    /// 연동 정보 확인하고 패널 갱신
    /// </summary>
    public void ConnectPanelUpdate()
    {
        //오피스 등급
        string gradeType = Single.MasterData.dataOfficeGradeType.GetData(LocalPlayerData.OfficeGradeType).name;
        Util.SetMasterLocalizing(txtmp_AccountType, new MasterLocalData(gradeType));

        // common_sign_out - 특정 계정 연동하기
        Util.SetMasterLocalizing(txtmp_arzMETA_Off, new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.ARZMETA));
        Util.SetMasterLocalizing(txtmp_Naver_Off, new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.NAVER));
        Util.SetMasterLocalizing(txtmp_Google_Off, new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.GOOGLE));
        Util.SetMasterLocalizing(txtmp_Apple_Off, new MasterLocalData("common_sign_out", LOGIN_PROVIDER_TYPE.APPLE));
        SetActiveChangePassword(false);

        if (LocalPlayerData.SocialLoginInfo != null)
        {
            int count = LocalPlayerData.SocialLoginInfo.Count;

            for (int i = 0; i < count; i++)
            {
                int type = LocalPlayerData.SocialLoginInfo[i].providerType;

                // common_sign_in - 특정 계정 연동됨
                switch ((LOGIN_PROVIDER_TYPE)type)
                {
                    case LOGIN_PROVIDER_TYPE.ARZMETA:
                        {
                            togplus_Social_arzMETA.SetToggleIsOnWithoutNotify(true);
                            Util.SetMasterLocalizing(txtmp_arzMETA_On, new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.ARZMETA));
                            SetActiveChangePassword(true);
                            break;
                        }
                    case LOGIN_PROVIDER_TYPE.NAVER:
                        {
                            togplus_Social_naver.SetToggleIsOnWithoutNotify(true);
                            Util.SetMasterLocalizing(txtmp_Naver_On, new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.NAVER));
                            break;
                        }
                    case LOGIN_PROVIDER_TYPE.GOOGLE:
                        {
                            togplus_Social_google.SetToggleIsOnWithoutNotify(true);
                            Util.SetMasterLocalizing(txtmp_Google_On, new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.GOOGLE));
                            break;
                        }
                    case LOGIN_PROVIDER_TYPE.APPLE:
                        {
                            togplus_Social_apple.SetToggleIsOnWithoutNotify(true);
                            Util.SetMasterLocalizing(txtmp_Apple_On, new MasterLocalData("common_sign_in", LOGIN_PROVIDER_TYPE.APPLE));
                            break;
                        }
                }
            }
        }
    }

    /// <summary>
    /// 연동되어 있는 계정 클릭 시, 연동 해제 팝업
    /// </summary>
    bool IsLinkAccount(LOGIN_PROVIDER_TYPE _TYPE)
    {
        int count = LocalPlayerData.SocialLoginInfo.Count;

        // 로그인한 계정은 연동 해제 불가능
        if (LocalPlayerData.ProviderType == (int)_TYPE)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("common_error_sign_disconnect")))
                .ChainPopupAction(new PopupAction(() =>
                {
                    ConnectPanelUpdate();
                }));
            return true;
        }

        for (int i = 0; i < count; i++)
        {
            if (LocalPlayerData.SocialLoginInfo[i].providerType == (int)_TYPE)
            {
                // 연동을 해제하시겠습니까?
                PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("common_confirm_sign_disconnect")))
                    .ChainPopupAction(new PopupAction(() =>
                    {
                        // 연동해제 OK 버튼
                        Single.Web.account.ReleaseLinkedAccount((int)_TYPE, (res) =>
                        {
                            if (res.socialLoginInfo != null && res.socialLoginInfo.Length != 0)
                                LocalPlayerData.SocialLoginInfo = res.socialLoginInfo.ToList();
                        });
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                        if (LocalPlayerData.Method.IsSocialLogined())
                            GamePot.logout();
#endif
                    },
                    () =>
                    {
                        // 연동해제 Cancel 버튼 - 액션없이 Togplus On
                        switch ((int)_TYPE)
                        {
                            case 1: togplus_Social_arzMETA.SetToggleIsOnWithoutNotify(true); break;
                            case 2: togplus_Social_naver.SetToggleIsOnWithoutNotify(true); break;
                            case 3: togplus_Social_google.SetToggleIsOnWithoutNotify(true); break;
                            case 4: togplus_Social_apple.SetToggleIsOnWithoutNotify(true); break;
                        }
                    }));
                return true;
            }
        }
        return false;
    }

    private void OnClick_LoginSDKForLink(LOGIN_PROVIDER_TYPE providerType)
    {
        if (IsLinkAccount(providerType)) return;
        
        linkProviderType = (int)providerType;

        switch (providerType)
        {
            case LOGIN_PROVIDER_TYPE.ARZMETA:
                PushPanel<Panel_LinkAccount>();
                break;
            case LOGIN_PROVIDER_TYPE.NAVER:
            case LOGIN_PROVIDER_TYPE.GOOGLE:
            case LOGIN_PROVIDER_TYPE.APPLE:
#if UNITY_STANDALONE || UNITY_EDITOR
                string combineUrl = SetLoginUrl(providerType);
                WebViewCallback(combineUrl);
#elif (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
                if (LocalPlayerData.Method.IsSocialLogined())
                {
                    GamePot.logout();
                    firstLogin = true;
                }

                NCommon.LoginType gamepotLoginType = SetGamePotLoginType(providerType);
                GamePot.login(gamepotLoginType);
#endif
                break;
        }
    }

    /// <summary>
    /// 소셜 계정 종류에 따른 로그인 url 설정
    /// </summary>
    /// <param name="providerType"></param>
    /// <returns></returns>
    private string SetLoginUrl(LOGIN_PROVIDER_TYPE providerType)
    {
        string url = string.Empty;
        switch (providerType)
        {
            case LOGIN_PROVIDER_TYPE.NAVER:
                url = Single.Web.PCAccountServerUrl + "/api/account/naver";
                break;

            case LOGIN_PROVIDER_TYPE.GOOGLE:
                url = Single.Web.PCAccountServerUrl + "/api/account/google";
                break;

            case LOGIN_PROVIDER_TYPE.APPLE:
                url = Single.Web.PCAccountServerUrl + "/api/account/apple";
                break;
        }
        return url;
    }

    private NCommon.LoginType SetGamePotLoginType(LOGIN_PROVIDER_TYPE providerType)
    {
        switch (providerType)
        {
            case LOGIN_PROVIDER_TYPE.NAVER:
                return NCommon.LoginType.NAVER;
            case LOGIN_PROVIDER_TYPE.GOOGLE:
                return NCommon.LoginType.GOOGLE;
            case LOGIN_PROVIDER_TYPE.APPLE:
                return NCommon.LoginType.APPLE;
            default:
                return NCommon.LoginType.NONE;
        }
    }

    /// <summary>
    /// PC 버전 계정 연동 웹뷰 콜백 처리
    /// </summary>
    /// <param name="url"></param>
#if UNITY_STANDALONE || UNITY_EDITOR
    private async void WebViewCallback(string url)
    {
        Single.WebView.OnReceiveCallback = async (str) => 
        { 
            // 연동 여부를 떠나 연동을 위한 로그인 성공 시, socialLoginInfo를 포함한 콜백을 받음
            if (str.Url.Contains("socialLoginInfo"))
            {
                // 이미 다른 계정에 연동되어 있는 경우,
                // 콜백 형태(value 생략) : action?error!284&memberInfo!memberId:,nickname:,stateMessage:,socialLoginInfo:[providerType:,accountToken:],memberCode:,avatarInfos:
                if (str.Url.Contains("error"))
                {
                    MemberInfo convertAccountInfo = ParseQueryMemberInfo(str.Url);
                    Single.WebView.CloseWebview();

                    await UniTask.NextFrame();

                    // 계정 전환 팝업을 띄우기 위해 
                    SceneLogic.instance.isUILock = false;

                    // 연동이 되지 않았기 때문에 토글 Off
                    ToggleOff(linkProviderType);

                    SceneLogic.instance.PushPopup<Popup_ConvertAccount>().SetData(convertAccountInfo, linkProviderType);
                }

                // 연동 성공
                // 콜백 형태 : action?socialLoginInfo![providerType:, accountToken:]
                else
                {
                    MemberAccountInfo[] socialLoginInfo = ParseQueryMemberAccountInfo(str.Url);
                    LocalPlayerData.SocialLoginInfo = socialLoginInfo.ToList();

                    Single.WebView.CloseWebview();

                    ConnectPanelUpdate();
                }
            }
            else return;
        };
        
        // 이전 로그인 기록 초기화를 위한 쿠키 삭제
        await StandaloneWebView.DeleteAllCookies();
        // Standalone 버전의 경우, Chromium 프로세스 종료를 해야 웹뷰 관련 설정을 할 수 있음(브라우저 언어 설정, useragent 설정)
        await StandaloneWebView.TerminateBrowserProcess();

        // TODO : 현재 언어 상태에 따라 브라우저 언어를 세팅하는 게 좋을 것 같습니다.
        // 브라우저 언어를 한국어로 세팅
        StandaloneWebView.SetCommandLineArguments("--lang=ko-KR");

        // 서버에서 일반 로그인이 아닌 계정 연동을 위한 로그인임을 알려주기 위한 UserAgent 세팅
        string userAgent = $"LoginData(isLinked=true, memberId={LocalPlayerData.MemberID})";
        Web.SetUserAgent(userAgent);

        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, url)));
    }
#endif

    /// <summary>
    /// 전환 가능한 계정 정보 파싱
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static MemberInfo ParseQueryMemberInfo(string url)
    {
        string memberInfo = null;

        foreach (Match match in Regex.Matches(url, @"memberInfo!(?<value>[^&]+)"))
        {
            memberInfo = Uri.UnescapeDataString(match.Groups["value"].Value);
            break;
        }

        // Dictionary로 선언된 AvatarInfos 변수 deserializ를 위한 json converter 설정
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new CustomJsonConverter_AvatarInfos());

        MemberInfo convertInfo = JsonConvert.DeserializeObject<MemberInfo>(memberInfo);

        return convertInfo;
    }

    /// <summary>
    /// 연동 성공한 계정 정보 파싱
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static MemberAccountInfo[] ParseQueryMemberAccountInfo(string url)
    {
        string result = null;
        List<MemberAccountInfo> memberAccountInfo = new List<MemberAccountInfo>();

        // match.Groups[0] : 전체 매치된 문자열([..])
        foreach (Match match in Regex.Matches(url, @"\[(.*?)\]"))
        {
            result = match.Groups[0].Value;
            break;
        }

        if (result != null)
        {
            string decodedJson = Uri.UnescapeDataString(result);

            JArray jsonArray = JArray.Parse(decodedJson);

            foreach (JObject jsonObj in jsonArray.Cast<JObject>())
            {
                int providerType = jsonObj.Value<int>("providerType");
                string accountToken = jsonObj.Value<string>("accountToken");

                MemberAccountInfo accountInfo = new MemberAccountInfo
                {
                    providerType = providerType,
                    accountToken = accountToken
                };

                memberAccountInfo.Add(accountInfo);
            }
        }

        return memberAccountInfo.ToArray();
    }

    /// <summary>
    /// Dictionary<string, int> 형식에 대한 컨버터 
    /// </summary>
    public class CustomJsonConverter_AvatarInfos : JsonConverter<Dictionary<string, int>>
    {
        /// <summary>
        /// 개체를 JSON 문자열로 직렬화
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, Dictionary<string, int> value, JsonSerializer serializer)
        {
            JObject obj = new JObject();
            foreach (KeyValuePair<string, int> pair in value)
            {
                obj.Add(pair.Key, pair.Value);
            }
            obj.WriteTo(writer);
        }

        /// <summary>
        /// JSON 문자열을 개체로 역직렬화
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override Dictionary<string, int> ReadJson(JsonReader reader, Type objectType, Dictionary<string, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject obj = JObject.Load(reader);
            var dictionary = new Dictionary<string, int>();

            foreach (var property in obj.Properties())
            {
                string key = property.Name;
                if (property.Value.Type == JTokenType.Integer)
                {
                    int val = property.Value.ToObject<int>();
                    dictionary.Add(key, val);
                }
            }

            return dictionary;
        }
    }

    /// <summary>
    /// 액션없이 Toggle Off
    /// </summary>
    public void ToggleOff(int linkProviderType)
    {
        switch (linkProviderType)
        {
            case 1: togplus_Social_arzMETA.SetToggleIsOnWithoutNotify(false); break;
            case 2: togplus_Social_naver.SetToggleIsOnWithoutNotify(false); break;
            case 3: togplus_Social_google.SetToggleIsOnWithoutNotify(false); break;
            case 4: togplus_Social_apple.SetToggleIsOnWithoutNotify(false); break;
        }
    }

    #region GAMEPOT 콜백 함수
    public void onLoginSuccess(NUserInfo userInfo)
    {
        Single.Web.account.LinkedAccount(userInfo.userid, pwd, linkProviderType, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_ALREADY_LINKED_OTHER_ACCOUNT)
            {
                // 이미 다른 계정에 연동되어 있습니다.
                if (res.memberInfo != null)
                {
                    SceneLogic.instance.PushPopup<Popup_ConvertAccount>().SetData(res.memberInfo, linkProviderType);
                }
                else
                    Debug.Log("MemberInfo is Null");
            }

            else
            {
                LocalPlayerData.SocialLoginInfo = res.socialLoginInfo.ToList();

                // 로컬 데이터 추가
                MemberAccountInfo info = new MemberAccountInfo();
                info.providerType = linkProviderType;
                info.accountToken = userInfo.userid;

                LocalPlayerData.SocialLoginInfo.Add(info);
            }

            ConnectPanelUpdate();
        },
        (res) =>
        {
            // 이미 연동된 계정 입니다.
            if (res.error == (int)WEBERROR.NET_E_ALREADY_PROVIDER_TYPELINKED_ACCOUNT)
            {
                // 이미 연동된 계정 - 액션없이 Togplus Off
                ToggleOff(linkProviderType);
            }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            if (LocalPlayerData.Method.IsSocialLogined())
                GamePot.logout();   
#endif
        });
    }

    public void onLoginCancel()
    {
        // 로그인 취소 - 액션없이 Togplus Off
        ToggleOff(linkProviderType);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (LocalPlayerData.Method.IsSocialLogined())
            GamePot.logout();
#endif
    }

    public void onLogoutFailure(NError error)
    {
        Debug.Log("onLogoutFailure() : " + error);
    }

    public void onLogoutSuccess()
    {
        Debug.Log("로그아웃 완료");
    }

    public void onDeleteMemberSuccess()
    {
        Debug.Log("탈퇴 완료");
    }

    public void onReceiveScheme(string scheme)
    {
        Debug.Log("***** onReceiveScheme() : scheme = " + scheme);
    }
    #endregion

    #endregion
}
