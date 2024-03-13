/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using FrameWork.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

class Item_Costume : FancyGridViewCell_Custom
{
    #region 변수
    private Item_CostumeData data;

    private RectTransform rectTransform;
    private Image img_Item;
    private GameObject go_Lock;
    private GameObject go_Glow;

    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        uIBase.GetUI_Button("btn_Costume", () => { if (!IsLock()) SelectIndex(); });
        #endregion

        #region Image
        img_Item = uIBase.GetUI_Img(nameof(img_Item));
        #endregion

        #region etc
        go_Lock = uIBase.GetChildGObject(nameof(go_Lock));
        go_Glow = uIBase.GetChildGObject(nameof(go_Glow));
        if (go_Glow != null)
        {
            go_Glow.SetActive(false);
        }
        rectTransform = GetComponent<RectTransform>();
        #endregion
    }


    #region 데이터 세팅 및 선택 로직
    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_CostumeData _data)
        {
            data = _data;

            base.UpdateContent(itemData);
        }
    }

    /// <summary>
    /// 데이터 세팅
    /// </summary>
    protected override void SetContent()
    {
        if (img_Item != null)
        {
            if (!string.IsNullOrEmpty(data.thumbnail))
            {
                if (!data.isSpecial)
                {
                    Sprite sprite = Single.ItemData.GetThumbnail(data.thumbnail);
                    if (sprite != null)
                    {
                        img_Item.sprite = sprite;
                    }
                }
                else
                {
                    LocalPlayerData.Method.Load_NFTThumbnail(Single.ItemData.GetItemData(AVATAR_PARTS_TYPE.nft_special).GetData(data.id).thumbnail, (_sprite) => img_Item.sprite = _sprite);
                }
            }
            else
            {
                Debug.Log(data.thumbnail + "의 썸네일이 존재하지 않습니다!");
            }
        }

        if (go_Lock != null)
        {
            go_Lock.SetActive(IsLock());
        }

        if (go_Glow != null)
        {
            bool b = IsSelected() && !IsLock();
            go_Glow.SetActive(b);
        }

        SetCellSize();
    }

    /// <summary>
    /// 아이템 선택 시 눌렀을 때 벗겨질 수 있는 아이템이면 인덱스 처리 (버튼 탭 시 호출)
    /// </summary>
    private void SelectIndex()
    {
        bool b = data.isReset && IsSelected();

        int itemId = b ? -1 : data.id;
        int selectIndex = b ? -1 : Index;

        // 아이템 선택
        if (Context.PreSelectIdx != Index || data.isReset)
        {
            data.action.Invoke(itemId);
            Context.OnCellClicked?.Invoke(selectIndex);
        }

        // 이전 아이템 인덱스로 저장
        Context.PreSelectIdx = selectIndex;
    }

    /// <summary>
    /// 아이템이 잠겼는지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsLock()
    {
        if (data == null) return false;

        switch ((PURCHASE_PRICE_TYPE)data.purchaseType)
        {
            case PURCHASE_PRICE_TYPE.비매품:
            case PURCHASE_PRICE_TYPE.무료:
            case PURCHASE_PRICE_TYPE.이벤트: // 사용자가 소지한 아이템인지 체크
                return false;
            case PURCHASE_PRICE_TYPE.반유료: // 사용자가 구매하거나 소지한 아이템인지 체크
            case PURCHASE_PRICE_TYPE.유료:
            default:
                return true;
        }
    }

    /// <summary>
    /// 선택된 아이템인지 여부
    /// </summary>
    /// <returns></returns>
    private bool IsSelected()
    {
        return Context.SelectedIndex == Index;
    }

    /// <summary>
    /// 아이템 사이즈 조정
    /// </summary>
    private void SetCellSize()
    {
        rectTransform.sizeDelta = data.cellSize;
    }
    #endregion
}
