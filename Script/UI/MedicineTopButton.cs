using FrameWork.Network;
using FrameWork.UI;
using Google.Protobuf;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MedicineTopButton : MonoBehaviour
{
    private void Start()
    {
        //Button btn_UserInfo = Util.Search<Button>(gameObject, nameof(btn_UserInfo));
        Button btn_RoomInfo = Util.Search<Button>(gameObject, nameof(btn_RoomInfo));
        Button btn_ExitRoom = Util.Search<Button>(gameObject, nameof(btn_ExitRoom));
        //btn_UserInfo.onClick.AddListener(() => SceneLogic.instance.PushPopup<Popup_OfficeUserInfo>());
        btn_RoomInfo.onClick.AddListener(() => SceneLogic.instance.PushPopup<Popup_OfficeRoomSave>());
        btn_ExitRoom.onClick.AddListener(() => SceneLogic.instance.Back());
    }

    //#region 변수

    //private Button btn_UserInfo;
    //private Button btn_RoomInfo;
    //private Button btn_ExitRoom;

    //private Scene_OfficeRoom medicineScene;
    ////private Scene_Medicine medicineScene;

    //#endregion


    //#region 초기화 

    //void Start()
    //{
    //    medicineScene = (Scene_OfficeRoom)SceneLogic.instance;
    //    //medicineScene = (Scene_Medicine)SceneLogic.instance;

    //    btn_UserInfo = Util.Search<Button>(gameObject, "btn_UserInfo");
    //    btn_RoomInfo = Util.Search<Button>(gameObject, "btn_RoomInfo");
    //    btn_ExitRoom = Util.Search<Button>(gameObject, "btn_ExitRoom");

    //    AddListener();

    //    RealtimeUtils.AddHandler(eSessionType.CONSULTING, PacketManager.MsgId.PKT_S_MEETING_CLOSING, this, S_MeetingClosing); // 방장이 방을 해산해서 나가졌을 때,, 
    //    RealtimeUtils.AddHandler(eSessionType.CONSULTING, PacketManager.MsgId.PKT_S_MEETING_BREAK_NOTICE, this, S_BreakNotice);
    //}
    //private void AddListener()
    //{
    //    var logic = FindObjectOfType<Scene_Medicine>();

    //    if (logic) logic.BackButtonEvent.AddListener(OnClickExitRoom);

    //    btn_UserInfo.onClick.AddListener(OnClickUserInfo);
    //    btn_RoomInfo.onClick.AddListener(OnClickRoomInfo);
    //    btn_ExitRoom.onClick.AddListener(OnClickExitRoom);
    //}

    //private void OnDestroy()
    //{
    //    if (!Single.RealTime) return;

    //    RealtimeUtils.RemoveHandler(eSessionType.CONSULTING, PacketManager.MsgId.PKT_S_MEETING_CLOSING, this);
    //    RealtimeUtils.RemoveHandler(eSessionType.CONSULTING, PacketManager.MsgId.PKT_S_MEETING_BREAK_NOTICE, this);

    //}

    //#endregion


    //#region 핸들러 
    ///// <summary>
    ///// 회의실 방이 Closing 되었을 때 받는 패킷 핸들러
    ///// 
    ///// 1. 방이 해산되었을 때 회의실에 있는 모든 클라이언트들한테 보냄
    ///// 2. 게스트가 나가기 버튼을 눌렀을 때 
    ///// </summary>
    ///// <param name="session"></param>
    ///// <param name="packet"></param>
    //private void S_MeetingClosing(PacketSession session, IMessage packet)
    //{

    //}

    //void S_BreakNotice(PacketSession session, IMessage packet)
    //{
    //    // S_MEETING_BREAK_NOTICE breakNotice = packet as S_MEETING_BREAK_NOTICE;
    //    Util.RunCoroutine(BreakNoticeLeave());
    //}
    //#endregion


    //#region 버튼 

    //private void OnClickUserInfo()
    //{
    //    //SceneLogic.instance.PushPopup("Popup_MedicineUserInfo", new PopupData(POPUPICON.NONE, string.Empty, string.Empty, BTN_TYPE.OK));
    //}

    //private void OnClickRoomInfo()
    //{
    //    //SceneLogic.instance.PushPopup("Popup_MedicineRoomInfo");
    //    SceneLogic.instance.PushPopup<Popup_OfficeRoomSave>();
    //}
    //private void OnClickExitRoom()
    //{
    //    string descriptString = string.Empty;
    //    string confirmString = string.Empty;
    //    UnityAction confirmAction = null;

    //    descriptString = "1163"; // 방을 나가시겠습니까?
    //    confirmString = "1091"; // 나가기
    //    confirmAction += LeavedRoom;

    //    //if (medicineScene.AmIHost())
    //    //{
    //    //    // confirmString = "11040"; // 방장 이관하기
    //    //    // confirmAction += HostAutoTransfer;
    //    //    descriptString = "1150"; // 방이 해산되지 않길 원하시는 경우, 설정에서 유저 하나를 이관해주세요. 정말로 방을 해산할까요?
    //    //    confirmString = "1151"; // 방 해산 및 나가기
    //    //    confirmAction += medicineScene.medicineModuleHandler.SendDisMiss;
    //    //}
    //    //else
    //    //{
    //    //    descriptString = "1163"; // 방을 나가시겠습니까?
    //    //    confirmString = "1091"; // 나가기
    //    //    confirmAction += LeavedRoom;
    //    //}

    //    SceneLogic.instance.PushPopupBasic(Cons.Popup_Basic,
    //        new PopupData(POPUPICON.NONE, string.Empty, new LocalData(Cons.Local_Arzmeta, descriptString), BTN_TYPE.ConfirmCancel,
    //            new LocalData(Cons.Local_Arzmeta, confirmString), new LocalData(Cons.Local_Arzmeta, "002")/*취소*/),
    //        new PopupAction(confirmAction, null));
    //}

    //// TODO : 나중에 이관하기 기능 로직 정해지면 구현 
    //private void HostAutoTransfer()
    //{

    //}

    ///// <summary>
    ///// 게스트들은 방 나가기
    ///// </summary>
    //private void LeavedRoom()
    //{
    //    Single.RealTime.JoinStackedServer();
    //}



    //IEnumerator<float> BreakNoticeLeave()
    //{
    //    if (medicineScene.AmIHost())
    //    {
    //        LeavedRoom();
    //        yield break;
    //    }

    //    bool isClick = false;

    //    yield return Timing.WaitForSeconds(.5f);

    //    SceneLogic.instance.PushPopupBasic(Cons.Popup_Basic, new PopupData(POPUPICON.NONE,
    //            new LocalData(Cons.Local_Arzmeta, "1161"),
    //            new LocalData(Cons.Local_Arzmeta, "1162"), BTN_TYPE.Confirm
    //            , new LocalData(Cons.Local_Arzmeta, "30033")),
    //        new PopupAction(null, () =>
    //        {
    //            isClick = true;
    //            LeavedRoom();
    //        }));

    //    yield return Timing.WaitForSeconds(10f);

    //    if (!isClick)
    //    {
    //        LeavedRoom();
    //    }

    //}
    //#endregion


}
