public enum eNoticeExposureState
{
    reset,
    once,
    done,
}

public enum eAppInfo
{
    onfContentsInfo,
    bannerInfo,
    screenInfo,
    csafEventInfo,
    noticeInfo,
}
public enum eNoticeType
{
    입장공지이벤트 = 1,
    공지사항,
    이벤트,
}
public enum eNoticeExposureType
{
    일일일회노출= 1,
    선택노출,
    상시노출,
}
public enum eGateway
{
    DEV,
    QA,
    STAGING,
    LIVE,
}

public enum eStack
{
    PUSH,
    POP,
}

public enum eSkybox
{
    Skybox,
    Black,
    White,
}

public enum ePlayerState
{
    None,
    Idle,
    Talking,
    Shopping,
}

public enum eEmoji
{
    LIKE,
    HI,
    WOW,
    HEART,
}
public enum EPacketID
{
    NONE,
    PLAYER,
    AI,
    TEST,
}

public enum Gender
{
    Female,
    Male
}

public enum GATEWAYSTATE
{
    DEV,
    LIVE,
}

public enum RotationOnly
{
    X,
    Y,
    Z,
    None
}

public enum RoomType
{
    None,

    // Land
    Arz,
    Busan,

    // Zone
    Conference,
    Game,
    Office,
    Store,
    Vote,
    Hospital,
    Festival,
    Exposition,

    // Room
    JumpingMatching,
    OXQuiz,
    Lecture,
    Meeting,
    Consulting,
    MyRoom,
    Exposition_Booth,
}

public enum SceneName
{
    None,

    Scene_Base_Loading,
    Scene_Base_Lobby,
    Scene_Base_Logo,
    Scene_Base_Patch,
    Scene_Base_Title,

    Scene_Land_Arz,
    Scene_Land_Busan,

    Scene_Room_JumpingMatching,
    Scene_Room_OXQuiz,

    Scene_Room_Lecture,
    Scene_Room_Lecture_22Christmas,

    Scene_Room_Meeting,
    Scene_Room_Meeting_22Christmas,
    Scene_Room_Meeting_Office,

    Scene_Room_Consulting,
    Scene_Room_Exposition_Booth,

    Scene_Room_MyRoom,

    Scene_Zone_Conference,
    Scene_Zone_Festival,
    Scene_Zone_Game,
    Scene_Zone_Office,
    Scene_Zone_Exposition,
    Scene_Zone_Store,
    Scene_Zone_Vote,

    Scene_Zone_Hospital,
}

public enum MasterDataKeyType
{
    AvatarParts,
    AvatarPartsType,
    ForbiddenWords,
    LanguageType,
    MemberType,
    NotiType,
    OsType,
    ProviderType,
    Quest,
    QuestConditionType,
    QuestGroupType,
    QuestMissionType,
    QuestNameType,
    QuestOpenType,
    QuestReward,
    QuestRewardType,
    QuestStepType,
}

public enum mixerType
{
    Master,
    BGM,
    Effect,
    Media,
}
public enum infinityCode_UserType
{
    All,
    My,
}

public enum tutorial_page
{
    One,
    Two,
    Three
}
public enum eQuestState //퀘스트 진행상태
{
    Playing,
    Completed,
}
public enum eRewardState //리워드 보상받은여부 상태
{
    Locked,
    Completed,
    Received,
}
public enum ePopupPivot
{
    up,
    down,
    center,
    right,
    left,
}

public enum eFade
{
    In,
    Out,
}

public enum eScreenType // 각 씬마다의 스크린타입..
{
    Square = 1,
    Conference,
    Etc,
    BigMain,
    General,
    University,
    Junior,
}

public enum eButtonTweenState
{
    oriDown,
    downOri,
    downUp,
    upOri,
}

public enum SizeUnit
{
    B,
    KB,
    MB,
    GB,
    TB,
    PB,
    EB,
    ZB,
    YB
}

/// <summary>
/// 마이룸 모드(플레이모드,에딧모드)
/// </summary>
public enum eMyRoomMode
{
    PLAYMODE,
    EDITMODE,
}

/// <summary>
/// 세션 종류 enum
/// </summary>
//public enum eSessionType
//{
//	MAIN,
//	GAME,
//	OFFICE,
//	MYROOM,
//	CONSULTING,
//	NONE
//}

/// <summary>
/// 서버 연결 상태 enum
/// </summary>
public enum eConnectState
{
    DISCONNECTED,
    CONNECTING,
    CONNECTED,
    ERROR,
    NONE
}

