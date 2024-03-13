using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UI.Extensions.ScrollView_Custom;
using TMPro;
using FrameWork.UI;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;

[RequireComponent(typeof(UIBase))]
class ItemGiftMailCell : FancyScrollRectCell_Custom
{
    private Image img_thumbnail;
    private Button btn_GiftboxDetail;
    private Button btn_GiftboxGet;
    private TMP_Text txtmp_Count;
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Summary;
    private TMP_Text txtmp_Time;
    private TMP_Text txtmp_GetItem;
    private GameObject go_GiftboxThumbnail;

    private UIBase uiBase;
    private Postbox mailData;

    public override void Initialize()
    {
        uiBase = GetComponent<UIBase>();

        btn_GiftboxDetail = uiBase.GetUI_Button(nameof(btn_GiftboxDetail), () => Context.OnCellClicked?.Invoke(Index));
        btn_GiftboxGet = uiBase.GetUI_Button(nameof(btn_GiftboxGet), OnClickGiftGet);
        img_thumbnail = uiBase.GetUI_Img(nameof(img_thumbnail));
        txtmp_Count = uiBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));
        txtmp_Title = uiBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_Summary = uiBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Summary));
        txtmp_Time = uiBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Time));
        txtmp_GetItem = uiBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_GetItem), new MasterLocalData("mailbox_receive"));
        go_GiftboxThumbnail = uIBase.GetChildGObject(nameof(go_GiftboxThumbnail));
    }

    void InitUI()
    {
        img_thumbnail.sprite = null;
        txtmp_Title.text = string.Empty;
        txtmp_Summary.text = string.Empty;
        txtmp_Time.text = string.Empty;
        txtmp_Count.text = string.Empty;
        btn_GiftboxGet.interactable = false;
        go_GiftboxThumbnail.SetActive(false);
    }

    public override void UpdateContent(Item_Data itemData)
    {
        InitUI();

        mailData = null;

        var selected = Context.SelectedIndex == Index;
        //image.color = selected
        //    ? new Color32(0, 255, 255, 100)
        //    : new Color32(255, 255, 255, 77);

        if (itemData is ItemGiftMailItemData)
        {
            mailData = ((ItemGiftMailItemData)itemData).postbox;
            
            txtmp_Time.text = Util.GetRemainTime(mailData.sendedAt, mailData.period);
            txtmp_Title.text = mailData.subject;
            txtmp_Summary.text = mailData.summary;

            if (mailData.item != null)
            {
                PostboxItem mailItemData = mailData.item;
                img_thumbnail.sprite = Util.GetItemIconSprite(mailItemData.appendValue);
                txtmp_Count.text = string.Format("x{0}", mailItemData.count);
                btn_GiftboxGet.interactable = true;
                go_GiftboxThumbnail.SetActive(true);
            }
        }
    }

    protected override void UpdatePosition(float normalizedPosition, float localPosition)
    {
        base.UpdatePosition(normalizedPosition, localPosition);

        //var wave = Mathf.Sin(normalizedPosition * Mathf.PI * 2) * 65;
        //transform.localPosition += Vector3.right * wave;
    }

    void OnClickGiftGet()
    {
        Single.Web.webPostbox.PostboxReceiveReq(mailData.id, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
            {
                Panel_Mailbox panel = SceneLogic.instance.GetPanel<Panel_Mailbox>();
                if (panel != null)
                {
                    View_Giftbox view = panel.GetView<View_Giftbox>();
                    if (view != null)
                        view.StartReciveItemEffect(new int[] { res.id }, res.receivedItems);
                }
            }
        });
    }
}