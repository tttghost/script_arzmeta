using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_EditScreen : PopupBase
{
    private TMP_InputField input_Url;
    private Button btn_Save;
    private string video;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        input_Url = GetUI<TMP_InputField>(nameof(input_Url));
        btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);
    }


    private void OnClick_Save()
    {
        if(video == string.Empty)
        {
            return;
        }

        Scene_Room_Exposition_Booth Scene_Room_Exposition_Booth = SceneLogic.instance as Scene_Room_Exposition_Booth;
        Scene_Room_Exposition_Booth.GetBoothDetail(SetScreenData);

        ///여기서부터!!! 
        ///부스 스크린 편집 기능
        ///case 1 : 비디오를 올려서 하는 부분 R&D  (3~7일)
        ///case 2 : 그냥 일반 youtube-url을 넣는다. (1~3일)
        if (LocalContentsData.GetBannerInfo(screenInfo.screenId) == null) //등록
        {
            Single.Web.CSAF.CreateCSAFScreen(screenInfo, OnCSAFScreen_CreateEdit, video);
        }
        else //수정
        {
            Single.Web.CSAF.EditCSAFScreen(screenInfo, OnCSAFScreen_CreateEdit, video);
        }

    }


    /// <summary>
    /// 박람회부스 배너 생성,수정 콜백
    /// </summary>
    /// <param name="res"></param>
    private void OnCSAFScreen_CreateEdit(ScreenInfoRes res)
    {
        switch ((WEBERROR)res.error)
        {
            case WEBERROR.NET_E_SUCCESS:
                //tempTex = await Util.Co_LoadExpositionFrame(screenInfo, true);
                //SetPanelFrame(tempTex);
                break;
            case WEBERROR.NET_E_DB_FAILED:
            case WEBERROR.NET_E_BAD_IMAGE:
                PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_error_append_anable")));
                break;
        }
    }

    /// <summary>
    /// 박람회부스 배너 삭제 콜백
    /// </summary>
    /// <param name="res"></param>
    private void OnCSAFScreen_Delete(DeleteBannerRes res)
    {
        switch ((WEBERROR)res.error)
        {
            case WEBERROR.NET_E_SUCCESS:
                //SetPanelFrame(tempTex);
                break;
            case WEBERROR.NET_E_DB_FAILED:
            case WEBERROR.NET_E_BAD_IMAGE:
                PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_error_append_anable")));
                break;
        }
    }





    public int screenId;
    private ScreenInfo screenInfo;

    public void SetData(int screenId)
    {
        this.screenId = screenId;
        SetScreenData();
    }

    /// <summary>
    /// 박람회부스 스크린데이터 셋업
    /// </summary>
    private void SetScreenData()
    {

        screenInfo = LocalContentsData.GetScreenInfo(screenId);

        if (screenInfo == null)
        {
            input_Url.text = string.Empty;
            screenInfo = LocalContentsData.CreateScreenInfo(screenId);
        }
        else
        {
             input_Url.text = screenInfo.uploadValue;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Video();
        }
    }
    private async void Video()
    {
        video = await Util.Co_FindLocalVideoPath();
        input_Url.text = video;
        DEBUG.LOG("result : " + video);
    }
}
