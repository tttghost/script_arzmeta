/************************************************************************************************************
 * 
 *          View_HUD_TopLeft class
 *              - 프로필 UI
 *              - 오피스 등급 / 오디오 / 비디오 채팅 버튼 UI
 *
 ************************************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using Protocol;

public class View_HUD_TopLeft : UIBase
{
    private Scene_OfficeRoom sceneofficeRoom;

    #region 프로파일 UI
    private TMP_Text txtmp_NickName = null;
    private GameObject oriAvatarTr = null;
    private AvatarPartsController selector = null;
    private Panel_CostumeInven panel_CostumeInven = null;
    private Animator anim_Avatar;
    private Toggle tog_TabVideoChat = null;
    private GameObject go_TabVideo_disable = null;
    #endregion

    #region 오피스 TopLeft UI(오피스 등급, 오디오, 비디오 버튼)
    private GameObject go_OfficeGrade;
    private Image imgGrade = null;
    private TMP_Text txtGrade = null;

    private Toggle togAudio;
    private Toggle togVideo;
    private float reClickDelay = 0.7f;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        SetMemberUI_WorldProfile();
        SetMemberUI_Office();
    }

    protected override void Start()
    {
        base.Start();

        sceneofficeRoom = SceneLogic.instance as Scene_OfficeRoom;

        bool isOffice = Util.UtilOffice.IsOffice();

        go_OfficeGrade.SetActive(isOffice);
        togAudio.gameObject.SetActive(isOffice);
        togVideo.gameObject.SetActive(isOffice);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ViewSetting();
    }

    #region 프로파일 UI
    private void SetMemberUI_WorldProfile()
    {
        base.SetMemberUI();

        txtmp_NickName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_NickName));
        GetUI_Button("btn_WorldProfile", () => PushPanel<Panel_CostumeInven>());

        GameObject go_RTView = GameObject.Find(nameof(go_RTView));
        if (go_RTView)
        {
            selector = go_RTView.GetComponentInChildren<AvatarPartsController>();
            anim_Avatar = go_RTView.GetComponentInChildren<Animator>();
            if (anim_Avatar != null)
            {
                selector.SetTarget(anim_Avatar.transform);
            }
        }

        tog_TabVideoChat = GetUI<Toggle>(nameof(tog_TabVideoChat));
        go_TabVideo_disable = GetChildGObject(nameof(go_TabVideo_disable));
        if (go_TabVideo_disable != null)
            go_TabVideo_disable.SetActive(false);
    }
    #endregion

    public void ViewSetting()
    {
        if (selector != null)
        {
            selector.SetAvatarParts(LocalPlayerData.AvatarInfo);
        }

        if (txtmp_NickName != null)
        {
            txtmp_NickName.text = LocalPlayerData.NickName;
        }
    }

    #region 오피스 TopLeft UI(오피스 등급, 오디오, 비디오 버튼)
    private void SetMemberUI_Office()
    {
        base.SetMemberUI();

        go_OfficeGrade = GetChildGObject(nameof(go_OfficeGrade));

        // 오피스 등급 표시 아이콘 & 텍스트
        imgGrade = GetUI<Image>("img_Grade");
        txtGrade = GetUI<TMP_Text>("txtmp_Grade");

        // 오디오 / 비디오 채팅
        togAudio = GetUI<Toggle>("tog_TabAudioChat");
        if (togAudio != null)
        {
            togAudio.onValueChanged.AddListener(OnValueChanged_LocalVoice);
        }

        togVideo = GetUI<Toggle>("tog_TabVideoChat");
        if (togAudio != null)
        {
            togVideo.onValueChanged.AddListener(OnValueChanged_LocalVideo);
        }
    }

    public async void OnValueChanged_LocalVoice(bool enable)
    {
        if (enable)
        {
            if (!Util.UtilOffice.CheckMicrophonePermission())
            {
                // 마이크 장치 접근권한이 없습니다
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_state_camera")));

                if (togAudio != null)
                    togAudio.isOn = false;
                return;
            }

            // 마이크 있는지 체크
            if (!Util.UtilOffice.CheckMicrophoneDevices())
            {
                // 마이크 장치가 없습니다
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_state_microphone")));

                if (togAudio != null)
                    togAudio.isOn = false;
                return;
            }

            if (IsHaveNoOfficePermission(eOfficePermissionMaster.VOICE, togAudio))
            {
                if (togAudio != null)
                    togAudio.isOn = false;
                return;
            }

            if (Single.Agora.GetLocalUser().isScreenShareEnabled)
            {
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("스크린 공유 중입니다.")));

                if (togAudio != null)
                    togAudio.isOn = false;
                return;
            }
        }

        var state = enable ? "언뮤트" : "뮤트";
        DEBUG.LOG($"로컬 플레이어 목소리를 {state} 합니다.", eColorManager.AGORA);

        await UniTask.NextFrame();

        Single.Sound.PlayEffect(Cons.click);

        Single.Agora.MuteLocalPlayerAudio(!enable);

        togAudio.enabled = false;

        await UniTask.Delay((int)(reClickDelay * 1000));

        togAudio.enabled = true;
    }

    public async void OnValueChanged_LocalVideo(bool enable)
    {
        if (enable)
        {
            if (!Util.UtilOffice.CheckCameraPermission())
            {
                // 카메라장치 사용권한이 없습니다
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_state_permission_camera")));

                if (togVideo != null)
                    togVideo.isOn = false;
                return;
            }

            // 카메라가 있는지 체크
            if (WebCamTexture.devices.Length == 0)
            {
                // 카메라 장치가 없습니다
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_state_camera")));

                if (togVideo != null)
                    togVideo.isOn = false;
                return;
            }

            if (IsHaveNoOfficePermission(eOfficePermissionMaster.VIDEO, togVideo))
            {
                if (togVideo != null)
                    togVideo.isOn = false;
                return;
            }

            if (Single.Agora.GetLocalUser().isScreenShareEnabled)
            {
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("스크린 공유 중입니다.")));

                if (togVideo != null)
                    togVideo.isOn = false;
                return;
            }
        }

        var state = enable ? "언뮤트" : "뮤트";
        DEBUG.LOG($"로컬 플레이어 비디오를 {state} 합니다.", eColorManager.AGORA);

        await UniTask.NextFrame();

        Single.Sound.PlayEffect(Cons.click);

        Single.Agora.MuteLocalPlayerVideo(!enable);

        togVideo.enabled = false;

        await UniTask.Delay((int)(reClickDelay * 1000));

        togVideo.enabled = true;
    }

    /// <summary>
    /// 오피스 권한 UI 설정 및 갱신
    /// </summary>
    /// <param name="newPermisionInfo"></param>
    public void UpdateUI_OfficePermission(OfficeUserInfo newPermisionInfo)
    {
        // 등급에 따른 아이콘 Sprite 설정
        if (imgGrade != null)
            imgGrade.sprite = Util.UtilOffice.GetAuthoritySprite(newPermisionInfo.Authority);

        // 등급에 따른 등급 텍스트 설정
        if (txtGrade != null)
        {
            string localKey = Util.UtilOffice.GetMasterLocal_OfficeAutority(newPermisionInfo.Authority);
            Util.SetMasterLocalizing(txtGrade, new MasterLocalData(localKey));
        }

        // 비디오 권한을 잃어 버린 경우, Mute 시켜줘야 함
        if (sceneofficeRoom.myPermission.VideoPermission && newPermisionInfo.VideoPermission == false)
        {
            togVideo.isOn = false;
            Single.Agora.MuteLocalPlayerVideo(true);
        }

        // 오디오 권한을 잃어 버린 경우, Mute 시켜줘야 함
        if (sceneofficeRoom.myPermission.VoicePermission && newPermisionInfo.VoicePermission == false)
        {
            togAudio.isOn = false;
            Single.Agora.MuteLocalPlayerAudio(true);
        }
    }

    private bool IsHaveNoOfficePermission(eOfficePermissionMaster permissionMaster, Toggle toggle)
    {
        if (sceneofficeRoom == null)
            return false;

        bool isRet = false;

        switch (permissionMaster)
        {
            case eOfficePermissionMaster.CHAT:
                {
                    if (sceneofficeRoom.myPermission.ChatPermission == false)
                    {
                        if (toggle != null)
                            toggle.isOn = false;

                        isRet = true;
                    }
                }
                break;

            case eOfficePermissionMaster.VOICE:
                {
                    if (sceneofficeRoom.myPermission.VoicePermission == false)
                    {
                        if (toggle != null)
                            toggle.isOn = false;

                        isRet = true;
                    }
                }
                break;

            case eOfficePermissionMaster.VIDEO:
                {
                    if (sceneofficeRoom.myPermission.VideoPermission == false)
                    {
                        if (toggle != null)
                            toggle.isOn = false;

                        isRet = true;
                    }
                    break;
                }

            case eOfficePermissionMaster.SCREEN:
                {
                    if (sceneofficeRoom.myPermission.ScreenPermission == false)
                    {
                        if (toggle != null)
                            toggle.isOn = false;

                        isRet = true;
                    }
                }
                break;
        }

        if (isRet)
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("office_reception_permission_unavailable")));

            if (toggle != null)
                toggle.isOn = false;
            isRet = true;
        }

        return isRet;
    }

    public void UpdateAudioChatToggleState(bool chatPermission)
    {
        if (togAudio != null)
            togAudio.isOn = chatPermission;

        Single.Agora.MuteLocalPlayerAudio(!chatPermission);
    }

    public void UpdateVideoChatToggleState(bool videoPermission)
    {
        if (togVideo != null)
            togVideo.isOn = videoPermission;

        Single.Agora.MuteLocalPlayerVideo(!videoPermission);
    }
    #endregion
}
