using CryptoWebRequestSample;
using Cysharp.Threading.Tasks;
using FrameWork.Network;
using Gpm.Ui.Sample;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

public class WebAccount
{
    private string AccountServerUrl => Single.Web.AccountServerUrl;

    /// <summary>
    /// 계정 생성
    /// 아즈메타 계정 생성 시에만 패스워드 필수, 이외에는 필요 없음
    /// </summary>
    public void CreateAccount(string accountToken, string password = null, Action<CreateAccountLoginPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            accountToken = ClsCrypto.EncryptByAES(accountToken),
            password = ClsCrypto.EncryptByAES(password),
            regPathType = (int)Single.Web.GetRegPathType(),
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.CreateAccount, packet), _res, _error);
    }

    /// <summary>
    /// 로그인
    /// 아즈메타 계정 로그인 시에만 패스워드 필수, 이외에는 필요 없음
    /// </summary>
    public void Login(string accountToken, string password = null, Action<LoginPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            accountToken = ClsCrypto.EncryptByAES(accountToken),
            password = ClsCrypto.EncryptByAES(password),
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.Login, packet), _res, _error);
    }

    /// <summary>
    /// 소셜 로그인 및 계정 생성
    /// </summary>
    public void SocialAccountLogin(Action<SocialAccountLoginPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        SocialAccountLoginPacketReq packet = new SocialAccountLoginPacketReq()
        {
            accountToken = ClsCrypto.EncryptByAES(LocalPlayerData.Method.AccountToken),
            providerType = LocalPlayerData.ProviderType,
            regPathType = (int)Single.Web.GetRegPathType(),
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.SocialAccountLogin, packet), _res, _error);
    }

    /// <summary>
    /// 계정 연동
    /// </summary>
    public void LinkedAccount(string accountToken, string password, int providerType, Action<CurrentAccountPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        LinkedAccountPacketReq packet = new LinkedAccountPacketReq()
        {
            accountToken = ClsCrypto.EncryptByAES(accountToken),
            providerType = providerType,
            password = ClsCrypto.EncryptByAES(password)
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.LinkedAccount, packet), _res, _error);
    }

    /// <summary>
    /// 계정 연동 해제
    /// </summary>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void ReleaseLinkedAccount(int providerType, Action<ReleaseLinkedAccountPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, AccountServerUrl, WebPacket.ReleaseLinkedAccount(providerType)), _res, _error);
    }

    /// <summary>
    /// 로그인 인증
    /// 로그인 시 얻은 로그인 토큰으로 유효성 검증
    /// </summary>
    public void LoginAuth(Action<LoginAuthPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Scene.SetDimOn();

        var packet = new
        {
            loginToken = ClsCrypto.EncryptByAES(LocalPlayerData.LoginToken)
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.LoginAuth, packet, false), _res, (error) =>
       {
           _error?.Invoke(error);
           Single.Scene.SetDimOff(1f);
       });
    }

    /// <summary>
    /// 자동 로그인
    /// </summary>
    public void AutoLogin(Action<LoginPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            memberId = ClsCrypto.EncryptByAES(LocalPlayerData.MemberID),
            providerType = LocalPlayerData.ProviderType
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.AutoLogin, packet, false), _res, _error);
    }

    #region 로그인 데이터 세팅
    /// <summary>
    /// 자동로그인
    /// </summary>
    public void AutoLogin()
    {
        AutoLogin((res) =>
        {
            SetLocalPlayerData(res);
        },

        (res) =>
        {
            if (LocalPlayerData.Method.DormentAccount(res.error)) return;

            LocalPlayerData.ResetData();

            Single.WebView.CleanDataAndCookie();

            Single.Scene.SetDimOff();
        });
    }

    /// <summary>
    /// 로컬플레이어데이터 기본 셋팅
    /// </summary>
    /// <param name="res"></param>
    public async void SetLocalPlayerData(LoginPacketRes res) => await Co_SetLocalPlayerData(res);

    private bool isAppInfoDone, isMemberInfo;
    async Task Co_SetLocalPlayerData(LoginPacketRes res)
    {
        isAppInfoDone = isMemberInfo = false;

        LocalPlayerData.SetMemberData(res.memberInfo);

        // 로그인 후 정보 가져오기
        Single.Web.member.GetAppInfo((str) =>
        {
            var appInfoRes = new GetAppInfoPacketRes();
            JObject jObject = (JObject)JsonConvert.DeserializeObject(str);

            foreach (var x in jObject)
            {
                eAppInfo eAppInfo = Util.String2Enum<eAppInfo>(x.Key);
                switch (eAppInfo)
                {
                    case eAppInfo.onfContentsInfo:
                        appInfoRes.onfContentsInfo = JsonConvert.DeserializeObject<OnfContentsInfo[]>(x.Value.ToString());
                        break;
                    case eAppInfo.bannerInfo:
                        appInfoRes.bannerInfo = JsonConvert.DeserializeObject<_BannerInfo[]>(x.Value.ToString(), new CustomJsonConverter_Banner());
                        break;
                    case eAppInfo.screenInfo:
                        appInfoRes.screenInfo = JsonConvert.DeserializeObject<_ScreenInfo[]>(x.Value.ToString(), new CustomJsonConverter_Screen());
                        break;
                    case eAppInfo.csafEventInfo:
                        appInfoRes.csafEventInfo = JsonConvert.DeserializeObject<CSAFEventInfo>(x.Value.ToString());
                        break;
                    case eAppInfo.noticeInfo:
                        appInfoRes.noticeInfo = JsonConvert.DeserializeObject<NoticeInfo[]>(x.Value.ToString());
                        break;
                    default: break;
                }
            }
            LocalPlayerData.SetAppInfo(appInfoRes);

            isAppInfoDone = true;
        });
        Single.Web.member.GetMemberInfo((memberInfoRes) =>
        {
            LocalPlayerData.SetMemberInfo(memberInfoRes);

            isMemberInfo = true;
        });

        await UniTask.WaitUntil(() => isAppInfoDone && isMemberInfo);

        SuccessLogin();
    }

    /// <summary>
    /// 데이터 정상 리스폰스 후 처리
    /// </summary>
    private void SuccessLogin()
    {
        //하트비트 (중복 로그인 체크용, 한 번 실행 시 1분 루프)
        Single.Web.member.CheckHeartBeat();

        // 웹소켓 연결
        Single.Socket.SocketIO3Connect();
        AppGlobalSettings.Instance.LoadData();

        // 입장공지 이니셜라이즈(1회성보기위해)
        Util.InitNoticeMember();

        // 첫 로그인, 첫 씬에서만 1:1 채팅 방법 안내 문구 추가
        if (LocalPlayerData.Method.IsFirst) SetChatNotice();

        LocalPlayerData.Method.IsLogined = true;

        SetEnter();
    }

    /// <summary>
    /// 씬 입장
    /// </summary>
    public void SetEnter()
    {
        if (string.IsNullOrEmpty(LocalPlayerData.NickName))
        {
            Single.Scene.FadeOut(1f, () => Single.Scene.LoadScene(SceneName.Scene_Base_Lobby));
        }
        else
        {
            RealtimeWebManager.GetLogin();
            RealtimeWebManager.Run<LoginRes>((_loginRes) =>
            {
                Single.RealTime.sessionId = _loginRes.sessionId;
                Single.RealTime.EnterRoom(RoomType.MyRoom, LocalPlayerData.MemberCode);
            });
        }
    }

    /// <summary>
    /// 1:1 채팅 방법 안내 문구 출력
    /// 최초 로그인 시, 전체 채팅과 1:1 채팅창에서 최상단에 한번만 출력함
    /// </summary>
    private void SetChatNotice()
    {
        // 서버 에러로 인해 여러번 값이 추가되는 경우 방지
        if (LocalPlayerData.Method.allChat.Count > 0 || LocalPlayerData.Method.dmChat.Count > 0) return;

        // 자식 클래스인 Item_S_ChatDM을 통해
        Item_S_ChatDM item_Notice = new Item_S_ChatDM
        {
            message = "chat_notice_1on1",
            color = Cons.ChatColor_White,
        };

        // 부모클래스인 Item_ChatDefault에 값 추가
        LocalPlayerData.Method.allChat.Add(item_Notice);
        LocalPlayerData.Method.dmChat.Add(item_Notice);
    }
    #endregion

    /// <summary>
    /// 이메일 인증번호 받기
    /// </summary>
    public void AuthEmail(string email, Action<CheckEmailPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        CheckEmailPacketReq packet = new CheckEmailPacketReq()
        {
            email = email
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.AuthEmail, packet), _res, _error);
    }

    /// <summary>
    /// 이메일 인증 확인
    /// </summary>
    public void ConfirmEmail(string email, int authCode, Action<AuthEmailPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        AuthEmailPacketReq packet = new AuthEmailPacketReq()
        {
            email = email,
            authCode = authCode
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.ConfirmEmail, packet), _res, _error);
    }

    /// <summary>
    /// 패스워드 재설정
    /// </summary>
    public void ResetPassword(string email, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        CheckEmailPacketReq packet = new CheckEmailPacketReq()
        {
            email = email
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.ResetPassword, packet), _res, _error);
    }

    /// <summary>
    /// 아즈메타 계정 여부 확인
    /// </summary>
    /// <param name="accountToken"></param>
    /// <param name="password"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void CheckArzmetaAccount(string accountToken, string password, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            accountToken = ClsCrypto.EncryptByAES(accountToken),
            password = ClsCrypto.EncryptByAES(password),
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, AccountServerUrl, WebPacket.CheckArzmetaAccount, packet), _res, _error);
    }
}
