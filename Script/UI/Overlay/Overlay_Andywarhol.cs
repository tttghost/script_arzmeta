using Cysharp.Threading.Tasks;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class Overlay_Andywarhol : MonoBehaviour
{
    private CanvasWebViewPrefab webViewPrefab;
    private TMP_InputField input_Url;
    private Button btn_GoBack;
    private Button btn_GoForward;
    private Button btn_Reload;
    private Button btn_Exit;
    public string InitialUrl;
    private async void Start()
    {
        //초기화
        webViewPrefab = GetComponentInChildren<CanvasWebViewPrefab>();
        webViewPrefab.InitialUrl = InitialUrl;
        await webViewPrefab.WaitUntilInitialized();
        input_Url = Util.Search<TMP_InputField>(gameObject, nameof(input_Url));

        //뒤로가기
        btn_GoBack = Util.Search<Button>(gameObject, nameof(btn_GoBack));
        btn_GoBack.onClick.AddListener(async ()=> 
        {
            var canGoBack = await webViewPrefab.WebView.CanGoBack();

            if (canGoBack)
            {
                webViewPrefab.WebView.GoBack();
            }
        });

        //앞으로가기
        btn_GoForward = Util.Search<Button>(gameObject, nameof(btn_GoForward));
        btn_GoForward.onClick.AddListener(async () =>
        {
            var canGoForward = await webViewPrefab.WebView.CanGoForward();

            if (canGoForward)
            {
                webViewPrefab.WebView.GoForward();
            }
        });

        //다시불러오기
        btn_Reload = Util.Search<Button>(gameObject, nameof(btn_Reload));
        btn_Reload.onClick.AddListener(() =>
        {
            webViewPrefab.WebView.LoadUrl(input_Url.text);
        });
        input_Url.onEndEdit.AddListener((str) => btn_Reload.onClick.Invoke());

        //나가기
        btn_Exit = Util.Search<Button>(gameObject, nameof(btn_Exit));
        btn_Exit.onClick.AddListener(SceneLogic.instance.Back);

        NextPage();
    }

    async void NextPage()
    {
        await webViewPrefab.WebView.WaitForNextPageLoadToFinish();
        input_Url.text = webViewPrefab.WebView.Url;
        btn_GoBack.interactable = await webViewPrefab.WebView.CanGoBack();
        btn_GoForward.interactable = await webViewPrefab.WebView.CanGoForward();
        NextPage();
    }
}
