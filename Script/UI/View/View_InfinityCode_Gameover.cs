using FrameWork.UI;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class View_InfinityCode_Gameover : UIBase
{
    private TMP_Text txtmp_subtitle;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        Panel_InfinityCode panel_InfinityCode = GetPanel<Panel_InfinityCode>();
        txtmp_subtitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_subtitle));
        
        GetUI_Button("btn_retry", () => panel_InfinityCode.ChangeView<View_InfinityCode_Game>());
        GetUI_TxtmpMasterLocalizing("txtmp_retry", new MasterLocalData("game_repeat"));
        GetUI_Button("btn_ranking", () => panel_InfinityCode.ChangeView<View_InfinityCode_Ranking>());
        GetUI_TxtmpMasterLocalizing("txtmp_ranking", new MasterLocalData("game_ranking"));
        GetUI_Button("btn_quit", () => panel_InfinityCode.ChangeView<View_InfinityCode_Intro>());
        GetUI_TxtmpMasterLocalizing("txtmp_quit", new MasterLocalData("common_quit"));
    }
    public void SetData(string failIdx)
    {
        Util.SetMasterLocalizing(txtmp_subtitle, new MasterLocalData(failIdx));
        Single.Sound.PlayEffect("effect_codelose_0");
    }
}
