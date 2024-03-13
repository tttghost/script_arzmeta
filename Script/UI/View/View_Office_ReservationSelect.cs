using System.Collections.Generic;
using FrameWork.UI;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.Events;

public class View_Office_ReservationSelect : UIBase
{
	#region Members

	[SerializeField] List<OfficeRoomReservationInfo> reservationInfos = new List<OfficeRoomReservationInfo>();
	int index = 0;

	Transform go_Root;
	GameObject go_IsObserve;
	GameObject go_IsPassword;

	Button btn_Prev;
	Button btn_Next;
	Button btn_Close;
	Button btn_Observe;
	Button btn_Share;
	Button btn_Interest;
	Button btn_EnterRoom;
	Button btn_EditRoom;

	Image img_Thumbnail;
	Image img_OfficeType;
	Image img_Lock;
	Image img_interest;

	TMP_Text txtmp_OfficeSpaceTitle;
	TMP_Text txtmp_OfficeSpaceCount;
	TMP_Text txtmp_ModeType;
	TMP_Text txtmp_TopicType;
	TMP_Text txtmp_RoomName;
	TMP_Text txtmp_PlayerCount;
	TMP_Text txtmp_Description;
	TMP_Text txtmp_HostName;
	TMP_Text txtmp_StartDate;
	TMP_Text txtmp_StartTime;
	TMP_Text txtmp_Order;

	TMP_Text txtmp_Share;
	TMP_Text txtmp_EnterRoom;

	TMP_Text txtmp_label_host;
	TMP_Text txtmp_label_createdDate;
	TMP_Text txtmp_label_createdTime;
	TMP_Text txtmp_label_playerCount;

	int year;
	int month;
	int day;

	public OfficeRoomReservationInfo ReservationInfo { get => reservationInfos[index]; }
	public List<OfficeRoomReservationInfo> ReservationInfos { get => reservationInfos; }
    private ShareLinkInfo shareLinkInfo = new ShareLinkInfo();

    #endregion




    #region Initialize

