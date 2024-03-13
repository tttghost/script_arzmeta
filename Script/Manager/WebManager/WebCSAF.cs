using FrameWork.Network;
using System;
using UnityEngine;

public class WebCSAF
{
    private string AccountServerUrl => Single.Web.AccountServerUrl;
    private string ContentServerUrl => Single.Web.ContentServerUrl;


    #region 유학박람회 라이선스
    public void GetLicenseInfo(Action<GetLicenseInfoRes> getLicenseInfoRes, Action<DefaultPacketRes> DefaultPacketRes = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, AccountServerUrl, WebPacket.GetExpoLicenseInfo, dim: false), getLicenseInfoRes, DefaultPacketRes);
    }
    #endregion

    #region 행사 입장

    public void EnterCSAF(Action<DefaultPacketRes> res, bool isErrorPopup = true)
    {
        var data = new
        {

        };
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.EnterCSAF, data, isErrorPopup: isErrorPopup), res, Single.Web.Error_Res);
    }
    #endregion

    #region 부스 생성/조회/편집/삭제
    /// <summary>
    /// 부스 생성
    /// </summary>
    public void CreateCSAFBooth(Texture2D iosTex, CreateCSAFBoothReq req, Action<CreateCSAFBoothRes> res, string image)
    {
        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.POST, ContentServerUrl, WebPacket.CreateCSAFBooth, req, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스목록 조회 (부스 리스트)
    /// </summary>
    public void GetCSAFBooths(Action<GetCSAFBoothsRes> res)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetCSAFBooths, dim: false), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스 편집
    /// </summary>
    public void EditCSAFBooth(int boothId, Texture2D iosTex, EditCSAFBoothsReq req, Action<CreateCSAFBoothRes> res, string image)
    {
        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditCSAFBooth(boothId), req, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스 삭제
    /// </summary>
    public void DeleteCSAFBooth(int boothId, Action<DeleteCSAFBoothRes> res)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.DeleteCSAFBooth(boothId)), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스 항목 조회 (부스 상세 정보)
    /// </summary>
    public void GetCSAFBoothDetail(int boothId, Action<GetCSAFBoothDetailRes> res)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetCSAFBoothDetail(boothId), dim: false), res, Single.Web.Error_Res);
    }

    public void GetCSAFBoothName(string boothName, Action<GetCSAFBoothNameRes> res)
    {
        var query = "?name=" + boothName;

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetCSAFBoothName + query, dim: false), res, Single.Web.Error_Res);
    }

    #endregion

    #region 부스배너 등록/편집/삭제

    /// <summary>
    /// 부스배너 등록
    /// </summary>
    public void CreateCSAFBanner(BannerInfo req, Texture2D iosTex, Action<BannerInfoRes> res, string image)
    {
        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.POST, ContentServerUrl, WebPacket.CreateCSAFBanner, req, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스배너 편집
    /// </summary>
    public void EditCSAFBanner(BannerInfo req, Texture2D iosTex, Action<BannerInfoRes> res, string image)
    {
        //익명타입
        int boothId = req.boothId;
        int bannerId = req.bannerId;
        switch ((FRAMEIMAGEAPPEND_TYPE)req.uploadType)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                {
                    if (image == Cons.ORIGIN)
                    {
                        image = string.Empty;
                        var editReq = new
                        {
                            req.interactionType,
                            req.interactionValue,
                        };
                        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditCSAFBanner(boothId, bannerId), editReq, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
                    }
                    else
                    {
                        var editReq = new
                        {
                            req.uploadType,
                            req.interactionType,
                            req.interactionValue,
                        };
                        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditCSAFBanner(boothId, bannerId), editReq, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
                    }

                }
                break;
            case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                {
                    image = string.Empty;
                    var editReq = new
                    {
                        req.uploadType,
                        req.uploadValue,
                        req.interactionType,
                        req.interactionValue,
                    };
                    Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditCSAFBanner(boothId, bannerId), editReq, thumbnailPath: image, iosTex: iosTex), res, Single.Web.Error_Res);
                }
                break;
        }

    }

    /// <summary>
    /// 부스배너 삭제
    /// </summary>
    public void DeleteCSAFBanner(BannerInfo req, Action<DeleteBannerRes> res)
    {
        int boothId = req.boothId;
        int bannerId = req.bannerId;
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.DeleteCSAFBanner(boothId, bannerId)), res, Single.Web.Error_Res);
    }
    #endregion

    #region 부스스크린 등록/편집/삭제
    /// <summary>
    /// 부스스크린 등록
    /// </summary>
    public void CreateCSAFScreen(ScreenInfo req, Action<ScreenInfoRes> res, string video)
    {
        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.POST, ContentServerUrl, WebPacket.CreateCSAFScreen, req, thumbnailPath: video), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스스크린 편집
    /// </summary>
    public void EditCSAFScreen(ScreenInfo req, Action<ScreenInfoRes> res, string video)
    {
        int boothId = req.boothId;
        int screenId = req.screenId;
        var data = new
        {
            req.uploadType,
            req.uploadValue,
            req.interactionType,
            req.interactionValue,
        };
        Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditCSAFScreen(boothId, screenId), data, thumbnailPath: video), res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 부스스크린 삭제
    /// </summary>
    public void DeleteCSAFScreen(ScreenInfo req, Action<DeleteScreenRes> res)
    {
        int boothId = req.boothId;
        int screenId = req.screenId;
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.DeleteCSAFScreen(boothId, screenId)), res, Single.Web.Error_Res);
    }
    #endregion

    #region 파일함 등록/조회/편집/삭제
    /// <summary>
    /// 파일함 목록 조회
    /// </summary>
    public void GetCSAFFileBoxList(int boothId, Action<GetCSAFFileboxPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetCSAFFileboxList(boothId), dim: false), _res, _error);
    }

    /// <summary>
    /// 파일함 등록
    /// </summary>
    public void RegisterCSAFFileBox(int boothId, int fileBoxType, string fileName, string link, Action<CSAFFilebox> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            boothId,
            fileBoxType,
            fileName,
            link,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.RegisterCSAFFileBox, packet), _res, _error);
    }

    /// <summary>
    /// 파일함 편집
    /// </summary>
    public void EditCSAFFilebox(int boothId, int fileId, int fileBoxType, string fileName, string link, Action<CSAFFilebox> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            fileBoxType,
            fileName,
            link,
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.EditDelCSAFFilebox(boothId, fileId), packet), _res, _error);
    }

    /// <summary>
    /// 파일함 삭제
    /// </summary>
    public void DeleteCSAFFilebox(int boothId, int fileId, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.EditDelCSAFFilebox(boothId, fileId)), _res, _error);
    }
    #endregion
}
