using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using GamePotUnity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_ConvertAccount : PopupBase
{
    #region 변수 _Curr_: 현재 계정 관련 변수, _Conv_ : 전환할 수 있는 계정 변수
    MemberInfo memberInfo;
    List<MemberAccountInfo> convertSocialInfo;
    private int wantConvertProviderType;

    private TMP_Text txtmp_Curr_Nickname;
    private TMP_Text txtmp_Curr_StateMessage;

    private TMP_Text txtmp_Conv_Nickname;
    private TMP_Text txtmp_Conv_StateMessage;

    private Image img_Curr_Profile;
    private Image img_Conv_Profile;

    private Image icon_Curr_Arzmeta;
    private Image icon_Curr_Naver;
    private Image icon_Curr_Google;
    private Image icon_Curr_Apple;

    private Image icon_Conv_Arzmeta;
    private Image icon_Conv_Naver;
    private Image icon_Conv_Google;
    private Image icon_Conv_Apple;

    private Button btn_Exit;
    private Button btn_Confirm;
    #endregion

    #region 초기화
    protected override void SetMemberUI()
    {
        popupAnimator = GetComponent<Animator>();

        // go_Top
        GetUI_TxtmpMasterLocalizing("txtmp_ChangeAccountGuide", new MasterLocalData("setting_confirm_login_switch"));
        btn_Exit = GetUI_Button(nameof(btn_Exit), OnClickExit);

        // go_Desc
        // view_CurrentAccount
        GetUI_TxtmpMasterLocalizing("txtmp_Curr_Account", new MasterLocalData("setting_current_login"));
        txtmp_Curr_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Curr_Nickname));
        txtmp_Curr_StateMessage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Curr_StateMessage));

        img_Curr_Profile = GetUI_Img(nameof(img_Curr_Profile));

        icon_Curr_Arzmeta = Util.Search<Image>(this.gameObject, nameof(icon_Curr_Arzmeta));
        icon_Curr_Naver = Util.Search<Image>(this.gameObject, nameof(icon_Curr_Naver));
        icon_Curr_Google = Util.Search<Image>(this.gameObject, nameof(icon_Curr_Google));
        icon_Curr_Apple = Util.Search<Image>(this.gameObject, nameof(icon_Curr_Apple));

        // view_ConvertAccount
        GetUI_TxtmpMasterLocalizing("txtmp_Conv_Account", new MasterLocalData("setting_switch_login"));
        txtmp_Conv_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Conv_Nickname));
        txtmp_Conv_StateMessage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Conv_StateMessage));

        img_Conv_Profile = GetUI_Img(nameof(img_Conv_Profile));

        icon_Conv_Arzmeta = Util.Search<Image>(this.gameObject, nameof(icon_Conv_Arzmeta));
        icon_Conv_Naver = Util.Search<Image>(this.gameObject, nameof(icon_Conv_Naver));
        icon_Conv_Google = Util.Search<Image>(this.gameObject, nameof(icon_Conv_Google));
        icon_Conv_Apple = Util.Search<Image>(this.gameObject, nameof(icon_Conv_Apple));

        // 로그인 전환 버튼
        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("setting_switch"));
        btn_Confirm = GetUI_Button(nameof(btn_Confirm), OnClickChangeLogin);

    }
    #endregion

    #region 메소드
    /// <summary>
    /// NET_E_ALREADY_LINKED_OTHER_ACCOUNT || NET_E_ALREADY_EXIST_EMAIL 시, 전환할 계정 정보 받아오기
    /// </summary>
    public void SetData(MemberInfo getData, int providerType)
    {
        memberInfo = getData;
        wantConvertProviderType = providerType;

        CurrentSetData();
        ConvertSetData();
    }

    /// <summary>
    /// 현재 계정 데이터 설정
    /// </summary>
    public void CurrentSetData()
    {
        ShowCurrentProviderType();

        LocalPlayerData.Method.GetAvatarSprite(LocalPlayerData.MemberCode, LocalPlayerData.AvatarInfo, SetCurrProfile);

        Util.SetMasterLocalizing(txtmp_Curr_Nickname, LocalPlayerData.NickName);
        Util.SetMasterLocalizing(txtmp_Curr_StateMessage, LocalPlayerData.StateMessage);
    }

    /// <summary>
    /// 현재 계정 - 연동된 소셜 계정 아이콘 보여주기
    /// </summary>
    private void ShowCurrentProviderType()
    {
        icon_Curr_Arzmeta.gameObject.SetActive(false);
        icon_Curr_Naver.gameObject.SetActive(false);
        icon_Curr_Google.gameObject.SetActive(false);
        icon_Curr_Apple.gameObject.SetActive(false);

        // 연동된 소셜 계정 아이콘 활성화
        for (int i = 0; i < LocalPlayerData.SocialLoginInfo.Count; i++)
        {
            switch ((LOGIN_PROVIDER_TYPE)LocalPlayerData.SocialLoginInfo[i].providerType)
            {
                case LOGIN_PROVIDER_TYPE.ARZMETA:
                    {
                        icon_Curr_Arzmeta.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.NAVER:
                    {
                        icon_Curr_Naver.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.GOOGLE:
                    {
                        icon_Curr_Google.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.APPLE:
                    {
                        icon_Curr_Apple.gameObject.SetActive(true);
                        break;
                    }
            }
        }
    }

    private void SetCurrProfile(Sprite sprite)
    {
        if (img_Curr_Profile != null)
        {
            img_Curr_Profile.sprite = sprite;
        }
    }

    /// <summary>
    /// 전환할 계정 데이터 설정
    /// </summary>
    public void ConvertSetData()
    {
        string convertNickname = memberInfo.nickname;
        string convertStatement = memberInfo.stateMessage;

        convertSocialInfo = memberInfo.socialLoginInfo.ToList();

        LocalPlayerData.Method.GetAvatarSprite(memberInfo.memberCode, memberInfo.avatarInfos, SetConvProfile);

        ShowConvertProviderType();

        Util.SetMasterLocalizing(txtmp_Conv_Nickname, convertNickname);
        Util.SetMasterLocalizing(txtmp_Conv_StateMessage, convertStatement);
    }

    /// <summary>
    /// 전환할 계정 - 연동된 소셜 계정 아이콘 보여주기
    /// </summary>
    private void ShowConvertProviderType()
    {
        icon_Conv_Arzmeta.gameObject.SetActive(false);
        icon_Conv_Naver.gameObject.SetActive(false);
        icon_Conv_Google.gameObject.SetActive(false);
        icon_Conv_Apple.gameObject.SetActive(false);

        // 연동된 소셜 계정 아이콘 활성화
        for (int i = 0; i < convertSocialInfo.Count; i++)
        {
            switch ((LOGIN_PROVIDER_TYPE)convertSocialInfo[i].providerType)
            {
                case LOGIN_PROVIDER_TYPE.ARZMETA:
                    {
                        icon_Conv_Arzmeta.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.NAVER:
                    {
                        icon_Conv_Naver.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.GOOGLE:
                    {
                        icon_Conv_Google.gameObject.SetActive(true);
                        break;
                    }
                case LOGIN_PROVIDER_TYPE.APPLE:
                    {
                        icon_Conv_Apple.gameObject.SetActive(true);
                        break;
                    }
            }
        }
    }

    private void SetConvProfile(Sprite sprite)
    {
        if (img_Conv_Profile != null)
        {
            img_Conv_Profile.sprite = sprite;
        }
    }

    /// <summary>
    /// 팝업 그냥 닫는 경우, 계정 연동 패널 업데이트
    /// </summary>
    private void OnClickExit()
    {
        View_Account view_Account = GetPanel<Panel_Setting>().GetView<View_Account>();

        SceneLogic.instance.PopPopup();

        // 로그인 취소 - 액션없이 Togplus Off
        view_Account.ToggleOff(wantConvertProviderType);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (LocalPlayerData.Method.IsSocialLogined())
            GamePot.logout();
#endif
    }

    /// <summary>
    /// 로그아웃 및 로그인 전환
    /// </summary>
    private void OnClickChangeLogin()
    {
        // 전환하고 싶은 providertype에 해당하는 계정 정보 저장
        MemberAccountInfo info = convertSocialInfo.FirstOrDefault(x => x.providerType == wantConvertProviderType);
        if (info != null)
        {
            if (info.providerType == 1) 
                ConvertAccount();
            
            // 소셜 계정 탈퇴 여부 확인
            else
            {
                Single.Web.member.CheckWithdrawalProgress(info.providerType, info.accountToken, null, (res) =>
                {
                    if (res.error == (int)WEBERROR.NET_E_IS_WITHDRAWAL_MEMBER)
                    {
                        LocalPlayerData.Method.CheckProcessWithdrawal(res,
                            () =>
                            {
                                ConvertAccount();
                            },
                            () => 
                            {
                                OnClickExit(); 
                            });
                        return;
                    }

                    ConvertAccount();
                });
            }
        }
    }

    private void ConvertAccount()
    {
        // Scene_Title 로드 및 로그인 전환을 위한 데이터 리셋
        Single.Scene.FadeOut(1f, () =>
        {
            RealtimeUtils.Disconnect();

            LocalPlayerData.ResetData();

            // 전환할 수 있는 계정이 아즈메타 계정일 경우, 로그인 패널로 이동(Scene_Title에 코드 작성)
            if (wantConvertProviderType == 1)
            {
                LocalPlayerData.Method.IsConvertAccount = true;
            }

            // 전환할 수 있는 계정이 소셜 계정일 경우, 자동 로그인
            else
            {
                // 소셜 계정 자동 로그인을 위한 LocalPlayerData 세팅
                LocalPlayerData.MemberID = memberInfo.memberId;
                LocalPlayerData.ProviderType = wantConvertProviderType;

                Single.Web.account.AutoLogin();
            }

            Single.Scene.LoadScene(SceneName.Scene_Base_Title);
        });
    }
#endregion
}
