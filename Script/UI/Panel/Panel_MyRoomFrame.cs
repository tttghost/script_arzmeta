using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_MyRoomFrame : Panel_FrameBase
{
    private GridItemData gridItemData;


    /// <summary>
    /// 미디어 편집
    /// </summary>
    protected override void OnClick_EditFrameImage()
    {
        base.OnClick_EditFrameImage();
        PushPopup<Popup_MyRoomFrame>().SetData(gridItemData);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="tex"></param>
    public void SetData(GridItemData gridItemData)
    {
        this.gridItemData = gridItemData;

        //Vector2 frameScale = Vector3.zero;
        //switch (fRAME_KIND)
        //{
        //    case FRAME_KIND.b_p_sframea: frameScale = new Vector2(1.5f, 1f); break;
        //    case FRAME_KIND.b_p_sframeb: frameScale = new Vector2(1f, 1f); break;
        //}
        //go_Frame.transform.GetComponent<RectTransform>().sizeDelta = frameScale * 800f;
        
        //SetSprite(sprite);

        //btn_EditInteraction.gameObject.SetActive(SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_Exposition_Booth);
        //moreInfo = SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_MyRoom ? string.Empty : "";
    }


    protected override void SetOwner()
    {
        base.SetOwner();
        btn_EditFrameImage.gameObject.SetActive(LocalPlayerData.Method.IsMyRoom);
    }


}


//using FrameWork.UI;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class Panel_MyRoomFrame : PanelBase
//{
//    private GameObject go_Frame;
//    private GameObject go_btn;


//    private Button btn_EditFrameImage; 
//    private TMP_Text txtmp_EditFrameImage;
//    private Image img_EditFrameImage;

//    private Button btn_Back;
//    private TMP_Text txtmp_Back;

//    private Image img_FrameMask;
//    private Image img_FrameImage;

//    private Button btn_EditInteraction;
//    private TMP_Text txtmp_EditInteraction;

//    private Button btn_MoreInfo;
//    private TMP_Text txtmp_MoreInfo;

//    private GridItemData gridItemData;

//    private string _moreInfo;
//    private string moreInfo
//    {
//        get
//        {
//            return _moreInfo;
//        }
//        set
//        {
//            _moreInfo = value;
//            bool urlExist = value != string.Empty;
//            btn_MoreInfo.gameObject.SetActive(urlExist);
//        }
//    }

//    protected override void SetMemberUI()
//    {
//        base.SetMemberUI();

//        go_Frame = GetChildGObject(nameof(go_Frame));
//        go_btn = GetChildGObject(nameof(go_btn));

//        btn_EditFrameImage = GetUI_Button(nameof(btn_EditFrameImage), OnClick_EditFrameImage);
//        txtmp_EditFrameImage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EditFrameImage), new MasterLocalData("office_booth_media_editing"));
//        img_EditFrameImage = GetUI_Img(nameof(img_EditFrameImage));

//        btn_Back = GetUI_Button(nameof(btn_Back), Back);
//        txtmp_Back = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Back));

//        img_FrameMask = GetUI_Img(nameof(img_FrameMask));
//        img_FrameImage = GetUI_Img(nameof(img_FrameImage));

//        btn_EditInteraction = GetUI_Button(nameof(btn_EditInteraction), OnClick_EditInteraction);
//        txtmp_EditInteraction = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EditInteraction), new MasterLocalData("office_booth_interaction_editing"));

//        btn_MoreInfo = GetUI_Button(nameof(btn_MoreInfo), OnClick_MoreInfo);
//        txtmp_MoreInfo = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MoreInfo));
//    }

//    /// <summary>
//    /// 인터렉션 편집
//    /// </summary>
//    private void OnClick_EditInteraction()
//    {
//        PushPopup<Popup_MyRoomInteraction>();
//    }

//    /// <summary>
//    /// 정보보기
//    /// </summary>
//    private void OnClick_MoreInfo()
//    {
//        Single.WebView.WebViewURL(moreInfo, true, false, true);
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="tex"></param>
//    public void SetData(FRAME_KIND fRAME_KIND , Sprite sprite, GridItemData gridItemData)
//    {
//        this.gridItemData = gridItemData;

//        Vector2 frameScale = Vector3.zero;
//        switch (fRAME_KIND)
//        {
//            case FRAME_KIND.b_p_sframea: frameScale = new Vector2(1.5f, 1f); break;
//            case FRAME_KIND.b_p_sframeb: frameScale = new Vector2(1f, 1f); break;
//        }
//        go_Frame.transform.GetComponent<RectTransform>().sizeDelta = frameScale * 800f;
//        SetSprite(sprite);

//        go_btn.SetActive(LocalPlayerData.IsMyRoom);
//        btn_EditInteraction.gameObject.SetActive(SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_Exposition_Booth);
//        moreInfo = SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_MyRoom ? string.Empty : "";
//    }

//    public void SetSprite(Sprite sprite)
//    {
//        img_FrameImage.sprite = sprite;
//        if(sprite == null)
//        {
//            return;
//        }
//        Util.ZoomImage_Crop(img_FrameImage);
//    }

//    private void OnClick_EditFrameImage()
//    {
//        PushPopup<Popup_MyRoomFrame>().SetData(gridItemData);
//    }

//}