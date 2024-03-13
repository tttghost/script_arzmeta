using System.Text;
using FrameWork.UI;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Protocol;
using Newtonsoft.Json;

public class Popup_OfficeUserInfo : PopupBase
{
    public View_OfficeUserInfo viewOfficeUserInfo { get; private set; }
    public View_OfficeObserverInfo viewOfficeObserverInfo { get; private set; }
    public View_OfficeWaitPlayers viewOfficeWaitePlayers { get; private set; }

    private TMP_Text txtmp_AdminNickName;
    private Image img_Thumbnail_Admin;

    private TogglePlus togplus_OfficeUserInfo = null;
    private TogglePlus togplus_OfficeObserverInfo = null;
    private TogglePlus togplus_OfficeWaitPlayers = null;

    private TMP_Text txtmp_CheckUser = null;
    private TMP_Text txtmp_CheckObserver = null;
    private TMP_Text txtmp_CheckWaitPlayer = null;

    private StringBuilder sbTemp = new StringBuilder();

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        // 관리자 이름
        txtmp_AdminNickName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_AdminNickName));

        // 관리자 Thumbnail
        img_Thumbnail_Admin = GetUI_Img(nameof(img_Thumbnail_Admin));

        viewOfficeUserInfo = GetView<View_OfficeUserInfo>();
        viewOfficeObserverInfo = GetView<View_OfficeObserverInfo>();
        viewOfficeWaitePlayers = GetView<View_OfficeWaitPlayers>();

        togplus_OfficeUserInfo = GetUI<TogglePlus>(nameof(togplus_OfficeUserInfo));
        togplus_OfficeUserInfo.SetToggleOnAction(() => ChangeView<View_OfficeUserInfo>());

        togplus_OfficeObserverInfo = GetUI<TogglePlus>(nameof(togplus_OfficeObserverInfo));
        togplus_OfficeObserverInfo.SetToggleOnAction(() => ChangeView<View_OfficeObserverInfo>());

        togplus_OfficeWaitPlayers = GetUI<TogglePlus>(nameof(togplus_OfficeWaitPlayers));
        togplus_OfficeWaitPlayers.SetToggleOnAction(() => ChangeView<View_OfficeWaitPlayers>());

        GetUI_Button("btn_Close", Back);

        txtmp_CheckUser = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CheckUser));
        txtmp_CheckObserver = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CheckObserver));
        txtmp_CheckWaitPlayer = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CheckWaitPlayer));
    }

    protected override void OnEnable()
    {
        SetOpenStartCallback(() => togplus_OfficeUserInfo.SetToggleIsOn(true));

        if (Util.UtilOffice.IsLectureRoom())
        {
            togplus_OfficeObserverInfo.gameObject.SetActive(true);
            togplus_OfficeWaitPlayers.gameObject.SetActive(true);
        }
        else
        {
            togplus_OfficeObserverInfo.gameObject.SetActive(false);
            togplus_OfficeWaitPlayers.gameObject.SetActive(true);
        }
    }

    // 참여자수, 참여자 최대인원, 관전자수, 관전자최대인원, 대기인원
    public void SetToogleText(S_OFFICE_GET_ROOM_INFO officeroominfo)
    {
        Util.SetMasterLocalizing(txtmp_CheckUser, new MasterLocalData(
            "office_attendance_check", officeroominfo.CurrentPersonnel, officeroominfo.Personnel));

        Util.SetMasterLocalizing(txtmp_CheckObserver, new MasterLocalData(
            "office_observer_check", officeroominfo.CurrentObserver, officeroominfo.Observer));

        Util.SetMasterLocalizing(txtmp_CheckWaitPlayer, new MasterLocalData(
            "office_waitlist_check", officeroominfo.CurrentWaiting));
    }

    public void UpdateUI_Admin(int autthority, UserData userdata, Dictionary<string, int> avatarDatas)
    {
        // 마스터 권한 + 닉네임 설정
        if(txtmp_AdminNickName != null)
        {
            var localAuthority = Util.UtilOffice.GetMasterLocal_OfficeAutority(autthority);
            sbTemp.Clear();
            sbTemp.Append(localAuthority);
            sbTemp.Append(" : ");
            sbTemp.Append(userdata.Nickname);
            Util.SetMasterLocalizing(txtmp_AdminNickName, sbTemp.ToString());
        }

        // 마스터 썸네일 설정
        if(img_Thumbnail_Admin != null)
        {
            LocalPlayerData.Method.GetAvatarSprite(
                userdata.OwnerId, avatarDatas,
                (sprite) => img_Thumbnail_Admin.sprite = sprite);
        }
    }
}

