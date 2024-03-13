using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View_BlockList : UIBase
{
    #region 변수
    private GameObject go_TextRig;
    private ScrollView_Custom scrollView;
    private List<Item_ManageFriendData> blockMemberItems = new List<Item_ManageFriendData>();
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_NoUser", new MasterLocalData("common_state_empty"));
        #endregion

        #region etc
        scrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollView_Custom>();
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        Init();
    }
    #endregion

    #region 
    /// <summary>
    /// 초기화 및 데이터 로드
    /// </summary>
    /// <returns></returns>
    private void Init()
    {
        SetActiveTextRig(false);

        // 목록 갱신
        Single.Web.friend.GetBlockFriendsToLocal(() => LoadData());
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
    private void LoadData()
    {
        blockMemberItems = new List<Item_ManageFriendData>();

        Member[] member = LocalPlayerData.Method.blockfriends;

        int count = member.Length;
        if (count == 0)
        {
            SetActiveTextRig(true);
        }

        for (int i = 0; i < count; i++)
        {
            Item_ManageFriendData item = new Item_ManageFriendData
            {
                memberCode = member[i].memberCode,
                nickname = member[i].nickname,
                message = member[i].stateMessage,
                avatarInfos = member[i].avatarInfos,
                type = FRIEND_TYPE.UNBLOCK,
                updateAction = Init // 차단 해제하면 목록 갱신해주기
            };
            blockMemberItems.Add(item);
        }
        scrollView.UpdateData(blockMemberItems.ConvertAll(x => x as Item_Data));
        scrollView.JumpTo(0);
    }
    #endregion
}
