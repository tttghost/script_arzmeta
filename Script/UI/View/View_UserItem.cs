namespace Gpm.Ui.Sample
{
    using FrameWork.UI;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class View_UserItemData : InfiniteScrollData
    {
        public defaultRanking defaultRanking;

        public View_UserItemData(defaultRanking defaultRanking)
        {
            this.defaultRanking = defaultRanking;
        }
    }
    public class View_UserItem : InfiniteScrollItem
    {
        private Image img_Icon;
        private List<Image> img_numList = new List<Image>();
        private Dictionary<string, Sprite> img_numDic = new Dictionary<string, Sprite>();
        private TMP_Text txtmp_time;
        private TMP_Text txtmp_name;
        private string rankPre = "icon_rank_"; //랭크의 앞단 스트링
        private string numPre = "img_ranknum_"; //랭크의 앞단 스트링
        private bool isOnce = false;
        private void SetRank(int rank)
        {
            if (!isOnce)
            {
                isOnce = true;
                for (int i = 0; i < 10; i++)
                {
                    string numPath = Cons.Path_Image + numPre + i;
                    img_numDic.Add(i.ToString(), Single.Resources.Load<Sprite>(numPath));
                }
            }

            //rank
            List<int> rankMedal = SceneLogic.instance.GetPanel<Panel_InfinityCode>().rankMedal;
            string medalPath = Cons.Path_Image + rankPre;
            if (rankMedal.Count < 3)
            {
                if (!rankMedal.Contains(rank))
                {
                    rankMedal.Add(rank);
                }
            }
            if(rankMedal.Contains(rank))
            {
                medalPath += (rankMedal.IndexOf(rank) + 1).ToString();
            }
            else
            {
                medalPath += "other";
            }
            SceneLogic.instance.GetPanel<Panel_InfinityCode>().rankMedal = rankMedal;
            img_Icon.sprite = Single.Resources.Load<Sprite>(medalPath);

            //time
            string rankStr = rank.ToString();
            for (int i = 0; i < img_numList.Count; i++) //전체 등수이미지 꺼주고
            {
                img_numList[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < rankStr.Length; i++) //해당 등수이미지 켜줌
            {
                img_numList[i].gameObject.SetActive(true);
                img_numList[i].sprite = img_numDic[rankStr[i].ToString()];
            }
        }
 
        private void InitData()
        {
            gameObject.SetActive(true);
            img_Icon = Util.Search<Image>(gameObject, nameof(img_Icon));
            img_numList.Add(Util.Search<Image>(gameObject, "img_num_0"));
            img_numList.Add(Util.Search<Image>(gameObject, "img_num_1"));
            img_numList.Add(Util.Search<Image>(gameObject, "img_num_2"));
            txtmp_time = Util.Search<TMP_Text>(gameObject, nameof(txtmp_time));
            txtmp_name = Util.Search<TMP_Text>(gameObject, nameof(txtmp_name));
        }

        private void SetData()
        {
            View_UserItemData view_UserItemData = (View_UserItemData)scrollData;
            defaultRanking defaultRanking = view_UserItemData.defaultRanking;

            SetRank(defaultRanking.rank);
            Util.SetMasterLocalizing(txtmp_time, defaultRanking.userScore.ToString());
            Util.SetMasterLocalizing(txtmp_name, defaultRanking.nickname.ToString());
        }
        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);
            InitData();
            SetData();
        }
    }
}