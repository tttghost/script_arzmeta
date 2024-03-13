using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Item_OfficeRoomList : DynamicScrollItem_Custom
{
	#region Members

	[HideInInspector] public OfficeItem officeItem;

	TMP_Text txtmp_RoomName;
	TMP_Text txtmp_HostName;
	TMP_Text txtmp_PlayerCount;
	TMP_Text txtmp_CreatedTime;
	TMP_Text txtmp_ModeType;

	Image img_Thumbnail;
	Image img_Lock;

	Button btn_ShowRoom;

	Sprite sprite_password;
	Sprite sprite_noPassword;

	#endregion



	#region Initialize

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		sprite_password = CommonUtils.Load<Sprite>(Cons.Path_Image + "icon_lock_01");
		sprite_noPassword = CommonUtils.Load<Sprite>(Cons.Path_Image + "icon_unlock_01");

		txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
		txtmp_HostName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_HostName));
		txtmp_PlayerCount = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerCount));
		txtmp_CreatedTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreatedTime));
		txtmp_ModeType = GetUI_TxtmpMasterLocalizing(nameof(txtmp_ModeType));

		btn_ShowRoom = GetUI_Button(nameof(btn_ShowRoom), OnClick_ShowRoom);

		img_Lock = GetUI_Img(nameof(img_Lock));
		img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));
	}

	public override void UpdateData(DynamicScrollData _scrollData)
	{
		base.UpdateData(_scrollData);

		var data = _scrollData as OfficeItem;

		SetRoomInfo(data);
	}

	private async void SetRoomInfo(OfficeItem _officeItem)
	{
		var roomInfo = _officeItem.roomInfo;
		
		Util.SetMasterLocalizing(txtmp_RoomName, roomInfo.roomName);
		Util.SetMasterLocalizing(txtmp_HostName, roomInfo.hostName);
		Util.SetMasterLocalizing(txtmp_PlayerCount, roomInfo.currentPersonnel + "/" + roomInfo.personnel);
		Util.SetMasterLocalizing(txtmp_CreatedTime, roomInfo.createdTime);
		Util.SetMasterLocalizing(txtmp_ModeType, GetModeType(roomInfo.modeType));
		img_Lock.sprite = GetLockType(roomInfo.isPassword);

        #region 썸네일
        Sprite thumbnail = await Util.UtilOffice.GetThumbnail_Office(roomInfo.roomCode, roomInfo.thumbnail);

		if (thumbnail != null)
		{
			img_Thumbnail.sprite = thumbnail;
		}
		else
		{
			img_Thumbnail.sprite = Util.UtilOffice.GetSpaceThumbnail(roomInfo.spaceInfoId.ToString()).sprite;
		}
        #endregion

        officeItem = _officeItem;
	}

	#endregion



	#region Button Events

	private void OnClick_ShowRoom()
    {
        GetPopup<Popup_OfficeRoomInfo>().SetCardInfo(officeItem.roomInfo);

        PushPopup<Popup_OfficeRoomInfo>();
    }

    #endregion



    #region Utils

    private string GetModeType(int _modeType)
	{
		var masterId = Single.MasterData.dataOfficeModeType.GetData(_modeType).name;

		return GetUI_TxtmpMasterLocalizing(nameof(txtmp_ModeType), new MasterLocalData(masterId)).text;
	}

	private Sprite GetLockType(bool _isPassword)
	{
		return Convert.ToBoolean(_isPassword) ? sprite_password : sprite_noPassword;
	}

	private Sprite GetSpaceThumbnail(string _spaceInfoId)
	{
		var filename = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(_spaceInfoId)).thumbnail;

		return Resources.Load<Sprite>(Cons.Path_OfficeThumbnail + filename);
	}

	#endregion
}