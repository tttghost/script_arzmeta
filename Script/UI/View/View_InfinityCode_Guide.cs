using FrameWork.UI;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_InfinityCode_Guide : UIBase
{

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        //기본정보
        GetUI_TxtmpMasterLocalizing("txtmp_title", new MasterLocalData("game_guide_eng"));
        GetUI_TxtmpMasterLocalizing("txtmp_subtitle", new MasterLocalData("game_infinitecode_desc"));
        GetUI_TxtmpMasterLocalizing("txtmp_success", new MasterLocalData("game_success"));
        GetUI_TxtmpMasterLocalizing("txtmp_fail", new MasterLocalData("game_failure"));

        //백키
        GetUI_Button("btn_back", Back);
        GetUI_TxtmpMasterLocalizing("txtmp_back", new MasterLocalData("common_back"));

        //게임시작키
        GetUI_Button("btn_gamestart", () => GetPanel<Panel_InfinityCode>().ChangeView<View_InfinityCode_Game>());
    }
}
