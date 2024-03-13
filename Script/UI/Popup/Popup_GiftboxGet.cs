using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using TMPro;


public class Popup_GiftboxGet : PopupBase
{
    private Button btn_Confirm;
    private Image img_thumbnail;
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Count;
    private TMP_Text txtmp_Confirm;

    private GridView_Custom gridView;
    private List<View_Giftbox.GiftItemInfo> receivedItems = new List<View_Giftbox.GiftItemInfo>();
    private ItemGiftMailGetCell[] nonScrollItems;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnConfirm);
        img_thumbnail = GetUI_Img(nameof(img_thumbnail));
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("common_popup_get"));
        txtmp_Confirm = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Confirm), new MasterLocalData("common_ok"));
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));
        
        gridView = GetChildGObject("go_ScrollView").GetComponent<GridView_Custom>();
        if (gridView != null)
        {
            gridView.OnCellClicked(index => { SelectCell(index); });
        }
        nonScrollItems = GetChildGObject("go_NonScrollView").GetComponentsInChildren<ItemGiftMailGetCell>(true);
    }

    private void OnDestroy()
    {
        receivedItems.Clear();
        receivedItems = null;
    }

    public void SetData(List<View_Giftbox.GiftItemInfo> items)
    {
        receivedItems = items;

        GenerateCells();
    }

    void SelectCell(int idx)
    {

    }

    void GenerateCells()
    {
        gridView.gameObject.SetActive(false);
        for (int i = 0; i < nonScrollItems.Length; i++)
            nonScrollItems[i].gameObject.SetActive(false);

        int dataCount = receivedItems.Count;

        // 아이템 개수가 5개이하 일경우 센터 고정해야 하기에 스크롤 미사용 처리
        if (dataCount > 5)
        {
            gridView.gameObject.SetActive(true);
            var items = Enumerable.Range(0, dataCount)
                .Select(i => new ItemGiftMailGetItemData() { id = receivedItems[i].giftItemId, count = receivedItems[i].count })
                .ToArray();

            gridView.UpdateContents(items);
            gridView.JumpTo(0);
        }
        else
        {
            for (int i = 0; i < nonScrollItems.Length; i++)
            {
                nonScrollItems[i].gameObject.SetActive(i < dataCount);
                if (i < dataCount)
                {
                    nonScrollItems[i].UpdateContent(new ItemGiftMailGetItemData() { id = receivedItems[i].giftItemId, count = receivedItems[i].count });
                }
            }
        }
    }
}