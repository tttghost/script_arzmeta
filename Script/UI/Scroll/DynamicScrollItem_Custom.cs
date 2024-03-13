using FrameWork.UI;
using System;
using UnityEngine;

/// <summary>
/// 설명 : 
/// 사용 스크롤뷰 스크립트 : DynamicScrollerBase_View / DynamicScrollerBase_Panel
/// 사용 방법 : 아이템으로 사용할 프리팹에 해당 스크립트를 상속받은 컴포넌트 추가
///            아이템 데이터 클래스에 DynamicScrollData 상속
/// 원본 예제(프로젝트) : Assets\_DEV\ThirdParty\GPM\UI\Sample\InfiniteScroll\Scene\DynamicSampleScene
/// 원본 예제(웹사이트) : https://github.com/nhn/gpm.unity
/// </summary>

public class DynamicScrollData
{

}

public class DynamicScrollItem_Custom : UIBase
{
    protected bool activeItem;

    protected DynamicScroll_Custom scroll = null;

    protected DynamicScrollData scrollData = null;
    protected Action<DynamicScrollData> selectCallback = null;
    protected Action<DynamicScrollData, RectTransform> updateSizeCallback = null;

    internal int itemIndex = -1;
    internal int dataIndex = -1;

    public bool IsActiveItem
    {
        get
        {
            return activeItem;
        }
    }

    internal void Initalize(DynamicScroll_Custom scroll, int itemIndex)
    {
        this.scroll = scroll;
        this.itemIndex = itemIndex;
    }

    internal void SetData(int dataIndex, bool notifyEvent = true)
    {
        this.scrollData = scroll.GetData(dataIndex);
        this.dataIndex = dataIndex;

        SetActive(true, notifyEvent);
    }

    internal void ClearData(bool notifyEvent = true)
    {
        this.scrollData = null;
        this.dataIndex = -1;

        SetActive(false, notifyEvent);
    }

    public void AddSelectCallback(Action<DynamicScrollData> callback)
    {
        selectCallback += callback;
    }

    public void RemoveSelectCallback(Action<DynamicScrollData> callback)
    {
        selectCallback -= callback;
    }

    public virtual void UpdateData(DynamicScrollData scrollData)
    {
        this.scrollData = scrollData;
    }

    protected void OnSelect()
    {
        if (selectCallback != null)
        {
            selectCallback(scrollData);
        }
    }

    public virtual void SetActive(bool active, bool notifyEvent = true)
    {
        activeItem = active;

        gameObject.SetActive(activeItem);


        if (notifyEvent == true)
        {
            if (scroll != null)
            {
                scroll.onChangeActiveItem.Invoke(dataIndex, activeItem);
            }
        }
    }

    public void SetSize(Vector2 sizeDelta)
    {
        ((RectTransform)transform).sizeDelta = sizeDelta;
        OnUpdateItemSize();
    }

    public void AddUpdateSizeCallback(Action<DynamicScrollData, RectTransform> callback)
    {
        updateSizeCallback += callback;
    }

    public void RemoveUpdateSizeCallback(Action<DynamicScrollData, RectTransform> callback)
    {
        updateSizeCallback -= callback;
    }

    protected void OnUpdateItemSize()
    {
        if (updateSizeCallback != null)
        {
            updateSizeCallback(scrollData, transform as RectTransform);
        }
    }
}
