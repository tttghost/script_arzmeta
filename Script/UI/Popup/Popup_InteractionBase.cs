using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_InteractionBase : PopupBase
{
    private TMP_InputField input_URL;
    private Button btn_DeleteURL;
    private Button btn_SaveURL;
    private Button btn_Back;

    private string tempURL;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        input_URL = GetUI_TMPInputField(nameof(input_URL));
        btn_DeleteURL = GetUI_Button(nameof(btn_DeleteURL), OnClick_DeleteURL);
        btn_SaveURL = GetUI_Button(nameof(btn_SaveURL), OnClick_SaveURL);
        btn_Back = GetUI_Button(nameof(btn_Back), Back);
    }

    public override void Back(int cnt = 1)
    {
        if (input_URL.text != tempURL)
        {
            PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("URL을 저장하지 않고 나가시겠습니까?")))
            .ChainPopupAction(new PopupAction(() => 
            {
                SceneLogic.instance.isUILock = false;
                base.Back(cnt);
            }));
        }
        else
        {
            base.Back(cnt);
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        tempURL = string.Empty; //기존 url 가져옴
    }
    private BannerInfo bannerInfo;

    private Texture2D tex;
    public void SetData(BannerInfo bannerInfo, Texture2D tex)
    {
        this.bannerInfo = bannerInfo;
        this.tex = tex;
    }

    private void OnClick_DeleteURL()
    {
        input_URL.text = string.Empty;
    }
    private void OnClick_SaveURL()
    {
        tempURL = input_URL.text;

        GetPopup<Popup_ExpositionFrame>();

        return;
        //if (input_URL.text != tempURL)
        //{
        //    //url저장로직
        //    Single.Web.EditCSAFBanner
        //}


        //if (LocalContentsData.GetBannerInfo(bannerInfo.bannerId) == null) //등록
        //{
        //    Single.Web.CreateCSAFBanner(bannerInfo, tex, OnCSAFBanner_CreateEdit, image);
        //}
        //else //수정
        //{
        //    Single.Web.EditCSAFBanner(bannerInfo, tex, OnCSAFBanner_CreateEdit, image);
        //}

        //case 1 : 인터렉션 편집이 미디어 편집 안쪽으로 들어갈 수 있을까요?
        //case 2 : 인터렉션 편집만 하는 API 를 만들어 주실 수 있을까요?
    }

}

