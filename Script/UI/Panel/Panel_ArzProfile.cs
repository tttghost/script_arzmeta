using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrameWork.Network;
using Unity.Linq;
using Lean.Common;
using Lean.Touch;
using MEC;
using System.Collections.Generic;
using CryptoWebRequestSample;
using System.Net;
using FrameWork.Cache;

/// <summary>
/// 인게임에서 자신 혹은 타인 프로필 보기
/// </summary>
public class Panel_ArzProfile : PanelBase
{
    #region 변수
    [Header("친구 요청 상태에 따른 아이콘 스프라이트")]
    [SerializeField] private Sprite sprite_Request; // icon_friendadd_01
    [SerializeField] private Sprite sprite_ReleaseRequest; // icon_friendadd_02

    private Button btn_MyRoom;
    private Button btn_RequestFriend;
    private TMP_Text txtmp_RequestFriend;
    private Transform avatarRig;
    private Image img_RequestFriend;
    private GameObject go_RT;
    private GameObject go_Proflie;
    private GameObject go_Edit;
    private GameObject go_Mine;
    private GameObject go_Other;
    private GameObject go_RequestFriend;
    private GameObject go_arzTALK;
    private RawImage go_Shadow;
    private RawImage go_RawImage;
    private AvatarPartsController controller;

    private Member memberInfo;

    private View_ArzProfile_Edit view_ArzProfile_Edit;

