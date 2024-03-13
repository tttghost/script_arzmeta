using db;
using FrameWork.UI;
using Office;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_OfficeSpaceSelect : UIBase
{
    private GameObject go_Observer;
    private TMP_Text txtmp_OfficeSpaceTitle;
    private TMP_Text txtmp_Sit_;
    private TMP_Text txtmp_User_;
    private TMP_Text txtmp_Observer_;
    private TMP_Text txtmp_Sit;
    private TMP_Text txtmp_User;
    private TMP_Text txtmp_Observer;
    private TMP_Text txtmp_OfficeSpaceName;
    private TMP_Text txtmp_OfficeSpaceDesc;
    private TMP_Text txtmp_OfficeSpaceExit;
    private Image img_Thumbnail;
    private Button btn_OfficeSpaceLeft;
    private Button btn_OfficeSpaceRight;
    private Button btn_OfficeSpaceExit;
    
    private TogglePlus togplus_OfficeSpaceSelect;
    private int chooseOfficeSpaceIdx; //고른 오피스공간 인덱스
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_Observer = GetChildGObject(nameof(go_Observer));

        txtmp_OfficeSpaceTitle = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceTitle), new MasterLocalData("office_title_space_select"));
        txtmp_Sit_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Sit_), new MasterLocalData("office_room_sitcapacity"));
        txtmp_User_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_User_), new MasterLocalData("office_room_maxcapacity"));
        txtmp_Observer_ = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Observer_), new MasterLocalData("office_maxobserver"));
        txtmp_Sit = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Sit));
        txtmp_User = GetUI_TxtmpMasterLocalizing(nameof(txtmp_User));
        txtmp_Observer = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Observer));
        txtmp_OfficeSpaceName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceName));
        txtmp_OfficeSpaceDesc = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceDesc));
        txtmp_OfficeSpaceExit = GetUI_TxtmpMasterLocalizing(nameof(txtmp_OfficeSpaceExit), new MasterLocalData("common_back"));
        img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));
        btn_OfficeSpaceLeft = GetUI_Button(nameof(btn_OfficeSpaceLeft), () => OnClick_PreviewOfficeSpace(--this.chooseOfficeSpaceIdx));
        btn_OfficeSpaceRight = GetUI_Button(nameof(btn_OfficeSpaceRight), () => OnClick_PreviewOfficeSpace(++this.chooseOfficeSpaceIdx));
        btn_OfficeSpaceExit = GetUI_Button(nameof(btn_OfficeSpaceExit), () => gameObject.SetActive(false));

        togplus_OfficeSpaceSelect = GetUI<TogglePlus>(nameof(togplus_OfficeSpaceSelect));
        togplus_OfficeSpaceSelect.SetToggleAction(() =>
        {
            togplus_OfficeSpaceSelect.tog.interactable = false;
            OnClick_PreviewOfficeSpaceSelect();
        }, () =>
        {
            togplus_OfficeSpaceSelect.tog.interactable = true;
        });
    }


    /// <summary>
    /// 오피스공간 미리보기
    /// </summary>
    public void OnClick_PreviewOfficeSpace(int chooseOfficeSpaceIdx)
    {
        Popup_OfficeRoomCreate popup_OfficeRoomCreate = GetPopup<Popup_OfficeRoomCreate>();
        List<OfficeSpaceInfo> officeRoomInfoList = popup_OfficeRoomCreate.officeSpaceInfoList;
        this.chooseOfficeSpaceIdx = (int)Mathf.Repeat(chooseOfficeSpaceIdx, officeRoomInfoList.Count); //선택
        OfficeSpaceInfo officeRoomInfo = officeRoomInfoList[this.chooseOfficeSpaceIdx];

        //데이터 셋업
        img_Thumbnail.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + "OfficeThumbnail/" + officeRoomInfo.thumbnail);
        togplus_OfficeSpaceSelect.SetToggleIsOn(popup_OfficeRoomCreate.selectOfficeSpaceIdx == this.chooseOfficeSpaceIdx);
        txtmp_Sit.text = officeRoomInfo.sitCapacity.ToString("00");
        txtmp_User.text = officeRoomInfo.maxCapacity.ToString("00");
        txtmp_Observer.text = officeRoomInfo.maxObserver.ToString("00");
        go_Observer.SetActive(popup_OfficeRoomCreate.officeGradeAuthority.isObserver == 1);
        Util.SetMasterLocalizing(txtmp_OfficeSpaceName, new MasterLocalData(officeRoomInfo.spaceName));
        Util.SetMasterLocalizing(txtmp_OfficeSpaceDesc, new MasterLocalData(officeRoomInfo.description));    
    }

    /// <summary>
    /// 오피스공간 선택(체크)
    /// </summary>
    /// <param name="previewOfficeSpace"></param>
    public void OnClick_PreviewOfficeSpaceSelect()
    {
        Popup_OfficeRoomCreate popup_OfficeRoomCreate = GetPopup<Popup_OfficeRoomCreate>();
        popup_OfficeRoomCreate.OnValueChanged_Space(chooseOfficeSpaceIdx);
        //popup_OfficeRoomCreate.ChangeView("");
    }
}