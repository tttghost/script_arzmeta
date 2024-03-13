using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using UnityEngine.Events;

public class Popup_TermsOfService : PopupBase
{
    #region 변수
    private Button btn_Confirm;
    private CanvasGroup cg_Confirm;

    private Toggle tog_AgreeAll;
    private Toggle tog_Age;
    private Toggle tog_Service;
    private Toggle tog_Privacy;

    private bool isChildToggle = false; //자식토글이 눌렸는지 여부
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Toggle
        tog_AgreeAll = GetUI_Toggle("tog_AgreeAll", (b) => OnValueChange_togAgreeAll(b));
        tog_Age = GetUI_Toggle("tog_Age", (b) => OnValueChange());
        tog_Service = GetUI_Toggle("tog_Service", (b) => OnValueChange());
        tog_Privacy = GetUI_Toggle("tog_Privacy", (b) => OnValueChange());
        #endregion

        #region Button
        btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnConfirm);
        if(btn_Confirm != null)
        {
            cg_Confirm = btn_Confirm.GetComponent<CanvasGroup>();
        }

        GetUI_Button("btn_Cancel", OnCancel);
        GetUI_Button("btn_showterms", OnClick_ShowTermOfService);
        GetUI_Button("btn_showagree", OnClick_ShowAgreement);
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("001"));
        GetUI_TxtmpMasterLocalizing("txtmp_Cancel", new MasterLocalData("002"));
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("2001"));
        GetUI_TxtmpMasterLocalizing("txtmp_desc01", new MasterLocalData("2002"));
        GetUI_TxtmpMasterLocalizing("txtmp_desc02", new MasterLocalData("2003"));
        GetUI_TxtmpMasterLocalizing("txtmp_desc03", new MasterLocalData("2004"));
        GetUI_TxtmpMasterLocalizing("txtmp_desc04", new MasterLocalData("2005"));
        GetUI_TxtmpMasterLocalizing("txtmp_showterms", new MasterLocalData("2006"));
        GetUI_TxtmpMasterLocalizing("txtmp_showagree", new MasterLocalData("2006"));
        #endregion

        BackAction_Custom = () => { SceneLogic.instance.OnClick_Back(); };
    }

    #region 초기화
    protected override void OnEnable()
    {
        if (tog_AgreeAll != null)
        {
            tog_AgreeAll.isOn = false;
        }
        if (tog_Age != null)
        {
            tog_Age.isOn = false;
        }
        if (tog_Service != null)
        {
            tog_Service.isOn = false;
        }
        if (tog_Privacy != null)
        {
            tog_Privacy.isOn = false;
        }

        if (cg_Confirm != null)
        {
            cg_Confirm.interactable = false;
        }
    }
    #endregion

    #region TermsOfService
    private void OnValueChange()
    {
        bool b = tog_Age.isOn && tog_Service.isOn && tog_Privacy.isOn;
        cg_Confirm.interactable = b;

        isChildToggle = true;
        tog_AgreeAll.isOn = b;
        isChildToggle = false;
    }

    private void OnValueChange_togAgreeAll(bool isOn)
    {
        if (!isChildToggle)
        {
            tog_Age.isOn = tog_Service.isOn = tog_Privacy.isOn = isOn;
        }
    }

    /// <summary>
    /// 이용약관
    /// </summary>
    private void OnClick_ShowTermOfService()
    {
        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, "https://arzmeta.net/terms"),
            new WebSetting(isPreNextBtnActive: false)));
    }

    /// <summary>
    /// 개인정보 수집 이용 동의서
    /// </summary>
    private void OnClick_ShowAgreement()
    {
        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, "https://arzmeta.net/privacy-consent"),
            new WebSetting(isPreNextBtnActive: false)));
    }

    protected override void OnConfirm()
    {
        base.OnConfirm();

        LocalPlayerData.Method.IsTos = 1;
    }

    protected override void OnCancel()
    {
        base.OnCancel();

        SceneLogic.instance.isUILock = false;
        PushPopup<Popup_Basic>()
                     .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("3012")))
                     .ChainPopupAction(new PopupAction(() =>
                    {
#if UNITY_EDITOR
                         UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
#else
		                    Application.Quit();
#endif
                     },
                     () =>
                     {
                         SceneLogic.instance.isUILock = false;
                         PushPopup<Popup_TermsOfService>();
                     }));
    }
    #endregion
}
