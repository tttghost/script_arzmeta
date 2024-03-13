using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Linq;
using System.Linq;
using System;
using UnityEngine.Events;
using FrameWork.UI;
using System.Net;
using CryptoWebRequestSample;

class Item_ManageFriend : FancyScrollRectCell_Custom
{
    #region 변수
    #region 현재 약속된 버튼 스프라이트 - 아이콘 (담당자: 송하정님, 20230407)
    //1. 요청(btn_bg_03 / icon_friendadd_01)
    //2. 요청 취소(btn_bg_03 / icon_friendadd_02)
    //3. 차단(btn_bg_02 / icon_block_01)
    //4. 차단 해제(btn_bg_02 / icon_block_02)
    //5. 수락(btn_bg_03 / icon_check_01)
    //6. 거절(btn_bg_03 / icon_cancel_01)
    //7. 삭제(btn_bg_02 / icon_frienddelete_01)
    #endregion
    [Header("배경 스프라이트")]
    [SerializeField] private Sprite sprite_Red;
    [SerializeField] private Sprite sprite_Yellow;

    [Header("아이콘")]
    [SerializeField] private Sprite icon_Request; // 요청
    [SerializeField] private Sprite icon_CancelRequest; // 요청 취소
    [SerializeField] private Sprite icon_Block; // 차단
    [SerializeField] private Sprite icon_ReleaseBlock; // 차단 해제
    [SerializeField] private Sprite icon_Accept; // 수락
    [SerializeField] private Sprite icon_Refusal; // 거절
    [SerializeField] private Sprite icon_Delete; // 삭제
    [SerializeField] private Sprite icon_Report; // 신고

    private List<GameObject> buttonObjs = new List<GameObject>();
    private List<Button> buttons = new List<Button>();
    private List<Image> images = new List<Image>();
    private List<Image> icons = new List<Image>();
    private List<TMP_Text> texts = new List<TMP_Text>();

    private int objCount;

    private Image img_Profile;
    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_StateMessage;
    private View_FriendList view_FriendList;
    private View_FriendManage view_FriendManage;

