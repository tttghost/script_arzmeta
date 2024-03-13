using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using db;

public class Popup_BizCardSelect : PopupBase
{
    #region 변수
    private List<BusinessCardTemplate> bizTemplates = new List<BusinessCardTemplate>();
    private List<BizCardBase> bizCards = new List<BizCardBase>();

    private Transform go_CardRig;
    private TMP_Text txtmp_Purchase;
    private Button btn_Select;
    private Button btn_Purchase;

    private int bizTemplateCount;
    private int curIndex;

    private Panel_BizCard panel_BizCard;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("businesscard_templet_title"));
        GetUI_TxtmpMasterLocalizing("txtmp_Select", new MasterLocalData("common_choice"));
        #endregion

        #region Button
        btn_Select = GetUI_Button(nameof(btn_Select), OnClick_Select);
        btn_Purchase = GetUI_Button(nameof(btn_Purchase), () => { });

        GetUI_Button("btn_Exit", SceneLogic.instance.PopPopup);
        GetUI_Button("btn_Pre", () => SetActiveTemplate(curIndex--));
        GetUI_Button("btn_Next", () => SetActiveTemplate(curIndex++));
        #endregion

        #region etc
        go_CardRig = GetChildGObject(nameof(go_CardRig)).transform;
        #endregion
    }

    #region 초기화
    protected override void Awake()
    {
        base.Awake();

        panel_BizCard = GetPanel<Panel_BizCard>();
        CreateBizCard();
    }

    protected override void OnEnable()
    {
        UpdateCardInfo();

        curIndex = 0;
        SetActiveTemplate(curIndex);
        SetActiveSelectOrPurchase();
    }

    /// <summary>
    /// 명함 종류별 프리팹 생성
    /// </summary>
    private void CreateBizCard()
    {
        bizTemplates = Single.MasterData.dataBusinessCardTemplate.GetList();
        bizTemplateCount = bizTemplates.Count;
        for (int i = 0; i < bizTemplateCount; i++)
        {
            BizCardBase bizCard = Single.Resources.Instantiate<BizCardBase>(Cons.Path_Prefab_UI_View + Util.GetBizPrefabName(bizTemplates[i].id, BIZTEMPLATE_TYPE.MINI));
            if (bizCard != null)
            {
                bizCard.SetData(CreateCardInfo());
                Util.SetParentPosition(bizCard.gameObject, go_CardRig);
                bizCard.gameObject.SetActive(false);

                bizCards.Add(bizCard);
            }
        }
    }

    /// <summary>
    /// 명함 정보 업데이트 - 아바타 변경 및 언어 설정 변경 대응
    /// </summary>
    private void UpdateCardInfo()
    {
        int count = bizCards.Count;
        for (int i = 0; i < count; i++)
        {
            bizCards[i].SetData(CreateCardInfo());
        }
    }

    /// <summary>
    /// 예시 문구 로컬라이징 및 아바타 정보 생성
    /// </summary>
    /// <returns></returns>
    private BizCardData CreateCardInfo()
    {
        return new BizCardData(
            new BizCardInfo
            {
                name = Util.GetMasterLocalizing("common_example_name"),
                phone = Util.GetMasterLocalizing("common_example_phone"),
                email = Util.GetMasterLocalizing("common_example_mail"),
                addr = Util.GetMasterLocalizing("common_example_addr"),
                fax = Util.GetMasterLocalizing("common_example_fax"),
                job = Util.GetMasterLocalizing("common_example_job"),
                position = Util.GetMasterLocalizing("common_example_position"),
                intro = Util.GetMasterLocalizing("common_example_cardintro")
            },
            LocalPlayerData.MemberCode,
            LocalPlayerData.AvatarInfo);
    }
    #endregion

    #region 
    /// <summary>
    /// 명함 프리팹 비/활성화
    /// </summary>
    /// <param name="index"></param>
    private void SetActiveTemplate(int index)
    {
        curIndex = CurIndex(index);

        for (int i = 0; i < bizTemplateCount; i++)
        {
            bool b = i == curIndex;
            bizCards[i].gameObject.SetActive(b);
        }
    }

    /// <summary>
    /// 인덱스 계산
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int CurIndex(int index)
    {
        if (index >= bizTemplateCount)
        {
            return 0;
        }
        else if (index < 0)
        {
            return bizTemplateCount - 1;
        }

        return index;
    }

    /// <summary>
    /// 선택 및 구매 버튼 비/활성화
    /// </summary>
    private void SetActiveSelectOrPurchase()
    {
        bool b = (panel_BizCard.GetBizCount() + 1) <= panel_BizCard.availableBizCount;
        btn_Select.gameObject.SetActive(b);
        btn_Purchase.gameObject.SetActive(!b);

    }

    /// <summary>
    /// 선택하기
    /// </summary>
    private void OnClick_Select()
    {
        SceneLogic.instance.PopPopup();

        if (panel_BizCard != null)
        {
            panel_BizCard.AddBizCardView(bizTemplates[curIndex].id);
        }
    }
    #endregion
}
