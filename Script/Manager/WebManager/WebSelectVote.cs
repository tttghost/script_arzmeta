using FrameWork.Network;
using System;
public class WebSelectVote 
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    /// <summary>
    /// KTMF 투표 정보 가져오기
    /// </summary>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void GetKTMFVoteInfo(Action<GetKTMFVoteInfoPacketRes> _res, Action<DefaultPacketRes> _error = null, bool isErrorPopup = true)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.KTMFVote, isErrorPopup: isErrorPopup), _res, _error);
    }

    /// <summary>
    /// KTMF 투표하기
    /// </summary>
    /// <param name="voteId"></param>
    /// <param name="itemNum"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void SelectKTMFVote(int voteId, int itemNum, Action<SelectKTMFVotePacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            voteId,
            itemNum
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.KTMFVote, packet, dim: false), _res, _error);
    }

    /// <summary>
    /// KTMF 좋아요
    /// </summary>
    /// <param name="voteId"></param>
    /// <param name="itemNum"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void LikeKTMF(int voteId, int itemNum, Action<LikeKTMFVotePacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            voteId,
            itemNum
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.KTMFLike, packet), _res, _error);
    }

    /// <summary>
    /// KTMF 결과 가져오기
    /// </summary>
    /// <param name="voteId"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void GetKTMFResult(int voteId, Action<GetKTMFResultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.KTMFVoteResult(voteId), dim: false), _res, _error);
    }

    /// <summary>
    /// KTMF 이메일 약관 동의 체크
    /// </summary>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void CheckKTMFEmail(Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null, bool isErrorPopup = true)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.KTMFEmailCheck, dim: false, isErrorPopup: isErrorPopup), _res, _error);
    }
}
