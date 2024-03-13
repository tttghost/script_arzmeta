using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup_ReqRecFriend : PopupBase
{
    #region 변수
    private TMP_Text txtmp_Count;
    private GameObject go_TextRig;
    private TogglePlus togplus_Receive;
    private TogglePlus togplus_Request;
    private ScrollView_Custom scrollView;

    private int ReqestMaxCount;
    private int ReciveMaxCount;
    private List<Item_ManageFriendData> friendItems = new List<Item_ManageFriendData>();
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));

        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("friend_request_manage"));
        GetUI_TxtmpMasterLocalizing("txtmp_Receive", new MasterLocalData("10010"));
        GetUI_TxtmpMasterLocalizing("txtmp_Request", new MasterLocalData("10011"));
        GetUI_TxtmpMasterLocalizing("txtmp_NoUser", new MasterLocalData("common_state_empty"));
        #endregion

        #region Button
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion

        #region TogglePlus
        togplus_Receive = GetUI<TogglePlus>(nameof(togplus_Receive));
        if (togplus_Receive != null)
        {
            togplus_Receive.SetToggleOnAction(() => { Init(FRIEND_TYPE.RECIVE); });
        }
        togplus_Request = GetUI<TogglePlus>(nameof(togplus_Request));
        if (togplus_Request != null)
        {
            togplus_Request.SetToggleOnAction(() => { Init(FRIEND_TYPE.REQUEST); });
        }
        #endregion

        #region etc
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        scrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollView_Custom>();
        #endregion
    }

    #region 초기화
    protected override void Start()
    {
        ReqestMaxCount = Single.MasterData.dataFunctionTable.GetData((int)FUNCTION_TYPE.REQUEST_FRIEND_MAX).value;
        ReciveMaxCount = Single.MasterData.dataFunctionTable.GetData((int)FUNCTION_TYPE.RICEVE_FRIEND_MAX).value;

        SetCount(0, ReciveMaxCount);
    }

    protected override void OnEnable()
    {
        if (togplus_Receive != null)
        {
            togplus_Receive.SetToggleIsOnWithoutNotify(true);
            togplus_Request.SetToggleIsOnWithoutNotify(false);
        }

        Init(FRIEND_TYPE.RECIVE);
    }
    #endregion

    #region
    /// <summary>
    /// 초기화 및 데이터 로드
    /// </summary>
    /// <returns></returns>
    private void Init(FRIEND_TYPE curState)
    {
        SetActiveTextRig(false);

        switch (curState)
        {
            case FRIEND_TYPE.RECIVE: Single.Web.friend.GetReceiveFriendsToLocal(() => LoadData(curState)); break;
            case FRIEND_TYPE.REQUEST: Single.Web.friend.GetRequestFriendsToLocal(() => LoadData(curState)); break;
            default: break;
        }
    }

    /// <summary>
    /// 목록 카운트 표시
    /// </summary>
    /// <param name="count"></param>
    private void SetCount(int count, int maxCount)
    {
        if (txtmp_Count != null)
        {
            txtmp_Count.text = $"{count} / {maxCount}";
        }
    }

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
    private void LoadData(FRIEND_TYPE curState)
    {
        friendItems = new List<Item_ManageFriendData>();

        Friend[] friends = null;
        int maxCount = 0;
        switch (curState)
        {
            case FRIEND_TYPE.RECIVE: friends = LocalPlayerData.Method.receivefriends; maxCount = ReciveMaxCount; break;
            case FRIEND_TYPE.REQUEST: friends = LocalPlayerData.Method.requestFriends; maxCount = ReqestMaxCount; break;
            default: return;
        }

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
                type = curState,
                updateAction = () => Init(curState)
            };
            friendItems.Add(item);
        }
        scrollView.UpdateData(friendItems.ConvertAll(x => x as Item_Data));
        scrollView.JumpTo(0);

        SetCount(count, maxCount);
    }
    #endregion
}
