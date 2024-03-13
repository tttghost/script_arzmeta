using FrameWork.Network;
using System;

public class WebShortLink
{
    private string HomepageBackendUrl => Single.Web.HomepageBackendUrl;

    public void CreateRoomShortLink(int linkType, string roomCode, string mobileDynamicLink, string pcDynamicLink, Action<CreateShortLink> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            linkType,
            roomCode,
            mobileDynamicLink,
            pcDynamicLink,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, HomepageBackendUrl, WebPacket.CreateShortLink, packet), _res, _error);
    }
}
