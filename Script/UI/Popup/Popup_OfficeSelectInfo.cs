//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using db;
//using FrameWork.UI;
//using Office;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class Popup_OfficeSelectInfo : Popup_Basic
//{
//    #region members

//    Image img_thumbNail;
//    Image img_officeTypeIcon;
//    Image img_isPasswordIcon;
//    Image img_roomCodeCopy;

//    Button btn_close;
//    Button btn_roomCopy;
//    Button btn_enterRoom;
//    Button btn_roomCode;
    
//    TMP_Text txtmp_hostName;
//    TMP_Text txtmp_createTime;
//    TMP_Text txtmp_playerCount;
//    TMP_Text txtmp_roomName;
//    TMP_Text txtmp_description;
//    TMP_Text txtmp_roomCode;
//    TMP_Text txtmp_enterRoom;

//    Panel_OfficeRoom panelOfficeRoom;

//    ServerInfoRes info;

//    eOfficeModeType curOfficeType = eOfficeModeType.NONE;

//    int worldId = 0;
//    List<OfficeSpaceInfo> officeRoomInfos;
//    string thumbnailName;
    
//    #endregion

//    protected override void SetMemberUI()
//    {
//        popupAnimator = GetComponent<Animator>();

//        Init();
//    }

//    /// <summary>
//    /// 팝업 활성화 하기 전 등록
//    /// </summary>
//    public void Init()
//    {
//        img_thumbNail = GetUI_Img("img_thumbNail");
//        img_officeTypeIcon = GetUI_Img("img_officeTypeIcon");
//        img_isPasswordIcon = GetUI_Img("img_isPasswordIcon");
//        img_roomCodeCopy = GetUI_Img("img_roomCodeCopy");
        
//        btn_roomCopy = GetUI_Button("btn_roomCopy", CopyRoomInfo);
//        //btn_enterRoom = GetUI_Button("btn_enterRoom", OnClickEnterRoom);
//        btn_roomCode = GetUI_Button("btn_roomCode",
//            () => { img_roomCodeCopy.gameObject.SetActive(!img_roomCodeCopy.gameObject.activeSelf); });

//        txtmp_hostName = GetUI_Txtmp("txtmp_hostName");
//        txtmp_createTime = GetUI_Txtmp("txtmp_createTime");
//        txtmp_playerCount = GetUI_Txtmp("txtmp_playerCount");
//        txtmp_roomName = GetUI_Txtmp("txtmp_roomName");
//        txtmp_description = GetUI_Txtmp("txtmp_description");
//        txtmp_roomCode = GetUI_Txtmp("txtmp_roomCode");
//        txtmp_enterRoom = GetUI_Txtmp("txtmp_enterRoom", new LocalData(Cons.Local_Arzmeta, "1007")); // 입장하기
//    }

//    protected override void Start()
//    {
//        base.Start();
//        btn_close = GetUI_Button("btn_close", SceneLogic.instance.PopPopup);
//    }

//    /// <summary>
//    /// 초기화 
//    /// </summary>
//    /// <param name="info"></param>
//    public void OpenViewInit(ServerInfoRes info)
//    {
//        this.info = info;
        
//        // SetData
//        moduleData = (MeetingModuleData) RealtimeUtils.GetModuleData(info, Cons.ModuleType_Meeting);
//        if(!panelOfficeRoom)
//            panelOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(Cons.Panel_OfficeRoom);
//        var officeRoomInfos = Single.MasterData.GetOfficeSpaceInfoDatas(Util.String2Enum<eOfficeModeType>(moduleData.officeType));
        
//        txtmp_hostName.text = moduleData.hostName;
//        txtmp_createTime.text = moduleData.createTime;
//        txtmp_playerCount.text = moduleData.currentPersonnel + "/" + moduleData.personnel; // 사람 수
//        txtmp_roomName.text = moduleData.roomName;
//        txtmp_description.text = moduleData.description;
//        txtmp_roomCode.text = moduleData.roomCode.PadLeft(8, '0');

//        img_roomCodeCopy.gameObject.SetActive(false);
//        img_isPasswordIcon.gameObject.SetActive(moduleData.isPassword);

