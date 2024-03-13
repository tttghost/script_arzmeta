using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using db;
using FrameWork.UI;
using MEC;
using Office;
using TMPro;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup_BaseRoomCreate : PopupBase
{
    #region 변수

    //go
    protected GameObject go_ThumbnailPreview;

    //txtmp
    protected TMP_Text txtmp_RoomTitle;
    protected TMP_Text txtmp_RoomName;
    protected TMP_Text txtmp_RoomDesc;
    protected TMP_Text txtmp_CreateRoom;

    //input
    protected TMP_InputField input_RoomName;
    protected TMP_InputField input_RoomDesc;

    //img
    private Sprite _spriteThumbnail;
    protected Sprite spriteThumbnail
    {
        get => _spriteThumbnail;
        set
        {
            _spriteThumbnail = value;

            img_ThumbnailPreview.sprite = value;
            go_ThumbnailPreview.SetActive(value != null);
            btn_ThumbnailPreviewClose.gameObject.SetActive(value != null);
            Util.ZoomImage_Crop(img_ThumbnailPreview);
        }
    }
    protected Image img_ThumbnailPreview;

    //btn
    protected Button btn_Exit;
    protected Button btn_ThumbnailLoad;
    protected Button btn_ThumbnailPreviewClose;
    protected Button btn_RemoveRoom;
    protected Button btn_CreateRoom;


    private string _thumbnailPath;
    protected string thumbnailPath
    {
        get => _thumbnailPath;
        set
        {
            _thumbnailPath = value;
            OnThumbnailPath(value);
            IsSameRoom();
        }
    }

    protected Texture2D tempTex; //임시저장 텍스쳐
    #endregion
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_ThumbnailPreview = GetChildGObject(nameof(go_ThumbnailPreview));
        
        txtmp_RoomTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomTitle));//타이틀
        txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
        txtmp_RoomDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomDesc));
        txtmp_CreateRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateRoom));//개설or예약버튼

        input_RoomName = GetUI_TMPInputField(nameof(input_RoomName), (str) => IsSameRoom());
        input_RoomDesc = GetUI_TMPInputField(nameof(input_RoomDesc), (str) => IsSameRoom());
        
        btn_Exit = GetUI_Button(nameof(btn_Exit), Back);//닫기
        btn_ThumbnailLoad = GetUI_Button(nameof(btn_ThumbnailLoad), OnClick_ThumbnailLoad);
        btn_ThumbnailPreviewClose = GetUI_Button(nameof(btn_ThumbnailPreviewClose), OnClick_ThumbnailReset);
            
        btn_CreateRoom = GetUI_Button(nameof(btn_CreateRoom), OnClick_CreateRoom); //방만들기    
        btn_RemoveRoom = GetUI_Button(nameof(btn_RemoveRoom), OnClick_RemoveRoom); //방예약하기   

        img_ThumbnailPreview = GetUI_Img(nameof(img_ThumbnailPreview));

    }
    /// <summary>
    /// 방 만들기 클릭
    /// </summary>
    protected virtual void OnClick_CreateRoom()
    {
    }

    /// <summary>
    /// 룸 생성 리퀘스트
    /// </summary>
    protected virtual void CreateRoom()
    {
    }

    /// <summary>
    /// 룸 예약/수정 리퀘스트
    /// </summary>
    /// 
    protected virtual void ReservOrModifyRoom()
    {
    }

    /// <summary>
    /// 나의예약 취소
    /// </summary>
    protected virtual void OnClick_RemoveRoom()
    {
    }

    /// <summary>
    /// 썸네일미리보기
    /// </summary>
    protected async void OnClick_ThumbnailLoad()
    {
        tempTex = null;
        string instancePath = await Util.Co_FindLocalTexPath();

        if (string.IsNullOrEmpty(instancePath))
        {
            return;
        }

        thumbnailPath = instancePath;

        tempTex = await Util.Co_LoadLocalAsyncTex(thumbnailPath);
        spriteThumbnail = Util.Tex2Sprite(tempTex);

        btn_ThumbnailLoad.gameObject.SetActive(true);
    }

    /// <summary>
    /// 썸네일 초기화
    /// </summary>
    protected virtual void OnClick_ThumbnailReset()
    {
        thumbnailPath = string.Empty;
        spriteThumbnail = null;
    }

    /// <summary>
    /// 같은 방인지 여부 판단 같은지여부 판단
    /// </summary>
    protected virtual void IsSameRoom()
    {

    }

    /// <summary>
    /// 썸네일패스 콜백
    /// </summary>
    /// <param name="value"></param>
    protected virtual void OnThumbnailPath(string value)
    {

    }
}
