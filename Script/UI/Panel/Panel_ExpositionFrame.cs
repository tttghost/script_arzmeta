using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_ExpositionFrame : Panel_FrameBase
{
    private BannerInfo bannerInfo;

    public int GetBannerId()
    {
        return bannerInfo.bannerId;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        btn_EditFrameImage.gameObject.SetActive(LocalContentsData.IsMyBooth());
    }
    public void SetData(BannerInfo bannerInfo)
    {
        this.bannerInfo = bannerInfo;

        //url 정보 더보기 버튼 활성화/비활성화
        btn_MoreInfo.gameObject.SetActive(false);
        if (LocalContentsData.GetBannerInfo(bannerInfo.bannerId) != null && bannerInfo.interactionValue != Cons.EMPTY)
        {
            btn_MoreInfo.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 이미지 편집
    /// </summary>
    protected override void OnClick_EditFrameImage()
    {
        base.OnClick_EditFrameImage();
        PushPopup<Popup_ExpositionFrame>().SetData(bannerInfo, fRAME_KIND);
    }

    /// <summary>
    /// 정보보기
    /// </summary>
    protected override void OnClick_MoreInfo()
    {
        base.OnClick_MoreInfo();

        Single.WebView.OpenWebView(new WebViewData(
            new WebDataSetting(WEBVIEWTYPE.URL, bannerInfo.interactionValue)));
    }
}
