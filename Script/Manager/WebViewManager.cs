using System;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Vuplex.WebView;
using Vuplex.WebView.Internal;
using MEC;
using UnityEngine.UI;
using CryptoWebRequestSample;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum WEBVIEWTYPE { URL, HTML, }
public enum WEBVIEW_SIZE { FULL, WINDOW, }

public class WebViewManager : Singleton<WebViewManager>
{
    #region 변수
    // 웹뷰 로딩이 완료되었을 때 호출
    public UnityAction OnStartCallback = null;
    // ESC 키, 안드로이드 백키, 툴바 종료 버튼, 예기치 못한 종료 시 호출
    public UnityAction OnBackBtnCallback = null;
    // Url 변경 시마다 호출
    public UnityAction<UrlChangedEventArgs> OnReceiveCallback = null;

    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;
    private RectTransform rectTransform;

    private CanvasWebViewPrefab webView;

    private Canvas webViewCanvas;
    private Transform webViewRig;
    private Panel_WebViewToolbar webViewToolbar;

    private WebSetting setting;
    #endregion

    #region 초기화

    public TogglePlus togplus_Today { get; private set; }
    protected override void AWAKE()
    {
        base.AWAKE();

        webViewCanvas = CreateCanvas();
        webViewCanvas.enabled = false;

        webViewToolbar = CreateToolbar();

        webViewRig = webViewToolbar.transform.Find("go_WebViewRig");

        togplus_Today = Util.Search<TogglePlus>(gameObject, nameof(togplus_Today));
        togplus_Today.gameObject.SetActive(false);
    }

    /// <summary>
    /// 캔버스 오브젝트 생성
    /// </summary>
    /// <returns></returns>
    private Canvas CreateCanvas()
    {
        var go = new GameObject("WebviewCanvas");
        go.AddComponent<GraphicRaycaster>();
        go.transform.SetParent(transform);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        return canvas;
    }

    /// <summary>
    /// 툴바 패널 프리팹 생성
    /// </summary>
    /// <returns></returns>
    private Panel_WebViewToolbar CreateToolbar()
    {
        var Panel_WebViewToolbar = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_UI_Panel + typeof(Panel_WebViewToolbar).Name); // 패스 변경
        Panel_WebViewToolbar.transform.SetParent(webViewCanvas.transform);

        rectTransform = Panel_WebViewToolbar.GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.offsetMin = originalOffsetMin = Vector2.zero;
        rectTransform.offsetMax = originalOffsetMax = Vector2.zero;
        Panel_WebViewToolbar.transform.localScale = Vector3.one;

        var toolbarCom = Panel_WebViewToolbar.GetComponent<Panel_WebViewToolbar>();
        toolbarCom.enabled = false;

