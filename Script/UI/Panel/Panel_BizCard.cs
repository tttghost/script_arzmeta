using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Unity.Linq;
using MEC;

public class Panel_BizCard : PanelBase
{
    #region 변수
    private List<BizCardInfo> oriBizCards = new List<BizCardInfo>(); // 원래 명함 리스트 (데이터)
    private List<TogPlus_BizCard> togs = new List<TogPlus_BizCard>(); // 토글 관리 리스트 (오브젝트)
    private List<BizCardBase> bizCards = new List<BizCardBase>(); // 명함 관리 리스트 (오브젝트)

    private Transform go_CardRig;
    private Transform go_ScrollView;
    private ToggleGroup toggleGroup;

    private GameObject addView;

    public int availableBizCount { get; set; } // 사용자가 현재 생성할 수 있는 명함 갯수 (기본 명함 수 + 결재한 명함 수)
    private int maxBizCount; // 최대 명함 갯수 제한

    public Action BackAction;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Save", new MasterLocalData("common_save"));
        #endregion

        #region Button
        GetUI_Button("btn_Back", OnClick_Back);
        GetUI_Button("btn_Save", OnClick_Save);
        #endregion

        #region etc
        go_CardRig = GetChildGObject(nameof(go_CardRig)).transform;
        GameObject scrollView = GetChildGObject(nameof(go_ScrollView));
        if (scrollView != null)
        {
            go_ScrollView = scrollView.GetComponent<ScrollRect>().content;
            toggleGroup = scrollView.GetComponent<ToggleGroup>();
        }
        #endregion

