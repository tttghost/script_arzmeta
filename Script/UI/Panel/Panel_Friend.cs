using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_Friend : PanelBase
{
    #region 변수
    private TogglePlus togplus_FriendlList;
    private TogglePlus togplus_FriendManage;
    private TogglePlus togplus_ArzTalk;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("arzphone_friend_title"));
        GetUI_TxtmpMasterLocalizing("txtmp_FriendlListOn", new MasterLocalData("friend_list"));
        GetUI_TxtmpMasterLocalizing("txtmp_FriendlListOff", new MasterLocalData("friend_list"));
        GetUI_TxtmpMasterLocalizing("txtmp_FriendManageOn", new MasterLocalData("friend_manage"));
        GetUI_TxtmpMasterLocalizing("txtmp_FriendManageOff", new MasterLocalData("friend_manage"));
        GetUI_TxtmpMasterLocalizing("txtmp_ArzTalkOn", new MasterLocalData("10105"));
        GetUI_TxtmpMasterLocalizing("txtmp_ArzTalkOff", new MasterLocalData("10105"));
        #endregion

        #region Button
        GetUI_Button("btn_Back", Back);
        #endregion

        #region TogglePlus
        togplus_FriendlList = GetUI<TogglePlus>(nameof(togplus_FriendlList));
        if (togplus_FriendlList != null)
        {
            togplus_FriendlList.SetToggleOnAction(() => { ChangeView(Cons.View_FriendList); });
        }
        togplus_FriendManage = GetUI<TogglePlus>(nameof(togplus_FriendManage));
        if (togplus_FriendManage != null)
        {
            togplus_FriendManage.SetToggleOnAction(() => { ChangeView(Cons.View_FriendManage); });
        }
        togplus_ArzTalk = GetUI<TogglePlus>(nameof(togplus_ArzTalk)); // 개발 보류 기능
        if (togplus_ArzTalk != null)
        {
            togplus_ArzTalk.SetToggleOnAction(() => { ChangeView(Cons.View_ArzTalk); });
        }
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        ChangeView(Cons.View_FriendList);

        // 이게 토글 변경으로 왜 안 될까...
        if (togplus_FriendlList != null)
        {
            togplus_FriendlList.SetToggleIsOnWithoutNotify(true);
        }
        if (togplus_FriendManage != null)
        {
            togplus_FriendManage.SetToggleIsOnWithoutNotify(false);
        }
        if (togplus_ArzTalk != null)
        {
            togplus_ArzTalk.SetToggleIsOnWithoutNotify(false);
        }
    }
    #endregion
}
