using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using MEC;

public class Panel_MyRoomControl : PanelBase
{

    private Button btn_ItemSave;
    private Button btn_ItemLoad;
    private Button btn_ExitEditmode;
    private TogglePlus togplus_InvenPanel;
    private Button btn_ItemHistory;

    public float buttonScalTime = 0.5f;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_ItemSave = GetUI_Button(nameof(btn_ItemSave), MyRoomManager.Instance.OnClick_ItemSave);

        btn_ItemLoad = GetUI_Button(nameof(btn_ItemLoad), MyRoomManager.Instance.OnClick_ItemLoad);

        btn_ExitEditmode = GetUI_Button(nameof(btn_ExitEditmode), MyRoomManager.Instance.OnClick_ExitEditmode);

        togplus_InvenPanel = GetUI_TogglePlus(nameof(togplus_InvenPanel))
            .SetToggleAction(OnValueChanged_InvenPanelVisible)
            .SetToggleIsOn(true);

        btn_ItemHistory = GetUI_Button(nameof(btn_ItemHistory), () => PushPopup<Popup_ItemHistory>());
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        SaveLoadBtnInteractable(false);
    }


    /// <summary>
    /// 세이브로드버튼 인터렉터블 업데이트
    /// </summary>
    public void SaveLoadBtnInteractable()
    {
        bool isEqualMyRoom = Single.Web.myRoom.EqualEditmodeAndGrid();
        SaveLoadBtnInteractable(!isEqualMyRoom);
    }

    /// <summary>
    /// 세이브로드버튼 인터렉터블 강제
    /// </summary>
    public void SaveLoadBtnInteractable(bool b)
    {
        btn_ItemSave.interactable = b;
        btn_ItemLoad.interactable = b;
    }

    /// <summary>
    /// 인벤패널 보이게/안보이게
    /// </summary>
    /// <param name="iNVENLOCK"></param>
    private void OnValueChanged_InvenPanelVisible(bool isOn)
    {
        MyRoomManager.Instance.iNVENLOCK = isOn ? eINVENLOCK.UNLOCK : eINVENLOCK.LOCK;
        MyRoomManager.Instance.panel_MyRoomInven.SetInvenLock(MyRoomManager.Instance.iNVENLOCK);
    }

    /// <summary>
    /// 컨트롤패널 보이게/안보이게
    /// </summary>
    public void OnClick_ControlPanelVisible(eCONTROLLOCK eCONTROLLOCK) => Co_ControlPanelVisible(eCONTROLLOCK).RunCoroutine();
    private IEnumerator<float> Co_ControlPanelVisible(eCONTROLLOCK eCONTROLLOCK)
    {
        //입력제어
        float oriAlpha = 0f;
        float targetAlpha = 0f;
        switch (eCONTROLLOCK)
        {
            case eCONTROLLOCK.LOCK:
                canvasGroup.blocksRaycasts = false;
                oriAlpha = 1f;
                break;
            case eCONTROLLOCK.UNLOCK:
                canvasGroup.blocksRaycasts = true;
                targetAlpha = 1f;
                break;
        }

        //버튼 EaseingFunction 셋업
        EasingFunction.Ease ease = default;
        switch (eCONTROLLOCK)
        {
            case eCONTROLLOCK.LOCK:
                ease = EasingFunction.Ease.EaseInBack;
                break;
            case eCONTROLLOCK.UNLOCK:
                ease = EasingFunction.Ease.EaseOutBack;
                break;
        }
        EasingFunction.Function function = EasingFunction.GetEasingFunction(ease);

        //버튼 스케일 셋업
        CheckMouseOverPanel checkMouseOverPanel = GetComponent<CheckMouseOverPanel>();
        List<RectTransform> rectTransforms = checkMouseOverPanel.target;
        float oriScale = 0;
        float targetScale = 0;
        switch (eCONTROLLOCK)
        {
            case eCONTROLLOCK.LOCK:
                oriScale = 1f;
                break;
            case eCONTROLLOCK.UNLOCK:
                targetScale = 1f;
                break;
        }

        //버튼 스케일&알파값 조정
        float curTime = 0f;
        while (curTime < 1f)
        {
            curTime += Timing.DeltaTime / buttonScalTime;
            for (int i = 0; i < rectTransforms.Count; i++)
            {
                rectTransforms[i].localScale = Vector3.one * function.Invoke(oriScale, targetScale, curTime);
            }
            canvasGroup.alpha = Mathf.Lerp(oriAlpha, targetAlpha, curTime);
            yield return Timing.WaitForOneFrame;
        }
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            rectTransforms[i].localScale = Vector3.one * targetScale;
        }
        canvasGroup.alpha = targetAlpha;
    }

}