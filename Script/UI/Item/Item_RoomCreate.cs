using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 룸을 즉시개설 하거나 예약개설을 할 수 있다.
/// </summary>
public class Item_RoomCreate : UIBase
{
	protected TMP_Text txtmp_Title;
	protected TMP_Text txtmp_Description;
	protected TMP_Text txtmp_CreateResv;
	protected TMP_Text txtmp_CreateNow;
	protected Image img_Thumbnail;
	public Button btn_CreateResv { get; private set; }
	protected Button btn_CreateNow;

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
		txtmp_Description = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Description));
		txtmp_CreateResv = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateResv), new MasterLocalData("office_reservation_creation"));
		txtmp_CreateNow = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateNow), new MasterLocalData("office_instant_creation"));

		img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));

        btn_CreateResv = GetUI_Button(nameof(btn_CreateResv), OnClick_RoomResv);
        btn_CreateNow = GetUI_Button(nameof(btn_CreateNow), OnClick_RoomCreate);
    }

	/// <summary>
	/// 예약개설
	/// </summary>
    protected virtual void OnClick_RoomResv()
	{

	}

	/// <summary>
	/// 즉시개설
	/// </summary>
	protected virtual void OnClick_RoomCreate()
    {

    }

}
