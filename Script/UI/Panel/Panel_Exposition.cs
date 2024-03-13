using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using TMPro;
using System;
using FrameWork.Socket;
using System.Linq;

public enum ExpositionPosition
{
    None,
    License,
    Participant,
}

public class Panel_Exposition : PanelBase, INetworkRoom
{
    #region Members

    public ExpositionPosition expositionPosition = ExpositionPosition.License;

    TMP_Text txtmp_Title;

    Toggle tog_EnterRoom;
    Toggle tog_CreateRoom;

    TMP_Text txtmp_RoomEnter;
    TMP_Text txtmp_OfficeCreate;

    Button btn_Close;

    #endregion



    #region Initialize

    protected override void OnEnable()
    {
        base.OnEnable();

        RefreshToggleMenu();
    }

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_RoomEnter = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomEnter), new MasterLocalData("office_booth_enter"));
        txtmp_OfficeCreate = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeCreate), new MasterLocalData("office_booth_creation"));

        tog_EnterRoom = GetUI_Toggle(nameof(tog_EnterRoom), () => SelectView(nameof(View_Exposition_EnterRoom)));
        tog_CreateRoom = GetUI_Toggle(nameof(tog_CreateRoom), () => SelectView(nameof(View_Exposition_CreateRoom)));

        btn_Close = GetUI_Button(nameof(btn_Close), () => PopPanel());

        BackAction_Custom = () => PopPanel();

        SetExpositionTitle();
    }

    protected override IEnumerator<float> Co_OpenStartAct()
    {
        yield return Timing.WaitForOneFrame;

        SelectView(nameof(View_Exposition_EnterRoom));

        RefreshList();
    }

    protected override IEnumerator<float> Co_SetCloseEndAct()
    {
        yield return Timing.WaitForOneFrame;

        SelectView(nameof(View_Exposition_EnterRoom));
    }

    public void SelectView(string _viewName)
    {
        switch (_viewName)
        {
            case nameof(View_Exposition_EnterRoom):
                tog_EnterRoom.isOn = true;
                break;

            case nameof(View_Exposition_CreateRoom):
                tog_CreateRoom.isOn = true;
                break;
        }

        ChangeView(_viewName);
    }

    private void RefreshToggleMenu()
    {
        if (expositionPosition != ExpositionPosition.License)
        {
            tog_CreateRoom.gameObject.SetActive(false);
        }
    }

    private void SetExpositionTitle()
    {
        if (LocalPlayerData.csafEventInfo == null
            || string.IsNullOrEmpty(LocalPlayerData.csafEventInfo.name))
        {
            GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("map_studyinkoreafair_name"));
        }
        else
        {
            GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), LocalPlayerData.csafEventInfo.name);
        }
    }

    #endregion



    #region Network Core Methods

    public void CreateRoom()
    {
        throw new System.NotImplementedException();
    }




    public void JoinRoom<T>(T _roomInfo)
    {

    }





    public void CreateOrJoin<T>(T _roomData)
    {
        var roomData = _roomData as RoomData;

        RealtimeWebManager.SetQuery(Util.String2Enum<RoomType>(roomData.roomType));

        RealtimeWebManager.AddQuery(Query.roomCode, roomData.roomCode);

        RealtimeWebManager.GetRoom();

        RealtimeWebManager.Run<ExpositionRoomInfoRes[]>(CreateOrJoin);
    }

    public void CreateOrJoin(ExpositionRoomInfoRes[] _roomInfos) => Timing.RunCoroutine(Co_CreateOrJoin(_roomInfos));

    private IEnumerator<float> Co_CreateOrJoin(ExpositionRoomInfoRes[] _roomInfos)
    {
        var roomInfo = RealtimeUtils.GetRoomInfo(_roomInfos);

        if (roomInfo != null)
        {
            LocalContentsData.roomCode = GetPopup<Popup_ExpositionRoomInfo>().booth.roomCode;

            Single.RealTime.JoinRoom(roomInfo);
        }

        else
        {
            yield return Timing.WaitUntilTrue(() => !RealtimeWebManager.IsRecieved());

            var roomCode = GetPopup<Popup_ExpositionRoomInfo>().booth.roomCode;

            var roomData = RealtimeUtils.MakeRoomData(RoomType.Exposition_Booth);

            roomData.roomCode = roomCode;

            LocalContentsData.roomCode = roomData.roomCode;

            RealtimeWebManager.CreateRoom(roomData);

            RealtimeWebManager.Run<ExpositionRoomInfoRes>(Single.RealTime.JoinRoom);
        }
    }

    public void CreateAndJoin<T>(T _roomData)
    {

    }



    public void SearchAndJoin(string _roomCode)
    {

    }



    public bool isDim = false;

    public void RefreshList()
    {
        Single.Web.CSAF.GetCSAFBooths((res) =>
        {
            var view = GetView<View_Exposition_EnterRoom>().GetView<View_Exposition_RoomList>();

            view.Refresh(res.booths);
        });
    }

    public IEnumerator<float> Co_RefreshList(float _refreshRate)
    {
        throw new NotImplementedException();
    }

    #endregion
}
