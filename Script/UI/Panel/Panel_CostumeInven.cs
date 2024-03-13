using db;
using FrameWork.UI;
using Lean.Common;
using MEC;
using Newtonsoft.Json;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_CostumeInven : PanelBase
{
    #region 변수
    [Header("기본 아이템 사이즈")]
    [SerializeField] private Vector2 OriginItemSize = new Vector2(140, 140);
    [Header("NFT 탭 아이템 사이즈")]
    [SerializeField] private Vector2 NFTItemSize = new Vector2(270, 270);

    public Action backAction; // 스토어에서 사용 중

    private TMP_Text txtmp_Bubble;
    private TMP_Text txtmp_CategoryTitle;
    private TMP_Text txtmp_AvatarPartsDescription;
    private Button btn_Save;
    private GameObject go_TextRig;

    private AvatarPartsController selector;
    private GridView_Custom gridView;
    private LeanDragPanel dragPanel;
    private Animator anim_bubble; // 말풍선 애니메이터
    private Animator anim_Avatar; // 아바타 애니메이터

    private string fadeIn = "FadeIn"; // 말풍선 애니 스테이트 - 열림
    private string fadeOut = "FadeOut"; // 말풍선 애니 스테이트 - 닫힘
    private AVATAR_PARTS_TYPE curPartsType;

    private TogglePlus[] toggleArr;
    private List<AVATAR_PARTS_TYPE> avatarPartsTypeList = Util.Enum2List<AVATAR_PARTS_TYPE>(); // 아바타 파츠 타입 Enum => List로 변환한 것
    private Dictionary<string, int> avatarDataIndexDic = new Dictionary<string, int>(); // 선택한 아바타 정보
    private Dictionary<string, int> preAvatarIndexDic = new Dictionary<string, int>(); // 이전 데이터 비교용 딕셔너리
    private List<Item> curItems = new List<Item>(); // 선택된 카테고리의 아이템 리스트 (소팅됨)
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Bubble = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Bubble));
        txtmp_CategoryTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CategoryTitle));
        txtmp_AvatarPartsDescription = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AvatarPartsDescription));

        GetUI_TxtmpMasterLocalizing("txtmp_Save", new MasterLocalData("4019"));
        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("4016"));
        GetUI_TxtmpMasterLocalizing("txtmp_NoItem", new MasterLocalData("item_category_nft_ktmfspecial_empty"));
        #endregion

        #region Button
        btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);

        GetUI_Button("btn_Back", Back);
        GetUI_Button("btn_Reset", () => { SetSaveButtonUI(true); OnClick_ItemReset(); });
        GetUI_Button("btn_Random", () => { SetSaveButtonUI(true); OnClick_ItemRandomSelect(); });
        #endregion

        #region ToggleGroup
        ToggleGroup togg_Category = GetUI<ToggleGroup>(nameof(togg_Category));
        if (togg_Category != null)
        {
            toggleArr = togg_Category.GetComponentsInChildren<TogglePlus>();
            int togCount = toggleArr.Length;
            for (int i = 0; i < togCount; i++)
            {
                var num = (int)avatarPartsTypeList[i];
                toggleArr[i].SetToggleOnAction(() => OnValueChanged_Category(num));
            }
        }
        #endregion

        #region etc
        GameObject go_AvatarView = GameObject.Find(nameof(go_AvatarView));
        if (go_AvatarView)
        {
            selector = go_AvatarView.GetComponent<AvatarPartsController>();
            anim_Avatar = Util.Search<Animator>(go_AvatarView, "AvatarParts");
            if (anim_Avatar != null)
            {
                selector.SetTarget(anim_Avatar.transform);
            }
        }

        go_TextRig = GetChildGObject(nameof(go_TextRig));
        dragPanel = GetChildGObject("go_RawImage").GetComponent<LeanDragPanel>();
        if (dragPanel != null)
        {
            dragPanel.rotate = GameObject.Find("go_RTView").GetComponentInChildren<LeanManualRotate>();
        }
        anim_bubble = GetChildGObject("go_Bubble").GetComponent<Animator>();
        gridView = GetChildGObject("go_ScrollView").GetComponent<GridView_Custom>();
        if (gridView != null)
        {
            gridView.OnCellClicked(index => { SelectCell(index); SetSaveButtonUI(true); });
        }

        BackAction_Custom = OnClick_Back;

        preAvatarIndexDic = Single.ItemData.GetAvatarResetInfo().ToDictionary(x => x.Key, x => x.Value);

        #endregion
    }

    #region 초기화
    public override void Back(int cnt = 1)
    {
        OnClick_Back();
    }

    protected override IEnumerator<float> Co_SetCloseEndAct()
    {
        anim_Avatar.Rebind();
        dragPanel.ResetRotate();

        // 이전 패널이 아즈폰일 때 아바타 투명화
        if (ArzMetaManager.Instance.PhoneController.isPhone)
        {
            MyPlayer.instance.SetMyPlayerVisible(false);
        }

        return base.Co_SetCloseEndAct();
    }

    protected override void OnEnable()
    {
        Init();
        AddHandler();
    }

    protected override void OnDisable()
    {
        selector.SetAvatarParts(LocalPlayerData.AvatarInfo);
        RemoveHandler();
    }

    private void AddHandler()
    {
        Single.Socket.item_S_ChangeInven_Handler += InitTab;
        Single.Socket.item_S_ChangeAvatarInfo_Handler += InitTabAndResetCostume;
    }

    private void RemoveHandler()
    {
        Single.Socket.item_S_ChangeInven_Handler -= InitTab;
        Single.Socket.item_S_ChangeAvatarInfo_Handler -= InitTabAndResetCostume;
    }

    /// <summary>
    /// 아바타 파츠 인벤 갱신 시 
    /// NFT 탭일때 갱신
    /// </summary>
    public void InitTab()
    {
        if (IsNFTTab()) OnValueChanged_Category((int)curPartsType);
    }

    /// <summary>
    /// 아바타 인포 갱신 시
    /// 패널 내부 데이터 갱신 및 탭 갱신
    /// </summary>
    public void InitTabAndResetCostume()
    {
        LoadAvatarInfo();
        OnValueChanged_Category((int)curPartsType);
    }

    private void Init()
    {
        SetActiveTextRig(false);

        #region 카테고리 관련
        SetOpenStartCallback(() => toggleArr[0].SetToggleIsOn(true));
        #endregion

        #region 아바타 데이터 로드
        LoadAvatarInfo();
        if (selector != null)
        {
            SetAvatarPrefabs();
        }
        #endregion

        #region 말풍선 관련
        if (anim_bubble != null)
        {
            anim_bubble.Rebind();
        }

        if (txtmp_Bubble != null)
        {
            Util.SetMasterLocalizing(txtmp_Bubble, string.Empty);
        }
        #endregion

        #region 아바타 RawImage 관련
        if (dragPanel != null)
        {
            dragPanel.ResetRotate();
        }
        #endregion

        #region 저장하기 버튼 초기화
        SetSaveButtonUI(false);
        #endregion
    }

    private void LoadAvatarInfo() => avatarDataIndexDic = LocalPlayerData.AvatarInfo.ToDictionary(x => x.Key, x => x.Value);

    /// <summary>
    /// 아바타 프리팹 세팅
    /// </summary>
    private void SetAvatarPrefabs() => selector.SetAvatarParts(avatarDataIndexDic);

    #endregion

    #region 탭 변경 및 버튼아이템 생성
    /// <summary>
    /// 탭 변경 시 해당 카테고리 아이템으로 변경
    /// </summary>
    /// <param name="idx"></param>
    private void OnValueChanged_Category(int idx)
    {
        curPartsType = (AVATAR_PARTS_TYPE)idx;

        ChangeCategoryTitle(); // 카테고리 제목 변경
        ChangeAvatarPartDec(); // 아바타 파츠 이름 변경

        gridView.ChangeValueSelection(-1); // 해당 탭에서 선택 아이템 초기화
        gridView.JumpToWithOutUpdate(0); // 해당 탭에서 업데이트 없이 맨 위로

        curItems = SortItemList(curPartsType);
        GenerateCells(curItems);
    }

    /// <summary>
    /// 카테고리 제목 변경
    /// </summary>
    private void ChangeCategoryTitle()
    {
        string title = string.Empty;
        switch (curPartsType)
        {
            case AVATAR_PARTS_TYPE.hair: title = "4002"; break;
            case AVATAR_PARTS_TYPE.top: title = "4005"; break;
            case AVATAR_PARTS_TYPE.bottom: title = "4006"; break;
            case AVATAR_PARTS_TYPE.onepiece: title = "4004"; break;
            case AVATAR_PARTS_TYPE.shoes: title = "4007"; break;
            case AVATAR_PARTS_TYPE.accessory: title = "4003"; break;
            case AVATAR_PARTS_TYPE.nft_special: title = "item_category_nft_ktmfspecial"; break;
            default: break;
        }

        Util.SetMasterLocalizing(txtmp_CategoryTitle, new MasterLocalData(title));
    }

    /// <summary>
    /// 아바타파츠 이름 변경
    /// </summary>
    private void ChangeAvatarPartDec()
    {
        string str = Util.EnumInt2String(curPartsType);
        Item item = avatarDataIndexDic.ContainsKey(str) && avatarDataIndexDic[str] != -1 ? GetItemData(curPartsType, avatarDataIndexDic[str]) : null;

        if (item != null)
        {
            Util.SetMasterLocalizing(txtmp_AvatarPartsDescription, new MasterLocalData(item.name));
        }
        else
        {
            Util.SetMasterLocalizing(txtmp_AvatarPartsDescription, string.Empty);
        }
    }

    /// <summary>
    /// 스크롤뷰 데이터 생성
    /// </summary>
    /// <param name="items"></param>
    private void GenerateCells(List<Item> items)
    {
        gridView.SetAxisCellCount(IsNFTTab() ? 3 : 5);

        var cellSize = IsNFTTab() ? NFTItemSize : OriginItemSize;
        gridView.SetCellSize(cellSize);

        var avatarItems = new List<Item_CostumeData>();

        int itemsNum = items.Count;
        for (int i = 0; i < itemsNum; i++)
        {
            Item_CostumeData item = new Item_CostumeData
            {
                id = items[i].id,
                thumbnail = items[i].thumbnail,
                purchaseType = items[i].purchaseType,
                isReset = IsReset(items[i].id),
                isSpecial = IsNFTTab(),
                action = SetSelectData,
                cellSize = cellSize
            };
            avatarItems.Add(item);
        }
        gridView.UpdateContents(avatarItems.ConvertAll(x => x as Item_Data));

        SetActiveTextRig(itemsNum <= 0);
        UpdateCurSelectItem(); // 현재 선택된 인덱스 표시
    }

    /// <summary>
    /// 현재 선택된 인덱스 표시
    /// </summary>
    private void UpdateCurSelectItem()
    {
        if (avatarDataIndexDic.ContainsKey(Util.EnumInt2String(curPartsType)))
        {
            int idx = AvatarIdToIndex(avatarDataIndexDic[Util.EnumInt2String(curPartsType)]);
            gridView.ChangeValuePreSelection(idx); // 선택된 인덱스 갱신
            SelectCell(idx); // 선택하기
        }
        else
        {
            gridView.ChangeValuePreSelection(-1);
        }
    }

    /// <summary>
    /// 아이템 없을 시 비/활성화
    /// </summary>
    /// <param name="b"></param>
    private void SetActiveTextRig(bool b)
    {
        if (go_TextRig != null)
        {
            go_TextRig.SetActive(b);
        }
    }
    #endregion

    #region 아이템 선택 처리
    /// <summary>
    /// 아이템 선택
    /// </summary>
    /// <param name="index"></param>
    protected void SelectCell(int index)
    {
        if (gridView.DataCount == 0) return;

        gridView.UpdateSelection(index);
    }

    /// <summary>
    /// 선택 시 데이터 처리
    /// </summary>
    /// <param name="id"></param>
    private void SetSelectData(int id)
    {
        SaveSelectItemData(id); // 아이템 선택 저장
        ChangeAvatarPartDec(); // 아이템 이름 변경

        if (id <= 0)
        {
            selector.ChangeResetCloth(curPartsType);
            return;
        }

        Item item = GetItemData(curPartsType, id);
        if (item != null)
        {
            selector.SetAvatarParts(avatarDataIndexDic, log: false);
            if (!IsNFTTab())
            {
                selector.WearSelectParts(curPartsType, item.prefab, Single.ItemData.GetMaterials(item.id)); // 아이템 착용
            }

            ItemUseEffect itemUseEffect = GetItemUseEffect(item.id);
            if (itemUseEffect != null)
            {
                PlayAvatarAnimation(itemUseEffect.animationName); // 애니메이션 실행
                ShowSpeechBubble(itemUseEffect.chat); // 말풍선 실행
            }
        }
    }
    #endregion

    #region 아바타 선택 저장
    /// <summary>
    /// 선택한 아바타 파츠 인덱스 저장
    /// </summary>
    private void SaveSelectItemData(int id)
    {
        if (IsNFTTab() && !IsWearNFTCostume())
        {
            preAvatarIndexDic = avatarDataIndexDic.ToDictionary(x => x.Key, x => x.Value);
            SetSelectItem(true, AVATAR_PARTS_TYPE.hair,
                                AVATAR_PARTS_TYPE.top,
                                AVATAR_PARTS_TYPE.bottom,
                                AVATAR_PARTS_TYPE.onepiece,
                                AVATAR_PARTS_TYPE.shoes,
                                AVATAR_PARTS_TYPE.accessory);
        }
        else if (!IsNFTTab() && IsWearNFTCostume())
        {
            SetSelectItem(true, AVATAR_PARTS_TYPE.nft_special);
            avatarDataIndexDic = preAvatarIndexDic.ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            switch (curPartsType)
            {
                case AVATAR_PARTS_TYPE.top:
                case AVATAR_PARTS_TYPE.bottom:
                case AVATAR_PARTS_TYPE.shoes:
                    SetSelectItem(true, AVATAR_PARTS_TYPE.onepiece);
                    SetSelectItem(false, AVATAR_PARTS_TYPE.top,
                                         AVATAR_PARTS_TYPE.bottom,
                                         AVATAR_PARTS_TYPE.shoes);
                    break;
                case AVATAR_PARTS_TYPE.onepiece:
                    SetSelectItem(true, AVATAR_PARTS_TYPE.top,
                                        AVATAR_PARTS_TYPE.bottom,
                                        AVATAR_PARTS_TYPE.shoes);
                    break;
            }
        }

        avatarDataIndexDic[Util.EnumInt2String(curPartsType)] = id;
    }

    /// <summary>
    /// 선택한 코스튬에 따른 이전 옷 인덱스 덮어쓰기
    /// 딕셔너리에 키값이 없으면 만들고, 있으면 값 넣기
    /// </summary>
    private void SetSelectItem(bool isReset, params AVATAR_PARTS_TYPE[] args)
    {
        foreach (var type in args)
        {
            string TypeStr = Util.EnumInt2String(type);

            if (!avatarDataIndexDic.ContainsKey(TypeStr))
            {
                avatarDataIndexDic.Add(TypeStr, -1);
            }

            if (isReset)
            {
                avatarDataIndexDic[TypeStr] = -1;
            }
            else
            {
                var curPartMatName = new List<string>();
                switch (type)
                {
                    case AVATAR_PARTS_TYPE.hair: curPartMatName = selector.GetSkinPart(BasePartsSelector.PARTS_TYPE.hair).materialName; break;
                    case AVATAR_PARTS_TYPE.top: curPartMatName = selector.GetSkinPart(BasePartsSelector.PARTS_TYPE.top).materialName; break;
                    case AVATAR_PARTS_TYPE.bottom: curPartMatName = selector.GetSkinPart(BasePartsSelector.PARTS_TYPE.bottom).materialName; break;
                    case AVATAR_PARTS_TYPE.shoes: curPartMatName = selector.GetSkinPart(BasePartsSelector.PARTS_TYPE.shoes).materialName; break;
                    case AVATAR_PARTS_TYPE.accessory: curPartMatName = selector.GetSkinPart(BasePartsSelector.PARTS_TYPE.accessory).materialName; break;
                    default: break;
                }
                SelectCurrentItem(type, curPartMatName);
            }
        }
    }

    /// <summary>
    /// 코스튬 이전에 선택했던 오브젝트로 인덱스 변경
    /// </summary>
    private void SelectCurrentItem(AVATAR_PARTS_TYPE type, List<string> name)
    {
        if (name == null || name.Count <= 0) return;

        List<Item> items = GetItemDataList(type);
        Item item = items.FirstOrDefault(x => Single.ItemData.GetMaterials(x.id)[0] == name[0]);

        int curIndex = item != null ? item.id : -1;
        avatarDataIndexDic[Util.EnumInt2String(type)] = curIndex;
    }
    #endregion

    #region 아바타 애니메이션 실행
    /// <summary>
    /// 아바타 애니메이션 실행
    /// </summary>
    /// <param name="aniName"></param>
    private void PlayAvatarAnimation(string aniName)
    {
        if (string.IsNullOrEmpty(aniName))
        {
            Debug.Log("해당 애니메이션이 없습니다!");
            return;
        }

        if (anim_Avatar != null)
        {
            if (!anim_Avatar.IsInTransition(0))
            {
                anim_Avatar.CrossFade(aniName, 0.1f);
            }
        }
    }
    #endregion

    #region 말풍선 띄우기
    /// <summary>
    /// 말풍선 띄우기
    /// </summary>
    /// <param name="idx"></param>
    private void ShowSpeechBubble(string idx)
    {
        if (string.IsNullOrEmpty(idx)) return;

        if (txtmp_Bubble != null)
        {
            Util.SetMasterLocalizing(txtmp_Bubble, new MasterLocalData(idx));
        }

        if (anim_bubble != null)
        {
            anim_bubble.Rebind();
            Util.RunCoroutine(Co_ShowBubble(), "ShowBubble");
        }
    }

    private float aniTimeOffset = 0.5f; // 애니메이션 길이 여분 더하기
    private IEnumerator<float> Co_ShowBubble(float time = 1f)
    {
        anim_bubble.SetTrigger(fadeIn);

        yield return Timing.WaitForSeconds(time + aniTimeOffset);

        anim_bubble.SetTrigger(fadeOut);
    }
    #endregion

    #region 아이템 리셋, 랜덤
    /// <summary>
    /// 아이템 리셋
    /// </summary>
    private void OnClick_ItemReset()
    {
        // 아바타 파츠 변경
        avatarDataIndexDic = Single.ItemData.GetAvatarResetInfo().ToDictionary(x => x.Key, x => x.Value);
        SetAvatarPrefabs();

        // 아이템 선택
        bool b = avatarDataIndexDic.TryGetValue(Util.EnumInt2String(curPartsType), out int value);
        int index = b ? AvatarIdToIndex(value) : -1;
        SelectCell(index);
    }

    private Dictionary<string, int> randomDic;
    /// <summary>
    /// 아이템 랜덤 선택
    /// </summary>
    private void OnClick_ItemRandomSelect()
    {
        // 데이터 비교용 딕셔너리 저장
        randomDic = avatarDataIndexDic.ToDictionary(x => x.Key, x => x.Value);
        avatarDataIndexDic.Clear();

        // 랜덤
        Random();

        // 옷 세팅
        SetAvatarPrefabs();
        OnValueChanged_Category((int)curPartsType);
    }

    private void Random()
    {
        if (LocalPlayerData.AvatarPartsInvens == null
            || LocalPlayerData.AvatarPartsInvens.Length < 1) return;

        bool isNFT = UnityEngine.Random.Range(0, 10) < 1; // NFT는 1할 확률
        if (isNFT)
        {
            RandomSettingIndex(AVATAR_PARTS_TYPE.nft_special);
        }
        else
        {
            bool isOnepiece = UnityEngine.Random.Range(0, 2) == 1;
            if (isOnepiece)
            {
                RandomSettingIndex(AVATAR_PARTS_TYPE.onepiece); // 코스튬
            }
            else
            {
                RandomSettingIndex(AVATAR_PARTS_TYPE.top); // 상의
                RandomSettingIndex(AVATAR_PARTS_TYPE.bottom); // 하의
                RandomSettingIndex(AVATAR_PARTS_TYPE.shoes); // 신발
            }
            RandomSettingIndex(AVATAR_PARTS_TYPE.hair); // 헤어
            RandomSettingIndex(AVATAR_PARTS_TYPE.accessory); // 악세사리
        }
    }

    /// <summary>
    /// 랜덤으로 파츠 변경
    /// </summary>
    /// <param name="type"></param>
    /// <param name="typeStr"></param>
    private void RandomSettingIndex(AVATAR_PARTS_TYPE type)
    {
        List<Item> sortItems = SortItemList(type);

        if (sortItems.Count == 0) return;

        int index = sortItems.Count == 1 ? 0 : UnityEngine.Random.Range(0, sortItems.Count);
        string typeStr = Util.EnumInt2String(type);

        if (randomDic.TryGetValue(typeStr, out int value))
        {
            if (value == sortItems[index].id)
            {
                RandomSettingIndex(type); // 이전과 같은 아이템이면 재귀
                return;
            }
        }

        avatarDataIndexDic[typeStr] = sortItems[index].id;
    }
    #endregion

    #region 저장
    /// <summary>
    /// 저장하기
    /// </summary>
    private void OnClick_Save()
    {
        int count = avatarPartsTypeList.Count;
        for (int i = 0; i < count; i++)
        {
            string numStr = Util.EnumInt2String(avatarPartsTypeList[i]);
            if (avatarDataIndexDic.TryGetValue(numStr, out int value))
            {
                if (value <= 0) avatarDataIndexDic.Remove(numStr);
            }
        }

        Single.Web.member.Avatar(avatarDataIndexDic, (res) =>
        {
            if (res.avatarInfos != null && res.avatarInfos.Count != 0)
            {
                LocalPlayerData.AvatarInfo = res.avatarInfos.ToDictionary(x => x.Key, x => x.Value);
            }
            else
            {
                Single.ItemData.AvatarSettingInit();
            }

            Util.SendAvatarInfoRealTime(LocalPlayerData.AvatarInfo);
            MyPlayer.instance.GetController<AvatarPartsController>(Cons.AvatarPartsController).SetAvatarParts(LocalPlayerData.AvatarInfo);

            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("10301")));
        });
    }

    /// <summary>
    /// 저장하기 버튼 UI 세팅
    /// </summary>
    /// <param name="isActive"></param>
    private void SetSaveButtonUI(bool isActive) => btn_Save.interactable = isActive;

    /// <summary>
    /// 뒤로가기
    /// </summary>
    private void OnClick_Back()
    {
        var originAvatarDataDic = LocalPlayerData.AvatarInfo.OrderBy(x => x.Value);
        var changeAvatarDataDic = avatarDataIndexDic.OrderBy(x => x.Value);

        string originAvatarData = JsonConvert.SerializeObject(originAvatarDataDic);
        string changeAvatarData = JsonConvert.SerializeObject(changeAvatarDataDic);

        if (originAvatarData.Equals(changeAvatarData))
        {
            SceneLogic.instance.PopPanel();
        }
        else
        {
            PushPopup<Popup_Basic>()
             .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("8001")))
             .ChainPopupAction(new PopupAction(() => { SceneLogic.instance.isUILock = false; SceneLogic.instance.PopPanel(); }));
        }
    }
    #endregion

    #region 기능 메소드
    /// <summary>
    /// 아이템 데이터 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private Item GetItemData(AVATAR_PARTS_TYPE type, int id)
    {
        return Single.ItemData.GetItemData(type).GetData(id);
    }

    /// <summary>
    /// 아이템 데이터 리스트 가져오기
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private List<Item> GetItemDataList(AVATAR_PARTS_TYPE type)
    {
        return Single.ItemData.GetItemData(type).GetList();
    }

    /// <summary>
    /// 아이템 사용 효과 데이터 가져오기
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private ItemUseEffect GetItemUseEffect(int id)
    {
        return Single.MasterData.dataItemUseEffect.GetData(id);
    }

    /// <summary>
    /// 나의 인벤토리에 있는 아이템만 포함시키기
    /// </summary>
    /// <returns></returns>
    private List<Item> SortItemList(AVATAR_PARTS_TYPE type)
    {
        List<Item> sortList = new List<Item>();
        List<Item> oriList = GetItemDataList(type);
        int count = oriList.Count;
        for (int i = 0; i < count; i++)
        {
            if (IsMyInvenParts(oriList[i].id))
            {
                sortList.Add(oriList[i]);
            }
        }
        return sortList;
    }

    /// <summary>
    /// 내 인벤토리에 있는 아이템인지 여부
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool IsMyInvenParts(int id)
    {
        AvatarPartsInvens invens = LocalPlayerData.AvatarPartsInvens.FirstOrDefault(x => x.itemId == id);
        return invens != null;
    }

    /// <summary>
    /// 한 번 더 눌러서 착용 해제 가능 여부
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool IsReset(int id)
    {
        ItemUseEffect itemUseEffect = GetItemUseEffect(id);
        if (itemUseEffect != null)
        {
            return selector.IsResetCloth((AVATAR_PARTS_TYPE)itemUseEffect.partsType);
        }
        return false;
    }

    /// <summary>
    /// 아바타 id를 List의 Index값으로 변환
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private int AvatarIdToIndex(int id)
    {
        int count = curItems.Count;
        for (int i = 0; i < count; i++)
        {
            if (curItems[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 현재 탭이 NFT Special인가
    /// </summary>
    /// <returns></returns>
    private bool IsNFTTab() => curPartsType == AVATAR_PARTS_TYPE.nft_special;

    private bool IsWearNFTCostume() => Single.ItemData.IsWearNFTCostume(avatarDataIndexDic);
    #endregion
}