using FrameWork.UI;
using System.Linq;

public class View_Office_CreateRoom : UIBase
{
    public eOfficeExposureType exposureType;

	protected override void Start()
	{
        base.Start();

        exposureType = eOfficeExposureType.Office;

        var data = Single.MasterData.dataOfficeExposure.GetList().Where(x => x.exposureType == (int)exposureType).ToList();

        for (int i = 0; i < data.Count; i++)
        {
            var item = Single.Resources.Instantiate<Item_OfficeCreate>(Cons.Path_Prefab_Item + typeof(Item_OfficeCreate).Name, transform);

            item.SetData(data[i].modeType);
        }
    }
}
