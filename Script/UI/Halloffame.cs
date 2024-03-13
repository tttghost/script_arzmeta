using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Halloffame : MonoBehaviour
{
    /*
    public float scrollbarSpeed = 100f;
    private RectTransform canvas;
    private RectTransform content;
    private Scrollbar scrollbar;
    private void Start()
    {
        canvas = Util.Search<RectTransform>(gameObject, "Canvas");
        content = Util.Search<RectTransform>(gameObject, "Content");
        scrollbar = Util.Search<Scrollbar>(gameObject, "Scrollbar Vertical");
        InitData();
    }

    private void Update()
    {
        //TestCode();
        InfinityScrolling();
    }

    #region 데이터로드


    /// <summary>
    /// 데이터 셋업
    /// </summary>
    public void InitData()
    {
        Create_View_Title();
        Create_View_Year(); //뷰이어 생성시 자동으로 하위개체 생성


    }

    /// <summary>
    /// 뷰타이틀 생성
    /// </summary>
    private void Create_View_Title()
    {
        UIBase view_Title = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_HallOfFame + "View_Title", content.transform).GetComponent<UIBase>();
        view_Title.gameObject.SetActive(true);
        view_Title.GetUI_Txtmp("txtmp_MainTitle", "메인타이틀");
        view_Title.GetUI_Txtmp("txtmp_SubTitle", "서브타이틀");
        view_Title.GetUI_Txtmp("txtmp_Year", "년도");
    }


    /// <summary>
    /// 뷰_이어 생성
    /// </summary>
    private void Create_View_Year()
    {
        // 명전 전체 리스트
        List<db.HallOfFame> hofAllList = Single.MasterData.dataHallOfFame.GetList(); 

        //연도별 명전리스트 생성
        Dictionary<int, List<db.HallOfFame>> hofYearDic = new Dictionary<int, List<db.HallOfFame>>(); 

        // 분류작업 1 : 전체리스트에서 연도별 딕셔너리로 재분류 (ex - 2008년도에 해당하는 데이터 전체)
        for (int i = 0; i < hofAllList.Count; i++) 
        {
            db.HallOfFame hof = hofAllList[i];
            Util.DicList(ref hofYearDic, hof, hof.year);            //if (!hofYearDic.ContainsKey(hof.year))
            //{
            //    List<db.HallOfFame> dummyList = new List<db.HallOfFame>();
            //    dummyList.Add(hof);
            //    hofYearDic.Add(hof.year, dummyList);
            //}
            //else
            //{
            //    List<db.HallOfFame> dummyList = hofYearDic[hof.year];
            //    dummyList.Add(hof);
            //    hofYearDic[hof.year] = dummyList;
            //}
        }

        // 추출한 연도별 딕셔너리에서 어워드별 데이터 추출하기 위해 어워드뷰 생성
        foreach (var hofYear in hofYearDic)
        {
            // 프리팹 생성
            UIBase view_Year = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_HallOfFame + "View_Year", content).GetComponent<UIBase>();
            view_Year.gameObject.SetActive(true);

            // 데이터 셋
            string hofYearName = hofYear.Key.ToString();
            view_Year.GetUI_Txtmp("txtmp_Year", hofYearName);
            UIBase view_Award_Parent = view_Year.GetUI<UIBase>("View_Award_Parent");
            view_Award_Parent.gameObject.SetActive(true);

            // 어워드뷰 생성
            Create_View_Award(hofYear.Value, view_Award_Parent.transform);
        }
    }

    /// <summary>
    /// 뷰_어워드 생성
    /// </summary>
    /// <param name="hofYearList">연도 기준 명전 리스트</param>
    /// <param name="parent"></param>
    private void Create_View_Award(List<db.HallOfFame> hofYearList, Transform parent)
    {
        // 어워드별 명전 리스트 딕셔너리 생성
        Dictionary<int, List<db.HallOfFame>> hofAwardDic = new Dictionary<int, List<db.HallOfFame>>();

        // 분류작업 2 : 해당 연도별 리스트에서 어워드별 명전 리스트에 추가 (ex - 2008년도의 일반부에 해당하는 데이터 전체)
        for (int i = 0; i < hofYearList.Count; i++) 
        {
            db.HallOfFame hofYear = hofYearList[i];
            Util.DicList(ref hofAwardDic, hofYear, hofYear.hallOfFameType);
        }

        // 어워드별 딕셔너리에서 랭크데이터 생성
        foreach (var hofAward in hofAwardDic)
        {
            // 프리팹 생성
            UIBase view_Award = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_HallOfFame + "View_Award", parent).GetComponent<UIBase>();
            view_Award.gameObject.SetActive(true);

            // 데이터 셋
            string awardType = Single.MasterData.dataHallOfFameType.GetData(hofAward.Key).name;
            view_Award.GetUI_Txtmp("txtmp_Award", awardType);
            UIBase view_Rank_Parent = view_Award.GetUI<UIBase>("View_Rank_Parent");
            view_Rank_Parent.gameObject.SetActive(true);

            // 랭크뷰 생성
            Create_View_Rank(hofAward.Value, view_Rank_Parent.transform);
        }

    }

    /// <summary>
    /// 뷰_랭크 생성
    /// </summary>
    /// <param name="hofAwardList"></param>
    /// <param name="parent"></param>
    private void Create_View_Rank(List<db.HallOfFame> hofAwardList, Transform parent)
    {
        // 랭크는 순차 출력
        for (int i = 0; i < hofAwardList.Count; i++) 
        {
            // 프리팹 로드
            UIBase view_Rank = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_HallOfFame + "View_Rank", parent).GetComponent<UIBase>();
            view_Rank.gameObject.SetActive(true);

            // 데이터 셋
            db.HallOfFame hofAward = hofAwardList[i];
            view_Rank.GetUI_Img("img_Rank", "icon_rank" + hofAward.rank.ToString());
            view_Rank.GetUI_Txtmp("txtmp_Name", hofAward.teamName);
            view_Rank.GetUI_Img("img_Country", "img_country_" + hofAward.countryCodeId.ToString());
        }
    }
    #endregion


    #region 로직

    private float skipSpeed = 1f;
    /// <summary>
    /// 무한스크롤 테스트했던 코드
    /// </summary>
    private void TestCode()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            skipSpeed = 0.1f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            skipSpeed = 1f;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            scrollbar.value -= (scrollbarSpeed / content.sizeDelta.y) * skipSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            scrollbar.value += (scrollbarSpeed / content.sizeDelta.y) * skipSpeed * Time.deltaTime;
        }

    }

    /// <summary>
    /// 무한 스크롤링
    /// </summary>
    private void InfinityScrolling() // 틱 현상 나중에 수정...
    {
        scrollbar.value -= (scrollbarSpeed / content.sizeDelta.y) * skipSpeed * Time.deltaTime;
        if (content.localPosition.y - canvas.sizeDelta.y / 2f >= content.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y)
        {
            content.transform.GetChild(0).SetAsLastSibling();
            scrollbar.value = 1f;
        }
    }

    #endregion
    */
}


