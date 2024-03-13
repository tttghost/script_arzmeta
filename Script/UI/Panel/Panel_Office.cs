using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using TMPro;
using System;

public class Panel_Office : PanelBase, INetworkRoom
{
	#region Members

	public RoomType roomType = RoomType.Meeting;
	[HideInInspector] public bool isDim = false;

	TMP_Text txtmp_Title;
	TMP_Text txtmp_RoomEnter;
	TMP_Text txtmp_ReservationInfo;
	TMP_Text txtmp_OfficeCreate;
	TMP_Text txtmp_Grade;
	TMP_Text txtmp_Upgrade;

	Toggle tog_EnterRoom;
	Toggle tog_CreateRoom;
	Toggle tog_Reservation;

	Button btn_Close;
	Button btn_Upgrade;

	OfficeRoomInfoRes roomInfo;
	OfficeRoomReservationInfo reservationInfo;

	public CoroutineHandle handle_Refresh;

	#endregion


	#region Initialize

	protected override void OnEnable()
	{
		base.OnEnable();

		ChangeView(nameof(View_Office_EnterRoom), Cons.MASTER_OFFICEROOM_ENTER);

		// 카메라, 마이크 하드웨어 접근 권한요청 하기
		Util.UtilOffice.RequestOfficePermission();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		Timing.KillCoroutines(handle_Refresh);
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		if (Util.UtilOffice.IsOffice())
		{
			Util.UtilOffice.DestroyIfNotOffice(this.gameObject);
			return;
		}

		btn_Close = GetUI_Button(nameof(btn_Close), () => PopPanel());
		btn_Upgrade = GetUI_Button(nameof(btn_Upgrade), null);

		tog_EnterRoom = GetUI_Toggle(nameof(tog_EnterRoom), () => ChangeView(nameof(View_Office_EnterRoom), Cons.MASTER_OFFICEROOM_ENTER));
		tog_CreateRoom = GetUI_Toggle(nameof(tog_CreateRoom), () => ChangeView(nameof(View_Office_CreateRoom), Cons.MASTER_OFFICEROOM_TITLE_SET));
		tog_Reservation = GetUI_Toggle(nameof(tog_Reservation), () => ChangeView(nameof(View_Office_Reservation), Cons.MASTER_OFFICE_RESERVATION));

		txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
		txtmp_RoomEnter = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomEnter), new MasterLocalData(Cons.MASTER_OFFICEROOM_ENTER));
		txtmp_OfficeCreate = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeCreate), new MasterLocalData(Cons.MASTER_OFFICEROOM_TITLE_SET));
		txtmp_ReservationInfo = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ReservationInfo), new MasterLocalData(Cons.MASTER_OFFICE_RESERVATION));
		txtmp_Grade = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Grade), new MasterLocalData("office_mygrade", "일반"));
		txtmp_Upgrade = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Upgrade), new MasterLocalData("office_upgrade"));

		roomType = RoomType.Meeting;
	}

	protected override IEnumerator<float> Co_SetCloseEndAct()
	{
		yield return Timing.WaitForOneFrame;

		ChangeView(nameof(View_Office_EnterRoom), Cons.MASTER_OFFICEROOM_ENTER);
	}

	public void ChangeView(string _viewName, string _masterData)
	{
		switch(_viewName)
		{
			case nameof(View_Office_EnterRoom):
				tog_EnterRoom.isOn = true;
				break;
			case nameof(View_Office_CreateRoom):
				tog_CreateRoom.isOn = true;
				break;
			case nameof(View_Office_Reservation):
				tog_Reservation.isOn = true;
				break;
		}

		ChangeView(_viewName);

		Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData(_masterData));
	}

	public void RefreshUpdate()
	{
		isDim = false;

		RefreshList();

		handle_Refresh = Timing.RunCoroutine(Co_RefreshList(60f));
	}

#endregion




