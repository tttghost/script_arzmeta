using BKK;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class View_System : UIBase
{
    #region 변수
    private Slider sld_BGM;
    private Slider sld_Effect;
    private Slider sld_Media;

    private CanvasGroup go_BGM;

    private TogglePlus togplus_Low;
    private TogglePlus togplus_High;
    private TogglePlus togplus_Ko;
    private TogglePlus togplus_En;
    private TogglePlus togplus_Reception;
    private TogglePlus togplus_Unsubscribe;

    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        // 사운드 관련
        GetUI_TxtmpMasterLocalizing("txtmp_SoundTitle", new MasterLocalData("10202"));
        GetUI_TxtmpMasterLocalizing("txtmp_BGM", new MasterLocalData("9400"));
        GetUI_TxtmpMasterLocalizing("txtmp_Effect", new MasterLocalData("9401"));
        GetUI_TxtmpMasterLocalizing("txtmp_Media", new MasterLocalData("9405"));

        // 시스템 관련
        GetUI_TxtmpMasterLocalizing("txtmp_SystemTitle", new MasterLocalData("9004"));
        GetUI_TxtmpMasterLocalizing("txtmp_GraphicQuality", new MasterLocalData("10203"));
        GetUI_TxtmpMasterLocalizing("txtmp_Low", new MasterLocalData("10204"));
        GetUI_TxtmpMasterLocalizing("txtmp_High", new MasterLocalData("10205"));
        GetUI_TxtmpMasterLocalizing("txtmp_Language", new MasterLocalData("9402"));
        GetUI_TxtmpMasterLocalizing("txtmp_Ko", new MasterLocalData("9403"));
        GetUI_TxtmpMasterLocalizing("txtmp_En", new MasterLocalData("9404"));
        GetUI_TxtmpMasterLocalizing("txtmp_PushPopup", new MasterLocalData("10215"));
        GetUI_TxtmpMasterLocalizing("txtmp_Reception", new MasterLocalData("10216"));
        GetUI_TxtmpMasterLocalizing("txtmp_Unsubscribe", new MasterLocalData("10217"));
        #endregion

        #region Toggle
        togplus_Low = GetUI<TogglePlus>(nameof(togplus_Low));
        if (togplus_Low != null)
        {
            togplus_Low.SetToggleOnAction(() => Util.Quality = QUALITY_LEVEL.Low);
        }
        togplus_High = GetUI<TogglePlus>(nameof(togplus_High));
        if (togplus_High != null)
        {
            togplus_High.SetToggleOnAction(() => Util.Quality = QUALITY_LEVEL.Medium);
        }
        togplus_Ko = GetUI<TogglePlus>(nameof(togplus_Ko));
        if (togplus_Ko != null)
        {
            togplus_Ko.SetToggleOnAction(() => AppGlobalSettings.Instance.language = Language.Korean);
        }
        togplus_En = GetUI<TogglePlus>(nameof(togplus_En));
        if (togplus_En != null)
        {
            togplus_En.SetToggleOnAction(() => AppGlobalSettings.Instance.language = Language.English);
        }
        togplus_Reception = GetUI<TogglePlus>(nameof(togplus_Reception));
        togplus_Unsubscribe = GetUI<TogglePlus>(nameof(togplus_Unsubscribe));
        #endregion

        #region etc
        go_BGM = GetChildGObject(nameof(go_BGM)).GetComponent<CanvasGroup>();

        sld_BGM = GetUI<Slider>(nameof(sld_BGM));
        if (sld_BGM != null)
        {
            sld_BGM.value = AppGlobalSettings.Instance.volumeBGM;
            sld_BGM.onValueChanged.AddListener((volume) =>
            {
                Single.Sound.SetVolume("BGM", volume);
                AppGlobalSettings.Instance.volumeBGM = volume;
            });
        }
        sld_Effect = GetUI<Slider>(nameof(sld_Effect));
        if (sld_Effect != null)
        {
            sld_Effect.value = AppGlobalSettings.Instance.volumeEffect;
            sld_Effect.onValueChanged.AddListener((volume) =>
            {
                Single.Sound.SetVolume("Effect", volume);
                AppGlobalSettings.Instance.volumeEffect = volume;
            });
        }
        sld_Media = GetUI<Slider>(nameof(sld_Media));
        if (sld_Media != null)
        {
            sld_Media.value = AppGlobalSettings.Instance.volumeMedia;
            sld_Media.onValueChanged.AddListener((volume) =>
            {
                Single.Sound.SetMediaSliderValue(volume);
                AppGlobalSettings.Instance.volumeMedia = volume;
            });
        }
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        base.OnEnable();
        RefreshVolumeUI();
        RefreshSystemUI();
    }

    /// <summary>
    /// 배경음악 볼륨 조절 불/가능 여부
    /// </summary>
    public void RefreshVolumeUI()
    {
        if (go_BGM != null)
        {
            bool b = Single.Sound.playMedia;

            go_BGM.alpha = b ? 0.3f : 1f;
            go_BGM.interactable = !b;
        }
    }

    /// <summary>
    /// 토글 값 Init
    /// </summary>
    public void RefreshSystemUI()
    {
        // 퀄리티 세팅
        if (togplus_Low != null && togplus_High != null)
        {
            bool b = Util.Quality == QUALITY_LEVEL.Low;
            togplus_Low.SetToggleIsOnWithoutNotify(b);
            togplus_High.SetToggleIsOnWithoutNotify(!b);
        }
        // 언어
        if (togplus_Ko != null && togplus_En != null)
        {
            bool b = AppGlobalSettings.Instance.language == Language.Korean;
            togplus_Ko.SetToggleIsOnWithoutNotify(b);
            togplus_En.SetToggleIsOnWithoutNotify(!b);
        }
        // 푸쉬 알림
        if (togplus_Reception != null && togplus_Unsubscribe != null)
        {

        }
    }
    #endregion
}

