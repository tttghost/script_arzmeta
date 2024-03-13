using FrameWork.UI;
using TMPro;
using UnityEngine;

public class View_Office_RecommendRoomList : UIBase
{
	#region Members

	DynamicScrollItem_Custom itemPrefab;
    DynamicScroll_Custom scroll;

    TMP_Text txtmp_Caution;

	#endregion


	#region Initialize

	protected override void OnEnable()
	{
		base.OnEnable();

		txtmp_Caution.gameObject.SetActive(false);
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		itemPrefab = Resources.Load<DynamicScrollItem_Custom>(Cons.Path_Prefab_Item + " item_OfficeRoomRecommend");

        scroll = GetComponentInChildren<DynamicScroll_Custom>();
        scroll.itemPrefab = itemPrefab;

        txtmp_Caution = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Caution), new MasterLocalData("1168"));

    }

	#endregion
}
