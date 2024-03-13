using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Item_CommercePurchase : UIBase
{
    #region 변수
    private TMP_Text txtmp_CommercePurchase;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_CommercePurchase = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CommercePurchase));
        #endregion
    }

    #region
    public void SetData(string itemName, string size, string color, string quantity)
    {
        Util.SetMasterLocalizing(txtmp_CommercePurchase, new MasterLocalData("30009", Util.GetMasterLocalizing(itemName), size, color, quantity));

        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
    #endregion
}
