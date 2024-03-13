using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.Network;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_FindPassword : PanelBase
{
    #region 변수
    private TMP_InputField input_Email;
    private TMP_Text txtmp_WarningEmail;
    private Image img_FindPasswordIcon;
    private Button btn_FindPassword;

    private Color oriIconColor;
    private Color disableBtnColor = Cons.Color_WhiteGray;
    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        btn_FindPassword = GetUI_Button(nameof(btn_FindPassword), OnClick_FindPassword);
        if (btn_FindPassword != null)
        {
            btn_FindPassword.interactable = false;
        }

        GetUI_Button("btn_Back", () => SceneLogic.instance.Back());
        #endregion

        #region InputField
        input_Email = GetUI_TMPInputField(nameof(input_Email), OnValueChanged_Email);
        #endregion

        #region TMP_Text
        txtmp_WarningEmail = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmail));

        GetUI_TxtmpMasterLocalizing("txtmp_SubTitle", new MasterLocalData("common_notice_passwordreset_01"));
        GetUI_TxtmpMasterLocalizing("txtmp_FindPassword", new MasterLocalData("004"));
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3024"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Email", new MasterLocalData("3016"));
        GetUI_TxtmpMasterLocalizing("txtmp_Email", new MasterLocalData("012"));
        #endregion

        #region Image
        img_FindPasswordIcon = GetUI_Img(nameof(img_FindPasswordIcon));
        if (img_FindPasswordIcon != null)
        {
            oriIconColor = img_FindPasswordIcon.color;
        }
        #endregion
    }

    #region 초기화

    protected override void OnEnable()
    {
        if (input_Email != null)
        {
            input_Email.Clear();
        }

        if (img_FindPasswordIcon != null)
        {
            img_FindPasswordIcon.color = disableBtnColor;
        }

        if (txtmp_WarningEmail != null)
        {
            Util.SetActive_Warning(txtmp_WarningEmail, "3027");
        }
    }
    #endregion

    #region 패스워드
    /// <summary>
    /// 이메일 제한
    /// </summary>
    /// <param name="email"></param>
    void OnValueChanged_Email(string email)
    {
        bool b = email.Length >= 1 && Util.EmailRestriction(email);

        img_FindPasswordIcon.color = b ? oriIconColor : disableBtnColor;
        btn_FindPassword.interactable = b;

        Util.SetActive_Warning(txtmp_WarningEmail, b || email.Length < 1 ? "3027" : "3018");
    }

    /// <summary>
    /// 이메일로 임시 비밀번호 발송
    /// </summary>
    void OnClick_FindPassword()
    {
        Single.Web.account.ResetPassword(input_Email.text, (res) =>
        {
            // 서버측 코드에서 try catch문에서 고의적 오류를 내지 못하므로 일단 오류 없이 리턴으로 에러 반환
            if (res.error != (int)WEBERROR.NET_E_SUCCESS)
            {
                PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("common_state_overcount_emailauth")));
                return;
            }

            string localData = Util.GetMasterLocalizing("common_notice_passwordreset_02") + "\n\n" + Util.GetMasterLocalizing("common_notice_passwordreset_03");
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.CHECK, BTN_TYPE.Confirm, null, new MasterLocalData(localData)))
                .ChainPopupAction(new PopupAction(() =>
                {
                    SceneLogic.instance.isUILock = false;
                    SceneLogic.instance.PopPanel();
                }));
        });
    }
    #endregion
}
