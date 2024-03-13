using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using MEC;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using GamePotUnity;
using UnityEngine.UI;

public class Panel_Title : PanelBase
{
    #region 변수
    private Button btn_Login;
    private bool isLoginDone = false;
	#endregion

	private void OnDestroy()
	{
        RealtimeWebManager.callback_confirm -= () => btn_Login.interactable = true;
    }

	protected override void OnEnable()
    {
        RefreshUI();
    }

    protected override void SetMemberUI()
    {
        #region Button
        btn_Login = GetUI_Button(nameof(btn_Login), OnClick_Login);
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("3019"));
        #endregion

        RealtimeWebManager.callback_confirm += () => btn_Login.interactable = true;
    }

    #region Title
    private void OnClick_Login()
    {
        if (isLoginDone) return;
        RefreshUI(false);

        LOGIN_PROVIDER_TYPE curType = (LOGIN_PROVIDER_TYPE)LocalPlayerData.ProviderType;
        switch (curType)
        {
            case LOGIN_PROVIDER_TYPE.NAVER:
            case LOGIN_PROVIDER_TYPE.GOOGLE:
            case LOGIN_PROVIDER_TYPE.APPLE:
                Debug.Log(curType + " 자동 로그인!");
                SocialAutoLogin();
                break;
            case LOGIN_PROVIDER_TYPE.ARZMETA:
            default:
                CheckLoginTokenAndAutoLogin();
                break;
        }
    }

    /// <summary>
    /// 자동로그인 처리
    /// </summary>
    void CheckLoginTokenAndAutoLogin()
    {
        if (!string.IsNullOrEmpty(LocalPlayerData.LoginToken) && LocalPlayerData.ProviderType != 0)
        {
            LoginAuth();
        }
        else
        {
            LocalPlayerData.ResetData();
            PushPanel<Panel_SocialLogin>(false);
        }
    }

    /// <summary>
    /// 로그인 토큰으로 멤버아이디 가져오기
    /// </summary>
    public void LoginAuth()
    {
        isLoginDone = false;

        Single.Web.account.LoginAuth((res) =>
        {
            LocalPlayerData.MemberID = res.memberId;
            Single.Web.account.AutoLogin();

            // GAMEPOT 공지사항 사용을 위한 서드파티 로그인
            GamePot.loginByThirdPartySDK(res.memberId);
        }
        , (error) =>
        {
            RefreshUI();
        });
    }

    /// <summary>
    /// 소셜 자동로그인 
    /// </summary>
    public void SocialAutoLogin()
    {
        isLoginDone = false;

        if (!string.IsNullOrEmpty(LocalPlayerData.Method.AccountToken) && LocalPlayerData.ProviderType != 0)
        {
            Single.Web.account.SocialAccountLogin((res) =>
            {
                LocalPlayerData.MemberID = res.memberId;
                Single.Web.account.AutoLogin();
            }
            , (error) =>
             {
                 RefreshUI();
             });
        }
        else
        {
            LocalPlayerData.ResetData();
            PushPanel<Panel_SocialLogin>(false);
        }
    }
    #endregion

    public void RefreshUI(bool isRefresh = true)
    {
        isLoginDone = !isRefresh;
        btn_Login.interactable = isRefresh;
    }
}