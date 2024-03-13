using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_AddFriend : PopupBase
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("friend_add"));
        #endregion

        #region Button
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        ChangeView(Cons.View_SearchFriend);
    }
    #endregion
}
