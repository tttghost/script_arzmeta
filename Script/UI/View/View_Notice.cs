using FrameWork.UI;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Vuplex.WebView;

/// <summary>
/// 사용 안 함 - 보류
/// </summary>
public class View_Notice : DynamicScrollerBase_View
{
    #region 변수
    private List<Item_NoticeData> _data = new List<Item_NoticeData>();

    private TMP_InputField input_Search;

    private TMP_Dropdown dropdown;

    private bool isSearching = false;
    private NEWSSEARCH_TYPE curType;
    private string curKeyword;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Search", OnClick_Search);
        GetUI_Button("btn_Reset", OnClick_Reset);
        #endregion

        #region InputField
        input_Search = GetUI_TMPInputField(nameof(input_Search));
        #endregion

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Search", new MasterLocalData("6010"));
        #endregion

        #region etc
        dropdown = GetChildGObject("go_Dropdown").GetComponent<TMP_Dropdown>();

        if (scroll != null)
        {
            scroll.AddSelectCallback((data) =>
            {
                int selectDataIndex = scroll.GetDataIndex(data);
                SetExpanded(data);
                scroll.UpdateAllData();
            });
        }
        #endregion
    }

    /// <summary>
    /// 펼쳐지는 아이템 세팅
    /// </summary>
    /// <param name="data"></param>
    private void SetExpanded(DynamicScrollData data = null)
    {
        if (GetDataCount() == 0) return;

        int count = GetDataCount();
        for (int i = 0; i < count; i++)
        {
            bool isSelect = _data[i] == data ? !_data[i].isExpanded : false;
            _data[i].isExpanded = isSelect;
        }
    }

    #region 초기화
    protected override void OnEnable()
    {
        dropdown.value = 0;
        input_Search.text = string.Empty;
        curType = NEWSSEARCH_TYPE.None;
        curKeyword = null;

        Clear();
        InitData();
    }

    public void InitData(int lastId = 0)
    {
        Util.RunCoroutine(Co_InitData(lastId), "InitData_Notice");
    }

    private IEnumerator<float> Co_InitData(int lastId = 0)
    {
        yield return Timing.WaitForOneFrame;
        LoadData(lastId);
    }

    /// <summary>
    /// 서버에서 데이터 받아오기 및 높이 설정
    /// </summary>
    protected override void LoadData(int lastId = 0)
    {
        base.LoadData(lastId);

        // 서버에서 전체 데이터 받아오기
        //Single.Web.GetNews(lastId, curType, curKeyword, (res) =>
        //{
        //    int count = res.news.Length;
        //    if (count == 0)
        //    {
        //        if (!string.IsNullOrEmpty(curKeyword) && scroll.GetDataCount() == 0)
        //        {
        //            SceneLogic.instance.PushPopup(Cons.PopupHorizontal,
        //                new PopupData(POPUPICON.WARNING, string.Empty, new LocalData(Cons.Local_Arzmeta, "6014"), BTN_TYPE.OK),
        //                new PopupAction(OnClick_Reset));
        //        }
        //        return;
        //    }
        //    for (int i = 0; i < count; i++)
        //    {
        //        // 데이터 편집
        //        NoticesItem item = new NoticesItem();
        //        item.id = res.news[i].id;
        //        item.notcieType = res.news[i].NewsType.type;
        //        item.subject = res.news[i].subject;
        //        item.isTopFix = res.news[i].isTopFix == 1;
        //        item.isSearch = curType != NEWSSEARCHTYPE.None;
        //        item.viewCount = res.news[i].viewCount;
        //        item.createdAt = res.news[i].createdAt;
        //        item.updatedAt = res.news[i].updatedAt;

        //        _data.Add(item);
        //        InsertData(item);
        //    }
        //},
        //(error, res) =>
        //{
        //    if (SceneLogic.instance.GetPopup(Cons.PopupHorizontal).CheckResponseError(res))
        //        return;
        //});

        #region 더미 데이터
        Item_NoticeData item = new Item_NoticeData();
        item.id = 0;
        item.notcieType = 1;
        item.subject = "아즈메타에 오신 걸 환영합니다!";
        item.content = "우리들의 메타버스, 아즈메타에 오신 여러분을 진심으로 환영합니다.\n다양한 공간을 탐험하며 새로운 인연을 만들어보세요.\n\n문의사항 및 문제 발생 시 help.arzmeta@hancom.com로 메일 부탁드립니다.";
        item.isTopFix = false;
        item.isSearch = curType != NEWSSEARCH_TYPE.None;
        item.viewCount = 0;
        item.createdAt = "0";
        item.updatedAt = "0";

        #region 접을 수 있는 아이템일 시
        item.isExpanded = false;
        item.collapsedSize = curHeight;
        item.expandedSize = CalcHeight(item.content);
        #endregion
        #endregion

        _data.Add(item);
        InsertData(item);
    }
    #endregion

    #region News
    /// <summary>
    /// 공지사항 검색
    /// </summary>
    private void OnClick_Search()
    {
        isSearching = true;

        string keyWord = input_Search.text.TrimStart().TrimEnd();
        if (keyWord.Trim().Length < 1)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("6013")));
            return;
        }

        curKeyword = keyWord;
        curType = (NEWSSEARCH_TYPE)dropdown.value;

        Clear();
        InitData();
    }

    /// <summary>
    /// 검색 초기화
    /// </summary>
    private void OnClick_Reset()
    {
        if (!isSearching) return;
        isSearching = false;

        curType = NEWSSEARCH_TYPE.None;
        curKeyword = string.Empty;
        input_Search.text = string.Empty;

        Clear();
        InitData();
        Util.RunCoroutine(Co_MoveToFirstData(), "MoveToFirstData_Notice");
    }

    private IEnumerator<float> Co_MoveToFirstData()
    {
        yield return Timing.WaitForOneFrame;
        if (GetDataCount() > 0)
            scroll.MoveToFirstData();
    }

    /// <summary>
    /// 데이터 카운트 
    /// </summary>
    /// <returns></returns>
    private int GetDataCount()
    {
        return _data.Count;
    }

    /// <summary>
    /// 데이터 삭제
    /// </summary>
    public override void Clear()
    {
        base.Clear();

        _data.Clear();
        scroll.ClearData();
    }
    #endregion
}