/// <summary>
/// S_ENTER 및 각 모듈 Enter 상태를 위한 타입 
/// </summary>
public enum eEnterState
{
    NONE,
    PROCESSING,
    SUCCESS,
    FAIL,
    DISCONNECTED,
    WAITING,
    WAITINGREJECT
}

/// <summary>
/// 서버 모듈 타입
/// </summary>
public enum eMODULE_TYPE
{
    BASE_MODULE,
    CHAT_MODULE,
    MATCHING_MODULE,
    OX_MODULE,
    MEETING_MODULE,
    MAIN_MODULE,
    MYROOM_MODULE,
    NONE,
}
/// <summary>
/// 인터렉션
/// </summary>
public enum eINTERACTIONCUSTOM_TYPE
{
    b_p_singlebed,
    b_p_armchair,
    b_p_gamingchair,
    b_p_woodenchair,
    b_p_calmsofa,
    b_p_modernsofa,
    //SOFA_03,
    //TABLE_01,
}

public enum INTERACTION_POSITION
{
    b_p_singlebed,
    b_p_armchair,
    b_p_gamingchair,
    b_p_woodenchair,
    b_p_calmsofa,
    b_p_modernsofa,
}
public enum INTERACTION_TYPE
{
    b_p_singlebed,
    b_p_armchair,
    b_p_gamingchair,
    b_p_woodenchair,
    b_p_calmsofa,
    b_p_modernsofa,
}
public enum eINTERACTIONUNIQUE_TYPE
{
    MIRROR,
    GAME,
    MAP,
    DOOR,
    MAGAZINE,
    MAILBOX,
    FRAME,
}
public enum eROOMITEMLOCK
{
    LOCK,
    UNLOCK,
}
public enum eINVENLOCK
{
    LOCK,
    UNLOCK,
}
public enum eCONTROLLOCK
{
    LOCK,
    UNLOCK,
}

public enum Language
{
    Korean,
    English,
}

public enum eModuleType
{
    Base,       // 동기화 모듈
    Chat,       // 채팅 모듈
    Matching,   // 점핑매칭 게임 모듈
    OX,         // OX 게임 모듈
    Meeting,    // 회의실 모듈
    Main,       // 아즈메타 모듈
    Myroom,     // 마이룸 모듈
    Interaction,// 인터렉션 모듈
}

/// <summary>
/// 심심이 상태
/// </summary>
public enum eSimSimiState
{
    isidle,     //기본
    isexplane,  //대화
    isexciting, //신남
}

/// <summary>
/// 등급
/// </summary>
public enum GRADE
{
    일반 = 1,
    중급 = 2,
    상급 = 3,
    최상급 = 4,
}

/// <summary>
/// 아바타 프리셋
/// </summary>
public enum AVATAR_PRESET
{
    PRESET_1 = 1,
    PRESET_2 = 2,
    PRESET_3 = 3,
    PRESET_4 = 4
}

/// <summary>
/// 공지사항 검색 타입
/// </summary>
public enum NEWSSEARCH_TYPE
{
    None = -1,
    subject = 0,
    content = 1
}

/// <summary>
/// 서버 타입
/// </summary>
public enum SERVER_STATE
{
    ACTIVATE = 1,
    INACTIVATE = 2,
    TEST = 3,
    NEED_UPDATE = 4,
}

/// <summary>
/// 타인 정보 가져오기 타입
/// </summary>
public enum OTHERINFO_TYPE
{
    MEMBERID = 1,
    MEMBERCODE = 2
}

/// <summary>
/// 실시간서버 끊겼을때 타입
/// </summary>
public enum DISCONNECT_TYPE
{
    LEAVED,             // 그냥 나갈때
    DUPLICATED,         // 중복 로그인으로 튕길때
    SERVER_CHANGE,      // 내 스스로 서버 바꿀때
    WAITING_REJECTED,   // 오피스는 대기실이 있는데 거기에서 거절당했을 때
    CLOSING,            // 오피스에서 방장이 나가거나 방을 해산시켰을 때
    KICKED,             // 강퇴 당했을때
    DISCONNECTED,       // 끊길때, 사실상 수신 불가(끊겼으니까 수신을 못 받음)
}

/// <summary>
/// 마이룸 비/공개 상태
/// </summary>
public enum MYROOMSTATE_TYPE
{
    myroom_condition_anyone = 1,
    myroom_condition_friend = 2,
    myroom_condition_invite = 3,
    myroom_condition_nobody = 4,
}

