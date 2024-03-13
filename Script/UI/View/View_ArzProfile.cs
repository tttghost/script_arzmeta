using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using UnityEngine.UI;
using TMPro;

public class View_ArzProfile : UIBase
{
    #region 변수
    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_StateMessage;
    private TMP_Text txtmp_MemberCode;
    private Image img_Profile;

    private Member data;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_MemberCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MemberCode));
        txtmp_StateMessage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StateMessage));

        GetUI_TxtmpMasterLocalizing("txtmp_Residence", new MasterLocalData("3013"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCodeTitle", new MasterLocalData("014"));
        #endregion

        #region Button
        GetUI_Button("btn_CopyButton", () =>
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData(Util.GetMasterLocalizing("014") + $" {data.memberCode} " + Util.GetMasterLocalizing("009"))));
            Util.CopyToClipboard(data.memberCode);
        });
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        #endregion
    }

    #region Arzmeta
    /// <summary>
    /// 데이터 세팅
    /// </summary>
    public void SetData(Member data)
    {
        this.data = data;

        SetContent();
    }

    /// <summary>
    /// 콘텐츠에 데이터 넣기
    /// </summary>
    private void SetContent()
    {
        if (txtmp_Nickname != null)
        {
            Util.SetMasterLocalizing(txtmp_Nickname, data.nickname);
        }

        if (txtmp_MemberCode != null)
        {
            Util.SetMasterLocalizing(txtmp_MemberCode, data.memberCode);
        }

        if (txtmp_StateMessage != null)
        {
            if (string.IsNullOrEmpty(data.stateMessage))
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, new MasterLocalData("3066", data.nickname));
            }
            else
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, data.stateMessage);
            }
        }

        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => img_Profile.sprite = sprite);
        }
    }
    #endregion
}
