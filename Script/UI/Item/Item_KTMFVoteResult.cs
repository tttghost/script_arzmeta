using FrameWork.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Linq;
using UnityEngine.UI;
using TMPro;
using System.IO;

class Item_KTMFVoteResult : FancyScrollRectCell_Custom
{
    #region 변수
    private enum RankType { Frist = 1, Second = 2, Thrid = 3 }

    private Dictionary<string, Sprite> rankBGList = new Dictionary<string, Sprite>();
    private List<Image> img_numList = new List<Image>();
    private Dictionary<string, Sprite> sprite_numDic = new Dictionary<string, Sprite>();

    private TMP_Text txtmp_like;
    private TMP_Text txtmp_ItemName;
    private TMP_Text txtmp_VotePercentage;
    private Image img_Rank_bg;
    private Image img_Profile;
    private GameObject go_MyPick;
    private Slider sld_VoteProgress;

    private Sprite sprite_ori;

    private Item_KTMFResultData data;

    private View_KTMFVote view_KTMFVote;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_like = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_like));
        txtmp_ItemName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemName));
        txtmp_VotePercentage = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_VotePercentage));
        #endregion

        #region Image
        img_Rank_bg = uIBase.GetUI_Img(nameof(img_Rank_bg));
        img_Profile = uIBase.GetUI_Img(nameof(img_Profile));
        if (img_Profile != null)
        {
            sprite_ori = img_Profile.sprite;
        }
        #endregion

        #region Slider
        sld_VoteProgress = uIBase.GetUI<Slider>(nameof(sld_VoteProgress));
        #endregion

        #region Button
        uIBase.GetUI_Button("btn_KTMFVoteResult", () =>
        {
            Item_KTMFProfile item = view_KTMFVote.profiles.FirstOrDefault(x => x.ItemNum == data.rank.itemNum);
            if (item != null)
            {
                uIBase.PushPopup<Popup_KTMFVote>().SetData(item.data);
            }
        });
        #endregion

        #region etc
        view_KTMFVote = uIBase.GetPanel<Panel_KTMFVote>().GetView<View_KTMFVote>();
        go_MyPick = uIBase.GetChildGObject(nameof(go_MyPick));

        int bgCount = Util.EnumLength<RankType>();
        for (int i = 0; i < bgCount; i++)
        {
            string num = (i + 1).ToString();
            Sprite sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + $"icon_score_0{num}");
            if (sprite != null)
            {
                rankBGList.Add(num, sprite);
            }
        }

        Transform go_RankNum = uIBase.GetChildGObject("go_RankNum").transform;
        int count = go_RankNum.childCount;
        for (int i = 0; i < count; i++)
        {
            img_numList.Add(go_RankNum.GetChild(i).GetComponent<Image>());
        }

        for (int i = 0; i < 10; i++)
        {
            Sprite sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + $"img_ranknum2_{i}");
            if (sprite != null)
            {
                sprite_numDic.Add(i.ToString(), sprite);
            }
        }
        #endregion
    }

    #region 초기화
    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_KTMFResultData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    protected override void SetContent()
    {
        SetRank();

        if (img_Rank_bg != null)
        {
            SetIconBackground(data.rank.rank);
        }

        if (img_Profile != null)
        {
            img_Profile.sprite = sprite_ori;

            LocalPlayerData.Method.Load_KTMFProflie(data.rank.voteId, data.rank.itemNum, data.rank.imageName, (sprite) => img_Profile.sprite = sprite);
        }

        if (go_MyPick != null)
        {
            go_MyPick.SetActive(data.isSelect);
        }

        if (txtmp_ItemName != null)
        {
            Util.SetMasterLocalizing(txtmp_ItemName, data.rank.name);
        }

        if (txtmp_VotePercentage != null)
        {
            Util.SetMasterLocalizing(txtmp_VotePercentage, data.resultLocal);
        }

        if (sld_VoteProgress != null)
        {
            sld_VoteProgress.value = Mathf.Clamp(data.graphRate, 0f, 1f);
        }

        if (txtmp_like != null)
        {
            Util.SetMasterLocalizing(txtmp_like, new MasterLocalData("vote_likes", $"<b>{data.rank.likeCount}</b>"));
        }
    }

    private void SetRank()
    {
        string rankStr = data.rank.rank.ToString();
        for (int i = 0; i < img_numList.Count; i++)
        {
            img_numList[i].gameObject.SetActive(false);
        }

        if (data.rank.voteCount <= 0) return;

        for (int i = 0; i < rankStr.Length; i++)
        {
            img_numList[i].gameObject.SetActive(true);
            img_numList[i].sprite = sprite_numDic[rankStr[i].ToString()];
        }
    }

    private void SetIconBackground(int rank)
    {
        Sprite sprite = null;
        img_Rank_bg.gameObject.SetActive(false);

        if (data.rank.voteCount <= 0) return;

        switch ((RankType)rank)
        {
            case RankType.Frist:
            case RankType.Second:
            case RankType.Thrid: sprite = rankBGList[rank.ToString()]; break;
            default: break;
        }

        if (sprite != null)
        {
            img_Rank_bg.sprite = sprite;
            img_Rank_bg.gameObject.SetActive(true);
        }
    }

    #endregion
}
