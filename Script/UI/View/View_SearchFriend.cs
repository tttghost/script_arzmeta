using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class View_SearchFriend : UIBase
{
    #region 변수
    public FRIEND_TYPE type;

    [SerializeField] private Item_ManageFriend item_ManagerFriend;

    private TMP_Dropdown dropdown_Search;
    private TMP_InputField input_Search;
    private GameObject go_ItemRig;
    private GameObject go_TextRig;

    private FRIENDREQUEST_TYPE searchType;
    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        GetUI_Button("btn_Search", OnClick_Search);
        #endregion

        #region InputField
        input_Search = GetUI_TMPInputField(nameof(input_Search), masterLocalData: new MasterLocalData("6013"));
        #endregion

        #region Dropdown
        dropdown_Search = GetUI_TMPDropdown(nameof(dropdown_Search), (index) => { searchType = (FRIENDREQUEST_TYPE)(index + 1); }, new List<MasterLocalData>
        {
           new MasterLocalData("013"),
           new MasterLocalData("014")
        });
        #endregion

        #region etc
        go_ItemRig = GetChildGObject(nameof(go_ItemRig));
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        #endregion
    }

    #region 초기화
    protected override void Awake()
    {
        base.Awake();
        item_ManagerFriend.InitModule();

        input_Search.onSubmit.AddListener(OnInputFieldSubmit);
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 채팅할때 플레이어 이동 BLOCK
        if (MyPlayer.instance != null && MyPlayer.instance.TPSController.StarterInputs != null)
        {
            if (input_Search.isFocused)
            {
                MyPlayer.instance.TPSController.StarterInputs.blockSprint = true;
                MyPlayer.instance.TPSController.StarterInputs.blockJump = true;
                MyPlayer.instance.TPSController.StarterInputs.blockMove = true;
            }
            else
            {
                MyPlayer.instance.TPSController.StarterInputs.blockSprint = false;
                MyPlayer.instance.TPSController.StarterInputs.blockJump = false;
                MyPlayer.instance.TPSController.StarterInputs.blockMove = false;
            }
        }
#endif
    }

    protected override void OnEnable()
    {
        if (input_Search != null)
        {
            input_Search.Clear();
        }
        if (dropdown_Search != null)
        {
            dropdown_Search.Initialize();
        }

        SetActiveResult(FRIENDRESULT_TYPE.NONE);
        searchType = FRIENDREQUEST_TYPE.NICKNAME;
    }
    #endregion

    #region 검색하기
    /// <summary>
    /// 검색 로직 실행(인풋필드에서 Enter가 입력되었을 때)
    /// </summary>
    private void OnInputFieldSubmit(string searchValue)
    {
        // 검색 로직 실행
        FindFriend(searchValue);
    }

    /// <summary>
    /// 검색 로직 실행 (버튼 클릭 시)
    /// </summary>
    public void OnClick_Search()
    {
        FindFriend(input_Search.text);
    }

    private void FindFriend(string searchValue)
    {
        Single.Web.friend.FindFriend((int)searchType, searchValue, (res) =>
        {
            Item_ManageFriendData item = new Item_ManageFriendData
            {
                memberCode = res.member.friendMemberCode,
                nickname = res.member.friendNickname,
                message = res.member.friendMessage,
                createdAt = res.member.createdAt,
                avatarInfos = res.member.avatarInfos,
                type = LocalPlayerData.Method.IsFriend(res.member.friendMemberCode) && type != FRIEND_TYPE.BLOCK && type != FRIEND_TYPE.REPORT ? FRIEND_TYPE.NONE : type,
            };
            item_ManagerFriend.UpdateContent(item);

            SetActiveResult(FRIENDRESULT_TYPE.RESULT);
        },
        (error) =>
        {
            SetActiveResult(FRIENDRESULT_TYPE.NON_RESULT);
        });
    }

    /// <summary>
    /// 결과 있으면 켜주기
    /// </summary>
    /// <param name="result"></param>
    private void SetActiveResult(FRIENDRESULT_TYPE result)
    {
        if (go_ItemRig == null || go_TextRig == null) return;

        go_ItemRig.SetActive(false);
        go_TextRig.SetActive(false);

        switch (result)
        {
            case FRIENDRESULT_TYPE.RESULT: go_ItemRig.SetActive(true); break;
            case FRIENDRESULT_TYPE.NON_RESULT: go_TextRig.SetActive(true); break;
            default: break;
        }
    }
    #endregion
}