//        worldId = moduleData.worldId;
//        curOfficeType = Util.String2Enum<eOfficeModeType>(moduleData.officeType);

//        var thumbnailName = officeRoomInfos.FirstOrDefault(x => x.id == worldId)?.thumbnail;
//        SetImage(curOfficeType, thumbnailName);
//    }

//    ///// <summary>
//    ///// 초기화 
//    ///// </summary>
//    ///// <param name="info"></param>
//    //public void OpenViewInit( ServerInfoRes info )
//    //{
//    //    this.info = info;

//    //    // SetData
//    //    moduleData = (MeetingModuleData)RealtimeUtils.GetModuleData(info, Cons.ModuleType_Meeting);
//    //    if (!panelOfficeRoom)
//    //        panelOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(Cons.Panel_OfficeRoom);
//    //    var officeRoomInfos = Single.MasterData.GetOfficeSpaceInfoDatas(Util.String2Enum<eOfficeModeType>(moduleData.officeType));

//    //    txtmp_hostName.text = moduleData.hostName;
//    //    txtmp_createTime.text = moduleData.createTime;
//    //    txtmp_playerCount.text = moduleData.currentPersonnel + "/" + moduleData.personnel; // 사람 수
//    //    txtmp_roomName.text = moduleData.roomName;
//    //    txtmp_description.text = moduleData.description;
//    //    txtmp_roomCode.text = moduleData.roomCode.PadLeft(8, '0');

//    //    img_roomCodeCopy.gameObject.SetActive(false);
//    //    img_isPasswordIcon.gameObject.SetActive(moduleData.isPassword);

//    //    worldId = moduleData.worldId;
//    //    curOfficeType = Util.String2Enum<eOfficeModeType>(moduleData.officeType);

//    //    var thumbnailName = officeRoomInfos.FirstOrDefault(x => x.id == worldId)?.thumbnail;
//    //    SetImage(curOfficeType, thumbnailName);
//    //}

//    private void SetImage(eOfficeModeType officeType, string thumNailimage)
//    {
//        var iconStr = string.Empty;
//        switch (officeType)
//        {
//            case eOfficeModeType.MEETING:
//                // Debug.Log("회의실 아이콘 설정");
//                iconStr = "icon_meeting_01";
//                break;
//            case eOfficeModeType.LECTURE:
//                // Debug.Log("강의실 아이콘 설정");
//                iconStr = "icon_lecture_01";
//                break;
//            case eOfficeModeType.CONFERENCE:
//                // Debug.Log("컨퍼런스룸 아이콘 설정");
//                iconStr = "icon_conference_01";
//                break;
//        }
        
//        img_officeTypeIcon.sprite = Single.Resources.Load<Sprite>(Cons.Path_ArzPhoneIcon + iconStr);
//        img_thumbNail.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + "OfficeThumbnail/" + thumNailimage);
//    }

//    //private void OnClickEnterRoom()
//    //{
//    //    ConnectServer();
//    //}

//    // 팝업에서 들어갈 수 있어야됨
//    //private void ConnectServer()
//    //{
//    //    panelOfficeRoom.PasswordCheckEnterRoom(info);
//    //}

//    StringBuilder sb = new StringBuilder();

//    private void CopyRoomInfo()
//    {
//        sb.Clear();
//        sb.Clear();
//        sb.AppendLine($"{moduleData.hostName} 방으로 초대합니다!");
//        sb.AppendLine($"제목 : {moduleData.roomName}");
//        sb.AppendLine($"설명 : {moduleData.description}");
//        sb.AppendLine($"테마 : {moduleData.theme}");
//        sb.AppendLine($"일시 : {moduleData.createTime}");
//        sb.AppendLine("----------------------------------------");
//        sb.AppendLine($"룸 코드 : {moduleData.roomCode.PadLeft(8, '0')}");
//        if (moduleData.isPassword)
//        {
//            sb.AppendLine("비밀번호 : 비밀번호는 입장 후 확인 가능");
//        }

//        sb.AppendLine("----------------------------------------");

//        UniClipboard.SetText(sb.ToString());
//    }
//}