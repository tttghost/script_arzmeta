using UnityEngine;
using VLB;

public static class Cons
{
	#region Time : 시간
	public static readonly WaitForFixedUpdate Time_FrameRateFixedUpdate = new WaitForFixedUpdate();
	public static readonly WaitForSeconds Time_FrameRate = new WaitForSeconds(0.012f);
	public static readonly WaitForSeconds Time_FrameRateMove = new WaitForSeconds(0.012f);
	public static readonly WaitForSeconds Time_Sec_0D01 = new WaitForSeconds(0.01f);
	public static readonly WaitForSeconds Time_Sec_0D03 = new WaitForSeconds(0.03f);
	public static readonly WaitForSeconds Time_Sec_0D05 = new WaitForSeconds(0.05f);
	public static readonly WaitForSeconds Time_Sec_0D1 = new WaitForSeconds(0.1f);
	public static readonly WaitForSeconds Time_Sec_0D2 = new WaitForSeconds(0.2f);
	public static readonly WaitForSeconds Time_Sec_0D3 = new WaitForSeconds(0.3f);
	public static readonly WaitForSeconds Time_Sec_0D4 = new WaitForSeconds(0.4f);
	public static readonly WaitForSeconds Time_Sec_0D5 = new WaitForSeconds(0.5f);
	public static readonly WaitForSeconds Time_Sec_1 = new WaitForSeconds(1f);
	public static readonly WaitForSeconds Time_Sec_1D5 = new WaitForSeconds(1.5f);
	public static readonly WaitForSeconds Time_Sec_2 = new WaitForSeconds(2f);
	public static readonly WaitForSeconds Time_Sec_2D5 = new WaitForSeconds(2.5f);
	public static readonly WaitForSeconds Time_Sec_3 = new WaitForSeconds(3f);
	public static readonly WaitForSeconds Time_Sec_3D5 = new WaitForSeconds(3.5f);
	public static readonly WaitForSeconds Time_Sec_5 = new WaitForSeconds(5f);
	public static readonly WaitForSeconds Time_Sec_10 = new WaitForSeconds(10f);
	public static readonly WaitForSeconds Time_Sec_20 = new WaitForSeconds(20f);
	public static readonly WaitForEndOfFrame Time_WaitForEndOfFrame = new WaitForEndOfFrame();
	#endregion


	#region Color : 색상
	public static readonly Color Color_Red = new Color(0.89f, 0.24f, 0.13f, 1f);
	public static readonly Color Color_Orange = new Color(1f, 0.58f, 0.29f, 1f);
	public static readonly Color Color_Green = new Color(0.03f, 0.89f, 0.04f, 1f);
	public static readonly Color Color_MintGreen = new Color(0f, 0.91f, 0.74f, 1f);
	public static readonly Color Color_HotPink = new Color(0.9f, 0.29f, 0.57f, 1f);
	public static readonly Color Color_Blue = new Color(0.23f, 0.33f, 1f, 1f);
	public static readonly Color Color_Indigo = new Color(0.08f, 0.17f, 0.49f, 1f);
	public static readonly Color Color_Yellow = new Color(1f, 0.75f, 0.22f, 1f);
	public static readonly Color Color_Pink = new Color(0.92f, 0.1f, 0.9f, 1f);
	public static readonly Color Color_LightGreen = new Color(0.5f, 1f, 0.73f, 1f);
	public static readonly Color Color_Ivory = new Color(1f, 0.91f, 0.79f, 1f);
	public static readonly Color Color_Gray = new Color(0.53f, 0.53f, 0.53f, 1f);
	public static readonly Color Color_Brown = new Color(0.64f, 0.16f, 0.16f, 1f);
	public static readonly Color Color_WhiteGray = new Color(0.78f, 0.78f, 0.78f, 1f);
	public static readonly Color Color_White = new Color(1f, 1f, 1f, 1f);
	public static readonly Color Color_Black = new Color(0f, 0f, 0f, 1f);
	public static readonly Color Color_DarkGray = new Color(0.19f, 0.19f, 0.19f, 1f);
	public static readonly Color Color_SkyPink = new Color(0.97f, 0.77f, 0.77f, 1f);
	public static readonly Color Color_SkyBlue = new Color(0.77f, 0.97f, 0.97f, 1f);
	public static readonly Color Color_CobaltBlue = new Color(0.38f, 0.77f, 0.89f, 1f);
	public static readonly Color Color_Alpha200_White = new Color(1f, 1f, 1f, 0.78f);
	public static readonly Color Color_Purple = new Color(0.54f, 0.41f, 0.87f, 1f);

