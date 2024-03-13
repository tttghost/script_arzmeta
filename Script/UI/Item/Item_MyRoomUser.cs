using FrameWork.UI;
using Newtonsoft.Json;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_MyRoomUser : UIBase
{
    public UserData UserData { get; private set; }

    private Image img_Thumbnail;
    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_Message;
    private Button btn_PlayerCard;
    private Button btn_Kick;
    private TMP_Text txtmp_Kick;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        dontSetActiveFalse = true;
        img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));
        txtmp_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_Message = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Message));
        btn_PlayerCard = GetUI_Button(nameof(btn_PlayerCard), OnClick_PlayerCard);
        btn_Kick = GetUI_Button(nameof(btn_Kick), OnClick_Kick);
        txtmp_Kick = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Kick), new MasterLocalData("myroom_kick"));
    }

    private void OnClick_PlayerCard()
    {
        //해당유저 팝업 패널 오픈
        SceneLogic.instance.PopPopup();
        SceneLogic.instance.isUILock = false;
        PushPanel<Panel_ArzProfile>().SetPlayerInfo(OTHERINFO_TYPE.MEMBERCODE, UserData.OwnerId);
    }

    private void OnClick_Kick()
    {
        PushPopup<Popup_Basic>()
               .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("myroom_confirm_kick")))
               .ChainPopupAction(new PopupAction(() => MyRoomManager.Instance.myroomModuleHandler.C_MYROOM_KICK(UserData.OwnerId)));
    }



    public void SetData(UserData UserData)
    {
        if(this.UserData != null && this.UserData.ObjectId != UserData.ObjectId)
        {
            return;
        }
        this.UserData = UserData;


        var avatarData = JsonConvert.DeserializeObject<Dictionary<string, int>>(UserData.ObjectData);
        LocalPlayerData.Method.GetAvatarSprite(UserData.OwnerId, avatarData, (sprite) => img_Thumbnail.sprite = sprite);
        txtmp_Nickname.text = UserData.Nickname;
        txtmp_Message.text = UserData.StateMessage;

        btn_Kick.gameObject.SetActive(LocalPlayerData.Method.IsMyRoom && UserData.OwnerId != LocalPlayerData.MemberCode);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GetPopup<Popup_MyRoomSetting>().userDataHandler += SetData;
        if (UserData != null) SetData(UserData);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GetPopup<Popup_MyRoomSetting>().userDataHandler -= SetData;
    }
}
