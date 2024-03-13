using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_WebViewToolbar : MonoBehaviour
{
    [SerializeField] GameObject preNextBtn;

    private void Awake()
    {
        WebViewToolBar toolBar = GetComponentInChildren<WebViewToolBar>();
        if (toolBar != null)
        {
            toolBar.onGoBack.AddListener(Single.WebView.Goback);
            toolBar.onGoForward.AddListener(Single.WebView.GoForward);
            toolBar.onClose.AddListener(Single.WebView.CloseWebview);
        }
    }

    public void SetActivePreNextBtn(bool b)
    {
        preNextBtn.SetActive(b);
    }
}
