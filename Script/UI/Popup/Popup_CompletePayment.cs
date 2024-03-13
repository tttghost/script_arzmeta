using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Popup_CompletePayment : PopupBase
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Confirm", OnConfirm); // 원버튼 확인
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("008"));
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("30023"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sub", new MasterLocalData("30024"));
        GetUI_TxtmpMasterLocalizing("txtmp_State_0", new MasterLocalData("30035"));
        GetUI_TxtmpMasterLocalizing("txtmp_State_1", new MasterLocalData("30036"));
        GetUI_TxtmpMasterLocalizing("txtmp_State_2", new MasterLocalData("30037"));
        GetUI_TxtmpMasterLocalizing("txtmp_State_3", new MasterLocalData("30038"));
        GetUI_TxtmpMasterLocalizing("txtmp_State_4", new MasterLocalData("30039"));
        #endregion
    }


    protected override void OnConfirm()
    {
        base.OnConfirm();

        if (SceneLogic.instance._stackPanels.Count > 1)
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PopPanel();
        }
        StoreManager.Instance.ConveyorBeltController.complete = true;
    }
}
