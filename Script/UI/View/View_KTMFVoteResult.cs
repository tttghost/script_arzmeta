using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class View_KTMFVoteResult : UIBase
{
    #region 변수
    private VOTE_RESULT_TYPE resultType;

    private List<Item_KTMFResultData> resultDatas;

    private int fristRankCount = 0;

    private ScrollView_Custom scrollView;

    private GetKTMFVoteInfoPacketRes data;
    #endregion

    protected override void SetMemberUI()
    {
        #region etc
        scrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollView_Custom>();
        #endregion
    }

    #region 초기화
    public void SetData(GetKTMFVoteInfoPacketRes _data) => data = _data;

    protected override void OnEnable() => LoadData();

    /// <summary>
    /// 데이터 가져오기
    /// </summary>
    public void LoadData()
    {
        if (data != null)
        {
            Single.Web.selectVote.GetKTMFResult(data.selectVoteInfo.id, SetUI);
        }
    }

    /// <summary>
    /// 데이터 세팅하기
    /// </summary>
    /// <param name="res"></param>
    private void SetUI(GetKTMFResultPacketRes res)
    {
        resultDatas = new List<Item_KTMFResultData>();

        fristRankCount = res.rank[0].voteCount;
        resultType = (VOTE_RESULT_TYPE)res.resultExposureType;

        int count = res.rank.Length;
        for (int i = 0; i < count; i++)
        {
            resultDatas.Add(CreateScrollItem(res.rank[i]));
        }

        if (resultDatas.Count > 0)
        {
            scrollView.UpdateData(resultDatas.ConvertAll(x => x as Item_Data));
            scrollView.JumpTo(0);
        }
    }

    private Item_KTMFResultData CreateScrollItem(Rank rank)
    {
        return new Item_KTMFResultData(IsSelect(rank.itemNum), LoaclizeVote(resultType, rank.rate, rank.voteCount), CalcGraphRate(rank.voteCount), rank);
    }

    public void LikeItemUpdate(LikeInfo likeInfo)
    {
        Item_KTMFResultData item = resultDatas.FirstOrDefault(x => x.rank.itemNum == likeInfo.itemNum);
        if (item != null)
        {
            item.rank.likeCount = likeInfo.likeCount;
            scrollView.UpdateData(resultDatas.ConvertAll(x => x as Item_Data));
        }
    }
    #endregion

    #region 기능 메소드
    /// <summary>
    /// 투표 개수 표기
    /// 00% || 00표 || 00% / 00표
    /// </summary>
    /// <param name="rate"></param>
    /// <param name="voteCount"></param>
    /// <returns></returns>
    private string LoaclizeVote(VOTE_RESULT_TYPE type, float? rate, int voteCount)
    {
        string rateStr = rate != null ? rate.ToString() : " - ";

        switch (type)
        {
            case VOTE_RESULT_TYPE.RATE: return Util.GetMasterLocalizing("vote_state_percentage", rateStr);
            case VOTE_RESULT_TYPE.COUNT: return Util.GetMasterLocalizing("vote_state_point", voteCount);
            case VOTE_RESULT_TYPE.MUTIPLE: return Util.GetMasterLocalizing("vote_state_percentage_point", rateStr, voteCount);
            default: return null;
        }
    }

    /// <summary>
    /// 자신이 투표한 후보인가 체크
    /// </summary>
    /// <param name="itemNum"></param>
    /// <returns></returns>
    private bool IsSelect(int itemNum)
    {
        MyVote myVote = data.myVote.FirstOrDefault(x => x.itemNum == itemNum);
        return myVote != null;
    }

    private float CalcGraphRate(int voteCount) => voteCount / (float)fristRankCount;
    #endregion
}

public class Item_KTMFResultData : Item_Data
{
    public bool isSelect;
    public string resultLocal;
    public float graphRate;
    public Rank rank;

    public Item_KTMFResultData(bool isSelect, string resultLocal, float graphRate, Rank rank)
    {
        this.isSelect = isSelect;
        this.resultLocal = resultLocal;
        this.graphRate = graphRate;
        this.rank = rank;
    }
}