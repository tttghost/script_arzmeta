using System.Collections.Generic;
using System.Linq;
using FrameWork.UI;
using MEC;
using TMPro;
using UnityEngine.UI;


public class View_Office_EnterRoom : UIBase
{
	#region Members

	TMP_Text txtmp_EnterRoom;
	TMP_Text txtmp_RecommendList;
	TMP_Text txtmp_RoomList;
	TMP_Text txtmp_RoomCode;
	TMP_Text txtmp_Caution;

	Button btn_Refresh;
	Button btn_SearchRoom;

	TMP_InputField input_RoomCode;
	TMP_Dropdown dropdown_Topic;

	Toggle tog_RecommendList;
	Toggle tog_RoomList;

	int characterLimist = 8;

	#endregion



	#region Initialize

	protected override void OnDisable()
	{
		base.OnDisable();

		var panel = GetPanel<Panel_Office>();

		Timing.KillCoroutines(panel.handle_Refresh);
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		input_RoomCode.text = string.Empty;
		btn_SearchRoom.interactable = false;
		tog_RoomList.isOn = true;

		var dropdown_Label = dropdown_Topic.transform.GetChild(0).GetComponent<TMP_Text>();
		dropdown_Label.text = Util.GetMasterLocalizing(Single.MasterData.dataOfficeModeType.GetData(dropdown_Topic.value + 1).name);

		for (int i = 0; i < dropdown_Topic.options.Count; i++)
		{
			var masterId = Single.MasterData.dataOfficeModeType.GetData(i + 1).name;

			dropdown_Topic.options[i].text = Util.GetMasterLocalizing(new MasterLocalData(masterId));
		}

		Caution(false);
		ChangeView(nameof(View_Office_RoomList));

		GetPanel<Panel_Office>().RefreshUpdate();
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		tog_RoomList = GetUI_Toggle(nameof(tog_RoomList), () => { ChangeView(nameof(View_Office_RoomList)); });
		tog_RecommendList = GetUI_Toggle(nameof(tog_RecommendList), () => { ChangeView(nameof(View_Office_RecommendRoomList)); });

		txtmp_EnterRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterRoom), new MasterLocalData("common_search"));
		txtmp_RoomCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCode), new MasterLocalData("office_search_roomcode"));
		txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("1176"));

		input_RoomCode = GetUI_TMPInputField(nameof(input_RoomCode), OnValueChanged_RoomCode);
		input_RoomCode.characterLimit = characterLimist;

		txtmp_RecommendList = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RecommendList), new MasterLocalData("1023"));
		txtmp_RoomList = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomList), new MasterLocalData("office_room_list"));

		btn_SearchRoom = GetUI_Button(nameof(btn_SearchRoom), OnClick_SearchRoom);
		btn_Refresh = GetUI_Button(nameof(btn_Refresh), OnClick_Refresh);

		tog_RoomList.isOn = true;

		dropdown_Topic = GetUI<TMP_Dropdown>(nameof(dropdown_Topic));

		dropdown_Topic.options.Clear();
		dropdown_Topic.options.Add(new TMP_Dropdown.OptionData("회의"));
		dropdown_Topic.options.Add(new TMP_Dropdown.OptionData("강의"));

		for (int i = 0; i < dropdown_Topic.options.Count; i++)
		{
			var masterId = Single.MasterData.dataOfficeTopicType.GetData(i + 1).name;

			dropdown_Topic.options[i].text = Util.GetMasterLocalizing(new MasterLocalData(masterId));
		}

		dropdown_Topic.onValueChanged.AddListener(OnDropdownValueChanged);

		Caution(false);
		ChangeView(nameof(View_Office_RoomList));
	}

	#endregion



	#region Core Methods

	public void OnClick_Refresh()
	{
		var panel = GetPanel<Panel_Office>();

		panel.isDim = true;
		panel.RefreshList();
	}

	protected void OnClick_SearchRoom()
	{
		ArzMetaReservationController.Instance.Request(Define.RESERVATION);
		ArzMetaReservationController.Instance.Request(Define.INTEREST);

		Timing.RunCoroutine(Co_SearchRoom());
	}

	IEnumerator<float> Co_SearchRoom()
	{
		yield return Timing.WaitUntilTrue(() => ArzMetaReservationController.Instance.IsReceivedAll());

		var roomCode = GetRoomCode();

		GetPanel<Panel_Office>().SearchAndJoin(roomCode);
	}

	private void OnDropdownValueChanged(int _index)
	{
		GetPanel<Panel_Office>().roomType = (_index == 0) ? RoomType.Meeting : RoomType.Lecture;

		GetPanel<Panel_Office>().RefreshList();
	}

	#endregion



	#region Utils

	private void OnValueChanged_RoomCode(string _roomCode)
	{
		if (_roomCode.Length == 0)
		{
			Caution(false);

			btn_SearchRoom.interactable = false;

			return;
		}

		Caution(_roomCode.Length < characterLimist);

		btn_SearchRoom.interactable = (_roomCode.Length >= characterLimist);
	}

	public void Caution(bool _enable)
	{
		txtmp_Caution.gameObject.SetActive(_enable);
	}

	public string GetRoomCode()
	{
		var placeHodler = input_RoomCode.placeholder.GetComponent<TMP_Text>();

		return (input_RoomCode.text != string.Empty ? input_RoomCode.text : placeHodler.text).ToUpper();
	}

	public void ClearRoomCode()
	{
		input_RoomCode.Clear();
	}

	#endregion



	#region Tutorial

	public void SetRoomListToggle()
	{
		var roomToggle = GetUI<Toggle>("tog_TopicList");

		roomToggle.isOn = true;
	}

	public void SetRecommendListToggle()
	{
		var roomToggle = GetUI<Toggle>("tog_RecommendList");

		roomToggle.isOn = true;
	}

	#endregion
}
