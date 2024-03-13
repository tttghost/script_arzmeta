
using System;
using UnityEngine.UI.Extensions.EasingCore;
using UnityEngine.UI.Extensions;
using UnityEngine;
using UnityEngine.UI.Extensions.GridScrollView_Custom;

class GridView_Custom : FancyGridView<Item_Data, Context>
{
    class CellGroup : DefaultCellGroup { }

    [SerializeField] private FancyGridViewCell<Item_Data, Context> cellPrefab = default;
    protected override void SetupCellTemplate() => Setup<CellGroup>(cellPrefab);

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

    public float SpacingY
    {
        get => spacing;
        set
        {
            spacing = value;
            Relayout();
        }
    }

    public float SpacingX
    {
        get => startAxisSpacing;
        set
        {
            startAxisSpacing = value;
            Relayout();
        }
    }

    public void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }
        Context.SelectedIndex = index;
        Refresh();
    }

    // 한효주 - UpdateSelection 없이 현재 선택한 값만 바꿈
    public void ChangeValueSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }
        Context.SelectedIndex = index;
    }

    // 한효주 - UpdateSelection 없이 이전 선택했던 값만 바꿈
    public void ChangeValuePreSelection(int index)
    {
        if (Context.PreSelectIdx == index)
        {
            return;
        }
        Context.PreSelectIdx = index;
    }

    public void OnCellClicked(Action<int> callback)
    {
        Context.OnCellClicked = callback;
    }

    public void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Upper)
    {
        UpdateSelection(index);
        ScrollTo(index, duration, easing, GetAlignment(alignment));
    }

    public void JumpTo(int index, Alignment alignment = Alignment.Upper)
    {
        UpdateSelection(index);
        JumpTo(index, GetAlignment(alignment));
    }

    // 한효주 - UpdateSelection 없이 스크롤 위치만 바꿈 (스크롤링 o)
    public void ScrollToWithOutUpdate(int index, float duration, Ease easing, Alignment alignment = Alignment.Upper)
    {
        ScrollTo(index, duration, easing, GetAlignment(alignment));
    }

    // 한효주 - UpdateSelection 없이 스크롤 위치만 바꿈 (스크롤링 x)
    public void JumpToWithOutUpdate(int index, Alignment alignment = Alignment.Upper)
    {
        JumpTo(index, GetAlignment(alignment));
    }

    // 한효주 - 셀 사이즈 런타임에서 변경
    public void SetCellSize(Vector2 newSize)
    {
        cellSize = newSize;
    }

    // 한효주 - 그리드 그룹 개수 런타임에서 변경
    public void SetAxisCellCount(int count)
    {
        startAxisCellCount = count;
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
}
