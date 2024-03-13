using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Linq;

public class Popup_Map : Popup_Basic
{
	MinimapTeleport minimapTeleport;

	TMP_Text txtmp_Location;
	TMP_Text txtmp_Description;
	TMP_Text txtmp_Teleport;

	Image img_Location;

	Button btn_Teleport;
	Button btn_Close;

	ScrollRect scrollRect_BrandList;

	bool isTeleport = false;

	protected override void OnDisable()
	{
		base.OnDisable();

		isTeleport = false;
	}

	protected override void SetMemberUI()
	{
		popupAnimator = GetComponent<Animator>();

		btn_Teleport = GetUI_Button(nameof(btn_Teleport), OnClick_Teleport);
		btn_Close = GetUI_Button(nameof(btn_Close), OnClick_Close);
		img_Location = GetUI_Img(nameof(img_Location));

		txtmp_Teleport = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Teleport), new MasterLocalData("common_move"));

		scrollRect_BrandList = Util.Search<ScrollRect>(this.gameObject, nameof(scrollRect_BrandList));
	}

	private void OnClick_Teleport()
	{
		if (isTeleport) return;

		Debug.Log("TELEPORT");

		var panel = GetPanel<Panel_Map>();

		SceneLogic.instance.PopPopup();

		var position = new Vector3(minimapTeleport.posX, minimapTeleport.posY, minimapTeleport.posZ);
		var euler = new Vector3(0f, minimapTeleport.eulerY, 0f);

		panel.SetTeleportPoint(position, euler);

		panel.Teleport();

		isTeleport = true;
	}

	private void OnClick_Close()
	{
		SceneLogic.instance.PopPopup();
	}

	public void Init(MinimapTeleport _minimapTeleport)
	{
		minimapTeleport = _minimapTeleport;

		var path = "Addressable/Minimap/" + _minimapTeleport.image;
		img_Location.sprite = Resources.Load<Sprite>(path);

		txtmp_Location = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Location), new MasterLocalData(_minimapTeleport.name));
		txtmp_Description = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Description), new MasterLocalData(_minimapTeleport.description));

		CheckBrand(_minimapTeleport.masterId);
	}

	private void CheckBrand(int _masterId)
	{
		var data = Single.MasterData.dataMapExposulBrand.GetDictionary_intstring();

		foreach (var element in data)
		{
			if (_masterId == element.Key.Item1)
			{
				names.Add(element.Key.Item2);
			}
		}

		scrollRect_BrandList.gameObject.SetActive(names.Count > 0 ? true : false);

		MakeBrandList(names);

		names.Clear();
	}


	private List<string> names = new List<string>();
	private List<GameObject> brandLists = new List<GameObject>();

	public void MakeBrandList(List<string> _names)
	{
		var content = scrollRect_BrandList.transform.Search("Content");
		content.gameObject.Children().Destroy();
		brandLists.Clear();

		for (int i = 0; i < _names.Count; i++)
		{
			var barndList = Single.Resources.Instantiate<GameObject>(Cons.Path_Prefab_Item + "item_BrandList", content);

			barndList.GetComponentInChildren<TMP_Text>().text = _names[i];
			// 로컬라이징 안되어있음

			brandLists.Add(barndList);
		}
	}
}