	public static readonly Color Color_CodeGateBlue = new Color(0f, 0.75f, 1f, 1f);
    public static readonly Color codegateColor = new Color(0.17f, 0.56f, 1f); //코게
    public static readonly Color jagalchiColor = new Color(0.46f, 0.79f, 1f); //자갈치

    public static readonly Color32 ChatColor_White = new Color32(255, 255, 255, 255);
    public static readonly Color32 ChatColor_Green = new Color32(50, 145, 0, 255);
    public static readonly Color32 ChatColor_Yellow = new Color32(219, 183, 0, 255);
    #endregion


    #region String : 단순 스트링값
    //Sound
    public const string click = "click";

    // 버튼음
    public const string effect_btn_0 = "effect_btn_0";
    public const string effect_btn_1 = "effect_btn_1";
    public const string effect_btn_2 = "effect_btn_2";
    public const string effect_warn_0 = "effect_warn_0";
    public const string effect_info_0 = "effect_info_0";
    public const string effect_poshit_1 = "effect_poshit_1";
    public const string effect_poshit_2 = "effect_poshit_2";

    // 점프
    public const string effect_jump_ = "effect_jump_";
    public const string effect_jump_0 = "effect_jump_0";
    public const string effect_jump_1 = "effect_jump_1";
    public const string effect_jump_2 = "effect_jump_2";
    public const string effect_jump_3 = "effect_jump_3";
    public const string effect_jump_4 = "effect_jump_4";

    // 2단 점프
    public const string effect_hjump_ = "effect_hjump_";
    public const string effect_hjump_0 = "effect_hjump_0";
    public const string effect_hjump_1 = "effect_hjump_1";
    public const string effect_hjump_2 = "effect_hjump_2";
    public const string effect_hjump_3 = "effect_hjump_3";

    // 바다 빠짐
    public const string effect_dive_0 = "effect_dive_0";

    // 대쉬
    public const string effect_dash = "Dash_03";

    //BGM
    public const string bgm_game_0 = "bgm_game_0";
    public const string bgm_lobby_0 = "bgm_lobby_0";
    public const string bgm_theme_0 = "bgm_theme_0";
    public const string bgm_world_0 = "bgm_world_0";

    public const string Setting_Quality = "Setting_Quality";
    public const string Setting_Language = "Setting_Language";
    public const string Setting_VolumeMaster = "Setting_VolumedMaster";
    public const string Setting_VolumeBGM = "Setting_VolumeBGM";
    public const string Setting_VolumeEffect = "Setting_VolumeEffect";
    public const string Setting_VolumeMedia = "Setting_VolumeMedia";

    //Addessable
    public const string AddressablePath_Use_Asset_Database = "Use Asset Database (fastest)";
    public const string AddressablePath_Simulate_Groups = "Simulate Groups (advanced)";
    public const string AddressablePath_Use_Existing_Build = "Use Existing Build (requires built groups)";

    // Common
    public const string PlayerController = "PlayerController";
    public const string MyCamera = "PlayerController/CameraRig/TrackingSpace/CenterEyeAnchor";

    // Player Camera
    public const string CAMERAPARENT = "CameraParent";
    public const string PLAYER_PARENT = "PlayerParent";
    public const string AGORAUSER = "AgoraUser";
    public const string HAIR = "Hair";
    public const string TOP = "Top";
    public const string BOTTOM = "Bottom";
    public const string MAINCAMERA = "MainCamera";
    public const string PLAYER_CAMERAROOT = "PlayerCameraRoot";
    public const string NICKNAME = "NickName";
    public const string AVATARPARTS = "AvatarParts";
    public const string HUDPARENT = "HUDParent";
    public const string RIGHTHAND = "Bip001 R Hand";
    public const string LEFTHAND = "Bip001 L Hand";

    // MyPlayer controllers
    public const string TPSController = "ThirdPersonController";
    public const string AvatarPartsController = "AvatarPartsController";

    // TPS Controllers
    public const string CharacterController = "CharacterController";
    public const string DashController = "CharacterDashController";
    public const string CharacterParticleController = "CharacterParticleController";
    public const string FallController = "CharacterFallController";
    public const string VolumeController = "CharacterVolumeController";

