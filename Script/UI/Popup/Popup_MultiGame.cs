using System;
using FrameWork.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup_MultiGame : Popup_Basic
{
	#region Members

	[SerializeField] TMP_Text txtmp_Create;
	[SerializeField] TMP_Text txtmp_Placeholder;
	[SerializeField] TMP_Text txtmp_Caution;

	[SerializeField] Button btn_Create;
	[SerializeField] TMP_InputField input_Roomdata;

	public delegate void Delegater(string _character);

	#endregion



	#region Initialize

	protected override void OnEnable()
	{
		base.OnEnable();

		Util.SetMasterLocalizing(txtmp_Caution, string.Empty);

		input_Roomdata.caretPosition = 0;
		input_Roomdata.text = string.Empty;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		txtmp_Placeholder =		GetUI_TxtmpMasterLocalizing(nameof(txtmp_Placeholder), new MasterLocalData("game_intro_sample"));
		txtmp_Caution =			GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution));
		txtmp_Create =			GetUI_TxtmpMasterLocalizing(nameof(txtmp_Create), new MasterLocalData("001"));

		btn_Create =			GetUI_Button(nameof(btn_Create));
		input_Roomdata =		GetUI_TMPInputField(nameof(input_Roomdata));

		GetPanel<Panel_MultiGame>().roomName = txtmp_Placeholder.text;
	}

	protected override void Start()
	{
		base.Start();

		btn_Confirm.gameObject.SetActive(false);
	}

	public void SetData(string _type)
	{
		Action action = GetAction(_type);
		Delegater delegater = GetDelegate(_type);

		btn_Create.onClick.RemoveAllListeners();
		input_Roomdata.onValueChanged.RemoveAllListeners();

		input_Roomdata.onValueChanged.AddListener(delegater.Invoke);
		input_Roomdata.contentType = TMP_InputField.ContentType.Standard;
		input_Roomdata.characterLimit = 16;
		
		btn_Create = GetUI_Button(nameof(btn_Create), action.Invoke);

		var masterId_title = _type == Cons.CREATE ? "game_roomname" : "game_find_room";
		txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title), new MasterLocalData(masterId_title));

		var masterId_characterLimit = _type == Cons.CREATE ? "game_info_roomname_input" : "game_info_roomcode_search";
		txtmp_Desc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Desc), new MasterLocalData(masterId_characterLimit));

		Util.SetMasterLocalizing(txtmp_Placeholder, _type == Cons.CREATE ? GetPanel<Panel_MultiGame>().roomName : string.Empty);
	}

	#endregion



	#region Utils

	private void CreateInputLimit(string _roomName)
	{
		if (_roomName.Length > 15)
		{
			btn_Create.interactable = false;
			txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("game_info_roomname_input"));
			return;
		}

		btn_Create.interactable = true;

		Util.SetMasterLocalizing(txtmp_Caution, string.Empty);
	}

	private void SearchInputLimit(string _roomName)
	{
		if (_roomName.Length > 6)
		{
			btn_Create.interactable = false;
			txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("game_error_roomcode_overinput"));
		}

		else
		{
			btn_Create.interactable = true;
			Util.SetMasterLocalizing(txtmp_Caution, string.Empty);
		}
	}

	public Action GetAction(string _popupType)
	{
		Action action = null;
				
		switch (_popupType)
		{
			case Cons.CREATE:				
				action = GetPanel<Panel_MultiGame>().CreateRoom;
				break;
			case Cons.SEARCH:
				action = GetPanel<Panel_MultiGame>().SearchRoom;
				break;
		}

		return action;
	}

	public Delegater GetDelegate(string _popupType)
	{
		Delegater limit = null;

		switch (_popupType)
		{
			case Cons.CREATE:
				limit = CreateInputLimit;
				break;

			case Cons.SEARCH:
				limit = SearchInputLimit;
				break;
		}

		return limit;
	}

	public string GetRoomCode()
	{
		return (input_Roomdata.text == string.Empty ? txtmp_Placeholder.text : input_Roomdata.text).ToUpper();
	}


	public void SetCautionMessage(string _localId) => Util.SetMasterLocalizing(txtmp_Caution, new MasterLocalData(_localId));

	#endregion
}