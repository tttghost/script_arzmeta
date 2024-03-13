using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using FrameWork.UI;

/// <summary>
/// 사용 안 함 - 보류
/// </summary>
public class View_FAQ : DynamicScrollerBase_View
{
    #region 변수
    private List<db.Faq> faqData;
    private List<Item_FAQData> _data = new List<Item_FAQData>();

    private TMP_InputField input_Search;

    private bool isSearching = false;
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
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        if (GetDataCount() != 0)
        {
            SetExpanded();
            scroll.UpdateAllData();
        }
    }

    protected override void Start()
    {
        base.Start();

        faqData = Single.MasterData.dataFaq.GetList();

        AddSelectCallback();
        LoadData();
    }

    /// <summary>
    /// 서버에서 데이터 받아오기 및 높이 설정
    /// </summary>
    protected override void LoadData(int lastId = 0)
    {
        base.LoadData(lastId);

        int count = faqData.Count;
        for (int i = 0; i < count; i++)
        {
            // 데이터 편집
            Item_FAQData item = new Item_FAQData
            {
                id = i,
                isExpanded = false,
                title = faqData[i].question,
                contents = faqData[i].answer,
                collapsedSize = curHeight
            };
            // 높이 구하기
            item.expandedSize = CalcHeight(item.contents);

            _data.Add(item);
        }

        InsertAllData(_data);
    }

    /// <summary>
    /// 선택 콜백 등록
    /// </summary>
    private void AddSelectCallback()
    {
        if (scroll == null) return;

        scroll.AddSelectCallback((data) =>
        {
            int selectDataIndex = scroll.GetDataIndex(data);
            SetExpanded(data);
            scroll.UpdateAllData();
        });
    }
    #endregion

    #region FAQ
    /// <summary>
    /// FAQ 검색
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

        Clear();

        SetExpanded();
        int count = GetDataCount();
        for (int i = 0; i < count; i++)
        {
            if (_data[i].title.Contains(keyWord) || _data[i].contents.Contains(keyWord))
            {
                InsertData(_data[i]);
            }
        }

        if (scroll.GetDataCount() == 0)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, masterLocalDesc: new MasterLocalData("6014")))
                .ChainPopupAction(new PopupAction(OnClick_Reset));
        }
    }

    /// <summary>
    /// 검색 초기화
    /// </summary>
    private void OnClick_Reset()
    {
        if (!isSearching) return;
        isSearching = false;

        input_Search.text = string.Empty;

        Clear();

        SetExpanded();
        InsertAllData(_data);

        Util.RunCoroutine(Co_MoveToFirstData(), "MoveToFirstData");
    }

    private IEnumerator<float> Co_MoveToFirstData()
    {
        yield return Timing.WaitForOneFrame;
        if (GetDataCount() > 0) scroll.MoveToFirstData();
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

        scroll.ClearData();
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
    #endregion
}
