using Cysharp.Threading.Tasks;
using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MyRoomInven : PanelBase
{
    [HideInInspector] public int                    categoryType;
    [HideInInspector] public List<MyRoomItemData>   itemDataList = new List<MyRoomItemData>();  //인벤아이템 리스트
    [HideInInspector] public List<Slot>             slotList = new List<Slot>();                //인벤슬롯 리스트

    public GameObject                               inventoryItem;
    public GameObject                               inventorySlot;
    public int                                      slotAmount = 16;

    public delegate void InvenItemHandler(int itemId, int num);
    public InvenItemHandler                         handlerPlusRoomItem;                        //룸아이템 생성


    private GameObject                              go_Content;
    private ToggleGroup                             togg_Tab;
    private Scrollbar                               scrollbar_Horizontal;
    public float                                    invenLockTime = 0.2f;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        InitToggle();
        go_Content = GetChildGObject(nameof(go_Content));
        scrollbar_Horizontal = GetUI<Scrollbar>(nameof(scrollbar_Horizontal));
    }


    #region 유니티 함수


    /// <summary>
    /// 인벤토리 정보 셋업
    /// </summary>
    public void SetupInven()
    {
        ClearData();            // 기존 슬롯과 아이템의 오브젝트, 리스트 제거
        InitSlot();             // 슬롯초기화
        InitItem();             // 아이템초기화 ★
        ToggleIsOn(firstTog);   // 탭 초기값으로
    }

    /// <summary>
    /// 기존 슬롯과 아이템의 오브젝트, 리스트 제거
    /// </summary>
    private void ClearData()
    {
        foreach (var item in itemDataList)
        {
            Destroy(item.gameObject);
        }
        itemDataList.Clear();
        foreach (var item in slotList)
        {
            Destroy(item.gameObject);
        }
        slotList.Clear();
    }


    #endregion

    protected override async void OnEnable()
    {
        base.OnEnable();
        await UniTask.Delay(1);
        ToggleIsOn(firstTog);
    }

    Toggle firstTog;
    #region 토글
    /// <summary>
    /// 토글 초기화
    /// </summary>
    private void InitToggle()
    {
        togg_Tab = GetUI<ToggleGroup>(nameof(togg_Tab));
        Toggle[] togs = togg_Tab.GetComponentsInChildren<Toggle>();

        List<eFurnitureCategory> furnitureCategoryList = Util.Enum2List<eFurnitureCategory>();
        for (int i = 0; i < furnitureCategoryList.Count; i++)
        {
            int capture = i;
            Toggle tog = togs[capture];
            eFurnitureCategory furnitureCategory = furnitureCategoryList[capture];
            Util.SetMasterLocalizing(tog.GetComponentInChildren<TMP_Text>(), new MasterLocalData(furnitureCategory.ToString()));
            SetToggleIsOn(tog, () => OnValueChanged_Tab((int)furnitureCategory));

            if(capture == 0)
            {
                firstTog = tog;
            }
        }
    }


    /// <summary>
    /// 탭 눌렀을 때 아이템 카테고리에 따라 정렬
    /// </summary>
    /// <param name="categoryType"></param>
    private void OnValueChanged_Tab(int categoryType)
    {
        this.categoryType = categoryType;
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].itemId = -1;
            slotList[i].gameObject.SetActive(false);
        }
        if (categoryType == 0)//모든아이템보기
        {
            for (int i = 0; i < itemDataList.Count; i++)
            {
                MyRoomItemData itemData = itemDataList[i];
                itemData.slotId = i;
                slotList[i].gameObject.SetActive(true);
                slotList[i].itemId = itemData.itemId;
                //slotList[i].itemId = itemData.invenItem.itemId;
            }
        }
        else
        {
            for (int i = 0; i < itemDataList.Count; i++)
            {
                MyRoomItemData itemData = itemDataList[i];
                itemData.slotId = -1;
            }
            int idx = 0;
            for (int i = 0; i < itemDataList.Count; i++)
            {
                MyRoomItemData itemData = itemDataList[i];
                if (itemData.category == categoryType.ToString())
                {
                    itemData.slotId = idx;
                    slotList[idx].gameObject.SetActive(true);
                    slotList[idx].itemId = itemData.itemId;
                    //slotList[idx].itemId = itemData.invenItem.itemId;
                    idx++;
                }
            }
        }
        SortItem();
        scrollbar_Horizontal.value = 0;
    }

    #endregion



    #region 초기화 (슬롯 / 아이템)
    /// <summary>
    /// 슬롯 초기화 : 슬롯셋팅, 빈인벤아이템셋팅
    /// </summary>
    private void InitSlot()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            //슬롯오브젝트 추가
            Slot slot = Instantiate(inventorySlot, go_Content.transform).GetComponent<Slot>();
            slot.slotId = i;
            slot.itemId = -1;
            slotList.Add(slot);
        }
    }

    /// <summary>
    /// 아이템 초기화 : 인벤에 있는 아이템들 셋팅
    /// </summary>
    private void InitItem()
    {
        for (int i = 0; i < LocalPlayerData.InteriorItemInvens.Count; i++)
        {
            InteriorItemInvens invenItem = LocalPlayerData.InteriorItemInvens[i];
            PlusItem(invenItem);
        }
    }

    /// <summary>
    /// 인벤에 아이템 추가 : 없으면 인벤아이템추가, 있으면 +1
    /// </summary>
    /// <param name="invenItem"></param>
    public void PlusItem(InteriorItemInvens invenItem)
    {
        MyRoomItemData findItemData = itemDataList.FirstOrDefault(x => x.itemId == invenItem.itemId);

        if (findItemData == null)
        {
            CreateItem(invenItem);
        }
        else
        {
            findItemData.AddToList(invenItem.num);
        }
    }

    #endregion

    #region 아이템 생성 / 추가
    /// <summary>
    /// 아이템 생성 / 추가
    /// </summary>
    /// <param name="itemData"></param>
    public void OnPlusInvenItem(int itemId, int num)
    {
        SortItem();
        MyRoomItemData findItemData = itemDataList.FirstOrDefault(x => x.itemId == itemId);

        if (findItemData == null)
        {
            //해당아이템이 없다면 -> 새로추가
            CreateItem(new InteriorItemInvens(itemId, num));

            //임시예외처리 : 1개 다시 추가되었을때 탭 갱신하여 아이템슬롯 추가용
            OnValueChanged_Tab(categoryType);
        }
        else
        {
            //만약 해당아이템이 있다면 -> 수량추가
            findItemData.AddToList(num);
        }
    }

    /// <summary>
    /// 아이템 생성
    /// </summary>
    /// <param name="invenItem">아이템종류</param>
    /// <param name="slotId">어떤슬롯에 추가될 것인지</param>
    private void CreateItem(InteriorItemInvens interiorItemInvens)
    {
        //기존아이템 있는지?
        int slotId = slotList.FindIndex(x => x.itemId == interiorItemInvens.itemId);

        //없으면 빈 슬롯 찾기
        if (slotId == -1)
        {
            slotId = slotList.FindIndex(x => x.itemId == -1);
        }

        Slot slot = slotList[slotId];
        slot.itemId = interiorItemInvens.itemId;
        slotList[slotId] = slot;

        MyRoomItemData itemData = Instantiate(inventoryItem).GetComponent<MyRoomItemData>();
        itemData.SetItemData(slotId, interiorItemInvens);
        itemDataList.Add(itemData);
    }
    #endregion



    #region 아이템 정렬
    /// <summary>
    /// 아이템 정렬
    /// </summary>
    public void SortItem()
    {
        for (int i = 0; i < itemDataList.Count; i++)
        {
            SortRecursion(i);
        }
    }

    /// <summary>
    /// 아이템 정렬 재귀함수
    /// </summary>
    /// <param name="idx"></param>
    private void SortRecursion(int idx)
    {
        MyRoomItemData itemData = itemDataList[idx];
        if (itemData.slotId < 1)
        {
            return;
        }
        Slot prevSlot = slotList[itemData.slotId - 1];
        Slot curSlot = slotList[itemData.slotId];
        if (prevSlot != null && prevSlot.itemId == -1) //이전꺼가 아이템이 없으면
        {
            curSlot.itemId = -1; //현재꺼에 아이템 제거하고
            itemData.slotId--; //슬롯 한칸 앞으로
        }
    }
    #endregion



    #region 아이템 제거
    /// <summary>
    /// 인벤토리에서 아이템 제거
    /// </summary>
    /// <param name="itemData"></param>
    public void RemoveItem(MyRoomItemData itemData)
    {
        int lastExistSlotId = itemDataList.LastOrDefault(x=>x.slotId > -1).slotId;
        slotList[lastExistSlotId].gameObject.SetActive(false);
        slotList[itemData.slotId].itemId = -1;
        itemDataList.Remove(itemData);
        Destroy(itemData.gameObject);
        SortItem();
    }

    /// <summary>
    /// 이벤트_인벤토리에서 아이템 감소 -> leftStack이 0이 되면 제거
    /// </summary>
    /// <param name="itemId"></param>
    public void OnInitRoomItem(int itemId, int num)
    {
        MyRoomItemData itemData = itemDataList.FirstOrDefault(x => x.itemId == itemId);
        try
        {
            itemData.RemoveToList(num);
        }
        catch (System.Exception e)
        {
            Debug.LogError("error: " + e.Message);
            throw;
        }
        
    }
    #endregion



    #region 기능

    /// <summary>
    /// 아이템 내림차순 성렬
    /// </summary>
    public void SetItemDataList()
    {
        itemDataList = itemDataList.OrderBy(x => x.slotId).ToList();
    }

    /// <summary>
    /// 인벤락시 버튼 안눌리게..
    /// </summary>
    /// <param name="isInvenLock"></param>
    public void SetInvenLock(eINVENLOCK iNVENLOCK) => Co_SetInvenLock(iNVENLOCK).RunCoroutine();
    private IEnumerator<float> Co_SetInvenLock(eINVENLOCK iNVENLOCK)
    {
        //위/아래로 이동
        RectTransform rectTr = GetComponent<RectTransform>();

        //알파값셋팅
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        float oriPivot = 0f;
        float targetPivot = 0f;
        float oriAlpha = 0f;
        float targetAlpha = 0f;

        //버튼 EaseingFunction 셋업
        EasingFunction.Ease ease = default;

        switch (iNVENLOCK)
        {
            case eINVENLOCK.LOCK:
                targetPivot = 1f;
                oriAlpha = 1f;
                canvasGroup.blocksRaycasts = false;
                ease = EasingFunction.Ease.EaseInBack;
                break;
            case eINVENLOCK.UNLOCK:
                oriPivot = 1f;
                targetAlpha = 1f;
                canvasGroup.blocksRaycasts = true;
                ease = EasingFunction.Ease.EaseOutBack;
                break;
        }

        EasingFunction.Function function = EasingFunction.GetEasingFunction(ease);


        float curTime = 0f;
        while (curTime < 1f)
        {
            curTime += Timing.DeltaTime / invenLockTime;
            rectTr.pivot = new Vector2(0.5f, function(oriPivot, targetPivot, curTime));
            canvasGroup.alpha = Mathf.Lerp(oriAlpha, targetAlpha, curTime);
            yield return Timing.WaitForOneFrame;
        }

        rectTr.pivot = new Vector2(0.5f, targetPivot);
        canvasGroup.alpha = targetAlpha;
    }


    #endregion
}
