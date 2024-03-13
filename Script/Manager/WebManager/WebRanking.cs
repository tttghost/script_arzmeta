using FrameWork.Network;
using System;
using UnityEngine;

public class WebRanking
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    public void Test_InfinityCode()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AllMyRanking((res) => { });
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AllRanking((res) => { });
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MyRanking((res) => { });
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RecordRanking(0f, (res) => { });
        }
    }

    /// <summary>
    /// 전체랭킹보기
    /// </summary>
    /// <param name="_res"></param>
    public void AllMyRanking(Action<allMyRankingRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.AllMyRanking, dim: false), _res, _error);
    }

    /// <summary>
    /// 전체랭킹보기
    /// </summary>
    /// <param name="_res"></param>
    public void AllRanking(Action<allRankingRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.AllRanking, dim: false), _res, _error);
    }

    /// <summary>
    /// 나의 랭킹 보기
    /// </summary>
    /// <param name="_res"></param>
    public void MyRanking(Action<myRankingRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.MyRanking, dim: false), _res, _error);
    }

    /// <summary>
    /// 랭킹 기록하기
    /// </summary>
    /// <param name="_res"></param>
    public void RecordRanking(float score, Action<allMyRankingRes> _res, Action<DefaultPacketRes> _error = null)
    {
        rankingReq rankingReq = new rankingReq
        {
            score = score,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.RecordRanking, rankingReq), _res, _error);
    }
}
