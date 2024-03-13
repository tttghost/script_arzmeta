using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Popup_AccountManagement : PopupBase
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_IDInfo", LocalPlayerData.Email);
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("setting_account_account"));
        GetUI_TxtmpMasterLocalizing("txtmp_IDInfoTitle", new MasterLocalData("setting_account_id"));
        GetUI_TxtmpMasterLocalizing("txtmp_ChangePasswordTitle", new MasterLocalData("9102"));
        #endregion

        #region Button
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        GetUI_Button("btn_ChangePassword", () =>
        {
            SceneLogic.instance.PopPopup();
            SceneLogic.instance.isUILock = false;
            PushPopup<Popup_ChangePassword>();
        });
        #endregion
    }
}
