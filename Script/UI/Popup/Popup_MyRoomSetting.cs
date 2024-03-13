/* 
 * 마이룸 유저리스트 보기(강퇴, 룸상태변경)
 */
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Newtonsoft.Json;
using Protocol;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Protocol.S_BASE_ADD_OBJECT.Types;
public class Popup_MyRoomSetting : PopupBase
{
    #region 변수

    //기본 정보
    //public enum MYROOMSETTING_TYPE
    //{
    //    KICK,
    //    SHUTDOWN,
    //}
    //private MYROOMSETTING_TYPE  _mYROOMSETTING_TYPE;
    //public MYROOMSETTING_TYPE   mYROOMSETTING_TYPE
    //{
    //    get => _mYROOMSETTING_TYPE;
    //    set
    //    {
    //        _mYROOMSETTING_TYPE = value;

    //        go_KickGroup.SetActive(false);
    //        go_ShutdownGroup.SetActive(false);
    //        switch (mYROOMSETTING_TYPE)
    //        {
    //            case MYROOMSETTING_TYPE.KICK:
    //                go_KickGroup.SetActive(true);
    //                break;
    //            case MYROOMSETTING_TYPE.SHUTDOWN:
    //                go_ShutdownGroup.SetActive(true);
    //                break;
    //        }
    //    }
    //}

    private string              OwnerId;

    //기본 그룹
    private Button              btn_Back;
    private Image               img_OwnerThumbnail;
    private Button              btn_OwnerProfile;
    //private TMP_Text            txtmp_VisitMode;
    private TMP_Text            txtmp_OwnerNickname;
    private Button              btn_DynamicLink;
    
    //킥 그룹
    private GameObject          go_KickGroup;
    private TMP_Text            txtmp_UserCount;
    private ScrollRect          scrollview_FiniteVer;

    //킥 그룹 - 오너 그룹
    //private GameObject          go_OwnerGroup;
    private Button              btn_Editmode;
    //private Button              btn_Setting;
    private TMP_Text            txtmp_KickAll;
    private Button              btn_KickAll; 

    //셧다운 그룹
    //private GameObject          go_ShutdownGroup;
    //private Button              btn_Prev;
    //private TMP_Text            txtmp_RoomSetting;
    //private TMP_Text            txtmp_VisitType;
    private TMP_Dropdown        dropdown_VisitType; //방문객이면 인터렉터블 false

    //유저 리스트 관련

    private List<Item_MyRoomUser> item_MyRoomUsers = new List<Item_MyRoomUser>(); //유저 정보 가지고있기
    private ShareLinkInfo shareLinkInfo = new ShareLinkInfo();
    private int                 _userCount;
    public int                  userCount
    {
        get => _userCount;
        set
        {
            _userCount = value;
            if (txtmp_UserCount != null)
            {
                Util.SetMasterLocalizing(txtmp_UserCount, new MasterLocalData("myroom_visitor_list", _userCount, "20"));
            }
        }
    }
    #endregion

    #region 함수

    #region 캐싱
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        go_KickGroup =          GetChildGObject(nameof(go_KickGroup));
        //go_ShutdownGroup =      GetChildGObject(nameof(go_ShutdownGroup));
        //go_OwnerGroup =         GetChildGObject(nameof(go_OwnerGroup));
        //go_OwnerGroup.SetActive(LocalPlayerData.Method.IsMyRoom);

        img_OwnerThumbnail =    GetUI_Img(nameof(img_OwnerThumbnail));
        txtmp_OwnerNickname =   GetUI_TxtmpMasterLocalizing(nameof(txtmp_OwnerNickname));
        //txtmp_VisitMode =       GetUI_TxtmpMasterLocalizing(nameof(txtmp_VisitMode));
        //txtmp_RoomSetting =     GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomSetting), new MasterLocalData("myroom_room_setting"));
        //txtmp_VisitType =       GetUI_TxtmpMasterLocalizing(nameof(txtmp_VisitType), new MasterLocalData("myroom_setting_visit"));
        btn_OwnerProfile =      GetUI_Button(nameof(btn_OwnerProfile), OnClick_OwnerProfile);
        btn_DynamicLink =       GetUI_Button(nameof(btn_DynamicLink), OnClick_DynamicLink);

