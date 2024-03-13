using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FrameWork.UI;
using UnityEngine.UI;

public class View_BizCard_A_Edit : BizCardBase
{
    #region 변수
    private Image img_Profile;
    private TMP_Text txtmp_WarningName;
    private TMP_Text txtmp_WarningJob;
    private TMP_Text txtmp_WarningPhoneNumber;
    private TMP_Text txtmp_WarningEmail;
    private TMP_InputField input_Name;
    private TMP_InputField input_Job;
    private TMP_InputField input_PhoneNumber;
    private TMP_InputField input_Email;
    private TogglePlus togplus_Defalut;

    private Panel_BizCard panel_BizCard;

    private bool isCurName = false;
    private bool isCurJob = false;
    private bool isCurPhoneNumber = false;
    private bool isCurEmail = false;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_WarningName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningName));
        txtmp_WarningJob = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningJob));
        txtmp_WarningPhoneNumber = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningPhoneNumber));
        txtmp_WarningEmail = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningEmail));
        #endregion

        #region TMP_InputField
        input_Name = GetUI_TMPInputField(nameof(input_Name), OnValueChanged_Name, masterLocalData: new MasterLocalData("common_name"));
        input_Job = GetUI_TMPInputField(nameof(input_Job), OnValueChanged_Job, masterLocalData: new MasterLocalData("common_job"));
        input_PhoneNumber = GetUI_TMPInputField(nameof(input_PhoneNumber), OnValueChanged_PhoneNumber, masterLocalData: new MasterLocalData("common_phone"));
        if (input_PhoneNumber != null)
        {
            input_PhoneNumber.keyboardType = TouchScreenKeyboardType.NumberPad;
            input_PhoneNumber.onSelect.AddListener(OnSelect_PhoneNumber);
            input_PhoneNumber.onDeselect.AddListener(OnDeselect_PhoneNumber);
        }
        input_Email = GetUI_TMPInputField(nameof(input_Email), OnValueChanged_Email, masterLocalData: new MasterLocalData("common_mail"));
        if (input_Email != null)
        {
            input_Email.keyboardType = TouchScreenKeyboardType.EmailAddress;
        }
        #endregion

        #region TogglePlus
        togplus_Defalut = GetUI<TogglePlus>(nameof(togplus_Defalut));
        if (togplus_Defalut != null)
        {
            togplus_Defalut.SetToggleAction((b) =>
            {
                int curIndex = b ? data.index : -1;
                panel_BizCard.SetActiveDefault(curIndex);
            });
        }
        #endregion

        #region Button
        GetUI_Button("btn_Delete", () => panel_BizCard.DeleteBizCardView(data.index));
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        panel_BizCard = GetPanel<Panel_BizCard>();
    }

    protected override void SetContent()
    {
        if (input_Name != null)
        {
            input_Name.text = data.bizCard.name;
        }

        if (input_Job != null)
        {
            input_Job.text = data.bizCard.job;
        }

        if (input_PhoneNumber != null)
        {
            input_PhoneNumber.text = data.bizCard.phone;
        }

        if (input_Email != null)
        {
            input_Email.text = data.bizCard.email;
        }

        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => { img_Profile.sprite = sprite; });
        }

        if (togplus_Defalut != null)
        {
            SetActiveDefault(data.IsDefault());
        }

        if (txtmp_WarningName != null)
        {
            Util.SetActive_Warning(txtmp_WarningName);
        }

        if (txtmp_WarningJob != null)
        {
            Util.SetActive_Warning(txtmp_WarningJob);
        }

        if (txtmp_WarningPhoneNumber != null)
        {
            Util.SetActive_Warning(txtmp_WarningPhoneNumber);
        }

        if (txtmp_WarningEmail != null)
        {
            Util.SetActive_Warning(txtmp_WarningEmail);
        }
    }
    #endregion

    #region 
    /// <summary>
    /// 이름 실시간 제한
    /// </summary>
    /// <param name="name"></param>
    private void OnValueChanged_Name(string name)
    {
        data.bizCard.name = name;

        bool curName = isCurName = Util.BizNameRestriction(name);
        string str = name.Length == 0 || curName ? null : "businesscard_error_name_form";
        Util.SetActive_Warning(txtmp_WarningName, str);
    }

    /// <summary>
    /// 직업 실시간 제한
    /// </summary>
    /// <param name="job"></param>
    private void OnValueChanged_Job(string job)
    {
        data.bizCard.job = job;

        bool curJob = isCurJob = Util.BizJobRestriction(job);
        string str = job.Length == 0 || curJob ? null : "businesscard_error_job_form";
        Util.SetActive_Warning(txtmp_WarningJob, str);
    }

    /// <summary>
    /// 핸드폰 번호 실시간 제한
    /// </summary>
    /// <param name="number"></param>
    private void OnValueChanged_PhoneNumber(string number)
    {
        bool curNumber = isCurPhoneNumber = Util.BizPhoneNumberRestriction(number.Replace("-", ""));
        string str = number.Length == 0 || curNumber ? null : "businesscard_error_phone_form";
        Util.SetActive_Warning(txtmp_WarningPhoneNumber, str);
    }

    /// <summary>
    /// 핸드폰 번호 인풋필드 선택 시 처리
    /// </summary>
    /// <param name="number"></param>
    private void OnSelect_PhoneNumber(string number)
    {
        input_PhoneNumber.text = Util.RegularReplaceOnlyNumber(number);
    }

    /// <summary>
    /// 핸드폰 번호 인풋필드 비선택 시 처리
    /// </summary>
    /// <param name="number"></param>
    private void OnDeselect_PhoneNumber(string number)
    {
        input_PhoneNumber.text = data.bizCard.phone = Util.RegularAddHyphenPhonNumber(number);
    }

    /// <summary>
    /// 이메일 실시간 제한
    /// </summary>
    /// <param name="email"></param>
    private void OnValueChanged_Email(string email)
    {
        data.bizCard.email = email;

        bool curEmail = isCurEmail = Util.BizEmailRestriction(email);
        string str = email.Length == 0 || curEmail ? null : "businesscard_error_mail_form";
        Util.SetActive_Warning(txtmp_WarningEmail, str);
    }
    #endregion

    #region 
    /// <summary>
    /// 데이터에 오류가 있는지 체크
    /// </summary>
    /// <returns></returns>
    public override bool CheckData()
    {
        return isCurName && isCurJob && isCurPhoneNumber && isCurEmail;
    }

    /// <summary>
    /// 대표 명함 토글 상태 변경
    /// </summary>
    /// <param name="b"></param>
    public override void SetActiveDefault(bool b)
    {
        togplus_Defalut.SetToggleIsOnWithoutNotify(b);
    }

    /// <summary>
    /// 대표 명함인지 체크
    /// </summary>
    /// <returns></returns>
    public override (bool isOn, int index) CheckDefault()
    {
        return (togplus_Defalut.GetToggleIsOn(), data.index);
    }
    #endregion
}
