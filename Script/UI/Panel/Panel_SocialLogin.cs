using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System;
using GamePotUnity;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Vuplex.WebView;
using System.Text.RegularExpressions;
using System.Linq;

public class Panel_SocialLogin : PanelBase
{
    #region 변수
    private Animator anim;
    private Dictionary<string, string> socialLoginInfo;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Social_arzMETA", () => PushPanel<Panel_ArzLogin>(false));

#if UNITY_STANDALONE || UNITY_EDITOR
        GetUI_Button("btn_Social_naver", () => OnClick_PCSocialLogin(LOGIN_PROVIDER_TYPE.NAVER));
        GetUI_Button("btn_Social_google", () => OnClick_PCSocialLogin(LOGIN_PROVIDER_TYPE.GOOGLE));
        GetUI_Button("btn_Social_apple", () => OnClick_PCSocialLogin(LOGIN_PROVIDER_TYPE.APPLE));
#elif UNITY_ANDROID || UNITY_IOS
        // 로그인 GamePot SDK
        GetUI_Button("btn_Social_naver", () => OnClick_LoginSDK(NCommon.LoginType.NAVER));
        GetUI_Button("btn_Social_google", () => OnClick_LoginSDK(NCommon.LoginType.GOOGLE));
        GetUI_Button("btn_Social_apple", () => OnClick_LoginSDK(NCommon.LoginType.APPLE));
#endif

        GetUI_Button("btn_Back", () => SceneLogic.instance.Back());
        #endregion

        #region TMP_Text

        GetUI_TxtmpMasterLocalizing("txtmp_SocialTitle", new MasterLocalData("3044"));
        #endregion

        #region etc
        anim = GetComponent<Animator>();
        BackAction_Custom = () => Util.RunCoroutine(Co_FadeAction("FadeOut", () =>
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PopPanel();
        }));
        #endregion
    }

    public IEnumerator<float> Co_FadeAction(string animationName, Action action = null)
    {
        anim.Play(animationName);
        yield return Timing.WaitForSeconds(0.5f);
        action?.Invoke();
    }

    #region 초기화
    protected override void Start()
    {
        base.Start();

        // 게임팟 로그인 관련 콜백 등록
        GamePotManager.cbLoginSuccess = onLoginSuccess;
        GamePotManager.cbLoginCancel = onLoginCancel;
        GamePotManager.cbLogoutFailure = onLoginFailure;
    }

    protected override void OnDisable()
    {
        if (anim != null)
            anim.Rebind();
    }
    #endregion

    #region PC 소셜 로그인
    private async void OnClick_PCSocialLogin(LOGIN_PROVIDER_TYPE providerType)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        string combineUrl = SetLoginUrl(providerType);

        // 로그인 성공 후, 서버에서 보내주는 로그인 정보 파싱
        Single.WebView.OnReceiveCallback = (str) =>
        {
            // 콜백 형태(value 생략) : action?memberId!&accountToken!&provider!
            if (str.Url.Contains("accountToken"))
            {
                socialLoginInfo = ParseQueryString(str.Url);
                foreach ((string key, string value) in socialLoginInfo.Select(x => (x.Key, x.Value)))
                {
                    if (socialLoginInfo.ContainsKey("memberId"))
                        LocalPlayerData.MemberID = socialLoginInfo["memberId"];

                    if (socialLoginInfo.ContainsKey("accountToken"))
                        LocalPlayerData.Method.AccountToken = socialLoginInfo["accountToken"];

                    if (socialLoginInfo.ContainsKey("provider"))
                        LocalPlayerData.ProviderType = int.Parse(socialLoginInfo["provider"]);

                    Debug.Log($"{key}={value}");
                }

                // 탈퇴 여부 확인 및 로그인
                LoginWithCheckWithdrawal(null);
            }
        };

        // 이전 로그인 기록 초기화를 위한 쿠키 삭제
        await StandaloneWebView.DeleteAllCookies();
        // Standalone 버전의 경우, Chromium 프로세스 종료를 해야 웹뷰 관련 설정을 할 수 있음(브라우저 언어 설정, useragent 설정)
        await StandaloneWebView.TerminateBrowserProcess();

        // TODO : 현재 언어 상태에 따라 브라우저 언어를 세팅하는 걸로 변경하면 좋을 것 같습니다.
        // 브라우저 언어를 한국어로 세팅
        StandaloneWebView.SetCommandLineArguments("--lang=ko-KR");

        string userAgent = $"{Application.productName}/{Application.version} ({SystemInfo.operatingSystem})";
        Web.SetUserAgent(userAgent);

        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, combineUrl)));
