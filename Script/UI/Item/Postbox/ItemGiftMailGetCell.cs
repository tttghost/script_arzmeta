using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

class ItemGiftMailGetCell : FancyGridViewCell_Custom
{
    private Image img_thumbnail;
    private TMP_Text txtmp_Count;
    private ItemGiftMailGetItemData data;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        img_thumbnail = uIBase.GetUI_Img(nameof(img_thumbnail));
        txtmp_Count = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_Count));
    }

    public override void UpdateContent(Item_Data itemData)
    {
        data = (ItemGiftMailGetItemData)itemData;

        img_thumbnail.sprite = Util.GetItemIconSprite(data.id);
        txtmp_Count.text = string.Format("x{0}", data.count);
    }
}