    private bool isEditting = false;
    private bool isFriendRes = false;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_RequestFriend = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RequestFriend));

        GetUI_TxtmpMasterLocalizing("txtmp_ProflieEdit", new MasterLocalData("4021"));
        GetUI_TxtmpMasterLocalizing("txtmp_Block", new MasterLocalData("10007"));
        GetUI_TxtmpMasterLocalizing("txtmp_Report", new MasterLocalData("6021"));
        GetUI_TxtmpMasterLocalizing("txtmp_MyRoom", new MasterLocalData("4022"));
        GetUI_TxtmpMasterLocalizing("txtmp_BizCard", new MasterLocalData("10104"));
        GetUI_TxtmpMasterLocalizing("txtmp_Save", new MasterLocalData("1087"));
        GetUI_TxtmpMasterLocalizing("txtmp_arzTALK", new MasterLocalData("arzphone_a:rztalk"));
        #endregion

        #region Button
        btn_MyRoom = GetUI_Button(nameof(btn_MyRoom), OnClick_MyRoom);
        btn_RequestFriend = GetUI_Button(nameof(btn_RequestFriend), OnClick_Friend);

        GetUI_Button("btn_ProflieEdit", OnClick_ProflieEdit);
        GetUI_Button("btn_Block", OnClick_Block);
        GetUI_Button("btn_Report", OnClick_Report);
        GetUI_Button("btn_arzTALK", 준비중입니다);
        GetUI_Button("btn_BizCard", OnClick_BizCard);
        GetUI_Button("btn_Save", () => view_ArzProfile_Edit.CheckAndSaveData());
        GetUI_Button("btn_Back", OnClick_Back);
        #endregion

        #region Image
        img_RequestFriend = GetUI_Img(nameof(img_RequestFriend));
        go_Shadow = GetChildGObject(nameof(go_Shadow)).GetComponent<RawImage>();
        go_RawImage = GetChildGObject(nameof(go_RawImage)).GetComponent<RawImage>();
        #endregion

        #region etc
        go_RT = GetChildGObject(nameof(go_RT));
        if (go_RT != null)
        {
            controller = go_RT.GetComponentInChildren<AvatarPartsController>();
            avatarRig = go_RT.GetComponentInChildren<LeanManualRotate>().transform;
        }
        go_Proflie = GetChildGObject(nameof(go_Proflie));
        go_Edit = GetChildGObject(nameof(go_Edit));
        go_Mine = GetChildGObject(nameof(go_Mine));
        go_Other = GetChildGObject(nameof(go_Other));
        go_arzTALK = GetChildGObject(nameof(go_arzTALK));
        go_RequestFriend = GetChildGObject(nameof(go_RequestFriend));
        view_ArzProfile_Edit = GetView<View_ArzProfile_Edit>();

        BackAction_Custom = OnClick_Back;
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
        base.Start();

        SetRenderTexture();
    }

    private void SetRenderTexture()
    {
        if (go_RT == null) return;

        Camera renderCam = go_RT.GetComponentInChildren<Camera>();
        renderCam.enabled = false;

        RenderTexture rt = new RenderTexture(350, 700, 24);
        renderCam.targetTexture = rt;

        if (go_Shadow != null)
        {
            go_Shadow.texture = rt;
        }
        if (go_RawImage != null)
        {
            go_RawImage.texture = rt;
        }

        renderCam.enabled = true;
    }

    protected override void OnDisable()
    {
        // 회전값 초기화
        avatarRig.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 닉네임, 멤버ID 세팅
    /// </summary>
    public void SetPlayerInfo(OTHERINFO_TYPE type, string info)
    {
        Single.Web.others.MemberInfo(type, info, (res) => SetPlayerInfo(res.othersMember));
    }

    /// <summary>
    /// 닉네임, 멤버ID 세팅
    /// (인게임에서 플레이어 탭해서 보는 정보)
    /// (공통으로 사용)
    /// </summary>
    public void SetPlayerInfo(Member memberInfo)
    {
        isEditting = false;
        this.memberInfo = memberInfo;

        // UI 세팅
        BlockCheck();
        SetActivePresonalUI();
        SetFriendBtnState();
        SetProflieImage().RunCoroutine();
        SetView();
    }

    /// <summary>
    /// 해당 사용자가 차단한 사용자인지 확인 및 버튼 비/활성화
    /// </summary>
    private void BlockCheck()
    {
        Single.Web.friend.GetBlockFriendsToLocal(() => btn_RequestFriend.interactable = !LocalPlayerData.Method.IsBlockfriend(memberInfo.memberCode));
    }

    /// <summary>
    /// 사용자가 나 혹은 상대방일 시 각 버튼 비/활성화
    /// </summary>
    private void SetActivePresonalUI()
    {
        // 프로필 보기인지 편집인지
        go_Edit.SetActive(isEditting);
        go_Proflie.SetActive(!isEditting);

        if (!isEditting)
        {
            // 내 것인지 남의 것인지
            go_Mine.SetActive(IsMine());
            go_Other.SetActive(!IsMine());

            SetActiveMyRoomBtn();

            if (!IsMine())
            {
                // 친구인지 아닌지
                Single.Web.friend.GetFriendsToLocal(() =>
                {
                    bool b = LocalPlayerData.Method.IsFriend(memberInfo.memberCode);
                    go_RequestFriend.SetActive(!b);
                    go_arzTALK.SetActive(b);
                });
            }
        }
    }

    /// <summary>
    /// 마이룸 버튼 비/활성화
    /// </summary>
    private void SetActiveMyRoomBtn()
    {
        bool isCurScene = false;
        switch (SceneLogic.instance.GetSceneType())
        {
            case SceneName.Scene_Room_MyRoom:
                // 현재 내 위치가 동일인의 마이룸이 아닐 시 활성화
                isCurScene = LocalPlayerData.Method.roomCode != memberInfo.memberCode && IsMyRoomState(); break;
            case SceneName.Scene_Land_Arz:
            case SceneName.Scene_Land_Busan:
            case SceneName.Scene_Zone_Conference:
            case SceneName.Scene_Room_Consulting:
            case SceneName.Scene_Zone_Game:
            case SceneName.Scene_Zone_Store:
            case SceneName.Scene_Zone_Vote:
            case SceneName.Scene_Zone_Festival:
                isCurScene = IsMyRoomState(); break;
            default: break;
        }
        btn_MyRoom.interactable = isCurScene;
    }

    /// <summary>
    /// 해당 사용자의 방 상태가 들어갈 수 있는 상태인지 여부 
    /// </summary>
    /// <returns></returns>
    private bool IsMyRoomState()
    {
        // 내 프로필이면 무조건 활성화
        if (memberInfo.memberCode == LocalPlayerData.MemberCode) return true;

        switch ((MYROOMSTATE_TYPE)memberInfo.myRoomStateType)
        {
            case MYROOMSTATE_TYPE.myroom_condition_anyone: return true;
            default: return false;
        }
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

            ChangeBtnIcon();
        });
        isFriendRes = false;
    }

    /// <summary>
    /// 친구요청 <=> 친구요청취소 아이콘 변경
    /// </summary>
    private void ChangeBtnIcon()
    {
        Sprite spr = IsRequestFriend() ? sprite_ReleaseRequest : sprite_Request;
        if (img_RequestFriend != null)
        {
            img_RequestFriend.sprite = spr;
        }
    }

    /// <summary>
    /// 내가 친구 요청을 보낸 사용자인지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsRequestFriend()
    {
        return LocalPlayerData.Method.IsRequestFriend(memberInfo.memberCode);
    }

    /// <summary>
    /// 아바타 뷰 세팅
    /// </summary>
    private IEnumerator<float> SetProflieImage()
    {
        if (controller != null)
        {
            yield return Timing.WaitUntilTrue(() => controller.gameObject.activeInHierarchy);

            if (memberInfo.avatarInfos != null && memberInfo.avatarInfos.Count != 0)
            {
                controller.SetAvatarParts(memberInfo.avatarInfos);
            }
            else
            {
                controller.SetAvatarParts(Single.ItemData.GetAvatarResetInfo()); // 아바타 리셋
            }
        }
    }

    /// <summary>
    /// 데이터 세팅 및 초기 뷰 켜기
    /// </summary>
    private void SetView()
    {
        ChangeView<View_ArzProfile>().SetData(memberInfo);
    }
    #endregion

    #region 
    /// <summary>
    /// 마이룸으로 가기
    /// </summary>
    private void OnClick_MyRoom()
    {
        MasterLocalData localData = IsMine() ? new MasterLocalData("40010") : new MasterLocalData("5122", memberInfo.nickname);

        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.CHECK, BTN_TYPE.ConfirmCancel, null, localData))
            .ChainPopupAction(new PopupAction(() =>
            {
                RealtimeExceptionHandler.callback_error = () => btn_MyRoom.interactable = false; //임시 익셉션핸들러
                Single.RealTime.EnterRoom(RoomType.MyRoom, memberInfo.memberCode); //원래 팝업이 열려야하지만 안열림. 추후 수정, PopupBase부분, 현규 : Roon으로 되어있는부분도 수정
            }));
    }

    /// <summary>
    /// 친구 요청 / 친구 요청 취소
    /// </summary>
    private void OnClick_Friend()
    {
        if (isFriendRes) return;

        isFriendRes = true;

        if (IsRequestFriend())
        {
            // 요청 취소
            Single.Web.friend.CancelRequestFriend(memberInfo.memberCode, (res) =>
            {
                OpenToastPopup("friend_reception_request_cancel");
            },
            (res) =>
            {
                isFriendRes = false;
            });
        }
        else
        {
            // 친구 요청
            Single.Web.friend.RequestFriend(memberInfo.memberCode, (int)FRIENDREQUEST_TYPE.MEMBERCODE, (res) =>
            {
                OpenToastPopup("5116");
            },
            (res) =>
            {
                isFriendRes = false;
            });
        }
    }

    /// <summary>
    /// 버튼 변경 및 토스트 팝업 실행
    /// </summary>
    /// <param name="local"></param>
    private void OpenToastPopup(string local)
    {
        SetFriendBtnState();
        OpenToast<Toast_Basic>()
        .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData(local, memberInfo.nickname)));
    }

    /// <summary>
    /// 프로필 편집
    /// </summary>
    private void OnClick_ProflieEdit()
    {
        isEditting = true;

        SetActivePresonalUI();
        ChangeView<View_ArzProfile_Edit>().SetData(memberInfo);
    }

    /// <summary>
    /// 차단하기
    /// </summary>
    private void OnClick_Block()
    {
        if (LocalPlayerData.Method.IsBlockfriend(memberInfo.memberCode))
        {
            // 이미 차단한 유저
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("5120")));
        }
        else
        {
            // 차단하시겠습니까?
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("5119", memberInfo.nickname)))
                .ChainPopupAction(new PopupAction(() =>
                {
                    Single.Web.friend.BlockFriend(memberInfo.memberCode, (res) =>
                    {
                        OpenToast<Toast_Basic>()
                        .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("common_reception_block", memberInfo.nickname)));

                        BlockCheck();
                        SetFriendBtnState();
                    });
                }));
        }
    }

    /// <summary>
    /// 신고하기
    /// </summary>
    private void OnClick_Report()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null,new MasterLocalData("5121", memberInfo.nickname)))
            .ChainPopupAction(new PopupAction(() =>
            {
                string combineUrl = Single.Web.WebviewUrl + "/login";
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                     { "jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken) },
                     { "type", Util.EnumInt2String(ARZMETA_HOMEPAGE_TYPE.Report) },
                     { "nickname", WebUtility.UrlEncode(memberInfo.nickname) },
                     { "memberCode", memberInfo.memberCode }
                };
                Single.WebView.OpenWebView(new WebViewData(
                    new WebDataSetting(WEBVIEWTYPE.URL, combineUrl, dic)));
            }));
    }

    /// <summary>
    /// 명함 패널 열기
    /// </summary>
    private void OnClick_BizCard()
    {
        PushPanel<Panel_BizCard>();
    }

    /// <summary>
    /// 뒤로가기
    /// </summary>
    private void OnClick_Back()
    {
        if (isEditting)
        {
            if (!view_ArzProfile_Edit.CompareCheck())
            {
                PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("8001")))
                    .ChainPopupAction(new PopupAction(() => SetOffEditMode()));
            }
            else
            {
                SetOffEditMode();
            }
        }
        else
        {
            SceneLogic.instance.PopPanel();
        }
    }

    /// <summary>
    /// 편집모드 끄기
    /// </summary>
    private void SetOffEditMode()
    {
        isEditting = false;

        memberInfo.nickname = LocalPlayerData.NickName;
        memberInfo.stateMessage = LocalPlayerData.StateMessage;

        SetActivePresonalUI();
        SetView();
    }

    /// <summary>
    /// 프로필이 내 것인지 아닌지 판단
    /// </summary>
    /// <returns></returns>
    private bool IsMine()
    {
        return LocalPlayerData.MemberCode == memberInfo.memberCode;
    }
    #endregion
}