/// <summary>
/// 내부에서 사용하는 수치값
/// </summary>
public enum FUNCTION_TYPE
{
    REQUEST_FRIEND_MAX = 1001, // 보낸 친구 신청 최대값
    RICEVE_FRIEND_MAX = 1002, // 받은 친구 신청 최대값
    TOTAL_FRIEND_MAX = 1003, // 친구 최대 수

    BIZCARD_DEFAULT = 2011, // 명함 기본 보유 개수
    BIZCARD_MAX = 2012, // 명함 최대 보유 개수

    OFFICE_ENTER_TIME = 2021, // 오피스 룸 시작 전 입장 가능 시간(분)

    NOMAL_INVEN_SLOT_MAX = 3011, // 일반 인벤토리 최대 슬롯 개수
    FURNITURE_INVEN_SLOT_MAX = 3012, // 가구 인벤토리 최대 슬롯 개수
    COSTUME_INVEN_SLOT_MAX = 3013, // 코스튬 인벤토리 최대 슬롯 개수

    MAXIMUM_IMAGE_UPLOAD_CAPACITY = 3021, // 이미지 최대 업로드 용량
    MAXIMUM_IMAGE_UPLOAD_RESOLUTION = 3022, // 이미지 최대 업로드 해상도

    EMAIL_AUTH_MAX = 4001, // 이메일 및 본인인증 횟수 제한
}

/// <summary>
/// 다이나믹 링크 타입
/// </summary>
public enum SHARELINK_TYPE
{
    OFFIICE_INFO,
    OFFICE_ENTER,
    MYROOM_ENTER,
}

public enum FRAMEIMAGEAPPEND_TYPE
{
    로컬이미지 = 1,
    URL이미지 = 2,
}

public enum FRAME_KIND
{
    b_p_sframea = 1,
    b_p_sframeb = 2,
    b_p_sframec = 3,
}

public enum WILDCARD_TYPE
{
    MYROOM_FRAME,
    EXPOSITION_BANNER,
    EXPOSITION_SCREEN,
}

public enum eDisk
{
    KB = 1,
    MB = 2,
    GB = 3,
    TB = 4,
}

public enum eScreenContentType // 미디어 상태
{
    none = 0, //일반영상
    storage = 1, //스토리지영상
    youtubeNormal = 2, //유튜브 일반영상
    youtubeLive = 3, //유튜브 라이브영상
}

public enum ARZMETA_HOMEPAGE_TYPE 
{
    Center = 1, // 고객 센터
    Report = 2, // 신고하기
    KTMF_Email_Kr = 3, // KTMF 이메일 약관동의 한국어 페이지
    KTMF_Email_En = 4, // KTMF 이메일 약관동의 영어 페이지
}

/// <summary>
/// 심플파일브라우저 셋 필터
/// </summary>

public enum FILEBROWSER_SETFILTER
{
    IMAGE,
    PDF,
}

#region 인터렉션 관련 enum
public enum INTERACTION_ACTION
{
    interaction_sit,
    interaction_sleep,
    action_sit,
    action_laydown,
    action_stand,
    action_sleep_stand_up,
}

public enum INTERACTION_TYPE2
{
    sit,
    sleep,
}
#endregion

#region 친구 관련 enum
/// <summary>
/// 친구 관련 타입
/// </summary>
public enum FRIEND_TYPE
{
    NONE, // 전부 끔
    ADD, // 친구 신청 하기
    BLOCK, // 친구 차단 하기
    UNBLOCK, // 친구 차단 해제 하기
    RECIVE, // 친구 요청 받은 상태
    REQUEST, // 친구 요청 보낸 상태
    DELETE, // 친구 삭제 하기
    REPORT // 신고하기
}

/// <summary>
/// 친구 검색 시 결과 유무
/// </summary>
public enum FRIENDRESULT_TYPE
{
    NONE,
    RESULT,
    NON_RESULT
}

/// <summary>
/// 친구 검색 타입
/// </summary>
public enum FRIENDREQUEST_TYPE
{
    NICKNAME = 1,
    MEMBERCODE = 2,
}
#endregion

#region 오피스 관련 enum
public enum OfficeModeType
{
    Meeting = 1,
    Lecture,
    Conference,
    Consulting,
    None,
}

public enum eOfficeGradeType
{
    Basic = 1,
    Pro = 2,
}
public enum eOpenType
{
    Instant = 0,
    Reservation = 1,
    Modify = 2,
}

