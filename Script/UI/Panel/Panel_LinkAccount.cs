using System.Linq;
using System.Text.RegularExpressions;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GamePotUnity;

public class Panel_LinkAccount : Panel_CreateAccount
{
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("setting_account_linking"));
        GetUI_TxtmpMasterLocalizing("txtmp_SignUp", new MasterLocalData("setting_account_linking"));
    }

    protected override void OnClick_EmailCheck()
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
            // 계정 연동의 경우, 계정 전환 팝업 오픈
            if (res.error == (int)WEBERROR.NET_E_ALREADY_EXIST_EMAIL)
            {
                if (res.memberInfo != null)
                {
                    SceneLogic.instance.PushPopup<Popup_ConvertAccount>().SetData(res.memberInfo, (int)LOGIN_PROVIDER_TYPE.ARZMETA);
                }
                else
                {
                    Debug.Log("MemberInfo is Null");
                }
                return;
            }

            // 계정 생성의 경우, 팝업 닫기
            // 이미 해당 이메일로 생성된 계정이 있습니다.
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

    protected override void OnClick_JoinConfirm()
    {
        // 아즈메타 계정 연동(비밀번호 필요)
        Single.Web.account.LinkedAccount(currentEmail, input_Password.text, (int)LOGIN_PROVIDER_TYPE.ARZMETA, (res) =>
        {
            LocalPlayerData.SocialLoginInfo = res.socialLoginInfo.ToList();

            // PopPanel 시, View_Account로 이동
            GetPanel<Panel_Setting>().forceView = Cons.View_Account;
            PopPanel();

            // GAMEPOT 공지사항 사용을 위한 서드파티 로그인
            GamePot.loginByThirdPartySDK(res.memberInfo.memberId);
        });
    }

    protected override void OnClick_Back()
    {
        int count = LocalPlayerData.SocialLoginInfo.Count;

        for (int i = 0; i < count; i++)
        {
            if (LocalPlayerData.SocialLoginInfo[i].providerType != (int)LOGIN_PROVIDER_TYPE.ARZMETA)
            {
                // 토글 off 및 View_Account로 이동
                Panel_Setting panel_Setting = GetPanel<Panel_Setting>();
                View_Account view_Account = panel_Setting.GetView<View_Account>();

                view_Account.togplus_Social_arzMETA.SetToggleIsOnWithoutNotify(false);
                panel_Setting.forceView = Cons.View_Account;
            }
        }
        base.OnClick_Back();
    }
}