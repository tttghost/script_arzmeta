using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using FrameWork.Network;
using FrameWork.UI;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Popup_FrameBase : PopupBase
{
    #region 변수
    //기본변수 캐싱
    protected GameObject go_Frame;
    protected GameObject go_Arrow;
    protected Scrollbar scrollbar_ScrollbarVertical;

    protected GameObject go_LoadType;

    protected GameObject go_ImageAlbum;
    protected GameObject go_ImageURL;

    protected GameObject go_LoadError;

    protected TMP_Text txtmp_Title_;

    protected TMP_Text txtmp_AttachType_;
    protected TMP_Text txtmp_AlbumAttach_;
    protected TMP_Text txtmp_URLAttach_;
    
    protected TMP_Text txtmp_LoadError;
    protected TMP_Text txtmp_Target; //핵심텍스트

    protected TMP_Text txtmp_Preview_;
    //protected TMP_Text txtmp_Frame_;
    
    protected TMP_Text txtmp_Upload_;
    
    
    protected Button btn_Back;


    protected Button btn_ImageAlbum;
    protected Button btn_ImageURL;

    protected Button btn_Delete;
    protected Button btn_Upload;
    
    
    protected TMP_InputField input_ImageURL;

    protected TogglePlus togplus_AlbumAttach;
    protected TogglePlus togplus_URLAttach;

    private Sprite _frameSprite;
    protected Sprite frameSprite
    {
        get => _frameSprite;
        set
        {
            img_Frame.sprite = _frameSprite = value;
            Util.ZoomImage_Crop(img_Frame);
            go_Frame.SetActive(value != null);
        }
    }

    protected Image img_Frame; //핵심 이미지

    protected Texture2D tempTex; //임시저장 텍스쳐

    protected FRAMEIMAGEAPPEND_TYPE _fRAMEIMAGEAPPEND_TYPE;
    protected FRAMEIMAGEAPPEND_TYPE fRAMEIMAGEAPPEND_TYPE
    {
        get => _fRAMEIMAGEAPPEND_TYPE;
        set
        {
            _fRAMEIMAGEAPPEND_TYPE = value;

            go_ImageAlbum.gameObject.SetActive(false);
            go_ImageURL.gameObject.SetActive(false);
            switch (_fRAMEIMAGEAPPEND_TYPE)
            {
                case FRAMEIMAGEAPPEND_TYPE.로컬이미지: go_ImageAlbum.gameObject.SetActive(true); break;
                case FRAMEIMAGEAPPEND_TYPE.URL이미지: go_ImageURL.gameObject.SetActive(true); break;
            }
            Util.RefreshLayout(gameObject, nameof(go_LoadType));
        }
    }

    #endregion

    #region 함수

    #region 초기화, 셋데이터
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        go_Frame = GetChildGObject(nameof(go_Frame));
        go_Arrow = GetChildGObject(nameof(go_Arrow));
        scrollbar_ScrollbarVertical = GetUI<Scrollbar>(nameof(scrollbar_ScrollbarVertical));
        scrollbar_ScrollbarVertical?.onValueChanged.AddListener((value) => go_Arrow.SetActive(value > 0.01f));

        go_LoadType = GetChildGObject(nameof(go_LoadType));
        go_ImageAlbum = GetChildGObject(nameof(go_ImageAlbum));
        go_ImageURL = GetChildGObject(nameof(go_ImageURL));
        go_LoadError = GetChildGObject(nameof(go_LoadError));

        txtmp_Title_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title_), new MasterLocalData("myroom_frame_edit"));

        txtmp_AttachType_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AttachType_), new MasterLocalData("myroom_appendtype"));
        txtmp_AlbumAttach_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AlbumAttach_), new MasterLocalData("myroom_appendtype_image"));
        txtmp_URLAttach_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_URLAttach_), new MasterLocalData("myroom_appendtype_url"));

        txtmp_LoadError = GetUI_TxtmpMasterLocalizing(nameof(txtmp_LoadError));

        txtmp_Target = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Target));

        txtmp_Preview_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Preview_), new MasterLocalData("myroom_preview"));
        //txtmp_Frame_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Frame_), new MasterLocalData("common_state_empty"));

        txtmp_Upload_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Upload_), new MasterLocalData("common_save"));


        btn_Back = GetUI_Button(nameof(btn_Back), Back);
        btn_ImageAlbum = GetUI_Button(nameof(btn_ImageAlbum), OnClick_TempDataLoad);
        btn_ImageURL = GetUI_Button(nameof(btn_ImageURL), OnClick_TempDataLoad);
        btn_Delete = GetUI_Button(nameof(btn_Delete), OnClick_TempDataDelete);
        btn_Upload = GetUI_Button(nameof(btn_Upload), OnClick_UploadData);


        input_ImageURL = GetUI_TMPInputField(nameof(input_ImageURL));


        togplus_AlbumAttach = GetUI<TogglePlus>(nameof(togplus_AlbumAttach));
        togplus_AlbumAttach.SetToggleOnAction(() => fRAMEIMAGEAPPEND_TYPE = FRAMEIMAGEAPPEND_TYPE.로컬이미지);
        togplus_URLAttach = GetUI<TogglePlus>(nameof(togplus_URLAttach));
        togplus_URLAttach.SetToggleOnAction(() => fRAMEIMAGEAPPEND_TYPE = FRAMEIMAGEAPPEND_TYPE.URL이미지);

        img_Frame = GetUI_Img(nameof(img_Frame));

    }
    protected override async void OnEnable()
    {
        base.OnEnable();
        await Task.Delay(10);
        Util.RefreshScrollView(gameObject, "sview_Frame");
    }
    protected virtual void OnClick_Back()
    {  
    }

    /// <summary>
    /// 셋 데이터
    /// </summary>
    /// <param name="num"></param>
    //public void SetData(GridItemData gridItemData)
    //{
    //    MyRoomFrameImage tempMyRoomFrameImage = LocalPlayerData.MyRoomFrameImages.SingleOrDefault(x => x.num == gridItemData.num);


    //    if (tempMyRoomFrameImage != null) // 앨범에 저장된 서버데이터이미지 있으면 -> 로컬에 있으면 : 로컬꺼 사용, 로컬에 없으면 : 다운로드
    //    {
    //        LoadFileOnData(tempMyRoomFrameImage);
    //    }
    //    else //앨범에 저장된 이미지가 없다면 -> 새로 저장
    //    {
    //        myRoomFrameImage = new MyRoomFrameImage(gridItemData.itemId, gridItemData.num);

    //        OnClick_TempDataDelete();

    //        togplus_AlbumAttach.SetToggleIsOn(true);

    //        btn_Upload.interactable = false;
    //    }

    //    BackAction_Custom = OnClick_Back;
    //}

    /// <summary>
    /// 서버에 앨범데이터가 무조껀 있기 때문에 로드 한다.
    /// </summary>
    //private async void LoadFileOnData(MyRoomFrameImage myRoomFrameImage)
    //{
    //    this.myRoomFrameImage = myRoomFrameImage.DeepCopy(); //깊은복사
    //    switch ((FRAMEIMAGEAPPEND_TYPE)this.myRoomFrameImage.uploadType)
    //    {
    //        case FRAMEIMAGEAPPEND_TYPE.로컬이미지: togplus_AlbumAttach.SetToggleIsOn(true); break;
    //        case FRAMEIMAGEAPPEND_TYPE.URL이미지: togplus_URLAttach.SetToggleIsOn(true); break;
    //    }

    //    tempTex = await Util.Co_LoadFrame(myRoomFrameImage, true); //첫로드, 없으면서버에서 이미지 다운

    //    OnData();

    //    btn_Upload.interactable = false;
    //}
    #endregion

    #region 텍스쳐 임시저장
    /// <summary>
    /// 데이터 임시저장
    /// </summary>
    protected virtual void OnClick_TempDataLoad()
    {
        //await Co_TempDataLoad();

        ////가져온텍스트가 없다면? or 링크가 이미지가 아니라면?

        //tempTex = await Util.Co_LoadFrame(myRoomFrameImage, false); //더미로 템프이미지 가져오기때문에 저장하지 않음
        //if (tempTex == null) return;
        //OnData();
    }

    /// <summary>
    /// 어떤 타입인지에 따라 type과 imageName 추가
    /// </summary>
    /// <returns></returns>
    //private async UniTask Co_TempDataLoad()
    //{
    //    myRoomFrameImage.uploadType = (int)fRAMEIMAGEAPPEND_TYPE;

    //    //텍스쳐 임시저장
    //    switch (fRAMEIMAGEAPPEND_TYPE)
    //    {
    //        case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
    //            {
    //                myRoomFrameImage.imageName = await Util.Co_FindLocalTexPath();
    //            }
    //            break;//로컬앨범 풀네임 이다. formdata용 path

    //        case FRAMEIMAGEAPPEND_TYPE.URL이미지:
    //            {
    //                myRoomFrameImage.imageName = input_URLLoad.text;
    //            }
    //            break;
    //    }
    //}
    #endregion

    #region 임시텍스쳐 삭제
    /// <summary>
    /// 임시저장 데이터 지우기
    /// </summary>
    protected virtual void OnClick_TempDataDelete()
    {
        //tempTex = null;
        //OnData();
    }
    #endregion

    #region 임시텍스쳐 서버에 저장
    /// <summary>
    /// 서버에 저장하기
    /// </summary>
    protected virtual void OnClick_UploadData()
    {
//        if (tempTex != null)
//        {
//            Texture2D iosTex = null; //ios 이미지 확장자 "heic", "heif", "heix", "hevc" 대응용 tex
//#if UNITY_IOS
//            iosTex = tempTex;
//#endif
//            Single.Web.MyRoomFrameImageUpload(myRoomFrameImage, iosTex,
//               (res) =>
//               {
//                   if (res.error == (int)WEBERROR.NET_E_SUCCESS)
//                   {
//                       LocalPlayerData.MyRoomFrameImages = res.frameImages;
//                       myRoomFrameImage = LocalPlayerData.MyRoomFrameImages.SingleOrDefault(x => x.num == myRoomFrameImage.num).DeepCopy();

//                       SetPanelMyRoomFrame(tempTex);

//                       _ = Util.Co_LoadFrame(myRoomFrameImage, true); //이미 tempTex에 저장되어 있기 때문에 Local에 파일만 저장하기 원래 _ 사용
//                   }
//                   else if (res.error == (int)WEBERROR.NET_E_DB_FAILED || res.error == (int)WEBERROR.NET_E_BAD_IMAGE) //부적절한 이미지 이넘 추가
//                   {
//                       PushPopup<Popup_Basic>()
//                       .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("myroom_error_append_anable")));
//                   }
//               });
//        }
//        else
//        {
//            Single.Web.MyRoomFrameImageDelete(myRoomFrameImage,
//                (res) =>
//                {
//                    LocalPlayerData.MyRoomFrameImages = res.frameImages;

//                    SetPanelMyRoomFrame(tempTex);

//                });
//        }
    }


    /// <summary>
    /// 바뀐이미지 브로드캐스트&팝팝업&토스트메세지
    /// </summary>
    /// <param name="tex"></param>
    protected virtual void SetPanelFrame(Texture2D tex)
    {
        //C_WILDCARD(myRoomFrameImage.num);
        //GetPanel<Panel_MyRoomFrame>().SetSprite(Util.Tex2Sprite(tex));

        //PopPopup();
        //SceneLogic.instance.isUILock = false;

        //GetOpenToast<Toast_Basic>()
        //    .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData("008"), 1.5f));
    }

    /// <summary>
    /// 마이룸 액자 실시간갱신 - 전체
    /// </summary>
    /// <param name="num"></param>
    protected virtual void C_WILDCARD(int num)
    {
        //C_WILDCARD packet = new C_WILDCARD();
        //packet.Code = (int)WILDCARD_TYPE.MYROOM_FRAME;
        //packet.Data = num.ToString();
        //packet.Type = 1;
        //Single.RealTime.Send(packet);
    }

    #endregion

    #region 공통함수
    /// <summary>
    /// 데이터 적용
    /// </summary>
    /// <param name="targetName"></param>
    /// <param name="tempTex"></param>
    protected virtual void OnData_Texture()
    {
        //btn_Upload.interactable = true;
        //input_URLLoad.text = "";
        //go_LoadError.SetActive(false);

        //if (tempTex == null)
        //{
        //    txtmp_Target.text = "";
        //    txtmp_Frame_.gameObject.SetActive(true);
        //    btn_Delete.gameObject.SetActive(false);

        //    img_FrameImage.sprite = null;
        //}
        //else
        //{
        //    txtmp_Target.text = myRoomFrameImage.imageName.Split('/').Last();
        //    if ((FRAMEIMAGEAPPEND_TYPE)myRoomFrameImage.uploadType == FRAMEIMAGEAPPEND_TYPE.로컬이미지)
        //    {
        //        //15메가 넘는지 or 가로나 세로 길이가 4200이 넘는지 체크
        //        int maxMB = 15;
        //        int maxWidthHeight = 4200;
        //        if (await Util.Co_GetFileSize(myRoomFrameImage.imageName, eDisk.MB) > maxMB
        //            || Util.CheckTexWidthHeight(maxWidthHeight, tempTex))
        //        {
        //            btn_Upload.interactable = false;

        //            go_LoadError.SetActive(true);
        //            Util.SetMasterLocalizing(txtmp_LoadError, new MasterLocalData("myroom_info_appendimage", "4200", "15"));
        //            return;
        //        }

        //        //이름 서버에서 타임스탬프 넣은것으로 수정
        //        string[] temp = txtmp_Target.text.Split('_');
        //        if (temp[0].Length == 14) //타임적용된것 서버에서 받은것
        //        {
        //            txtmp_Target.text = string.Join("_", temp.Skip(1));
        //        }
        //    }

        //    txtmp_Frame_.gameObject.SetActive(false);
        //    btn_Delete.gameObject.SetActive(true);

        //    img_FrameImage.sprite = Util.Tex2Sprite(tempTex);
        //}
    }

    protected void SetSprite(Sprite sprite)
    {
        frameSprite = sprite;
    }

    #endregion

    #endregion
}













