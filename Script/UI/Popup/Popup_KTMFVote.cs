using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_KTMFVote : PopupBase
{
    #region 변수
    private Image img_Profile;
    private TMP_Text txtmp_Vote;
    private TMP_Text txtmp_ItemName;
    private TMP_Text txtmp_Description;
    private TMP_Text txtmp_Like;
    private Button btn_Vote;
    private TogglePlus togplus_Like;
    private GameObject go_MyPick;

    private Sprite sprite_ori;

    private Item_KTMFProfileData data;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Vote = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Vote));
        txtmp_ItemName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemName));
        txtmp_Description = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Description));
        txtmp_Like = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Like));

        GetUI_TxtmpMasterLocalizing("txtmp_ShowVideo", new MasterLocalData("vote_title_videoclip"));
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        if (img_Profile != null)
        {
            sprite_ori = img_Profile.sprite;
        }
        #endregion

        #region Button
        btn_Vote = GetUI_Button(nameof(btn_Vote), OnClick_Vote);

        GetUI_Button("btn_ShowVideo", () => PushPopup<Popup_KTMFVideoPlayer>().SetData(data.voteItem.videoUrl));
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion

        togplus_Like = GetUI<TogglePlus>(nameof(togplus_Like));
        if (togplus_Like != null)
        {
            togplus_Like.SetToggleAction((b) => OnValueChanged_Like());
        }

        go_MyPick = GetChildGObject(nameof(go_MyPick));
    }

    #region 초기화
    public void SetData(Item_KTMFProfileData _data)
    {
        data = _data;

        SetUI();
    }

    private void SetUI()
    {
        if (txtmp_Vote != null)
        {
            string local = data.isSelect ? "vote_reception_complete" : "vote_title_vote";
            Util.SetMasterLocalizing(txtmp_Vote, new MasterLocalData(local));
        }

        if (txtmp_ItemName != null)
        {
            Util.SetMasterLocalizing(txtmp_ItemName, data.voteItem.name);
        }

        if (txtmp_Description != null)
        {
            Util.SetMasterLocalizing(txtmp_Description, data.voteItem.description);
        }

        if (img_Profile != null)
        {
            img_Profile.sprite = sprite_ori;

            LocalPlayerData.Method.Load_KTMFProflie(data.voteId, data.voteItem.itemNum, data.voteItem.imageName, (sprite) => img_Profile.sprite = sprite);
        }

        if (btn_Vote != null)
        {
            btn_Vote.interactable = !data.isSelect;
        }

        if (togplus_Like != null)
        {
            togplus_Like.SetToggleIsOnWithoutNotify(data.voteItem.isLike == 1);
        }

        SetLikeUI(data.voteItem.likeCount);

        SetSelectItem(data.isSelect);
    }
    #endregion

    #region 
    /// <summary>
    /// 투표하기
    /// </summary>
    private void OnClick_Vote()
    {
        Single.Web.selectVote.SelectKTMFVote(data.voteId, data.voteItem.itemNum, (res) =>
        {
            GetPanel<Panel_KTMFVote>().GetVoted(res.myVote);

            SetCloseEndCallback(() =>
            {
                SceneLogic.instance.isUILock = false;
                PushPopup<Popup_KTMFVoteDone>().SetData(data);
            });
            SceneLogic.instance.PopPopup();
        });
    }

    private void OnValueChanged_Like()
    {
        Single.Web.selectVote.LikeKTMF(data.voteId, data.voteItem.itemNum, (res) =>
        {
            GetPanel<Panel_KTMFVote>().GetLiked(res.likeInfo);
            SetLikeUI(res.likeInfo.likeCount);
        });
    }

    private void SetLikeUI(int likeCount)
    {
        if (txtmp_Like != null)
        {
            Util.SetMasterLocalizing(txtmp_Like, new MasterLocalData("vote_likes", $"<b>{likeCount}</b>"));
        }
    }

    private void SetSelectItem(bool isSelect)
    {
        if (go_MyPick != null)
        {
            go_MyPick.SetActive(isSelect);
        }
    }
    #endregion
}
