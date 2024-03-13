//using Coffee.UISoftMask;
//using FrameWork.UI;
//using MEC;
//using Office;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.Linq;
//using UnityEngine;
//using UnityEngine.UI;

//public class Panel_OfficeTutorial : PanelBase
//{
//    //public List<GameObject> go_Contents = new List<GameObject>();
//    public Queue<Step> go_StepQueue = new Queue<Step>();

//    private GameObject curOfficeTutorial;
//    private Step curStep;

//    private Button btn_NextStep;
//    private Button btn_Skip;

//    protected override void SetMemberUI()
//    {
//        base.SetMemberUI();
//        btn_NextStep = GetUI_Button(nameof(btn_NextStep), () => OnClick_NextStep());
//        btn_Skip = GetUI_Button(nameof(btn_Skip), () => OnClick_Skip());
//        BackAction_Panel = OnClick_Skip;
//    }

//    /// <summary>
//    /// 튜토리얼 시작부분
//    /// </summary>
//    /// <param name="tutorialName"></param>
//    public Panel_OfficeTutorial StartTutorial(string tutorialName)
//    {
//        //if(!string.IsNullOrEmpty(PlayerPrefs.GetString(tutorialName))) // 해당 튜토리얼을 진행한 적이 있으면
//        //{
//        //   return null;
//        //}
//        //PlayerPrefs.SetString(tutorialName, "isDone");

//        curOfficeTutorial = gameObject.Child(tutorialName);
//        curOfficeTutorial.SetActive(true);

//        //List<Step> stepList = curOfficeTutorial.Descendants().Where(x => x.GetComponent<Step>()).OfComponent<Step>().ToList();
//        List<Step> stepList = curOfficeTutorial.GetComponentsInChildren<Step>(true).ToList();
//        stepList.ForEach(x => x.gameObject.SetActive(false));
//        go_StepQueue = new Queue<Step>(stepList);

//        curStep = null;
//        OnClick_NextStep();
//        return this;
//    }

//    /// <summary>
//    /// 오피스 더미 추천목록 보기
//    /// </summary>
//    public void OnClick_OfficeRecommendDummyCreate()
//    {

//        View_OfficeRoomEnter View_OfficeRoomEnter = GetPanel<Panel_OfficeRoom>().GetView<View_OfficeRoomEnter>();
//        //View_OfficeRoomEnter.tog_RecommendList.isOn = true;
//        ToggleIsOn(View_OfficeRoomEnter.tog_RecommendList);
//        View_OfficeRoomEnter.GetView<View_OfficeRoomRecommendList>().CreateDummy();

//    }
//    public void OnClick_OfficeRecommendDummyClear()
//    {
//        GetPanel<Panel_OfficeRoom>()
//        .GetView<View_OfficeRoomEnter>()
//        .GetView<View_OfficeRoomRecommendList>()
//        .ClearDummy();
//    }

//    /// <summary>
//    /// 오피스 더미 토픽목록 보기
//    /// </summary>
//    public void OnClick_OfficeTopicDummyCreate()
//    {

//        View_OfficeRoomEnter View_OfficeRoomEnter = GetPanel<Panel_OfficeRoom>().GetView<View_OfficeRoomEnter>();
//        //View_OfficeRoomEnter.tog_TopicRoom.isOn = true;
//        ToggleIsOn(View_OfficeRoomEnter.tog_TopicRoom);
//        View_OfficeRoomEnter.GetView<View_OfficeRoomTopicList>().CreateDummy();
//    }
//    /// </summary>
//    public void OnClick_OfficeTopicDummyClear()
//    {
//        GetPanel<Panel_OfficeRoom>()
//          .GetView<View_OfficeRoomEnter>()
//          .ChangeView<View_OfficeRoomTopicList>()
//          .ClearDummy();
//    }

//    public void OnClick_OfficeCreate()
//    {
//        Popup_OfficeRoomCreate popup_OfficeRoomCreate = SceneLogic.instance.GetPopup<Popup_OfficeRoomCreate>();
//        popup_OfficeRoomCreate.PushPopup();
//        popup_OfficeRoomCreate.Create_OfficeRoom(eOfficeModeType.MEETING, eOfficeOpenType.즉시개설);
//    }

//    /// <summary>
//    /// 소프트 마스크 enable
//    /// </summary>
//    /// <param name="go"></param>
//    /// <param name="alpha"></param>
//    private void SetSoftMask(Step go, float alpha)
//    {
//        SoftMask[] SoftMask = go.transform.GetComponentsInParent<SoftMask>();
//        foreach (var item in SoftMask)
//        {
//            item.alpha = alpha;
//        }
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="delayTime"></param>
//    /// <returns></returns>
//    IEnumerator<float> Co_NextStep()
//    {
//        if (curStep)
//        {
//            curStep.onClose?.Invoke();
//            SetSoftMask(curStep, 0f);
//            curStep.gameObject.SetActive(false);
//        }
//        btn_NextStep.interactable = false;
//        btn_Skip.interactable = false;
//        if (go_StepQueue.Count == 0)
//        {
//            OnClick_Skip();
//            yield break;
//        }
//        curStep = go_StepQueue.Dequeue();
//        yield return Timing.WaitForSeconds(curStep.openDelay);

//        Util.CopyRectTransform(btn_NextStep.GetComponent<RectTransform>(), curOfficeTutorial.GetComponent<RectTransform>());

//        GameObject target = curStep.gameObject.Child("go_Target");
//        if (target)
//        {
//            Util.CopyRectTransform(btn_NextStep.GetComponent<RectTransform>(), target.GetComponent<RectTransform>());
//        }

//        curStep.onOpen?.Invoke();
//        SetSoftMask(curStep, 1f);
//        curStep.gameObject.SetActive(true);

//        btn_NextStep.interactable = true;
//        btn_Skip.interactable = true;
//    }

//    /// <summary>
//    /// 넥스트스탭 클릭
//    /// </summary>
//    public void OnClick_NextStep()
//    {
//        Co_NextStep().RunCoroutine();
//    }

//    public void OnClick_Skip()
//    {
//        curOfficeTutorial.SetActive(false);
//        SceneLogic.instance.PopPanel();
//    }
//}
