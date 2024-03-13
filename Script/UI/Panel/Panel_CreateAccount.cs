using FrameWork.Network;
using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using UnityEngine.EventSystems;
using System.Linq;
using MEC;
using GamePotUnity;

public class Panel_CreateAccount : PanelBase
{
    #region 변수
    public enum BTN_STATE
    {
        Email_Nomal,
        Email_Success,
        Auth_Nomal,
        Auth_Success,
    }

    public enum CHECK_STATE
    {
        Email,
        Auth,
        Auth_Success,
        Auth_Fail,
        Auth_Exprie,
        Password,
        PasswordCheck,
    }

    private Sprite oriSprite;

    protected TMP_InputField input_EmailCheck;
    protected TMP_InputField input_EmailAuth;
    protected TMP_InputField input_Password;
    protected TMP_InputField input_PasswordCheck;
    protected Button btn_EmailCheck;
    protected Button btn_EmailAuth;
    protected Button btn_SignUp;
    protected TMP_Text txtmp_Timer;
    protected TMP_Text txtmp_WarningEmailCheck;
    protected TMP_Text txtmp_WarningEmailAuth;
    protected TMP_Text txtmp_WarningPassword;
    protected TMP_Text txtmp_WarningPasswordCheck;
    protected Image img_EmailCheck;
    protected Image img_EmailAuth;
    protected Image img_SignUpIcon;
    protected CanvasGroup cg_EmailAuth;

    protected bool isCurEmailCheck;
    protected bool isCurEmailAuth;
    protected bool isCurPassword;
    protected bool isCurPasswordCheck;

    protected Color oriIconColor;
    protected Color disabledColor = Cons.Color_WhiteGray;

    protected EventTrigger showPassword;
    protected EventTrigger showPasswordCheck;

