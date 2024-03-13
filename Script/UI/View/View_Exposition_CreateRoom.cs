using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using FrameWork.UI;
using System.Linq;

public class View_Exposition_CreateRoom : UIBase
{
	private Item_ExpositionCreate item_ExpositionCreate;

	protected override void Start()
	{
		base.Start();

		LoadItem();
	}
	protected override void OnEnable()
	{
		base.OnEnable();

		RefreshExpositionItem();
	}
	private void LoadItem()
	{
		var item_OfficeCreate = Single.Resources.Instantiate<Item_OfficeCreate>(Cons.Path_Prefab_Item + typeof(Item_OfficeCreate).Name, transform);
		item_OfficeCreate.SetData((int)OfficeModeType.Meeting);
		item_OfficeCreate.btn_CreateResv.gameObject.SetActive(false);
		item_OfficeCreate.RemoveCreateOnClick();
		item_OfficeCreate.AddCreateOnClick(() => SceneLogic.instance.PushPanel<Panel_Office>());

		item_ExpositionCreate = Single.Resources.Instantiate<Item_ExpositionCreate>(Cons.Path_Prefab_Item + typeof(Item_ExpositionCreate).Name, transform);
		item_ExpositionCreate.gameObject.SetActive(true);
	}

	private void RefreshExpositionItem()
	{
		Single.Web.CSAF.GetCSAFBooths((res) =>
		{
			if ((WEBERROR)res.error != WEBERROR.NET_E_SUCCESS)
			{
				return;
			}

			if (res.booths != null)
			{
				var booth = res.booths.FirstOrDefault(booth => booth.memberCode == LocalPlayerData.MemberCode);

				if (booth != null)
				{
					if (booth.isHide)
					{
						item_ExpositionCreate.SetButtonModifyBooth(booth);
					}

					else item_ExpositionCreate.SetButtonMoveToBooth(booth);
				}

				else
				{
					item_ExpositionCreate.SetButtonCreateBooth();
				}
			}
			else
			{
				DEBUG.LOG("Booth Not Found.");
			}
		});
	}
}