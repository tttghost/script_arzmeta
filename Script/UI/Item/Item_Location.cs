using FrameWork.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Location : MonoBehaviour
{
	[SerializeField] MinimapTeleport minimapTeleport;

	Button item_Location;
	TMP_Text txtmp_Location;

	private void OnEnable()
	{

	}

	private void Awake()
	{
		item_Location = this.GetComponent<Button>();
		item_Location.onClick.AddListener(OnClick_Location);

		txtmp_Location = this.transform.Search(nameof(txtmp_Location)).GetComponent<TMP_Text>();	
	}

	public void Init(MinimapTeleport _minimapTeleport)
	{
		minimapTeleport = _minimapTeleport;

		txtmp_Location = this.transform.Search(nameof(txtmp_Location)).GetComponent<TMP_Text>();

		Util.SetMasterLocalizing(txtmp_Location, new MasterLocalData(_minimapTeleport.name));
	}

	private void OnClick_Location()
	{
		var panel = SceneLogic.instance.GetPanel<Panel_Map>();

		panel.FocusCameraOnPoint(minimapTeleport);
	}
}
