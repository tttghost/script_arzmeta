using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using UnityEngine.UI;
using TMPro;

public class View_BizCard_A : BizCardBase
{
    #region 변수
    private TMP_Text txtmp_Name;
    private TMP_Text txtmp_Job;
    private TMP_Text txtmp_PhoneNumber;
    private TMP_Text txtmp_Email;
    private Image img_Profile;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Name = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Name));
        txtmp_Job = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Job));
        txtmp_PhoneNumber = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PhoneNumber));
        txtmp_Email = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Email));
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        #endregion
    }

    #region 초기화
    protected override void SetContent()
    {
        if (txtmp_Name != null)
        {
            txtmp_Name.text = data.bizCard.name;
        }

        if (txtmp_Job != null)
        {
            txtmp_Job.text = data.bizCard.job;
        }

        if (txtmp_PhoneNumber != null)
        {
            txtmp_PhoneNumber.text = data.bizCard.phone;
        }

        if (txtmp_Email != null)
        {
            txtmp_Email.text = data.bizCard.email;
        }

        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => { img_Profile.sprite = sprite; });
        }
    }
    #endregion
}
