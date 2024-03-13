using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MEC;
using UnityEngine.Localization.Components;

public class Toast_Basic : ToastBase
{
    #region 변수
    private ToastData_Basic toastData;

    private TMP_Text txtmp_Title;
    #endregion

    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Title = GetUI_TxtmpMasterLocalizing("txtmp_Title");
        #endregion
    }
    #endregion

    protected override void SavaData(object data)
    {
        if (data is ToastData_Basic _data)
        {
            toastData = _data;
            curToastTime = toastData.time;
        }
    }

    protected override void SetContent()
    {
        if (txtmp_Title != null)
        {
            if (!string.IsNullOrEmpty(toastData.descText))
            {
                Util.SetMasterLocalizing(txtmp_Title, toastData.descText);
            }
            else if (toastData.descLocal != null)
            {
                Util.SetMasterLocalizing(txtmp_Title, toastData.descLocal);
            }
        }

        if (toastData.type == TOASTICON.Wrong)
        {
            SoundWorng();
        }
        else
        {
            SoundNomal();
        }
    }
}

#region ToastData
/// <summary>
/// 토스트 데이터
/// </summary>
public class ToastData_Basic
{
    public TOASTICON type;
    public MasterLocalData descLocal;
    public string descText;
    public float time;

    public ToastData_Basic(TOASTICON type, string descText, float time = 3f)
    {
        this.type = type;
        this.descText = descText;
        this.time = time;
    }
    public ToastData_Basic(TOASTICON type, MasterLocalData titleLocal, float time = 3f)
    {
        this.type = type;
        this.descLocal = titleLocal;
        this.time = time;
    }
}
#endregion
