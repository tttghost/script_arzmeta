using System;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using RenderHeads.Media.AVProVideo;

public class Panel_VideoSharing : PanelBase
{
    private WebViewVideoShare webviewVideoShare;
    private MediaPlayer mediaPlayer;
    public UnityEvent onClose = new UnityEvent();

    #region UI Properties
    private GameObject go_Buffering;
    private Button button_Sync;
    private Button button_GoBack;
    private Button button_GoForward;
    private Button button_Home;
    private TMP_InputField urlField;
    #endregion

    #region urlStack
    private Stack<string> urlGoBackStack = new Stack<string>();
    private Stack<string> urlGoForwardStack = new Stack<string>();
    private string currentUrl;
    #endregion

    OfficeShareManager SHARE_MNG { get { return OfficeShareManager.INST; } }

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_Buffering = GetChildGObject(nameof(go_Buffering));
        if (go_Buffering != null)
            go_Buffering.SetActive(false);

        GetUI_Button("btn_Close_2", () => Back());
        button_Sync = GetUI_Button("btn_Sync", () => DelayAction(() => OnClickShare(), button_Sync, 0.7f));
        button_GoBack = GetUI_Button("Button_GoBack", () => DelayAction(GoBack, button_GoBack));
        button_GoForward = GetUI_Button("Button_GoForward", () => DelayAction(GoForward, button_GoForward));
        button_Home = GetUI_Button("Button_Home", () => DelayAction(GoHome, button_Home));
        urlField = GetUI_TMPInputField("input_URLField", null, EnterUrl);

        webviewVideoShare = FindObjectOfType<WebViewVideoShare>(true);
        mediaPlayer = GetComponentInChildren<MediaPlayer>(true);
        mediaPlayer.Events.AddListener(OnMediaPlayerEvent);

