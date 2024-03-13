using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;
using FrameWork.Network;
using FrameWork.UI;
using Protocol;
using Newtonsoft.Json;

public class View_OfficeUserInfo : View_OfficeRealTime
{
    private ScrollRect sview_Content;
    private GameObject go_PermissionTextGroup;
    private GameObject go_PermissionToggleGroup;
    private Button btn_ChangeSendData;

    // 현재 참여중인 오피스룸 정보
    private S_OFFICE_GET_ROOM_INFO officeRoomInfo = null;

    // 오피스룸에 참여하고 있는 참여자 권한정보
    private List<Item_OfficeUser> scrollitemList = new List<Item_OfficeUser>();
    public Item_OfficeUser itemMasterUser { get; private set; }

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        GetUI_TxtmpMasterLocalizing("txtmp_Position", new MasterLocalData("office_participant_type"));
        GetUI_TxtmpMasterLocalizing("txtmp_Display", new MasterLocalData("office_permission_screen"));
        GetUI_TxtmpMasterLocalizing("txtmp_Chat", new MasterLocalData("office_permission_chat"));
        GetUI_TxtmpMasterLocalizing("txtmp_Voice", new MasterLocalData("office_permission_voicechat"));
        GetUI_TxtmpMasterLocalizing("txtmp_Video", new MasterLocalData("office_permission_videochat"));
        GetUI_TxtmpMasterLocalizing("txtmp_KickOut", new MasterLocalData("office_kick"));
        GetUI_TxtmpMasterLocalizing("txtmp_ChangeSendData", new MasterLocalData("common_save"));

        TogglePlus togplus_MasterChat = GetUI<TogglePlus>(nameof(togplus_MasterChat));
        togplus_MasterChat.SetToggleAction(OnValueChanged_MasterChat);

        TogglePlus togplus_MasterVoice = GetUI<TogglePlus>(nameof(togplus_MasterVoice));
        togplus_MasterVoice.SetToggleAction(OnValueChanged_MasterVoice);

        TogglePlus togplus_MasterVideo = GetUI<TogglePlus>(nameof(togplus_MasterVideo));
        togplus_MasterVideo.SetToggleAction(OnValueChanged_MasterVideo);
        

        // ScrollView 캐싱
        sview_Content = GetUI<ScrollRect>(nameof(sview_Content));

        go_PermissionTextGroup = GetChildGObject(nameof(go_PermissionTextGroup));
        go_PermissionToggleGroup = GetChildGObject(nameof(go_PermissionToggleGroup));

