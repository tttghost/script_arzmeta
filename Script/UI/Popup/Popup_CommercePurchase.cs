using FrameWork.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using System.Collections.Generic;

public class Popup_CommercePurchase : PopupBase
{
    #region 변수
    [SerializeField] private Item_CommercePurchase purchaseItem;
    private List<Item_CommercePurchase> purchaseList = new List<Item_CommercePurchase>();

    private Transform go_Pos;
    private ScrollRect go_ScrollView;

    private TMP_Text txtmp_Money;
    private TMP_InputField input_Name;
    private TMP_InputField input_Contact;
    private TMP_InputField input_Address;
    private TMP_InputField input_DetailAddress;
    private TMP_InputField input_Requirements;

    private float totalPrice;
    private string itemName;
    private string size;
    private string color;
    private int quantity;
    #endregion

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        #region Button
        GetUI_Button("btn_Confirm", OnConfirm); // 투 버튼 확인
        GetUI_Button("btn_Cancel", OnCancel); // 투 버튼 취소
        #endregion

        #region TMP_Text
        txtmp_Money = GetUI_TxtmpMasterLocalizing("txtmp_Money");

        GetUI_TxtmpMasterLocalizing("txtmp_Confirm", new MasterLocalData("001"));
        GetUI_TxtmpMasterLocalizing("txtmp_Cancel", new MasterLocalData("002"));
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("30007"));
        GetUI_TxtmpMasterLocalizing("txtmp_PurchaseListTitle", new MasterLocalData("30008"));
        GetUI_TxtmpMasterLocalizing("txtmp_AddressTitle", new MasterLocalData("30010"));
        GetUI_TxtmpMasterLocalizing("txtmp_Name", new MasterLocalData("30011"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Name", new MasterLocalData("30011"));
        GetUI_TxtmpMasterLocalizing("txtmp_Contact", new MasterLocalData("30012"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Contact", new MasterLocalData("30013"));
        GetUI_TxtmpMasterLocalizing("txtmp_Address", new MasterLocalData("30016"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Address", new MasterLocalData("30016"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_DetailAddress", new MasterLocalData("30017"));
        GetUI_TxtmpMasterLocalizing("txtmp_Requirements", new MasterLocalData("30018"));
        GetUI_TxtmpMasterLocalizing("txtmp_Placeholder_Requirements", new MasterLocalData("30019"));
        GetUI_TxtmpMasterLocalizing("txtmp_TotalCostTitle", new MasterLocalData("30020"));
        #endregion

        #region InputField
        input_Name = GetUI_TMPInputField("input_Name");
        input_Contact = GetUI_TMPInputField("input_Contact");
        input_Address = GetUI_TMPInputField("input_Address");
        input_DetailAddress = GetUI_TMPInputField("input_DetailAddress");
        input_Requirements = GetUI_TMPInputField("input_Requirements");
        #endregion

        #region etc
        go_Pos = GetChildGObject("go_Pos").transform;
        go_ScrollView = GetChildGObject("go_ScrollView").GetComponent<ScrollRect>();
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        base.OnEnable();

        if (input_Name != null)
        {
            input_Name.Clear();
        }
        if (input_Contact != null)
        {
            input_Contact.Clear();
        }
        if (input_Address != null)
        {
            input_Address.Clear();
        }
        if (input_DetailAddress != null)
        {
            input_DetailAddress.Clear();
        }
        if (input_Requirements != null)
        {
            input_Requirements.Clear();
        }
    }
    #endregion

    #region Purchase
    public void SetData(List<StoreAvatarParts> storeCustom)
    {
        Util.RefreshScrollView(go_ScrollView.gameObject, nameof(go_ScrollView));

        int count = storeCustom.Count;
        if (count < 1) return;

        // 초기화
        totalPrice = 0;
        if (purchaseList.Count != 0)
        {
            foreach (var item in purchaseList)
            {
                item.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < count; i++)
        {
            itemName = storeCustom[i].itemNameID; // 이름
            size = Util.Enum2String(storeCustom[i].size); // 사이즈
            color = Util.Enum2String(storeCustom[i].color); // 색깔
            quantity = storeCustom[i].quantity; // 수량

            // 필요한 만큼 생성
            if (purchaseList.Count < i + 1)
            {
                purchaseList.Add(CreateItem());
            }

            purchaseList[i].SetData(itemName, size, color, quantity.ToString()); // 아이템이름 ( 사이즈 / 색깔 ) 수량 로컬라이징

            totalPrice += storeCustom[i].TotalAmount();
        }

        Util.SetMasterLocalizing(txtmp_Money, totalPrice.ToString("N0") + " KRW");
    }

    Item_CommercePurchase CreateItem()
    {
        GameObject go = Instantiate(purchaseItem.gameObject);
        if (go_Pos != null)
        {
            Util.SetParentPosition(go, go_Pos);
        }
        go.SetActive(false);
        Item_CommercePurchase purchase = go.GetComponent<Item_CommercePurchase>();
        return purchase;
    }

    protected override void OnConfirm()
    {
        base.OnConfirm();

        SceneLogic.instance.isUILock = false;
        SceneLogic.instance.PushPopup<Popup_CompletePayment>();
    }

    protected override void OnCancel()
    {
        base.OnCancel();

        if (StoreManager.Instance.ConveyorBeltController.Switch != null && StoreManager.Instance.ConveyorBeltController.Switch.interacting)
        {
            SceneLogic.instance.isUILock = false;
            SceneLogic.instance.PopPanel();

            SceneLogic.instance.GetPanel<Panel_ConveyorBelt>().CloseUI();
            StoreManager.Instance.AvatarChangeController.InitOutfit();
            StoreManager.Instance.PurchaseController.ResetBasket();
        }
        StoreManager.Instance.ConveyorBeltController.complete = true;
    }
    #endregion
}