    protected override void OnDisable()
	{
		base.OnDisable();

		for (int i = 0; i < deleteInfos.Count; i++)
		{
			ArzMetaReservationController.Instance.RemoveNormal(deleteInfos[i]);
		}

		deleteInfos.Clear();

		BackAction_Custom -= OnClick_Close;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		index = 0;

		BackAction_Custom += OnClick_Close;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		go_Root = this.transform.Search(nameof(go_Root));
		go_IsObserve = this.transform.Search(nameof(go_IsObserve)).gameObject;
		go_IsPassword = this.transform.Search(nameof(go_IsPassword)).gameObject;

		btn_Close = GetUI_Button(nameof(btn_Close), OnClick_Close);
		btn_Prev = GetUI_Button(nameof(btn_Prev), OnClick_Prev);
		btn_Next = GetUI_Button(nameof(btn_Next), OnClick_Next);
		btn_Observe = GetUI_Button(nameof(btn_Observe), OnClick_Observe);
		btn_EditRoom = GetUI_Button(nameof(btn_EditRoom), OnClick_Edit);
		btn_Share = GetUI_Button(nameof(btn_Share), OnClick_Share);
		btn_EnterRoom = GetUI_Button(nameof(btn_EnterRoom), OnClick_EnterRoom);

		img_OfficeType = GetUI_Img(nameof(img_OfficeType));
		img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));
		img_Lock = GetUI_Img(nameof(img_Lock));
		img_interest = GetUI_Img(nameof(img_interest));

		txtmp_OfficeSpaceTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceTitle));
		txtmp_OfficeSpaceCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceCount));
		txtmp_ModeType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ModeType));
		txtmp_PlayerCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerCount));
		txtmp_TopicType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_TopicType));
		txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
		txtmp_Description = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Description));
		txtmp_HostName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_HostName));
		txtmp_StartDate = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StartDate));
		txtmp_StartTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StartTime));
		txtmp_Order = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Order));

		txtmp_label_host = GetUI_TxtmpMasterLocalizing(nameof(txtmp_label_host), new MasterLocalData("office_participant_type_manager"));
		txtmp_label_createdDate = GetUI_TxtmpMasterLocalizing(nameof(txtmp_label_createdDate), new MasterLocalData("office_reservation_day"));
		txtmp_label_createdTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_label_createdTime), new MasterLocalData("office_reservation_starttime"));
		txtmp_label_playerCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_label_playerCount), new MasterLocalData("office_room_people"));

		txtmp_Share = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Share), new MasterLocalData("common_share"));
		txtmp_EnterRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterRoom), new MasterLocalData("common_enter"));

		btn_Interest = GetUI_Button(nameof(btn_Interest), OnClick_Interest);

		btn_Next.interactable = false;
		btn_Prev.interactable = false;

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
	}

	public void SetInfo(List<OfficeRoomReservationInfo> _list)
	{
		index = 0;

		_list = _list.OrderBy(x => x.info.startTime).ToList();

		reservationInfos = GetCurrentList(_list);

		RefreshCardButtonUI();

		RefreshCardUI(reservationInfos[index]);
	}

	#endregion




	#region Core Methods

	public void Modify(OfficeRoomReservationInfo _reservation) => Util.RunCoroutine(Co_Modify(_reservation));

	IEnumerator<float> Co_Modify(OfficeRoomReservationInfo _reservation)
	{
		Single.Scene.SetDimOn();

		if (_reservation.info.repeatDay != 0)
		{
			ArzMetaReservationController.Instance.Request(Define.RESERVATION);

			yield return Timing.WaitUntilTrue(() => ArzMetaReservationController.Instance.IsReceived(Define.RESERVATION));

			var reservations = ArzMetaReservationController.Instance.ReservationRepeats;

			for (int i = 0; i < reservations.Count; i++)
			{
				if (reservations[i].info.roomCode == _reservation.info.roomCode)
				{
					reservations[i] = _reservation;
					break;
				}
			}

			ArzMetaReservationController.Instance.ShowWeekCalendar();
		}

		else
		{
			if (_reservation.reservationDateTime != reservationInfos[index].reservationDateTime)
			{
				ArzMetaReservationController.Instance.UnRegister(reservationInfos[index]);

				reservationInfos[index] = _reservation;

				ArzMetaReservationController.Instance.Register(_reservation);
			}

			else reservationInfos[index] = _reservation;
		}

		RefreshCardUI(_reservation);

		reservationInfos[index] = _reservation;


		var list = ArzMetaCalendar.GetEventList(year, month, day);

		if (list.Count <= 0) Show(false);


		yield return Timing.WaitForSeconds(.5f);

		var roomCode = reservationInfos[index].info.roomCode;
		var roomName = reservationInfos[index].info.roomName;

		ArzMetaReservationController.Instance.RefreshRoomName(roomCode, roomName);

		Single.Scene.SetDimOff();
	}





	public void RefreshCardUI(OfficeRoomReservationInfo _reservationInfo)
	{
		txtmp_OfficeSpaceTitle.text = Util.UtilOffice.GetOfficeSpaceTitle(_reservationInfo);
		txtmp_OfficeSpaceCount.text = Util.UtilOffice.GetOfficeSpaceCount(reservationInfos);

		txtmp_ModeType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ModeType), new MasterLocalData(Util.UtilOffice.GetModeType(_reservationInfo.info.modeType)));
		txtmp_TopicType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_TopicType), new MasterLocalData(Util.UtilOffice.GetTopicType(_reservationInfo.info.topicType)));

		img_OfficeType.sprite = Util.UtilOffice.GetOfficeType(_reservationInfo.info.topicType);

		img_Thumbnail.sprite = Util.UtilOffice.GetSpaceThumbnail(_reservationInfo.info.spaceInfoId.ToString()).sprite;
		_reservationInfo.info.thumbnail = null;

		go_IsObserve.SetActive(_reservationInfo.info.observer > 0);

		img_Lock.sprite = Util.UtilOffice.GetLockType(_reservationInfo.info.isPassword);

		go_IsObserve.SetActive(_reservationInfo.info.observer > 0);
		btn_Observe.gameObject.SetActive(_reservationInfo.info.observer > 0 && !_reservationInfo.isMine);

		btn_EditRoom.gameObject.SetActive(_reservationInfo.isMine);

		btn_Interest.gameObject.SetActive(!_reservationInfo.isMine);
		btn_Interest.transform.Search("img_interest").GetComponent<Image>().sprite = Util.UtilOffice.GetInterest(_reservationInfo.info.roomCode);

		txtmp_RoomName.text = _reservationInfo.info.roomName;
		txtmp_Description.text = _reservationInfo.info.description;
		txtmp_HostName.text = _reservationInfo.info.nickName;

		txtmp_StartDate.text = _reservationInfo.info.repeatDay != 0 ?
			Util.UtilOffice.GetStartDate(_reservationInfo.info.repeatDay, _reservationInfo.reservationDateTime).repeat :
			Util.UtilOffice.GetStartDate(_reservationInfo.info.repeatDay, _reservationInfo.reservationDateTime).normal;

		txtmp_StartTime.text = Util.Int2StringTime(_reservationInfo.info.startTime);

		if (_reservationInfo.info.repeatDay != 0)
		{
			var repeatDays = Util.CheckDayOfWeek(_reservationInfo.info.repeatDay);
			var index = Util.GetDayOfWeekIndex(DateTime.Now.DayOfWeek);

			if (repeatDays[index])
			{
				var startTime = Util.String2DateTime(DateTime.Now.ToString(), _reservationInfo.info.startTime);
				var endTime = startTime.AddMinutes(_reservationInfo.info.runningTime);

				btn_EnterRoom.interactable = Util.UtilOffice.CheckEnterAvailable(startTime, endTime);
			}

			else btn_EnterRoom.interactable = false;
		}

		else
		{
			var startTime = _reservationInfo.reservationDateTime;
			var endTime = _reservationInfo.reservationDateTime.AddMinutes(_reservationInfo.info.runningTime);

			btn_EnterRoom.interactable = Util.UtilOffice.CheckEnterAvailable(startTime, endTime);
		}

		txtmp_PlayerCount.text = 0 + "/" + _reservationInfo.info.personnel;
		txtmp_Order.text = (index + 1) + " / " + reservationInfos.Count;
	}

	private void RefreshCardButtonUI()
	{
		btn_Next.interactable = false;

		if (reservationInfos.Count == 0) Close();

		else if (reservationInfos.Count == 1) btn_Next.interactable = false;

		else
		{
			btn_Next.interactable = true;
			btn_Prev.interactable = false;
		}
	}
















	public void SetDate(int _year, int _month, int _day)
	{
		this.year = _year;
		this.month = _month;
		this.day = _day;
	}

	public void Flush()
	{
		index = 0;

		reservationInfos = ArzMetaCalendar.GetEventList(this.year, this.month, this.day);

		reservationInfos = reservationInfos.OrderBy(x => x.info.startTime).ToList();

		if (reservationInfos.Count <= 0)
		{
			Close();

			return;
		}

		RefreshCardButtonUI();

		RefreshCardUI(reservationInfos[index]);
	}


	private List<OfficeRoomReservationInfo> GetCurrentList(List<OfficeRoomReservationInfo> _list)
	{
		List<OfficeRoomReservationInfo> current = new List<OfficeRoomReservationInfo>();
		List<OfficeRoomReservationInfo> removed = new List<OfficeRoomReservationInfo>();

		for (int i = 0; i < _list.Count; i++)
		{
			current.Add(_list[i]);
		}

		for (int i = 0; i < current.Count; i++)
		{
			if (GetPanel<Panel_Calendar>().IsReservationToggleOn() == false)
			{
				if (current[i].isMine) removed.Add(current[i]);
			}

			if (GetPanel<Panel_Calendar>().IsInterestToggleOn() == false)
			{
				if (!current[i].isMine) removed.Add(current[i]);
			}
		}

		for (int i = 0; i < removed.Count; i++)
		{
			current.Remove(removed[i]);
		}

		return current;
	}

	public void Close() => OnClick_Close();

	#endregion




	#region Button Events

	private void OnClick_EnterRoom()
	{
		Single.Scene.SetDimOn();

		if (reservationInfos[index].isMine)
		{
			var panel = GetPanel<Panel_Office>();

			var data = panel.MakeRoomData(reservationInfos[index]);

			panel.CreateAndJoin(data);
		}

		else
		{
			var roomCode = reservationInfos[index].info.roomCode;

			var roomType = reservationInfos[index].info.modeType == 1 ? RoomType.Meeting : RoomType.Lecture;

			RealtimeWebManager.SetQuery(roomType);

			RealtimeWebManager.AddQuery(Query.roomCode, roomCode);

			RealtimeWebManager.GetRoom();

			RealtimeWebManager.Run<OfficeRoomInfoRes[]>(GetRoomResult);
		}
	}

	private void GetRoomResult(OfficeRoomInfoRes[] _roomInfos)
	{
		if (_roomInfos.Length <= 0)
		{
			StackPanel<Panel_OfficeWaitRoom>(false, true, true);

			var panel = GetPanel<Panel_OfficeWaitRoom>();

			panel.SetInfo(reservationInfos[index]);

			panel.transform.SetAsLastSibling();

			panel.Show(true);

			return;
		}

		Timing.RunCoroutine(Co_Verify(_roomInfos[0]));
	}

	private IEnumerator<float> Co_Verify(OfficeRoomInfoRes _roomInfo)
	{
		if (_roomInfo.isShutdown)
		{
			yield break;
		}

		if (_roomInfo.isPassword)
		{
			GetPopup<Popup_OfficePassword>().Init(_roomInfo);
			PushPopup<Popup_OfficePassword>();
		}
	}



	private void OnClick_Edit()
	{
		//GetPopup<Popup_OfficeRoomCreate>().Modify_OfficeRoom(reservationInfos[index].info);
		//PushPopup<Popup_OfficeRoomCreate>();
		PushPopup<Popup_OfficeRoomCreate>().Modify_OfficeRoom(reservationInfos[index].info);
	}

    private void OnClick_Share()
    {
        shareLinkInfo.roomType = SHARELINK_TYPE.OFFIICE_INFO;
        shareLinkInfo.nickName = reservationInfos[index].info.nickName;
        shareLinkInfo.roomName = reservationInfos[index].info.roomName;
        shareLinkInfo.description = reservationInfos[index].info.description;
        shareLinkInfo.roomCode = reservationInfos[index].info.roomCode;
        shareLinkInfo.isPassword = Convert.ToInt32(reservationInfos[index].info.isPassword);
        shareLinkInfo.password = reservationInfos[index].info.password;
        shareLinkInfo.topicType = reservationInfos[index].info.topicType;
        shareLinkInfo.startTime = Util.Int2StringTime(reservationInfos[index].info.startTime);

        CreateShareLink.CreateLink(shareLinkInfo);
    }


    private void OnClick_Observe()
	{

	}

	private void OnClick_Interest()
	{
		var view = GetPanel<Panel_Office>().GetView<View_Office_Reservation>();

		view.ShowContent(Define.INTEREST, false, false);

		Timing.RunCoroutine(Co_Interest());
	}

	private IEnumerator<float> Co_Interest()
	{
		yield return Timing.WaitUntilTrue(() => ArzMetaReservationController.Instance.IsReceived(Define.INTEREST));

		var roomCode = reservationInfos[index].info.roomCode;

		if (!Util.UtilOffice.IsInterest(roomCode))
		{
			Single.Web.office.Office_WaitReservationReq(roomCode, (res) => AddInterest(res));
		}

		else
		{
			UnityAction confirm = () => Single.Web.office.Office_CancelReservationWait(roomCode, (res) => DeleteInterest(res, reservationInfos[index]));

			PushPopup<Popup_Basic>()
				.ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("office_confirm_myreservation_del")))
				.ChainPopupAction(new PopupAction(confirm));
		}
	}

	private void AddInterest(Office_WaitOfficeReservRes _res)
	{
		img_interest.sprite = Util.UtilOffice.GetInterest(true);

		deleteInfos.Remove(reservationInfos[index]);
	}

	private void DeleteInterest(DefaultPacketRes _res, OfficeRoomReservationInfo _reservationInfo)
	{
		img_interest.sprite = Util.UtilOffice.GetInterest(false);

		deleteInfos.Add(_reservationInfo);
	}

	List<OfficeRoomReservationInfo> deleteInfos = new List<OfficeRoomReservationInfo>();

	private void OnClick_Prev()
	{
		index = Mathf.Clamp(--index, 0, reservationInfos.Count);

		if (index == 0) btn_Prev.interactable = false;
		btn_Next.interactable = true;

		RefreshCardUI(reservationInfos[index]);

	}

	private void OnClick_Next()
	{
		index = Mathf.Clamp(++index, 0, reservationInfos.Count);

		if (index >= reservationInfos.Count - 1) btn_Next.interactable = false;
		btn_Prev.interactable = true;

		RefreshCardUI(reservationInfos[index]);
	}

	private void OnClick_Close()
	{
		ArzMetaReservationController.Instance.Clear();

		Show(false);
	}

	#endregion




	#region Utils

	public void Show(bool _show)
	{
		this.gameObject.SetActive(_show);
		this.GetComponent<CanvasGroup>().alpha = _show ? 1f : 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = _show ? true : false;
	}

	#endregion
}