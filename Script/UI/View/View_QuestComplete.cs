using db;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MEC;

public class View_QuestComplete : UIBase
{

    //#region 변수 & 기본셋팅
    //private Panel_Quest panel_Quest;
    //private Panel_CostumeInven panel_CostumeInven;

    //private TMP_Text txtmp_RewardTitle; //1퀘스트달성타이틀
    //private TMP_Text txtmp_RewardName_Input; //2퀘스트이름
    //private TMP_Text txtmp_RewardRequirement_Input; //3퀘스트조건
    //private Image img_RewardThumbnail_Input; //4보상썸네일
    //private Button btn_Wear; //5장착하기
    //private TMP_Text txtmp_Wear; //5장착하기
    //private Button btn_Confirm; //6확인(장착안하기)
    //private TMP_Text txtmp_Confirm; //6확인(장착안하기)

    //protected override void SetMemberUI()
    //{
    //    base.SetMemberUI();
    //    panel_Quest = SceneLogic.instance.GetPanel<Panel_Quest>("Panel_Quest");
    //    panel_CostumeInven = SceneLogic.instance.GetPanel<Panel_CostumeInven>(Cons.Panel_CostumeInven);

    //    txtmp_RewardTitle = GetUI_Txtmp("txtmp_RewardTitle", Cons.Local_Arzmeta, "20006");//1퀘스트달성타이틀

    //    txtmp_RewardName_Input = GetUI_Txtmp("txtmp_RewardName_Input");//2퀘스트이름
    //    txtmp_RewardRequirement_Input = GetUI_Txtmp("txtmp_RewardRequirement_Input");//3퀘스트조건
    //    img_RewardThumbnail_Input = GetUI<Image>("img_RewardThumbnail_Input"); //4보상썸네일

    //    btn_Wear = GetUI_Button("btn_Wear", OnClick_Wear); //5장착하기 -> 논스택 푸쉬패널도필요하다..
    //    txtmp_Wear = GetUI_Txtmp("txtmp_Wear", Cons.Local_Arzmeta, "20013"); //5장착하기
    //    btn_Confirm = GetUI_Button("btn_Confirm", OnClick_Confirm); //6확인(장착안하기)
    //    txtmp_Confirm = GetUI_Txtmp("txtmp_Confirm", Cons.Local_Arzmeta, "001"); //6확인(장착안하기)
    //}

    //#endregion



    //#region 데이터 업데이트
    ////public void SetData(Quest master_curQuest, int res_curQuestId)
    ////{
    ////    // 8퀘스트명
    ////    string questName = panel_Quest.questGroupTypeDic[master_curQuest.questConditionType].name;
    ////    txtmp_RewardName_Input.LocalText(questName);

    ////    // 9달성조건
    ////    string questMission = panel_Quest.questMissionTypeDic[master_curQuest.questMissionType].nameId.ToString();
    ////    txtmp_RewardRequirement_Input.LocalText(Cons.Local_Quest, questMission); 

    ////    // 10보상파츠
    ////    avatarPartsId = panel_Quest.questRewardDic[res_curQuestId].avatarPartsId;
    ////    string thumbnail = panel_Quest.avatarPartsDic[avatarPartsId].thumbnailName;
    ////    img_RewardThumbnail_Input.sprite = Single.AvatarData.GetThumbnail(thumbnail);
    ////}
    //#endregion

    //int avatarPartsId = 0;

    //#region OnClick
    //private void OnClick_Wear()
    //{
    //    panel_Quest.UpdateData(panel_Quest.curQuestState, () =>
    //    {
    //        panel_Quest.ChangeView(string.Empty);

    //        SceneLogic.instance.SwapPanel(Cons.Panel_CostumeInven);
    //        SceneLogic.instance.SetCurOpenEndCallback(() =>
    //        {
    //            Util.RunCoroutine(Co_SetUpClothes(), "SetUpClothes");
    //        });
    //    });
    //}

    //IEnumerator<float> Co_SetUpClothes()
    //{
    //    int index = panel_Quest.avatarPartsDic[avatarPartsId].avatarPartsType - 1;
    //    ToggleIsOn(panel_CostumeInven.toggleList[index]);
    //    //Util.ToggleIsOn(panel_CostumeInven.togg_Category, panel_CostumeInven.toggleList[index]);

    //    yield return Timing.WaitUntilTrue(() => panel_CostumeInven.isSelectDone);

    //    panel_CostumeInven.ScrollTo(avatarPartsId);
    //    panel_CostumeInven.SetSaveButtonUI(true);
    //}
    //private void OnClick_Confirm()
    //{
    //    panel_Quest.UpdateData(panel_Quest.curQuestState, ()=>
    //    {
    //        panel_Quest.ChangeView(string.Empty);
    //    });
    //}
    //#endregion

}
