using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Popup_ChangePassword : PopupBase
{
    #region 변수
    private TMP_Text txtmp_WarningOldPassword;
    private TMP_Text txtmp_WarningNewPassword;
    private TMP_Text txtmp_WarningNewPasswordCheck;
    private Button btn_Confirm;
    private TMP_InputField input_OldPassword;
    private TMP_InputField input_NewPassword;
    private TMP_InputField input_NewPasswordCheck;
    private EventTrigger event_ShowOldPassword;
    private EventTrigger event_ShowNewPassword;
    private EventTrigger event_ShowNewPasswordCheck;

    private bool isCurOldPassword = false;
    private bool isCurNewPassword = false;
    private bool isCurNewPasswordCheck = false;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_WarningOldPassword = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningOldPassword));
        txtmp_WarningNewPassword = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningNewPassword));
        txtmp_WarningNewPasswordCheck = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningNewPasswordCheck));

        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("9102"));
        GetUI_TxtmpMasterLocalizing("txtmp_OldPasswordTitle", new MasterLocalData("60000"));
        GetUI_TxtmpMasterLocalizing("txtmp_NewPasswordTitle", new MasterLocalData("60001"));
        GetUI_TxtmpMasterLocalizing("txtmp_NewPasswordCheckTitle", new MasterLocalData("60002"));
        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("001"));
        #endregion

        #region Button
        btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnConfirm);

        GetUI_Button("btn_Exit", () => SceneLogic.instance.PopPopup());
        #endregion

        #region InputField
        input_OldPassword = GetUI_TMPInputField(nameof(input_OldPassword), OnValueChanged_OldPassword, masterLocalData: new MasterLocalData("setting_account_password"));
        input_NewPassword = GetUI_TMPInputField(nameof(input_NewPassword), OnValueChanged_NewPassword, masterLocalData: new MasterLocalData("setting_account_newpassword1"));
        input_NewPasswordCheck = GetUI_TMPInputField(nameof(input_NewPasswordCheck), OnValueChanged_NewPasswordCheck, masterLocalData: new MasterLocalData("setting_account_newpassword2"));
        #endregion

        #region etc
        event_ShowOldPassword = GetUI_Button("btn_ShowOldPassword").GetComponent<EventTrigger>();
        event_ShowNewPassword = GetUI_Button("btn_ShowNewPassword").GetComponent<EventTrigger>();
        event_ShowNewPasswordCheck = GetUI_Button("btn_ShowNewPasswordCheck").GetComponent<EventTrigger>();
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        Util.AddTriggerEntry(event_ShowOldPassword, input_OldPassword);
        Util.AddTriggerEntry(event_ShowNewPassword, input_NewPassword);
        Util.AddTriggerEntry(event_ShowNewPasswordCheck, input_NewPasswordCheck);
    }

    protected override void OnEnable()
    {
        isCurOldPassword = isCurNewPassword = isCurNewPasswordCheck = false;

        if (input_OldPassword != null)
        {
            input_OldPassword.Clear();
            input_OldPassword.contentType = TMP_InputField.ContentType.Password;
        }
        if (input_NewPassword != null)
        {
            input_NewPassword.Clear();
            input_NewPassword.contentType = TMP_InputField.ContentType.Password;
        }
        if (input_NewPasswordCheck != null)
        {
            input_NewPasswordCheck.Clear();
            input_NewPasswordCheck.contentType = TMP_InputField.ContentType.Password;
        }

        if (btn_Confirm != null)
        {
            SetInteractionConfirm();
        }

        // Warning 초기화하기
        Util.SetActive_Warning(txtmp_WarningOldPassword);
        Util.SetActive_Warning(txtmp_WarningNewPassword);
        Util.SetActive_Warning(txtmp_WarningNewPasswordCheck);
    }
    #endregion

    #region
    /// <summary>
    /// 현재 비밀번호 실시간 제한
    /// </summary>
    /// <param name="oldPassword"></param>
    private void OnValueChanged_OldPassword(string oldPassword)
    {
        // 한글 바로 삭제
        input_OldPassword.text = oldPassword = Util.RegularReplaceWithoutKorean(oldPassword);

        #region 제한
        bool isCur = Util.PasswordRestriction(oldPassword);
        isCurOldPassword = isCur;

        // 비밀번호는 8~20자 이내 영어, 숫자, 기호로 입력해야합니다.
        Util.SetActive_Warning(txtmp_WarningOldPassword, oldPassword.Length == 0 || isCur ? null : "3004");
        #endregion

        OnValueChanged_NewPassword(input_NewPassword.text);
        SetInteractionConfirm();
    }

    /// <summary>
    /// 새로운 비밀번호 실시간 제한
    /// </summary>
    /// <param name="newPassword"></param>
    private void OnValueChanged_NewPassword(string newPassword)
    {
        // 한글 바로 삭제
        input_NewPassword.text = newPassword = Util.RegularReplaceWithoutKorean(newPassword);

        #region 제한
        isCurNewPassword = false;
        string str = default;
        if (Util.PasswordRestriction(newPassword) && newPassword != input_OldPassword.text)
        {
            isCurNewPassword = true;
            str = "60003"; // 사용 가능한 비밀번호입니다
        }
        else if (newPassword.Length == 0)
        {
            str = null;
        }
        else if (newPassword == input_OldPassword.text)
        {
            str = "60004";
        }
        else
        {
            str = "3004"; // 비밀번호는 8~20자 이내 영어, 숫자, 기호로 입력해야합니다.
        }
        Util.SetActive_Warning(txtmp_WarningNewPassword, str);
        #endregion

        OnValueChanged_NewPasswordCheck(input_NewPasswordCheck.text);
        SetInteractionConfirm();
    }

    /// <summary>
    /// 비밀번호 확인 실시간 제한
    /// </summary>
    /// <param name="newPasswordCheck"></param>
    private void OnValueChanged_NewPasswordCheck(string newPasswordCheck)
    {
        // 한글 바로 삭제
        input_NewPasswordCheck.text = newPasswordCheck = Util.RegularReplaceWithoutKorean(newPasswordCheck);

        #region 제한
        isCurNewPasswordCheck = false;
        string str = default;
        if (Util.PasswordRestriction(newPasswordCheck) && newPasswordCheck == input_NewPassword.text)
        {
            isCurNewPasswordCheck = true;
            str = "3035"; // 비밀번호가 일치합니다.
        }
        else if (newPasswordCheck.Length == 0)
        {
            str = null;
        }
        else if (newPasswordCheck != input_NewPassword.text)
        {
            str = "3036"; // 비밀번호가 일치하지 않습니다.
        }
        else
        {
            str = "3004"; // 비밀번호는 8~20자 이내 영어, 숫자, 기호로 입력해야합니다.
        }
        Util.SetActive_Warning(txtmp_WarningNewPasswordCheck, str);
        #endregion

        SetInteractionConfirm();
    }

    /// <summary>
    /// 확인 버튼 비/활성화
    /// </summary>
    private void SetInteractionConfirm()
    {
        btn_Confirm.interactable = isCurOldPassword && isCurNewPassword && isCurNewPasswordCheck;
    }

    protected override void OnConfirm()
    {
        Single.Web.member.ChangePassword(input_OldPassword.text, input_NewPassword.text, (res) =>
        {
            LocalPlayerData.LoginToken = res.loginToken;

            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("60006")));

            base.OnConfirm();
        });
    }
    #endregion
}
