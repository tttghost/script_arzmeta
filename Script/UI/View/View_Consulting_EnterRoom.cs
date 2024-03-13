using FrameWork.UI;
using TMPro;
using UnityEngine.UI;

public class View_Consulting_EnterRoom : UIBase
{
	Button btn_EnterRoom;

	TMP_Text txtmp_RoomCodeEnter;
	TMP_Text txtmp_RecommendList;
	TMP_Text txtmp_TopicList;
	TMP_Text txtmp_InputRoomCode;
	TMP_Text txtmp_EnterCodeCaution;

	TMP_InputField input_RoomCode;

	protected override void OnEnable()
    {
		input_RoomCode.text = string.Empty;

		btn_EnterRoom.interactable = false;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		btn_EnterRoom = GetUI_Button(nameof(btn_EnterRoom), EnterRoom);

		TMP_Text txtmp_EnterRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterRoom), new MasterLocalData("common_enter"));
		TMP_Text txtmp_MedicineBanner = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MedicineBanner), new MasterLocalData("office_notice_counseling_welcome"));

		txtmp_EnterCodeCaution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterCodeCaution), new MasterLocalData("office_error_roomcode"));
		txtmp_InputRoomCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_InputRoomCode), new MasterLocalData("office_entering_with_roomcode"));

		input_RoomCode = GetUI_TMPInputField(nameof(input_RoomCode), OnValueChanged_RoomCode);
		input_RoomCode.characterLimit = 8;

		txtmp_EnterCodeCaution.gameObject.SetActive(false);
	}

	private void OnValueChanged_RoomCode(string _roomCode)
	{
		if (_roomCode.Length == 0)
		{
			Caution(false);

			btn_EnterRoom.interactable = false;

			return;
		}

		Caution(_roomCode.Length < 8);

		btn_EnterRoom.interactable = (_roomCode.Length >= 8);
	}

	public void Caution(bool _enable)
	{
		txtmp_EnterCodeCaution.gameObject.SetActive(_enable);

		Util.SetMasterLocalizing(txtmp_EnterCodeCaution, new MasterLocalData("office_error_roomcode"));
	}

	public string GetRoomCode()
	{
		var placeHolder = input_RoomCode.placeholder.GetComponent<TMP_Text>();

		return (input_RoomCode.text == string.Empty ? placeHolder.text : input_RoomCode.text).ToUpper();
	}

	public void NoRoom()
	{
		input_RoomCode.text = string.Empty;

		txtmp_EnterCodeCaution.gameObject.SetActive(true);

		Util.SetMasterLocalizing(txtmp_EnterCodeCaution, new MasterLocalData("game_error_room_nonexist"));
	}

	private void EnterRoom()
	{
		var roomCode = GetRoomCode();

		GetPanel<Panel_Consulting>().SearchAndJoin(roomCode);
	}
}