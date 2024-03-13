namespace Gpm.Ui.Sample
{
    using FrameWork.UI;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class View_InfinityCode_Ranking : View_InfinityCode_RankBase
    {
        private Button btn_retry;
        private TMP_Text txtmp_date;
        protected override void SetMemberUI()
        {
            base.SetMemberUI();

            ///시간갱신
            txtmp_date = GetUI_TxtmpMasterLocalizing(nameof(txtmp_date));
            GetUI_Button("btn_date", ()=> OnValueChanged_ViewRank(curInfinityCode_UserType));

            ///리트라이
            btn_retry = GetUI_Button(nameof(btn_retry), () => panel_InfinityCode.ChangeView<View_InfinityCode_Game>());

            ///백키시 인트로
            GetUI_Button("btn_exit", Back);

        }
        protected override void Start()
        {
            base.Start();
            infiniteScroll.itemPrefab = Single.Resources.Load<GameObject>(Cons.Path_Prefab_UI_View + "View_InfinityCodeRankingItem").GetComponent<InfiniteScrollItem>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            //직전뷰에 따른 리트라이버튼 활성화/비활성화
            if(panel_InfinityCode.prevView is View_InfinityCode_Intro)
            {
                btn_retry.gameObject.SetActive(false);
            }
            else
            {
                btn_retry.gameObject.SetActive(true);
            }
        }
        protected override void OnValueChanged_ViewRank(infinityCode_UserType infinityCode_UserType)
        {
            base.OnValueChanged_ViewRank(infinityCode_UserType);
            StartCoroutine(SetText());
        }
        private IEnumerator SetText()
        {
            yield return null;
            Util.SetMasterLocalizing(txtmp_date, new MasterLocalData("game_notice_infinitecode_time", System.DateTime.Now.ToString("yyyy.MM.dd HH:mm"))); //마지막 업데이트 시간 갱신
        }
    }
}