        return toolbarCom;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!webView) return;

            OnBackBtnCallback?.Invoke();
            ResetCallback();

            Destroy(webView.gameObject);
            webViewCanvas.enabled = false;
        }
    }

    #endregion

    #region 코어 메소드

    /// <summary>
    /// 웹뷰 열기
    /// </summary>
    public void OpenWebView(WebViewData data)
    {
        setting = data.webSetting;
        SetWebView(data.dataSetting);
    }

    private async void SetWebView(WebDataSetting urlSetting) => await Co_SetWebView(urlSetting);

    /// <summary>
    /// 코어 메소드
    /// </summary>
    private async Task Co_SetWebView(WebDataSetting urlSetting)
    {
        if (!CreateWebView()) return;
        webViewCanvas.enabled = true;
        await webView.WaitUntilInitialized();
        await WebViewAddCallback();

        webViewToolbar.SetActivePreNextBtn(setting.isPreNextBtnActive);
        SetWebviewSize(setting.webview_size);

        OpenWeb(urlSetting);
    }

    /// <summary>
    /// 웹뷰 없을 시 생성, 콜백 추가
    /// </summary>
    /// <returns></returns>
    private bool CreateWebView()
    {
        // 웹뷰 생성
        if (!webView)
        {
            webView = CanvasWebViewPrefab.Instantiate();
            webView.transform.SetParent(webViewRig);
            webView.Native2DModeEnabled = true;

            var rectTr = webView.GetComponent<RectTransform>();

            rectTr.anchoredPosition3D = Vector3.zero;
            rectTr.offsetMin = Vector2.zero;
            rectTr.offsetMax = Vector2.zero;
            webView.transform.localScale = Vector3.one;

            if (!webView) return false;
        }

        return true;
    }

    /// <summary>
    /// 웹뷰 열기
    /// </summary>
    /// <param name="url"></param>
    private async Task WebViewAddCallback()
    {
        // 팝업 생성
        if (setting.isStack)
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PushPopup<Popup_Empty>();
        }

        // 로드 중에 상태 변화시 호출
        webView.WebView.LoadProgressChanged += delegate (object sender, ProgressChangedEventArgs args)
        {
            WebViewLogger.Log($"Load progress changed: {args.Type}, {args.Progress}");
            switch (args.Type)
            {
                case ProgressChangeType.Started:
                    WebViewLogger.Log("로딩 시작");
                    break;
                case ProgressChangeType.Finished: // 유니웹뷰의 OnPageFinished와 같은 기능
                    WebViewLogger.Log("로딩 완료");
                    OnStartCallback?.Invoke();
                    OnStartCallback = null;
                    break;
                case ProgressChangeType.Failed: // 유니웹뷰의 OnPageErrorReceived와 같은 기능
                    WebViewLogger.Log($"로딩 실패: [{webView.WebView.Url}]");
                    break;
                case ProgressChangeType.Updated:
                    WebViewLogger.Log("웹뷰 로드 중 업데이트");
                    break;
            }
        };

        // 종료시 호출
        webView.WebView.CloseRequested += delegate (object sender, EventArgs args)
        {
            Debug.Log("웹뷰 종료" + args);
            OnBackBtnCallback?.Invoke();
            OnBackBtnCallback = null;
        };

        // 페이지 로드 실패시 호출.
        //webView.WebView.PageLoadFailed += delegate (object sender, EventArgs args)
        //{ 
        //WebViewLogger.LogError("Error!: " + args);
        //};

        // 메세지 받을때 호출. 유니웹뷰에 OnMessageReceived와 같은 기능
        webView.WebView.MessageEmitted += delegate (object sender, EventArgs<string> args)
        {
            WebViewLogger.Log("Json Received: " + args.Value);
        };

        webView.WebView.UrlChanged += (object sender, UrlChangedEventArgs args) =>
        {
            WebViewLogger.Log("Url Changed To: " + args.Url);
            OnReceiveCallback?.Invoke(args);
        };

        await UniTask.NextFrame();
    }

    private void OpenWeb(WebDataSetting uRLSetting)
    {
        switch (uRLSetting.type)
        {
            case WEBVIEWTYPE.URL: webView.WebView.LoadUrl(uRLSetting.str, uRLSetting.httpHeaders); break;
            case WEBVIEWTYPE.HTML: webView.WebView.LoadHtml(uRLSetting.str); break;
            default: break;
        }

        IsHide();
    }
    #endregion

    #region 웹뷰 관련
    /// <summary>
    /// 웹뷰 종료 및 닫기
    /// </summary>
    public void CloseWebview()
    {
        if (webView)
        {
            OnBackBtnCallback?.Invoke();
            ResetCallback();

            // 더미 UI 스택을 쌓았을 시
            if (setting.isStack)
            {
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PopPopup();
            }

            webViewCanvas.enabled = false;
            webView.Destroy();
        }

        AccountLink();
    }

    /// <summary>
    /// 이전 페이지
    /// </summary>
    public void Goback()
    {
        if (webView.WebView == null || !webView.WebView.IsInitialized) return;
        webView.WebView.GoBack();
    }

    /// <summary>
    /// 다음 페이지
    /// </summary>
    public void GoForward()
    {
        if (webView.WebView == null || !webView.WebView.IsInitialized) return;
        webView.WebView.GoForward();
    }
    #endregion

    #region 캐시 및 쿠키

    /// <summary>
    /// 웹에 있는 모든 캐시 삭제 (현재 사용 안함)
    /// </summary>
    public void CleanAllData()
    {
        //Web.ClearAllData();
    }

    /// <summary>
    /// 웹에 있는 모든 쿠키를 삭제 (현재 사용 안함)
    /// </summary>
    /// <param name="url"></param>
    /// <param name="cookieName"></param>
    public void DeleteCookie(string url, string cookieName = null)
    {
        //Web.CookieManager.DeleteCookies(url, cookieName);
    }

    /// <summary>
    /// 웹에 있는 모든 캐시와 쿠키 삭제  (현재 사용 안함)
    /// </summary>
    /// <param name="curUrl"></param>
    /// <param name="cookieName"></param>
    public void CleanDataAndCookie(string curUrl = null)
    {
        CleanAllData();
        if (!string.IsNullOrEmpty(curUrl))
            DeleteCookie(curUrl);
    }

    #endregion

    #region 기타 메소드
    /// <summary>
    /// 웹뷰 사이즈 조정
    /// </summary>
    private void SetWebviewSize(WEBVIEW_SIZE WEBVIEW_SIZE)
    {
        float amount = 0f;

        switch (WEBVIEW_SIZE)
        {
            case WEBVIEW_SIZE.FULL: amount = 0f; break;
            case WEBVIEW_SIZE.WINDOW: amount = -100f; break;
            default: break;
        }

        Util.AdjustOffsets(rectTransform, originalOffsetMin, originalOffsetMax, amount);
    }

    private void IsHide()
    {
        var b = !setting.isHide;
        webView.Visible = b;
        webViewCanvas.enabled = b;
    }

    /// <summary>
    /// 등록된 콜백 리셋
    /// </summary>
    private void ResetCallback()
    {
        OnStartCallback = null;
        OnReceiveCallback = null;
        OnBackBtnCallback = null;
    }

    /// <summary>
    /// 계정연동 관련 처리
    /// </summary>
    private void AccountLink()
    {
        // 연동 취소 시, 켜진 토글 끄는 예외 처리
        if (SceneLogic.instance.GetPrevPanel() != null)
        {
            if (SceneLogic.instance.GetPrevPanel().name == typeof(Panel_Phone).Name)
            {
                View_Account view_account = SceneLogic.instance.GetPanel<Panel_Setting>().GetView<View_Account>();
                view_account.ToggleOff(view_account.linkProviderType);
            }
        }
    }
    #endregion
}

