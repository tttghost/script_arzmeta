using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameWork.UI;
using UnityEngine.UI;
using System;
using TMPro;

//dustin: 예전 코드 갈아엎음
//public class Panel_Tutorial : PanelBase
//{
//    private tutorial_page current_page;

//    public tutorial_page Current_page
//    {
//        get { return current_page; }
//        set 
//        {
//            //to do
//            switch (value)
//            {
//                case tutorial_page.One:
//                    gestureGuideImage.gameObject.SetActive(true);
//                    gestureGuideImage.sprite = gestureGuideSprites[0];

//                    //dialogueText.LocalText(Cons.Local_NPC, "2002");
//                    dialogueText.text = dialogueStrings[0];

//                    tapGuideText.text = tapGuideStrings[0];
//                    break;
//                case tutorial_page.Two:
//                    gestureGuideImage.sprite = gestureGuideSprites[1];
//                    dialogueText.text = dialogueStrings[1];
//                    break;
//                case tutorial_page.Three:
//                    gestureGuideImage.gameObject.SetActive(false);
//                    dialogueText.text = dialogueStrings[2];
//                    tapGuideText.text = tapGuideStrings[1];
//                    break;
//                default:
//                    break;
//            }

//            current_page = value; 
//            Debug.Log("Dustin: tutorial current is " + current_page);
//        }
//    }

//    //private Button btn_next;
//    //private Button btn_Next;

//    [SerializeField] private Image gestureGuideImage;
//    public Image GestureGuideImage
//    {
//        get { return gestureGuideImage; }
//        set { gestureGuideImage = value; }
//    }

//    [SerializeField] private TMP_Text dialogueText;
//    public TMP_Text DialogueText
//    {
//        get { return dialogueText; }
//        set { dialogueText = value; }
//    }

//    [SerializeField] private TMP_Text tapGuideText;
//    public TMP_Text TapGuideText
//    {
//        get { return tapGuideText; }
//        set { tapGuideText = value; }
//    }

//    public List<Sprite> gestureGuideSprites;
//    public List<string> dialogueStrings;
//    public List<string> tapGuideStrings;



//    protected override void SetMemberUI()
//    {
//        Debug.Log("Dustin:  in SetMemberUI ");

//        //initializing data
//        dialogueStrings.Add("화면을 확대·축소하면 시점을 조절할 수 있어 !");
//        dialogueStrings.Add("화면을 좌·우로 드래그하면 원하는 방향으로돌려 볼 수 있어!");
//        dialogueStrings.Add("다양한 기능들이 준비되어 있으니 해봐.\n 코드게이트 2022! 대회 즐기러 출발해! 출발해");
//        tapGuideStrings.Add("탭하여 넘어가기");
//        tapGuideStrings.Add("탭하여 가이드 종료");

//        //Setting current page
//        Current_page = tutorial_page.One;

//        //btn_next = GetUI_Button("btn_next");
//        //btn_Next = GetUI_Button("btn_Next");
//        //btn_next.onClick.AddListener(() => GoNextPage());
//        //btn_Next.onClick.AddListener(() => GoNextPage());
//    }

//    public void GoNextPage()
//    {
//        Debug.Log("Dustin:  in GoNextPage ");
//        if ((int)Current_page < 2)
//        {
//            Current_page++;
//        }
//        else
//        {
//            Current_page = tutorial_page.One;

//            gameObject.SetActive(false);
//        }
//    }
//}
public class Panel_Tutorial : PanelBase
{
    private Image img_BG;
    private Image img_GestureGuide;
    //private Image img_Profile;
    //private Image img_Balloon;
    private TMP_Text txtmp_Dialogue;
    private TMP_Text txtmp_TapGuide;
    private TMP_Text txtmp_GUIGuide1;
    private TMP_Text txtmp_GUIGuide2;
    private TMP_Text txtmp_GUIGuide3;
    private TMP_Text txtmp_GUIGuide4;
    private TMP_Text txtmp_GUIGuide5;
    private TMP_Text txtmp_GUIGuide6;
    private TMP_Text txtmp_GUIGuide7;
    private Button btn_Next;

