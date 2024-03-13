using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using FrameWork.UI;
using System;
using Protocol;

public class View_ArzProfile_Edit : UIBase
{
    #region 변수
    private Sprite oriSprite;

    private TMP_Text txtmp_WarningNickname;
    private TMP_Text txtmp_StateMessageCount;
    private Button btn_NicknameCheck;
    private Image img_NicknameCheck;
    private Image img_Profile;
    private TMP_InputField input_Nickname;
    private TMP_InputField input_StateMessage;

    private Member data;

    private string CurNickname;
    private string CurStateMessage;

    private int stateMessageMax = 120;
    private bool isNicknameCheck = false;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_WarningNickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningNickname));
        txtmp_StateMessageCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StateMessageCount));

        GetUI_TxtmpMasterLocalizing("txtmp_MemberCode", LocalPlayerData.MemberCode);
        GetUI_TxtmpMasterLocalizing("txtmp_NicknameCheck", new MasterLocalData("4018"));
        GetUI_TxtmpMasterLocalizing("txtmp_Residence", new MasterLocalData("3013"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCodeTitle", new MasterLocalData("014"));
        #endregion

        #region Button
        btn_NicknameCheck = GetUI_Button(nameof(btn_NicknameCheck), OnClick_NicknameCheck);
        if (btn_NicknameCheck != null)
        {
            img_NicknameCheck = btn_NicknameCheck.GetComponent<Image>();
            oriSprite = img_NicknameCheck.sprite;
        }
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        #endregion

        #region InputField
        input_Nickname = GetUI_TMPInputField(nameof(input_Nickname), OnValueChanged_Nickname, masterLocalData: new MasterLocalData("013"));
        input_StateMessage = GetUI_TMPInputField(nameof(input_StateMessage), OnValueChanged_StateMessage);
        if (input_StateMessage != null)
        {
            input_StateMessage.onDeselect.AddListener(OnDeselect_StateMessage);
        }
        #endregion
    }

    #region 초기화
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
        isNicknameCheck = false;

        CurNickname = data.nickname;
        CurStateMessage = ChangeIsNullOrEmpty(data.stateMessage);

        // 버튼
        if (btn_NicknameCheck != null)
        {
            btn_NicknameCheck.interactable = false;
        }

        // 버튼 이미지
        if (img_NicknameCheck != null)
        {
            img_NicknameCheck.sprite = oriSprite;
        }

        // 인풋필드
        if (input_Nickname != null)
        {
            input_Nickname.Clear();
            input_Nickname.text = ChangeIsNullOrEmpty(data.nickname);
        }

        if (input_StateMessage != null)
        {
            input_StateMessage.Clear();

            Util.SetMasterLocalizing(input_StateMessage.placeholder, new MasterLocalData("3066", data.nickname));
            input_StateMessage.text = ChangeIsNullOrEmpty(data.stateMessage);
        }

        // 텍스트
        if (txtmp_StateMessageCount != null)
        {
            CountStateMessageLength();
        }

        // 프로필 사진
        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => img_Profile.sprite = sprite);
        }

        Util.SetActive_Warning(txtmp_WarningNickname);
    }
    #endregion

    #region Arzmeta_Edit
    /// <summary>
    /// 상태메시지 글자 수 계산
    /// </summary>
    private void CountStateMessageLength()
    {
        Util.SetMasterLocalizing(txtmp_StateMessageCount, $"{input_StateMessage.text.Length} / {stateMessageMax}");
    }

    /// <summary>
    /// 닉네임 실시간 제한
    /// </summary>
    private void OnValueChanged_Nickname(string nickname)
    {
        isNicknameCheck = false;

        // 버튼 변경
        img_NicknameCheck.sprite = oriSprite;

        #region 제한
        bool isCurrent = Util.NicknameRestriction(nickname);
        btn_NicknameCheck.interactable = isCurrent;

        string str = nickname.Length == 0 || isCurrent ? null : "4011";

        Util.SetActive_Warning(txtmp_WarningNickname, str);
        #endregion
    }

    /// <summary>
    /// 닉네임 중복확인
    /// </summary>
    private void OnClick_NicknameCheck()
    {
        string nickname = input_Nickname.text;
        if (!Util.NicknameRestriction(nickname))
        {
            Util.SetActive_Warning(txtmp_WarningNickname, "4011"); // 닉네임은 2~12자 이내 한글, 영문, 숫자로 입력해주세요!
            PushPopup<Popup_Basic>()
              .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("4011")));
            return;
        }
        else if (Util.CheckForbiddenWords(nickname))
        {
            Util.SetActive_Warning(txtmp_WarningNickname, "4012"); // 부적절한 닉네임은 사용할 수 없어요!
            PushPopup<Popup_Basic>()
              .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("4012")));
            return;
        }

        Single.Web.member.CheckNickName(nickname, (res) =>
        {
            isNicknameCheck = true;

            CurNickname = nickname;
            Util.SetActive_Warning(txtmp_WarningNickname, "4013");

            // 버튼 변경
            img_NicknameCheck.sprite = Util.GetBtnSelectSprite();
        },
        (res) =>
        {
            Util.SetActive_Warning(txtmp_WarningNickname);
        });
    }

    /// <summary>
    /// 상태메시지 실시간 제한
    /// </summary>
    private void OnValueChanged_StateMessage(string stateMessage)
    {
        if (!Util.StateMessageRestriction(stateMessage))
        {
            input_StateMessage.text = CurStateMessage;
            return;
        }

        CountStateMessageLength();
        CurStateMessage = stateMessage;
    }

    /// <summary>
    /// 상태메시지 인풋필드 디셀렉 될 시 금칙어 검사
    /// </summary>
    private void OnDeselect_StateMessage(string stateMessage)
    {
        if (stateMessage.Length <= 0) return;

        input_StateMessage.text = CurStateMessage = Util.ChangeForbiddenWordsToStar(stateMessage);
    }

    /// <summary>
    /// 저장하기
    /// </summary>
    public void CheckAndSaveData()
    {
        if (IsCheck())
        {
            Single.Web.member.UpdateMyProfile(CurNickname, CurStateMessage, (res) =>
            {
                LocalPlayerData.NickName = data.nickname = res.nickname;
                LocalPlayerData.StateMessage = data.stateMessage = res.stateMessage;

                PKT_C_SET_NICKNAME();

                OpenToast<Toast_Basic>()
                .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("10301")));
            });
        }
    }

    /// <summary>
    /// 문제 있는지 체크 반환
    /// </summary>
    /// <returns></returns>
    private bool IsCheck()
    {
        if (CompareNickname())
        {
            isNicknameCheck = true;
        }
        
        if (!isNicknameCheck)
        {
            PushPopup<Popup_Basic>()
               .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("4015")));

            return false;
        }
        return true;
    }

	/// <summary>
	/// 실시간 닉네임 변경
	/// </summary>
	/// <param name="eSessionType"></param>
	private void PKT_C_SET_NICKNAME()
	{
		if (MyPlayer.instance != null)
		{
            C_SET_NICKNAME packet = new C_SET_NICKNAME
            {
                Nickname = LocalPlayerData.NickName
            };

            Single.RealTime.Send(packet);
		}
	}
    #endregion

    #region 기능 메소드
	/// <summary>
	/// 저장하기 전 데이터와 현 데이터가 같은지
	/// </summary>
	/// <returns></returns>
	public bool CompareCheck()
    {
        return NicknameIsNullOrEmpty() && input_StateMessage.text == ChangeIsNullOrEmpty(data.stateMessage);
    }

    /// <summary>
    /// 인풋필드가 비어있거나 인풋필드의 닉네임과 데이터의 닉네임이 일치하면 true 반환
    /// </summary>
    /// <returns></returns>
    private bool NicknameIsNullOrEmpty()
    {
        if (string.IsNullOrEmpty(input_Nickname.text) && !string.IsNullOrEmpty(data.nickname))
        {
            CurNickname = data.nickname;
            return true;
        }

        return CompareNickname();
    }

    /// <summary>
    /// 인풋필드의 닉네임과 데이터가 일치하는지 여부
    /// </summary>
    /// <returns></returns>
    private bool CompareNickname()
    {
        return input_Nickname.text == data.nickname;
    }

    /// <summary>
    /// 스트링이 비어있는지 확인하고 비어있으면 "", 아니면 해당 스트링 반환
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string ChangeIsNullOrEmpty(string str)
    {
        return !string.IsNullOrEmpty(str) ? str : "";
    }
    #endregion
}
