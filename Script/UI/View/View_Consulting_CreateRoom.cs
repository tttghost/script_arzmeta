using FrameWork.UI;
using System.Linq;

public class View_Consulting_CreateRoom : UIBase
{
	eOfficeExposureType exposureType;

	protected override void Start()
	{
		base.Start();

		exposureType = eOfficeExposureType.Consulting;

		var data = Single.MasterData.dataOfficeExposure.GetList().Where(x => x.exposureType == (int)exposureType).ToList();

		for (int i = 0; i < data.Count; i++)
		{
			var item = Single.Resources.Instantiate<Item_OfficeCreate>(Cons.Path_Prefab_Item + "Item_OfficeCreate", transform);
			item.btn_CreateResv.gameObject.SetActive(false);
			item.SetData(data[i].modeType);
		}
	}
}
