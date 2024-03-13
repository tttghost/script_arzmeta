using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.ScrollView_Custom;
/// <summary>
/// 설명 : 유한 오브젝트 재활용 스크롤뷰 아이템 Base Class (애니메이션 없음)
/// 사용 스크롤뷰 스크립트 : ScrollView_Custom
/// 사용 방법 : 아이템으로 사용할 프리팹에 해당 스크립트를 상속받은 컴포넌트 추가
///            ScrollView_Custom에 아이템 프리팹 연결
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\Unity UI Extensions\2.2.7\UI Extensions Samples\FancyScrollView\07_ScrollRect
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
[RequireComponent(typeof(UIBase))]
class FancyScrollRectCell_Custom : FancyScrollRectCell<Item_Data, Context>
{
    protected UIBase uIBase;
    private bool isInit = false;

    private void Awake()
    {
        InitModule();
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

    protected override void UpdatePosition(float normalizedPosition, float localPosition)
    {
        base.UpdatePosition(normalizedPosition, localPosition);
    }

    public void InitModule()
    {
        if (isInit) return;

        if(uIBase ??= GetComponent<UIBase>())
        {
            isInit = true;

            uIBase.Initialize();
            SetMemberUI();
        }
    }
}
