using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup_MedicinePassword : Popup_Basic
{
    public RoomInfoRes data = new RoomInfoRes();
    private TMP_InputField input_Password = null;
    


    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        input_Password = GetUI_TMPInputField(nameof(input_Password));

        Util.SetMasterLocalizing(txtmp_Desc, new MasterLocalData("1175"));
        btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClickConfirmCustom);

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        input_Password.text = string.Empty;
    }

    private void OnClickConfirmCustom()
    {
        if (input_Password.text == string.Empty)
        {
            input_Password.text = " ";
            return;
        }
        else
        {
            input_Password.text = input_Password.text.ToUpper();
        }
    }
}
