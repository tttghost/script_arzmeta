using System.Collections.Generic;
using System.Linq;
using FrameWork.UI;
using MEC;
using TMPro;
using UnityEngine.UI;

public class View_Exposition_EnterRoom : UIBase
{
	#region Members

	TMP_Text txtmp_EnterRoom;
	TMP_Text txtmp_Caution;

	Button btn_Refresh;
	Button btn_SearchRoom;

	TMP_InputField input_RoomCode;

	DynamicScroll_Custom scroll;

	TMP_Dropdown dropdown_Topic;

	#endregion



	#region Initialize

	protected override void OnDisable()
	{
		base.OnDisable();

		dropdown_Topic.value = 0;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		input_RoomCode.contentType = TMP_InputField.ContentType.Standard;
		input_RoomCode.text = string.Empty;
		btn_SearchRoom.interactable = false;

		var dropdown_Label = dropdown_Topic.transform.GetChild(0).GetComponent<TMP_Text>();
		dropdown_Label.text = Util.GetMasterLocalizing("office_booth_name");

		dropdown_Topic.options[0].text = Util.GetMasterLocalizing("office_booth_name");
		dropdown_Topic.options[1].text = Util.GetMasterLocalizing("office_booth_code");

		var panel = SceneLogic.instance.GetPanel<Panel_Exposition>();
		panel.RefreshList();

		Caution(false);
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		txtmp_EnterRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterRoom), new MasterLocalData("common_search"));
		txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("office_booth_notice_nonexistence"));

		input_RoomCode = GetUI_TMPInputField(nameof(input_RoomCode), OnValueChanged_RoomCode);
		input_RoomCode.characterLimit = 16;

		btn_SearchRoom = GetUI_Button(nameof(btn_SearchRoom), OnClick_SearchRoom);
		btn_Refresh = GetUI_Button(nameof(btn_Refresh), OnClick_Refresh);

		dropdown_Topic = GetUI<TMP_Dropdown>(nameof(dropdown_Topic));
		dropdown_Topic.onValueChanged.AddListener(OnDropdownValueChanged);

		Caution(false);

		ChangeView(nameof(View_Exposition_RoomList));
	}

	#endregion



	#region Core Methods

	public void OnClick_Refresh()
	{
		GetPanel<Panel_Exposition>().isDim = true;
		GetPanel<Panel_Exposition>().RefreshList();
	}

	protected void OnClick_SearchRoom()
	{
		switch (dropdown_Topic.value)
		{
			case 0:
				SearchAsRoomName();
				break;
			case 1:
				SearchAsRoomCode();
				break;
			default:
				break;
		}
	}

	private void SearchAsRoomCode()
	{
		var roomCode = GetRoomCode();

		Single.Web.CSAF.GetCSAFBooths((res) =>
		{
			var view = GetView<View_Exposition_RoomList>();

			view.Refresh(res.booths);

			Booth searchBooth = res.booths.FirstOrDefault(booth => booth.roomCode == roomCode);

			if (searchBooth != null)
			{
				GetPopup<Popup_ExpositionRoomInfo>().SetExpositionCardInfo(searchBooth);

				PushPopup<Popup_ExpositionRoomInfo>();
			}

			else
			{
				PushPopup<Popup_Basic>().ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("office_booth_notice_nonexistence")));
			}
		});
	}

	private void SearchAsRoomName()
	{
		var boothName = GetRoomCode();

		Single.Web.CSAF.GetCSAFBoothName(boothName, (res) =>
		{
			if(res.booth != null)
			{
				var view = GetView<View_Exposition_RoomList>();

				view.Refresh(res.booth);
			}

			else
			{
				PushPopup<Popup_Basic>().ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, null, new MasterLocalData("office_booth_notice_nonexistence")));
			}
		});
	}

	#endregion



	#region Utils

	private void OnValueChanged_RoomCode(string _roomCode)
	{
		if (_roomCode.Length == 0)
		{
			GetPanel<Panel_Exposition>().RefreshList();

			btn_SearchRoom.interactable = false;
		}

		else btn_SearchRoom.interactable = true;

		if (dropdown_Topic.value == 0) return;

		if (_roomCode.Length == 0)
		{
			Caution(false);

			return;
		}

		Caution(_roomCode.Length < input_RoomCode.characterLimit);

		btn_SearchRoom.interactable = (_roomCode.Length >= input_RoomCode.characterLimit);
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

	public void OnDropdownValueChanged(int value)
	{
		switch(value)
		{
			case 0:
				input_RoomCode.contentType = TMP_InputField.ContentType.Standard;
				input_RoomCode.characterLimit = 16;
				input_RoomCode.text = string.Empty;
				btn_SearchRoom.interactable = true;
				break;
			case 1:
				input_RoomCode.contentType = TMP_InputField.ContentType.IntegerNumber;
				input_RoomCode.characterLimit = 8;
				input_RoomCode.text = string.Empty;
				btn_SearchRoom.interactable = false;
				break;
			default:
				break;
		}
	}

	#endregion
}