        btn_Editmode =          GetUI_Button(nameof(btn_Editmode), OnClick_Editmode);
        //btn_Setting =           GetUI_Button(nameof(btn_Setting), () => mYROOMSETTING_TYPE = MYROOMSETTING_TYPE.SHUTDOWN);
        btn_Back =              GetUI_Button(nameof(btn_Back), Back);
        //btn_Prev =              GetUI_Button(nameof(btn_Prev), () => mYROOMSETTING_TYPE = MYROOMSETTING_TYPE.KICK);

        btn_KickAll =           GetUI_Button(nameof(btn_KickAll), OnClick_KickAll);
        txtmp_KickAll =         GetUI_TxtmpMasterLocalizing(nameof(txtmp_KickAll), new MasterLocalData("myroom_kick_all"));

        txtmp_UserCount =       GetUI_TxtmpMasterLocalizing(nameof(txtmp_UserCount));
        scrollview_FiniteVer =  GetUI<ScrollRect>(nameof(scrollview_FiniteVer));

        dropdown_VisitType =    GetUI_TMPDropdown(nameof(dropdown_VisitType), OnValueChanged_VisitType, new List<MasterLocalData>
        {
            new MasterLocalData("myroom_condition_anyone") ,
            new MasterLocalData("myroom_condition_nobody") ,
        });

        bool isMyRoom = LocalPlayerData.Method.IsMyRoom;
        btn_Editmode.gameObject.SetActive(isMyRoom);
        btn_KickAll.gameObject.SetActive(isMyRoom);
        dropdown_VisitType.interactable = isMyRoom;

