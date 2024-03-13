using FrameWork.UI;
using Office;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_OfficeGradeUpgrade : PopupBase
{
    private TMP_Text txtmp_Recommand;
    private TMP_Text txtmp_RecommandRequest;
    private TMP_Text txtmp_MyGrade;
    private TMP_Text txtmp_ConfirmOffice;
    private TMP_Text txtmp_CancelOffice;

    private Button btn_ConfirmOffice;
    private Button btn_CancelOffice;

    //private eOfficeGradeType eOfficeGradeType;

    //private Popup_OfficeRoomCreate popup_OfficeRoomCreate;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_Recommand = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Recommand), new MasterLocalData("office_grade_limit_info_common"));
        txtmp_RecommandRequest = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RecommandRequest), new MasterLocalData("office_confirm_upgrade"));
        txtmp_MyGrade = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MyGrade));
        txtmp_ConfirmOffice = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ConfirmOffice), new MasterLocalData("office_upgrade"));
        txtmp_CancelOffice = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CancelOffice), new MasterLocalData("common_cancel"));

        btn_ConfirmOffice = GetUI_Button(nameof(btn_ConfirmOffice), OnClick_ConfirmOffice);
        btn_CancelOffice = GetUI_Button(nameof(btn_CancelOffice), OnClick_CancelOffice);
    }

    protected override void Start()
    {
        base.Start();
        //popup_OfficeRoomCreate = SceneLogic.instance.GetPopup<Popup_OfficeRoomCreate>();
    }
    private void OnClick_ConfirmOffice()
    {
        //LocalPlayerData.OfficeGradeType = (int)eOfficeGradeType;
        //popup_OfficeRoomCreate.eOfficeGradeType = eOfficeGradeType;
        //popup_OfficeRoomCreate.RefreshOfficeGrade();
        
        OnClick_CancelOffice();

        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("40000")));

    }
    private void OnClick_CancelOffice()
    {
        SceneLogic.instance.Back();
    }
    public void SetData(eOfficeGradeType eOfficeGradeType)
    {
        //this.eOfficeGradeType = eOfficeGradeType;
        string gradeName = Single.MasterData.dataOfficeGradeType.GetData(LocalPlayerData.OfficeGradeType).name;
        string localGradeName = Util.GetMasterLocalizing(gradeName);
        //txtmp_Recommand.text = localGradeName;
        //txtmp_MyGrade.text = eOfficeGradeType.ToString();
        Util.SetMasterLocalizing(txtmp_MyGrade, new MasterLocalData("office_mygrade", localGradeName));
    }
}
