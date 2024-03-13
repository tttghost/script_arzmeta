using db;
using FrameWork.UI;
using Gpm.Ui.Sample;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Panel_Quest : PanelBase
{

  //  #region 변수 & 기본셋팅

  //  [HideInInspector] public View_QuestComplete view_QuestComplete;
  //  [HideInInspector] public eQuestState curQuestState;

  //  private TMP_Text txtmp_QuestTitle; //1퀘스트타이틀

  //  private ToggleGroup togg_QuestTab;
  //  private Toggle tog_CurrentQuest; //3진행중인퀘스트
  //  private TMP_Text txtmp_CurrentQuest;
  //  private Toggle tog_CompleteQuest; //4완료한퀘스트
  //  private TMP_Text txtmp_CompleteQuest;

  //  private TMP_Text txtmp_CompletePercentAll; //5전체퀘스트 달성률
  //  private TMP_Text txtmp_CompletePercentAll_Input; //7전체퀘스트 달성률 퍼센테이지
  //  private Slider sld_QuestGaugeAll; //6모든퀘스트게이지

  //  private GameObject go_Content; //item_Quest이 셋업 될 스크롤뷰의 컨텐츠영역
  //  private Button btn_Back; //15뒤로
  //  private TMP_Text txtmp_Back; //15뒤로

  //  private InfiniteScroll_Custom infiniteScroll;

  //  //딕셔너리목록
  //  //public Dictionary<int, Quest> questDic;
  //  //public Dictionary<int, QuestGroupType> questGroupTypeDic; //퀘스트 그룹(대분류)
  //  //public Dictionary<int, QuestNameType> questNameTypeDic; //퀘스트 미션(소분류)
  //  //public Dictionary<int, QuestMissionType> questMissionTypeDic; //퀘스트 미션(소분류)
  //  //public Dictionary<int, QuestReward> questRewardDic; //퀘스트 보상
  //  public Dictionary<int, AvatarParts> avatarPartsDic; //아바타파츠 목록



  //  protected override void SetMemberUI()
  //  {
  //      base.SetMemberUI();

  //      ///컴포넌트 연결
  //      txtmp_QuestTitle = GetUI_Txtmp("txtmp_QuestTitle", Cons.Local_Arzmeta, "20000");//1퀘스트타이틀

  //      togg_QuestTab = GetUI<ToggleGroup>("togg_QuestTab");
  //      tog_CurrentQuest = GetUI_Toggle("tog_CurrentQuest", () => UpdateData(eQuestState.Playing));//3진행중인퀘스트
  //      txtmp_CurrentQuest = GetUI_Txtmp("txtmp_CurrentQuest", Cons.Local_Arzmeta, "20002");
  //      tog_CompleteQuest = GetUI_Toggle("tog_CompleteQuest", () => UpdateData(eQuestState.Completed));//4완료한퀘스트
  //      txtmp_CompleteQuest = GetUI_Txtmp("txtmp_CompleteQuest", Cons.Local_Arzmeta, "20003");

  //      txtmp_CompletePercentAll = GetUI_Txtmp("txtmp_CompletePercentAll", Cons.Local_Arzmeta, "20004");//5전체달성률
  //      txtmp_CompletePercentAll_Input = GetUI_Txtmp("txtmp_CompletePercentAll_Input");//7모든퀘스트 달성률 퍼센테이
  //      sld_QuestGaugeAll = GetUI<Slider>("sld_QuestGaugeAll"); //6모든퀘스트게이지

  //      go_Content = GetChildGObject("go_Content");//item_Quest이 셋업 될 스크롤뷰의 컨텐츠영역
		//btn_Back = GetUI_Button("btn_Back",
		//	() =>
		//	{
		//		SceneLogic.instance.Back();

		//		if (ArzMetaManager.Instance.PhoneController.isPhone)
		//		{
		//			SceneLogic.instance.PushPanel<Panel_Phone>(Cons.Panel_Phone);
		//		}
		//	}
		//);
		//txtmp_Back = GetUI_Txtmp("txtmp_Back", Cons.Local_Arzmeta, "001");
  //      infiniteScroll = GetComponentInChildren<InfiniteScroll_Custom>();
  //      infiniteScroll.itemPrefab = Single.Resources.Load<InfiniteScrollItem_Custom>(Cons.Path_Prefab_View + "View_Quest");
  //      view_QuestComplete = GetView<View_QuestComplete>(Cons.View_QuestComplete);

  //      InitDictionary();

  //  }

  //  /// <summary>
  //  /// 딕셔너리 셋팅
  //  /// </summary>
  //  public void InitDictionary()
  //  {
  //      //questDic = Single.MasterData.dataQuest.GetDictionary();
  //      //questGroupTypeDic = Single.MasterData.dataQuestGroupType.GetDictionary();
  //      //questNameTypeDic = Single.MasterData.dataQuestNameType.GetDictionary();
  //      //questMissionTypeDic = Single.MasterData.dataQuestMissionType.GetDictionary();
  //      //questRewardDic = Single.MasterData.dataQuestReward.GetDictionary();
  //      avatarPartsDic = Single.MasterData.dataAvatarParts.GetDictionary_int();
  //  }
  //  protected override void OnEnable()
  //  {
  //      base.OnEnable();
  //      ToggleIsOn(tog_CurrentQuest);
  //      //Util.ToggleIsOn(togg_QuestTab, tog_CurrentQuest);
  //  }

  //  protected override void OnDisable()
  //  {
  //      base.OnDisable();
  //      infiniteScroll.ClearData();
  //  }

  //  #endregion

 

  //  #region 데이터 업데이트
  //  /// <summary>
  //  /// 내 퀘스트정보 최신화
  //  /// </summary>
  //  /// <param name="eQuestState">퀘스트 상태, 현재진행형퀘스트, 완료한퀘스트</param>
  //  /// <param name="act">패널 닫고 나갈때 필요한 액션</param>
  //  public void UpdateData(eQuestState eQuestState, Action act = null)
  //  {
  //      curQuestState = eQuestState;
  //      Single.Web.Quest_GetMemberInfo_Req((res) =>
  //      {
  //          Single.Web.Quest_GetMemberInfo_Res(res);
  //          UpdateItem(eQuestState, res);
  //          act?.Invoke();
  //      });
  //  }


  //  /// <summary>
  //  /// 퀘스트상태에 따른 무한스크롤 아이템 업데이트
  //  /// </summary>
  //  /// <param name="eQuestState"></param>
  //  /// <param name="res"></param>
  //  public void UpdateItem(eQuestState eQuestState, QuestGetMemberInfoRes res)
  //  {
  //      //무한스크롤 아이템 초기화
  //      infiniteScroll.ClearData();

  //      //무한스크롤 아이템 업데이트
  //      List<MemberQuest> curMemberQuestList = new List<MemberQuest>();
  //      switch (eQuestState)
  //      {
  //          case eQuestState.Playing: curMemberQuestList = res.memberQuest.ToList(); break;
  //          case eQuestState.Completed: curMemberQuestList = res.completedMemberQuest.ToList(); break;
  //      }
  //      curMemberQuestList = curMemberQuestList.OrderByDescending(x => eRewardState.Completed == GetRewardState(x)).ToList();
  //      for (int i = 0; i < curMemberQuestList.Count; i++)
  //      {
  //          infiniteScroll.InsertData(new View_QuestData(curMemberQuestList[i], res.memberQuestLog.ToList()));
  //      }

  //      //슬라이더 게이지 및 퍼센트
  //      //sld_QuestGaugeAll.value = (float)res.completedMemberQuest.ToList().Count / Single.MasterData.dataQuest.GetList().Count;
  //      txtmp_CompletePercentAll_Input.text = (sld_QuestGaugeAll.value * 100).ToString("##0") + " %";

  //  }
  //  #endregion

  //  /// <summary>
  //  /// 리워드상태 가져오기
  //  /// </summary>
  //  /// <param name="memberQuest"></param>
  //  /// <returns></returns>
  //  private eRewardState GetRewardState(MemberQuest memberQuest)
  //  {
  //      int isCompleted = memberQuest.isCompleted;
  //      int isReceived = memberQuest.isReceived;
  //      eRewardState eRewardState = default;
  //      if (isReceived == 0)
  //      {
  //          if (isCompleted == 0)
  //          {
  //              eRewardState = eRewardState.Locked;
  //          }
  //          else if (isCompleted == 1)
  //          {
  //              eRewardState = eRewardState.Completed;
  //          }
  //      }
  //      else
  //      {
  //          eRewardState = eRewardState.Received;
  //      }
  //      return eRewardState;
  //  }
}
