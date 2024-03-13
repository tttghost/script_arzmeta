
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.EasingCore;
using UnityEngine.UI.Extensions.ScrollView_Custom;
/// <summary>
/// 설명 : 유한 오브젝트 재활용 스크롤뷰 (애니메이션 없음)
/// 사용 아이템 스크립트 : FancyScrollRectCell_Custom
/// 사용 방법 : 스크롤뷰 오브젝트에 해당 스크립트를 상속받은 컴포넌트 추가
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\Unity UI Extensions\2.2.7\UI Extensions Samples\FancyScrollView\07_ScrollRect
/// 깃북 설명 : https://app.gitbook.com/o/P24J6mGIMxZ1TZZ26BF6/s/FKvL5BhhY47dSP7Pft7v/assets/ui/unity-ui-extensions/fancyscrollview/07_scrollrect
/// </summary>

class ScrollView_Custom : FancyScrollRect<Item_Data, Context>
{
    [SerializeField] float cellSize = 100f;
    [SerializeField] public GameObject cellPrefab = default;

    protected override float CellSize => cellSize;
    protected override GameObject CellPrefab => cellPrefab;
    public int DataCount => ItemsSource.Count;

    public float PaddingTop
    {
        get => paddingHead;
        set
        {
            paddingHead = value;
            Relayout();
        }
    }

    public float PaddingBottom
    {
        get => paddingTail;
        set
        {
            paddingTail = value;
            Relayout();
        }
    }

    public float Spacing
    {
        get => spacing;
        set
        {
            spacing = value;
            Relayout();
        }
    }

    public void OnCellClicked(Action<int> callback)
    {
        Context.OnCellClicked = callback;
    }

    /// <summary>
    /// 아이템 스크롤뷰에 생성
    /// 기존 아이템 전부 삭제 후 새로 받은 아이템으로 다시 생성함
    /// </summary>
    /// <param name="items"></param>
    public void UpdateData(IList<Item_Data> items)
    {
        UpdateContents(items);
    }

    public void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Middle)
    {
        UpdateSelection(index);
        ScrollTo(index, duration, easing, GetAlignment(alignment));
    }

    public void JumpTo(int index, Alignment alignment = Alignment.Middle)
    {
        UpdateSelection(index);
        JumpTo(index, GetAlignment(alignment));
    }

    float GetAlignment(Alignment alignment)
    {
        switch (alignment)
        {
            case Alignment.Upper: return 0.0f;
            case Alignment.Middle: return 0.5f;
            case Alignment.Lower: return 1.0f;
            default: return GetAlignment(Alignment.Middle);
        }
    }

    void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();
    }
}
