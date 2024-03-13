using System.Text;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Google.Protobuf;
using FrameWork.Network;
using FrameWork.UI;
using Newtonsoft.Json;

public partial class Item_OfficeUser : UIBase
{
	private Scene_OfficeRoom scene_OfficeRoom;

    public OfficeUserInfo permissionInfo { get; private set; }

    public string waitingUserClientId { get; private set; }

    private string nickName;

	private GameObject go_AdminBg;
    private GameObject go_GuestBg;
    private GameObject go_ThumbNail;

    private StringBuilder sbTemp = new StringBuilder();

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        scene_OfficeRoom = (Scene_OfficeRoom)SceneLogic.instance;

        go_AdminBg = GetChildGObject( nameof( go_AdminBg ) );
        go_GuestBg = GetChildGObject( nameof( go_GuestBg ) );
        go_ThumbNail = GetChildGObject( nameof( go_ThumbNail ) );

        SetMemberUI_Permission();
        SetMemberUI_Observer();
        SetMemberUI_Waitting();
    }
}
