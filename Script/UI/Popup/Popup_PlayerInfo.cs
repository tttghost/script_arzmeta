using Cysharp.Threading.Tasks;
using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using Protocol;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_PlayerInfo : PopupBase
{
    private enum PERMISSION_TYPE
    {
        ScreenShare,
        VoiceChat,
        VideoChat,
        MessageChat
    }

    #region 변수
    [Header("친구 요청 상태에 따른 아이콘 스프라이트")]
    [SerializeField] private Sprite sprite_Request; // icon_friendadd_01
    [SerializeField] private Sprite sprite_ReleaseRequest; // icon_friendadd_02

    [Header("참가자 구분 아이콘 스프라이트")]
    [SerializeField] private Sprite sprite_Host; // 관리자 icon_office_host_01
    [SerializeField] private Sprite sprite_Manager; // 부관리자 icon_office_manager_01
    [SerializeField] private Sprite sprite_Speaker; // 발표자 icon_office_speaker_01
    [SerializeField] private Sprite sprite_Guest; // 일반참가자 icon_office_guest_01
    [SerializeField] private Sprite sprite_Audience; // 청중 icon_office_audience_01
    [SerializeField] private Sprite sprite_Observer; // 관전자 icon_office_observer_01

    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_RequestFriend;
    private TMP_Text txtmp_Authority;
    private GameObject go_Nomal;
    private GameObject go_OfficeAdmin;
    private GameObject go_RequestFriend;
    private GameObject go_arzTALK;
    private GameObject go_OfficeAdminPanel;
    private Button btn_RequestFriend;
    private Button btn_ShowBizCard;
    private Image img_RequestFriend;
    private Image img_Authority;
    private TogglePlus togplus_ScreenShare;
    private TogglePlus togplus_VoiceChat;
    private TogglePlus togplus_VideoChat;
    private TogglePlus togplus_MessageChat;

    private Member memberInfo;
    private OfficeUserInfo userinfo;

    private Scene_OfficeRoom sceneOfficeRoom;
    private bool isFriendRes = false;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_RequestFriend = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RequestFriend), new MasterLocalData("5113"));
        txtmp_Authority = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Authority));

        GetUI_TxtmpMasterLocalizing("txtmp_arzTALK", new MasterLocalData("arzphone_a:rztalk"));
        GetUI_TxtmpMasterLocalizing("txtmp_ShowProfile", new MasterLocalData("5112"));
        GetUI_TxtmpMasterLocalizing("txtmp_ShowBizCard", new MasterLocalData("common_show_businesscard"));
        GetUI_TxtmpMasterLocalizing("txtmp_OfficeAdminTitle", new MasterLocalData("office_profile_permission"));
        GetUI_TxtmpMasterLocalizing("txtmp_ScreenShareOn", new MasterLocalData("office_profile_screenshare"));
        GetUI_TxtmpMasterLocalizing("txtmp_ScreenShareOff", new MasterLocalData("office_profile_screenshare"));
        GetUI_TxtmpMasterLocalizing("txtmp_VoiceChatOn", new MasterLocalData("office_profile_voicechat"));
        GetUI_TxtmpMasterLocalizing("txtmp_VoiceChatOff", new MasterLocalData("office_profile_voicechat"));
        GetUI_TxtmpMasterLocalizing("txtmp_VideoChatOn", new MasterLocalData("office_profile_videochat"));
        GetUI_TxtmpMasterLocalizing("txtmp_VideoChatOff", new MasterLocalData("office_profile_videochat"));
        GetUI_TxtmpMasterLocalizing("txtmp_MessageChatOn", new MasterLocalData("office_profile_chat"));
        GetUI_TxtmpMasterLocalizing("txtmp_MessageChatOff", new MasterLocalData("office_profile_chat"));
        #endregion

        #region Button
        btn_RequestFriend = GetUI_Button(nameof(btn_RequestFriend), OnClick_RequestFriend);
        btn_ShowBizCard = GetUI_Button(nameof(btn_ShowBizCard), OnClick_ShowBizCard);

        GetUI_Button("btn_ShowProfile", OnClick_ShowProfile);
        GetUI_Button("btn_arzTALK", 준비중입니다);
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion

        #region Image
        img_RequestFriend = GetUI_Img(nameof(img_RequestFriend));
        img_Authority = GetUI_Img(nameof(img_Authority));
        #endregion

        #region TogglePlus
        togplus_ScreenShare = GetUI<TogglePlus>(nameof(togplus_ScreenShare));
        togplus_VoiceChat = GetUI<TogglePlus>(nameof(togplus_VoiceChat));
        togplus_VideoChat = GetUI<TogglePlus>(nameof(togplus_VideoChat));
        togplus_MessageChat = GetUI<TogglePlus>(nameof(togplus_MessageChat));
        #endregion

        #region etc
        go_Nomal = GetChildGObject(nameof(go_Nomal));
        go_OfficeAdmin = GetChildGObject(nameof(go_OfficeAdmin));
        go_OfficeAdminPanel = GetChildGObject(nameof(go_OfficeAdminPanel));
        go_RequestFriend = GetChildGObject(nameof(go_RequestFriend));
        go_arzTALK = GetChildGObject(nameof(go_arzTALK));
        #endregion
    }

    /// <summary>
    /// 임시
    /// </summary>
    private void 준비중입니다()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("40000")));
    }

    #region 초기화
    protected override void Start()
    {
        if (IsOffice())
        {
            sceneOfficeRoom = SceneLogic.instance as Scene_OfficeRoom;

            AddHandler();

            if (IsAdmin())
            {
                if (togplus_ScreenShare != null)
                {
                    togplus_ScreenShare.SetToggleAction((b) => C_OFFICE_SET_PERMISSION(PERMISSION_TYPE.ScreenShare, b));
                }

                if (togplus_VoiceChat != null)
                {
                    togplus_VoiceChat.SetToggleAction((b) => C_OFFICE_SET_PERMISSION(PERMISSION_TYPE.VoiceChat, b));
                }

                if (togplus_VideoChat != null)
                {
                    togplus_VideoChat.SetToggleAction((b) => C_OFFICE_SET_PERMISSION(PERMISSION_TYPE.VideoChat, b));
                }

                if (togplus_MessageChat != null)
                {
                    togplus_MessageChat.SetToggleAction((b) => C_OFFICE_SET_PERMISSION(PERMISSION_TYPE.MessageChat, b));
                }
            }
        }
    }

    protected override void OnEnable()
    {
        SetOpenEndCallback(() => Util.RefreshLayout(gameObject, "PopupRoot"));
    }

    private void OnDestroy()
    {
        if (IsOffice())
        {
            RemoveHandler();
        }
    }
    #endregion

    #region 실시간서버
    private void AddHandler()
    {
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION, this, S_OFFICE_GET_PERMISSION);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this, S_OFFICE_SET_PERMISSION);
    }

    private void RemoveHandler()
    {
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this);
    }

    /// <summary>
    /// 상대의 권한 설정 정보 가져오기 (Send)
    /// </summary>
    /// <param name="memberCode"></param>
    private void C_OFFICE_GET_PERMISSION(string memberCode)
    {
        C_OFFICE_GET_PERMISSION packet = new C_OFFICE_GET_PERMISSION
        {
            ClientId = memberCode
        };

        Single.RealTime.Send(packet);
    }

    /// <summary>
    /// 상대의 권한 설정 정보 가져오기 (Recive)
    /// </summary>
    /// <param name="session"></param>
    /// <param name="packet"></param>
    private void S_OFFICE_GET_PERMISSION(PacketSession session, IMessage packet)
    {
        var message = packet as S_OFFICE_GET_PERMISSION;
        if (message.Permission != null)
        {
            userinfo = message.Permission;

            SetAuthority(userinfo.Authority);

            if (IsAdmin())
            {
                SetToggles();
            }
        }
    }

    /// <summary>
    /// 상대의 권한 설정 변경하기 (Send)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="b"></param>
    private void C_OFFICE_SET_PERMISSION(PERMISSION_TYPE type, bool b)
    {
        switch (type)
        {
            case PERMISSION_TYPE.ScreenShare: userinfo.ScreenPermission = b; break;
            case PERMISSION_TYPE.VoiceChat: userinfo.VoicePermission = b; break;
            case PERMISSION_TYPE.VideoChat: userinfo.VideoPermission = b; break;
            case PERMISSION_TYPE.MessageChat: userinfo.ChatPermission = b; break;
        }

        C_OFFICE_SET_PERMISSION packet = new C_OFFICE_SET_PERMISSION();
        packet.Permissions.Add(userinfo);

        Single.RealTime.Send(packet);
    }

    /// <summary>
    /// 상대의 권한 설정 변경하기 (Recive)
    /// </summary>
    /// <param name="session"></param>
    /// <param name="packet"></param>
    private void S_OFFICE_SET_PERMISSION(PacketSession session, IMessage packet)
    {
        S_OFFICE_SET_PERMISSION message = packet as S_OFFICE_SET_PERMISSION;

        if (message.Code == "TOO_MANY_SCREEN_PERMISSION")
        {
            togplus_ScreenShare.SetToggleIsOn(false);
        }

        Util.UtilOffice.OpenWarningPopup(message.Code);

        DEBUG.LOG("S_OFFICE_SET_PERMISSION = " + message.Code, eColorManager.REALTIME);
    }
    #endregion

    #region UI 세팅
    /// <summary>
    /// 탭한 플레이어 정보 세팅
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="memberInfo"></param>
    public void SetPlayerInfo(OTHERINFO_TYPE type, string memberInfo)
    {
        isInitUI = false;
        Single.Web.others.MemberInfo(type, memberInfo, (res) =>
       {
           this.memberInfo = res.othersMember;

           SetUI();
       });
    }
    public bool isInitUI = false;
    /// <summary>
    /// 전체 UI 설정
    /// </summary>
    private void SetUI()
    {
        SetNickname();
        SetPopupUI();

        if (!IsOffice())
        {
            BlockCheck();
            SetFriendBtnState();
        }
        else
        {
            ShowBizCardCheck();
            C_OFFICE_GET_PERMISSION(memberInfo.memberCode);
        }
        isInitUI = true;
    }

    /// <summary>
    /// 닉네임 세팅
    /// </summary>
    private void SetNickname()
    {
        if (txtmp_Nickname == null) return;

        if (!string.IsNullOrEmpty(memberInfo.nickname))
        {
            Util.SetMasterLocalizing(txtmp_Nickname, memberInfo.nickname);
        }
        else
        {
            Util.SetMasterLocalizing(txtmp_Nickname, new MasterLocalData("013"));
        }
    }

    /// <summary>
    /// 팝업 형태 세팅
    /// </summary>
    private void SetPopupUI()
    {
        go_Nomal.SetActive(false);
        go_OfficeAdmin.SetActive(false);

        if (!IsOffice())
        {
            go_Nomal.SetActive(true);

            Single.Web.friend.GetFriendsToLocal(() =>
            {
                go_arzTALK.SetActive(IsFriend());
                go_RequestFriend.SetActive(!IsFriend());
            });
        }
        else
        {
            go_OfficeAdmin.SetActive(true);
            go_OfficeAdminPanel.SetActive(IsAdmin());
        }
    }

    /// <summary>
    /// 해당 사용자가 차단한 사용자인지 확인 및 버튼 비/활성화
    /// </summary>
    /// <returns></returns>
    private void BlockCheck()
    {
        Single.Web.friend.GetBlockFriendsToLocal(() => btn_RequestFriend.interactable = !LocalPlayerData.Method.IsBlockfriend(memberInfo.memberCode));
    }

    /// <summary>
    /// 친구 버튼 텍스트 세팅
    /// </summary>
    private void SetFriendBtnState()
    {
        Single.Web.friend.GetRequestFriendsToLocal(() =>
        {
            string id = IsRequestFriend() ? "5115" : "5113"; // 요청 취소 / 친구 요청
            Util.SetMasterLocalizing(txtmp_RequestFriend, new MasterLocalData(id));

            ChangeBtnSprite();
        });

        isFriendRes = false;
    }

    /// <summary>
    /// 친구 요청 / 요청 취소 상태에 따른 버튼 아이콘 스프라이트 변경
    /// </summary>
    private void ChangeBtnSprite()
    {
        if (img_RequestFriend == null) return;

        img_RequestFriend.sprite = IsRequestFriend() ? sprite_ReleaseRequest : sprite_Request;
    }

    /// <summary>
    /// 비즈니스 카드 노출을 체크했거나 보여줄 수 있는 명함이 있는지 여부 확인 및 버튼 비/활성화
    /// </summary>
    /// <returns></returns>
    private void ShowBizCardCheck()
    {
        // 차후 오피스 입장 설정 등에서 명함 노출값 조건에 추가
        btn_ShowBizCard.interactable = memberInfo.bizCard != null;
    }
    #endregion

    #region 버튼 리스너
    /// <summary>
    /// 프로필 보기
    /// </summary>
    private void OnClick_ShowProfile()
    {
        SceneLogic.instance.PopPopup();
        SceneLogic.instance.isUILock = false;
        PushPanel<Panel_ArzProfile>().SetPlayerInfo(memberInfo);
    }

    /// <summary>
    /// 비즈니스 명함 보기
    /// </summary>
    private void OnClick_ShowBizCard()
    {
        PushPopup<Popup_ShowBizCard>().SetData(new BizCardData(memberInfo.bizCard, memberInfo.memberCode, memberInfo.avatarInfos));
    }

    /// <summary>
    /// 친구 요청 / 친구 요청 취소
    /// </summary>
    private void OnClick_RequestFriend()
    {
        if (isFriendRes) return;

        isFriendRes = true;

        if (IsRequestFriend())
        {
            // 요청 취소
            Single.Web.friend.CancelRequestFriend(memberInfo.memberCode, (res) => OpenToastPopup("friend_reception_request_cancel"), (error) => isFriendRes = false);
        }
        else
        {
            // 친구 요청
            Single.Web.friend.RequestFriend(memberInfo.memberCode, (int)FRIENDREQUEST_TYPE.MEMBERCODE, (res) => OpenToastPopup("5116"), (error) => isFriendRes = false);
        }
    }

    private void OpenToastPopup(string local)
    {
        SetFriendBtnState();
        OpenToast<Toast_Basic>()
        .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData(local, memberInfo.nickname)));
    }

    /// <summary>
    /// 직급 세팅
    /// </summary>
    /// <param name="Authority"></param>
    private void SetAuthority(int Authority)
    {
        string str = null;
        Sprite sprite = null;

        switch ((OfficeAuthority)Authority)
        {
            case OfficeAuthority.관리자: str = "1025"; sprite = sprite_Host; break;
            case OfficeAuthority.부관리자: str = "1026"; sprite = sprite_Manager; break;
            case OfficeAuthority.일반참가자: str = "1027"; sprite = sprite_Guest; break;
            case OfficeAuthority.발표자: str = "1028"; sprite = sprite_Speaker; break;
            case OfficeAuthority.청중: str = "1029"; sprite = sprite_Audience; break;
            case OfficeAuthority.관전자: str = "1030"; sprite = sprite_Observer; break;
            default: break;
        }

        if (txtmp_Authority != null)
        {
            Util.SetMasterLocalizing(txtmp_Authority, new MasterLocalData(str));
        }

        if (img_Authority != null)
        {
            img_Authority.sprite = sprite;
        }
    }

    /// <summary>
    /// 토글 세팅하기
    /// </summary>
    private void SetToggles()
    {
        // 화면 공유
        if (togplus_ScreenShare != null)
        {
            togplus_ScreenShare.SetToggleIsOnWithoutNotify(userinfo.ScreenPermission);
        }
        // 음성 채팅
        if (togplus_VoiceChat != null)
        {
            togplus_VoiceChat.SetToggleIsOnWithoutNotify(userinfo.VoicePermission);
        }
        // 화상 채팅
        if (togplus_VideoChat != null)
        {
            togplus_VideoChat.SetToggleIsOnWithoutNotify(userinfo.VideoPermission);
        }
        // 문자 채팅
        if (togplus_MessageChat != null)
        {
            togplus_MessageChat.SetToggleIsOnWithoutNotify(userinfo.ChatPermission);
        }
    }
    #endregion

    #region 기능 메소드
    /// <summary>
    /// 친구 요청 여부
    /// </summary>
    /// <returns></returns>
    private bool IsRequestFriend()
    {
        return LocalPlayerData.Method.IsRequestFriend(memberInfo.memberCode);
    }

    /// <summary>
    /// 친구 여부
    /// </summary>
    /// <returns></returns>
    private bool IsFriend()
    {
        return LocalPlayerData.Method.IsFriend(memberInfo.memberCode);
    }

    /// <summary>
    /// 오피스인지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsOffice() => Util.UtilOffice.IsOffice();

    /// <summary>
    /// 내가 오피스에서 방장인지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsAdmin()
    {
        if (!IsOffice()) return false;

        if (sceneOfficeRoom != null)
        {
            return sceneOfficeRoom.IsAdmin();
        }
        return false;
    }

    public string GetClientId()
    {
        return memberInfo.memberCode;
    }
    #endregion
}
