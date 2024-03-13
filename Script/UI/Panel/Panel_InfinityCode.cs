using FrameWork.UI;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_InfinityCode : PanelBase
{
    public UIBase prevView; //직전 뷰, 뷰랭크-리트라이버튼 제어용
    public List<int> rankMedal = new List<int>();
    public float worldBestScore;
    public float myBestScore;
    private Transform STACKCAMERATr;

    protected override void OnEnable()
    {
        base.OnEnable();

        if(STACKCAMERATr == null)
        {
            STACKCAMERATr = SceneLogic.instance.stackCamera.transform;
        }
        STACKCAMERATr.transform.position = new Vector3(0f, 4f, -4f);
        STACKCAMERATr.transform.eulerAngles = Vector3.right * 20f;

        Single.Sound.PlayBGM("bgm_game_1");

        ClosePanel<Panel_HUD>();

        ChangeView<View_InfinityCode_Intro>();
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        if (STACKCAMERATr != null)
        {
            STACKCAMERATr.transform.position = Vector3.zero;
            STACKCAMERATr.transform.eulerAngles = Vector3.zero;
        }

        Single.Sound.PlayBGM("bgm_game_0");

        OpenPanel<Panel_HUD>();
    }
    public override T ChangeView<T>(bool leave = false)
    {
        prevView = GetActiveView();
        return base.ChangeView<T>(leave);
    }
    public override void Back(int cnt = 1)
    {
        if (GetActiveView() is View_InfinityCode_Intro)
        {
            base.Back(cnt);
        }
        else if (GetActiveView() is View_InfinityCode_Ranking
            || GetActiveView() is View_InfinityCode_Guide
            || GetActiveView() is View_InfinityCode_Game
            )
        {
            ChangeView<View_InfinityCode_Intro>();
        }
    }
}
