using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public partial class Popup_OfficeRoomCreate : Popup_BaseRoomCreate
{

	/// <summary>
	/// Web에서 받은 데이터를 실시간데이터로 변환
	/// </summary>
	/// <param name="memberReservationInfo"></param>
	/// <returns></returns>
	private OfficeRoomData ConvertRuntimeData(OfficeReservationInfo memberReservationInfo)
	{
		Single.Scene.SetDimOn();

		OfficeRoomData data = new OfficeRoomData();
		data.roomCode = memberReservationInfo.roomCode;
		data.roomName = memberReservationInfo.roomName;
		data.description = memberReservationInfo.description;
		data.spaceInfoId = memberReservationInfo.spaceInfoId.ToString();
		data.thumbnail = string.IsNullOrEmpty(memberReservationInfo.thumbnail) ? "" : memberReservationInfo.thumbnail;
		data.modeType = memberReservationInfo.modeType;
		data.topicType = memberReservationInfo.topicType;
		data.password = string.IsNullOrEmpty(memberReservationInfo.password) ? "" : memberReservationInfo.password;
		data.personnel = memberReservationInfo.personnel;
		data.isWaitingRoom = Convert.ToBoolean(memberReservationInfo.isWaitingRoom);
		data.observer = memberReservationInfo.observer;
		data.isAdvertising = Convert.ToBoolean(memberReservationInfo.isAdvertising);
		data.runningTime = memberReservationInfo.runningTime;
		data.creatorId = memberReservationInfo.memberCode;
		data.roomType = GetRoomType(data.modeType);
		data.sceneName = GetSceneName(data.spaceInfoId);
		return data;
	}

	protected override void CreateRoom()
	{
		base.CreateRoom();
		Single.Scene.SetDimOn();

		var panel = GetPanel<Panel_Office>();

		Office_CreateOfficeReservReq officeRoomInfo = GetOfficeReservReqData();
		Dictionary<string, string> dummyDic = GetOfficeFormData(officeRoomInfo);

		Single.Web.office.Office_CreateOfficeReservReq(dummyDic, thumbnailPath, null, null, (res) =>
		{
			if (res.error == (int)WEBERROR.NET_E_SUCCESS)
			{
				SaveOfficeThumbnail(res.memberReservationInfo.roomCode, res.memberReservationInfo.thumbnail);
				OfficeRoomData runtimeData = ConvertRuntimeData(res.memberReservationInfo);
				panel.CreateAndJoin(runtimeData);
			}
		});
	}

	#region 상담실 

	private void CreateRoom_Consulting()
	{
		var panel = GetPanel<Panel_Consulting>();

		Single.Web.office.Office_CreateRoomCodeReq((res) =>
		{
			OfficeRoomData data = new OfficeRoomData();

			data.roomName = input_RoomName.text != string.Empty ? input_RoomName.text : input_RoomName.placeholder.GetComponent<TMP_Text>().text;
			data.roomCode = res.roomCode;
			data.description = input_RoomDesc.text != string.Empty ? input_RoomDesc.text : input_RoomDesc.placeholder.GetComponent<TMP_Text>().text;
			data.personnel = curCapacity;
			data.runningTime = (playTime.hour * 60) + (playTime.min);

			data.creatorId = LocalPlayerData.MemberCode;
			data.roomType = RoomType.Consulting.ToString();
			data.sceneName = SceneName.Scene_Room_Consulting.ToString();

			panel.CreateAndJoin(data);
		});
	}

	#endregion


	private string GetRoomType(int _modeType)
	{
		var roomType = RoomType.None;

		if(_modeType == ((int)OfficeModeType.Meeting))
		{
			roomType = RoomType.Meeting;
		}

		else if (_modeType == ((int)OfficeModeType.Lecture))
		{
			roomType = RoomType.Lecture;
		}

		return roomType.ToString();
	}

	private string GetSceneName(string _spaceInfoId)
	{
		var sceneName = Single.MasterData.dataOfficeSpaceInfo.GetData(int.Parse(_spaceInfoId)).sceneName;

		return sceneName;
	}
}