using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using TMPro;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;

public class Popup_GiftboxDetail : PopupBase
{
    private Image img_thumbnail;
    private Button btn_Exit;
    private Button btn_GiftboxGet;
    private TMP_Text txtmp_Count;
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Summary;
    private TMP_Text txtmp_Desc;
    private TMP_Text txtmp_ItemDetail;
    private TMP_Text txtmp_Time;
    private TMP_Text txtmp_Confirm;
    private GameObject go_GiftboxThumbnail;

    private Postbox mailData;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        btn_Exit = GetUI_Button(nameof(btn_Exit), Back);
        btn_GiftboxGet = GetUI_Button("btn_GiftboxGet", OnClickGiftGet);
        img_thumbnail = GetUI_Img("img_thumbnail");
        txtmp_Title = GetUI_TxtmpMasterLocalizing("txtmp_Title");
        txtmp_Summary = GetUI_TxtmpMasterLocalizing("txtmp_Summary");
        txtmp_Desc = GetUI_TxtmpMasterLocalizing("txtmp_Desc");
        txtmp_ItemDetail = GetUI_TxtmpMasterLocalizing("txtmp_ItemDetail");
        txtmp_Count = GetUI_TxtmpMasterLocalizing("txtmp_Count");
        txtmp_Time = GetUI_TxtmpMasterLocalizing("txtmp_Time");
        txtmp_Confirm = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Confirm), new MasterLocalData("mailbox_receive"));
        go_GiftboxThumbnail = GetChildGObject(nameof(go_GiftboxThumbnail));
    }

    void OnClickGiftGet()
    {
        Single.Web.webPostbox.PostboxReceiveReq(mailData.id, (res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
            {
                SetCloseEndCallback(() =>
                {
                    SceneLogic.instance.isUILock = false;
                    Panel_Mailbox panel = SceneLogic.instance.GetPanel<Panel_Mailbox>();
                    if (panel != null)
                    {
                        View_Giftbox view = panel.GetView<View_Giftbox>();
                        if (view != null)
                            view.StartReciveItemEffect(new int[] { res.id }, res.receivedItems);
                    }
                });
                SceneLogic.instance.PopPopup();
            }
        });
    }

    void InitUI()
    {
        img_thumbnail.sprite = null;
        txtmp_Title.text = string.Empty;
        txtmp_Summary.text = string.Empty;
        txtmp_Desc.text = string.Empty;
        txtmp_ItemDetail.text = string.Empty;
        txtmp_Count.text = string.Empty;
        txtmp_Time.text = string.Empty;
        btn_GiftboxGet.interactable = false;
        go_GiftboxThumbnail.SetActive(false);
    }

    public void SetData(int idx)
    {
        InitUI();

        if (idx > LocalPlayerData.Method.GetGiftMailCount())
        {
            Debug.LogErrorFormat("cant find giftMail idx : {0}", idx);
            return;
        }

        mailData = LocalPlayerData.Method.GetGiftMailByIndex(idx);

        txtmp_Time.text = Util.GetRemainTime(mailData.sendedAt, mailData.period);
        txtmp_Title.text = mailData.subject;
        txtmp_Summary.text = mailData.summary;
        txtmp_Desc.text = mailData.content;

        if (mailData.item != null)
        {
            PostboxItem itemData = mailData.item;
            
            img_thumbnail.sprite = Util.GetItemIconSprite(itemData.appendValue);
            
            txtmp_ItemDetail.text = Single.MasterData.TryGetItem(itemData.appendValue, out db.Item itemDb) ? Util.GetMasterLocalizing(itemDb.name) : string.Empty;
            txtmp_Count.text = string.Format("x{0}", itemData.count);

            btn_GiftboxGet.interactable = true;
            go_GiftboxThumbnail.SetActive(true);
        }
    }
}