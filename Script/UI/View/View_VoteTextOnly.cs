using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using static EasingFunction;
using System.Linq;

public class View_VoteTextOnly : UIBase
{
    #region 변수
    public float durTime = 1f;
    public Ease ease = Ease.EaseInBack;

    TMP_Text txtmp_Subject;
    TMP_Text txtmp_Term;
    TMP_Text txtmp_Content;
    TMP_Text txtmp_ATitle;
    TMP_Text txtmp_BTitle;
    TMP_Text txtmp_A;
    TMP_Text txtmp_B;

    Slider sld_A;
    Slider sld_B;

    GameObject go_Slider;

    float total, aPrecent, bPrecent;
    int aCount, bCount;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Subject = GetUI_TxtmpMasterLocalizing("txtmp_Subject");
        txtmp_Term = GetUI_TxtmpMasterLocalizing("txtmp_Term");
        txtmp_Content = GetUI_TxtmpMasterLocalizing("txtmp_Content");
        txtmp_ATitle = GetUI_TxtmpMasterLocalizing("txtmp_ATitle");
        txtmp_BTitle = GetUI_TxtmpMasterLocalizing("txtmp_BTitle");
        txtmp_A = GetUI_TxtmpMasterLocalizing("txtmp_A");
        txtmp_B = GetUI_TxtmpMasterLocalizing("txtmp_B");
        #endregion

        #region Slider
        sld_A = GetUI<Slider>("sld_A");
        sld_B = GetUI<Slider>("sld_B");
        #endregion

        #region etc
        go_Slider = GetChildGObject("go_Slider");
        if (go_Slider != null)
            go_Slider.SetActive(false);
        #endregion
    }

    #region VoteTextOnly
    public void SetData(GetVoteInfoPacketRes voteData)
    {
        gameObject.SetActive(true);

        InitData(voteData);
    }

    protected virtual void InitData(GetVoteInfoPacketRes voteData)
    {
        // 제목 넣기
        if (txtmp_Subject != null)
            Util.SetMasterLocalizing(txtmp_Subject, voteData.voteInfo.name);

        // 기간 넣기
        if (txtmp_Term != null)
        {
            string start = Util.ConvertDate(voteData.voteInfo.startedAt);
            string end = Util.ConvertDate(voteData.voteInfo.endedAt);
            Util.SetMasterLocalizing(txtmp_Term, new MasterLocalData("6031", start, end));
        }

        // 내용 넣기
        if (txtmp_Content != null)
            Util.SetMasterLocalizing(txtmp_Content, voteData.voteInfo.question);

        if (go_Slider != null)
        {
            go_Slider.SetActive(false);

            switch ((VOTE_STATE_TYPE)voteData.voteInfo.stateType)
            {
                case VOTE_STATE_TYPE.PROGRESS:
                    if (voteData.voteInfo.isExposingResult == 1 && voteData.isVote == 1)
                        ShowResult(voteData);
                    break;
                case VOTE_STATE_TYPE.COMPLETED:
                    ShowResult(voteData);
                    break;
                default: break;
            }
        }
    }

    /// <summary>
    /// 결과 노출
    /// </summary>
    /// <param name="data"></param>
    void ShowResult(GetVoteInfoPacketRes voteData)
    {
        #region 타이틀
        string aTitle = string.Empty;
        string bTitle = string.Empty;

        switch ((VOTE_ALTER_RES_TYPE)voteData.voteInfo.alterResType)
        {
            case VOTE_ALTER_RES_TYPE.O_X: aTitle = "6027"; bTitle = "6028"; break;
            case VOTE_ALTER_RES_TYPE.YES_NO: aTitle = "6029"; bTitle = "6030"; break;
            default: break;
        }
        if (txtmp_ATitle != null)
            Util.SetMasterLocalizing(txtmp_ATitle, new MasterLocalData(aTitle));
        if (txtmp_BTitle != null)
            Util.SetMasterLocalizing(txtmp_BTitle, new MasterLocalData(bTitle));
        #endregion

        Util.RunCoroutine(Co_Result(voteData));

        go_Slider.SetActive(true);
    }

    #region 결과
    IEnumerator<float> Co_Result(GetVoteInfoPacketRes voteData)
    {
        total = aPrecent = bPrecent = 0f;
        aCount = bCount = 0;

        List<ResultInfo> resultInfoList = voteData.resultInfo.ToList();

        int count = resultInfoList.Count;
        if (count == 0) yield break;
        for (int i = 0; i < count; i++)
        {
            total += resultInfoList[i].count;
        }

        for (int i = 0; i < count; i++)
        {
            switch (i)
            {
                case 0:
                    aCount = resultInfoList[i].count;
                    aPrecent = aCount / total;
                    break;
                case 1:
                    bCount = resultInfoList[i].count;
                    bPrecent = bCount / total;
                    break;
            }
        }

        Function function = GetEasingFunction(ease);
        float curTime = 0f;
        while (curTime < 1f)
        {
            curTime += Time.deltaTime / durTime;
            float _aPrecent = Mathf.Lerp(0, aPrecent, function(0f, 1f, curTime));
            float _bPercent = Mathf.Lerp(0, bPrecent, function(0f, 1f, curTime));
            float _aCount = Mathf.Lerp(0, aCount, function(0f, 1f, curTime));
            float _bCount = Mathf.Lerp(0, bCount, function(0f, 1f, curTime));

            SetUpdateSliderValue(_aPrecent, _bPercent);
            SetUpdateResultValue((int)_aPrecent, (int)_aCount, (int)_bPercent, (int)_bCount, (VOTE_RESULT_TYPE)voteData.voteInfo.resultType);

            yield return Timing.WaitForOneFrame;
        }

        SetUpdateSliderValue(aPrecent, bPrecent);
        SetUpdateResultValue((int)(aPrecent * 100), aCount, (int)(bPrecent * 100), bCount, (VOTE_RESULT_TYPE)voteData.voteInfo.resultType);
    }

    /// <summary>
    /// 결과 텍스트 변경
    /// </summary>
    /// <param name="a"></param>
    /// <param name="aCount"></param>
    /// <param name="b"></param>
    /// <param name="bCount"></param>
    /// <param name="voteResultType"></param>
    void SetUpdateResultValue(int a, int aCount, int b, int bCount, VOTE_RESULT_TYPE voteResultType)
    {
        string aResult = string.Empty;
        string bResult = string.Empty;

        switch (voteResultType)
        {
            case VOTE_RESULT_TYPE.RATE:
                aResult = string.Format("{0}%", a);
                bResult = string.Format("{0}%", b);
                break;
            case VOTE_RESULT_TYPE.COUNT:
                aResult = aCount.ToString();
                bResult = bCount.ToString();
                break;
            case VOTE_RESULT_TYPE.MUTIPLE:
                aResult = string.Format("{0}%({1})", a, aCount);
                bResult = string.Format("{0}%({1})", b, bCount);
                break;
        }

        if (txtmp_A != null)
            Util.SetMasterLocalizing(txtmp_A, aResult);
        if (txtmp_B != null)
            Util.SetMasterLocalizing(txtmp_B, bResult);
    }

    /// <summary>
    /// 슬라이드 값 변경
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    void SetUpdateSliderValue(float a, float b)
    {
        if (sld_A != null)
            sld_A.value = a;
        if (sld_B != null)
            sld_B.value = b;
    }
    #endregion
    #endregion
}
