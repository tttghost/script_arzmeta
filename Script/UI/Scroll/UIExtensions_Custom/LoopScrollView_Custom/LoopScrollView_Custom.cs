using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.LoopScrollView_Custom;
using UnityEngine.UI.Extensions.EasingCore;

/// <summary>
/// 설명 : 무한 오브젝트 재활용 스크롤뷰 Base Class (애니메이션 있음) [뫼비우스의 띠처럼 처음과 마지막이 연결되어있음]
/// FancyScrollView에서 Loop 옵션이 On일 경우, 마지막 셀이 첫 번째 셀 앞에 정렬되고 첫번째 셀이 마지막 셀 뒤에 정렬되도록 셀이 순환됨
/// Scroller의 MoveType을 Unrestricted로 설정해 스크롤 범위를 무제한으로 설정
/// 사용 아이템 스크립트 : FancyCell_Custom
/// 사용 방법 : 스크롤뷰 오브젝트에 해당 스크립트를 상속받은 컴포넌트 추가
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\Unity UI Extensions\2.2.7\UI Extensions Samples\FancyScrollView\03_InfiniteScroll
/// 
/// </summary>
class LoopScrollView_Custom : FancyScrollView<Item_Data, Context>
{
    public Scroller scroller = default;
    public GameObject cellPrefab = default;

    [Header("셀 선택 시 업데이트")]
    public bool IsUpdate = true;
    [Header("셀 선택 시 스크롤")]
    public bool IsScrollTo = true;

    protected override GameObject CellPrefab => cellPrefab;

    protected override void Initialize()
    {
        base.Initialize();

        Context.OnCellClicked = SelectCell;

        scroller.OnValueChanged(UpdatePosition);
        scroller.OnSelectionChanged(UpdateSelection);
    }

    /// <summary>
    /// 선택된 아이템이 바뀌었을 때 호출
    /// </summary>
    /// <param name="index"></param>
    public void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();
    }

    /// <summary>
    /// 아이템 스크롤뷰에 생성
    /// 기존 아이템 전부 삭제 후 새로 받은 아이템으로 다시 생성함
    /// </summary>
    /// <param name="items"></param>
    public void UpdateData(IList<Item_Data> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }

    /// <summary>
    /// 다음 아이템 선택 (마지막 인덱스일 시 처음으로)
    /// </summary>
    public void SelectNextCell()
    {
        int index = Context.SelectedIndex + 1;
        if (index >= ItemsSource.Count)
            index = 0;

        SelectCell(index);
    }

    /// <summary>
    /// 이전 아이템 선택 (처음일 시 마지막 인덱스로)
    /// </summary>
    public void SelectPrevCell()
    {
        int index = Context.SelectedIndex - 1;
        if (index < 0)
            index = ItemsSource.Count - 1;

        SelectCell(index);
    }

    /// <summary>
    /// 아이템 선택
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isScroll"></param>
    public void SelectCell(int index)
    {
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex)
        {
            return;
        }

        if (IsUpdate)
        {
            UpdateSelection(index);
        }

        if (IsScrollTo)
        {
            ScrollTo(index);
        }
    }

    /// <summary>
    /// 해당 인덱스의 아이템 있는 곳으로 스크롤링
    /// </summary>
    /// <param name="index"></param>
    public void ScrollTo(int index)
    {
        scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
    }

    /// <summary>
    /// 해당 인덱스의 아이템이 있는 곳으로 이동 (스크롤링 x)
    /// </summary>
    /// <param name="index"></param>
    public void JumpTo(int index)
    {
        scroller.JumpTo(index);
    }

    /// <summary>
    /// 해상도에 따른 아이템 스크롤링 간격 적용
    /// </summary>
    /// <param name="interval"></param>
    public void SetCellIntercal(float interval)
    {
        if (interval < 0f || interval > 1f) return;
        cellInterval = interval;
    }
}
