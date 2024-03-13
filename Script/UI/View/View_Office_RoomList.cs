using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MEC;

public class View_Office_RoomList : UIBase
{
	#region Members

	DynamicScroll_Custom scroll;
	TMP_Text txtmp_Caution;

	#endregion



	#region Initialize

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		var prefab = Resources.Load<DynamicScrollItem_Custom>(Cons.Path_Prefab_Item + "item_OfficeRoomList");

		scroll = this.GetComponentInChildren<DynamicScroll_Custom>();
		scroll.itemPrefab = prefab;

		txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("1168"));
	}

	#endregion



	#region Core Methods

	public void Refresh(OfficeRoomInfoRes[] _roomInfos) => Util.RunCoroutine(Co_Refresh(_roomInfos));

	private IEnumerator<float> Co_Refresh(OfficeRoomInfoRes[] _roomInfos)
	{
		yield return Timing.WaitForOneFrame;

		scroll.ClearData();

		for (int i = 0; i < _roomInfos.Length; i++)
		{
			var item = new OfficeItem();

			item.roomInfo = _roomInfos[i];
			item.openType = eOpenType.Instant;

			scroll.InsertData(item);
		}

		scroll.UpdateAllData();

		txtmp_Caution.gameObject.SetActive(_roomInfos.Length <= 0);
	}

	#endregion
}