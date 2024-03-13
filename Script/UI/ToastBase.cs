using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=======================================================
// 신규 토스트 팝업 제작 시
// 1.
// 2.
// 3.
// 4.
// 5.
// 6.
// 7.
//=======================================================

public class ToastBase : UIBase
{
    #region 변수
    // 다른 토스트 팝업보다 우선시 되어 떠야할 경우 true 체크
    public bool isTopPriority = false;

    protected Animator toastAnim;
    private string toastOpen = "Open";
    private string toastClose = "Close";
    private float aniOffset = 0.5f;

    protected float curToastTime = -1f;
    #endregion

    protected override void SetMemberUI()
    {
        #region etc
        toastAnim = GetComponent<Animator>();
        if (toastAnim == null)
        {
            toastAnim = gameObject.AddComponent<Animator>();
            toastAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("BuildAssetBundle/UI/Animation/Anim_ToastPopup");
        }
        #endregion

        LoadIcon();
    }

    private void OnDestroy()
    {
        Util.ClearQueue_Coroutine(Cons.Toast);
    }

    /// <summary>
    /// 아이콘 스프라이트 로드
    /// </summary>
    protected virtual void LoadIcon() { }

    /// <summary>
    /// 토스트 팝업 데이터 세팅 및 애니메이션 실행 본체 메소드
    /// base 상속 필수
    /// </summary>
    public virtual void ChainToastData(object data) 
    { 
        if (isTopPriority) ClearCoroutine();
        Util.ProcessQueue_Coroutine(Cons.Toast, Co_ToastPopup(data), tag: Cons.Toast);
    }

    /// <summary>
    /// 데이터 변환 및 저장
    /// 자식 클래스에서 해당 토스트 팝업 데이터 형식에 맞는 클래스로 정보 변환
    /// </summary>
    /// <param name="data"></param>
    protected virtual void SavaData(object data) { }

    /// <summary>
    /// 데이터 세팅
    /// </summary>
    protected virtual void SetContent() { }

    /// <summary>
    /// 다른 토스트 팝업보다 우선시 되어 떠야할 경우
    /// isTopPriority를 true 하면 해당 토스트 팝업이 호출되면 이전에 있던 모든 베이직 토스트 팝업이 취소된다
    /// </summary>
    protected virtual void ClearCoroutine()
    {
        Util.ClearQueue_Coroutine(Cons.Toast, actClear: () =>
         {
             if (OpenToast<Toast_Basic>() is ToastBase toastBase)
             {
                 if (!toastBase._myGameObject.activeInHierarchy) return;

                 toastBase.ToastPopupClose();
             }
         });
    }

    #region 토스트 팝업 사운드
    protected void SoundWorng() => Single.Sound.PlayWarningPopupSound();

    protected void SoundNomal() => Single.Sound.PlayOneButtonPopupSound();
    #endregion

    #region 토스트 팝업 데이터 세팅 및 애니메이션
    /// <summary>
    /// 토스트 팝업 열기
    /// </summary>
    /// <param name="time"></param>
    public void ToastPopupOpen(object data) => Util.RunCoroutine(Co_ToastPopup(data));

    /// <summary>
    /// 토스트 팝업 닫기
    /// </summary>
    public void ToastPopupClose() => Util.RunCoroutine(Co_CloseToastPopup(), Cons.Toast);

    public IEnumerator<float> Co_ToastPopup(object data)
    {
        SavaData(data);
        SetContent();

        if (curToastTime < 0f) yield break;

        yield return Timing.WaitUntilDone(Co_OpenToastPopup());
        yield return Timing.WaitUntilDone(Co_CloseToastPopup());
    }

    protected virtual IEnumerator<float> Co_OpenToastPopup()
    {
        _myGameObject.SetActive(true);
        transform.SetAsLastSibling();
        toastAnim.Rebind();

        OpenStartAct();
        toastAnim.SetTrigger(toastOpen);
        yield return Timing.WaitForSeconds(aniOffset);
        OpenEndAct();

        yield return Timing.WaitForSeconds(curToastTime); // 기본 3초 대기
    }

    protected virtual IEnumerator<float> Co_CloseToastPopup()
    {
        CloseStartAct();
        toastAnim.SetTrigger(toastClose);
        yield return Timing.WaitForSeconds(aniOffset);
        CloseEndAct();

        toastAnim.Rebind();
        _myGameObject.SetActive(false);
    }
    #endregion
}
