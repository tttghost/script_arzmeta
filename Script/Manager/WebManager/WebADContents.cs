using FrameWork.Network;
using System;
public class WebADContents 
{
    private string AccountServerUrl => Single.Web.AccountServerUrl;
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    public void ADContents(int contentsId, Action<ADContentsRes> aDContentsRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        var packet = new { contentsId };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.ADContents, packet), aDContentsRes, DefaultPacketRes);
    }

    public void GetMoneyInfo(Action<GetMoneyInfoRes> getMoneyInfoRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, AccountServerUrl, WebPacket.GetMoneyInfo, dim: false), getMoneyInfoRes, DefaultPacketRes);
    }
}
