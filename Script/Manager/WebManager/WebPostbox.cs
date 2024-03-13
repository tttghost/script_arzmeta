using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.Network;
using FrameWork.UI;
using System;

public class WebPostbox
{
    public delegate void HandlerPostboxReceive(InteriorItemInvens interiorItemInvens);
    public HandlerPostboxReceive handlerPostboxReceive;

    /// <summary> 선물함 목록 내려 받기 완료 후 콜백. 뉴 아이콘 갱신용 </summary>
    public Action OnResponseGiftMailList;

    /// <summary>
    /// 우편함 목록 조회
    /// </summary>
    /// <param name="postBoxRes"></param>
    /// <param name="defaultRes"></param>
    public void PostboxReq(Action<PostboxRes> PostboxRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, Single.Web.ContentServerUrl, WebPacket.Postbox, dim: false), (PostboxRes res) =>
        {
            LocalPlayerData.Method.SetPostboxes(res.postboxes);
            PostboxRes?.Invoke(res);
            OnResponseGiftMailList?.Invoke();
        }, DefaultPacketRes);
    }

    /// <summary>
    /// 우편함 개별 수령하기
    /// </summary>
    /// <param name="id"></param>
    /// <param name="PostboxRecieveRes"></param>
    /// <param name="DefaultPacketRes"></param>
    public void PostboxReceiveReq(int id, Action<PostboxRecieveRes> PostboxRecieveRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, Single.Web.ContentServerUrl, WebPacket.PostboxRecieve(id.ToString())), (PostboxRecieveRes res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
                PostboxRecieveRes(res);
            else
                SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(res.error);
        }, DefaultPacketRes);
    }

    /// <summary>
    /// 우편함 전체 수령하기
    /// </summary>
    /// <param name="PostboxReceiveAllRes"></param>
    /// <param name="DefaultPacketRes"></param>
    public void PostboxReceiveAllReq(Action<PostboxReceiveAllRes> PostboxReceiveAllRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, Single.Web.ContentServerUrl, WebPacket.PostboxReceiveAll), (PostboxReceiveAllRes res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
                PostboxReceiveAllRes(res);
            else
                SceneLogic.instance.GetPopup<Popup_Basic>().CheckResponseError(res.error);
        }, DefaultPacketRes);
    }
}