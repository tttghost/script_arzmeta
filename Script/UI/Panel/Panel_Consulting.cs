using FrameWork.UI;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Consulting : PanelBase, INetworkRoom
{
	Button btn_Close;

	Toggle tog_EnterRoom;
	Toggle tog_CreateRoom;

	TMP_Text txtmp_title;
	TMP_Text txtmp_EnterRoom;
	TMP_Text txtmp_CreateRoom;

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		btn_Close = GetUI_Button(nameof(btn_Close), () => { SceneLogic.instance.Back(); });

		tog_EnterRoom = GetUI_Toggle(nameof(tog_EnterRoom), OnClick_EnterRoom);
		tog_CreateRoom = GetUI_Toggle(nameof(tog_CreateRoom), OnClick_CreateRoom);

		txtmp_title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_title), new MasterLocalData("office_space_consulting001"));
		txtmp_EnterRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_EnterRoom), new MasterLocalData(Cons.MASTER_OFFICEROOM_ENTER));
		txtmp_CreateRoom = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateRoom), new MasterLocalData(Cons.MASTER_OFFICEROOM_TITLE_SET));		
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		yield return Timing.WaitForOneFrame;

		tog_EnterRoom.isOn = true;

		ChangeView(nameof(View_Consulting_EnterRoom));
	}


	#region Core Methods

	public void CreateRoom()
	{
		throw new System.NotImplementedException();
	}

	public void JoinRoom<T>(T _roomInfo)
	{
		var roomInfo = _roomInfo as OfficeRoomInfoRes;

		Single.RealTime.roomType.target = RoomType.Consulting;

		Single.RealTime.JoinRoom(roomInfo);
	}

	public void CreateAndJoin<T>(T _roomData)
	{
		var roomData = _roomData as OfficeRoomData;

		RealtimeWebManager.CreateRoom(roomData);

		RealtimeWebManager.Run<OfficeRoomInfoRes>(JoinRoom);
	}

	public void CreateOrJoin<T>(T _roomData)
	{
		throw new System.NotImplementedException();
	}

	public void SearchAndJoin(string _roomCode)
	{
		RealtimeWebManager.SetQuery(RoomType.Consulting);

		RealtimeWebManager.AddQuery(Query.roomCode, _roomCode);

		RealtimeWebManager.GetRoom();

		RealtimeWebManager.Run<OfficeRoomInfoRes[]>(JoinSearchRoom);
	}

	public void JoinSearchRoom(OfficeRoomInfoRes[] _roomInfo)
	{
		if (_roomInfo.Length <= 0)
		{
			var view = GetView<View_Consulting_EnterRoom>();

			view.NoRoom();

			return;
		}

		var roomInfo = _roomInfo[0];

		JoinRoom(roomInfo);
	}

	public IEnumerator<float> Co_RefreshList(float _refreshRate)
	{
		throw new System.NotImplementedException();
	}

	public void RefreshList()
	{
		throw new System.NotImplementedException();
	}

	#endregion



	#region Events 

	private void OnClick_EnterRoom()
	{
		ChangeView(nameof(View_Consulting_EnterRoom));
	}

	private void OnClick_CreateRoom()
	{
		ChangeView(nameof(View_Consulting_CreateRoom));
	}

	#endregion
}
