using FrameWork.UI;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class Panel_Kiosk : PanelBase
{
    #region 변수
    private enum SortBuket
    {
        Top,
        Bottom,
        Hair,
    }

    private StoreManager store;
    private AvatarPartsController controller;

    private List<StoreAvatarParts> allItemList = new List<StoreAvatarParts>(); // 전체 시착 아이템
    private List<StoreAvatarParts> selectItemList = new List<StoreAvatarParts>(); // 선택 시착 아이템

    private GameObject go_RT;
    private GameObject mannequin;
    private Transform parentTr;
    private RawImage go_RawImage;

    private LoopScrollView_Custom scrollView;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_Purchase", new MasterLocalData("30005"));
        GetUI_TxtmpMasterLocalizing("txtmp_PurchaseAll", new MasterLocalData("30006"));
        #endregion

        #region Button
        GetUI_Button("btn_Back", () => SceneLogic.instance.Back());
        GetUI_Button("btn_Purchase", OnClick_Purchase);
        GetUI_Button("btn_PurchaseAll", OnClick_PurchaseAll);
        #endregion

        #region etc
        go_RawImage = GetChildGObject(nameof(go_RawImage)).GetComponent<RawImage>();
        scrollView = GetChildGObject("go_ScrollView").GetComponent<LoopScrollView_Custom>();
        if (scrollView != null)
        {
            float scrollViewWidth = GetChildGObject("go_Item").GetComponent<RectTransform>().rect.width;
            float itemWidth = scrollView.cellPrefab.transform.Find("Image").GetComponent<RectTransform>().rect.width;
            float interval = Mathf.Clamp((itemWidth / scrollViewWidth) / 2f, 0.16f, 0.2f);
            scrollView.SetCellIntercal(interval);
        }
        go_RT = GetChildGObject(nameof(go_RT));
        if (go_RT != null)
        {
            controller = go_RT.GetComponentInChildren<AvatarPartsController>();
            parentTr = controller.transform;
        }
        #endregion

        store = StoreManager.Instance;
    }

    #region 초기화
    protected override void Start()
    {
        SetRenderTexture();
    }

    private void SetRenderTexture()
    {
        if (go_RT == null) return;

        Camera renderCam = go_RT.GetComponentInChildren<Camera>();
        renderCam.enabled = false;

        RenderTexture rt = new RenderTexture(700, 700, 24);
        renderCam.targetTexture = rt;

        if (go_RawImage != null)
        {
            go_RawImage.texture = rt;
        }

        renderCam.enabled = true;
    }

    protected override void OnEnable()
    {
        if (allItemList.Count != 0) allItemList.Clear();
        if (selectItemList.Count != 0) selectItemList.Clear();

        Init().RunCoroutine();
    }

    protected override void OnDisable()
    {
        if (mannequin != null) Destroy(mannequin);
    }

    private IEnumerator<float> Init()
    {
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance);
        yield return Timing.WaitUntilTrue(() => store.PurchaseController);

        // 데이터 세팅
        // 모자 - 상의 - 하의 순으로 정렬
        int count = store.PurchaseController.basket.Count;
        for (int i = 1; i < 4; i++)
        {
            for (int j = 0; j < count; j++)
            {
                if (store.PurchaseController.basket[j].type.ToString() == ((AVATAR_PARTS_TYPE)i).ToString())
                    allItemList.Add(store.PurchaseController.basket[j].DeepCopy());
            }
        }

        // 아이템 로드
        LoadData();

        // 마네킹 만들기
        CreateMannequin();
    }

    /// <summary>
    /// 아이템 로드
    /// </summary>
    private void LoadData()
    {
        // 아이템 로드
        List<Item_CommerceData> commercesItems = new List<Item_CommerceData>();
        int count = AllItemListCount();
        for (int i = 0; i < count; i++)
        {
            Item_CommerceData item = new Item_CommerceData
            {
                id = i,
                costum = allItemList[i]
            };

            commercesItems.Add(item);
        }

        scrollView.UpdateData(commercesItems.ConvertAll(x => x as Item_Data));
        scrollView.JumpTo(SortSelectCell());
    }
    #endregion

    #region Kiosk
    /// <summary>
    /// 상의 - 하의 - 모자 순 선택
    /// </summary>
    /// <returns></returns>
    private int SortSelectCell()
    {
        int count = AllItemListCount();
        if (count < 1) return 0;

        int length = Util.EnumLength<SortBuket>();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < count; j++)
            {
                if (allItemList[j].type.ToString() == ((SortBuket)i).ToString())
                    return j;
            }
        }
        return 0;
    }

    /// <summary>
    /// 아바타 복제하기
    /// </summary>
    private void CreateMannequin()
    {
        mannequin = LoadMannequin();

        Transform anim_Avatar = mannequin.GetComponentInChildren<Animator>().transform;
        controller.SetTarget(anim_Avatar);

        Util.ChangeLayerMask(mannequin, Define.Player);
    }

    /// <summary>
    /// 마네킹 복제
    /// </summary>
    /// <returns></returns>
    private GameObject LoadMannequin()
    {
        GameObject avatar = MyPlayer.instance.Childrens[Cons.PLAYER_PARENT].GetComponentInChildren<Animator>().gameObject;
        GameObject dummy = Instantiate(avatar, parentTr.position, parentTr.rotation, parentTr);

        Destroy(dummy.GetComponent<PlayerSound>());

        return dummy;
    }

    /// <summary>
    /// 선택 구매하기
    /// </summary>
    private void OnClick_Purchase()
    {
        selectItemList.Clear();

        int count = AllItemListCount();
        for (int i = 0; i < count; i++)
        {
            if (allItemList[i].purchase)
                selectItemList.Add(allItemList[i]);
        }

        if (selectItemList.Count < 1)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("30034")));
            return;
        }

        PushPopup<Popup_CommercePurchase>().SetData(selectItemList);
    }

    /// <summary>
    /// 전체 구매하기
    /// </summary>
    private void OnClick_PurchaseAll()
    {
        PushPopup<Popup_CommercePurchase>().SetData(allItemList);
    }

    /// <summary>
    /// 전체 리스트 아이템 개수 리턴
    /// </summary>
    /// <returns></returns>
    private int AllItemListCount() => allItemList.Count;

    /// <summary>
    /// 마네킹 변경
    /// </summary>
    public void ChangeAvatarSetting(StoreAvatarParts costum)
    {
        var type = Util.String2Enum<BasePartsSelector.PARTS_TYPE>(costum.type.ToString());
        controller.ChangeParts(type, costum.TargetColor(), new List<string>() { "temp" });
    }
    #endregion
}
