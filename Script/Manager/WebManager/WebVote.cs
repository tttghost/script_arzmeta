using FrameWork.Network;
using System;

public class WebVote 
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    /// <summary> 
    /// 투표 목록 가져오기
    /// </summary>
    /// <param name="_res"></param>
    public void GetVoteInfo(Action<GetVoteInfoPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.Vote, dim: false), _res, _error);
    }

    /// <summary>
    /// 투표 하기
    /// </summary>
    public void Vote(int voteId, int[] response, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            voteId,
            response
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.Vote, packet), _res, _error);
    }
}
