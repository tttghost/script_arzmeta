using FrameWork.UI;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_InfinityCode_Success : UIBase
{

    private TMP_Text txtmp_worldnewrecord;
    private TMP_Text txtmp_missioncomplete;
    private Image img_ribbon;

    #region timer관련
    private Dictionary<string, Sprite> numDic = new Dictionary<string, Sprite>();
    private List<Image> timerImageList = new List<Image>();
    private float _timer;
    public float timer //시간
    {
        get
        {
            return _timer;
        }
        set
        {
            _timer = value;
            SetTimer(_timer);
        }
    }
    private void SetTimer(float curTime)
    {
        if (!isOnce)
        {
            isOnce = true;
            for (int i = 0; i < 10; i++)
            {
                string path = $"{Cons.Path_Image}Img_num_{i}";
                numDic.Add(i.ToString(), Single.Resources.Load<Sprite>(path));
            }
        }
        string timer = Mathf.Floor((curTime * 100f)).ToString("0000");
        for (int i = 0; i < timerImageList.Count; i++)
        {
            timerImageList[i].sprite = numDic[timer[i].ToString()];
        }
    }

    private bool isOnce = false;
    /// <summary>
    /// 타이머를 작동하기위한 초기 셋팅
    /// </summary>
    private void InitTimer()
    {
        timerImageList.Add(GetUI_Img("img_10"));
        timerImageList.Add(GetUI_Img("img_1"));
        timerImageList.Add(GetUI_Img("img_01"));
        timerImageList.Add(GetUI_Img("img_001"));
    }
    #endregion
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        txtmp_worldnewrecord = GetUI_TxtmpMasterLocalizing(nameof(txtmp_worldnewrecord));
        img_ribbon = GetUI_Img(nameof(img_ribbon));
        txtmp_missioncomplete = GetUI_TxtmpMasterLocalizing(nameof(txtmp_missioncomplete), new MasterLocalData("game_infinitecode_time"));
        InitTimer();
    }
    public void SetData(float timer)
    {
        this.timer = timer;
        Panel_InfinityCode panel_InfinityCode = GetPanel<Panel_InfinityCode>();
        img_ribbon.gameObject.SetActive(false);
        if (timer < panel_InfinityCode.worldBestScore) //세계기록
        {
            img_ribbon.gameObject.SetActive(true);
            Util.SetMasterLocalizing(txtmp_worldnewrecord, new MasterLocalData("game_infinitecode_recode_breaking_overall"));
        }
        else if (timer < panel_InfinityCode.myBestScore) //내기록
        {
            img_ribbon.gameObject.SetActive(true);
            Util.SetMasterLocalizing(txtmp_worldnewrecord, new MasterLocalData("game_infinitecode_recode_breaking_mine"));
        }
        Single.Sound.PlayEffect("effect_codewin_0");
        StartCoroutine(Co_GoRanking());
    }

    /// <summary>
    /// 3초후 랭킹으로 돌아간다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Co_GoRanking()
    {
        yield return Cons.Time_Sec_3;

        Single.Web.ranking.RecordRanking(timer, (res) =>
        {
            Panel_InfinityCode panel_InfinityCode = GetPanel<Panel_InfinityCode>();
            panel_InfinityCode.ChangeView<View_InfinityCode_Ranking>();
            //web예외처리 : 만약 데이터 못 쌓으면 어떻게진행??
        });
    }
}