    // Player Input
    public const string StarterAssetsInputs = "StarterAssetsInputs";
    public const string PlayerInput = "PlayerInput";
    public const string Animator = "Animator";

    public const string LeanPinchCamera = "go_LeanPinch";
    public const string ViewJoyStickZone = "view_JoystickZone";
    public const string ViewChat = "view_Chat";
    public const string ViewChat02 = "View_Chat_02";
    public const string ViewTop = "view_Top";
    public const string ViewTouch = "view_TouchZone";

    public const string JoyStickMove = "joystick_Move";
    public const string JoyStickJump = "joystick_Jump";
    public const string JoyStickDash = "joystick_Dash";
    public const string JoyStickLeft = "joystick_left";
    public const string joystick_Vehicle_Left = "joystick_Vehicle_Left";
    public const string joystick_Vehicle_Right = "joystick_Vehicle_Right";

    // Player Animation
    public const string Movement = "Movement";
    public const string Grounded = "Grounded";
    public const string Jump = "Jump";
    public const string Dash = "Dash";
    public const string FreeFall = "FreeFall";



    //Input
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";

    public const string Toast = "Toast";

    #endregion


    #region Prefab : 프리팹명

    // Base
    public const string PanelBase = "PanelBase";

	// Basic
	public const string Popup_Basic = "Popup_Basic";

	// Dummy
	public const string Panel_Empty = "Panel_Empty";
	public const string Popup_Dummy = "Popup_Dummy";

	// LogoScene
	public const string Panel_HancomLogo = "Panel_HancomLogo";
	public const string Panel_ArzmetaLogo = "Panel_ArzmetaLogo";
	public const string Panel_HancomForntisLogo = "Panel_HancomForntisLogo";

	// TitleScene
	public const string Panel_Title = "Panel_Title";
	public const string Panel_SocialLogin = "Panel_SocialLogin";
	public const string Panel_ArzLogin = "Panel_ArzLogin";
	public const string Panel_CreateAccount = "Panel_CreateAccount";
	public const string Panel_FindPassword = "Panel_FindPassword";
	public const string Panel_DormantAccount = "Panel_DormantAccount";

	// LobbyScene
	public const string Panel_Preset = "Panel_Preset";
	public const string Panel_ArzProfile_Create = "Panel_ArzProfile_Create";
	public const string Panel_ArzProfile_Check = "Panel_ArzProfile_Check";

	// Game 관련
	public const string Panel_MultiGame = "Panel_MultiGame";
	public const string Panel_SingleGame = "Panel_SingleGame";
	public const string Panel_InfinityCode = "Panel_InfinityCode";

	// 
	public const string Panel_LinkAccount = "Panel_LinkAccount";
	public const string Panel_ChatBot = "Panel_ChatBot";

	// Popup
	public const string Popup_ScrollView = "Popup_ScrollView";
	public const string Popup_CommercePurchase = "Popup_CommercePurchase";
	public const string Popup_PayArowanaToken = "Popup_PayArowanaToken";
	public const string Popup_CompletePayment = "Popup_CompletePayment";
	public const string Popup_TermsOfService = "Popup_TermsOfService";
	public const string Popup_BrandList = "Popup_BrandList";
	public const string Popup_PlayerInfo = "Popup_PlayerInfo";
	public const string Popup_ConvertAccount = "Popup_ConvertAccount";
	public const string Popup_AddFriend = "Popup_AddFriend";
	public const string Popup_BlockFriend = "Popup_BlockFriend";
	public const string Popup_ReqRecFriend = "Popup_ReqRecFriend";
	public const string Popup_ShowBizCard = "Popup_ShowBizCard";

	// Toast
	public const string Toast_Basic = "Toast_Basic";
	public const string Toast_TwoBtn = "Toast_TwoBtn";

	// Setting
	public const string View_System = "View_System";
	public const string View_Account = "View_Account";

	public const string View_QuestComplete = "View_QuestComplete";

	// Office
	public const string View_EnterRoom_Office	= "View_EnterRoom_Office";
	public const string View_CreateRoom_Office		= "View_CreateRoom_Office";
	public const string View_Office_Reservation	 = "View_Office_Reservation";

	public const string View_OfficeRoomReservationSelect = "View_OfficeRoomReservationSelect";
	public const string View_MedicineRoomEnter = "View_MedicineRoomEnter";

