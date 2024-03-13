using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MEC;

public class View_Exposition_RoomList : UIBase
{
	#region Members

	ScrollView_Custom scroll;
	TMP_Text txtmp_Caution;

	#endregion



	#region Initialize

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		var prefab = Resources.Load<FancyScrollRectCell_Custom>(Cons.Path_Prefab_Item + "item_ExpositionRoomList");

		scroll = this.GetComponentInChildren<ScrollView_Custom>();
		scroll.cellPrefab = prefab.gameObject;

		txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("1168"));
	}

	#endregion



	#region Core Methods

	public void Refresh(Booth[] _booths) => Util.RunCoroutine(Co_Refresh(_booths));

	private IEnumerator<float> Co_Refresh(Booth[] _booths)
	{
		yield return Timing.WaitForOneFrame;

		List<Item_ExpositionBooth> booths = new List<Item_ExpositionBooth>();

		for (int i = 0; i < _booths.Length; i++)
		{
			var item = new Item_ExpositionBooth();

			if (_booths[i].isHide) continue;

			item.boothInfo.id = _booths[i].id;
			item.boothInfo.roomCode = _booths[i].roomCode;
			item.boothInfo.name = _booths[i].name;
			item.boothInfo.modeType = _booths[i].modeType;
			item.boothInfo.topicType = _booths[i].topicType;
			item.boothInfo.description = _booths[i].description;
			item.boothInfo.spaceInfoId = _booths[i].spaceInfoId;
			item.boothInfo.thumbnail = _booths[i].thumbnail;
			item.boothInfo.nickname = _booths[i].nickname;
			item.boothInfo.memberCode = _booths[i].memberCode;
			item.boothInfo.memberId = _booths[i].memberId;

			booths.Add(item);
		}

		scroll.UpdateData(booths.ConvertAll(x => x as Item_Data));
		scroll.JumpTo(0);

		txtmp_Caution.gameObject.SetActive(_booths.Length <= 0);
	}

	#endregion
}