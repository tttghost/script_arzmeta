using FrameWork.Network;
using System;

public class WebOthers 
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    /// <summary>
    /// 타인의 정보 가져오기 1: memberId, 2: memberCode
    /// </summary>
    public void MemberInfo(OTHERINFO_TYPE type, string otherMemberInfo, Action<MemberInfoPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.MemberInfo(type, otherMemberInfo), dim: false), _res, _error);
    }
}
