using System.Collections.Generic;
using db;
using Office;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오피스룸 입장하기에서 추천으로 보여지는 아이템들
/// </summary>
public class Item_OfficeRoomRecommend : DynamicScrollItem_Custom
{
    #region members

    [SerializeField] OfficeModeType curOfficeType = OfficeModeType.None;

    Image img_Thumbnail;
    Image img_PasswordIcon;
    TMP_Text txtmp_RoomName;
    TMP_Text txtmp_HostName;
    TMP_Text txtmp_Number;
    TMP_Text txtmp_CreateTime;
    TMP_Text txtmp_RoomTheme;

    Button btn_OfficeItem;

    bool isPrivateRoom, isPasswordRoom;

    Panel_Office panelOfficeRoom;
    RoomInfoRes info; // 방 정보
    MeetingItemData data; // item 
    List<OfficeSpaceInfo> officeSpaceInfos = new List<OfficeSpaceInfo>();
    #endregion
    
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        
        SetInit();
    }

    protected override void Start()
    {
        base.Start();
        
    }

    private void SetInit()
    {
        img_PasswordIcon = GetUI_Img(nameof(img_PasswordIcon));
        img_Thumbnail = GetUI_Img(nameof(img_Thumbnail));
        txtmp_RoomName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomName));
        txtmp_HostName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_HostName)); // 호스트 닉네임을 못받아서 비어있는 상태여야할 듯 
        txtmp_Number = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Number)); // 사람 수 
        txtmp_CreateTime = GetUI_TxtmpMasterLocalizing(nameof(txtmp_CreateTime)); // 회의실 방 생성 시간 
        txtmp_RoomTheme = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomTheme)); // 테마

        btn_OfficeItem = GetUI_Button(nameof(btn_OfficeItem), OnClickOfficeItem);
    }


    public override void UpdateData(DynamicScrollData scrollData)
    {
        //MeetingsItem data = (MeetingsItem) scrollData;
        
        //info = data.ServerInfo;
        //meetmoduleData  = (MeetingModuleData) RealtimeUtils.GetModuleData(info, Cons.ModuleType_Meeting);
        //curOfficeType   = Util.String2Enum<eOfficeModeType>(meetmoduleData.officeType);
        
        //if(!panelOfficeRoom)
        //    panelOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(Cons.Panel_OfficeRoom);
        //officeSpaceInfos = Single.MasterData.GetOfficeSpaceInfoDatas(curOfficeType);
        
        //txtmp_HostName.text   = meetmoduleData.hostName; // 호스트 이름 
        //txtmp_CreateDate.text = meetmoduleData.createTime; // 생성 시간
        //txtmp_RoomName.text   = meetmoduleData.roomName; // 방 이름
        //txtmp_RoomTheme.text  = meetmoduleData.theme; // 테마
        //txtmp_Number.text     = meetmoduleData.currentPersonnel + "/" + meetmoduleData.personnel; // 사람 수

        //isPrivateRoom  = meetmoduleData.isPrivate;
        //isPasswordRoom = meetmoduleData.isPassword;

        //var imgName = officeSpaceInfos.FirstOrDefault(infos => infos.id == meetmoduleData.worldId)?.thumbnail; // 썸네일 적용
        //img_Thumbnail.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + "OfficeThumbnail/" + imgName);
        
        //img_PasswordIcon.gameObject.SetActive(meetmoduleData.isPassword); // 비밀번호 여부 이미지 활성/비활성
        
        base.UpdateData(scrollData);
    }
    
    /// <summary>
    /// Item 버튼을 눌렀을 때 상세보기 팝업 실행
    /// </summary>
    private void OnClickOfficeItem()
    {
        
    }

   
}