#region WebView Data Class
public class WebViewData
{
    public WebDataSetting dataSetting;
    public WebSetting webSetting = new WebSetting();

    public WebViewData(WebDataSetting dataSetting, WebSetting webSetting = null)
    {
        this.dataSetting = dataSetting;
        this.webSetting = webSetting ?? new WebSetting();
    }
}

public class WebDataSetting
{
    public WEBVIEWTYPE type;
    public string str;
    public Dictionary<string, string> httpHeaders = null;

    public WebDataSetting(WEBVIEWTYPE type, string str, Dictionary<string, string> httpHeaders = null)
    {
        this.type = type;
        this.str = str;
        this.httpHeaders = httpHeaders;
    }
}

public class WebSetting
{
    public bool isStack = true;
    public bool isHide = false;
    public bool isPreNextBtnActive = true;
    public WEBVIEW_SIZE webview_size = WEBVIEW_SIZE.FULL;

    public WebSetting(bool isStack = true, bool isHide = false, bool isPreNextBtnActive = true, WEBVIEW_SIZE webview_size = WEBVIEW_SIZE.FULL)
    {
        this.isStack = isStack;
        this.isHide = isHide;
        this.isPreNextBtnActive = isPreNextBtnActive;
        this.webview_size = webview_size;
    }
}
#endregion
