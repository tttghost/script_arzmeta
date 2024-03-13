using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using Gpm.Ui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class Item_Notice : DynamicItemBase
{
    #region 변수
    public string content; // 임시 콘텐츠 텍스트

    private TMP_Text txtmp_NoticeType;
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Views; 
    private TMP_Text txtmp_Contents;

    private Image img_TitleBG;

    private GameObject go_TopBG;

    private Item_NoticeData data;

    private Color color_Event = Cons.Color_SkyBlue;
    private Color color_Guide = Cons.Color_MintGreen;

    private View_Notice view_Notice;

    [SerializeField] private Color color;
    [SerializeField] private Sprite sprite;
    #endregion

    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Item", OnClick_Select);
        #endregion

        #region TMP_Text
        txtmp_NoticeType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_NoticeType));
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_Views = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Views));
        txtmp_Contents = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Contents));
        #endregion

        #region Image
        img_TitleBG = GetUI_Img(nameof(img_TitleBG));
        #endregion

        #region etc
        go_TopBG = GetUI_Img("img_TopBG").gameObject;

        view_Notice = GameObject.Find("View_Notice").GetComponent<View_Notice>();

        currentSize = GetComponent<RectTransform>().sizeDelta;
        #endregion
    }
    #endregion

    public override void UpdateData(DynamicScrollData scrollData)
    {
        data = (Item_NoticeData)scrollData;

        #region 접을 수 있는 아이템일 시
        ChangeContentSize(data.Size);
        #endregion

        base.UpdateData(scrollData);
    }

    /// <summary>
    /// 프리팹에 데이터 세팅
    /// </summary>
    protected override void SetContent()
    {
        base.SetContent();

        // 우선순위 구분 색 변경, 우선순위 텍스트 변경
        if (txtmp_NoticeType != null)
        {
            string str = default;
            Color color = default;
            switch (data.notcieType)
            {
                // 안내랑 이벤트만 있음
                case 1: str = "9501"; color = color_Guide; break;
                case 2: str = "9502"; color = color_Event; break;
                default: str = "9501"; color = color_Guide; break;
            }
            txtmp_NoticeType.color = color;
            Util.SetMasterLocalizing(txtmp_NoticeType, new MasterLocalData(str));
        }
        // 전체 글일 때 상단고정 글 강조 색상 변경
        if (img_TitleBG != null && go_TopBG != null)
        {
            bool b = !data.isSearch && data.isTopFix;
            go_TopBG.SetActive(b);
        }
        // 제목
        if (txtmp_Title != null)
        {
            Util.SetMasterLocalizing(txtmp_Title, data.subject);
        }
        // 조회수
        if (txtmp_Views != null)
        {
            Util.SetMasterLocalizing(txtmp_Views, data.viewCount.ToString());
        }

        #region 접을 수 있는 아이템일 시
        // 내용
        if (txtmp_Contents != null)
        {
            Util.SetMasterLocalizing(txtmp_Contents, data.content);
        }
        #endregion
    }

    /// <summary>
    /// 웹뷰 켜기
    /// </summary>
    public override void OnClick_Select()
    {
        base.OnClick_Select();

        //Single.Web.GetNewsPost(data.id, (res) =>
        //{
        //    data.content = res.news.content;
        //    Single.WebView.WebViewHTMLLoad(content, "https://www.google.co.kr");
        //});

        //Single.WebView.WebViewHTML(content, "https://www.google.co.kr");

        //SceneLogic.instance.PushPopup(Cons.PopupHorizontal, 
        //    new PopupData(POPUPICON.NONE, string.Empty,
        //    "우리들의 메타버스,\n아즈메타에 오신 여러분을 진심으로 환영합니다.\n다양한 공간을 탐험하며 새로운 인연을 만들어보세요.\n\n문의사항 및 문제 발생 시 help.arzmeta@hancom.com로 메일 부탁드립니다.", BTN_TYPE.OK));
    }
}