#region Core Methods

	public void CreateRoom()
	{
		throw new System.NotImplementedException();
	}




	public void JoinRoom<T>(T _roomInfo)
	{
		LocalContentsData.isOfficeEnter = false;
		// LocalContentsData.isOfficeEnter = true; // 6월 8일이후

		var roomInfo = _roomInfo as OfficeRoomInfoRes;

		this.roomInfo = roomInfo;

		this.reservationInfo = null;

		Single.RealTime.roomType.target = this.roomType;

		//룸코드가 없다 == 내가 만든다->크리에이트단에서 넣는다, 룸코드가있다=입장이다
		if(roomInfo.roomCode != null)
        {
			LocalContentsData.roomName = roomInfo.roomName;
		}

		Single.RealTime.JoinRoom(roomInfo);
	}





	public void CreateOrJoin<T>(T _roomData)
	{
		var roomData = _roomData as OfficeRoomData;

		LocalContentsData.roomCode = roomData.roomCode;

		Single.RealTime.SetRoom(roomData);

		Single.RealTime.EnterRoom(this.roomType);
	}




	public void CreateAndJoin<T>(T _roomData)
	{
		var roomData = _roomData as OfficeRoomData;

		RealtimeWebManager.CreateRoom(roomData);

		LocalContentsData.roomName = roomData.roomName;
		LocalContentsData.roomCode = roomData.roomCode;

		RealtimeWebManager.Run<OfficeRoomInfoRes>(JoinRoom);
	}




	public void SearchAndJoin(string _roomCode)
	{
		Single.Web.office.GetRequest_OfficeRoomInfo(_roomCode, (res) => SearchRoom(res), (res) => Single.Scene.SetDimOff());
	}

	public void SearchRoom(OfficeRoomReservationWebInfo _roomInfo)
	{
		var popup = GetPopup<Popup_OfficeRoomInfo>();

		popup.CheckIsRealtime(_roomInfo);

		popup.callback_setCardInfo = () =>
		{
			PushPopup<Popup_OfficeRoomInfo>();

			popup.callback_setCardInfo = null;
		};
	}




	public void JoinIfExist(OfficeRoomInfoRes _roomInfo)
	{
		var roomType = _roomInfo.modeType == 1 ? RoomType.Meeting : RoomType.Lecture;

		RealtimeWebManager.SetQuery(roomType);

		RealtimeWebManager.AddQuery(Query.roomCode, _roomInfo.roomCode);

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<OfficeRoomInfoRes[]>(JoinExistRoom);
	}

	private void JoinExistRoom(OfficeRoomInfoRes[] _roomInfos)
	{
		var roomInfo = _roomInfos.Length > 0 ? _roomInfos[0] : null;

		if (roomInfo != null) JoinRoom(roomInfo);

		else
		{
			SceneLogic.instance.PopPopup();
			SceneLogic.instance.isUILock = false;
			PushPopup<Popup_Basic>()
				.ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.Confirm, null, new MasterLocalData("1161"))
			);
		}
	}




	public void JoinReservation(OfficeRoomReservationInfo _reservationInfo)
	{
		roomInfo = null;

		reservationInfo = _reservationInfo;

		var roomType = _reservationInfo.info.modeType  == 1 ? RoomType.Meeting : RoomType.Lecture;

		RealtimeWebManager.SetQuery(roomType);

		RealtimeWebManager.AddQuery(Query.roomCode, _reservationInfo.info.roomCode);

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<OfficeRoomInfoRes[]>(JoinReservationRoom);
	}

	private void JoinReservationRoom(OfficeRoomInfoRes[] _roomInfos)
	{
		var roomInfo = _roomInfos.Length > 0 ? _roomInfos[0] : null;

		if (roomInfo != null) JoinRoom(roomInfo);

		else
		{
			SceneLogic.instance.PopPopup();

			if (reservationInfo.isMine)
			{
				OfficeRoomData data = MakeRoomData(reservationInfo);

				var panel = GetPanel<Panel_Office>();

				CreateAndJoin(data);
			}

			else
			{
				// 입장 불가 팝업
				//SceneLogic.instance.GetPopupBasic<Popup_Basic>()
				//	.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData(("office_error_beforestart"))))
				//	.PushPopup();

				StackPanel<Panel_OfficeWaitRoom>(false, true, true);
				
				var panel = GetPanel<Panel_OfficeWaitRoom>();

				panel.SetInfo(reservationInfo);

				panel.transform.SetAsLastSibling();

				panel.Show(true);
			}

			reservationInfo = null;
		}
	}




	public void SuperJoin(Dictionary<string, string> _roomInfo)
	{
		var roomInfo = new OfficeRoomInfoRes();
		roomInfo.roomId = _roomInfo["roomid"];
		roomInfo.roomType = Util.UtilOffice.GetOfficeRoomType(_roomInfo["topictype"]).ToString();
		roomInfo.sceneName = RealtimeUtils.GetOfficeSceneName(_roomInfo["spaceinfoid"]);

		var roomData = new OfficeRoomData();
		roomData.roomType = roomType.ToString();
		roomData.password = _roomInfo["password"];

		Single.RealTime.SetRoom(roomData);

		JoinRoom(roomInfo);
	}




	public IEnumerator<float> Co_RefreshList(float _refreshRate)
	{
		while (this.enabled)
		{
			yield return Timing.WaitForSeconds(_refreshRate);

			isDim = false;

			RefreshList();
		}
	}

	public void RefreshList()
	{
		RealtimeWebManager.SetQuery(this.roomType);

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<OfficeRoomInfoRes[]>(GetRoomList);
	}

	private void GetRoomList(OfficeRoomInfoRes[] _roomInfos) => Util.RunCoroutine(Co_GetRoomList(_roomInfos));

	private IEnumerator<float> Co_GetRoomList(OfficeRoomInfoRes[] _roomInfos)
	{
		if (_roomInfos.Length <= 0)
		{
			var popup = GetPopup<Popup_OfficeRoomInfo>();
			
			if(popup.IsReservationRoom())
			{
				if (popup.gameObject.activeSelf)
				{
					SceneLogic.instance.PopPopup();
				}
			}
		}

		if (isDim) { Single.Scene.SetDimOn(); yield return Timing.WaitForSeconds(.5f); }

		var view = GetView<View_Office_EnterRoom>().GetView<View_Office_RoomList>();

		view.Refresh(_roomInfos);

		if (Single.Scene.isDim) { Single.Scene.SetDimOff(); isDim = false; }
	}

