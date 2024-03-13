using FrameWork.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class View_FriendList : UIBase
{
    #region 변수
    private TMP_Text txtmp_Count;
    private GameObject go_TextRig;
    private ScrollView_Custom scrollView;

    private int friendMaxCount;
    private List<Item_FriendData> friendItems = new List<Item_FriendData>();
    private List<Item_FriendData> bookmarkList;
    private List<Item_FriendData> nomalList;
    private List<Item_FriendData> sortList;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));

        GetUI_TxtmpMasterLocalizing("txtmp_NoUser", new MasterLocalData("common_state_empty"));
        #endregion

        #region Button
        GetUI_Button("btn_Refresh", () => Init());
        #endregion

        #region etc
        scrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollView_Custom>();
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        friendMaxCount = Single.MasterData.dataFunctionTable.GetData((int)FUNCTION_TYPE.TOTAL_FRIEND_MAX).value;

        SetCount(0);
    }

    protected override void OnEnable()
    {
        AddListener();
        
        Init();
        SetActiveTextRig(false);
    }

    protected override void OnDisable()
    {
        RemoveListener();
    }

    private void AddListener()
    {
        Single.Socket.item_S_FriendList_Handler += LoadData;
    }

    private void RemoveListener()
    {
        Single.Socket.item_S_FriendList_Handler -= LoadData;
    }
    #endregion

    #region
    /// <summary>
    /// 목록 카운트 표시
    /// </summary>
    /// <param name="count"></param>
    private void SetCount(int count)
    {
        if (txtmp_Count != null)
        {
            txtmp_Count.text = $"{count} / {friendMaxCount}";
        }
    }

    /// <summary>
    /// 초기화 및 데이터 로드
    /// </summary>
    /// <returns></returns>
    public void Init() => Single.Socket.C_FriendList();


    /// <summary>
    /// 비어있습니다 텍스트 비/활성화
    /// </summary>
    /// <param name="active"></param>
    private void SetActiveTextRig(bool active)
    {
        if (go_TextRig != null)
        {
            go_TextRig.SetActive(active);
        }
    }

    /// <summary>
    /// 아이템 로드
    /// </summary>
    private void LoadData(FriendWebSocket[] item_S_FriendList)
    {
        friendItems = new List<Item_FriendData>();

        int count = item_S_FriendList.Length;
        
        SetActiveTextRig(count == 0);

        for (int i = 0; i < count; i++)
        {
            var item = new Item_FriendData
            {
                memberId = item_S_FriendList[i].friendMemberId,
                memberCode = item_S_FriendList[i].friendMemberCode,
                nickname = item_S_FriendList[i].friendNickname,
                message = item_S_FriendList[i].friendMessage,
                createdAt = item_S_FriendList[i].createdAt,
                avatarInfos = item_S_FriendList[i].avatarInfos,
                isOnline = item_S_FriendList[i].isOnline,
                bookmark = item_S_FriendList[i].bookmark == 1,
                bookmarkedAt = item_S_FriendList[i].bookmarkedAt,
                updateAction = Init
            };
            friendItems.Add(item);
        }
        scrollView.UpdateData(SortList(friendItems).ConvertAll(x => x as Item_Data));
        scrollView.JumpTo(0);

        SetCount(count);
    }

    /// <summary>
    /// 고정 여부에 따른 목록 분류 및 정렬
    /// 1. 고정인가?
    /// 2. 고정된 날짜가 더 최근인가?
    /// 3. 닉네임 가나다순
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    private List<Item_FriendData> SortList(List<Item_FriendData> datas)
    {
        bookmarkList = new List<Item_FriendData>();
        nomalList = new List<Item_FriendData>();

        int count = datas.Count;
        for (int i = 0; i < count; i++)
        {
            List<Item_FriendData> list = datas[i].bookmark ? bookmarkList : nomalList;
            list.Add(datas[i]);
        }

        sortList = new List<Item_FriendData>();
        if (bookmarkList.Count != 0)
        {
            bookmarkList = SortBookmark(bookmarkList);
            sortList.AddRange(bookmarkList);
        }
        if (nomalList.Count != 0)
        {
            sortList.AddRange(nomalList);
        }

        return sortList;
    }

    /// <summary>
    /// 제한 사항에 따른 정렬
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private List<Item_FriendData> SortBookmark(List<Item_FriendData> data)
    {
        var sortData = data.OrderBy(x => x.bookmarkedAt).ThenBy(x => x.nickname);
        return sortData.ToList();
    }
    #endregion
}
