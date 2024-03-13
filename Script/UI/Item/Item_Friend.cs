using FrameWork.UI;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using FrameWork.Network;
using FrameWork.Socket;

class Item_Friend : FancyScrollRectCell_Custom
{
    #region 변수
    private Image img_Profile;
    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_StateMessage;
    private GameObject go_FollowSpawn;
    private GameObject go_Offline;
    private TogglePlus togplus_Fix;

    private Item_FriendData data;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Nickname = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_StateMessage = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_StateMessage));

        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Follow", new MasterLocalData("arztalk_friend_follow"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Spawn", new MasterLocalData("arztalk_friend_bring"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Offline", new MasterLocalData("arztalk_friend_break"));
        #endregion

        #region Button
        uIBase.GetUI_Button("btn_Follow", OnClick_Follow);
        uIBase.GetUI_Button("btn_Spawn", OnClick_Spawn);
        uIBase.GetUI_Button("btn_FriendInfo", () => { SceneLogic.instance.PushPanel<Panel_ArzProfile>().SetPlayerInfo(OTHERINFO_TYPE.MEMBERCODE, data.memberCode); });
        #endregion

        #region Image
        img_Profile = uIBase.GetUI_Img(nameof(img_Profile));
        #endregion

        #region TogglePlus
        togplus_Fix = uIBase.GetUI<TogglePlus>("togplus_Fix");
        if (togplus_Fix != null)
        {
            togplus_Fix.SetToggleAction((b) => ToggleBookmark());
        }
        #endregion

        #region etc
        go_FollowSpawn = uIBase.GetChildGObject(nameof(go_FollowSpawn));
        go_Offline = uIBase.GetChildGObject(nameof(go_Offline));
        #endregion
    }
    #region

    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_FriendData _data)
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

        if (togplus_Fix != null)
        {
            togplus_Fix.SetToggleIsOnWithoutNotify(data.bookmark);
        }

        if (img_Profile != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(data.memberCode, data.avatarInfos, (sprite) => { img_Profile.sprite = sprite; });
        }

        SetActiveOnOffline();
    }

    /// <summary>
    /// 상단 고정 토글 액션
    /// </summary>
    private void ToggleBookmark()
    {
        Single.Web.friend.BookmarkFriend(data.memberCode, (res) => { data.updateAction?.Invoke(); });
    }

    /// <summary>
    /// 사용자가 접속중인지 여부에 따른 오브젝트 비/활성화
    /// </summary>
    private void SetActiveOnOffline()
    {
        if (go_FollowSpawn != null)
        {
            go_FollowSpawn.SetActive(data.isOnline);
        }

        if (go_Offline != null)
        {
            go_Offline.SetActive(!data.isOnline);
        }
    }

    /// <summary>
    /// 상대방이 있는 곳으로 따라가기
    /// </summary>
    private void OnClick_Follow()
    {
        if (string.IsNullOrEmpty(data.memberId)) return;

        Single.Socket.C_FriendFollow(data.memberId);
    }

    /// <summary>
    /// 상대방을 내가 있는 곳으로 불러오기
    /// </summary>
    private void OnClick_Spawn()
    {
        if (string.IsNullOrEmpty(data.memberId)) return;

        Single.Socket.C_FriendBring(data.memberId, data.nickname);
    }
    #endregion
}
