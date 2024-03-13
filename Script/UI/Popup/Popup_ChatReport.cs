using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using FrameWork.Network;
using CryptoWebRequestSample;
using Lean.Touch;
using Unity.Linq;
using System.Linq;
using Lean.Common;
using System.Net;

public class Popup_ChatReport : PopupBase
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("6021"));
        #endregion

        #region Button
        GetUI_Button("btn_Exit", () => SceneLogic.instance.PopPopup());
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        ChangeView(Cons.View_SearchFriend);
    }
    #endregion
}
