using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;
using System.Collections;

/// <summary>
/// DynamicScroll 에셋 이용 Item 뼈대 스크립트
/// </summary>
public class DynamicItemBase : DynamicScrollItem_Custom
{
    #region 변수
    protected Vector2 currentSize;
    #endregion

    #region 초기화
    protected override void SetMemberUI() 
    { 
        currentSize = GetComponent<RectTransform>().sizeDelta;
    }
    #endregion

    #region 데이터 세팅
    /// <summary>
    /// 데이터 세팅
    /// </summary>
    /// <param name="_data"></param>
    public override void UpdateData(DynamicScrollData scrollData)
    {
        base.UpdateData(scrollData);

        SetContent();
    }

    /// <summary>
    /// 프리팹에 데이터 세팅
    /// </summary>
    protected virtual void SetContent() { }


    /// <summary>
    /// 선택됐을 때 액션 실행
    /// </summary>
    public virtual void OnClick_Select()
    {
        OnSelect();
    }

    /// <summary>
    /// 클릭 시 사이즈 변경
    /// Scroll 컴포넌트에 다이나믹 사이즈 체크 되어있나 확인
    /// </summary>
    /// <param name="f"></param>
    protected virtual void ChangeContentSize(float f)
    {
        ((RectTransform)transform).sizeDelta = new Vector2(currentSize.x, f);
        OnUpdateItemSize();
    }
    #endregion
}
