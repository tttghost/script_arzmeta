using FrameWork.UI;
using UnityEngine.Events;

public class View_OfficeRealTime : UIBase
{
	protected Scene_OfficeRoom sceneOfficeRoom;

	protected UnityEvent CALLBACK_SEND_PERMISSION = new UnityEvent();

	/// <summary>
	/// 실시간서버 패킷 핸들러 등록(서버에서 보내는 패킷을 받을 핸들러)
	/// </summary>

	protected override void OnEnable()
	{
		base.OnEnable();

		AddHandler();

		REQUEST_NFO();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		RemoveHandler();
	}

	protected virtual void AddHandler() { }

	/// <summary>
	/// 실시간서버 패킷 핸들러 삭제
	/// </summary>
	protected virtual void RemoveHandler() { }

	public virtual void REQUEST_NFO() { }

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		sceneOfficeRoom = SceneLogic.instance as Scene_OfficeRoom;
	}

	protected void CallackSendPermission()
	{
		SceneLogic.instance.PushPopup<Popup_Basic>()
			.ChainPopupData(new PopupData(
				POPUPICON.NONE, BTN_TYPE.Confirm, null,
				new MasterLocalData("office_reception_save")));
	}
}