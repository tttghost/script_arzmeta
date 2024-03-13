using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 회원가입 시 최초 생성하는 아즈메타 명함
/// </summary>
public class Panel_ArzProfile_Create : PanelBase
{
    #region 변수
    public Sprite checkSprite;
    private Sprite oriSprite;

    public int curPresetId { get; set; }

    private TMP_Text txtmp_WarningNickname;
    private TMP_Text txtmp_MemberCode;
    private TMP_InputField input_Nickname;
    private TMP_InputField input_StateMessage;
    private Image img_NicknameCheck;

    private AvatarPartsController avatarPartsController;

    private string curNickname = null;
    private string curStatrMessage = null;

    private bool isNicknameCheck = false;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_WarningNickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_WarningNickname));
        txtmp_MemberCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MemberCode));

        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3069"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Nickname", new MasterLocalData("013"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCodeTitle", new MasterLocalData("014"));
        GetUI_TxtmpMasterLocalizing("txtmp_NicknameCheck", new MasterLocalData("4018"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_StateMessage", new MasterLocalData("3061"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sub", new MasterLocalData("3062"));
        #endregion

        #region Button
        Button btn_NicknameCheck = GetUI_Button(nameof(btn_NicknameCheck), OnClick_NicknameCheck);
        if (btn_NicknameCheck != null)
        {
            img_NicknameCheck = btn_NicknameCheck.GetComponent<Image>();
            oriSprite = img_NicknameCheck.sprite;
        }
        GetUI_Button("btn_Back", () => SceneLogic.instance.Back());
        GetUI_Button("btn_Next", OnClick_Next);
        #endregion

        #region TMP_InputField
        input_Nickname = GetUI_TMPInputField(nameof(input_Nickname), OnValueChanged_Nickname);
        input_StateMessage = GetUI_TMPInputField(nameof(input_StateMessage), OnValueChanged_StateMessage);
        if (input_StateMessage != null)
        {
            input_StateMessage.onDeselect.AddListener(OnDeselect_StateMessage);
        }
        #endregion

        #region etc
        GameObject go_AvatarView = GameObject.Find(nameof(go_AvatarView));
        avatarPartsController = go_AvatarView.GetComponent<AvatarPartsController>();
        if (avatarPartsController)
        {
            Animator anim_Avatar = Util.Search<Animator>(go_AvatarView, "AvatarParts");
            if (anim_Avatar != null)
            {
                avatarPartsController.SetTarget(anim_Avatar.transform);
            }
        }
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        base.OnEnable();

        if (txtmp_MemberCode != null)
        {
            Util.SetMasterLocalizing(txtmp_MemberCode, LocalPlayerData.MemberCode);
        }

        if (input_Nickname != null)
        {
            input_Nickname.Clear();
        }

        if (input_StateMessage != null)
        {
            input_StateMessage.Clear();
        }

        if (img_NicknameCheck != null)
        {
            ChangeBtnSprite(false);
        }

        if (txtmp_WarningNickname != null)
        {
            Util.SetActive_Warning(txtmp_WarningNickname);
        }
    }
    #endregion

    #region BusinessCard
    /// <summary>
    /// 닉네임 
    /// </summary>
    /// <param name="nickname"></param>
    private void OnValueChanged_Nickname(string nickname)
    {
        isNicknameCheck = false;

        bool b = nickname.Length == 0 || Util.NicknameRestriction(nickname);
        string str = b ? null : "4011";

        Util.SetActive_Warning(txtmp_WarningNickname, str);

        // 버튼 변경
        ChangeBtnSprite(false);
    }

    /// <summary>
    /// 닉네임 중복확인
    /// </summary>
    private void OnClick_NicknameCheck()
    {
        string nickname = input_Nickname.text;
        if (!Util.NicknameRestriction(nickname))
        {
            Util.SetActive_Warning(txtmp_WarningNickname, "4011");
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("4011")));
            return;
        }
        if (Util.CheckForbiddenWords(nickname))
        {
            Util.SetActive_Warning(txtmp_WarningNickname, "4012");
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("4012")));
            return;
        }

        Single.Web.member.CheckNickName(nickname, (res) =>
        {
            isNicknameCheck = true;
            curNickname = nickname;
            Util.SetActive_Warning(txtmp_WarningNickname, "4013");

            // 버튼 변경
            ChangeBtnSprite(true);
        },
        (res) =>
        {
            Util.SetActive_Warning(txtmp_WarningNickname);
        });
    }

    /// <summary>
    /// 상태메시지 실시간 제한
    /// </summary>
    /// <param name="stateMessage"></param>
    private void OnValueChanged_StateMessage(string stateMessage)
    {
        if (stateMessage.Length > 120)
        {
            input_StateMessage.text = curStatrMessage;
            return;
        }
        curStatrMessage = stateMessage;
    }

    /// <summary>
    /// 상태메시지 인풋필드 디셀렉 될 시 금칙어 검사
    /// </summary>
    /// <param name="stateMessage"></param>
    private void OnDeselect_StateMessage(string stateMessage)
    {
        if (stateMessage.Length <= 0) return;

        input_StateMessage.text = curStatrMessage = Util.ChangeForbiddenWordsToStar(stateMessage);
    }

    /// <summary>
    /// 저장 및 다음으로 이동
    /// </summary>
    private void OnClick_Next()
    {
        if (!isNicknameCheck)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("4015")));
            return;
        }

        // 전체 저장하기
        Single.Web.member.SetAvatarPreset(curPresetId, curNickname, curStatrMessage, (res) =>
        {
            LocalPlayerData.AvatarInfo = res.avatarInfos;
            LocalPlayerData.NickName = res.nickname;
            LocalPlayerData.StateMessage = res.stateMessage;

            SceneLogic.instance.isUILock = false;
            PushPanel<Panel_ArzProfile_Check>();
        });
    }

    private void ChangeBtnSprite(bool isCheck)
    {
        img_NicknameCheck.sprite = isCheck ? checkSprite : oriSprite;
    }
    #endregion
}
