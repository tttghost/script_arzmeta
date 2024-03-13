using FrameWork.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using MEC;

public class Panel_NPC : PanelBase
{

    ////private GameObject go_RawImage;
    //private GameObject btn_NPCBtn;
    //private GameObject go_NPCBtns;
    //private TMP_Text txtmp_Nickname;
    //private TMP_Text txtmp_Dialogue;
    //private Button btn_Next;
    //private GameObject go_Next;
    //private Button btn_Exit;
    //private Action closeAct = null;
    //private NPCs curNPCs;
    //private eSessionType type = eSessionType.MAIN;
    //protected override void SetMemberUI()
    //{
    //    base.SetMemberUI();
    //    //go_RawImage = GetChildGObject("go_RawImage");
    //    btn_NPCBtn = Single.Resources.Load<GameObject>(Cons.Path_Prefab_NPC + "btn_NPCBtn");
    //    go_NPCBtns = GetChildGObject("go_NPCBtns");
    //    for (int i = 0; i < 3; i++)
    //    {
    //        Instantiate(btn_NPCBtn, go_NPCBtns.transform);
    //    }
    //    txtmp_Nickname = GetUI_Txtmp("txtmp_Nickname");
    //    txtmp_Dialogue = GetUI_Txtmp("txtmp_Dialogue");
    //    btn_Next = GetUI_Button("btn_Next");
    //    go_Next = GetChildGObject("go_Next");
    //    btn_Exit = GetUI_Button("btn_Exit", () => SceneLogic.instance.Back());

    //}

    ///// <summary>
    ///// 초기 셋업
    ///// </summary>
    //public void SetStart(int id, NPCs npcs, Action act)
    //{
    //    db.Npc npc = Single.MasterData.dataNpc.GetData(id);
    //    txtmp_Nickname.LocalText(Cons.Local_NPC, npc.nameId.ToString());

    //    curNPCs = npcs;

    //    closeAct = act;

    //    SetUpdate(npc.seqId, npc.id);

    //    //if(2 == id)
    //    //{
    //    //    btn_Next.gameObject.SetActive(true);
    //    //    go_Next.SetActive(true);
    //    //    //btn_Next.onClick.AddListener(GoTutorial);
    //    //    btn_Next.onClick.AddListener(OpenTutorial);
    //    //}
    //}
    //private void OnDisable()
    //{
    //    closeAct?.Invoke();
    //}

    ///// <summary>
    ///// 재귀로 엔피씨 계속  도는 함수
    ///// </summary>
    ///// <param name="sequence"></param>
    //private void SetUpdate(int sequence, int id)
    //{
    //    btn_Next.onClick.RemoveAllListeners();
    //    btn_Next.gameObject.SetActive(true);
    //    go_Next.SetActive(false);
    //    go_NPCBtns.SetActive(false);

    //    db.NpcSeq npcSeq = Single.MasterData.dataNpcSeq.GetData(sequence);
    //    db.NpcSeqAct npcSeqAct = Single.MasterData.dataNpcSeqAct.GetData(sequence);
    //    txtmp_Dialogue.LocalText(Cons.Local_NPC, npcSeq.textId.ToString());
    //    curNPCs.SetAnimation(npcSeq.aniName);
    //    if(npcSeqAct.seqIdAct1 == 0) //액션이 없을 때
    //    {
    //        btn_Next.onClick.AddListener(() =>
    //        {
    //            if (id == 2)
    //            {
    //                Single.Sound.PlayEffect("voice_npcbird_1", 0.7f);// 사운드 추가 - BKK -

    //                OpenTutorial();
    //            }
    //            else if (id == 4) //shark면
    //            {
    //                SceneLogic.instance.PushPopupBasic(Cons.Popup_Basic,
    //                    new PopupData(POPUPICON.NONE, string.Empty, new MasterLocalData("common_confirm_gamezone_enter"),
    //                        BTN_TYPE.ConfirmCancel),
    //                    new PopupAction(() =>
    //                    {
    //                        // 나갔다가 돌아왔을 때 비어있는 팝업이 생길 수 있음
    //                        LocalContentsData.SetTransformPlayerPrefs(new Vector3(23.5f, -8.76549f, -4.154357f), new Vector3(0, -35.376f, 0));
    //                        Util.RunCoroutine(CoMoveScene(Cons.Scene_GameZone));
    //                    }));
    //            }
    //            else
    //            {
    //                Single.Sound.PlayEffect("voice_npcbye_0", 0.7f);// 사운드 추가 - BKK -

    //                SceneLogic.instance.Back();
    //            }

    //        });

