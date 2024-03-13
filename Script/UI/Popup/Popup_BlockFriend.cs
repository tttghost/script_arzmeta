using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_BlockFriend : PopupBase
{
    #region 변수
    private TogglePlus togplus_NewBlock;
    private TogglePlus togplus_BlockList;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("fiend_block_management"));
        GetUI_TxtmpMasterLocalizing("txtmp_NewBlock", new MasterLocalData("fiend_block_add"));
        GetUI_TxtmpMasterLocalizing("txtmp_BlockList", new MasterLocalData("fiend_block_list"));
        #endregion

        #region Button
        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        #endregion

        #region TogglePlus
        togplus_NewBlock = GetUI<TogglePlus>(nameof(togplus_NewBlock));
        if (togplus_NewBlock != null)
        {
            togplus_NewBlock.SetToggleOnAction(() => { ChangeView(Cons.View_SearchFriend); });
        }
        togplus_BlockList = GetUI<TogglePlus>(nameof(togplus_BlockList));
        if (togplus_BlockList != null)
        {
            togplus_BlockList.SetToggleOnAction(() => { ChangeView(Cons.View_BlockList); });
        }
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        ChangeView(Cons.View_SearchFriend);

        if (togplus_NewBlock != null)
        {
            togplus_NewBlock.SetToggleIsOnWithoutNotify(true);
        }
        if (togplus_BlockList != null)
        {
            togplus_BlockList.SetToggleIsOnWithoutNotify(false);
        }
    }
    #endregion
}
