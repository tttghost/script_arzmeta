using UnityEngine.UI.Extensions.LoopScrollView_Custom;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using FrameWork.UI;

/// <summary>
/// 설명 : 무한 오브젝트 재활용 스크롤뷰 아이템 Base Class (애니메이션 있음)
/// 사용 스크롤뷰 스크립트 : LoopScrollView_Custom
/// 사용 방법 : 아이템으로 사용할 프리팹에 해당 스크립트를 상속받은 컴포넌트 추가
///            LoopScrollView_Custom에 아이템 프리팹 연결
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\Unity UI Extensions\2.2.7\UI Extensions Samples\FancyScrollView\03_InfiniteScroll
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// 
[RequireComponent(typeof(UIBase))]
class FancyCell_Custom : FancyCell<Item_Data, Context>
{
    [SerializeField] protected Animator animator;

    protected UIBase uIBase;

    protected static string animatorHashName; // Null이면 에셋이 작동하지 않습니다. 해당 스크롤 애니메이션 이름을 넣어주세요.

    protected float currentPosition = 0;

    private bool isInit = false;

    private void Awake()
    {
        InitModule();
        SetAnimatorHash();
    }

    /// <summary>
    /// UI 세팅 (UIBase를 상속받는 것이 아니라 하단의 #region Module을 수동 업데이트 해줘야 한다)
    /// </summary>
    protected virtual void SetMemberUI() { }

    public override void UpdateContent(Item_Data itemData) => SetContent();

    /// <summary>
    /// 받아온 데이터로 아이템 콘텐츠 세팅
    /// </summary>
    protected virtual void SetContent() { }

    /// <summary>
    /// AnimatorHash 세팅 (필수)
    /// </summary>
    protected virtual void SetAnimatorHash() { }

    public override void UpdatePosition(float position)
    {
        currentPosition = position;

        if (animator.isActiveAndEnabled)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
        }

        animator.speed = 0;
    }

    #region 초기화
    protected static class AnimatorHash
    {
        public static readonly int Scroll = Animator.StringToHash(animatorHashName);
    }

    public void InitModule()
    {
        if (isInit) return;

        if (uIBase ??= GetComponent<UIBase>())
        {
            isInit = true;

            uIBase.Initialize();
            SetMemberUI();
        }
    }

    protected virtual void OnEnable() => UpdatePosition(currentPosition);
    #endregion
}
