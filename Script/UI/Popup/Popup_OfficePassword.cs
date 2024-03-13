using FrameWork.Network;
using FrameWork.UI;
using TMPro;

public class Popup_OfficePassword : Popup_Basic
{
	TMP_InputField input_Password = null;
	TMP_Text txtmp_Roomdata;

	OfficeRoomInfoRes roomInfo;
	OfficeRoomReservationInfo reservationInfo;

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		input_Password = GetUI_TMPInputField("input_Password");
		GetUI_TxtmpMasterLocalizing("txtmp_Desc", new MasterLocalData("1175"));

		txtmp_Roomdata = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Roomdata));
		Util.SetMasterLocalizing(txtmp_Roomdata, string.Empty);

		input_Password.onValueChanged.AddListener(OnValueChanged_Password);
	}

	protected override void OnDisable()
	{
		base.OnEnable();

		roomInfo = null;
		reservationInfo = null;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		input_Password.text = null;
		Util.SetMasterLocalizing(txtmp_Roomdata, string.Empty);
	}

	public void Init(OfficeRoomInfoRes _roomInfo)
	{
		roomInfo = _roomInfo;
	}

	public void Init(OfficeRoomReservationInfo _reservationInfo)
	{
		reservationInfo = _reservationInfo;
	}

	protected override void OnConfirm()
	{
		base.OnConfirm();

		var panel = GetPanel<Panel_Office>();

		OfficeRoomData roomData = new OfficeRoomData();
		roomData.roomType = panel.roomType.ToString();
		roomData.password = input_Password.text.ToUpper();

		RealtimeWebManager.SetRoomData(roomData);

		if(roomInfo != null)
		{
			panel.JoinRoom(roomInfo);
		}

		if(reservationInfo != null)
		{
			Single.Web.office.Office_CheckRoomPasswordReq(reservationInfo.info.roomCode, input_Password.text, Match, Error);
		}
	}

	private void Match(Office_CheckRoomPassword _checkOffice)
	{
		reservationInfo.info.password = input_Password.text;

		var panel = GetPanel<Panel_Office>();

		panel.JoinReservation(reservationInfo);
	}

	private void Error(DefaultPacketRes _res)
	{

	}

	private void OnValueChanged_Password(string _value)
	{
		input_Password.text = _value.ToUpper();
	}
}
