using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.GridScrollView_Custom;

/// <summary>
/// 설명 : 유한 오브젝트 재활용 그리드 스크롤뷰 아이템 Base Class 
/// 사용 스크롤뷰 스크립트 : GridView_Custom
/// 사용 방법 : 아이템으로 사용할 프리팹에 해당 스크립트를 상속받은 컴포넌트 추가
///            GridView_Custom에 아이템 프리팹 연결
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\Unity UI Extensions\2.2.7\UI Extensions Samples\FancyScrollView\08_GridView
/// </summary>
[RequireComponent(typeof(UIBase))]
class FancyGridViewCell_Custom : FancyGridViewCell<Item_Data, Context>
{
    protected UIBase uIBase;
    private bool isInit = false;

    private void Awake()
    {
        InitModule();
    }

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

        if (uIBase ??= GetComponent<UIBase>())
        {
            isInit = true;

            uIBase.Initialize();
            SetMemberUI();
        }
    }
}