        btn_ChangeSendData = GetUI_Button(nameof(btn_ChangeSendData), OnClick_SendPermissions);
    }

    #region 실시간서버
    protected override void AddHandler()
    {
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_ROOM_INFO, this, S_OFFICE_GET_ROOM_INFO);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION_ALL, this, S_OFFICE_GET_PERMISSION_ALL);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this, S_OFFICE_SET_PERMISSION);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this, S_OFFICE_KICK);

        if (sceneOfficeRoom.IsAdmin())
        {
            go_PermissionTextGroup.SetActive(true);
            go_PermissionToggleGroup.SetActive(true);
        }
        else
        {
            go_PermissionTextGroup.SetActive(true);
            go_PermissionToggleGroup.SetActive(false);
        }
    }

    /// <summary>
    ///  오피스룸 정보 & 참여자 권한정보 요청
    /// </summary>
    protected override void RemoveHandler()
    {
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_ROOM_INFO, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_GET_PERMISSION_ALL, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_SET_PERMISSION, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_OFFICE_KICK, this);
    }

    public override void REQUEST_NFO()
    {
        // 오피스룸 정보 요청
        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());

        // 오피스룸에 참여중인 사용자 전체 권한정보 요청
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }


    private void S_OFFICE_GET_ROOM_INFO(PacketSession session, IMessage packet)
    {
        DEBUG.LOG("룸정보 받음 : S_OFFICE_GET_ROOM_INFO", eColorManager.REALTIME);

        officeRoomInfo = packet as S_OFFICE_GET_ROOM_INFO;

        // Popup_OfficeUserInfo 토글 이름에 인원수 표시 하기
        Popup_OfficeUserInfo popup = SceneLogic.instance.GetPopup<Popup_OfficeUserInfo>();
        if (popup != null)
        {
            popup.SetToogleText(officeRoomInfo);
        }
    }

    private async void S_OFFICE_GET_PERMISSION_ALL(PacketSession session, IMessage packet)
    {
        S_OFFICE_GET_PERMISSION_ALL message = packet as S_OFFICE_GET_PERMISSION_ALL;

        // 관리자 일 때만, 권한 변경 후 저장 버튼 활성화 시키기
        if (sceneOfficeRoom.IsAdmin())
            btn_ChangeSendData.interactable = true;
        else
            btn_ChangeSendData.interactable = false;

        // 오피스 사용자 권한정보 정렬
        List<OfficeUserInfo> sortingList = new List<OfficeUserInfo>();
        sortingList.AddRange(message.Permissions);
        sortingList.Sort((a, b) => a.Authority.CompareTo(b.Authority));

        //// 오피스룸 참여자 수와, Object 수가 다를 때, 같아질 때까지 대기
        //// 아바타 Tuhumbnail 을 그리기 위해서 대기해야 한다
        //if ( sortingList.Count > sceneOfficeRoom.networkHandler.GetObjectIds.Count )
        //{
        //    ASYNC_WAIT_ADD_OBJECTID( sortingList.Count );
        //    return;
        //}

        // 스크롤 아이템 삭제
        DestoryAllItems();
        scrollitemList.Clear();

        UserData userdata = null;
        int objectId = -1;
        Dictionary<string, int> avatarDatas = null;

        foreach (OfficeUserInfo userinfo in sortingList)
        {
            // 관전자는 별도의 탭에 모아서 보여주기로 했기 때문에, pass
            if ((OfficeAuthority)userinfo.Authority == OfficeAuthority.관전자)
                continue;

            // 1. ClientId 를 통해, Object ID 가져오기
            // 2. Object ID 를 통해, UserData 가져오기
            if (sceneOfficeRoom.networkHandler.GetObjectIds.TryGetValue(userinfo.ClientId, out objectId))
            {
                if (sceneOfficeRoom.networkHandler.UserDatas.TryGetValue(objectId, out userdata))
                {
                    avatarDatas = JsonConvert.DeserializeObject<Dictionary<string, int>>(userdata.ObjectData);

                    // 입장인원  스크롤 아이템 프리팹 생성
                    Item_OfficeUser scrollitem = Single.Resources.Instantiate<Item_OfficeUser>(
                            Cons.Path_Prefab_Item + "Item_OfficeUser",
                            sview_Content.content.transform);

                    scrollitem.SetData_UserPermission(
                            userdata, userinfo,
                            (OfficeAuthority)sceneOfficeRoom.myPermission.Authority,
                            avatarDatas);

                    scrollitem.gameObject.SetActive(true);
                    scrollitemList.Add(scrollitem);

                    if ((OfficeAuthority)userinfo.Authority == OfficeAuthority.관리자)
                    {
                        // 관리자 UI 갱신
                        Popup_OfficeUserInfo popup = sceneOfficeRoom.GetPopup<Popup_OfficeUserInfo>();
                        if (popup != null)
                            popup.UpdateUI_Admin(userinfo.Authority, userdata, avatarDatas);

                        itemMasterUser = scrollitem;
                    }
                }
            }
            else
            {
                float time = 5.0f;

                while (time >= 0.0f)
                {
                    objectId = -1;
                    time -= Time.fixedDeltaTime;

                    await UniTask.WaitUntil(() =>
                    sceneOfficeRoom.networkHandler.GetObjectIds.TryGetValue(userinfo.ClientId, out objectId));
                    break;
                }

                if (objectId != -1)
                {
                    REQUEST_NFO();
                    break;
                }
            }
        }

        CALLBACK_SEND_PERMISSION?.Invoke();
        CALLBACK_SEND_PERMISSION.RemoveListener(CallackSendPermission);
    }

    ///// <summary>
    ///// Scene_OfficeRoom 에.networkHandler.GetObjectIds 에 Add 될 때까지 대기
    ///// </summary>
    ///// <param name="permissionCount"></param>
    //private async void ASYNC_WAIT_ADD_OBJECTID(int permissionCount)
    //{
    //    while (true)
    //    {
    //        await UniTask.WaitUntil(() => sceneOfficeRoom.networkHandler.GetObjectIds.Count == permissionCount);

    //        break;
    //    }

    //    REQUEST_NFO();
    //}

    //private async void ASYNC_WAIT_OBJECTID(int permissionCount)
    //{
    //    while (true)
    //    {
    //        await UniTask.WaitUntil(() => sceneOfficeRoom.networkHandler.GetObjectIds.Count == permissionCount);

    //        break;
    //    }
    //}

    private void S_OFFICE_SET_PERMISSION(PacketSession session, IMessage packet)
    {
        S_OFFICE_SET_PERMISSION message = packet as S_OFFICE_SET_PERMISSION;
        Util.UtilOffice.OpenWarningPopup(message.Code);

        // 전체 권한 다시 받기
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }

    private void S_OFFICE_KICK(PacketSession session, IMessage packet)
    {
        S_OFFICE_KICK message = packet as S_OFFICE_KICK;
        if (message.Success == false)
        {
            DEBUG.LOG("S_OFFICE_KICK : 사용자 KICK 실패", eColorManager.REALTIME);
        }

        // 전체 권한 다시 받기
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }
    #endregion

    #region 버튼, 토글 콜백 함수
    private void OnClick_SendPermissions()
    {
        C_OFFICE_SET_PERMISSION packet = new C_OFFICE_SET_PERMISSION();
        foreach (Item_OfficeUser permissioninfo in scrollitemList)
        {
            packet.Permissions.Add(permissioninfo.permissionInfo);
        }
        Single.RealTime.Send(packet);

        CALLBACK_SEND_PERMISSION.AddListener(CallackSendPermission);

        // 오피스룸 정보 요청
        Single.RealTime.Send(new C_OFFICE_GET_ROOM_INFO());

        // 오피스룸에 참여중인 사용자 전체 권한정보 요청
        Single.RealTime.Send(new C_OFFICE_GET_PERMISSION_ALL());
    }

    private void OnValueChanged_MasterChat(bool isOn)
    {
        foreach (Item_OfficeUser permissioninfo in scrollitemList)
        {
            permissioninfo.SetPermissionToggleState(eOfficePermissionMaster.CHAT, isOn);
        }
    }

    private void OnValueChanged_MasterVoice(bool isOn)
    {
        foreach (Item_OfficeUser permissioninfo in scrollitemList)
        {
            permissioninfo.SetPermissionToggleState(eOfficePermissionMaster.VOICE, isOn);
        }
    }

    private void OnValueChanged_MasterVideo(bool isOn)
    {
        foreach (Item_OfficeUser permissioninfo in scrollitemList)
        {
            permissioninfo.SetPermissionToggleState(eOfficePermissionMaster.VIDEO, isOn);
        }
    }
    #endregion

    #region 유틸함수
    public void SetPermissionToggleState_All(OfficeUserInfo userinfo, bool isOn)
    {
        foreach (Item_OfficeUser scrollitem in scrollitemList)
        {
            if (userinfo.ClientId == scrollitem.permissionInfo.ClientId)
                continue;

            scrollitem.SetPermissionToggleState(eOfficePermissionMaster.SCREEN, false);
        }
    }

    public void DestoryAllItems()
    {
        int count = sview_Content.content.transform.childCount;
        for (int i = 0; i < count; ++i)
        {
            Transform child = sview_Content.content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }
    #endregion
}
