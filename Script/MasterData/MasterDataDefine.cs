namespace db {

	public class AdContents 
	{
		public int  id; // 아이디
		public int  moneyType; // 재화 타입
		public string reward; // 보상
	};

	public class AppendType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class AreaType 
	{
		public int  type; // 구역 타입
		public string name; // 이름
	};

	public class AvatarPartsColorType 
	{
		public int  type; // 색상 타입
		public string name; // 이름
	};

	public class AvatarPartsGroupType 
	{
		public int  type; // 그룹 타입
		public string name; // 이름
	};

	public class AvatarPartsSizeType 
	{
		public int  type; // 사이즈 타입
		public string name; // 이름
	};

	public class AvatarPartsStateType 
	{
		public int  type; // 파츠 상태 타입
		public string name; // 이름
	};

	public class AvatarPartsType 
	{
		public int  type; // 아바타 파츠 타입
		public string name; // 이름
	};

	public class AvatarPreset 
	{
		public int  presetType; // 프리셋 타입
		public int  partsType; // 아바타 파츠 타입
		public int  itemId; // 아이템 아이디
	};

	public class AvatarPresetType 
	{
		public int  type; // 프리셋 타입
		public string name; // 이름
	};

	public class AvatarResetInfo 
	{
		public int  partsType; // 아바타 파츠 타입
		public int  itemId; // 아이디
	};

	public class BannerInfo 
	{
		public int  id; // 아이디
		public int  spaceType; // 공간 타입
		public int  spaceDetailType; // 공간 상세 타입
		public string positionImage; // 위치 이미지
		public int  width; // x크기
		public int  height; // y크기
		public int  mediaRollingType; // 미디어롤링타입
		public int  bannerType; // 배너 타입
		public int  mediaExposureType; // 미디어 노출 타입
	};

	public class BannerType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class BoothBannerInfo 
	{
		public int  id; // 아이디
		public int  spaceType; // 공간 타입
		public int  spaceDetailType; // 공간 상세 타입
		public int  width; // x사이즈
		public int  height; // y사이즈
		public int  mediaRollingType; // 미디어 롤링 타입
		public int  bannerType; // 배너 타입
	};

	public class BoothScreenInfo 
	{
		public int  id; // 아이디
		public int  spaceType; // 공간 타입
		public int  spaceDetailType; // 공간 상세 타입
		public string description; // 설명
		public int  width; // x사이즈
		public int  height; // y사이즈
		public int  mediaRollingType; // 미디어 롤링 타입
	};

	public class BusinessCardTemplate 
	{
		public int  id; // 템플릿 아이디
		public int  purchaseType; // 구매 타입
		public int  price; // 가격
		public int  nameField; // 이름 필드
		public int  phoneField; // 전화번호 필드
		public int  emailField; // 이메일 필드
		public int  addrField; // 주소 필드
		public int  faxField; // 팩스 필드
		public int  jobField; // 직업 필드
		public int  positionField; // 직책 필드
		public int  introField; // 소개 필드
		public string thumbnailName; // 썸네일 이름
	};

	public class BuySellType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class CategoryType 
	{
		public int  type; // 카테고리 타입
		public string name; // 이름
	};

	public class CommerceZoneItem 
	{
		public int  itemId; // 아이템 아이디
		public int  arwPrice; // 아로와나 가격
		public int  krwPrice; // 원화 가격
		public int  groupType; // 그룹 타입
		public int  colorType; // 색상 타입
		public int  sizeType; // 사이즈 타입
	};

	public class CommerceZoneMannequin 
	{
		public int  id; // 아이디
		public int  modelType; // 마네킹 모델 타입
		public int  partsType; // 아바타 파츠 타입
		public int  itemId; // 아이템 아이디
	};

	public class CountryCode 
	{
		public int  id; // 국가 코드 아이디
		public string nameId; // 이름 아이디
		public int  code; // 국가 코드
	};

	public class DynamicLinkType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class EventSpaceType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class Faq 
	{
		public int  id; // 아이디
		public string question; // 질문
		public string answer; // 응답
	};

	public class FileBoxType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class ForbiddenWords 
	{
		public int  id; // 아이디
		public string text; // 텍스트
	};

	public class FunctionTable 
	{
		public int  id; // 아이디
		public string description; // 설명
		public int  value; // 값
	};

	public class GradeType 
	{
		public int  type; // 등급 타입
		public string name; // 이름
	};

	public class InquiryType 
	{
		public int  type; // 문의 타입
		public string name; // 이름
	};

	public class InteriorInstallInfo 
	{
		public int  itemId; // 아이템 아이디
		public string prefab; // 프리팹
		public int  layerType; // 레이어 타입
		public int  xSize; // x사이즈
		public int  ySize; // y사이즈
		public int  removable; // 제거가능여부
	};

	public class Item 
	{
		public int  id; // 아이디
		public int  itemType; // 아이템 타입
		public int  categoryType; // 카테고리 타입
		public int  packageType; // 패키지 타입
		public string name; // 이름
		public string description; // 설명
		public string prefab; // 프리팹
		public string thumbnail; // 썸네일
		public int  capacity; // 최대 개수
		public int  isNesting; // 중첩여부
		public int  purchaseType; // 구매 타입
		public int  purchasePrice; // 구매 가격
		public int  saleType; // 판매 타입
		public int  salePrice; // 판매 가격
		public int  gradeType; // 등급 타입
		public int  buySellType; // 구매 판매 타입
	};

	public class ItemMaterial 
	{
		public int  itemId; // 아이템 아이디
		public int  num; // 번호
		public string material; // 머터리얼
	};

	public class ItemType 
	{
		public int  type; // 아이템 타입
		public string name; // 이름
	};

	public class ItemUseEffect 
	{
		public int  itemId; // 아이템 아이디
		public string chat; // 챗
		public string animationName; // 애니메이션 이름
		public int  partsType; // 아바타 파츠 타입
	};

	public class JumpingMatchingGameType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class JumpingMatchingLevel 
	{
		public int  id; // 레벨 아이디
		public int  tileToHintInt; // 타일생성간격
		public int  hintInt; // 힌트간격
		public int  quizeToDesInt; // 퀴즈발판파괴간격
		public int  DesToFinInt; // 파괴후종료까지간격
		public int  nextRoundInt; // 다음라운드까지간격
		public int  showQuizeSec; // 퀴즈노출시간
		public int  gameType; // 게임 타입
		public int  spawnPaintCount; // 그림발판생성갯수
		public int  paintCount; // 그림 종류 갯수
		public string hintLevel; // 힌트레벨
	};

	public class KtmfSpecialItem 
	{
		public int  costumeId; // 코스튬 아이디
		public int  partsId; // 파츠 아이디
	};

	public class LandType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class LanguageType 
	{
		public int  type; // 언어 타입
		public string name; // 이름
	};

	public class LayerType 
	{
		public int  type; // 레이어 타입
		public string name; // 이름
	};

	public class LicenseFunction 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class LicenseType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class LicenseTypeInfo 
	{
		public int  licenseType; // 라이선스 타입
		public int  licenseFunc; // 라이선스 기능
		public int  value; // 데이터
	};

	public class Localization 
	{
		public string id; // 다국어 아이디
		public string kor; // 한국어
		public string eng; // 영어
	};

	public class MannequinModelType 
	{
		public int  type; // 모델 타입
		public string name; // 이름
	};

	public class MapExposulBrand 
	{
		public int  mapExposulInfoId; // 지도 노출 아이디
		public string brandName; // 브랜드 이름
	};

	public class MapExposulInfo 
	{
		public int  id; // 아이디
		public int  landType; // 랜드 타입
		public int  mapInfoType; // 맵 정보 타입
		public int  sort; // 정렬
		public string image; // 이미지
		public string name; // 이름
		public string description; // 설명
		public int  positionX; // x 좌표
		public int  positionY; // y 좌표
		public int  positionZ; // z 좌표
		public int  rotationY; // y 로테이션
	};

	public class MapInfoType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class MediaExposureType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class MediaRollingType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class MoneyType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class MyRoomStateType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class NewsType 
	{
		public int  type; // 공지 타입
		public string name; // 이름
	};

	public class NoticeExposureType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class NoticeType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class NpcArrange 
	{
		public int  npcId; // npc 아이디
		public int  sceneType; // 타입
		public int  positionX; // x좌표
		public int  positionY; // y좌표
		public int  positionZ; // z좌표
		public int  rotationY; // y로테이션
		public string animation; // 애니메이션
	};

	public class NpcCostume 
	{
		public int  npcId; // npc 아이디
		public int  partsType; // 아바타 파츠 타입
		public int  itemId; // 아이템 아이디
	};

	public class NpcList 
	{
		public int  id; // 아이디
		public int  npcType; // npc타입
		public int  npcLookType; // npc외형타입
		public string name; // 이름
		public string prefab; // 프리팹
	};

	public class NpcLookType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class NpcType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class ObjectInteractionType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class OfficeAlarmType 
	{
		public int  type; // 알람 타입
		public string name; // 이름
	};

	public class OfficeAuthority 
	{
		public int  modeType; // 모드 타입
		public int  permissionType; // 참가자 타입
		public int  chatLock; // 채팅 제한
		public int  voiceLock; // 음성 제한
		public int  videoChatLock; // 화상채팅 제한
		public int  webSharePermission; // 웹공유권한
		public int  kick; // 강퇴 기능
		public int  selectHost; // 호스트 지정
		public int  selectSubHost; // 부호스트 지정
		public int  selectGuest; // 게스트 지정
		public int  selectAnnouncer; // 발표자 지정
		public int  selectListener; // 청중 지정
		public int  selectObserver; // 관전자 지정
		public int  changeRoomInfo; // 룸 정보 변경
		public int  closeRoom; // 룸 폐쇄
	};

	public class OfficeBookmark 
	{
		public int  id; // 아이디
		public string name; // 이름
		public string thumbnail; // 썸네일
		public string url; // 경로
	};

	public class OfficeDefaultOption 
	{
		public int  permissionType; // 참가자 타입
		public int  charControl; // 캐릭터 컨트롤
		public int  camControl; // 캠 컨트롤
		public int  actionEmotion; // 액션 이모션
		public int  chat; // 채팅
		public int  voiceChat; // 음성 채팅
		public int  videoChat; // 화상 채팅
		public int  web; // 웹
		public int  webShare; // 웹 공유
		public int  videoPlayer; // 비디오 재생
		public int  videoPlayerShare; // 비디오 재생 공유
		public int  spawnType; // 스폰 타입
		public int  movable; // 이동 가능 여부
		public int  selectSeat; // 자리 이동
	};

	public class OfficeExposure 
	{
		public int  exposureType; // 노출 타입
		public int  modeType; // 모드 타입
	};

	public class OfficeExposureType 
	{
		public int  type; // 노출 타입
		public string name; // 이름
	};

	public class OfficeGradeAuthority 
	{
		public int  gradeType; // 등급 타입
		public int  isUsePaidRoom; // 유료 룸 사용 가능 여부
		public int  capacityLimit; // 최대 인원 설정 제한
		public int  reserveLimit; // 예약 갯수 제한
		public int  isThumbnail; // 썸네일 설정 가능 여부
		public int  isWaitingRoom; // 대기실 설정 가능 여부
		public int  isAdvertising; // 홍보 노출 가능 여부
		public int  isObserver; // 관전자 설정 가능 여부
		public int  isChangeAdmin; // 관리자 변경 가능 여부
		public int  timeLimit; // 사용 시간 제한
		public int  isChangeTime; // 사용 시간 변경 가능 여부
	};

	public class OfficeGradeType 
	{
		public int  type; // 등급 타입
		public string name; // 이름
	};

	public class OfficeMode 
	{
		public int  modeType; // 모드 타입
		public int  gradeType; // 등급 타입
		public int  privateYn; // 공개 여부
		public int  passwordYn; // 패스워드 여부
		public string icon; // 아이콘
		public string modeDesc; // 모드 설명
		public string roomName; // 룸 이름
		public string roomDesc; // 룸 설명
		public int  startMin; // 시작 분
		public int  minGap; // 분 간격
		public int  timeCount; // 갯수
	};

	public class OfficeModeSlot 
	{
		public int  modeType; // 모드 타입
		public int  permissionType; // 참가자 타입
	};

	public class OfficeModeType 
	{
		public int  type; // 모드 타입
		public string name; // 이름
	};

	public class OfficePermissionType 
	{
		public int  type; // 참가자 타입
		public string name; // 이름
	};

	public class OfficeProductItem 
	{
		public int  productId; // 상품 아이디
		public string name; // 상품 이름
		public int  officeGradeType; // 등급 타입
		public int  period; // 이용 기간
	};

	public class OfficeSeatInfo 
	{
		public int  spaceId; // 공간 아이디
		public int  num; // 번호
		public string seatName; // 좌석 이름
	};

	public class OfficeShowRoomInfo 
	{
		public int  id; // 아이디
		public int  roomId; // 룸 아이디
		public string roomName; // 룸 이름
		public string roomDesc; // 룸 설명
		public string thumbnail; // 썸네일 이름
	};

	public class OfficeSpaceInfo 
	{
		public int  id; // 아이디
		public int  modeType; // 모드 타입
		public int  exposureOrder; // 노출 순서
		public string description; // 설명
		public string spaceName; // 공간 이름
		public string thumbnail; // 썸네일
		public string sceneName; // 씬이름
		public int  sitCapacity; // 자리 공간
		public int  defaultCapacity; // 기본 인원
		public int  minCapacity; // 최소 인원
		public int  maxCapacity; // 최대 인원
		public int  maxObserver; // 최대 관전인원
	};

	public class OfficeSpawnType 
	{
		public int  type; // 스폰 타입
		public string name; // 이름
	};

	public class OfficeTopicType 
	{
		public int  type; // 토픽 타입
		public string name; // 이름
	};

	public class OnfContentsType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class OsType 
	{
		public int  type; // OS 타입
		public string name; // 이름
		public string storeUrl; // 스토어Url
	};

	public class PackageType 
	{
		public int  type; // 패키지 타입
		public string name; // 이름
	};

	public class PaymentProductManager 
	{
		public int  id; // 상품 아이디
		public int  moneyType; // 재화 타입
		public int  price; // 가격
		public int  purchaseLimit; // 구매 한도
	};

	public class PaymentStateType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class PostalEffectType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class PostalItemProperty 
	{
		public int  itemType; // 아이템 타입
		public int  categoryType; // 카테고리 타입
		public int  postalEffectType; // 연출 타입
		public string effectResource; // 연출 리소스
	};

	public class PostalMoneyProperty 
	{
		public int  moneyType; // 재화 타입
		public int  postalEffectType; // 연출 타입
		public string effectResource; // 연출 리소스
	};

	public class PostalType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class ProviderType 
	{
		public int  type; // 제공 타입
		public string name; // 이름
	};

	public class QuizAnswerType 
	{
		public int  type; // 정답 타입
		public string name; // 이름
	};

	public class QuizLevel 
	{
		public int  level; // 레벨
		public int  waitTime; // 대기시간
		public int  playTime; // 플레이시간
	};

	public class QuizQuestionAnswer 
	{
		public int  id; // 퀴즈 아이디
		public string questionId; // 질문 아이디
		public int  answerType; // 정답 타입
	};

	public class QuizRoundTime 
	{
		public int  round; // 라운드
		public int  TimeType; // 시간 타입
	};

	public class QuizTimeType 
	{
		public int  type; // 시간 타입
		public string name; // 이름
	};

	public class ReportReasonType 
	{
		public int  type; // 사유타입
		public string name; // 이름
	};

	public class ReportType 
	{
		public int  type; // 신고 타입
		public string name; // 이름
	};

	public class ScreenContentType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class ScreenInfo 
	{
		public int  id; // 아이디
		public int  spaceType; // 공간 타입
		public int  spaceDetailtype; // 공간 상세 타입
		public string description; // 설명
		public string positionImage; // 위치 이미지
		public int  width; // x크기
		public int  height; // y크기
		public int  mediaRollingType; // 미디어롤링타입
		public int  mediaExposureType; // 미디어 노출 타입
	};

	public class SelectVoteStateType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class SpaceDetailType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class SpaceInfo 
	{
		public int  spaceType; // 공간 타입
		public int  spaceDetailType; // 공간 상세 타입
	};

	public class SpaceType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class StoreType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class UploadType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VideoScreenInfo 
	{
		public int  id; // 스크린 아이디
		public int  worldType; // 월드 타입
		public int  areaType; // 구역 타입
		public string screenName; // 스크린 이름
	};

	public class VoteAlterResponse 
	{
		public int  id; // 아이디
		public int  alterResType; // 양일 응답 타입
		public string name; // 이름
	};

	public class VoteAlterResType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VoteDivType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VoteResType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VoteResultExposureType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VoteResultType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class VoteStateType 
	{
		public int  type; // 타입
		public string name; // 이름
	};

	public class WorldAreaInfo 
	{
		public int  worldType; // 월드 타입
		public int  areaType; // 구역 타입
	};

	public class WorldType 
	{
		public int  type; // 월드 타입
		public string name; // 이름
	};

}
