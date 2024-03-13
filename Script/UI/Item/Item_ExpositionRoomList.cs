using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using FrameWork.UI;

class Item_ExpositionRoomList : FancyScrollRectCell_Custom
{
    #region Members

    [HideInInspector] public Booth boothInfo;

    TMP_Text txtmp_RoomName;
    TMP_Text txtmp_RoomExplain_bg;

    Image img_Thumbnail;
    Button btn_ShowRoom;

    #endregion



    #region Initialize

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_RoomName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
        txtmp_RoomExplain_bg = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomExplain_bg));

        btn_ShowRoom = uIBase.GetUI_Button(nameof(btn_ShowRoom), OnClick_ShowRoom);

        img_Thumbnail = uIBase.GetUI_Img(nameof(img_Thumbnail));
    }

    public override void UpdateContent(Item_Data itemData)
    {
        if (itemData is Item_ExpositionBooth _data)
        {
            SetRoomInfo(_data.boothInfo);

            base.UpdateContent(itemData);
        }
    }

    private void SetRoomInfo(Booth _boothInfo)
    {
        if (txtmp_RoomName != null)
            Util.SetMasterLocalizing(txtmp_RoomName, _boothInfo.name);
        if (txtmp_RoomExplain_bg != null)
            Util.SetMasterLocalizing(txtmp_RoomExplain_bg, _boothInfo.description);

        // 썸네일
        if (img_Thumbnail != null)
        {
            if (!string.IsNullOrEmpty(_boothInfo.thumbnail))
            {
                LocalPlayerData.Method.Load_ExpositionThumbnail(_boothInfo.id, _boothInfo.thumbnail, SetSprite);
            }
            else
            {
                SetSprite(Util.GetLogoSprite());
            }
        }

        boothInfo = _boothInfo;
    }
    private void SetSprite(Sprite sprite)
    {
        img_Thumbnail.sprite = sprite;
        Util.ZoomImage_Crop(img_Thumbnail);
    }

    #endregion



    #region Button Events

    private void OnClick_ShowRoom()
    {
        uIBase.GetPopup<Popup_ExpositionRoomInfo>().SetExpositionCardInfo(boothInfo);
        uIBase.PushPopup<Popup_ExpositionRoomInfo>();
    }

    #endregion
}