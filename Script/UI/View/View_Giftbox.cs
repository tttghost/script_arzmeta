using FrameWork.UI;
using Gpm.Ui;
using Gpm.Ui.Sample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using db;

public class View_Giftbox : UIBase
{
    public class GiftItemInfo
    {
        //public int postId;
        public int giftItemId;
        public int numForInterior;
        public int count;
    }

    private TMP_Text txtmp_NoItem;
    private TMP_Text txtmp_Summary;
    private TMP_Text txtmp_Count;
    private TMP_Text txtmp_GetItemAll;

    private Button btn_Back;
    private Button btn_GetItemAll;
    private Button btn_RefreshItem;
    
    private ScrollView_Custom scroll;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_NoItem = GetUI_TxtmpMasterLocalizing(nameof(txtmp_NoItem), new MasterLocalData("common_state_empty"));
        txtmp_Summary = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Summary), new MasterLocalData("mailbox_notice_autodelete"));
        txtmp_Count = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count), new MasterLocalData("common_page", "0", "50"));
        txtmp_GetItemAll = GetUI_TxtmpMasterLocalizing(nameof(txtmp_GetItemAll), new MasterLocalData("mailbox_receive_all"));
        btn_Back = GetUI_Button(nameof(btn_Back), Back);
        btn_RefreshItem = GetUI_Button(nameof(btn_RefreshItem), OnClick_RefreshItem);
        btn_GetItemAll = GetUI_Button(nameof(btn_GetItemAll), OnClick_GetItemAll);

        InitScroll();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        GenerateCells();
    }

    /// <summary> 스크롤 초기화 </summary>
    private void InitScroll()
    {
        scroll = GetComponentInChildren<ScrollView_Custom>(true);
        scroll.OnCellClicked((idx) => SceneLogic.instance.PushPopup<Popup_GiftboxDetail>().SetData(idx));
    }

    /// <summary> 아이템 새로고침 </summary>
    private void OnClick_RefreshItem()
    {
        Single.Web.webPostbox.PostboxReq((res) => GenerateCells());
    }

    /// <summary> 아이템 모두받기 </summary>
    private void OnClick_GetItemAll()
    {
        if (LocalPlayerData.Method.GetGiftMailCount() <= 0)
            return;

        Single.Web.webPostbox.PostboxReceiveAllReq((res) =>
        {
            if (res.error == (int)WEBERROR.NET_E_SUCCESS)
            {
                StartReciveItemEffect(res.ids, res.receivedItems);
            }
        });
    }

    void GenerateCells()
    {
        int dataCount = LocalPlayerData.Method.GetGiftMailCount();
        var items = Enumerable.Range(0, dataCount)
            .Select(i => new ItemGiftMailItemData() { postbox = LocalPlayerData.Method.GetGiftMailByIndex(i) })
            .ToArray();

        scroll.UpdateData(items);
        scroll.JumpTo(0);

        txtmp_NoItem.gameObject.SetActive(dataCount <= 0);
    }

    /// <summary> 우편 아이템 수령 후 연출 혹은 요약 팝업 호출 처리 /// </summary>
    public void StartReciveItemEffect(int[] postIds, ReceivedItem[] items)
    {
        // 받을 보상이 없는 경우 무시
        if (items.Length <= 0)
        {
            OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.None, new MasterLocalData("mailbox_error_overholding")));
            return;
        }

        // 인벤토리에 보상 아이템 추가
        for (int i = 0; i < items.Length; i++)
        {
            int itemId = items[i].itemId;
            int itemType = items[i].itemType;
            int interiorNum = items[i].num;

            if (!Single.MasterData.TryGetItem(itemId, out db.Item itemDb))
                DEBUG.LOGERROR(string.Format("cant find item in db : {0}", itemId));
            else
            {
                switch((ITEM_TYPE)itemType)
                {
                    case ITEM_TYPE.NOMAL:
                        break;
                    case ITEM_TYPE.INTERIOR:
                        LocalPlayerData.Method.AddInterior(itemId, interiorNum);
                        break;
                    case ITEM_TYPE.COSTUME:
                        LocalPlayerData.Method.AddAvatarParts(itemId);
                        break;
                }
            }
        }

        List<GiftItemInfo> receivedItems = new List<GiftItemInfo>();    // 같은 id를 하나로 묶은 전체 획득 아이템
        Queue<GiftItemInfo> effectItems = new Queue<GiftItemInfo>();    // 같은 id를 하나로 묶은 연출이 필요한 획득 아이템

        for (int i = 0; i < items.Length; i++)
        {
            int receivedId = items[i].itemId;
            int num = items[i].num;

            if (Single.MasterData.TryGetItem(receivedId, out Item itemDb))
            {
                // 특수 연출이 존재하는 아이템 필터링
                bool isExistEffect = false;
                PostalItemProperty itemPropertyDb = Single.MasterData.dataPostalItemProperty.GetData(itemDb.itemType, itemDb.categoryType);
                if (itemPropertyDb != null)
                {
                    PostalEffectType effectTypeDb = Single.MasterData.dataPostalEffectType.GetData(itemPropertyDb.postalEffectType);
                    if (effectTypeDb != null)
                    {
                        switch((POSTAL_EFFECT_TYPE)effectTypeDb.type)
                        {
                            case POSTAL_EFFECT_TYPE.FULL_SCREEN:
                                isExistEffect = true;
                                break;
                            default:
                                isExistEffect = false;
                                break;
                        }
                    }
                }

                if (isExistEffect)
                {
                    GiftItemInfo effectItem = effectItems.FirstOrDefault(c => c.giftItemId == receivedId);
                    if (effectItem != null)
                        effectItem.count++;
                    else
                    {
                        effectItem = new GiftItemInfo() { giftItemId = receivedId, count = 1, numForInterior = num };
                        effectItems.Enqueue(effectItem);
                    }
                }

                GiftItemInfo receivedItem = receivedItems.FirstOrDefault(c => c.giftItemId == receivedId);
                if (receivedItem != null)
                    receivedItem.count++;
                else
                {
                    receivedItem = new GiftItemInfo() { giftItemId = receivedId, count = 1, numForInterior = num };
                    receivedItems.Add(receivedItem);
                }
            }
        }

        if (effectItems.Count > 0)
        {
            Panel_GiftboxGet panelGiftBoxGet = SceneLogic.instance.PushPanel<Panel_GiftboxGet>();
            panelGiftBoxGet.SetData(effectItems);
            panelGiftBoxGet.SetCloseEndCallback(() =>
            {
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PushPopup<Popup_GiftboxGet>().SetData(receivedItems);
            });
        }
        else
            SceneLogic.instance.PushPopup<Popup_GiftboxGet>().SetData(receivedItems);

        // 아이템 받기 완료 우편 삭제 후 스크롤 뷰 갱신
        for (int i = 0; i < postIds.Length; i++)
        {
            LocalPlayerData.Method.RemovePostbox(postIds[i]);
        }
        GenerateCells();
    }
}