        ReadyToShare();
    }

    public override void Back(int cnt = 1)
    {
        base.Back(cnt);
        
        if (!SHARE_MNG.IsSharing(eSHARE_TYPE.VIDEO))
        {
            mediaPlayer.Control.Stop();
            mediaPlayer.Control.CloseMedia();
        }

        GetPanel<Panel_HUD>().Show(true);
        onClose?.Invoke();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        GetPanel<Panel_HUD>().Show(false);
        button_Sync.image.color = SHARE_MNG.IsSharing(eSHARE_TYPE.VIDEO)? Color.red : Color.white;
    }

    void OnDestroy()
    {
        mediaPlayer.Events.RemoveListener(OnMediaPlayerEvent);
    }

    public override void OpenStartAct()
    {
        base.OpenStartAct();
        
        ClosePanel<Panel_HUD>();

        // BKK TODO: 로컬라이징 추가되면 수정할 것
        // GetUI_Txtmp("txtmp_Title", new LocalData(Cons.Local_Arzmeta, "1062"));
    }

    public override void CloseStartAct()
    {
        base.CloseStartAct();
        OpenPanel<Panel_HUD>();
        onClose?.Invoke();
    }
    
    public void OnSetActiveBuffer(bool isActive)
    {
        if (go_Buffering != null)
        {
            go_Buffering.SetActive(isActive);
        }
    }

    private async void DelayAction(Action action, Button button, float delay = 0.2f)
    {
        Single.Sound.PlayEffect(Cons.click);

        action?.Invoke();

        button.enabled = false;
        await UniTask.Delay((int)(delay * 1000));
        button.enabled = true;
    }

    private void GoBack()
    {
        if (urlGoBackStack.Count == 0) return;

        var peek = urlGoBackStack.Peek();

        if (peek == null) return;

        var url = urlGoBackStack.Pop();
        urlGoForwardStack.Push(currentUrl);

        if (ValidateURL(url))
            webviewVideoShare.OpenVideo(url);

        urlField.SetTextWithoutNotify(url);
    }

    private void GoForward()
    {
        if (urlGoForwardStack.Count == 0) return;

        var peek = urlGoForwardStack.Peek();

        if (peek == null) return;

        var url = urlGoForwardStack.Pop();
        urlGoBackStack.Push(urlField.text);

        webviewVideoShare.OpenVideo(url);

        urlField.SetTextWithoutNotify(url);
    }

    private void GoHome()
    {
        webviewVideoShare.CloseVideo();
        urlField.SetTextWithoutNotify(string.Empty);
    }

    private void EnterUrl(string url)
    {
        if (ValidateURL(url) == false)
            return;

        webviewVideoShare.OpenVideo(url);

        urlGoForwardStack.Clear();

        if (currentUrl != url)
        {
            urlGoBackStack.Push(currentUrl);
            currentUrl = url;
        }
    }

    public bool ValidateURL(string url)
    {
        bool isValidate = !string.IsNullOrEmpty(url);

        if (url.IsYoutubeVideo() == false)
        {
            isValidate = false;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            urlField.SetTextWithoutNotify(string.Empty);
            isValidate = false;
        }

        if (isValidate == false)
            webviewVideoShare.ShowWarningPopup();

        return isValidate;
    }

    private void OnMediaPlayerEvent(MediaPlayer mediaPlayer, MediaPlayerEvent.EventType eventType, ErrorCode errorCode)
    {
        bool isSharing = SHARE_MNG.IsSharing(eSHARE_TYPE.VIDEO);

        switch (eventType)
        {
            // 미디어 플레이어에서 Buffering 표시가 안나오기 때문에
            // 자체 버퍼링 UI 표시가 필요
            case MediaPlayerEvent.EventType.FirstFrameReady:
                {
                    if (isSharing)
                    {
                        OnSetActiveBuffer(false);
                    }
                }
                break;
            case MediaPlayerEvent.EventType.Error:
                {
                    webviewVideoShare.ShowWarningPopup();

                    if (isSharing)
                    {
                        OnSetActiveBuffer(false);
                    }

                    SHARE_MNG.DisableSharing();

                    if (button_Sync)
                        button_Sync.image.color = Color.white;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// // 호스트(나)가 비디오 공유 버튼을 눌렀을 때 호출되는 함수
    /// </summary>
    public async void OnClickShare()
    {
        // 올바른 유튜브 링크인지 체크
        if (ValidateURL(urlField.text) == false)
            return;

        OfficeShareManager shareMng = SHARE_MNG;

        // 권한 체크
        if (!shareMng.IsHaveScreenPermission())
        {
            ShowPermissionPopup();
            return;
        }

        bool isSharing = shareMng.IsSharing(eSHARE_TYPE.VIDEO);
        if (isSharing)
        {
            shareMng.DisableSharing();

            button_Sync.image.color = Color.white;

            SetStateUI();
        }
        else
        {
            if (!mediaPlayer.Control.IsPlaying())
            {
                EnterUrl(urlField.text);

                OnSetActiveBuffer(true);

                Color startColor = Color.white;
                Color endColor = Color.red;
                float duration = 0.5f;
                float elapsedTime = 0.0f;

                while (mediaPlayer.Control.CanPlay() == false)
                {
                    float t = elapsedTime / duration;

                    Color lerpedColor = Color.Lerp(startColor, endColor, t);
                    button_Sync.image.color = lerpedColor;

                    elapsedTime += Time.deltaTime;

                    if (elapsedTime > duration)
                    {
                        elapsedTime = 0.0f;
                        startColor = Color.red;
                        endColor = Color.white;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update);

                    button_Sync.image.color = Color.red;
                }

                OnSetActiveBuffer(false);
            }
            else
                button_Sync.image.color = Color.red;

            SetStateUI();

            SHARE_MNG.EnableSharing(eSHARE_TYPE.VIDEO);
        }
    }

    void SetStateUI()
    {
        // 비디오 공유가 되면 비디오채팅을 꺼주고 "화면 공유중" UI 표시
        AgoraUser agoraUser = Single.Agora.GetLocalUser();
        if (agoraUser != null)
        {
            if (SHARE_MNG.IsSharing(eSHARE_TYPE.VIDEO))
                agoraUser.SetAfterScreenShare();
            else
                agoraUser.SetBeforeScreenShare();
        }
    }

    /// <summary>
    /// 권한이 없을 때 UI 조작 제어 팝업 활성화
    /// </summary>
    public void ShowPermissionPopup()
    {
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("1190")));
    }

    public void SetUrlFieldText(string _url)
    {
        if (urlField)
            urlField.text = _url;

        EnterUrl(_url);
    }

    public MediaPlayer GetMediaPlayer()
    {
        return mediaPlayer;
    }

    // 공유하기를 위한 사전작업.
    // 미디어 플레이어가 코루틴으로 돌아가기에 항상 Active 상태를 유지해야 함으로 Paanel_VideoSharing 생성 시 미디어 플레이어 오브젝트를 UI_Canvas 아래로 이동 시킴
    // 씬 전환 시 패널과 UI_Canvas는 삭제 되기에 미디어 플레이어 오브젝트는 별도로 원복조치는 안함
    void ReadyToShare()
    {
        if (Util.NullCheck(mediaPlayer, GetType(), nameof(mediaPlayer)))
            return;

        // 미디어 플레이어 오브젝트 이동
        mediaPlayer.transform.SetParent(SceneLogic.instance.canvas.transform);
    }
}