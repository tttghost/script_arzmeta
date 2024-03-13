using db;
using FrameWork.UI;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
public class View_QuestData : DynamicScrollData
{
    public MemberQuest memberQuest;
    public List<MemberQuestLog> memberQuestLog;

    public View_QuestData(MemberQuest memberQuest, List<MemberQuestLog> memberQuestLog)
    {
        this.memberQuest = memberQuest;
        this.memberQuestLog = memberQuestLog;
    }
}
public class View_Quest : DynamicScrollItem_Custom
{

    #region 변수 & 기본셋팅
    private TMP_Text txtmp_QuestName_Input; //8퀘스트이름
    private TMP_Text txtmp_QuestMission_Input; //9퀘스트성공조건
    private TMP_Text txtmp_QuestRewardType_Input; //10보상타입이름

    private Slider sld_QuestGauge; //11퀘스트게이지
    private TMP_Text txtmp_QuestStep_Input;//12퀘스트단계표시 ??????? 퍼센테이지랑 어떻게 할지?

    private Image img_QuestThumbnail_Input; //13퀘스트썸네일
    private GameObject go_Lock; //13락
    private GameObject go_Complete; //13해금

    private Button btn_RewardState; //14보상받기버튼
    private TMP_Text txtmp_RewardState_Input; //14보상받기텍스트

    private Panel_Quest panel_Quest; //부모인패널 가져와 캐싱
    private int res_curQuestId; //리스판스 받은 퀘스트아이디
    //private Quest master_curQuest;

    private Image img_highlight; //보상받을때 하이라이트 부분


    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        panel_Quest = GetPanel<Panel_Quest>();

        img_highlight = GetUI_Img(nameof(img_highlight));

