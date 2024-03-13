using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;

/// <summary>
/// 아이템들은 인스턴셰이트되자마자 SetMemberUI가 실행된다. 그 이후 SetData가 실행된다.
/// </summary>
public class Item_OfficeCreate : Item_RoomCreate
{
    private OfficeModeType modeType;
    public void SetData(int _modeType)
	{
        modeType = (OfficeModeType)_modeType;

        var dataOfficeModeType = Single.MasterData.dataOfficeModeType.GetData(_modeType);
		Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData(dataOfficeModeType.name));

		var dataOfficeMode = Single.MasterData.dataOfficeMode.GetData(_modeType);
		Util.SetMasterLocalizing(txtmp_Description, new MasterLocalData(dataOfficeMode.modeDesc));

		img_Thumbnail.sprite = Single.Resources.Load<Sprite>(Cons.Path_ArzPhoneIcon + dataOfficeMode.icon);
	}

    /// <summary>
    /// 오피스 예약하기
    /// </summary>
    protected override void OnClick_RoomResv()
    {
        switch ((eOfficeGradeType)LocalPlayerData.OfficeGradeType)
        {
            case eOfficeGradeType.Basic:
                GetOfficeResv(OnGetOfficeResv);
                break;
            case eOfficeGradeType.Pro:
                ShowPopup(eOpenType.Reservation);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 오피스예약정보 가져오기
    /// </summary>
    private void GetOfficeResv(Action<Office_GetReservationInfo> res)
    {
        Single.Web.office.GetRequest_OfficeReservation(res);
    }

    /// <summary>
    /// 오피스 예약정보 가져오기 콜백 = 
    /// </summary>
    /// <param name="res"></param>
    private void OnGetOfficeResv(Office_GetReservationInfo res)
    {
        if((WEBERROR)res.error != WEBERROR.NET_E_SUCCESS)
        {
            return;
        }

        // 내 오피스 예약개수가 1개 이상이면 == 업그레이드 팝업
        if (res.myReservations.Length > 0)
        {
            string grade = Single.MasterData.dataOfficeGradeType.GetData(LocalPlayerData.OfficeGradeType).name;
            string localId = Util.GetMasterLocalizing(grade);
            string str1 = Util.GetMasterLocalizing("office_grade_limit_info_reservation");
            string str2 = Util.GetMasterLocalizing("office_mygrade", localId);
            MasterLocalData desc = new MasterLocalData($"{str1}\n\n{str2}");

            PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, desc, new MasterLocalData("office_upgrade"), new MasterLocalData("common_cancel")))
            .ChainPopupAction(new PopupAction(() => LocalPlayerData.OfficeGradeType++));
        }
        else
        {
            ShowPopup(eOpenType.Reservation);
        }
    }

    /// <summary>
    /// 즉시개설
    /// </summary>
    protected override void OnClick_RoomCreate()
    {
        ShowPopup(eOpenType.Instant);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_openType"></param>
    private void ShowPopup(eOpenType _openType)
	{
		PushPopup<Popup_OfficeRoomCreate>().Create_OfficeRoom(modeType, _openType);
	}

    public void RemoveCreateOnClick()
    {
        btn_CreateNow.onClick.RemoveAllListeners();
    }

	public void AddCreateOnClick(Action _action)
	{
		btn_CreateNow.onClick.AddListener(() => _action?.Invoke());

        txtmp_CreateNow = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateNow), new MasterLocalData("1012"));
	}
}