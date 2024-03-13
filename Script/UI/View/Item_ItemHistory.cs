using FrameWork.UI;
using Hypertonic.GridPlacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_ItemHistory : UIBase
{
    private ItemHistory     itemHistory;

    private GameObject      go_PaidFree;
    private GameObject      go_Paid;
    private GameObject      go_Free;
    private TMP_Text        txtmp_PaidFree;

    private Image           img_ItemHistory;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        Button btn_ItemHistory = GetUI_Button(nameof(btn_ItemHistory));
        img_ItemHistory = btn_ItemHistory.image;

        GetUI_Button("btn_ItemMinus", OnClick_ItemMinus);

        go_PaidFree = GetChildGObject(nameof(go_PaidFree));
        go_Paid = GetChildGObject(nameof(go_Paid));
        go_Free = GetChildGObject(nameof(go_Free));

        txtmp_PaidFree = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PaidFree));
    }


    public void SetData(ItemHistory itemHistory)
    {
        this.itemHistory = itemHistory;

        img_ItemHistory.sprite = Util.GetItemIconSprite(itemHistory.item.id);

        //txtmp_PaidFree.text = historyItem.item.purchasePrice.ToString();

        //go_PaidFree.SetActive(false);
        //go_Paid.SetActive(false);
        //go_Free.SetActive(false);

        //switch (historyItem.item.purchaseType)
        //{
        //    case 0:
        //        break;
        //    case 1: //무료
        //        go_PaidFree.SetActive(true);
        //        go_Free.SetActive(true);
        //        break;
        //    case 2: //유료
        //        go_PaidFree.SetActive(true);
        //        go_Paid.SetActive(true);
        //        break;
        //    default:
        //        break;
        //}

        MyRoomManager.Instance.gridSystem.handlerMinusHistoryItem += OnMinusHistoryItem;
    }

    /// <summary>
    /// 설치된아이템 취소버튼
    /// 인벤아이템 +
    /// 그리드아이템-, 히스토리아이템-
    /// </summary>
    public void OnClick_ItemMinus()
    {
        MyRoomManager.Instance.gridSystem.handlerPlusInvenItem?.Invoke(itemHistory.item.id, itemHistory.num);   //인벤아이템 +

        MyRoomManager.Instance.gridSystem.handlerMinusHistoryItem?.Invoke(itemHistory.num);                     //그리드아이템-, 히스토리아이템-

        //MyRoomCustomManager.Instance.panel_MyRoomControl.SaveLoadBtnInteractable();                           //세이브로드버튼 인터렉터블 업데이트
    }


    /// <summary>
    /// 그리드아이템-, 히스토리아이템-
    /// </summary>
    /// <param name="num"></param>
    private void OnMinusHistoryItem(int num)
    {
        if (itemHistory.num != num)
        {
            return;
        }
        //재화갱신
        //MyRoomManager.Instance.SetMinusMoney(itemHistory.item);

        string gridId = Single.MasterData.GetLayerType(itemHistory.placementObj.name);
        MyRoomManager.Instance.gridSystem.SelectedGridManager(gridId); //그리드 체인지

        //그리드아이템 -(근처코드)
        GridManagerAccessor.GridManager.DeleteObject(itemHistory.placementObj);

        //히스토리아이템 이벤트 -
        MyRoomManager.Instance.gridSystem.handlerMinusHistoryItem -= OnMinusHistoryItem;

        //히스토리아이템 리스트 -
        MyRoomManager.Instance.popup_ItemHistory.itemHistoryList.Remove(itemHistory);

        //히스토리아이템 -
        Destroy(gameObject);
    }

}