	public const string Popup_OfficeSelectInfo = "Popup_OfficeSelectInfo";
	public const string Popup_OfficeRoomInfo = "Popup_OfficeRoomInfo";
	public const string Popup_OfficeUserInfo = "Popup_OfficeUserInfo";
	public const string Popup_OfficePassword = "Popup_OfficePassword";

	public const string View_OfficeWaitPlayers = "View_OfficeWaitPlayers";
	public const string View_OfficeUserInfo = "View_OfficeUserInfo";

	// BusinessCard
	public const string View_BizCard_Add = "View_BizCard_Add";
	public const string View_BizCard_Add_Mini = "View_BizCard_Add_Mini";
	public const string View_ArzProfile = "View_ArzProfile";
	public const string View_ArzProfile_Edit = "View_ArzProfile_Edit";
	public const string View_BizCard_A = "View_BizCard_A";
	public const string View_BizCard_A_Mini = "View_BizCard_A_Mini";
	public const string View_BizCard_A_Edit = "View_BizCard_A_Edit";
	public const string togplus_BizCard = "togplus_BizCard";

	public const string MASTER_OFFICEROOM_ENTER = "office_room_enter";
	public const string MASTER_OFFICEROOM_TITLE_SET = "office_title_room_set";
	public const string MASTER_OFFICE_RESERVATION = "office_my_reservation";

	// Panel_Friend 관련
	public const string View_FriendList = "View_FriendList";
	public const string View_FriendManage = "View_FriendManage";
	public const string View_ArzTalk = "View_ArzTalk";
	public const string View_SearchFriend = "View_SearchFriend";
	public const string View_BlockList = "View_BlockList";

	// Panel_Notice : 공지사항
	public const string Panel_Notice = "Panel_Notice";

	// 스크롤뷰용 애니메이터 해시 값
	public const string Ani_CommerceScroll = "Ani_CommerceScroll"; // 커머스존 스크롤뷰
	public const string Ani_PresetScroll = "Ani_PresetScroll"; // 프리셋 스크롤뷰
	public const string Ani_VerticalScroll = "Ani_VerticalScroll";

	#region KTMF
	// Panel
	public const string Panel_KTMFVote = "Panel_KTMFVote";
	
	// Popup
	public const string Popup_KTMFVote = "Popup_KTMFVote";
	
	// Toast
	public const string Toast_KTMFVote = "Toast_KTMFVote";

	// View
	public const string View_KTMFVote = "View_KTMFVote";
	public const string View_KTMFVoteResult = "View_KTMFVoteResult";

	// Item
	public const string Item_KTMFPage = "Item_KTMFPage";
	public const string Item_KTMFProfile = "Item_KTMFProfile";
	public const string item_KTMFVoteResult = "item_KTMFVoteResult";
	public const string tog_KTMFVote = "tog_KTMFVote";
	#endregion

	#endregion


	#region Resources : 순수데이터

	//ScriptableObject
	public const string SO_Editmode = "SO_Editmode";

	//Path

	//Addressable안쓰고 Resources쓰는것 임시 패스
	public const string Path_Addressable = "Addressable/";

	public const string Path_Avatar = Path_Addressable + "Avatar/";

	public const string Path_Thumbnail = Path_Addressable + "Thumbnail/";
	public const string Path_Thumbnail_consumable = Path_Thumbnail + "consumable/";
	public const string Path_Thumbnail_product = Path_Thumbnail + "product/";
	public const string Path_Thumbnail_material = Path_Thumbnail + "material/";
	public const string Path_Thumbnail_tool = Path_Thumbnail + "tool/";
	public const string Path_Thumbnail_vehicle = Path_Thumbnail + "vehicle/";
	public const string Path_Thumbnail_pet = Path_Thumbnail + "pet/";
	public const string Path_Thumbnail_other = Path_Thumbnail + "other/";
	public const string Path_Thumbnail_furniture = Path_Thumbnail + "furniture/";
	public const string Path_Thumbnail_decoration = Path_Thumbnail + "decoration/";
	public const string Path_Thumbnail_specialty = Path_Thumbnail + "specialty/";
	public const string Path_Thumbnail_floor = Path_Thumbnail + "floor/";
	public const string Path_Thumbnail_hair = Path_Thumbnail + "hair/";
	public const string Path_Thumbnail_top = Path_Thumbnail + "top/";
	public const string Path_Thumbnail_bottom = Path_Thumbnail + "bottom/";
	public const string Path_Thumbnail_onepiece = Path_Thumbnail + "onepiece/";
	public const string Path_Thumbnail_shoes = Path_Thumbnail + "shoes/";
	public const string Path_Thumbnail_accessory = Path_Thumbnail + "accessory/";

