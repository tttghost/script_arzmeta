using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.IO;

public class Item_KTMFProfile : UIBase
{
    #region 변수
    public int ItemNum => data.voteItem.itemNum;

    private TMP_Text txtmp_ItemName;
    private TMP_Text txtmp_Like;
    private Image img_Profile;
    private GameObject go_MyPick;
    private GameObject go_LikeSelect;
    private GameObject go_LikeDeselect;

    private Sprite sprite_ori;

    private bool IsLike => data.voteItem.isLike == 1;

    public Item_KTMFProfileData data { get; private set; }
    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        GetComponent<Button>().onClick.AddListener(() => PushPopup<Popup_KTMFVote>().SetData(data));
        #endregion

        #region TMP_Text
        txtmp_ItemName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemName));
        txtmp_Like = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Like));
        #endregion

        #region Image
        img_Profile = GetUI_Img(nameof(img_Profile));
        if (img_Profile != null)
        {
            sprite_ori = img_Profile.sprite;
        }
        #endregion

        #region etc
        go_MyPick = GetChildGObject(nameof(go_MyPick));
        go_LikeSelect = GetChildGObject(nameof(go_LikeSelect));
        go_LikeDeselect = GetChildGObject(nameof(go_LikeDeselect));
        #endregion
    }

    #region 초기화
    public void SetData(Item_KTMFProfileData _data)
    {
        data = _data;

        SetUI();
    }

    private void SetUI()
    {
        if (txtmp_ItemName != null)
        {
            Util.SetMasterLocalizing(txtmp_ItemName, data.voteItem.name);
        }

        if (img_Profile != null)
        {
            img_Profile.sprite = sprite_ori;
            LocalPlayerData.Method.Load_KTMFProflie(data.voteId, data.voteItem.itemNum, data.voteItem.imageName, (sprite) => img_Profile.sprite = sprite);
        }

        SelectItem(data.isSelect);
        LikeItem();
    }
    #endregion

    #region 
    /// <summary>
    /// 자신이 투표한 아이템 체크 상태 및 데이터 변경
    /// </summary>
    /// <param name="isSelect"></param>
    public void ChangeSelectState(bool isSelect)
    {
        data.isSelect = isSelect;

        SelectItem(data.isSelect);
    }

    /// <summary>
    /// 자신이 좋아요 누른 아이템 상태 및 데이터 변경
    /// </summary>
    /// <param name="likeInfo"></param>
    public void ChangeLikeState(LikeInfo likeInfo)
    {
        data.voteItem.isLike = likeInfo.isLike;
        data.voteItem.likeCount = likeInfo.likeCount;

        LikeItem();
    }

    private void SelectItem(bool isSelect)
    {
        if (go_MyPick != null)
        {
            go_MyPick.SetActive(isSelect);
        }
    }

    private void LikeItem()
    {
        if (txtmp_Like != null)
        {
            Util.SetMasterLocalizing(txtmp_Like, new MasterLocalData("vote_likes", $"<b>{data.voteItem.likeCount}</b>"));
        }

        if (go_LikeSelect != null)
        {
            go_LikeSelect.SetActive(IsLike);
        }

        if (go_LikeDeselect != null)
        {
            go_LikeDeselect.SetActive(!IsLike);
        }
    }
    #endregion
}

public class Item_KTMFProfileData
{
    public int voteId;
    public bool isSelect;
    public VoteItem voteItem;

    public Item_KTMFProfileData(int voteId, bool isSelect, VoteItem voteItem)
    {
        this.voteId = voteId;
        this.isSelect = isSelect;
        this.voteItem = voteItem;
    }
}
