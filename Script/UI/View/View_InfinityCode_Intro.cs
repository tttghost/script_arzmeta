namespace Gpm.Ui.Sample
{
    using FrameWork.UI;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class View_InfinityCode_Intro : View_InfinityCode_RankBase
    {
        protected override void Start()
        {
            base.Start();
            infiniteScroll.itemPrefab = Single.Resources.Load<GameObject>(Cons.Path_Prefab_UI_View + "View_InfinityCodeIntroItem").GetComponent<InfiniteScrollItem>();
        }
        protected override void SetMemberUI()
        {
            base.SetMemberUI();

            ///게임시작
            Button btn_gamestart = GetUI_Button(nameof(btn_gamestart), () => panel_InfinityCode.ChangeView<View_InfinityCode_Game>(), "effect_codestart_0");

            ///백키 팝패널
            Button btn_back = GetUI_Button(nameof(btn_back), Back);
            TMP_Text txtmp_back = GetUI_TxtmpMasterLocalizing(nameof(txtmp_back), new MasterLocalData("common_back"));

            ///게임가이드로
            Button btn_guide = GetUI_Button(nameof(btn_guide), () => panel_InfinityCode.ChangeView<View_InfinityCode_Guide>(true));

            ///랭크보기
            Button btn_ranking_detail = GetUI_Button(nameof(btn_ranking_detail), () => panel_InfinityCode.ChangeView<View_InfinityCode_Ranking>());
            TMP_Text txtmp_ranking_detail = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ranking_detail), new MasterLocalData("game_ranking_detail"));

        }
    }
}