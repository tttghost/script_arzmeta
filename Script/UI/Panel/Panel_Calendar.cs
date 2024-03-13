using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FrameWork.UI;
using MEC;
using System;
using UnityEngine.EventSystems;

public class Panel_Calendar : PanelBase
{
	#region Members

	ArzMetaCalendar calendar;

	WeekDate week = new WeekDate();
	const string WEEKFORMAT = "{0:D4}. {1:D2}. {2:D2}";
	public bool isWeekMode = true;

	Button btn_NextYear;
	Button btn_PrevYear;
	Button btn_NextMonth;
	Button btn_PrevMonth;
	Button btn_Home;
	Button btn_Back;
	Button btn_ChangeMode;

	TMP_Text txtmp_Date;
	TMP_Text txtmp_ChangeMode;

	TMP_Text txtmp_Title;
	TMP_Text txtmp_Home;

	TMP_Text txtmp_Meeting;
	TMP_Text txtmp_Lecture;


	[HideInInspector] public TogglePlus togplus_Reservation;
	[HideInInspector] public TogglePlus togplus_Interest;

	[HideInInspector] public View_Office_ReservationSelect view_ReservationSelect;

	#endregion




	#region Initialize

	protected override void OnEnable()
	{
		base.OnEnable();

		togplus_Reservation.SetToggleIsOn(true);
		togplus_Interest.SetToggleIsOn(true);
		isWeekMode = true;

		BackAction_Custom += OnClick_Back;
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		ShowWeekCalendar();

		BackAction_Custom -= OnClick_Back;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		if (Util.UtilOffice.IsOffice())
		{
			Util.UtilOffice.DestroyIfNotOffice(this.gameObject);

			return;
		}

		calendar = GetComponent<ArzMetaCalendar>();
		view_ReservationSelect = GetView<View_Office_ReservationSelect>();

		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);

		btn_NextYear = GetUI_Button(nameof(btn_NextYear), OnClick_NextYear);
		btn_PrevYear = GetUI_Button(nameof(btn_PrevYear), OnClick_PreviousYear);
		btn_NextMonth = GetUI_Button(nameof(btn_NextMonth), OnClick_NextMonth);
		btn_PrevMonth = GetUI_Button(nameof(btn_PrevMonth), OnClick_PreviousMonth);
		btn_Home = GetUI_Button(nameof(btn_Home), OnClick_Home);

