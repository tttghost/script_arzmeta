using FrameWork.Network;
using System;
using UnityEngine;

public class WebOffice
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    /// <summary>
    /// 오피스 예약하기
    /// </summary>
    /// <param name="data"></param>
    /// <param name="thumbnailPath"></param>
    /// <param name="roomCode"></param>
    /// <param name="callback"></param>
    public void Office_CreateOfficeReservReq(object data, string thumbnailPath, string roomCode, Texture2D iosTex, Action<Office_CreateOfficeReservRes> callback)
    {
        string subUrl = string.IsNullOrEmpty(roomCode) ? WebPacket.createOfficeReserv : WebPacket.updateOfficeReserv(roomCode);

        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.POST, ContentServerUrl, subUrl, data, thumbnailPath: thumbnailPath, iosTex: iosTex), callback, Single.Web.Error_Res);
    }

    /// <summary>
    /// 오피스 룸 코드 생성 리퀘스트
    /// </summary>
    public void Office_CreateRoomCodeReq(Action<Office_CreateRoomCodeRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.Office_CreateRoomCodeReq, dim: false), _res, _error);
    }

    /// <summary>
    /// 오피스 비밀 번호 확인
    /// </summary>
    public void Office_CheckRoomPasswordReq(string _roomCode, string _password, Action<Office_CheckRoomPassword> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            roomCode = _roomCode,
            password = _password,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.CheckRoomPassword, packet), _res, _error);
    }

    /// <summary>
    /// 오피스 홍보 노출 리스트 
    /// </summary>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void Office_GetIsAdvertisingList(Action<AdvertisingOfficeListRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.Office_GetIsAdvertisingList, dim: false), _res, _error);
    }

    public void GetRequest_OfficeReservation(Action<Office_GetReservationInfo> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetRequest_OfficeReservation, dim: false), _res, _error);
    }

    public void GetRequest_OfficeRoomInfo(string roomCode, Action<OfficeRoomReservationWebInfo> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetRequest_OfficeRoomInfo(roomCode), dim: false), _res, _error);
    }

    public void GetRequest_OfficeInterest(Action<Office_GetReservationWaitInfo> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetRequest_OfficeInterest, dim: false), _res, _error);
    }

    public void Office_CancelReservation(string roomCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.Office_CancelReservation(roomCode)), _res, _error);
    }

    public void Office_CancelReservationWait(string roomCode, Action<DefaultPacketRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.Office_CancelReservationWait(roomCode), dim: false), _res, _error);
    }

    /// <summary>
    /// 오피스 대기하기 
    /// </summary>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void Office_WaitReservationReq(string _roomCode, Action<Office_WaitOfficeReservRes> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            roomCode = _roomCode,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.Office_WaitReservationReq, packet, false), _res, _error);
    }
}