/// <summary>
/// 업로드실패 (작업해야함)
/// </summary>
//private void OpenPopup_UploadFail()
//{
//    PushPopup<Popup_Basic>()
//        .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, 
//        masterLocalDesc: new MasterLocalData("myroom_error_append_anable"), 
//        masterLocalConfirm: new MasterLocalData("common_ok")));
//}

/// <summary>
/// 이미지 불러오기 테스트
/// </summary>
/// <param name="tex"></param>
//private  void TEST()
//{
//    NativeGallery.GetImageFromGallery(async (path) =>
//    {
//        if (!string.IsNullOrEmpty(path))
//        {
//            Texture2D tex = await NativeGallery.LoadImageAtPathAsync(path,maxSize:256, markTextureNonReadable : false);
//            byte[] bytes = tex.EncodeToPNG();
//            Util.CreateFile(Path.Combine(Application.persistentDataPath, "test", "abc.png"),bytes);
//        }
//    });
//}

/// <summary>
/// 로컬앨범 권한
/// </summary>
/// <returns></returns>
//private bool LocalAlbumPermission()
//{
//    bool permission = true;
//    if (fRAMEIMAGEAPPEND_TYPE == FRAMEIMAGEAPPEND_TYPE.로컬이미지)
//    {
//        NativeGallery.Permission readPermission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read); 
//        switch (readPermission)
//        {
//            case NativeGallery.Permission.ShouldAsk: permission = false; NativeGallery.RequestPermission(NativeGallery.PermissionType.Read); break;
//            case NativeGallery.Permission.Granted: permission = true; break;
//            case NativeGallery.Permission.Denied: permission = false; break;
//        }
//    }
//    return permission;
//}