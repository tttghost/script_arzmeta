using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup_PayArowanaToken : PopupBase
{
    #region 변수
    private TMP_Text txtmp_PurchaseList;
    private TMP_Text txtmp_ArowanaCoin;
    private TMP_Text txtmp_Money;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Confirm", OnConfirm); // 투 버튼 확인
        GetUI_Button("btn_Cancel", OnCancel); // 투 버튼 취소
        #endregion

        #region TMP_Text
        txtmp_PurchaseList = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PurchaseList));
        txtmp_ArowanaCoin = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ArowanaCoin));
        txtmp_Money = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Money));

        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("001"));
        GetUI_TxtmpMasterLocalizing("txtmp_Cancel", new MasterLocalData("002"));
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("30021"));
        #endregion
    }

    #region PayArowanaToken
    public void SetData(string itemName, string size, string color, int totalQuantity, int arowanaCoin, int money)
    {
        if (totalQuantity < 2)
        {
            Util.SetMasterLocalizing(txtmp_PurchaseList, $"{Util.GetMasterLocalizing(itemName)} ( {size} / {color} )");
        }
        else
        {
            Util.SetMasterLocalizing(txtmp_PurchaseList, new MasterLocalData("30022", Util.GetMasterLocalizing(itemName), size, color, (totalQuantity - 1).ToString()));
        }
        Util.SetMasterLocalizing(txtmp_ArowanaCoin, arowanaCoin.ToString("N0") + " ARW");
        Util.SetMasterLocalizing(txtmp_Money, money.ToString("N0") + " KRW");
    }

    protected override void OnConfirm()
    {
        base.OnConfirm();

        SceneLogic.instance.isUILock = false;
        SceneLogic.instance.PushPopup<Popup_CompletePayment>();
    }

    protected override void OnCancel()
    {
        base.OnCancel();

        if (StoreManager.Instance.ConveyorBeltController.Switch != null &&
            StoreManager.Instance.ConveyorBeltController.Switch.interacting)
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PopPanel();

            SceneLogic.instance.GetPanel<Panel_ConveyorBelt>().CloseUI();
            StoreManager.Instance.AvatarChangeController.InitOutfit();
            StoreManager.Instance.PurchaseController.ResetBasket();
        }
        StoreManager.Instance.ConveyorBeltController.complete = true;
    }
    #endregion
}