		btn_ChangeMode = GetUI_Button(nameof(btn_ChangeMode), OnClick_ChangeMode);
		txtmp_ChangeMode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChangeMode), new MasterLocalData(isWeekMode ? "office_weekly" : "office_monthly"));

		txtmp_Date = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Date));

		togplus_Reservation = GetUI<TogglePlus>(nameof(togplus_Reservation));
		togplus_Reservation.SetToggleAction((isToggleOn) => OnClick_ReservationToggle(isToggleOn));

		togplus_Interest = GetUI<TogglePlus>(nameof(togplus_Interest));
		togplus_Interest.SetToggleAction((isToggleOn) => OnClick_InterestToggle(isToggleOn));

		txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData("office_my_calendar"));
		txtmp_Home = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Home), new MasterLocalData("office_today"));

		txtmp_Meeting = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Meeting), new MasterLocalData(Single.MasterData.dataOfficeModeType.GetData(1).name));
		txtmp_Lecture = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Lecture), new MasterLocalData(Single.MasterData.dataOfficeModeType.GetData(2).name));
		
		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		showStart += () => this.transform.SetAsLastSibling();
		hideEnd += () => this.gameObject.SetActive(false);
	}

	protected override IEnumerator<float> Co_SetCloseStartAct()
	{
		Show(false, _instant: true);

		var view = GetPanel<Panel_Office>().GetView<View_Office_Reservation>();

		view.ShowContent(view.content, false);

		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha <= 0);
	}

	#endregion




	#region Core Methods

	public void ShowReservations(List<OfficeRoomReservationInfo> _list)
	{
		if (_list.Count <= 0) return;

		view_ReservationSelect.SetInfo(_list);
		view_ReservationSelect.Show(true);

		GetView<View_Office_ReservationSelect>().gameObject.SetActive(true);
	}

	public void ShowMonthCalendar()
	{
		ArzMetaReservationController.Instance.ShowMonthCalendar();
	}

	public void ShowWeekCalendar()
	{
		ArzMetaReservationController.Instance.RefreshWeekCount();
		ArzMetaReservationController.Instance.ShowWeekCalendar();
	}

	#endregion




	#region Button Events

	private void OnClick_Back()
	{
		ArzMetaReservationController.Instance.Clear();

		if (view_ReservationSelect.gameObject.activeSelf)
		{
			view_ReservationSelect.Close();
			return;
		}

		SceneLogic.instance.PopPanel();
	}

	private void OnClick_ChangeMode()
	{
		isWeekMode = !isWeekMode;

		txtmp_ChangeMode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ChangeMode), new MasterLocalData(isWeekMode ? "office_weekly" : "office_monthly"));

		if (isWeekMode)
		{
			ShowWeekCalendar();
		}

		else ShowMonthCalendar();
	}

	private void OnClick_ReservationToggle(bool _isToggleOn)
	{
		ArzMetaReservationController.Instance.OnClick_ShowReservation(_isToggleOn);
	}

	private void OnClick_InterestToggle(bool _isToggleOn)
	{
		ArzMetaReservationController.Instance.OnClick_ShowInterest(_isToggleOn);
	}

	private void OnClick_NextYear()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.date.year++;

		calendar.RefreshEvents();
	}

	private void OnClick_PreviousYear()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.date.year--;

		calendar.RefreshEvents();
	}

	private void OnClick_NextMonth()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.date.month = (calendar.date.month + 1) % 13;

		if (calendar.date.month == 0)
		{
			calendar.date.year++;
			calendar.date.month = 1;
		}

		calendar.RefreshEvents();
	}

	private void OnClick_PreviousMonth()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.date.month = (calendar.date.month - 1) % 13;

		if (calendar.date.month == 0)
		{
			calendar.date.year--;
			calendar.date.month = 12;
		}

		calendar.RefreshEvents();
	}

	private void OnClick_NextWeek()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.RefreshEvents();

		calendar.callback_nextWeek?.Invoke();
	}

	private void OnClick_PreviousWeek()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.RefreshEvents();

		calendar.callback_prevWeek?.Invoke();
	}

	private void OnClick_DaySelect()
	{
		calendar.UnMarkDay(calendar.date.day);

		string slot_name = EventSystem.current.currentSelectedGameObject.name;
		int slot_position = int.Parse(slot_name.Substring(5, (slot_name.Length - 5)));

		calendar.date.day = calendar.GetDayInSlot(slot_position);
		calendar.date.dayOfWeek = calendar.GetDayOfWeek(calendar.date.year, calendar.date.month, calendar.date.day);

		calendar.MarkDay(calendar.date.day);

		if (calendar.delegate_daySelect != null) calendar.delegate_daySelect(calendar.date);

		if (ArzMetaCalendar.GetEventList(calendar.date.year, calendar.date.month, calendar.date.day).Count > 0)
		{
			if (calendar.delegate_eventSelect == null) return;

			calendar.delegate_eventSelect(calendar.date, ArzMetaCalendar.GetEventList(calendar.date.year, calendar.date.month, calendar.date.day));
		}
	}

	private void OnClick_Home()
	{
		calendar.UnMarkDay(calendar.date.day);

		calendar.SetCurrentTime();

		calendar.RefreshCalendar(calendar.date.month, calendar.date.year);

		calendar.MarkDay(calendar.date.day);

		calendar.delegate_home?.Invoke(calendar.date);
	}

	public void OnClick_RegisterHome(Action _action)
	{
		btn_Home = GetUI_Button(nameof(btn_Home), () => _action?.Invoke());
	}

	#endregion




	#region Utils

	public void SwitchUIEvent()
	{
		btn_NextMonth.GetComponent<Button>().onClick.RemoveAllListeners();
		btn_PrevMonth.GetComponent<Button>().onClick.RemoveAllListeners();

		if(isWeekMode)
		{
			btn_NextMonth.GetComponent<Button>().onClick.AddListener(() => OnClick_NextWeek());
			btn_PrevMonth.GetComponent<Button>().onClick.AddListener(() => OnClick_PreviousWeek());
		}
		else
		{
			btn_NextMonth.GetComponent<Button>().onClick.AddListener(() => OnClick_NextMonth());
			btn_PrevMonth.GetComponent<Button>().onClick.AddListener(() => OnClick_PreviousMonth());
		}
	}

	public void RefreshLabel(int _value)
	{
		txtmp_Date.text = calendar.GetYearAndMonth(_value);
	}

	public void RefreshWeekLabelUI(int _weekStartDate)
	{
		week.startDate = _weekStartDate;

		if (week.startDate + 6 > DateTime.DaysInMonth(calendar.date.year, calendar.date.month))
		{
			int leftovers = 0;
			int month = calendar.date.month + 1 < 13 ? calendar.date.month + 1 : 1;
			int year = month == 1 ? calendar.date.year + 1 : calendar.date.year;

			for (int i = week.startDate; i < week.startDate + 7; i++)
			{
				if (i > DateTime.DaysInMonth(calendar.date.year, calendar.date.month)) leftovers++;
			}

			week.start = string.Format(WEEKFORMAT, calendar.date.year, calendar.date.month, week.startDate);
			week.end = string.Format(WEEKFORMAT, year, month, leftovers);
		}

		else if (week.startDate < 1)
		{
			int month = calendar.date.month - 1 < 1 ? 12 : calendar.date.month - 1;
			int year = month == 12 ? calendar.date.year - 1 : calendar.date.year;

			week.start = string.Format(WEEKFORMAT, year, month, (DateTime.DaysInMonth(year, month) + week.startDate));
			week.end = string.Format(WEEKFORMAT, calendar.date.year, calendar.date.month, (7 + week.startDate - 1));
		}

		else
		{
			week.start = string.Format(WEEKFORMAT, calendar.date.year, calendar.date.month, week.startDate);
			week.end = string.Format(WEEKFORMAT, calendar.date.year, calendar.date.month, (week.startDate + 6));
		}

		txtmp_Date.text = week.start + " - " + week.end;
	}

	public bool IsReservationToggleOn()
	{
		return togplus_Reservation.GetToggleIsOn();
	}

	public bool IsInterestToggleOn()
	{
		return togplus_Interest.GetToggleIsOn();
	}

	#endregion
}