	public const string Path_AudioClip = Path_Addressable + "AudioClip/";

	public const string Path_ArzPhoneIcon = Path_Addressable + "ArzPhone/Icon/";
	public const string Path_Image = Path_Addressable + "Image/";
	public const string Path_OfficeThumbnail = Path_Image + "OfficeThumbnail/";

	public const string Path_MasterData = Path_Addressable + "MasterData/";

	public const string Path_Prefab = Path_Addressable + "Prefab/";
	public const string Path_Prefab_Common = Path_Prefab + "Common/";
	public const string Path_Prefab_GameObject = Path_Prefab + "GameObject/";
	public const string Path_Prefab_HallOfFame = Path_Prefab + "HallOfFame/";
	public const string Path_Prefab_NPC = Path_Prefab + "NPC/";
	public const string Path_Prefab_Particle = Path_Prefab + "Particle/";
	public const string Player_Prefab_Player = Path_Prefab + "Player/";
	public const string Player_Realtime = Player_Prefab_Player + "Player_Realtime";
	public const string Player_Observer = Player_Prefab_Player + "Player_Observer";
	public const string Path_Interaction = Path_Prefab + "Interaction/";
	public const string Path_Prefab_UI = Path_Prefab + "UI/";
	public const string Path_Prefab_UI_Panel = Path_Prefab_UI + "Panel/";
	public const string Path_Prefab_UI_Popup = Path_Prefab_UI + "Popup/";
	public const string Path_Prefab_UI_Toast = Path_Prefab_UI + "Toast/";
	public const string Path_Prefab_UI_View = Path_Prefab_UI + "View/";
	public const string Path_Prefab_Item = Path_Prefab + "Item/";

	public const string Path_ScriptableObject = Path_Addressable + "ScriptableObject/";
	public const string Path_Animator = Path_Addressable + "Animator/";
	public const string Path_RenderTexture = Path_Addressable + "RenderTexture/";

	// 스토리지 주소
	public const string Path_Storage_NFT = "nft/ktmf";
	public const string Path_Storage_KTMFVote = "select-vote";
	public const string Path_Storage_ExpositionVote = "booth";

	// 로컬라이제이션 카테고리
	public const string Local_Arzmeta = "Arzmeta";
	public const string Local_Quest = "Quest";
	public const string Local_NPC = "NPC";
	public const string Local_AvatarParts = "AvatarParts";
	public const string Local_InfinityCodes = "InfinityCodes";
	public const string Local_Game = "Game";
	public const string Local_OXQuiz = "OXQuiz";
	public const string Local_Terms = "Terms";

	//Layer
	public const string Layer_Default = "Default";
	public const string Layer_TransparentFX = "TransparentFX";
	public const string Layer_IgnoreRaycast = "Ignore Raycast";
	public const string Layer_PostProcessing = "Post Processing";
	public const string Layer_Water = "Water";
	public const string Layer_UI = "UI";
	public const string Layer_Ignore = "Ignore";
	public const string Player = "Player";
	public const string Layer_NPC = "NPC";
	public const string OtherPlayer = "OtherPlayer";
	public const string Layer_TouchZone = "TouchZone";
	public const string Layer_OutLine = "OutLine";
	public const string Layer_InteractionArea = "InteractionArea";
	public const string Layer_Book = "Book";
	public const string Layer_NoneInteractable = "NoneInteractable";
	public const string NonCollideable = "NoneCollideable";
	public const string Layer_Code = "Code";

	//마이룸
	public const string Path_MyRoom = "MyRoom";

	//Animation

	//Animator

	//AudioClip

	//DB

	//Font

	//Particle
	public const string EF_summon = "EF_summon";
	public const string EF_Dummy = "EF_Dummy";
	public const string EF_Star = "EF_Star";

	//Sprite

	//Video

	//Web
	public const string QUERY_SERVER_TYPE = "/Rooms?servertype=";
	public const string InnerServerUrl	 = "192.168.0.47";
	public const string RequestMakeRoomStr  = "/MakeRoom";
	//public const string InnerServerUrl = "192.168.10.168";

