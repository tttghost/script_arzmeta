using System;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 웹뷰 공유 UI 컴포넌트입니다.
///
/// Panel_WebviewSharing은 패널이 닫힌 상태에서도 웹뷰 텍스쳐가 갱신되어야하기 때문에 항상 켜져있으며 알파값과 인터렉터블 여부 조절로 패널을 여닫습니다.
/// 때문에 Open과 Close라는 전용 함수가 따로 존재합니다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public sealed class Panel_WebviewSharing : PanelBase
{
    public UnityEvent onClose = new UnityEvent();
    private WebviewController webviewController;
    private OfficeShareManager SHARE_MNG { get { return OfficeShareManager.INST; } }

    #region override
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_Button("btn_Close_2", () => SceneLogic.instance.Back());

        webviewController = GetComponentInChildren<WebviewController>(true);
        webviewController.toolBar.onShareWeb.AddListener(OnClickShare);
        webviewController.toolBar.onYoutube.AddListener((url) => OnClickYoutube(url));
    }

    public override void Back(int cnt = 1)
    {
        base.Back(cnt);

        GetPanel<Panel_HUD>().Show(true);
        onClose?.Invoke();

        // 공유중이 아닌 경우 웹 플레이 중지
        if (!SHARE_MNG.IsSharing(eSHARE_TYPE.WEB))
            webviewController.StopWebVideo();
    }

    public override void CloseEndAct()
    {
        MyPlayer.instance.EnableController(true, 0.2f);
    }
    #endregion

    #region MonoBehaviour
    protected override void Start()
    {
        base.Start();

        SHARE_MNG.ChangeShareTypeEvent.AddListener(OnChangeShareType);
    }

    protected override void OnEnable()
    {
        GetPanel<Panel_HUD>().Show(false);
    }
    #endregion

    /// <summary>
    /// 화면 공유를 활성화/비활성화합니다.(Button OnClick 전용 함수)
    /// </summary>
    public void OnClickShare()
    {
        if (SHARE_MNG.IsSharing(eSHARE_TYPE.WEB))
            SHARE_MNG.DisableSharing();
        else
            SHARE_MNG.EnableSharing(eSHARE_TYPE.WEB);
    }

    public void OnClickYoutube(string url)
    {
        // 공유중이 아닌 경우 웹 플레이 중지
        if (!SHARE_MNG.IsSharing(eSHARE_TYPE.WEB))
            webviewController.StopWebVideo();

        SceneLogic.instance.FinishPopPanelEvent.AddListener(() => {
            Panel_VideoSharing panel = SceneLogic.instance.PushPanel<Panel_VideoSharing>(false);
            panel.SetOpenEndCallback(() => panel.SetUrlFieldText(url));
        });
        Back();
    }

    void OnChangeShareType(eSHARE_TYPE shareType)
    {
        webviewController.SetShareIndicator(shareType == eSHARE_TYPE.WEB);
        webviewController.toolBar.shareWeb.image.color = shareType == eSHARE_TYPE.WEB ? Color.red : Color.white;

        if (shareType == eSHARE_TYPE.VIDEO)
            webviewController.StopWebVideo();
    }
}
