using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using Google.Protobuf.Collections;
using MEC;
using FrameWork.Network;
using FrameWork.UI;
using Protocol;
using static Protocol.S_BASE_ADD_OBJECT.Types;
using Newtonsoft.Json;

public class View_OfficeObserverInfo : View_OfficeRealTime
{
    private GameObject go_MenuTextGroup;
    private GameObject go_MenuToggleGroup;

    // 관리자 정보 UI
    private TMP_Text txtmp_AdminNickName;
    private Image img_Thumbnail_Admin;
    private Button btn_ChangeSendData;

    private ScrollRect sview_Content;
    private TMP_Text txtmp_Caution;

    private TMP_Text txtmp_ChangeSendData;

    // 오피스룸에 참여하고 있는 참여자 권한정보
    private List<Item_OfficeUser> scrollitemList = new List<Item_OfficeUser>();

    private StringBuilder sbTemp = new StringBuilder();

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        // 관리자 이름
        txtmp_AdminNickName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AdminNickName));

        GetUI_TxtmpMasterLocalizing("txtmp_ChangeAudience", new MasterLocalData("office_getting_enter"));
        GetUI_TxtmpMasterLocalizing("txtmp_ChatAll", new MasterLocalData("office_permission_chat"));
        GetUI_TxtmpMasterLocalizing("txtmp_Kick", new MasterLocalData("office_kick"));

        // 스크롤에 아무것도 없을 때 비어있음을 표시할 텍스트
        txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("common_state_empty"));

        TogglePlus togplus_ChatPermissionAll =  GetUI<TogglePlus>(nameof(togplus_ChatPermissionAll));
        togplus_ChatPermissionAll.SetToggleAction(OnValueChanged_ChatPermissionAll);

        // 관리자 Thumbnail
        img_Thumbnail_Admin = GetUI_Img(nameof(img_Thumbnail_Admin));

        // 정보 반영 버튼
        btn_ChangeSendData = GetUI_Button(nameof(btn_ChangeSendData), OnClick_SendPermissions);
        txtmp_ChangeSendData = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChangeSendData), new MasterLocalData("common_save"));

        // ScrollView 캐싱
        sview_Content = GetUI<ScrollRect>(nameof(sview_Content));

        go_MenuTextGroup = GetChildGObject(nameof(go_MenuTextGroup));
        go_MenuToggleGroup = GetChildGObject(nameof(go_MenuToggleGroup));
    }

    #region 실시간서버
    protected override void AddHandler()
    {
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION_ALL, this, S_OFFICE_GET_PERMISSION_ALL);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this, S_OFFICE_SET_PERMISSION);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this, S_OFFICE_KICK);

        if (sceneOfficeRoom.IsAdmin())
        {
            if (go_MenuTextGroup != null)
                go_MenuTextGroup.SetActive(true);

            if (go_MenuToggleGroup != null)
                go_MenuToggleGroup.SetActive(true);

            if (btn_ChangeSendData != null)
                btn_ChangeSendData.interactable = true;
        }
        else
        {
            if (go_MenuTextGroup != null)
                go_MenuTextGroup.SetActive(true);

            if (go_MenuToggleGroup != null)
                go_MenuToggleGroup.SetActive(false);

            if (btn_ChangeSendData != null)
                btn_ChangeSendData.interactable = false;
        }
    }

    protected override void RemoveHandler()
    {
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION_ALL, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this);
    }

    public override void REQUEST_NFO()
    {
        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());

        // 오피스룸에 참여중인 사용자 전체 권한정보 요청
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }


    private void S_OFFICE_SET_PERMISSION(PacketSession session, IMessage packet)
    {
        S_OFFICE_SET_PERMISSION message = packet as S_OFFICE_SET_PERMISSION;
        Util.UtilOffice.OpenWarningPopup(message.Code);
        DEBUG.LOG("S_OFFICE_SET_PERMISSION : " + message.Code, eColorManager.REALTIME);

        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }

    private void S_OFFICE_GET_PERMISSION_ALL(PacketSession session, IMessage packet)
    {
        S_OFFICE_GET_PERMISSION_ALL message = packet as S_OFFICE_GET_PERMISSION_ALL;

        // 스크롤 아이템 삭제
        DestoryAllItems();
        scrollitemList.Clear();

        UserData userdata = null;
        int objectId = -1;
        int observerCount = 0;
        Dictionary<string, int> avatarDatas = null;

        foreach (OfficeUserInfo userinfo in message.Permissions)
        {
            switch ((OfficeAuthority)userinfo.Authority)
            {
            case OfficeAuthority.관리자:
                {
                    // 1. ClientId 를 통해, Object ID 가져오기
                    if (sceneOfficeRoom.networkHandler.GetObjectIds.TryGetValue(userinfo.ClientId, out objectId))
                    {
                        // 2. Object ID 를 통해, UserData 가져오기
                        if (sceneOfficeRoom.networkHandler.UserDatas.TryGetValue(objectId, out userdata))
                        {
                            if (txtmp_AdminNickName != null)
                            {
                                var localAuthority = Util.UtilOffice.GetMasterLocal_OfficeAutority(userinfo.Authority);
                                sbTemp.Clear();
                                sbTemp.Append(localAuthority);
                                sbTemp.Append(" : ");
                                sbTemp.Append(userdata.Nickname);
                                txtmp_AdminNickName.text = sbTemp.ToString();
                            }

                            avatarDatas = JsonConvert.DeserializeObject<Dictionary<string, int>>(userdata.ObjectData);
                            if (img_Thumbnail_Admin != null)
                            {
                                LocalPlayerData.Method.GetAvatarSprite(
                                    userdata.OwnerId, avatarDatas, (sprite) => img_Thumbnail_Admin.sprite = sprite);
                            }
                        }
                    }
                }
                break;

            case OfficeAuthority.관전자:
                {
                    ClientData clientdata = null;
                    if (sceneOfficeRoom.networkHandler.Clients.TryGetValue(userinfo.ClientId, out clientdata))
                    {
                        Item_OfficeUser scrollitem =
                        Single.Resources.Instantiate<Item_OfficeUser>(
                            Cons.Path_Prefab_Item + "Item_OfficeUser",
                            sview_Content.content.transform);

                        scrollitem.SetData_Observer(userinfo, clientdata.nickname,
                            (OfficeAuthority)sceneOfficeRoom.myPermission.Authority);

                        scrollitem.gameObject.SetActive(true);

                        scrollitemList.Add(scrollitem);

                        observerCount += 1;
                    }
                }
                break;

            default:
                continue;
            }
        }

        if (observerCount > 0)
        {
            txtmp_Caution.gameObject.SetActive(false);
            if (sceneOfficeRoom.IsAdmin() && btn_ChangeSendData != null)
                btn_ChangeSendData.interactable = true;
        }
        else
        {
            txtmp_Caution.gameObject.SetActive(true);
            if (sceneOfficeRoom.IsAdmin() && btn_ChangeSendData != null)
                btn_ChangeSendData.interactable = false;
        }

        CALLBACK_SEND_PERMISSION?.Invoke();
        CALLBACK_SEND_PERMISSION.RemoveAllListeners();
    }

    private void S_OFFICE_KICK(PacketSession session, IMessage packet)
    {
        S_OFFICE_KICK message = packet as S_OFFICE_KICK;
        if (message.Success == false)
        {
            DEBUG.LOG("S_OFFICE_KICK : 사용자 KICK 실패", eColorManager.REALTIME);
        }

        REQUEST_NFO();
    }
    #endregion

    private void OnClick_SendPermissions()
    {
        C_OFFICE_SET_PERMISSION packet = new C_OFFICE_SET_PERMISSION();
        
        foreach (Item_OfficeUser itemofficeuser in scrollitemList)
        {
            packet.Permissions.Add(itemofficeuser.permissionInfo);
        }

        Single.RealTime.Send(packet);

        CALLBACK_SEND_PERMISSION.AddListener(CallackSendPermission);
    }

    private void OnValueChanged_ChatPermissionAll(bool isOn)
    {
        foreach (Item_OfficeUser scrollitem in scrollitemList)
        {
            scrollitem.SetChatToggleState(isOn);
        }
    }

    /// <summary>
    /// 스크롤 아이템 삭제 
    /// </summary>
    public void DestoryAllItems()
    {
        int count = sview_Content.content.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform child = sview_Content.content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}