    //        go_Next.SetActive(true);
    //    }
    //    else if(npcSeqAct.seqIdAct2 == 0) //액션이 한가지 일때 (단일진행)
    //    {
    //        btn_Next.onClick.AddListener(() =>
    //        {
    //            Random.InitState((int) (Time.time * 100));
    //            if (id == 2)
    //            {
    //                Single.Sound.PlayEffect($"voice_npcbird_{Random.Range(0, 1)}", 0.7f);// 사운드 추가 - BKK -
    //            }
    //            else
    //            {
    //                Single.Sound.PlayEffect($"voice_talk_{Random.Range(0, 4)}", 0.7f);// 사운드 추가 - BKK -
    //            }
    //            SetUpdate(npcSeqAct.seqIdAct1, id);
    //        });
    //        go_Next.SetActive(true);
    //    }
    //    else //액션이 여러가지 일때 (다중선택)
    //    {
    //        btn_Next.gameObject.SetActive(false);
    //        go_NPCBtns.SetActive(true);
    //        for (int i = 0; i < go_NPCBtns.transform.childCount; i++)
    //        {
    //            go_NPCBtns.transform.GetChild(0).gameObject.SetActive(false);
    //        }
    //        if (npcSeqAct.seqIdAct1 != 0)
    //        {
    //            SetAction(0, npcSeqAct.seqIdAct1, npcSeqAct.seqIdText1, id);
    //        }
    //        if (npcSeqAct.seqIdAct2 != 0)
    //        {
    //            SetAction(1, npcSeqAct.seqIdAct2, npcSeqAct.seqIdText2, id);
    //        }
    //        if (npcSeqAct.seqIdAct3 != 0)
    //        {
    //            SetAction(2, npcSeqAct.seqIdAct3, npcSeqAct.seqIdText3, id);
    //        }
    //    }
    //}

    //private void SetAction(int idx, int seq, int text, int id) 
    //{
    //    GameObject go = go_NPCBtns.transform.GetChild(idx).gameObject;
    //    Button btn = go.GetComponentInChildren<Button>();
    //    TMP_Text txtmp = go.GetComponentInChildren<TMP_Text>();
    //    go.SetActive(true);
    //    btn.onClick.AddListener(() => SetUpdate(seq, id));
    //    txtmp.LocalText(Cons.Local_NPC, text.ToString()); //여기까지!!!!
    //}

    ////private void OpenTutorial()
    ////{
    ////    this.SetCloseEndCallback();
    ////    btn_Next.onClick.RemoveListener(OpenTutorial);
    ////    SceneLogic.instance.Back();
    ////    //Debug.Log("Dustin: after Back()");
    ////    SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial).gameObject.SetActive(true);

    ////    //PushPanel 사용하려는 시도
    ////    //Panel_Tutorial panel_Tutorial = SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial);
    ////    //SceneLogic.instance.PushPanel(panel_Tutorial, false);
    ////    //Debug.Log("Dustin: panel_Tutorial is " + panel_Tutorial);
    ////    ////SceneLogic.instance._stackPanels.Pop();
    ////}

    ////private void GoTutorial()
    ////{
    ////    ////기존 잘 작동하는 코드
    ////    //SceneLogic.instance.Back();
    ////    //btn_Next.onClick.RemoveListener(GoTutorial);
    ////    //SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial).gameObject.SetActive(true);


    ////    //to do 
    ////    this.SetCloseEndCallback(OpenTutorial);
    ////    btn_Next.onClick.RemoveListener(GoTutorial);
    ////    SceneLogic.instance.Back();
    ////    //Debug.Log("Dustin: after Back()");

    ////    //PushPanel 사용하려는 시도
    ////    //Panel_Tutorial panel_Tutorial = SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial);
    ////    //SceneLogic.instance.PushPanel(panel_Tutorial, false);
    ////    //Debug.Log("Dustin: panel_Tutorial is " + panel_Tutorial);
    ////    ////SceneLogic.instance._stackPanels.Pop();
    ////}

    //public void OpenTutorial()
    //{
    //    //DEBUG.LOG("Dustin : in OpenTutorial");
    //    Panel_Tutorial panel_Tutorial = SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial);
    //    panel_Tutorial.InitializeCurrentPanel();
    //    SceneLogic.instance.PushPanel(panel_Tutorial, false);
    //    //btn_Next.onClick.RemoveListener(OpenTutorial);

    //    //if (Co_OpenTutorialWrapper != null)
    //    //{
    //    //    StopCoroutine(Co_OpenTutorialWrapper);
    //    //}
    //    //Co_OpenTutorialWrapper = StartCoroutine(Co_OpenTutorial());
    //}

    ////public Coroutine Co_OpenTutorialWrapper = null;
    ////public IEnumerator Co_OpenTutorial()
    ////{
    ////    yield return new WaitForSeconds(3f);

    ////    DEBUG.LOG("Dustin : in OpenTutorial");
    ////    Panel_Tutorial panel_Tutorial = SceneLogic.instance.GetPanel<Panel_Tutorial>(Cons.Panel_Tutorial);
    ////    SceneLogic.instance.PushPanel(panel_Tutorial, false);
    ////}

    ///// <summary>
    ///// 씬 이동
    ///// </summary>
    ///// <returns></returns>
    //IEnumerator<float> CoMoveScene(string sceneName)
    //{
    //    RealtimeUtils.SyncStop(type);

    //    Single.Scene.SetDimOn();

    //    yield return Timing.WaitUntilTrue(() => RealtimeUtils.IsSyncStop(type));

    //    Single.Scene.LoadScene(sceneName);

    //}
}