    protected string currentEmail;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_WarningEmailCheck = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmailCheck));
        txtmp_WarningEmailAuth = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmailAuth));
        txtmp_WarningPassword = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningPassword));
        txtmp_WarningPasswordCheck = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningPasswordCheck));
        txtmp_Timer = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Timer));

        GetUI_TxtmpMasterLocalizing("txtmp_SignUp", new MasterLocalData("3042"));
        GetUI_TxtmpMasterLocalizing("txtmp_Send", new MasterLocalData("004"));
        GetUI_TxtmpMasterLocalizing("txtmp_EmailAuth", new MasterLocalData("005"));
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3042"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_EmailAuth", new MasterLocalData("007"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Password", new MasterLocalData("3033"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_PasswordCheck", new MasterLocalData("3033"));
        GetUI_TxtmpMasterLocalizing("txtmp_Email", new MasterLocalData("012"));
        GetUI_TxtmpMasterLocalizing("txtmp_EmailAuthTitle", new MasterLocalData("007"));
        GetUI_TxtmpMasterLocalizing("txtmp_Password", new MasterLocalData("006"));
        GetUI_TxtmpMasterLocalizing("txtmp_PasswordCheck", new MasterLocalData("3034"));
        #endregion

        #region Button
        btn_EmailCheck = GetUI_Button(nameof(btn_EmailCheck), OnClick_EmailCheck);
        if (btn_EmailCheck != null)
        {
            img_EmailCheck = btn_EmailCheck.GetComponent<Image>();
            oriSprite = img_EmailCheck.sprite;
        }
        btn_EmailAuth = GetUI_Button(nameof(btn_EmailAuth), OnClick_EmailAuth);
        if (btn_EmailAuth != null)
        {
            img_EmailAuth = btn_EmailAuth.GetComponent<Image>();
        }
        btn_SignUp = GetUI_Button(nameof(btn_SignUp), OnClick_JoinConfirm);

        GetUI_Button("btn_Back", OnClick_Back);
        #endregion

        #region InputField
        input_EmailCheck = GetUI_TMPInputField(nameof(input_EmailCheck), OnValueChanged_CheckEmail);
        input_EmailAuth = GetUI_TMPInputField(nameof(input_EmailAuth), OnValueChanged_AuthEmail);
        input_Password = GetUI_TMPInputField(nameof(input_Password), OnValueChanged_InputPassWord);
        input_PasswordCheck = GetUI_TMPInputField(nameof(input_PasswordCheck), OnValueChanged_InputCheck);
        #endregion

        #region Image
        img_SignUpIcon = GetUI_Img(nameof(img_SignUpIcon));
        if (img_SignUpIcon != null)
        {
            oriIconColor = img_SignUpIcon.color;
        }
        #endregion

        #region etc
        showPassword = GetUI_Button("btn_ShowPassword").GetComponent<EventTrigger>();
        showPasswordCheck = GetUI_Button("btn_ShowPasswordCheck").GetComponent<EventTrigger>();

        cg_EmailAuth = GetChildGObject("go_EmailAuth").GetComponent<CanvasGroup>();
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        isStarted = true;

        Util.AddTriggerEntry(showPassword, input_Password);
        Util.AddTriggerEntry(showPasswordCheck, input_PasswordCheck);
    }

    protected override void OnEnable()
    {
        isCurEmailCheck = isCurEmailAuth = isCurPassword = isCurPasswordCheck = false;

        SetActiveCGEmailAuth(false);

        if (btn_EmailCheck != null) btn_EmailCheck.interactable = false;
        if (btn_SignUp != null) btn_SignUp.interactable = false;

        if (input_EmailCheck != null) input_EmailCheck.Clear();
        if (input_Password != null) input_Password.Clear();
        if (input_PasswordCheck != null) input_PasswordCheck.Clear();

        if (img_EmailCheck != null) img_EmailCheck.sprite = oriSprite;
        if (img_EmailAuth != null) img_EmailAuth.sprite = oriSprite;
        if (img_SignUpIcon != null) img_SignUpIcon.color = disabledColor;

        if (txtmp_Timer != null) Util.SetMasterLocalizing(txtmp_Timer, string.Empty);
        SetActive_Warning(CHECK_STATE.Email);
        SetActive_Warning(CHECK_STATE.Password);
        SetActive_Warning(CHECK_STATE.PasswordCheck);
    }
    #endregion

    #region 이메일
    /// <summary>
    /// 이메일 실시간 조건 확인
    /// </summary>
    /// <param name="email"></param>
    private void OnValueChanged_CheckEmail(string email)
    {
        InitTimer();

        isCurEmailCheck = false;
        btn_EmailCheck.interactable = Util.EmailRestriction(email);

        ChangeBtnSprite(BTN_STATE.Email_Nomal);
        SetActive_Warning(CHECK_STATE.Email, email);
    }

    /// <summary>
    /// 이메일로 보안코드 발송
    /// </summary>
    protected virtual void OnClick_EmailCheck()
    {
        InitTimer();

        var email = input_EmailCheck.text;
        if (!Regex.IsMatch(email, "@"))
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("3040")));
            return;
        }

        Single.Web.account.AuthEmail(email, (res) =>
        {
            isCurEmailCheck = true;
            isCurEmailAuth = false;

            currentEmail = email;

            ChangeBtnSprite(BTN_STATE.Email_Success);
            SetActiveCGEmailAuth(true);

            Util.RunCoroutine(Co_Timer(res.remainTime), "AuthTimer");

            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.CHECK, BTN_TYPE.Confirm, null, new MasterLocalData("3022")));

            ActiveConfirmInteract();
        });
    }

    #region Timer

    private bool isInitTimer = false;
    protected void InitTimer()
    {
        ChangeBtnSprite(BTN_STATE.Auth_Nomal);
        SetActiveCGEmailAuth(false);

        if (isInitTimer) return;
        isInitTimer = true;

        Util.KillCoroutine("AuthTimer");
        if (txtmp_Timer != null) Util.SetMasterLocalizing(txtmp_Timer, string.Empty);
    }

    protected IEnumerator<float> Co_Timer(int time)
    {
        isInitTimer = false;

        float curTime = time;
        int min, sec;
        while (curTime > 0)
        {
            if (isCurEmailAuth) yield break;

            if (isUpdate)
            {
                isUpdate = false;
                var backSec = (DateTime.Now - pauseDateTime).TotalSeconds;
                curTime -= (float)backSec;
            }
            else
            {
                curTime -= Time.deltaTime;
            }

            min = (int)(curTime / 60);
            sec = (int)(curTime % 60);
            Util.SetMasterLocalizing(txtmp_Timer, string.Format("{0:D1}:{1:D2}", min, sec));
            yield return Timing.WaitForOneFrame;
        }

        if (!isCurEmailAuth)
        {
            isCurEmailCheck = false;
            btn_EmailAuth.interactable = false;

            SetActiveCGEmailAuth(false);
            SetActive_Warning(CHECK_STATE.Auth_Exprie);
        }
    }

    private bool isStarted = false;
    private bool isUpdate = false;
    private DateTime pauseDateTime;

    private void OnApplicationPause(bool isPause)
    {
        if (isStarted)
        {
            if (isPause)
                pauseDateTime = DateTime.Now;
            else
                isUpdate = true;
        }
    }
    #endregion

    /// <summary>
    /// 인증코드 실시간 조건 확인
    /// </summary>
    /// <param name="code"></param>
    private void OnValueChanged_AuthEmail(string code)
    {
        input_EmailAuth.text = code = Util.RegularReplaceOnlyNumber(code);
        isCurEmailAuth = false;
        btn_EmailAuth.interactable = code.Length == 0 || Util.AuthCodeRestriction(code);

        ChangeBtnSprite(BTN_STATE.Auth_Nomal);
        SetActive_Warning(CHECK_STATE.Auth, code);
    }

    /// <summary>
    /// 인증 번호 맞는지 확인
    /// </summary>
    private void OnClick_EmailAuth()
    {
        var emailAuthCode = input_EmailAuth.text;
        Single.Web.account.ConfirmEmail(currentEmail, int.Parse(emailAuthCode), (res) =>
        {
            InitTimer();

            input_EmailAuth.text = emailAuthCode;
            isCurEmailAuth = true;

            ChangeBtnSprite(BTN_STATE.Auth_Success);
            SetActive_Warning(CHECK_STATE.Auth_Success);
            ActiveConfirmInteract();
        },
        (res) =>
        {
            input_EmailAuth.Clear();

            SetActive_Warning(CHECK_STATE.Auth_Fail);
            ActiveConfirmInteract();
        });
    }

    protected void SetActiveCGEmailAuth(bool b)
    {
        if (cg_EmailAuth != null)
        {
            cg_EmailAuth.interactable = b;
            if (!b)
            {
                if (input_EmailAuth != null) input_EmailAuth.Clear();
                SetActive_Warning(CHECK_STATE.Auth);
            }
        }
    }
    #endregion

    #region 패스워드
    /// <summary>
    /// 패스워드 설정
    /// </summary>
    /// <param name="password"></param>
    private void OnValueChanged_InputPassWord(string input)
    {
        input_Password.text = input = Util.RegularReplaceWithoutKorean(input);
        isCurPassword = Util.PasswordRestriction(input);

        OnValueChanged_InputCheck(input_PasswordCheck.text);
        SetActive_Warning(CHECK_STATE.Password, input);
        ActiveConfirmInteract();
    }

    /// <summary>
    /// 패스워드 재확인
    /// </summary>
    /// <param name="check"></param>
    protected void OnValueChanged_InputCheck(string input)
    {
        input_PasswordCheck.text = input = Util.RegularReplaceWithoutKorean(input);
        isCurPasswordCheck = Util.PasswordRestriction(input) && input == input_Password.text;

        SetActive_Warning(CHECK_STATE.PasswordCheck, input);
        ActiveConfirmInteract();
    }

    /// <summary>
    /// 작성 항목 여부에 따른 계정생성 비/활성화
    /// </summary>
    /// <returns></returns>
    protected void ActiveConfirmInteract()
    {
        bool b = isCurPassword && isCurPasswordCheck && isCurEmailCheck && isCurEmailAuth;
        btn_SignUp.interactable = b;
        img_SignUpIcon.color = b ? oriIconColor : disabledColor;
    }

    /// <summary>
    /// 회원가입 확인
    /// </summary>
    protected virtual void OnClick_JoinConfirm()
    {
        LocalPlayerData.ProviderType = (int)LOGIN_PROVIDER_TYPE.ARZMETA;
        Single.Web.account.CreateAccount(currentEmail, input_Password.text, (res) =>
        {
            DEBUG.LOG("WebManager " + GetType().Name + " 회원 가입이 완료되었습니다!", eColorManager.UI);

            LocalPlayerData.LoginToken = res.loginToken;
            Single.Web.account.LoginAuth((res) =>
            {
                LocalPlayerData.MemberID = res.memberId;
                Single.Web.account.AutoLogin();

                // 공지사항 사용을 위한 GAMEPOT Arzmeta 로그인
                GamePot.loginByThirdPartySDK(res.memberId);
            });
        });
    }

    protected virtual void OnClick_Back()
    {
        SceneLogic.instance.Back();
    }
    #endregion

    /// <summary>
    /// 경고 문구 변경
    /// </summary>
    /// <param name="eState"></param>
    /// <param name="input"></param>
    public void SetActive_Warning(CHECK_STATE eState, string input = null)
    {
        bool isEmpty = string.IsNullOrEmpty(input);

        TMP_Text targetTxtmp = null;
        string warningStr = null;
        switch (eState)
        {
            case CHECK_STATE.Email:
                if (txtmp_WarningEmailCheck != null) targetTxtmp = txtmp_WarningEmailCheck;
                if (!isEmpty) warningStr = Util.EmailRestriction(input) ? null : "3018";
                break;
            case CHECK_STATE.Auth:
            case CHECK_STATE.Auth_Success:
            case CHECK_STATE.Auth_Fail:
            case CHECK_STATE.Auth_Exprie:
                if (txtmp_WarningEmailAuth != null) targetTxtmp = txtmp_WarningEmailAuth;
                warningStr = eState switch
                {
                    CHECK_STATE.Auth_Success => "3074",
                    CHECK_STATE.Auth_Fail => "3032",
                    CHECK_STATE.Auth_Exprie => "3031",
                    _ => null,
                };
                break;
            case CHECK_STATE.Password:
                if (txtmp_WarningPassword != null) targetTxtmp = txtmp_WarningPassword;
                if (!isEmpty) warningStr = input.Length == 0 || Util.PasswordRestriction(input) ? null : "3004";
                break;
            case CHECK_STATE.PasswordCheck:
                if (txtmp_WarningPasswordCheck != null) targetTxtmp = txtmp_WarningPasswordCheck;
                if (!isEmpty) warningStr = input.Length == 0 ? null :
                    (isCurPasswordCheck ? "3035" :
                    (input != input_Password.text ? "3036" : "3004"));
                break;
            default: break;
        }

        Util.SetActive_Warning(targetTxtmp, warningStr);
    }

    /// <summary>
    /// 버튼 스프라이트 변경
    /// </summary>
    /// <param name="eState"></param>
    public void ChangeBtnSprite(BTN_STATE eState)
    {
        switch (eState)
        {
            case BTN_STATE.Email_Nomal:
            case BTN_STATE.Email_Success:
                if (img_EmailCheck != null)
                    img_EmailCheck.sprite = eState switch
                    {
                        BTN_STATE.Email_Nomal => oriSprite,
                        BTN_STATE.Email_Success => Util.GetBtnSelectSprite(),
                        _ => null,
                    };
                break;
            case BTN_STATE.Auth_Nomal:
            case BTN_STATE.Auth_Success:
                if (img_EmailAuth != null)
                    img_EmailAuth.sprite = eState switch
                    {
                        BTN_STATE.Auth_Nomal => oriSprite,
                        BTN_STATE.Auth_Success => Util.GetBtnSelectSprite(),
                        _ => null,
                    };
                break;

            default: break;
        }
    }
}