    private Item_ManageFriendData data;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Nickname = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_StateMessage = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_StateMessage));
        #endregion

        #region Image
        img_Profile = uIBase.GetUI_Img(nameof(img_Profile));
        #endregion

        #region Button
        uIBase.GetUI_Button("btn_FriendInfo", () =>
        {
            SceneLogic.instance.PopPopup();
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PushPanel<Panel_ArzProfile>().SetPlayerInfo(OTHERINFO_TYPE.MEMBERCODE, data.memberCode);
        });
        #endregion

        #region etc
        GameObject go_Button = uIBase.GetChildGObject(nameof(go_Button));
        if (go_Button != null)
        {
            buttonObjs = go_Button.Children().ToList();
            objCount = buttonObjs.Count;
            for (int i = 0; i < objCount; i++)
            {
                Button button = buttonObjs[i].GetComponent<Button>();
                buttons.Add(button);

                Image image = buttonObjs[i].GetComponent<Image>();
                images.Add(image);

                Image icon = buttonObjs[i].Children().FirstOrDefault(x => x.name.Contains("img")).GetComponent<Image>();
                icons.Add(icon);

                TMP_Text text = buttonObjs[i].GetComponentInChildren<TMP_Text>();
                texts.Add(text);
            }
        }

        Panel_Friend panel_Friend = SceneLogic.instance.GetPanel<Panel_Friend>();
        if (panel_Friend != null)
        {
            view_FriendList = panel_Friend.GetView<View_FriendList>();
            view_FriendManage = panel_Friend.GetView<View_FriendManage>();
        }
        #endregion
    }

    #region

    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_ManageFriendData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    protected override void SetContent()
    {
        if (txtmp_Nickname != null)
        {
            txtmp_Nickname.text = data.nickname;
        }

        if (txtmp_StateMessage != null)
        {
            if (!string.IsNullOrEmpty(data.message))
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, data.message);
            }
            else
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, new MasterLocalData("3066", data.nickname));
            }
        }

        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => { img_Profile.sprite = sprite; });
        }

        SetDataButton();
    }

    /// <summary>
    /// 타입에 따라 켜주기
    /// </summary>
    /// <param name="type"></param>
    private void SetDataButton()
    {
        FRIEND_TYPE type = data.memberCode == LocalPlayerData.MemberCode ? FRIEND_TYPE.NONE : data.type;

        ButtonDatas btnDatas = null;
        switch (type)
        {
            case FRIEND_TYPE.NONE: // 전부 끄기
                {
                    SetActiveButton(0);
                }
                break;
            case FRIEND_TYPE.ADD: // 요청
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                        new ButtonData { local = "friend_request", sprite = sprite_Yellow, icon = icon_Request, action = OnClick_RequestFriend }
                    });
                }
                break;
            case FRIEND_TYPE.BLOCK: // 차단
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                        new ButtonData { local = "fiend_block", sprite = sprite_Red, icon = icon_Block, action = OnClick_BlockFriend }
                    });
                }
                break;
            case FRIEND_TYPE.UNBLOCK: // 차단 해제
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                       new ButtonData { local = "fiend_block_unblock", sprite = sprite_Red, icon = icon_ReleaseBlock, action = OnClick_ReleaseBlockFriend }
                    });
                }
                break;
            case FRIEND_TYPE.RECIVE: // 수락 / 거절 / 차단
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                        new ButtonData { local = "common_accept",sprite = sprite_Yellow, icon = icon_Accept, action = OnClick_AcceptFriend },
                        new ButtonData { local = "common_reject", sprite = sprite_Yellow, icon = icon_Refusal, action = OnClick_RefusalRequestFriend },
                        new ButtonData { local = "common_block", sprite = sprite_Red, icon = icon_Block, action = OnClick_BlockFriend }
                    });
                }
                break;
            case FRIEND_TYPE.REQUEST: // 요청 취소
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                       new ButtonData { local = "freind_request_cancel", sprite = sprite_Yellow, icon = icon_CancelRequest, action = OnClick_CancelRequestFriend }
                    });
                }
                break;
            case FRIEND_TYPE.DELETE: // 삭제
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                       new ButtonData { local = "6012", sprite = sprite_Red, icon = icon_Delete, action = OnClick_DeleteFriend }
                    });
                }
                break;
            case FRIEND_TYPE.REPORT: // 신고
                {
                    btnDatas = new ButtonDatas(new List<ButtonData>
                    {
                       new ButtonData { local = "6021", sprite = sprite_Red, icon = icon_Report, action = OnClick_Report }
                    });
                }
                break;
        }

        if (btnDatas != null)
        {
            UpdataButoon(btnDatas);
        }
    }

    /// <summary>
    /// 버튼에 데이터 세팅
    /// </summary>
    /// <param name="datas"></param>
    private void UpdataButoon(ButtonDatas datas)
    {
        int setTrueCount = datas.SetTrueCount();
        for (int i = 0; i < setTrueCount; i++)
        {
            // 로컬라이징
            if (texts[i] != null)
            {
                Util.SetMasterLocalizing(texts[i], new MasterLocalData(datas.btnDatas[i].local));
            }

            // 스프라이트 변경
            if (images[i] != null)
            {
                images[i].sprite = datas.btnDatas[i].sprite;
            }

            // 아이콘 변경
            if (icons[i] != null)
            {
                icons[i].sprite = datas.btnDatas[i].icon;
            }

            // 버튼 액션 추가
            if (buttons[i] != null)
            {
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(datas.btnDatas[i].action);
            }
        }

        SetActiveButton(setTrueCount);
    }

    /// <summary>
    /// 필요 버튼만 켜주기
    /// </summary>
    /// <param name="setTrueCount"></param>
    private void SetActiveButton(int setTrueCount)
    {
        for (int i = 0; i < objCount; i++)
        {
            bool b = i < setTrueCount;
            buttonObjs[i].SetActive(b);
        }
    }

    #region OnClickAction
    /// <summary>
    /// 친구 요청 
    /// </summary>
    private void OnClick_RequestFriend()
    {
        Single.Web.friend.RequestFriend(data.memberCode, (int)FRIENDREQUEST_TYPE.MEMBERCODE, (res) =>
        {
            OpenToastPopup("5116");
            data.updateAction?.Invoke();
        });
    }

    /// <summary>
    /// 친구 차단 
    /// </summary>
    private void OnClick_BlockFriend()
    {
        OpenBasicPopup("5119", () =>
        {
            Single.Web.friend.BlockFriend(data.memberCode, (res) =>
            {
                OpenToastPopup("common_reception_block");
                data.updateAction?.Invoke();

                UpdateFriendScrollData();
            });
        });
    }

    /// <summary>
    /// 친구 차단 해제 
    /// </summary>
    private void OnClick_ReleaseBlockFriend()
    {
        Single.Web.friend.ReleaseBlockFriend(data.memberCode, (res) =>
        {
            OpenToastPopup("common_reception_block_cancel");
            data.updateAction?.Invoke();
        });
    }

    /// <summary>
    /// 친구 수락 
    /// </summary>
    private void OnClick_AcceptFriend()
    {
        Single.Web.friend.AcceptFriend(data.memberCode, (res) =>
        {
            OpenToastPopup("friend_reception_friend");
            data.updateAction?.Invoke();

            UpdateFriendScrollData();
        });
    }

    /// <summary>
    /// 친구 요청 거절 
    /// </summary>
    private void OnClick_RefusalRequestFriend()
    {
        Single.Web.friend.RefusalRequestFriend(data.memberCode, (res) =>
        {
            OpenToastPopup("friend_reception_request_reject");
            data.updateAction?.Invoke();
        });
    }

    /// <summary>
    /// 친구 요청 취소 
    /// </summary>
    private void OnClick_CancelRequestFriend()
    {
        Single.Web.friend.CancelRequestFriend(data.memberCode, (res) =>
        {
            OpenToastPopup("friend_reception_request_cancel");
            data.updateAction?.Invoke();
        });
    }

    /// <summary>
    /// 친구 삭제 
    /// </summary>
    private void OnClick_DeleteFriend()
    {
        OpenBasicPopup("friend_confirm_delete", () =>
        {
            Single.Web.friend.DeleteFriend(data.memberCode, (res) =>
            {
                OpenToastPopup("friend_reception_delete");
                data.updateAction?.Invoke();
            });
        });
    }

    /// <summary>
    /// 신고하기
    /// </summary>
    private void OnClick_Report()
    {
        Dictionary<string, string> tokenDic = new Dictionary<string, string>
        {
            { "jwtAccessToken", ClsCrypto.EncryptByAES(LocalPlayerData.JwtAccessToken) },
            { "type", Util.EnumInt2String(ARZMETA_HOMEPAGE_TYPE.Report) },
            { "nickname", WebUtility.UrlEncode(data.nickname) },
            { "memberCode", data.memberCode }
        };

        Single.WebView.OnReceiveCallback = (str) =>
        { 
            if (str.Url == "arzmeta://AccuseDone")
            {
                Single.WebView.CloseWebview();
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PopPopup(); // 신고 팝업
            }
        };

        string combineUrl = Single.Web.WebviewUrl + "/login";
        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, combineUrl, tokenDic)));
    }

    /// <summary>
    /// 토스트 팝업
    /// </summary>
    /// <param name="local"></param>
    private void OpenToastPopup(string local)
    {
        uIBase.OpenToast<Toast_Basic>()
        .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData(local, data.nickname)));
    }

    /// <summary>
    /// 베이직 팝업 => 액션 실행
    /// </summary>
    /// <param name="local"></param>
    /// <param name="unityAction"></param>
    private void OpenBasicPopup(string local, UnityAction unityAction)
    {
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData(local, data.nickname)))
            .ChainPopupAction(new PopupAction(() => unityAction?.Invoke()));
    }

    /// <summary>
    /// 친구 목록과 친구 관리 목록 업데이트
    /// </summary>
    private void UpdateFriendScrollData()
    {
        if (view_FriendList != null)
        {
            view_FriendList.Init();
        }

        if (view_FriendManage != null)
        {
            view_FriendManage.Init();
        }
    }

    #endregion
    #endregion
}

#region 버튼에 세팅할 데이터 클래스
/// <summary>
/// 버튼에 세팅할 데이터 하위 클래스
/// </summary>
public class ButtonData
{
    public string local;
    public Sprite sprite;
    public Sprite icon;
    public UnityAction action;
}

/// <summary>
/// 버튼에 세팅할 데이터 상위 클래스
/// </summary>
public class ButtonDatas
{
    public List<ButtonData> btnDatas;

    public ButtonDatas(List<ButtonData> btnDatas)
    {
        this.btnDatas = btnDatas;
    }

    // 켜줄 버튼 갯수 카운트
    public int SetTrueCount()
    {
        int count = 0;
        if (btnDatas != null)
        {
            count = btnDatas.Count;
        }
        return count;
    }
}
#endregion 
