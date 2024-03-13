using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_ArzLogin : PanelBase
{
    #region 변수
    private TMP_InputField input_Email;
    private TMP_InputField input_Password;
    private EventTrigger showPassword;
    private Button btn_Login;
    private TMP_Text txtmp_Login;

    private Color oriColor;
    private Color disableColor = Cons.Color_Gray;

    private bool isCurEmail;
    private bool isCurPassword;
    #endregion

    #region 키보드 탭 - 변고경
    private void Update()
    {
        KeyboardInput();
    }
    private void KeyboardInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            // Tab + LeftShift는 위의 Selectable 객체를 선택
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            bool isInputField = next is InputField || next is TMP_InputField;

            if (next != null && isInputField)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Tab은 아래의 Selectable 객체를 선택
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            bool isInputField = next is InputField || next is TMP_InputField;

            if (next != null && isInputField)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick_Login();
        }
#endif
    }
    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        btn_Login = GetUI_Button(nameof(btn_Login), OnClick_Login);

        GetUI_Button("btn_SignUp", () => { PushPanel<Panel_CreateAccount>(false); });
        GetUI_Button("btn_GotoFindPassword", () => { PushPanel<Panel_FindPassword>(false); });
        GetUI_Button("btn_Back", Back);
        #endregion

        #region InputField
        input_Email = GetUI_TMPInputField(nameof(input_Email), CheckEmail, masterLocalData: new MasterLocalData("3016"));
        input_Password = GetUI_TMPInputField(nameof(input_Password), CheckPassword, masterLocalData: new MasterLocalData("3033"));
        if (input_Password != null)
        {
            input_Password.contentType = TMP_InputField.ContentType.Password;
        }
        #endregion

        #region TMP_Text
        txtmp_Login = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Login), new MasterLocalData("common_login"));
        if (txtmp_Login != null)
        {
            oriColor = txtmp_Login.color;
        }

        GetUI_TxtmpMasterLocalizing("txtmp_GotoFindPassword", new MasterLocalData("3007"));
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3047"));
        GetUI_TxtmpMasterLocalizing("txtmp_Email", new MasterLocalData("012"));
        GetUI_TxtmpMasterLocalizing("txtmp_Password", new MasterLocalData("006"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sign", new MasterLocalData("3011"));
        #endregion

        #region etc
        showPassword = GetUI_Button("btn_ShowPassword").GetComponent<EventTrigger>();
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        if (LocalPlayerData.Method.IsConvertAccount)
        {
            LocalPlayerData.Method.IsConvertAccount = false;
        }

        Util.AddTriggerEntry(showPassword, input_Password);
    }
    protected override void OnEnable()
    {
        isCurEmail = isCurPassword = false;

        if (input_Email != null)
        {
            input_Email.Clear();

            bool b = string.IsNullOrEmpty(LocalPlayerData.Email);
            input_Email.text = b ? string.Empty : LocalPlayerData.Email;
            isCurEmail = !b;
        }
        if (input_Password != null)
        {
            input_Password.Clear();
        }
        if (btn_Login != null)
        {
            btn_Login.interactable = false;
        }
        if (txtmp_Login != null)
        {
            txtmp_Login.color = disableColor;
        }
    }
    #endregion

    #region 로그인
    /// <summary>
    /// 이메일 실시간 제한 체크
    /// </summary>
    /// <param name="email"></param>
    private void CheckEmail(string email)
    {
        isCurEmail = email.Length > 0;
        CheckCurLogin();
    }

    /// <summary>
    /// 패스워드 실시간 제한 체크
    /// </summary>
    /// <param name="password"></param>
    private void CheckPassword(string password)
    {
        isCurPassword = password.Length > 0;
        CheckCurLogin();
    }

    private void CheckCurLogin()
    {
        bool b = isCurEmail && isCurPassword;
        txtmp_Login.color = b ? oriColor : disableColor;
        btn_Login.interactable = b;
    }

    /// <summary>
    /// 로그인
    /// </summary>
    public void OnClick_Login()
    {
        Single.Web.member.CheckWithdrawalProgress((int)LOGIN_PROVIDER_TYPE.ARZMETA, input_Email.text, input_Password.text,
            (res) =>
            {
                if ((WEBERROR)res.error == WEBERROR.NET_E_IS_WITHDRAWAL_MEMBER)
                {
                    LocalPlayerData.Method.CheckProcessWithdrawal(res,
                        () =>
                        {
                            Login();
                        },
                        () =>
                        {
                            Single.Scene.FadeOut(1f,
                                () =>
                                {
                                    Single.Scene.LoadScene(SceneName.Scene_Base_Title);
                                });
                        });
                    return;
                }

                Login();
            });
    }
    private void Login()
    {
        Single.Scene.isSceneLock = true;
        Single.Web.account.Login(input_Email.text, input_Password.text,
        (res) =>
        {
            Single.Web.account.SetLocalPlayerData(res);
        },
        (error) =>
        {
            if (LocalPlayerData.Method.DormentAccount(error.error))
                return;

            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("3009")))
                .ChainPopupAction(new PopupAction(() => input_Password.Clear()));
        });
    }
    #endregion
}