#endregion

#region Utils

	public void WaitRoom()
	{
		StackPanel<Panel_OfficeWaitRoom>(false, true, true);

		var panel = GetPanel<Panel_OfficeWaitRoom>();

		panel.SetInfo(roomInfo);

		panel.RefreshWaitRoomUI();

		panel.transform.SetAsLastSibling();

		panel.Show(true);
	}

	public OfficeRoomData MakeRoomData(OfficeRoomReservationInfo _reservationInfo)
	{
		var data = new OfficeRoomData();

		data.roomName = _reservationInfo.info.roomName;
		data.roomCode = _reservationInfo.info.roomCode;
		data.description = _reservationInfo.info.description;
		data.spaceInfoId = _reservationInfo.info.spaceInfoId.ToString();

		if (_reservationInfo.info.thumbnail == null)
		{
			_reservationInfo.info.thumbnail = string.Empty;
		}

		if(_reservationInfo.info.password == null)
		{
			_reservationInfo.info.password = string.Empty;
		}

		data.thumbnail = _reservationInfo.info.thumbnail;

		data.modeType = (int)_reservationInfo.info.modeType;
		data.topicType = _reservationInfo.info.topicType;

		data.password = _reservationInfo.info.password;
		data.personnel = _reservationInfo.info.personnel;

		data.observer = _reservationInfo.info.observer;
		data.isWaitingRoom = Convert.ToBoolean(_reservationInfo.info.isWaitingRoom);
		data.isAdvertising = Convert.ToBoolean(_reservationInfo.info.isAdvertising);
		data.runningTime = _reservationInfo.info.runningTime;

		data.creatorId = RealtimeUtils.GetMemberCode();
		data.roomType = Util.UtilOffice.GetRoomType(data.modeType).ToString();
		data.sceneName = Util.UtilOffice.GetSceneName(data.spaceInfoId);

		return data;
	}

	#endregion
}