    public List<Sprite> sprites_GestureGuide;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        img_BG = GetUI_Img(nameof(img_BG));
        img_GestureGuide = GetUI_Img(nameof(img_GestureGuide));
        //img_Profile = GetUI_Img("img_Profile");
        //img_Balloon = GetUI_Img("img_Balloon");
        txtmp_Dialogue = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Dialogue));
        txtmp_TapGuide = GetUI_TxtmpMasterLocalizing(nameof(txtmp_TapGuide));
        btn_Next = GetUI_Button(nameof(btn_Next), GoNextPage);
        txtmp_GUIGuide1 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide1), new MasterLocalData("2005"));
        txtmp_GUIGuide2 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide2), new MasterLocalData("2006"));
        txtmp_GUIGuide3 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide3), new MasterLocalData("2007"));
        txtmp_GUIGuide4 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide4), new MasterLocalData("2008"));
        txtmp_GUIGuide5 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide5), new MasterLocalData("2009"));
        txtmp_GUIGuide6 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide6), new MasterLocalData("2010"));
        txtmp_GUIGuide7 = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GUIGuide7), new MasterLocalData("2011"));

        InitializeCurrentPanel();
    }

    private tutorial_page current_page;

    public tutorial_page Current_page
    {
        get { return current_page; }
        set
        {
            //to do
            switch (value)
            {
                case tutorial_page.One:
                    img_GestureGuide.gameObject.SetActive(true);
                    img_GestureGuide.sprite = sprites_GestureGuide[0];
                    Util.SetMasterLocalizing(txtmp_Dialogue, new MasterLocalData("2001"));
                    Util.SetMasterLocalizing(txtmp_TapGuide, new MasterLocalData("2002"));
                    txtmp_GUIGuide1.gameObject.SetActive(false);
                    txtmp_GUIGuide2.gameObject.SetActive(false);
                    txtmp_GUIGuide3.gameObject.SetActive(false);
                    txtmp_GUIGuide4.gameObject.SetActive(false);
                    txtmp_GUIGuide5.gameObject.SetActive(false);
                    txtmp_GUIGuide6.gameObject.SetActive(false);
                    txtmp_GUIGuide7.gameObject.SetActive(false);
                    break;
                case tutorial_page.Two:
                    img_GestureGuide.sprite = sprites_GestureGuide[1];
                    Util.SetMasterLocalizing(txtmp_Dialogue, new MasterLocalData("2003"));
                    break;
                case tutorial_page.Three:
                    img_GestureGuide.gameObject.SetActive(false);
                    Util.SetMasterLocalizing(txtmp_Dialogue, new MasterLocalData("2004"));
                    Util.SetMasterLocalizing(txtmp_TapGuide, new MasterLocalData("2012"));
                    txtmp_GUIGuide1.gameObject.SetActive(true);
                    txtmp_GUIGuide2.gameObject.SetActive(true);
                    txtmp_GUIGuide3.gameObject.SetActive(true);
                    txtmp_GUIGuide4.gameObject.SetActive(true);
                    txtmp_GUIGuide5.gameObject.SetActive(true);
                    txtmp_GUIGuide6.gameObject.SetActive(true);
                    txtmp_GUIGuide7.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            current_page = value;
        }
    }

    public void InitializeCurrentPanel()
    {
        Current_page = tutorial_page.One;
    }

    public void GoNextPage()
    {
        Single.Sound.PlayEffect(Cons.click);

        if ((int)Current_page < 2)
        {
            Current_page++;
        }
        else
        {
            //gameObject.SetActive(false);
            //SceneLogic.instance._stackPanels.Pop();
            SceneLogic.instance.PopPanel(2);
        }
    }
}