        BackAction_Custom = () => { return; }; // 물리 백버튼 눌렀을 시 막음
    }

    #region 초기화
    protected override void Awake()
    {
        base.Awake();

        GetCount();
    }

    protected override void OnEnable()
    {
        InitBizCard();

        SetToggleIsOn(0, true);
    }

    protected override void OnDisable()
    {
        ClearData();
    }

    /// <summary>
    /// 생성 가능한 명함 수 및 최대 명함 수 가져오기
    /// </summary>
    private void GetCount()
    {
        // 차후 결제한 명함 수 계산
        availableBizCount = Single.MasterData.dataFunctionTable.GetData((int)FUNCTION_TYPE.BIZCARD_DEFAULT).value/* + 내가 결제한 명함 수*/;
        //maxBizCount = Single.MasterData.dataFunctionTable.GetData((int)FUNCTION_TYPE.BIZCARD_MAX).value;
        maxBizCount = 1;
    }

    /// <summary>
    /// 최초 명함 세팅
    /// </summary>
    private void InitBizCard()
    {
        oriBizCards = Util.DeepCopy(LocalPlayerData.BusinessCardInfos.ToList());

        if (oriBizCards != null && oriBizCards.Count > 0)
        {
            foreach (var info in oriBizCards)
            {
                CreateBizCardView(info);
            }
        }

        CreateAddView();
    }

    /// <summary>
    /// 오브젝트 및 데이터 삭제
    /// </summary>
    private void ClearData()
    {
        togs.Clear();
        bizCards.Clear();

        go_CardRig.gameObject.Children().Destroy();
        go_ScrollView.gameObject.Children().Destroy();
    }
    #endregion

    #region private
    /// <summary>
    /// 명함 생성
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isNew"></param>
    private void CreateBizCardView(BizCardInfo info)
    {
        BizCardBase bizCard = Single.Resources.Instantiate<BizCardBase>(Cons.Path_Prefab_UI_View + Util.GetBizPrefabName(info.templateId, BIZTEMPLATE_TYPE.EDIT));
        if (bizCard != null)
        {
            bizCards.Add(bizCard);

            bizCard.SetData(new BizCardData(info, LocalPlayerData.MemberCode, LocalPlayerData.AvatarInfo, GetBizCount()));
            Util.SetParentPosition(bizCard.gameObject, go_CardRig);

            bizCard.gameObject.SetActive(false);

            // 토글 만들기
            CreateToggle().SetData(GetBizCount(), LocalPlayerData.Method.IsBizDefault(info.num), (b) => { bizCard.gameObject.SetActive(b); });
        }
    }

    /// <summary>
    /// 추가 뷰 생성
    /// </summary>
    private void CreateAddView()
    {
        if (GetBizCount() >= maxBizCount) return;

        View_BizCard_Add addView = Single.Resources.Instantiate<View_BizCard_Add>(Cons.Path_Prefab_UI_View + Cons.View_BizCard_Add);
        if (addView != null)
        {
            this.addView = addView.gameObject;

            addView.AddListener(() =>
            {
                // 임시 콘텐츠 준비 중 - 차후 삭제
                if (GetBizCount() >= availableBizCount)
                {
                    PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("3055")));
                    return;
                }

                PushPopup<Popup_BizCardSelect>();
            });
            Util.SetParentPosition(addView.gameObject, go_CardRig);

            addView.gameObject.SetActive(false);

            // 토글 만들기
            CreateToggle().SetData(-1, false, (b) => { addView.gameObject.SetActive(b); });
        }
    }

    /// <summary>
    /// 토글 생성
    /// </summary>
    /// <returns></returns>
    private TogPlus_BizCard CreateToggle()
    {
        TogPlus_BizCard toggle = Single.Resources.Instantiate<TogPlus_BizCard>(Cons.Path_Prefab_UI + Cons.togplus_BizCard);
        if (toggle != null)
        {
            togs.Add(toggle);

            toggle.Tog.group = toggleGroup;
            Util.SetParentPosition(toggle.gameObject, go_ScrollView);
            SetToggleIsOn(LastToggleIndex(), false);

            toggle.gameObject.SetActive(true);
        }

        return toggle;
    }


    /// <summary>
    /// 명함 데이터 및 오브젝트 삭제
    /// </summary>
    /// <param name="index"></param>
    private void DeleteBizCard(int index)
    {
        bizCards[index].gameObject.Destroy();
        bizCards.RemoveAt(index);
    }

    /// <summary>
    /// 토글 데이터 및 오브젝트 삭제
    /// </summary>
    /// <param name="index"></param>
    private void DeleteToggle(int index)
    {
        togs[index].gameObject.Destroy();
        togs.RemoveAt(index);
    }

    /// <summary>
    /// 토글 인덱스 재정렬
    /// </summary>
    private void SortToggle()
    {
        int count = togs.Count;
        for (int i = 0; i < count; i++)
        {
            if (LastToggleIndex() == i) continue;

            togs[i].SetIndex(i + 1);
        }
    }

    /// <summary>
    ///  뒤로가기
    /// </summary>
    private void OnClick_Back()
    {
        if (!Util.EqualMyRoomList(LocalPlayerData.BusinessCardInfos, GetBizCardInfos()))
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("businesscard_confirm_exit_nosave")))
                .ChainPopupAction(new PopupAction(() => InvokeBackAction()));
            return;
        }

        InvokeBackAction();
    }

    private void InvokeBackAction()
    {
        BackAction?.Invoke();
        BackAction = null;

        SceneLogic.instance.isUILock = false;
        SceneLogic.instance.PopPanel();
    }

    #region 저장하기 
    /// <summary>
    /// 저장하기
    /// </summary>
    private void OnClick_Save() => Util.RunCoroutine(Co_Save(), "Save");

    private bool isDoneSaveBizCard = false;
    private IEnumerator<float> Co_Save()
    {
        // 변경사항 있을 시 저장
        if (!Util.EqualMyRoomList(LocalPlayerData.BusinessCardInfos, GetBizCardInfos()))
        {
            Util.RunCoroutine(Co_SaveBizCard(), "SaveBizCard");
            yield return Timing.WaitUntilTrue(() => isDoneSaveBizCard);
        }

        yield return Timing.WaitUntilDone(Co_SaveDefaultCard());

        OpenToast<Toast_Basic>()
        .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData("10301")));
    }

    /// <summary>
    /// 명함 저장
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_SaveBizCard()
    {
        isDoneSaveBizCard = false;

        int count = GetBizCount();

        if (count > 0)
        {
            if (!CheckData()) yield break;
        }

        Single.Web.member.UpdateMyCard(GetBizCardInfos(), (res) =>
        {
            LocalPlayerData.BusinessCardInfos = res.businessCardInfos;

            if (res.businessCardInfos == null || res.businessCardInfos.Length < 1)
            {
                isDoneSaveBizCard = true;
                return;
            }

            for (int i = 0; i < count; i++)
            {
                bizCards[i].UpdateBizData(res.businessCardInfos[i]);
            }

            isDoneSaveBizCard = true;
        });

        yield return Timing.WaitForOneFrame;
    }

    private bool CheckData()
    {
        foreach (var item in bizCards)
        {
            if (!item.CheckData())
            {
                PushPopup<Popup_Basic>()
                    .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("businesscard_error_form")));
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 대표 명함 저장
    /// </summary>
    /// <returns></returns>
    IEnumerator<float> Co_SaveDefaultCard()
    {
        int defaultCardIndex = CheckDefaultCardIndex();

        if (defaultCardIndex < 0)
        {
            DeleteDefaultCardInfo();
        }
        else
        {
            BizCardBase bizCard = bizCards.FirstOrDefault(x => x.GetData().index == defaultCardIndex);
            if (bizCard != null)
            {
                BizCardInfo info = bizCard.GetData().bizCard;
                Single.Web.member.SetDefaultCardInfo(info.templateId, info.num, (res) => { LocalPlayerData.DefaultCardInfo = res.defaultCardInfo; });
            }
            else
            {
                DeleteDefaultCardInfo();
            }
        }

        yield return Timing.WaitForOneFrame;
    }

    private int CheckDefaultCardIndex()
    {
        foreach (var item in bizCards)
        {
            if (item.CheckDefault().isOn)
            {
                return item.CheckDefault().index;
            }
        }
        return -1;
    }

    /// <summary>
    /// 대표 명함 삭제
    /// </summary>
    private void DeleteDefaultCardInfo()
    {
        Single.Web.member.DeleteDefaultCardInfo((res) => { LocalPlayerData.DefaultCardInfo = null; });
    }
    #endregion
    #endregion

    #region public
    /// <summary>
    /// 명함 프리팹 추가
    /// </summary>
    /// <param name="templateId"></param>
    public void AddBizCardView(int templateId)
    {
        if (addView != null)
        {
            addView.Destroy();
            DeleteToggle(LastToggleIndex());
        }

        CreateBizCardView(new BizCardInfo { templateId = templateId });
        SetToggleIsOn(LastToggleIndex(), true);

        CreateAddView();
    }

    /// <summary>
    /// 삭제하기
    /// </summary>
    /// <param name="index"></param>
    public void DeleteBizCardView(int index)
    {
        int count = GetBizCount();
        int deleteIndex = -1;
        for (int i = 0; i < count; i++)
        {
            if (bizCards[i].GetData().index == index)
            {
                deleteIndex = i;
                break;
            }
        }

        DeleteBizCard(deleteIndex);
        DeleteToggle(deleteIndex);

        #region 토글 처리
        if (addView == null)
        {
            CreateAddView();
        }

        SortToggle();

        if (togs.Count > 1)
        {
            SetToggleIsOn(deleteIndex - 1, true);
        }
        #endregion
    }

    /// <summary>
    /// 대표명함 지정 아이콘 및 토글 처리
    /// </summary>
    /// <param name="index"></param>
    public void SetActiveDefault(int index)
    {
        int count = GetBizCount();
        for (int i = 0; i < count; i++)
        {
            bool b = bizCards[i].GetData().index == index;

            togs[i].SetActiveDefault(b);
            bizCards[i].SetActiveDefault(b);
        }
    }
    #endregion

    #region 기능 메소드
    /// <summary>
    /// 마지막 토글 인덱스 반환
    /// </summary>
    /// <returns></returns>
    private int LastToggleIndex() => togs.Count - 1;

    /// <summary>
    /// 명함 데이터 갯수 반환
    /// </summary>
    /// <returns></returns>
    public int GetBizCount() => bizCards.Count;

    /// <summary>
    /// 원하는 토글 켜기 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="b"></param>
    private void SetToggleIsOn(int index, bool b) => togs[index].TogPlus.SetToggleIsOn(b);

    /// <summary>
    /// 전체 명함 데이터 가져오기
    /// </summary>
    /// <returns></returns>
    private BizCardInfo[] GetBizCardInfos()
    {
        int count = GetBizCount();

        if (count < 1) return new BizCardInfo[0];

        List<BizCardInfo> editCardInfos = new List<BizCardInfo>();
        for (int i = 0; i < count; i++)
        {
            editCardInfos.Add(bizCards[i].GetData().bizCard);
        }
        return editCardInfos.ToArray();
    }
    #endregion
}
