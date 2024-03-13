using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class View_Godowon : UIBase
{
    [HideInInspector] public GodowonState eGodowonState;
    public enum GodowonState
    {
        Title,
        Letter,
        Godowon,
    }

    private GameObject go_GodowonTitle;
    private GameObject go_GodowonLetter;

    private CanvasWebViewPrefab canvasWebViewPrefab;
    private Button btn_Back;
    private const string godowonUrl = "https://www.godowon.com/last_letter/hancom.gdw";


    private TMP_Text txtmp_Homepage;
    private TMP_Text txtmp_Letter;
    private Button btn_Homepage;
    private Button btn_Letter;
    private Button btn_OpenLetter;

    private TMP_Text txtmp_GodowonBack;

    private Animator letterAni;
    private Image img_new_mark_01;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_GodowonTitle = GetChildGObject(nameof(go_GodowonTitle));

        txtmp_Homepage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Homepage), new MasterLocalData("mail_morningmail_homepage"));
        btn_Homepage = GetUI_Button(nameof(btn_Homepage), OnClick_Homepage);

        txtmp_Letter = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Letter), new MasterLocalData("mail_morningmail_today"));
        btn_Letter = GetUI_Button(nameof(btn_Letter), OnClick_Letter);


        go_GodowonLetter = GetChildGObject(nameof(go_GodowonLetter));
        letterAni = GetChildGObject("go_LetterAni").GetComponent<Animator>();
        btn_OpenLetter = GetUI_Button(nameof(btn_OpenLetter), OnClick_OpenLetter);
        //btn_OpenGodowon = GetUI_Button(nameof(btn_OpenGodowon), OnClick_OpenGodowon);
        canvasWebViewPrefab = GetComponentInChildren<CanvasWebViewPrefab>(true);

        btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
        //txtmp_GodowonBack = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GodowonBack), new MasterLocalData("common_back"));

        img_new_mark_01 = GetUI_Img(nameof(img_new_mark_01));
    }


    /// <summary>
    /// 시간체크해서 New아이콘 활성화/비활성화
    /// </summary>
    /// <param name="b"></param>
    private void Callback_CheckTime(bool b)
    {
        img_new_mark_01.gameObject.SetActive(!b);
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        ChangeGodowonState(GodowonState.Title);

        Callback_CheckTime(LocalPlayerData.Method.GetCheckTime());
        LocalPlayerData.Method.Handler_CheckTime += Callback_CheckTime;
        GetPanel<Panel_Mailbox>().BackAction_Custom += OnClick_Back;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LocalPlayerData.Method.Handler_CheckTime -= Callback_CheckTime;
        GetPanel<Panel_Mailbox>().BackAction_Custom -= OnClick_Back;
    }

    /// <summary>
    /// 편지패널 켜기
    /// </summary>
    private void OnClick_Letter()
    {
        ChangeGodowonState(GodowonState.Letter);
    }

    /// <summary>
    /// 편지 열기
    /// </summary>
    private void OnClick_OpenLetter()
    {
        LocalPlayerData.Method.SetCheckTime();
        btn_OpenLetter.gameObject.SetActive(false);
        letterAni.Play("animation_GodowonLetter");
    }

    /// <summary>
    /// 고도원페이지 오픈
    /// </summary>
    public void OnClick_OpenGodowon()
    {
        ChangeGodowonState(GodowonState.Godowon);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eGodowonState"></param>
    private void ChangeGodowonState(GodowonState eGodowonState)
    {
        this.eGodowonState = eGodowonState;

        go_GodowonTitle.SetActive(false);
        go_GodowonLetter.SetActive(false);
        canvasWebViewPrefab.gameObject.SetActive(false);
        GetPanel<Panel_Mailbox>().go_Center.SetActive(false);
        switch (eGodowonState)
        {
            case GodowonState.Title:
                go_GodowonTitle.SetActive(true);
                GetPanel<Panel_Mailbox>().go_Center.SetActive(true);
                break;
            case GodowonState.Letter:
                go_GodowonLetter.SetActive(true);
                btn_OpenLetter.gameObject.SetActive(true);
                letterAni.Play("animation_GodowonLetterFirst");
                break;
            case GodowonState.Godowon:
                canvasWebViewPrefab.gameObject.SetActive(true);
                LoadURL(godowonUrl);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnClick_Back()
    {
        switch (eGodowonState)
        {
            case GodowonState.Title:
                GetPanel<Panel_Mailbox>().BackAction_Custom -= OnClick_Back;
                Back();
                break;
            case GodowonState.Letter:
                ChangeGodowonState(GodowonState.Title);
                break;
            case GodowonState.Godowon:
                ChangeGodowonState(GodowonState.Letter);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnClick_Homepage()
    {
        Application.OpenURL("https://www.godowon.com");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_url"></param>
    private async void LoadURL(string _url)
    {
        //canvasWebViewPrefab.WebView.StopLoad();
        var cleared = _url.Trim();
        var replaced = cleared.Replace(" ", "%20");

        await canvasWebViewPrefab.WaitUntilInitialized(); //변고경 김지수 꿀팁
        canvasWebViewPrefab.WebView.LoadUrl(replaced);
    }
}