        txtmp_QuestName_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_QuestName_Input));
        txtmp_QuestMission_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_QuestMission_Input));
        txtmp_QuestRewardType_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_QuestRewardType_Input));

        sld_QuestGauge = GetUI<Slider>(nameof(sld_QuestGauge));
        txtmp_QuestStep_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_QuestStep_Input));

        img_QuestThumbnail_Input = GetUI<Image>(nameof(img_QuestThumbnail_Input));
        go_Lock = GetChildGObject(nameof(go_Lock));
        go_Complete = GetChildGObject(nameof(go_Complete));

        btn_RewardState = GetUI_Button(nameof(btn_RewardState), OnClick_ReceiveReward);
        txtmp_RewardState_Input = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RewardState_Input));
    }
    #endregion



    #region 데이터 업데이트
    /// <summary>
    /// 무한스크롤 퀘스트데이터 업데이트
    /// </summary>
    /// <param name="scrollData"></param>
    public override void UpdateData(DynamicScrollData scrollData)
    {
        base.UpdateData(scrollData);

        //View_QuestData view_QuestData = (View_QuestData)scrollData;

        //res_curQuestId = view_QuestData.memberQuest.questId;
        //master_curQuest = panel_Quest.questDic[res_curQuestId];

        //// 8퀘스트명
        //string questName = panel_Quest.questNameTypeDic[master_curQuest.questNameType].nameId.ToString();
        //txtmp_QuestName_Input.LocalText(Cons.Local_Quest, questName); //퀘스트명!!!!!
        //txtmp_QuestName_Input.GetComponent<LocalizeStringEvent>().enabled = true;
        //// 9달성조건
        //string questMission = panel_Quest.questMissionTypeDic[master_curQuest.questMissionType].nameId.ToString();
        //txtmp_QuestMission_Input.LocalText(Cons.Local_Quest, questMission); //달성조건!!!!!!!
        //txtmp_QuestMission_Input.GetComponent<LocalizeStringEvent>().enabled = true;
        //// 10보상파츠
        //int avatarPartsId = panel_Quest.questRewardDic[res_curQuestId].avatarPartsId; //이미지!!!!!!!!
        //txtmp_QuestRewardType_Input.LocalText(/*Cons.Local_AvatarParts,*/ panel_Quest.avatarPartsDic[avatarPartsId].description);

        //// 11단계슬라이더
        //int maxStep = master_curQuest.conditionMinValue;
        //int minStep = view_QuestData.memberQuestLog[master_curQuest.questConditionType - 1].count;
        //minStep = minStep > maxStep ? maxStep : minStep;
        //sld_QuestGauge.value = (float)minStep / maxStep;

        //// 12단계텍스트
        //txtmp_QuestStep_Input.LocalText //이부분수정...
        //    (
        //    new LocalData(Cons.Local_Arzmeta, "20016",
        //        new LocalData(minStep.ToString()),
        //        new LocalData(maxStep.ToString())
        //                 )
        //    );


        ////리워드상태값 가져옴, 아래 13, 14에 사용
        //eRewardState rewardState = GetRewardState(view_QuestData.memberQuest);

        //// 13썸네일
        //string thumbNail = panel_Quest.avatarPartsDic[avatarPartsId].thumbnailName;
        //Sprite thumbNailSprite = Single.AvatarData.GetThumbnail(thumbNail);
        //img_QuestThumbnail_Input.sprite = thumbNailSprite;
        //SetThumbNailByRewardState(rewardState);

        //// 14버튼
        //SetButtonByRewardState(rewardState);
    }
    #endregion



    #region 리워드

    /// <summary>
    /// 리워드 받기
    /// </summary>
    private void OnClick_ReceiveReward()
    {
        Single.Web.Quest_ReceiveReward_Req(res_curQuestId, (res) =>
        {
            Single.Web.Quest_ReceiveReward_Res(res);
            //panel_Quest.view_QuestComplete.SetData(master_curQuest, res_curQuestId);
            panel_Quest.ChangeView(Cons.View_QuestComplete);
        });
    }

    /// <summary>
    /// 리워드상태 가져오기
    /// </summary>
    /// <param name="memberQuest"></param>
    /// <returns></returns>
    private eRewardState GetRewardState(MemberQuest memberQuest)
    {
        int isCompleted = memberQuest.isCompleted;
        int isReceived = memberQuest.isReceived;
        eRewardState eRewardState = default;
        if (isReceived == 0)
        {
            if (isCompleted == 0)
            {
                eRewardState = eRewardState.Locked;
            }
            else if (isCompleted == 1)
            {
                eRewardState = eRewardState.Completed;
            }
        }
        else
        {
            eRewardState = eRewardState.Received;
        }
        return eRewardState;
    }

    /// <summary>
    /// 리워드상태에 따른 썸네일 변경
    /// </summary>
    /// <param name="rewardState"></param>
    private void SetThumbNailByRewardState(eRewardState rewardState)
    {
        go_Lock.SetActive(false);
        go_Complete.SetActive(false);
        img_highlight.gameObject.SetActive(false);
        switch (rewardState)
        {
            case eRewardState.Locked:
                go_Lock.SetActive(true);
                break;
            case eRewardState.Completed:
                go_Complete.SetActive(true);
                img_highlight.gameObject.SetActive(true);
                break;
            case eRewardState.Received:
                img_highlight.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 리워드상태에 따른 버튼 변경
    /// </summary>
    /// <param name="rewardState"></param>
    private void SetButtonByRewardState(eRewardState rewardState)
    {
        bool interactable = false;
        string entry_ButtonText = string.Empty;
        switch (rewardState)
        {
            case eRewardState.Locked:
                entry_ButtonText = "20011";
                break;
            case eRewardState.Completed:
                interactable = true;
                entry_ButtonText = "20012";
                break;
            case eRewardState.Received:
                entry_ButtonText = "20014";
                break;
        }
        btn_RewardState.interactable = interactable;
        Util.SetMasterLocalizing(txtmp_RewardState_Input, new MasterLocalData(entry_ButtonText));
    }
    #endregion

}