	public static readonly string RequestInnerServerUrl		 = $"http://{InnerServerUrl}:8080/Rooms";
	public static readonly string RequestInnerServerMakeRoom = $"http://{InnerServerUrl}:8080/MakeRoom";
    public const string Panel_MyRoomMain	= "Panel_MyRoomMain";
	public const string Panel_MyRoomControl = "Panel_MyRoomControl";
	public const string Panel_MyRoomInven	= "Panel_MyRoomInven";
	public const string Popup_ItemDetail	= "Popup_ItemDetail";
	public const string Popup_ItemSave		= "Popup_ItemSave";
	public const string Popup_ItemHistory	= "Popup_ItemHistory";

	//GameName
	public const string JUMPINGMATCHING = "JumpingMatching";
	public const string OXQUIZ = "OXQuiz";

	// Network Module type
	public const string MODULE_TYPE_BASE     = "Base";
	public const string MODULE_TYPE_MAIN     = "Main";
	public const string MODULE_TYPE_MYROOM   = "Myroom";
	public const string ModuleType_Meeting  = "Meeting";
	public const string ModuleType_Matching = "Matching";
	public const string ModuleType_OX		= "OX";
	public const string MODULE_TYPE_CHAT = "Chat";

	public const string SERVER_TYPE_ARZMETA  = "arzmeta";
	public const string ServerType_MyRoom	= "myroom";
	public const string SERVER_TYPE_MATCHING = "matching";
	public const string ServerType_Meeting  = "meeting";
	public const string SERVER_TYPE_OXQUIZ		= "ox";
	public const string ServerType_Medicine	= "medicine";

	public const string Panel_OfficeWaitRoom		= "Panel_OfficeWaitRoom";

	public const string Popup_OfficeRoomCreate      = "Popup_OfficeRoomCreate";
	public const string Popup_OfficeRoomSave        = "Popup_OfficeRoomSave";
	public const string Popup_OfficeGradeUpgrade    = "Popup_OfficeGradeUpgrade";
	public const string Popup_OfficeSetPosition		= "Popup_OfficeSetPosition";
	public const string Popup_OfficeExit			= "Popup_OfficeExit";

	public const string View_OfficeSpaceSelect	    = "View_OfficeSpaceSelect";
	public const string View_NumberInput		    = "View_NumberInput";


	// Realtime
	public const string SERVER_TYPE_MYROOM = "myroom";

	// GameZone
	public const string Popup_MultiGame = "Popup_MultiGame";
	public const string View_MultigameGuide = "View_MultigameGuide";

	public const string go_Content = "go_Content";
	public const string go_gamePreview = "go_gamePreview";

	public const string GameZone = "GameZone";
	public const string CreateSpot_FirstFloor = "CreateSpot_FirstFloor";
	public const string CreateSpot_SecondFloor = "CreateSpot_SecondFloor";
	public const string CreateSpot_EndGame = "CreateSpot_EndGame";

	public const string Big_Arcade = "Big_Arcade";

	public const string CREATE = "Create";
	public const string SEARCH = "Search";

	#endregion

	public const string Path_Resources = "Assets/_DEV/Resources/";
	public const string CustomBuildSetting = "Settings/CustomBuildSetting";
	public const string Path_CustomProjectSetting = Path_Resources + CustomBuildSetting + ".asset";

	public const string GAMEZONE = "GameZone";

	public const string EMPTY = "　";
	public const string ORIGIN = "origin";

	#region Realtime Server

	public const string SUCCESS				= "SUCCESS";
	public const string ROOM_IS_FULL		= "ROOM_IS_FULL";
	public const string ROOM_IS_SHUTDOWN	= "ROOM_IS_SHUTDOWN";
	public const string DUPLICATE			= "DUPLICATE";
	public const string WRONG_ROOM_ID		= "WRONG_ROOM_ID";
	public const string PASSWORD_FAIL		= "PASSWORD_FAIL";
	public const string KICKED				= "KICKED";
	public const string WAITING				= "WAITING";
	public const string ISOBSERVER			= "ISOBSERVER";

	public const int TIMEOUT = 1;

	public const string View_MedicineReservationInfo = "View_MedicineReservationInfo";

    #endregion


    // 심플파일브라우저 셋 필터
    public static string[] filterImage = { "", ".png", ".jpg" };
    public static string[] filterPDF = { "", ".pdf" };



}