public enum eOfficeTopicType
{
    미팅 = 1,
    강의,
    행사,
    상담,
    기타,
}

public enum OfficeAuthority
{
    NONE,
    관리자,
    부관리자,
    일반참가자,
    발표자,
    청중,
    관전자,
}

public enum eOfficeSpawnType
{
    기본 = 0,
    의자,
    강단
}

// 오피스 카메라 View
public enum eOfficeCameraType
{
    NONE,
    ALL,
    STAGE,
    SCREEN,
}

public enum eOfficeExposureType
{
    Office = 1,
    Consulting = 2,
}

public enum eOfficePermissionMaster
{
    CHAT,
    VOICE,
    VIDEO,
    SCREEN,
}
#endregion

#region 로그인 및 설정 관련 enum
public enum LoginAccount
{
    Hancom,
    Facebook,
    Google,
    Kakao,
    Naver,
    Guest
}

/// <summary>
/// 설정 패널 퀄리티 세팅
/// </summary>
public enum QUALITY_LEVEL
{
    Low = 0,
    Medium = 1,
    High = 2,
}

/// <summary>
/// 로그인 플랫폼
/// </summary>
public enum LOGIN_PROVIDER_TYPE
{
    ARZMETA = 1,
    NAVER = 2,
    GOOGLE = 3,
    APPLE = 4,
    KAKAO = 5
}

/// <summary>
/// 기기 플랫폼
/// </summary>
public enum OS_TYPE
{
    Android = 1,
    IOS = 2,
    Window = 3
}

/// <summary>
/// 가입 경로 플랫폼 타입
/// </summary>
public enum REGPATH_TYPE
{
    Android = 1,
    IOS = 2,
    //Window = 3, // 홈페이지 가입에서만 사용
    Etc = 4
}

/// <summary>
/// OnOff 콘텐츠
/// </summary>
public enum ONFCONTENTS_TYPE
{
    EnterPopup = 0,
    EventTap = 1,
    AndyWarhol = 2,
    Taekwondo = 3,
}

/// <summary>
/// 배너 컨텐츠 타입
/// </summary>
public enum UPLOAD_TYPE
{
    파일 = 1,
    URL = 2,
}
#endregion

#region 아이템 관련 enum
/// <summary>
/// 아바타 파츠 카테고리
/// </summary>
public enum AVATAR_PARTS_TYPE
{
    hair = 1,
    top = 2,
    bottom = 3,
    onepiece = 4,
    shoes = 5,
    accessory = 6,
    nft_special = 13,
}

public enum eFurnitureCategory
{
    common_all = 0,                    // 전체 
    item_category_furniture = 2001,    // 가구 
    item_category_decoration = 2002,   // 장식 
    item_category_specialty = 2003,    // 특수 
    item_category_floor = 2004,        // 바닥 
}

/// <summary>
/// 아이템 구분
/// </summary>
public enum ITEM_TYPE
{
    NOMAL = 1,
    INTERIOR = 2,
    COSTUME = 3,
    NFT = 4,
}

/// <summary>
/// 아이템 카테고리 구분
/// </summary>
public enum CATEGORY_TYPE
{
    // NOMAL
    consumable = 1001, // 소모품
    product = 1002, // 생산품
    material = 1003, // 재료
    tool = 1004, // 도구
    vehicle = 1005, // 탈것
    pet = 1006, // 펫
    other = 1007, // 기타
    // INTERIOR
    furniture = 2001, // 가구
    decoration = 2002, // 장식
    specialty = 2003, // 특수
    floor = 2004, // 바닥
    // COSTUME
    hair = 3001, // 헤어
    top = 3002, // 상의
    bottom = 3003, // 하의
    onepiece = 3004, // 전신
    shoes = 3005, // 신발
    accessory = 3006, // 액세서리
    // NFT_COSTUME
    nft_hair = 4001, // NFT 헤어
    nft_top = 4002, // NFT 상의
    nft_bottom = 4003, // NFT 하의
    nft_onepiece = 4004, // NFT 전신
    nft_shoes = 4005, // NFT 신발
    nft_accessory = 4006, // NFT 액세서리
    nft_special = 4007, // NFT 스페셜
}

/// <summary>
/// 패키지 구분
/// </summary>
public enum PACKAGE_TYPE
{
    NONE = 1, // 없음
    CONNECITY = 2, // 커넥시티
    CHRISTMAS = 3, // 크리스마스
    KTMF_SPECIAL = 4, // KTMF 스페셜
}