        userCount = 0;

    }

    /// <summary>
    /// 다이나믹링크
    /// </summary>
    private void OnClick_DynamicLink()
    {
        shareLinkInfo.roomType = SHARELINK_TYPE.MYROOM_ENTER;
        shareLinkInfo.roomName = LocalPlayerData.Method.roomOwnerName;
        shareLinkInfo.roomCode = LocalPlayerData.Method.roomCode;
        shareLinkInfo.roomId = LocalContentsData.roomId;

        CreateShareLink.CreateLink(shareLinkInfo);
    }
    #endregion

    #region 핸들러 추가/제거
    protected override void OnEnable()
    {
        base.OnEnable();

        AddHandler();

        List<UserData> UserDatas = SceneLogic.instance.networkHandler.UserDatas.values;
        AddObject(UserDatas);

        //mYROOMSETTING_TYPE = MYROOMSETTING_TYPE.KICK;
    }
    private void AddHandler()
    {
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_ADD_OBJECT, this, S_BASE_ADD_OBJECT);
        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_BASE_REMOVE_OBJECT, this, S_BASE_REMOVE_OBJECT);

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_SHUTDOWN, this, S_MYROOM_SHUTDOWN);

        RealtimeUtils.AddHandler(RealtimePacket.MsgId.PKT_S_MYROOM_GET_ROOMINFO, this, S_MYROOM_GET_ROOMINFO);
        C_MYROOM_GET_ROOMINFO();

    }
    protected override void OnDisable()
    {
        base.OnDisable();
        RemoveHandler();
    }
    private void RemoveHandler()
    {
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_ADD_OBJECT, this);
        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_BASE_REMOVE_OBJECT, this);

        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_SHUTDOWN, this);

        RealtimeUtils.RemoveHandler(RealtimePacket.MsgId.PKT_S_MYROOM_GET_ROOMINFO, this);
    }
    #endregion

    #region 아이템 추가
    private void S_BASE_ADD_OBJECT(PacketSession arg1, IMessage arg2)
    {

        S_BASE_ADD_OBJECT packet = arg2 as S_BASE_ADD_OBJECT;

        //가공 혼합
        List<UserData> UserDatas = GetUserData(packet.GameObjects);

        //실제 겜오브젝트생성 데이터 셋업
        AddObject(UserDatas);
    }

    /// <summary>
    /// 실시간서버 데이터를 가공(GameObjectInfos -> UserDatas)
    /// </summary>
    /// <param name="GameObjectInfos"></param>
    /// <returns></returns>
    private List<UserData> GetUserData(RepeatedField<GameObjectInfo> GameObjectInfos)
    {
        List<UserData> UserDatas = new List<UserData>();
        for (int i = 0; i < GameObjectInfos.Count; i++)
        {
            var GameObjectInfo = GameObjectInfos[i];
            UserData userData = new UserData(GameObjectInfo, SceneLogic.instance.networkHandler.Clients[GameObjectInfo.OwnerId]);
            UserDatas.Add(userData);
        }
        return UserDatas;
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    /// <param name="UserDatas"></param>
    private void AddObject(List<UserData> UserDatas)
    {
        for (int i = 0; i < UserDatas.Count; i++)
        {
            UserData UserData = UserDatas[i];

            if (item_MyRoomUsers.SingleOrDefault(x => x.UserData.ObjectId == UserData.ObjectId))
            {
                continue;
            }

            Item_MyRoomUser Item_MyRoomUser = Single.Resources.Instantiate<Item_MyRoomUser>(Cons.Path_Prefab_Item + nameof(Item_MyRoomUser), scrollview_FiniteVer.content.transform);
            Item_MyRoomUser.SetData(UserData);
            item_MyRoomUsers.Add(Item_MyRoomUser);

            userCount++;
        }
    }
    #endregion

    #region 아이템 제거
    private void S_BASE_REMOVE_OBJECT(PacketSession arg1, IMessage arg2)
    {
        S_BASE_REMOVE_OBJECT packet = arg2 as S_BASE_REMOVE_OBJECT;

        RemoveObject(packet.GameObjects.ToList());
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    /// <param name="ObjectIds"></param>
    private void RemoveObject(List<int> ObjectIds)
    {
        for (int i = 0; i < ObjectIds.Count; i++)
        {
            int ObjectId = ObjectIds[i];
            Item_MyRoomUser item_MyRoomUser = item_MyRoomUsers.SingleOrDefault(x => x.UserData.ObjectId == ObjectId);
            if (item_MyRoomUser)
            {
                Destroy(item_MyRoomUsers[ObjectId].gameObject);
                item_MyRoomUsers.Remove(item_MyRoomUser);
                userCount--;
            }
        }
    }
    #endregion

    #region 강퇴

    /// <summary>
    /// Web - 방문타입 변경
    /// </summary>
    /// <param name="arg0"></param>
    private void OnValueChanged_VisitType(int arg0)
    {
        //추후 이넘으로 변경
        if (arg0 == 0) //모두가능
        {
            Single.Web.myRoom.MyRoomChangeState(1, OnMyRoomChangeState);
        }
        else if (arg0 == 1) //비활성화
        {
            Single.Web.myRoom.MyRoomChangeState(4, OnMyRoomChangeState);
        }
    }

    /// <summary>
    /// Web - 방문타입 변경 콜백
    /// </summary>
    /// <param name="obj"></param>
    private void OnMyRoomChangeState(MyRoomState obj)
    {
        if ((WEBERROR)obj.error == WEBERROR.NET_E_SUCCESS)
        {
            bool isShutDown = default;
            switch ((MYROOMSTATE_TYPE)obj.myRoomStateType)
            {
                case MYROOMSTATE_TYPE.myroom_condition_anyone:
                    isShutDown = false;
                    break;
                case MYROOMSTATE_TYPE.myroom_condition_friend:
                case MYROOMSTATE_TYPE.myroom_condition_invite:
                case MYROOMSTATE_TYPE.myroom_condition_nobody:
                    isShutDown = true;
                    break;
            }
            MyRoomManager.Instance.myroomModuleHandler.C_MYROOM_SHUTDOWN(isShutDown);
        }
    }

    /// <summary>
    /// Realtime - 방문타입 변경 콜백
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void S_MYROOM_SHUTDOWN(PacketSession arg1, IMessage arg2)
    {
        S_MYROOM_SHUTDOWN packet = arg2 as S_MYROOM_SHUTDOWN;
        SetVisitType(packet.IsShutdown);
    }

    #endregion

    #region 오너 정보

    /// <summary>
    /// 현재 룸 정보 호출
    /// </summary>
    private void C_MYROOM_GET_ROOMINFO()
    {
        C_MYROOM_GET_ROOMINFO packet = new C_MYROOM_GET_ROOMINFO();

        Single.RealTime.Send(packet);
    }

    /// <summary>
    /// 현재 룸 정보 콜백
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void S_MYROOM_GET_ROOMINFO(PacketSession arg1, IMessage arg2)
    {
        S_MYROOM_GET_ROOMINFO packet = arg2 as S_MYROOM_GET_ROOMINFO;

        //오너 아이디
        OwnerId = packet.OwnerId;

        //오너 썸네일
        var avatarData = JsonConvert.DeserializeObject<Dictionary<string, int>>(packet.OwnerAvatarInfo);
        LocalPlayerData.Method.GetAvatarSprite(packet.OwnerId, avatarData, (sprite) => img_OwnerThumbnail.sprite = sprite);

        //오너 닉네임
        LocalPlayerData.Method.roomOwnerName = packet.OwnerNickname;
        Util.SetMasterLocalizing(txtmp_OwnerNickname, new MasterLocalData("myroom_title", LocalPlayerData.Method.roomOwnerName));

        SetVisitType(packet.IsShutdown);
    }

    /// <summary>
    /// 방문타입 변경
    /// </summary>
    /// <param name="IsShutdown"></param>
    private void SetVisitType(bool IsShutdown)
    {
        //오너 룸 상태
        string visitType;
        if (IsShutdown)
        {
            visitType = "myroom_condition_nobody";
        }
        else
        {
            visitType = "myroom_condition_anyone";
        }
        //Util.SetMasterLocalizing(txtmp_VisitMode, new MasterLocalData(visitType));
        Util.SetMasterLocalizing(dropdown_VisitType.captionText, new MasterLocalData(visitType));
        dropdown_VisitType.SetValueWithoutNotify(IsShutdown ? 1 : 0);

        // 룸 상태 변경 시 로컬데이터 즉시 저장 필요 - 20230816 한효주 추가
        // 차후 변경 필요
        LocalPlayerData.MyRoomStateType = IsShutdown ? 4 : 1;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            dropdown_VisitType.SetValueWithoutNotify(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            dropdown_VisitType.SetValueWithoutNotify(1);
        }
    }
    #endregion

    #region OnClick
    /// <summary>
    /// 아즈 프로필 보기
    /// </summary>
    private void OnClick_OwnerProfile()
    {
        PushPanel<Panel_ArzProfile>().SetPlayerInfo(OTHERINFO_TYPE.MEMBERCODE, OwnerId);
        SceneLogic.instance.isUILock = false;
        PopPopup();
    }

    /// <summary>
    /// 에디터모드 진입
    /// </summary>
    private void OnClick_Editmode()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("myroom_confirm_edit")))
            .ChainPopupAction(new PopupAction(() =>
            {
                SceneLogic.instance.isUILock = false;
                PopPopup();
                Scene.MyRoom.MyRoomModeChange(eMyRoomMode.EDITMODE);
                InteractionCustomManager.Instance.handlerMyRoomModeChange?.Invoke(eMyRoomMode.EDITMODE);
            }));
    }

    /// <summary>
    /// 모두 강퇴하기
    /// </summary>
    private void OnClick_KickAll()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, masterLocalDesc: new MasterLocalData("myroom_confirm_kick_all")))
            .ChainPopupAction(new PopupAction(() =>
            {
                foreach (var item_MyRoomUser in item_MyRoomUsers)
                {
                    if (item_MyRoomUser.UserData.OwnerId == LocalPlayerData.MemberCode)
                    {
                        continue;
                    }
                    MyRoomManager.Instance.myroomModuleHandler.C_MYROOM_KICK(item_MyRoomUser.UserData.OwnerId);
                }
            }));
    }
    #endregion

    #endregion

    public delegate void UserDataHandler(UserData userData);
    public UserDataHandler userDataHandler;

}
