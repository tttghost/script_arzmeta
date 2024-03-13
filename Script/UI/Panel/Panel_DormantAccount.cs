using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using System;
using MEC;

public class Panel_DormantAccount : PanelBase
{
    #region 변수
    private Sprite oriSprite;

    private Image img_EmailCheck;
    private Image img_EmailAuth;
    private Image img_WakeAccountIcon;
    private TMP_InputField input_EmailCheck;
    private TMP_InputField input_EmailAuth;
    private Button btn_EmailCheck;
    private Button btn_EmailAuth;
    private Button btn_WakeAccount;
    private TMP_Text txtmp_WarningEmailCheck;
    private TMP_Text txtmp_WarningEmailAuth;
    private TMP_Text txtmp_Timer;

    private bool isCurEmailCheck;
    private bool isCurEmailAuth;

    private Color oriBtnIconColor;
    private Color disabledColor = Cons.Color_WhiteGray;

    private string currentEmail;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_WarningEmailCheck = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmailCheck));
        txtmp_WarningEmailAuth = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmailAuth));
        txtmp_Timer = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Timer));

        GetUI_TxtmpMasterLocalizing("txtmp_Send", new MasterLocalData("004"));
        GetUI_TxtmpMasterLocalizing("txtmp_EmailAuth", new MasterLocalData("005"));
        GetUI_TxtmpMasterLocalizing("txtmp_WakeAccount", new MasterLocalData("3052"));
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3051"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_EmailAuth", new MasterLocalData("3030"));
        GetUI_TxtmpMasterLocalizing("txtmp_Email", new MasterLocalData("012"));
        GetUI_TxtmpMasterLocalizing("txtmp_EmailAuthTitle", new MasterLocalData("007"));
        #endregion

        #region Button
        btn_EmailCheck = GetUI_Button(nameof(btn_EmailCheck), OnClick_EmailCheck);
        if (btn_EmailCheck != null)
        {
            btn_EmailCheck.interactable = false;
            img_EmailCheck = btn_EmailCheck.GetComponent<Image>();
            oriSprite = img_EmailCheck.sprite;
        }
        btn_EmailAuth = GetUI_Button(nameof(btn_EmailAuth), OnClick_EmailAuth);
        if (btn_EmailAuth != null)
        {
            btn_EmailAuth.interactable = false;
            img_EmailAuth = btn_EmailAuth.GetComponent<Image>();
        }
        btn_WakeAccount = GetUI_Button(nameof(btn_WakeAccount), OnClick_WakeAccount);
        if (btn_WakeAccount != null)
        {
            btn_WakeAccount.interactable = false;
        }

        GetUI_Button("btn_Back", () =>
        {
            int count = SceneLogic.instance._stackPanels.Count - 1;
            SceneLogic.instance.Back(count);
        });
        #endregion

        #region InputField
        input_EmailCheck = GetUI_TMPInputField(nameof(input_EmailCheck), CheckEmail);
        input_EmailAuth = GetUI_TMPInputField(nameof(input_EmailAuth), AuthEmail);
        #endregion

        #region Image
        img_WakeAccountIcon = GetUI_Img(nameof(img_WakeAccountIcon));
        if (img_WakeAccountIcon != null)
        {
            oriBtnIconColor = img_WakeAccountIcon.color;
        }
        #endregion 
    }

    #region 초기화
    protected override void OnEnable()
    {
        isCurEmailCheck = false;
        isCurEmailAuth = false;

        if (input_EmailCheck != null)
        {
            input_EmailCheck.Clear();
        }
        
        if (input_EmailAuth != null)
        {
            input_EmailAuth.Clear();
            input_EmailAuth.interactable = false;
        }

        if (txtmp_Timer != null)
        {
            Util.SetMasterLocalizing(txtmp_Timer, string.Empty);
        }

        if (img_EmailCheck != null)
        {
            img_EmailCheck.sprite = oriSprite;
        }
        
        if (img_EmailAuth != null)
        {
            img_EmailAuth.sprite = oriSprite;
        }
        
        if (img_WakeAccountIcon != null)
        {
            img_WakeAccountIcon.color = disabledColor;
        }

        if (txtmp_WarningEmailCheck != null)
        {
            Util.SetActive_Warning(txtmp_WarningEmailCheck);
        }
        
        if (txtmp_WarningEmailAuth != null)
        {
            Util.SetActive_Warning(txtmp_WarningEmailAuth);
        }
    }

    protected override void Start()
    {
        isStarted = true;
    }
    #endregion

    #region 휴면 계정 해제
    /// <summary>
    /// 이메일 실시간 조건 확인
    /// </summary>
    /// <param name="email"></param>
    private void CheckEmail(string email)
    {
        isCurEmailCheck = false;

        // 버튼 변경
        img_EmailCheck.sprite = oriSprite;

        bool b = Util.EmailRestriction(email);
        btn_EmailCheck.interactable = b;

        Util.SetActive_Warning(txtmp_WarningEmailCheck, b ? null : "3018");

        InitTimer();
    }

    /// <summary>
    /// 이메일로 보안코드 발송
    /// </summary>
    private void OnClick_EmailCheck()
    {
        InitTimer();

        string email = input_EmailCheck.text;

        if (!Regex.IsMatch(email, "@"))
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("3040")));
            return;
        }

        Single.Web.DormantCheckEmail(email, (res) =>
        {
            isCurEmailCheck = true;
            isCurEmailAuth = false;

            currentEmail = email;
            // 버튼 변경
            img_EmailCheck.sprite = Util.GetBtnSelectSprite();
            input_EmailAuth.interactable = true;

            int tempTime = res.remainTime;
            Util.RunCoroutine(Co_Timer(tempTime), "Timer_DormantAccount");

            ActiveConfirmInteract();
        },
        (res) =>
        {
            input_EmailCheck.Clear();

            ActiveConfirmInteract();
        });

        Util.SetActive_Warning(txtmp_WarningEmailCheck);
    }

    #region Timer
    private bool isInitTimer = false;
    private void InitTimer()
    {
        if (isInitTimer) return;

        Util.KillCoroutine("Timer_DormantAccount");
        Util.SetMasterLocalizing(txtmp_Timer, string.Empty);

        input_EmailAuth.Clear();
        // 버튼 변경
        img_EmailAuth.sprite = oriSprite;
        input_EmailAuth.interactable = false;

        Util.SetActive_Warning(txtmp_WarningEmailAuth);

        isInitTimer = true;
    }

    private IEnumerator<float> Co_Timer(int time)
    {
        isInitTimer = false;

        float curTime = time;
        int min, sec;
        while (curTime > 0)
        {
            if (isCurEmailAuth) break;

            if (isUpdate)
            {
                var backSec = (DateTime.Now - pauseDateTime).TotalSeconds;
                curTime -= (float)backSec;
                isUpdate = false;
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
            input_EmailAuth.text = string.Empty;
            input_EmailAuth.interactable = false;
            Util.SetActive_Warning(txtmp_WarningEmailAuth, "3031");
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
            {
                pauseDateTime = DateTime.Now;
            }
            else
            {
                isUpdate = true;
            }
        }
    }
    #endregion

    /// <summary>
    /// 인증코드 실시간 조건 확인
    /// </summary>
    /// <param name="code"></param>
    private void AuthEmail(string code)
    {
        isCurEmailAuth = false;
        // 버튼 변경
        img_EmailAuth.sprite = oriSprite;

        input_EmailAuth.text = code = Util.RegularReplaceOnlyNumber(code);

        bool b = code.Length == 0 || Util.AuthCodeRestriction(code);

        Util.SetActive_Warning(txtmp_WarningEmailAuth, b ? null : "3032");
    }

    /// <summary>
    /// 인증 번호 맞는지 확인
    /// </summary>
    private void OnClick_EmailAuth()
    {
        string authCode = input_EmailAuth.text;

        Single.Web.DormantConfirmEmail(currentEmail, authCode, (res) =>
        {
            isCurEmailAuth = true;

            Util.SetMasterLocalizing(txtmp_Timer, string.Empty);

            // 버튼 변경
            img_EmailAuth.sprite = Util.GetBtnSelectSprite();

            Util.SetActive_Warning(txtmp_WarningEmailAuth, null);
            ActiveConfirmInteract();
        },
        (res) =>
        {
            isCurEmailAuth = false;

            input_EmailAuth.Clear();

            btn_EmailAuth.interactable = false;

            Util.SetActive_Warning(txtmp_WarningEmailAuth, "3032");
            ActiveConfirmInteract();
        });
    }

    /// <summary>
    /// 확인 버튼 비/활성화 체크
    /// </summary>
    /// <returns></returns>
    private bool ActiveConfirmInteract()
    {
        bool b = isCurEmailCheck && isCurEmailAuth;
        btn_WakeAccount.interactable = b;
        img_WakeAccountIcon.color = b ? oriBtnIconColor : disabledColor;

        return b;
    }

    /// <summary>
    /// 휴면 해제
    /// </summary>
    private void OnClick_WakeAccount()
    {
        #region 오류 체크
        if (!ActiveConfirmInteract())
        {
            PushPopup<Popup_Basic>()
             .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("90012")));
            return;
        }
        #endregion

        Single.Web.DormantAccount(currentEmail, (res) =>
        {
            LocalPlayerData.Method.isDorment = false;
            SetCloseEndCallback(() =>
            {
                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("3056")));
            });
            int count = SceneLogic.instance._stackPanels.Count - 1;
            SceneLogic.instance.Back(count);
        },
        (res) =>
        {
            OnEnable();
        });
    }
    #endregion
}
