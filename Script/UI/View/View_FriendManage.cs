using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class View_FriendManage : UIBase
{
    #region 변수
    private TMP_Text txtmp_Count;
    private GameObject go_TextRig;
    private ScrollView_Custom scrollView;

    private int friendMaxCount;
    private List<Item_ManageFriendData> friendItems = new List<Item_ManageFriendData>();
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));
        
        GetUI_TxtmpMasterLocalizing("txtmp_NoUser", new MasterLocalData("common_state_empty"));
        GetUI_TxtmpMasterLocalizing("txtmp_BlockFriend", new MasterLocalData("fiend_block_management"));
        GetUI_TxtmpMasterLocalizing("txtmp_AddFriend", new MasterLocalData("friend_add"));
        GetUI_TxtmpMasterLocalizing("txtmp_RequestFriend", new MasterLocalData("friend_request_manage"));
        #endregion

        #region Button
        GetUI_Button("btn_Refresh", () => Init());
        GetUI_Button("btn_BlockFriend", () => PushPopup<Popup_BlockFriend>());
        GetUI_Button("btn_AddFriend", () => PushPopup<Popup_AddFriend>());
        GetUI_Button("btn_RequestFriend", () => PushPopup<Popup_ReqRecFriend>());
        #endregion

        #region etc
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        scrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollView_Custom>();
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
        Init();
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
    /// <param name="active"></param>
    private void SetActiveTextRig(bool active)
    {
        if (go_TextRig != null)
        {
            go_TextRig.SetActive(active);
        }
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <returns></returns>
    public void Init()
    {
        SetActiveTextRig(false);

        // 목록 갱신
        Single.Web.friend.GetFriendsToLocal(() => LoadData());
    }

    /// <summary>
    /// 아이템 로드
    /// </summary>
    private void LoadData()
    {
        friendItems = new List<Item_ManageFriendData>();

        Friend[] friends = LocalPlayerData.Method.friends;
        
        int count = friends.Length;
        if (count == 0)
        {
            SetActiveTextRig(true);
        }

        for (int i = 0; i < count; i++)
        {
            Item_ManageFriendData item = new Item_ManageFriendData
            {
                memberCode = friends[i].friendMemberCode,
                nickname = friends[i].friendNickname,
                message = friends[i].friendMessage,
                createdAt = friends[i].createdAt,
                avatarInfos = friends[i].avatarInfos,
                type = FRIEND_TYPE.DELETE,
                updateAction = Init
            };
            friendItems.Add(item);
        }
        scrollView.UpdateData(friendItems.ConvertAll(x => x as Item_Data));
        scrollView.JumpTo(0);
        
        SetCount(count);
    }
    #endregion
}
