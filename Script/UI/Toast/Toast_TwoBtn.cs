using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using UnityEngine.Events;
using System;
using FrameWork.UI;

public class Toast_TwoBtn : ToastBase
{
    #region 변수
    private ToastData_TwoBtn toastData;

    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Confirm;
    private TMP_Text txtmp_Cancel;
    private TMP_Text txtmp_Seconds;
    private Button btn_Confirm;
    private Button btn_Cancel;
    private CanvasGroup canvasGroup_UI;

    private MasterLocalData defaultConfirmLocal = new MasterLocalData("10012"); // 수락
    private MasterLocalData defaultCancelLocal = new MasterLocalData("10013"); // 거절

    private Action confirmAction = null;
    private Action cancelAction = null;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_Confirm = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Confirm), defaultConfirmLocal);
        txtmp_Cancel = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Cancel), defaultCancelLocal);
        txtmp_Seconds = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Seconds));
        #endregion

        #region Button
        btn_Confirm = GetUI_Button(nameof(btn_Confirm), () =>
        {
            confirmAction?.Invoke();
            confirmAction = null;
            ToastPopupClose();
        });
        btn_Cancel = GetUI_Button(nameof(btn_Cancel), () =>
        {
            cancelAction?.Invoke();
            cancelAction = null;
            ToastPopupClose();
        });
        #endregion

        #region etc
        canvasGroup_UI = GetChildGObject("go_UI").GetComponent<CanvasGroup>();
        #endregion
    }

    #region 초기화
    protected override void OnDisable()
    {
        CountDownManager.Instance.RemoveSecondAction(nameof(gameObject));
    }

    protected override void SavaData(object data)
    {
        if (data is ToastData_TwoBtn _data)
        {
            toastData = _data;
            curToastTime = toastData.time;
        }
    }

    /// <summary>
    /// 토스트 팝업 데이터 세팅 및 애니메이션 실행 본체 메소드
    /// </summary>
    protected override void SetContent()
    {
        // 초기화
        InitData();

        SoundNomal();

        if (txtmp_Title != null)
        {
            Util.SetMasterLocalizing(txtmp_Title, toastData.titleLocal);
        }

        if (toastData.action != null)
        {
            confirmAction = toastData.action.confirmAction;
            cancelAction = toastData.action.cancelAction;
        }

        if (toastData.btnLocal != null)
        {
            if (txtmp_Confirm != null)
            {
                MasterLocalData localData = toastData.btnLocal.confirmLocal ?? defaultConfirmLocal;
                Util.SetMasterLocalizing(txtmp_Confirm, localData);
            }
            if (txtmp_Cancel != null)
            {
                MasterLocalData localData = toastData.btnLocal.cancelLocal ?? defaultCancelLocal;
                Util.SetMasterLocalizing(txtmp_Cancel, localData);
            }
        }
    }

    /// <summary>
    /// 켜질 때마다 초기화시켜줄 것
    /// </summary>
    private void InitData()
    {
        if (canvasGroup_UI != null)
        {
            canvasGroup_UI.alpha = 0;
            canvasGroup_UI.blocksRaycasts = false;
        }

        SetOpenEndCallback(() =>
        {
            Timing.WaitUntilDone(Util.RunCoroutine(Co_SetOnOffBtn(true), "Co_SetOnOffBtn"));

            CountDownManager.Instance.SetCountDown(nameof(gameObject), (int)toastData.time);
            CountDownManager.Instance.AddSecondAction(nameof(gameObject), (count) => Util.SetMasterLocalizing(txtmp_Seconds, new MasterLocalData("common_count_second", count)));
        });

        SetCloseStartCallback(() =>
        {
            Util.RunCoroutine(Co_SetOnOffBtn(false), "Co_SetOnOffBtn");
        });
    }

    /// <summary>
    /// 버튼 캔버스 그룹 알파 조정
    /// </summary>
    private float curTime;
    private float durTime = 0.5f;
    private IEnumerator<float> Co_SetOnOffBtn(bool isSetOn)
    {
        curTime = 0f;

        float start = isSetOn ? 0 : 1;
        float end = isSetOn ? 1 : 0;

        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;

            float alpha = Mathf.Lerp(start, end, curTime);
            canvasGroup_UI.alpha = alpha;

            yield return Timing.WaitForOneFrame;
        }

        canvasGroup_UI.alpha = end;
        canvasGroup_UI.blocksRaycasts = isSetOn;
    }
    #endregion
}

#region ToastData
/// <summary>
/// 토스트 데이터
/// </summary>
public class ToastData_TwoBtn
{
    public TOASTICON type;
    public MasterLocalData titleLocal;
    public float time;
    public ToastAction action;
    public ToastBtnLocalData btnLocal;

    public ToastData_TwoBtn(TOASTICON type, MasterLocalData titleLocal, float time = 3f, ToastAction action = null, ToastBtnLocalData btnLocal = null)
    {
        this.type = type;
        this.titleLocal = titleLocal;
        this.time = time;
        this.action = action;
        this.btnLocal = btnLocal;
    }
}

/// <summary>
/// 팝업의 액션을 넣어줄때 사용한다.
/// </summary>
public class ToastAction
{
    public Action confirmAction;
    public Action cancelAction;
    public ToastAction(Action confirmAction, Action cancelAction = null)
    {
        this.confirmAction = confirmAction;
        this.cancelAction = cancelAction;
    }
}

/// <summary>
/// 팝업의 버튼 로컬을 넣어줄때 사용한다.
/// </summary>
public class ToastBtnLocalData
{
    public MasterLocalData confirmLocal; // 기본값 : 수락
    public MasterLocalData cancelLocal; // 기본값 : 거절
    public ToastBtnLocalData(MasterLocalData confirmLocal, MasterLocalData cancelLocal = null)
    {
        this.confirmLocal = confirmLocal;
        this.cancelLocal = cancelLocal;
    }
}
#endregion