#endif
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

    /// <summary>
    /// 쿼리 파싱 - 정규식을 이용해 키와 값은 !로 구분, 키는 &로 구분
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static Dictionary<string, string> ParseQueryString(string url)
    {
        var result = new Dictionary<string, string>();

        foreach (Match match in Regex.Matches(url, @"(?<key>\w+)!(?<value>[^&]+)"))
        {
            result.Add(match.Groups["key"].Value, Uri.UnescapeDataString(match.Groups["value"].Value));
        }

        return result;
    }
    #endregion

    #region 모바일 소셜 로그인
    public void OnClick_LoginSDK(NCommon.LoginType gamepotLoginType)
    {
        LOGIN_PROVIDER_TYPE type = default;

        GamePot.login(gamepotLoginType);

        switch (gamepotLoginType)
        {
            case NCommon.LoginType.NAVER:
                type = LOGIN_PROVIDER_TYPE.NAVER;
                break;

            case NCommon.LoginType.GOOGLE:
                type = LOGIN_PROVIDER_TYPE.GOOGLE;
                break;

            case NCommon.LoginType.APPLE:
                type = LOGIN_PROVIDER_TYPE.APPLE;
                break;
        }

        LocalPlayerData.ProviderType = (int)type;
    }
    #endregion

    #region PC / 모바일 소셜 로그인 공통 함수
    /// <summary>
    /// 탈퇴 여부 확인 및 로그인
    /// </summary>
    private void LoginWithCheckWithdrawal(NUserInfo userInfo)
    {
        string accountToken = string.Empty;
        // 플랫폼에 따른 accountToken 설정(탈퇴 여부 확인을 위해서는 accountToken이 필요함)
#if UNITY_STANDALONE || UNITY_EDITOR
        accountToken = LocalPlayerData.Method.AccountToken;
#elif UNITY_ANDROID || UNITY_IOS
        accountToken = userInfo.userid;
#endif

        Single.Web.member.CheckWithdrawalProgress(LocalPlayerData.ProviderType, accountToken, null, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_IS_WITHDRAWAL_MEMBER)
            {
                // 탈퇴 진행 팝업을 띄우기 위해 
                Single.WebView.CloseWebview();
                SceneLogic.instance.isUILock = false;

                LocalPlayerData.Method.CheckProcessWithdrawal(res,
                    // Confirm Action(탈퇴 취소 후, 로그인)
                    () =>
                    {
                        // NUserInfo는 GamePot 로그인 정보이므로 모바일에서만 사용함 -> PC버전과 에디터는 null로
#if UNITY_STANDALONE || UNITY_EDITOR
                        SocialLogin(null);
#elif UNITY_ANDROID || UNITY_IOS
                        SocialLogin(userInfo);
#endif
                    },
                    // Cancel Action(탈퇴 처리 진행 유지 -> 로그아웃 후 Title 씬으로 이동)
                    () =>
                    {
                        Single.Scene.FadeOut(1f,
                            () =>
                            {
#if UNITY_STANDALONE || UNITY_EDITOR
                                LocalPlayerData.ResetData();
#elif UNITY_ANDROID || UNITY_IOS
                                GamePot.logout();
#endif
                                Single.Scene.LoadScene(SceneName.Scene_Base_Title);
                            });
                    });
                return;
            }
            // NUserInfo는 GamePot 로그인 정보이므로 모바일에서만 사용함 -> PC버전과 에디터는 null로
#if UNITY_STANDALONE || UNITY_EDITOR
            SocialLogin(null);
#elif UNITY_ANDROID || UNITY_IOS
            SocialLogin(userInfo);
#endif
        });
    }

    private void SocialLogin(NUserInfo userInfo)
    {
        // PC, 에디터 버전의 경우, 로그인 성공 시 웹뷰 닫고 자동 로그인
#if UNITY_STANDALONE || UNITY_EDITOR
        Single.WebView.CloseWebview();
        Single.Web.account.AutoLogin();
        LocalPlayerData.Method.IsLogined = true;
        // 모바일의 경우, accountToken 설정 및 아즈메타 memberId 부여
#elif UNITY_ANDROID || UNITY_IOS
        LocalPlayerData.Method.AccountToken = userInfo.userid;

        Single.Web.account.SocialAccountLogin((res) =>
        {
            LocalPlayerData.MemberID = res.memberId;
            Single.Web.account.AutoLogin();

            LocalPlayerData.Method.IsLogined = true;
        });
#endif
    }
    #endregion

    #region GAMEPOT 콜백 함수
    public void onLoginSuccess(NUserInfo userInfo)
    {
        LoginWithCheckWithdrawal(userInfo);
    }

    public void onLoginCancel()
    {
        DEBUG.LOG("onLoginCancel()", eColorManager.GAMEPOTSDK);
    }

    public void onLoginFailure(NError error)
    {
        DEBUG.LOG("onLoginFailure()", eColorManager.GAMEPOTSDK);
    }
#endregion
}
