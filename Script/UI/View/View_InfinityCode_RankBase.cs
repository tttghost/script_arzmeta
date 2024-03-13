namespace Gpm.Ui.Sample
{
    using FrameWork.UI;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class View_InfinityCode_RankBase : UIBase
    {
        protected ToggleGroup togg_ranking;
        protected Toggle tog_ranking_all; //All 랭크 불러오기 위한 토글
        protected Toggle tog_ranking_my; //My 랭크 불러오기 위한 토글
        protected Panel_InfinityCode panel_InfinityCode; //부모패널
        protected InfiniteScroll infiniteScroll;
        protected infinityCode_UserType curInfinityCode_UserType; //토글기능할때 현재 유저타입(All, My) 판단을 위함

        protected override void SetMemberUI()
        {
            base.SetMemberUI();

            panel_InfinityCode = GetPanel<Panel_InfinityCode>();

            togg_ranking = GetUI<ToggleGroup>(nameof(togg_ranking));

            tog_ranking_all = GetUI_Toggle(nameof(tog_ranking_all), () => OnValueChanged_ViewRank(infinityCode_UserType.All));
            GetUI_TxtmpMasterLocalizing("txtmp_ranking_all", new MasterLocalData("game_ranking_total"));

            tog_ranking_my = GetUI_Toggle(nameof(tog_ranking_my), () => OnValueChanged_ViewRank(infinityCode_UserType.My));
            GetUI_TxtmpMasterLocalizing("txtmp_ranking_my", new MasterLocalData("game_record_mine"));

            infiniteScroll = GetComponentInChildren<InfiniteScroll>();

            curInfinityCode_UserType = infinityCode_UserType.All;
        }


        /// <summary>
        /// 랭크 보기
        /// </summary>
        protected virtual void OnValueChanged_ViewRank(infinityCode_UserType infinityCode_UserType)
        {
            curInfinityCode_UserType = infinityCode_UserType;
            infiniteScroll.ClearData();
            Panel_InfinityCode panel_InfinityCode = GetPanel<Panel_InfinityCode>();
            panel_InfinityCode.rankMedal.Clear();
            Single.Web.ranking.AllMyRanking((res) =>
            {
                panel_InfinityCode.worldBestScore = res.allRanking.Length > 0 ? res.allRanking[0].userScore : 999;
                panel_InfinityCode.myBestScore = res.myRanking.Length > 0 ? res.myRanking[0].userScore : 999;

                //순위권(3위) 안에 들었을 때 리워드
                //db.Quest master_quest = Single.MasterData.dataQuest.GetData(11);
                //int limitRank = master_quest.conditionMaxValue; //제한랭크
                //int questConditionType = master_quest.questConditionType;
                //for (int i = 0; i < res.rankingList.Length; i++)
                //{
                //    defaultRanking curRanking = res.rankingList[i];
                //    if (LocalPlayerData.NickName == curRanking.member.nickname 
                //    && curRanking.rank <= limitRank
                //    )
                //    {
                //        Single.Web.Quest_Process_Req(questConditionType, Single.Web.Quest_Process_Res); // 무한의코드 순위권(3위) 프로세스
                //        break;
                //    }
                //}

                defaultRanking[] selectRanking = default;
                switch (infinityCode_UserType)
                {
                    case infinityCode_UserType.All: selectRanking = (defaultRanking[])res.allRanking.Clone(); break;
                    case infinityCode_UserType.My: selectRanking = (defaultRanking[])res.myRanking.Clone(); break;
                    default:
                        break;
                }
                for (int i = 0; i < selectRanking.Length; i++)
                {
                    infiniteScroll.InsertData(new View_UserItemData(selectRanking[i]));
                }
            });
            
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            CurToggleUpdate();
        }

        private void CurToggleUpdate()
        {
            switch (curInfinityCode_UserType)
            {
                case infinityCode_UserType.All: Util.ToggleIsOn(togg_ranking, tog_ranking_all); break;
                case infinityCode_UserType.My: Util.ToggleIsOn(togg_ranking, tog_ranking_my); break;
            }
        }
    }
}