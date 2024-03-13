using FrameWork.UI;
using TMPro;
using UnityEngine.UI;

public class Item_OfficeUserInfo : DynamicScrollItem_Custom
{
    private Image img_playerProfile;
    private TMP_Text txtmp_PlayerName;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        img_playerProfile = GetUI_Img(nameof(img_playerProfile)); // 일단 기본 이미지 넣기
        txtmp_PlayerName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerName));
    }

    /// <summary>
    /// 게스트용 UI 업데이트 
    /// </summary>
    /// <param name="scrollData"></param>
    public override void UpdateData(DynamicScrollData scrollData)
    {
        MeetingGuestUiUserItemData data = (MeetingGuestUiUserItemData)scrollData;
        Util.SetMasterLocalizing(txtmp_PlayerName, new MasterLocalData("1096", data.NickName));

        base.UpdateData(scrollData);
    }
}
