using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using System.Linq;
using UnityEngine;

public class Item_ExpositionCreate : Item_RoomCreate
{
    private Booth booth;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        Util.SetMasterLocalizing(txtmp_Title, new MasterLocalData("office_mode_exhibition"));
        Util.SetMasterLocalizing(txtmp_Description, new MasterLocalData("office_mode_exhibition_desc"));
        img_Thumbnail.sprite = Single.Resources.Load<Sprite>(Cons.Path_ArzPhoneIcon + "icon_exhibition_01");
        btn_CreateResv.gameObject.SetActive(false);
    }

	/// <summary>
	/// 즉시개설
	/// </summary>
	protected override void OnClick_RoomCreate()
	{
		base.OnClick_RoomCreate();

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
						PushPopup<Popup_ExpositionRoomCreate>().Modify_ExpositionRoom(booth);
					}

					else PushPopup<Popup_ExpositionRoomInfo>().SetExpositionCardInfo(booth);
				}

				else
				{
					PushPopup<Popup_ExpositionRoomCreate>().Create_ExpositionRoom();
				}
			}
			else
			{
				DEBUG.LOG("Booth Not Found.");
			}
		});
	}

    /// <summary>
    /// 부스 생성
    /// </summary>
    public void SetButtonCreateBooth()
	{
        booth = null;

        Util.SetMasterLocalizing(txtmp_CreateNow, new MasterLocalData("office_booth_creation"));
    }

    /// <summary>
    /// 부스 수정
    /// </summary>
    /// <param name="_booth"></param>
    public void SetButtonModifyBooth(Booth _booth)
	{
        booth = _booth;

        Util.SetMasterLocalizing(txtmp_CreateNow, new MasterLocalData("office_booth_info_permute"));
    }

    /// <summary>
    /// 부스 이동
    /// </summary>
    /// <param name="_booth"></param>
    public void SetButtonMoveToBooth(Booth _booth)
    {
        booth = _booth;

        Util.SetMasterLocalizing(txtmp_CreateNow, new MasterLocalData("office_booth_move_mybooth"));
    }
}
