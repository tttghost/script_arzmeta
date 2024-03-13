using FrameWork.UI;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_KTMFVideoPlayer : PopupBase
{
    private MediaPlayer go_MediaPlayer;
    private GameObject go_Buffering;

    private float oriBGMVolume;

    private string path;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_Button("btn_Exit", OnCkick_Back);

        BackAction_Custom = OnCkick_Back;

        go_Buffering = GetChildGObject(nameof(go_Buffering));
        go_MediaPlayer = GetChildGObject(nameof(go_MediaPlayer)).GetComponent<MediaPlayer>();
        if (go_MediaPlayer != null)
        {
            go_MediaPlayer.Events.AddListener(ExecutionError);
        }
    }

    private void ExecutionError(MediaPlayer mp, MediaPlayerEvent.EventType eventType, ErrorCode code)
    {
        switch (eventType)
        {
            case MediaPlayerEvent.EventType.FirstFrameReady: AetActiveBuffer(false); break;
            case MediaPlayerEvent.EventType.Error: Invoke(nameof(PlayMedia), 1f); break;
            default: Debug.Log("ExecutionError :: " + eventType.ToString()); break;
        }
    }

    public void SetData(string path)
    {
        this.path = path;
        oriBGMVolume = AppGlobalSettings.Instance.volumeBGM;

        PlayMedia();
    }

    private void SetBGMVolume(bool isOn)
    {
        var volume = isOn ? oriBGMVolume : 0f;
        CommonUtils.SetBGMVolume(this, volume);
    }

    private async void PlayMedia()
    {
        if (string.IsNullOrEmpty(path)) return;

        if (go_MediaPlayer != null)
        {
            AetActiveBuffer(true);
            go_MediaPlayer.CloseMedia();

            var mediaPath = await Single.Screen.GetMediaPath(path, eScreenContentType.youtubeNormal);
            go_MediaPlayer.OpenMedia(new MediaPath(mediaPath, MediaPathType.AbsolutePathOrURL), true);
        }
    }

    private void AetActiveBuffer(bool isActive)
    {
        if (go_Buffering != null)
        {
            go_Buffering.SetActive(isActive);
        }
    }

    private void OnCkick_Back()
    {
        SceneLogic.instance.PopPopup();
    }

    protected override void OnEnable()
    {
        SetBGMVolume(false);
    }

    protected override void OnDisable()
    {
        SetBGMVolume(true);
    }
}
