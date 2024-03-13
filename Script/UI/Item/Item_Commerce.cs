using FrameWork.UI;
using MEC;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class Item_Commerce : FancyCell_Custom
{
    #region 변수
    [SerializeField] private Sprite disableSprite;
    private Sprite oriSprite;

    private Panel_Kiosk panel_Kiosk;

    private Item_CommerceData data;

    private Image img_Thumbnail;
    private Image img_CostumColor;
    private Image img_Cart;
    private TMP_Text txtmp_ItemName;
    private TMP_Text txtmp_Price;
    private TMP_Text txtmp_Size;
    private TMP_Text txtmp_Quantity;
    private Toggle tog_Cart;
    private CanvasGroup cg_cart_bg;
    private CanvasGroup cg_CnavasGroup;
    private CanvasGroup cg_Dim;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_ItemName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_ItemName));
        txtmp_Price = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Price));
        txtmp_Size = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Size));
        txtmp_Quantity = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Quantity));

        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_ItemTitle", new MasterLocalData("30002"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_ColorTitle", new MasterLocalData("30029"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_SizeTitle", new MasterLocalData("30027"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_QuantityTitle", new MasterLocalData("30003"));
        uIBase.GetUI_TxtmpMasterLocalizing("txtmp_Cart", new MasterLocalData("30004"));
        #endregion

        #region Button
        uIBase.GetUI_Button("btn_Dim", () => { if (Index >= 0) Context.OnCellClicked?.Invoke(Index); });
        uIBase.GetUI_Button("btn_MinusColor", () => OnClick_NextPrevious(Define.Option.Color, false));
        uIBase.GetUI_Button("btn_PlusColor", () => OnClick_NextPrevious(Define.Option.Color, true));
        uIBase.GetUI_Button("btn_MinusSize", () => OnClick_NextPrevious(Define.Option.Size, false));
        uIBase.GetUI_Button("btn_PlusSize", () => OnClick_NextPrevious(Define.Option.Size, true));
        uIBase.GetUI_Button("btn_MinusQuantity", () => OnClick_NextPrevious(Define.Option.Quantity, false));
        uIBase.GetUI_Button("btn_PlusQuantity", () => OnClick_NextPrevious(Define.Option.Quantity, true));
        #endregion

        #region Toggle
        tog_Cart = uIBase.GetUI_Toggle(nameof(tog_Cart), (b) => { OnValueChanged_Cart(b); });
        if (tog_Cart != null)
        {
            img_Cart = (Image)tog_Cart.targetGraphic;
            oriSprite = img_Cart.sprite;
        }
        #endregion

        #region Image
        img_Thumbnail = uIBase.GetUI_Img(nameof(img_Thumbnail));
        img_CostumColor = uIBase.GetUI_Img(nameof(img_CostumColor));
        #endregion

        #region etc
        panel_Kiosk = SceneLogic.instance.GetPanel<Panel_Kiosk>();

        cg_cart_bg = uIBase.GetUI_Img("img_cart_bg").GetComponent<CanvasGroup>();
        if (cg_cart_bg != null)
        {
            cg_cart_bg.alpha = 0f;
        }
        cg_CnavasGroup = uIBase.GetChildGObject("go_CnavasGroup").GetComponent<CanvasGroup>();
        if (cg_CnavasGroup != null)
        {
            cg_CnavasGroup.alpha = 0f;
        }
        cg_Dim = uIBase.GetChildGObject("go_Dim").GetComponent<CanvasGroup>();
        if (cg_Dim != null)
        {
            cg_Dim.alpha = 0f;
            cg_Dim.blocksRaycasts = false;
        }
        #endregion
    }

    #region Item_Commerce

    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_CommerceData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    protected override void SetAnimatorHash() => animatorHashName = Cons.Ani_CommerceScroll;

    protected override void SetContent()
    {
        ChangeLook(data.costum);

        if (txtmp_Price != null)
        {
            Util.SetMasterLocalizing(txtmp_Price, data.costum.price.ToString("N0") + " KRW");
        }

        Timing.RunCoroutine(Co_ActiveButtonToggle(Context.SelectedIndex == Index));

        if (tog_Cart != null)
        {
            tog_Cart.isOn = data.costum.purchase;
            img_Cart.sprite = data.costum.purchase ? oriSprite : disableSprite;
        }
    }

    /// <summary>
    /// 아이템 이름, 컬러 이미지, 썸네일, 마네킹 변경
    /// </summary>
    private void ChangeLook(StoreAvatarParts costum)
    {
        if (txtmp_ItemName != null)
        {
            Util.SetMasterLocalizing(txtmp_ItemName, new MasterLocalData(costum.itemNameID));
        }
        if (txtmp_Size != null)
        {
            Util.SetMasterLocalizing(txtmp_Size, costum.size.ToString());
        }
        if (txtmp_Quantity != null)
        {
            Util.SetMasterLocalizing(txtmp_Quantity, costum.quantity.ToString());
        }
        if (img_CostumColor != null)
        {
            img_CostumColor.color = costum.CurrentRGBColor();
        }

        int colorCount = costum.colorList.Length;
        for (int i = 0; i < colorCount; i++)
        {
            if (costum.colorList[i].color == costum.color)
            {
                img_Thumbnail.sprite = costum.colorList[i].thumbnail;
            }
        }
    }

    private IEnumerator<float> Co_ActiveButtonToggle(bool isbool)
    {
        float curTime = 0f;
        float durTime = 0.2f;

        // Canvas
        float start = isbool ? 0f : 1f;
        float end = isbool ? 1f : 0f;
        // Cart
        float startCart = data.costum.purchase && !isbool ? 0f : 1f;
        float endCart = data.costum.purchase && !isbool ? 1f : 0f;
        // Dim
        float startDim = !isbool ? 0f : 1f;
        float endDim = !isbool ? 1f : 0f;

        if (cg_CnavasGroup.alpha != end || cg_cart_bg.alpha != endCart)
        {
            while (curTime < 1f)
            {
                curTime += Time.deltaTime / durTime;

                cg_CnavasGroup.alpha = Mathf.Lerp(start, end, curTime); // Canvas
                cg_cart_bg.alpha = Mathf.Lerp(startCart, endCart, curTime); // Cart
                cg_Dim.alpha = Mathf.Lerp(startDim, endDim, curTime); // Dim

                yield return Timing.WaitForOneFrame;
            }
        }

        // Canvas
        cg_CnavasGroup.alpha = end;
        // Cart
        cg_cart_bg.alpha = endCart;
        // Dim
        cg_Dim.alpha = endDim;
        cg_Dim.blocksRaycasts = !isbool;
    }

    /// <summary>
    /// 옵션
    /// </summary>
    /// <param name="option"></param>
    /// <param name="isNext"></param>
    private void OnClick_NextPrevious(Define.Option option, bool isNext)
    {
        switch (option)
        {
            case Define.Option.Size:
                if (isNext) data.costum.PlusSize();
                else data.costum.MinusSize();
                break;
            case Define.Option.Color:
                if (isNext) data.costum.PlusColor();
                else data.costum.MinusColor();
                break;
            case Define.Option.Quantity:
                if (isNext) data.costum.PlusQuantity();
                else data.costum.MinusQuantity();
                break;
        }
        ChangeLook(data.costum);
        panel_Kiosk.ChangeAvatarSetting(data.costum);
    }

    /// <summary>
    /// 장바구니 선택 유무
    /// </summary>
    /// <param name="b"></param>
    private void OnValueChanged_Cart(bool b)
    {
        if (data != null)
        {
            data.costum.purchase = b;
            img_Cart.sprite = b ? oriSprite : disableSprite;
        }
    }

    public override void UpdatePosition(float position)
    {
        if (position <= 0.4f || position >= 0.6f)
        {
            if (Index >= 0)
            {
                Context.OnCellClicked?.Invoke(Index);
            }
        }

        base.UpdatePosition(position);
    }

    #endregion
}
