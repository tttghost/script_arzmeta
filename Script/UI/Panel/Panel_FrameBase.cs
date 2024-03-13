using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_FrameBase : PanelBase
{

    protected GameObject go_Frame;
    protected GameObject go_FrameTarget;

    protected Button btn_EditFrameImage;
    protected TMP_Text txtmp_EditFrameImage;
    protected Image img_EditFrameImage;

    protected Button btn_Back;
    protected TMP_Text txtmp_Back;

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



    protected Image img_Frame;

    protected Button btn_MoreInfo;
    protected TMP_Text txtmp_MoreInfo;



    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_Frame = GetChildGObject(nameof(go_Frame));
        go_FrameTarget = GetChildGObject(nameof(go_FrameTarget));

        btn_EditFrameImage = GetUI_Button(nameof(btn_EditFrameImage), OnClick_EditFrameImage);
        txtmp_EditFrameImage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EditFrameImage), new MasterLocalData("office_booth_media_editing"));
        img_EditFrameImage = GetUI_Img(nameof(img_EditFrameImage));

        btn_Back = GetUI_Button(nameof(btn_Back), Back);
        txtmp_Back = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Back));

        img_Frame = GetUI_Img(nameof(img_Frame));

        btn_MoreInfo = GetUI_Button(nameof(btn_MoreInfo), OnClick_MoreInfo);
        btn_MoreInfo.gameObject.SetActive(false);
        txtmp_MoreInfo = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MoreInfo), new MasterLocalData("More Info"));

        SetOwner();
    }

    public FRAME_KIND fRAME_KIND { get; private set; }

    /// <summary>
    /// 프레임 설정
    /// </summary>
    /// <param name="tex"></param>
    public void SetFrame(FRAME_KIND fRAME_KIND)
    {
        this.fRAME_KIND = fRAME_KIND;

        Vector2 frameScale = Vector2.zero;
        switch (fRAME_KIND)
        {
            case FRAME_KIND.b_p_sframea: frameScale = new Vector2(2f, 1.25f); break;
            case FRAME_KIND.b_p_sframeb: frameScale = new Vector2(1.5f, 1.5f); break;
            case FRAME_KIND.b_p_sframec: frameScale = new Vector2(1f, 2f); break;
        }
        go_FrameTarget.transform.GetComponent<RectTransform>().sizeDelta = frameScale * 400f;

    }

    /// <summary>
    /// 미디어 편집
    /// </summary>
    protected virtual void OnClick_EditFrameImage()
    {
        
    }

    /// <summary>
    /// 정보보기
    /// </summary>
    protected virtual void OnClick_MoreInfo()
    {

    }



    //protected virtual void SetData(Sprite sprite)
    //{
    //    SetSprite(sprite);

    //    //go_btn.SetActive(LocalPlayerData.IsMyRoom);
    //    //btn_EditInteraction.gameObject.SetActive(SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_Exposition_Booth);
    //    //moreInfo = SceneLogic.instance.GetSceneType() == SceneName.Scene_Room_MyRoom ? string.Empty : "";
    //}

    public virtual void SetSprite(Sprite sprite)
    {
        frameSprite = sprite;   
    }

    /// <summary>
    /// 주인인지 아닌지 판별
    /// </summary>
    protected virtual void SetOwner()
    {

    }   
}