/// <summary>
/// 구매 가격 타입
/// </summary>
public enum PURCHASE_PRICE_TYPE
{
    비매품 = 1,
    무료 = 2,
    반유료 = 3,
    유료 = 4,
    이벤트 = 5,
}

/// <summary>
/// 판매 가격 타입
/// </summary>
public enum SALE_PRICE_TYPE
{
    비매품 = 1,
    무료 = 2,
    반유료 = 3,
    유료 = 4,
    이벤트 = 5,
}

/// <summary>
/// 
/// </summary>
public enum SALEPURCHASE_TYPE
{
    구매가능_판매불가 = 1,
    구매불가_판매가능 = 2,
    구매판매가능 = 3,
    구매판매불가 = 4,
}

/// <summary>
/// 
/// </summary>
public enum NFT_CONTRACT_ADDRESS
{

}
#endregion

#region 팝업 관련 enum
/// <summary>
/// 팝업 버튼 타입
/// </summary>
public enum BTN_TYPE
{
    ConfirmCancel,
    Confirm,
}

/// <summary>
/// 팝업 아이콘 모드
/// </summary>
public enum POPUPICON
{
    NONE,
    CHECK,
    EMAIL,
    WARNING,
}

/// <summary>
/// 토스트 팝업 아이콘 모드
/// </summary>
public enum TOASTICON
{
    None,
    Current,
    Wrong,
    Lock
}
#endregion

#region 투표 관련 enum
/// <summary>
/// 투표 구분 타입
/// </summary>
public enum VOTE_DIV_TYPE
{
    ALTERNATIVE = 1, // 양자택일
    CHOICE = 2, // 선택
}

/// <summary>
/// 투표 양일 응답 타입
/// </summary>
public enum VOTE_ALTER_RES_TYPE
{
    O_X = 1, // O/X
    YES_NO = 2, //찬성/ 반대
}

/// <summary>
/// 투표 응답 타입
/// </summary>
public enum VOTE_RES_TYPE
{
    SINGLE = 1, // 단일 선택
    MULTIPLE = 2, // 복수 선택
}

/// <summary>
/// 투표 결과 노출 타입
/// </summary>
public enum VOTE_RESULT_TYPE
{
    RATE = 1, // 득표율
    COUNT = 2, // 득표수
    MUTIPLE = 3, // 득표율/득표수
}

/// <summary>
/// 투표 진행 상태
/// </summary>
public enum VOTE_STATE_TYPE
{
    SCHEDULED = 1,
    PROGRESS = 2,
    COMPLETED = 3,
    END = 4
}
#endregion

#region 명함 관련 enum
/// <summary>
/// 명함 타입
/// </summary>
public enum BIZCARD_TYPE
{
    BIZCARD_A = 1001,
}

/// <summary>
/// 로드할 명함 상태
/// </summary>
public enum BIZTEMPLATE_TYPE
{
    NOMAL, // 일반 명함
    MINI, // 작은 크기 명함
    EDIT //편집 상태 명함
}
#endregion

#region KTMF 관련 enum
/// <summary>
/// 남은 시간 구분
/// </summary>
public enum KTMF_REMAINTIME
{
    VoteEnd, // 투표 종료까지
    ResultStart, // 결과 노출 시작까지
    ResultEnd, // 결과 노출 종료까지
}

/// <summary>
/// 투표 결과 노출 방식
/// </summary>
public enum KTMF_RSULTTYPE
{
    Always = 1, // 상시 노출
    Voted = 2, // 투표자만 노출
    Unexposed = 3, // 미노출
}

#endregion

#region 채팅 관련 enum
public enum CHAT_MODE
{
    STANDARD,
    CHATTING,
    HIDDEN
}
public enum CHAT_TYPE
{
    ALL,
    WHISPER,
    SYSTEM,
}

public enum MONEY_TYPE
{
    무료재화 = 1,
    반무료재화 = 2,
    유료재화 = 3,
    이벤트재화 = 4,
    현금 = 5,
    비매품 = 6,
}
#endregion

#region 우편함 관련 enum
public enum POSTAL_EFFECT_TYPE
{
    NORMAL = 1001,
    FULL_SCREEN = 2001,
    FULL_SCREEN_RANDOM = 3001
}
#endregion

#region 유학박람회 CSAF enum
public enum CSAF_FILEBOX_TYPE 
{ 
    None = 0,
    Cloud = 1,
    File = 2,
    Video = 3,
}
#endregion