using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

/// <summary>
/// 웹패킷의 Url 적는 공간
/// </summary>
public static class WebPacket
{
    #region 게이트웨이 Gateway
    public static string Gateway(string arg0) => $"{arg0}/api/gateway";
    #endregion

    #region 계정 Account
    public static string CreateAccount = "/api/account/create";
    public static string Login = "/api/account/arzmetaLogin";
    public static string SocialAccountLogin = "/api/account/socialLogin";
    public static string LinkedAccount = "/api/account/linkedAccount";
    public static string ReleaseLinkedAccount(int arg0) => $"/api/account/releaseLinkedAccount/{arg0}";
    public static string LoginAuth = "/api/account/loginAuth";
    public static string AutoLogin = "/api/account/autoLogin";
    public static string AuthEmail = "/api/account/authEmail";
    public static string ConfirmEmail = "/api/account/confirmEmail";
    public static string ResetPassword = "/api/account/resetPassword";
    public static string CheckArzmetaAccount = "/api/account/checkArzmetaAccount";
    #endregion

    #region 사용자 Member
    public static string CheckLive = "/api/member/checkLive";
    public static string CheckNickName = "/api/member/checkNickname";
    public static string UpdateMyCard = "/api/member/updateMyCard";
    public static string UpdateMyProfile = "/api/member/updateMyProfile";
    public static string Avatar = "/api/member/avatar";
    public static string UpdateEmail = "/api/member/updateEmail";
    public static string Withdrawal = "/api/member/withdrawal";
    public static string SetAvatarPreset = "/api/member/setAvatarPreset";
    public static string GetMemberInfo = "/api/member/getMemberInfo";
    public static string GetAppInfo = "/api/member/getAppInfo";
    public static string ChangePassword = "/api/member/changePassword";
    public static string SetDefaultCardInfo = "/api/member/setDefaultCardInfo";
    public static string DeleteDefaultCardInfo = "/api/member/delDefaultCardInfo";
    public static string CheckWithdrawalProgress = "/api/member/check-withdrawal-progress";
    #endregion

    #region 친구
    public static string GetFriends = "/api/friend";
    public static string RequestFriend = "/api/friend/requestFriend";
    public static string GetRequestFriends = "/api/friend/getRequestFriends";
    public static string GetReceiveFriends = "/api/friend/receiveRequestFriends";
    public static string AcceptFriend(string arg0) => $"/api/friend/acceptFriend/{arg0}";
    public static string CancelRequestFriend(string arg0) => $"/api/friend/cancelRequestFriend/{arg0}";
    public static string RefusalRequestFriend(string arg0) => $"/api/friend/refusalRequestFriend/{arg0}";
    public static string BlockFriend = "/api/friend/blockFriend";
    public static string DeleteFriend(string arg0) => $"/api/friend/deleteFriend/{arg0}";
    public static string ReleaseBlockFriend(string arg0) => $"/api/friend/releaseBlockFriend/{arg0}";
    public static string GetBlockFriends = "/api/friend/getBlockFriends";
    public static string FindFriend(int arg0, string arg1) => $"/api/friend/findFriend/{arg0}/{arg1}";
    public static string BookmarkFriend = "/api/friend/bookmark";
    public static string FindFriendRoomId(string arg0) => $"/api/friend/findRoomId/{arg0}";
    #endregion

    #region 투표
    public static string Vote = "/api/vote";
    #endregion

    #region KTMF 투표
    public static string KTMFVote = "/api/select-vote";
    public static string KTMFLike = "/api/select-vote/like";
    public static string KTMFVoteResult(int arg0) => $"/api/select-vote/result/{arg0}";
    public static string KTMFEmailCheck = "/api/select-vote/ktmf-email";
    #endregion

    #region 타인의 정보
    public static string MemberInfo(OTHERINFO_TYPE arg0, string arg1) => $"/api/others/memberInfo?type={(int)arg0}&othersId={arg1}";
    #endregion

    #region 휴면계정
    public static string DormantCheckEmail = "/api/create/dormantCheckEmail";
    public static string DormantConfirmEmail = "/api/create/dormantConfirmEmail";
    public static string DormantAccount = "/api/create/releaseDormantAccount";
    #endregion

    #region 무한의코드
    public static string AllMyRanking = "/api/ranking/allMyRanking";
    public static string AllRanking = "/api/ranking/allRanking";
    public static string MyRanking = "/api/ranking/myRanking";
    public static string RecordRanking = "/api/ranking";
    #endregion

    #region 마이룸
    public static string MyRoomCreate_Req = "/api/myRoom/create";
    public static string MyRoomOthersRoomList_Req(string arg0) => $"/api/myRoom/othersRoomList/{arg0}";
    public static string MyRoomChangeState = "/api/myRoom/stateType";
    public static string MyRoomFrameImageUpload = "/api/myRoom/frame-image";
    public static string MyRoomFrameImageDelete(string arg0) => $"/api/myRoom/frame-image?frameImages={arg0}";
    #endregion

    #region 오피스
    public static string createOfficeReserv = "/api/office/createOfficeReserv/";
    public static string updateOfficeReserv(string arg0) => $"/api/office/updateOfficeReserv/{arg0}";
    public static string Office_CreateRoomCodeReq = "/api/office/createRoomCode";
    public static string Office_GetIsAdvertisingList = "/api/office/getIsAdvertisingList";
    public static string GetRequest_OfficeReservation = "/api/office/getReservInfo";
    public static string GetRequest_OfficeInterest = "/api/office/getWaitInfo";
    public static string GetRequest_OfficeRoomInfo(string _roomCode) => $"/api/office/getRoomInfo/{_roomCode}";
    public static string CheckRoomPassword = "/api/office/checkRoomCodePassword";
    public static string Office_CancelReservation(string arg0) => $"/api/office/deleteReservation/{arg0}";
    public static string Office_CancelReservationWait(string arg0) => $"/api/office/deleteWaiting/{arg0}";
    public static string Office_WaitReservationReq = "/api/office/waitOfficeReserv";
    #endregion

    #region 우편함
    public static string Postbox = "/api/postbox";
    public static string PostboxRecieve(string arg0) => $"/api/postbox/recieve/{arg0}";
    public static string PostboxReceiveAll = "/api/postbox/receive-all";
    #endregion

    #region 숏링크
    public static string CreateShortLink = "/api/room/createRoomShotLink";
    #endregion

    #region 스크린&배너
    //public static string GetScreen = "/api/screen-banner/getScreen";
    #endregion

    #region 재화, 코인
    public static string ADContents = "/ad-contents";
    public static string GetMoneyInfo = "/api/member/get-money-info";
    #endregion

    #region 유학박람회 라이선스
    public static string GetExpoLicenseInfo = "/api/member/get-csaf-admin";
    #endregion

    #region 유학박람회 (CSAF - Chongro Study Abroad Fair)

    //행사입장
    public static string EnterCSAF = "/api/csaf/event";
    
    //부스
    public static string CreateCSAFBooth = "/api/csaf/booth";
    public static string GetCSAFBooths = "/api/csaf/booths";
    public static string GetCSAFBoothName = "/api/csaf/booth-name";

    public static string EditCSAFBooth(int boothId) => BoothDefault(boothId);
    public static string DeleteCSAFBooth(int boothId) => BoothDefault(boothId);
    public static string GetCSAFBoothDetail(int boothId) => BoothDefault(boothId);
    private static string BoothDefault(int boothId) => $"{CreateCSAFBooth}/{boothId}";

    //배너
    public static string CreateCSAFBanner = "/api/csaf/banner";
    public static string EditCSAFBanner(int boothId, int bannerId) => BannerDefault(boothId, bannerId);
    public static string DeleteCSAFBanner(int boothId, int bannerId) => BannerDefault(boothId, bannerId);
    public static string BannerDefault(int boothId, int bannerId) => $"{CreateCSAFBanner}/{boothId}/{bannerId}";

    //스크린
    public static string CreateCSAFScreen = "/api/csaf/screen";
    public static string EditCSAFScreen(int boothId, int screenId) => ScreenDefault(boothId, screenId);
    public static string DeleteCSAFScreen(int boothId, int screenId) => ScreenDefault(boothId, screenId);
    public static string ScreenDefault(int boothId, int screenId) => $"{CreateCSAFScreen}/{boothId}/{screenId}";


    //파일함
    public static string GetCSAFFileboxList(int boothId) => $"/api/csaf/fileboxes/{boothId}";
    public static string RegisterCSAFFileBox = "/api/csaf/filebox";
    public static string EditDelCSAFFilebox(int boothId, int fileId) => $"/api/csaf/filebox/{boothId}/{fileId}";
    #endregion

    #region ChatGPT
    public const string ChatGPT = "https://api.openai.com/v1/chat/completions";
    #endregion
}

//=====================================

#region [구버전] 
public class CheckEmailPacketRes_Old : DefaultPacketRes
{
    public int remainTime;
}
#endregion

//=====================================

#region 단순 응답용 패킷
public class DefaultPacketRes
{
    public int error;
    public string errorMessage;
}
#endregion

#region 게이트 웨이
public class GatewayInfo
{
    public Gateway Gateway;
}

/// <summary>
/// [같은 속성]
/// 1) Gateway.osType & Gateway.OsType.type - 현재 기기 플랫폼 Android, Ios 등
/// 2) Gateway.stateType & Gateway.ServerState.state - 서버 비/활성/점검 상태 등
/// 3) Gateway.ServerType.type & Gateway.ServerType.ServerInfo.serverType - Dev, Live 등
/// 같은 이름 다른 형이면 터짐
/// 미리 선언한 클래스에 없는 클래스 형이 새로 추가되면 터짐
/// 미리 선언한 클래스에 없는 기본 파라미터(string, int...)가 새로 추가되는 건 안 터짐
/// </summary>
public class Gateway
{
    public int osType;
    public string appVersion;
    public int serverType;
    public int stateType; // ?
    public ServerType ServerType;
    public OsType OsType;
    public ServerState ServerState;
    public StateMessage StateMessage;
}

public class GatewayInfo_Update
{
    public OsType OsType;
    public ServerState ServerState;
    public StateMessage StateMessage;
}

public class ServerState
{
    public int state;
    public string name;
}

public class ServerType
{
    public int type;
    public string name;
    public ServerInfo ServerInfo;
}

public class ServerInfo
{
    public int serverType;
    public string accountServerUrl;
    public string pcAccountServerUrl;
    public string agoraServerUrl;
    public string contentServerUrl;
    public string lobbyServerUrl;
    public string meetingRoomServerUrl;
    public int meetingRoomServerPort;
    public string myRoomServerUrl;
    public int medicinePort;
    public string medicineUrl;
    public int myRoomServerPort;
    public string matchingServerUrl;
    public int matchingServerPort;
    public string OXServerUrl;
    public int OXServerPort;
    public string storageUrl;
    public string homepageUrl;
    public string webviewUrl;
    public int gameServerPort;
    public string linuxServerIp;
    public int linuxServerPort;
    public int linuxHttpPort;
    public string homepageBackendUrl;
    public string webSocketUrl;
    public string youtubeDlUrl;
}

public class OsType
{
    public int type;
    public string name;
    public string storeUrl;
}

public class StateMessage
{
    public int id;
    public string message;
}
#endregion

#region 계정 생성
public class CreateAccountLoginPacketRes : DefaultPacketRes
{
    public string loginToken;
}
#endregion

#region 로그인
public class LoginPacketRes : DefaultPacketRes
{
    public MemberData memberInfo;
}

public class MemberData
{
    public string memberId;
    public string memberCode;
    public int providerType;
    public int officeGradeType;
    public string email;
    public string nickname;
    public string stateMessage;
    public string jwtAccessToken;
    public string sessionId;
    public string loginToken;
    public int myRoomStateType;
    public MoneyReward moneyReward;
}

[System.Serializable]
public class MemberAccountInfo
{
    public int providerType;
    public string accountToken;
}

public class MoneyReward
{
    public int moneyType;
    public int rewardCount;
}
#endregion

#region 소셜 로그인
public class SocialAccountLoginPacketReq
{
    public string accountToken;
    public int providerType;
    public int regPathType;
}

public class SocialAccountLoginPacketRes : DefaultPacketRes
{
    public string memberId;
}
#endregion

#region 계정 연동
public class LinkedAccountPacketReq
{
    public string accountToken;
    public int providerType;
    public string password;
}

public class CurrentAccountPacketRes : DefaultPacketRes
{
    public MemberInfo memberInfo;
    public MemberAccountInfo[] socialLoginInfo;
}

public class ConvertAccountPacketRes : DefaultPacketRes
{
    public MemberInfo memberInfo;
}

[System.Serializable]
public class MemberInfo
{
    public string memberId;
    public string nickname;
    public string stateMessage;
    public string memberCode;
    public Dictionary<string, int> avatarInfos;
    public MemberAccountInfo[] socialLoginInfo;
}
#endregion

#region 계정 연동 해제

public class ReleaseLinkedAccountPacketRes : DefaultPacketRes
{
    public MemberAccountInfo[] socialLoginInfo;
}
#endregion

#region 로그인 토큰으로 멤버아이디 받아오기
public class LoginAuthPacketRes : DefaultPacketRes
{
    public string memberId;
}
#endregion

#region 아바타 설정
public class CreateAvatarPacketRes : DefaultPacketRes
{
    public Dictionary<string, int> avatarInfos;
}
#endregion

#region 비밀번호 변경
public class ChangePasswordPacketRes : DefaultPacketRes
{
    public string loginToken;
}
#endregion

#region 이메일 인증번호 받기 / 이메일 업데이트 / 패스워드 재설정
public class CheckEmailPacketReq
{
    public string email;
}

public class CheckEmailPacketRes : DefaultPacketRes
{
    public int remainTime;
    public MemberInfo memberInfo;
}
#endregion

#region 이메일 인증번호 확인
public class AuthEmailPacketReq
{
    public string email;
    public int authCode;
}

public class AuthEmailPacketRes : DefaultPacketRes
{
    public string authCode;
}
#endregion

#region 무한의코드

//req
[System.Serializable, Preserve]
public class rankingReq
{
    //public string memberId;
    //public string jwtAccessToken;
    //public string sessionId;
    public float score;
}

//res

[System.Serializable, Preserve]
public class defaultRanking
{
    public int rank;
    public float userScore;
    public string memberId;
    public string nickname;
    //public Member member;
}
//public class memberRanking
//{
//    public float userScore;
//    public int rank;

//    public Member member;
//}
//public class rankingList
//{
//    public float userScore;
//    public int rank;

//    public Member member;
//}
[System.Serializable, Preserve]
public class allMyRankingRes : DynamicScrollData
{
    public int error;
    public defaultRanking[] allRanking;
    public defaultRanking[] myRanking;
}
[System.Serializable, Preserve]
public class allRankingRes : DynamicScrollData
{
    public defaultRanking[] allRanking;
    public int error;
}
[System.Serializable, Preserve]
public class myRankingRes : DynamicScrollData
{
    public defaultRanking[] myRanking;
    public int error;
}

#endregion

#region KTMF 투표
public class GetKTMFVoteInfoPacketRes : DefaultPacketRes
{
    public SelectVoteInfo selectVoteInfo;
    public VoteItem[] voteItems;
    public MyVote[] myVote;

    /// <summary>
    /// + (true) : 아직 끝나지 않음
    /// - (false) : 끝남
    /// </summary>
    /// <param name="remainType"></param>
    /// <returns></returns>
    public bool GetRemainState(KTMF_REMAINTIME remainType)
    {
        return GetRemain(remainType) > 0;
    }

    public int GetRemain(KTMF_REMAINTIME remainType)
    {
        if (selectVoteInfo != null)
        {
            switch (remainType)
            {
                case KTMF_REMAINTIME.VoteEnd:
                    if (selectVoteInfo.remainEnd != null)
                    {
                        return int.Parse(selectVoteInfo.remainEnd);
                    }
                    break;
                case KTMF_REMAINTIME.ResultStart:
                    if (selectVoteInfo.remainResultStart != null)
                    {
                        return int.Parse(selectVoteInfo.remainResultStart);
                    }
                    break;
                case KTMF_REMAINTIME.ResultEnd:
                    if (selectVoteInfo.remainResultEnd != null)
                    {
                        return int.Parse(selectVoteInfo.remainResultEnd);
                    }
                    break;
                default:
                    return -1;
            }
        }
        return -1;
    }

    public void ResetRemain(KTMF_REMAINTIME remainType)
    {
        if (selectVoteInfo != null)
        {
            switch (remainType)
            {
                case KTMF_REMAINTIME.VoteEnd:
                    if (selectVoteInfo.remainEnd != null)
                    {
                        selectVoteInfo.remainEnd = "0";
                    }
                    break;
                case KTMF_REMAINTIME.ResultStart:
                    if (selectVoteInfo.remainResultStart != null)
                    {
                        selectVoteInfo.remainResultStart = "0";
                    }
                    break;
                case KTMF_REMAINTIME.ResultEnd:
                    if (selectVoteInfo.remainResultEnd != null)
                    {
                        selectVoteInfo.remainResultEnd = "0";
                    }
                    break;
                default: break;
            }
        }
    }
}

public class SelectKTMFVotePacketRes : DefaultPacketRes
{
    public MyVote[] myVote;
}

public class LikeKTMFVotePacketRes : DefaultPacketRes
{
    public LikeInfo likeInfo;
}

public class GetKTMFResultPacketRes : DefaultPacketRes
{
    public Rank[] rank;
    public int voteTotalCount;
    public int resultExposureType;
}

public class SelectVoteInfo
{
    public int id; // 투표 아이디
    public string name; // 투표 제목
    public int resultType; // 결과 노출 방식 (상시노출, 투표자만 노출, 미노출)
    public int voteCount; // 관리자 페이지에서 지정한 총 투표권 수
    public string startedAt; // 투표 시작 일시
    public string endedAt; // 투표 종료 일시
    public string remainEnd; // 투표 가능 기간 종료까지 남은 초
    public string remainResultStart; // 투표 결과 공개 시작까지 남은 초
    public string remainResultEnd; // 투표 결과 공개 종료(= 투표 완전 종료)까지 남은 초
}

public class VoteItem
{
    public int itemNum;
    public int displayNum;
    public string name;
    public string description;
    public string videoUrl;
    public string imageName;
    public int isLike;
    public int likeCount;
}

public class MyVote
{
    public int itemNum;
}

public class Rank
{
    public int rank;
    public int voteId;
    public int itemNum;
    public string name;
    public string imageName;
    public int voteCount;
    public int likeCount;
    public float? rate;
}

public class LikeInfo 
{
    public int itemNum;
    public int isLike;
    public int likeCount;
}

#endregion

/// <summary>
/// 이건뭐지
/// </summary>
public class CheckWithdrawalProgressPacketRes : DefaultPacketRes
{
    public string deltedAt;
}

//========= [ 디버그 출력용 패킷 ] =========//

#region 디버그 출력용
public class PreMaster
{
    public int debug;
}
public class PreMasterType
{
    public string appVersion;
    public int debug;
}
#endregion

//========= [ 프로필 패킷 ] =========//

#region 프로필 업데이트
public class UpdateProfilePacketRes : DefaultPacketRes
{
    public string nickname;
    public string stateMessage;
}
#endregion

#region 아바타 프리셋 설정
public class SetAvatarPresetPacketRes : DefaultPacketRes
{
    public Dictionary<string, int> avatarInfos;
    public string nickname;
    public string stateMessage;
}
#endregion

//========= [ 명함 패킷 ] =========//

#region 명함 업데이트
public class UpdateCardPacketRes : DefaultPacketRes
{
    public BizCardInfo[] businessCardInfos;
}

public class BizCardInfo : ICloneable
{
    public int templateId;
    public int num;
    public string name;
    public string phone;
    public string email;
    public string addr;
    public string fax;
    public string job;
    public string position;
    public string intro;

    public object Clone()
    {
        return new BizCardInfo()
        {
            templateId = this.templateId,
            num = this.num,
            name = this.name,
            phone = this.phone,
            email = this.email,
            addr = this.addr,
            fax = this.fax,
            job = this.job,
            position = this.position,
            intro = this.intro
        };
    }
}
#endregion

#region 기본 명함 설정
public class DefaultCardPacketRes : DefaultPacketRes
{
    public DefaultCardInfo defaultCardInfo;
}

public class DefaultCardInfo
{
    public int templateId;
    public int num;
}
#endregion

//========= [ 친구 패킷 ] =========//

#region 하위 메소드
public class Friend
{
    public string friendMemberCode;
    public string friendNickname;
    public string friendMessage;
    public string createdAt;
    public int bookmark;
    public string bookmarkedAt;
    public Dictionary<string, int> avatarInfos;
}

// 웹소켓에서만 추가 제공하는 항목
public class FriendWebSocket : Friend
{ 
    public string friendMemberId;
    public bool isOnline; 
}
#endregion

#region 친구 목록 가져오기 
public class GetFriendsPacketRes : DefaultPacketRes
{
    public Friend[] friends;
}
#endregion

#region 친구 요청 보낸 목록 가져오기
public class GetRequestFriendsPacketRes : DefaultPacketRes
{
    public Friend[] myRequestList;
}
#endregion

#region 친구 요청 받은 목록 가져오기
public class GetReceiveFriendsPacketRes : DefaultPacketRes
{
    public Friend[] myReceivedList;
}
#endregion

#region 친구 차단 목록 가져오기
public class GetBlockFriendsPacketRes : DefaultPacketRes
{
    public Member[] blockMembers;
}
#endregion

#region 친구 요청하기
public class FindFriendPacketRes : DefaultPacketRes
{
    public Friend member;
}
#endregion

#region 친구 즐겨찾기
public class BookmarkFriendPacketRes : DefaultPacketRes
{
    public int bookmark;
    public string bookmarkedAt;
}
#endregion

#region 친구 룸아이디 조회
public class FindRoomIdPacketRes
{
    public RoomId roomId;
}

public class RoomId
{
    public string roomId;
}
#endregion

//========= [ 사용자 관련 정보 가져오기 패킷 ] =========//

#region 사용자 관련 정보 가져오기
public class GetMemberInfoPacketRes : DefaultPacketRes
{
    public Dictionary<string, int> avatarInfos;
    public BizCardInfo[] businessCardInfos;
    public DefaultCardInfo defaultCardInfo;
    public MemberAccountInfo[] socialLoginInfo;
    public MyRoomList[] myRoomList;
    public MyRoomFrameImage[] myRoomFrameImages;
    public List<InteriorItemInvens> interiorItemInvens;
    public AvatarPartsInvens[] avatarPartsInvens;
    public List<MoneyInfos> moneyInfos;
    public List<MemberAdContents> memberAdContents;
}

public class AvatarPartsInvens
{
    public int itemId;
}
public class MoneyInfos
{
    public int moneyType;
    private int _count;
    public int count
    {
        get => _count;
        set
        {
            _count = value;
            if((MONEY_TYPE)moneyType == MONEY_TYPE.무료재화)
            {
                LocalPlayerData.Method.handlerMoneyInfos_JURI?.Invoke(_count);
            }
        }
    }
}

public class MemberAdContents
{
    public int contentsId;

    public MemberAdContents(int contentsId)
    {
        this.contentsId = contentsId;
    }
}
#endregion

//========= [ 앱 정보 가져오기 패킷 ] =========//

#region 앱 정보 가져오기
public class GetAppInfoPacketRes : DefaultPacketRes
{
    public OnfContentsInfo[] onfContentsInfo;
    public _BannerInfo[] bannerInfo;
    public _ScreenInfo[] screenInfo;
    public CSAFEventInfo csafEventInfo;
    public NoticeInfo[] noticeInfo;
}
public class _BannerInfo
{
    public int bannerId;
    public int uploadType;
    public string[] contents;
}

public class _ScreenInfo : ICloneable
{
    public int screenId;
    public int screenContentType;
    public string[] contents;

    public _ScreenInfo()
    {
    }
    public _ScreenInfo(int screenId)
    {
        this.screenId = screenId;
    }

    // ICloneable 인터페이스를 상속하여 구현한 DeepCopy 메서드
    public object Clone()
    {
        var copiedScreenInfo = new _ScreenInfo { screenId = this.screenId, screenContentType = this.screenContentType };

        if (this.contents != null)
        {
            copiedScreenInfo.contents = new string[this.contents.Length];
            Array.Copy(this.contents, copiedScreenInfo.contents, this.contents.Length);
        }

        return copiedScreenInfo;
    }
}


public class OnfContentsInfo
{
    public int onfContentsType;
    public int isOn;
}

public class CSAFEventInfo 
{
    public int id;
    public string name;
    public int eventSpaceType;
    public string startedAt;
    public string endedAt;
}

public class NoticeInfo 
{
    public int id;
    public int noticeType;
    public int noticeExposureType;
    public string subject;
    public string koLink;
    public string enLink;
    public string startedAt;
    public string endedAt;
}


#endregion

//========= [ 타인의 정보 가져오기 패킷 ] =========//

#region 타인의 정보 가져오기
public class MemberInfoPacketRes : DefaultPacketRes
{
    public Member othersMember;
}

public class Member
{
    public string memberCode;
    public int myRoomStateType;
    public string nickname;
    public string stateMessage;
    public Dictionary<string, int> avatarInfos;
    public BizCardInfo bizCard;
}
#endregion

//========= [ 투표 패킷 ] =========//

#region 하위 클래스
public class VoteInfo
{
    public int id; // 투표 아이디
    public string name; // 제목
    public string question; // 질문
    public string imageName; // 이미지 있을 시
    public int divType; // 투표 구분 타입 (양자택일, 선택)
    public int resType; // 투표 응답 타입 (단일 선택, 복수 선택)
    public int alterResType; // 투표 양일 응답 타입 (O/X, 찬성/ 반대)
    public int resultType; // 투표 결과 노출 타입 (득표율, 득표수, 득표율/득표수)
    public int isExposingResult; // 결과 중간 노출 여부
    public int isEnabledEdit; // 재투표 가능 여부
    public string startedAt; // 투표 시작 시간
    public string endedAt; // 투표 종료 시간
    public string resultEndedAt; // 결과 노출 종료 시간
    public int stateType; // 투표 상태 타입
}

public class VoteInfoExamples
{
    public int num;
    public string contents;
}

public class ResultInfo
{
    public int num;
    public int count;
}
#endregion

#region 투표 정보 가져오기
public class GetVoteInfoPacketRes : DefaultPacketRes
{
    public int isVote; // 내가 투표 했는지 여부
    public int isEnabledVoteInfo; // 투표가 가능한지 여부
    public int endedRemainDt; // 종료까지 남은 시간
    public int resultRemainDt; // 결과 노출 종료까지 남은 시간
    public VoteInfo voteInfo;
    public ResultInfo[] resultInfo; // 현재까지 결과
    public VoteInfoExamples[] VoteInfoExamples; // 선택지 정보
}
#endregion

//========== [실시간 서버 패킷] ==============//

#region 룸 생성 모듈 클래스 정의
/// <summary>
/// Reqeust 모듈을 담을 리스트 데이터
/// </summary>
//[System.Serializable, Preserve]
//public class MakeRooms
//{
//    public List<ModuleReq> modules;

//    public MakeRooms(List<ModuleReq> modules)
//    {
//        this.modules = modules;
//    }
//}


/// <summary>
/// type 지정해서 보내야됨
/// </summary>
[System.Serializable, Preserve]
public class ModuleReq
{
    public string type;
}

[System.Serializable, Preserve]
public class BaseModuleReq : ModuleReq
{
    public List<string> Scenes;
    public int Interval;

    public BaseModuleReq(string type, List<string> scenes, int interval)
    {
        this.type = type;
        Scenes = scenes;
        Interval = interval;
    }
}

[System.Serializable, Preserve]
public class ChatModuleReq : ModuleReq
{
    public ChatModuleReq(string type)
    {
        this.type = type;
    }
}

[System.Serializable, Preserve]
public class GameModuleReq : ModuleReq
{
    public string RoomName;
    public int MaxPlayerNumber;

    public GameModuleReq(string type, string roomName, int maxPlayerNumber)
    {
        this.type = type;
        RoomName = roomName;
        MaxPlayerNumber = maxPlayerNumber;
    }
}

[System.Serializable, Preserve]
public class MyroomModuleReq : ModuleReq
{
    public string MemberCode;

    public MyroomModuleReq(string type, string memberCode)
    {
        this.type = type;
        MemberCode = memberCode;
    }
}

/// <summary>
/// 오피스 만들기 요청 데이터 패킷
/// </summary>
public class OfficeModuleReq : ModuleReq
{
    public string RoomName;
    public string Description;
    public string RoomCode;
    public string Thumbnail;
    public string HostId;
    public string Password;
    public string SpaceInfoId;
    public int ModeType;
    public int TopicType;
    public int Personnel;
    public int Observer;
    public int RunningTime;
    public bool IsAdvertising;
    public bool IsWaitingRoom;

    public OfficeModuleReq(string type, OfficeRoomDataTest data)
    {
        this.type = type;
        RoomName = data.data.roomName;
        Description = data.data.description;
        RoomCode = data.data.roomCode;
        Thumbnail = data.data.thumbnail;
        HostId = data.hostId;
        Password = data.data.password;
        SpaceInfoId = data.data.spaceInfoId.ToString();
        ModeType = data.data.modeType;
        TopicType = data.data.topicType;
        Personnel = data.data.personnel;
        Observer = data.data.observer;
        RunningTime = data.data.runningTime;
        IsAdvertising = Convert.ToBoolean(data.data.isAdvertising);
        IsWaitingRoom = Convert.ToBoolean(data.data.isWaitingRoom);
    }
}
#endregion

#region 룸 생성 요청
/// <summary>
/// 게임방을 생성 할 때 서버한테 보내는 정보
/// </summary>
[System.Serializable, Preserve]
public class MakeGameRoomData
{
    public string serverUrl;

    // Base 
    public List<string> scenes;
    public int interval;

    // matching, ox
    public int maxPlayerNumber;
    public string serverType; //matching인지, ox인지
    public string roomName;
}

/// <summary>
/// 클라이언트가 회의실 만들기 요청할 때 보내는 정보들 [구버전], 
/// </summary>
[System.Serializable, Preserve]
public class MakeMeetingRoomData
{
    public string serverUrl;

    // Base 
    public List<string> scenes;
    public int interval;

    // Meeting
    public bool isPassword;
    public bool isPrivate;
    public int maxPlayerNumber;
    public string roomName;
    public string hostId;
    public string theme;
    public string description;
    public string password;
    public string officeType;
    public int time;
    public string thumbnailUrl;
    public bool initialChatPermission;
    public bool initialVideoPermission;
    public int worldId;
}


/// <summary>
/// 오피스 만들기 요청
/// </summary>
[System.Serializable, Preserve]
public class OfficeRoomDataTest
{
    public string url;
    public List<string> scenes;
    public int interval;
    public OfficeReservationInfo data;
    public string hostId;

    public OfficeRoomDataTest(string _url, List<string> _scenes, int _interval, OfficeReservationInfo _data, string _hostId)
    {
        this.url = _url;
        this.scenes = _scenes;
        this.interval = _interval;
        this.data = _data;
        this.hostId = _hostId;
    }
}




#endregion

#region 룸 데이터 받아오기

/// <summary>
/// 실시간 서버 방 정보 요청 후 처음 받아서 처리 할 때 사용하는 클래스
/// </summary>
[System.Serializable, Preserve]
public class ServerRoomsRes
{
    public string serverIP;

    public int serverPort;

    public string RoomId;

    public List<Dictionary<string, string>> Modules;
}

/// <summary>
/// 윈도우 가상서버로부터 받는 데이터 형식
/// </summary>
public class ServerRooms
{
    public string RoomId;

    public string ServerType;

    public List<Dictionary<string, string>> Modules;
}

#endregion

#region 룸 데이터 파싱

/// <summary>
/// 파싱 된 오피스 데이터
/// </summary>
[Serializable]
public class ServerOfficeInfoRes
{
    public List<ModuleData> Modules;
    public string ip;
    public int port;

    public OfficeReservationInfo info = new OfficeReservationInfo();
    public int currentPersonnel;
    public bool isShutdown;
    public bool isObserver;
    public string createdTime;
}


/// <summary>
/// 모듈
/// </summary>
[System.Serializable, Preserve]
public abstract class ModuleData
{
    public string type;

    public abstract void Parse(Dictionary<string, string> data);
}

/// <summary>
/// 오브젝트 동기화 모듈
/// </summary>
[System.Serializable, Preserve]
public class BaseModuleData : ModuleData
{
    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
    }
}


/// <summary>
/// 채팅이 필요한 서버 필요한 Chat 모듈
/// </summary>
[System.Serializable, Preserve]
public class ChatModuleData : ModuleData
{
    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
    }
}

/// <summary>
/// 메인 모듈(아즈월드, 부산월드 및 포탈 이동 시 사용)
/// </summary>
[System.Serializable, Preserve]
public class MainModuleData : ModuleData
{
    public int maxPlayerNumber;
    public int currentPlayerNumber;
    public string roomName;

    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
        maxPlayerNumber = int.Parse(data["maxPlayerNumber"]);
        currentPlayerNumber = int.Parse(data["currentPlayerNumber"]);
        roomName = data["roomName"];
    }
}

/// <summary>
/// 멀티 게임 모듈 (폴짝매칭, OX퀴즈)
/// </summary>
[System.Serializable, Preserve]
public class GameModuleData : ModuleData
{
    public int maxPlayerNumber;
    public int currentPlayerNumber;
    public string roomName;
    public string gameState;

    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
        maxPlayerNumber = int.Parse(data["maxPlayerNumber"]);
        currentPlayerNumber = int.Parse(data["currentPlayerNumber"]);
        roomName = data["roomName"];
        gameState = data["gameState"];
    }
}

/// <summary>
/// 마이룸 모듈 
/// </summary>
[System.Serializable, Preserve]
public class MyRoomModuleData : ModuleData
{
    public string memberCode;
    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
        memberCode = data["memberCode"];
    }
}

/// <summary>
/// 인터렉티브 모듈
/// </summary>
[System.Serializable, Preserve]
public class InteractionModuleData : ModuleData
{
    public override void Parse(Dictionary<string, string> data)
    {
        type = data["type"];
    }
}

#endregion

//========== [마이룸 패킷] ==============//

#region 마이룸 액자
public class MyRoomFrameImage_Res : DefaultPacketRes
{
    public MyRoomFrameImage[] frameImages;
}
public class MyRoomFrameImage
{
    public int itemId;
    public int num;
    public int uploadType;
    public string imageName;
    public MyRoomFrameImage DeepCopy()
    {
        return (MyRoomFrameImage)this.MemberwiseClone();
    }
    public MyRoomFrameImage(int itemId, int num)
    {
        this.itemId = itemId;
        this.num = num;
    }
}

//public class MyRoomFrameImages
//{
//    public MyRoomFrameImage[] myRoomFrameImages;
//}
#endregion

#region 마이룸 생성
public class MyRoomCreateReq
{
    public MyRoomList[] myRoomList;
}

public class MyRoomCreateRes : DefaultPacketRes
{
    public MyRoomList[] myRoomList;
}
#endregion

#region 타인의 마이룸 정보 조회
public class MyRoomOthersRoomListReq
{
    public string othersMemberCode;
}

public class MyRoomOthersRoomListRes : DefaultPacketRes
{
    public string othersMemberCode;
    public MyRoomList[] othersRoomList;
    public MyRoomFrameImage[] othersMyRoomFrameImages;
}
#endregion

#region 마이룸 하위 클래스
public class MyRoomList : IComparable<MyRoomList>
{
    public int itemId;
    public int num;
    public int layerType;
    public int x;
    public int y;
    public int rotation;
    public int CompareTo(MyRoomList other)
    {
        if (other == null) return 1;

        int itemIdCompare = itemId.CompareTo(itemId);

        if (itemIdCompare != 0) return itemIdCompare;

        int xCompare = this.x.CompareTo(other.x);
        if (xCompare != 0) return xCompare;

        return y.CompareTo(other.y);
    }
}

public class DeleteMyRoomList
{
    public int itemId;
    public int num;

    public DeleteMyRoomList(MyRoomList myRoomList)
    {
        this.itemId = myRoomList.itemId;
        this.num = myRoomList.num;
    }
}

/// <summary>
/// 인벤아이템 정보
/// </summary>
///
public class InteriorItemInvens
{
    public InteriorItemInvens()
    {
        this.itemId = -1;
    }
    public InteriorItemInvens(int itemId, int num)
    {
        this.itemId = itemId;
        this.num = num;
    }
    public int itemId;
    public int num;
}

public class MyRoomState : DefaultPacketRes
{
    public int myRoomStateType;
}
#endregion

//========== [퀘스트 패킷] ==============//

#region 퀘스트
//코어
public class QuestGetMemberInfoRes : DefaultPacketRes
{
    public MemberQuest[] memberQuest;
    public MemberQuest[] completedMemberQuest;
    public MemberQuestLog[] memberQuestLog;
}

public class QuestProcessReq
{
    public string memberId;
    public string jwtAccessToken;
    public string sessionId;
    public int questGroupType;
}

public class QuestProcessRes : DefaultPacketRes
{
    public MemberQuest MemberQuest;
    public MemberQuestLog MemberQuestLog;
}

public class QuestReceiveRewardRes : DefaultPacketRes
{
    public MemberAvatarInven memberAvatarInven;
    public MemberQuest[] memberQuest;
}

//재료
public class MemberQuest
{
    public int questId;
    public int isCompleted;
    public int isReceived;
}

public class MemberQuestLog
{
    public int questConditionType;
    public int count;
}

public class MemberAvatarInven
{
    public int avatarPartsId;
}
#endregion

#region 패킷생성 테스트
public class QuestInfoRes
{
    public QuestInfo[] questInfos;
}

//패킷생성 테스트
public class QuestInfo
{
    ///8 퀘스트이름 string questname
    ///9 퀘스트조건 string questmission
    ///10 보상타입이름 string rewardtype
    ///12 퀘스트현재단계 int curIdx
    ///12 퀘스트전체단계 int allIdx
    ///13 썸네일이름 string rewardImage
    public string questName;
    public string questMission;
    public string rewardType;
    public int curIdx;
    public int allIdx;
    public string thumbnail;
}
#endregion

//========== [AIChatbot 패킷] ==============//

#region AIChatbot
public class AIChatbotReq
{
    public string[] texts;
}
#endregion

//========== [유학박람회 CSAF 패킷] ==============//

#region 박람회 부스

/// <summary>
/// 부스 생성 Req
/// </summary>
public class CreateCSAFBoothReq
{
	public string name;
	public int topicType;
	public string description;
	public int spaceInfoId;
}

/// <summary>
/// 부스 생성 Res
/// </summary>
public class CreateCSAFBoothRes : DefaultPacketRes
{
    public Booth booth;
}

/// <summary>
/// 부스 조회 Res
/// </summary>
public class GetCSAFBoothsRes : DefaultPacketRes
{
    public Booth[] booths;
}

/// <summary>
/// 부스 상세 정보
/// </summary>
public class Booth
{
    public int id;
    public string roomCode;
    public string name;
    public int modeType;
    public int topicType;
    public string description;
    public int spaceInfoId;
    public string thumbnail;
    public string nickname;
    public string memberCode;
    public string memberId;
    public bool isHide;

    public Booth()
    {
        roomCode = string.Empty;
        name = string.Empty;
        description = string.Empty;
        topicType = 1;
        spaceInfoId = 5001;
    }
}

/// <summary>
/// 부스 간략 정보 (부스 조회시 정보)
/// </summary>
public class Booths
{
    public int id;
    public string roomCode;
    public string name;
    public int topicType;
    public string description;
    public int spaceInfoId;
    public string thumbnail;
    public string memberId;
    public string memberCode;
}

/// <summary>
/// 부스 수정 Req
/// </summary>
public class EditCSAFBoothsReq : CreateCSAFBoothReq
{
    public int isDelete;
}

/// <summary>
/// 부스 삭제 Req
/// </summary>
public class DeleteCSAFBoothRes : DefaultPacketRes
{
}

/// <summary>
/// 부스 항목 조회 Res
/// </summary>
public class GetCSAFBoothDetailRes : DefaultPacketRes
{
    public Booth booth;
    public BannerInfo[] bannerInfo;
    public ScreenInfo[] screenInfo;
}

/// <summary>
/// 부스 이름 조회 Res
/// </summary>
public class GetCSAFBoothNameRes : DefaultPacketRes
{
    public Booth[] booth;
}



//배너

/// <summary>
/// 배너 생성, 편집 Req
/// </summary>
public class BannerInfo
{
    public int boothId;
    public int bannerId;
    public int uploadType;
    public string uploadValue;
    public int interactionType;
    public string interactionValue;

    public BannerInfo(int boothId, int bannerId)
    {
        this.boothId = boothId;
        this.bannerId = bannerId;
        uploadType = -1;
        uploadValue = Cons.EMPTY;
        interactionType = 1;
        interactionValue = Cons.EMPTY;
    }
    public BannerInfo DeepCopy()
    {
        return (BannerInfo)this.MemberwiseClone();
    }
}



/// <summary>
/// 배너 생성, 편집 Res
/// </summary>
public class BannerInfoRes : DefaultPacketRes
{
    public BannerInfo bannerInfo;
}

/// <summary>
/// 배너 삭제 Res
/// </summary>
public class DeleteBannerRes : DefaultPacketRes
{
}

//스크린

/// <summary>
/// 스크린 생성, 편집 Req
/// </summary>
public class ScreenInfo
{
    public int boothId;
    public int screenId;
    public int uploadType;
    public string uploadValue;
    public int interactionType;
    public string interactionValue;
    public ScreenInfo(int boothId, int screenId)
    {
        this.boothId = boothId;
        this.screenId = screenId;
        uploadType = 2;
        uploadValue = Cons.EMPTY;
        interactionType = 1;
        interactionValue = Cons.EMPTY;
    }

    public ScreenInfo DeepCopy() => (ScreenInfo)MemberwiseClone();
}

/// <summary>
/// 스크린 생성, 편집 Res
/// </summary>
public class ScreenInfoRes : DefaultPacketRes
{
    public ScreenInfo screenInfo;
}

/// <summary>
/// 스크린 삭제 Res
/// </summary>
public class DeleteScreenRes : DefaultPacketRes
{
}

/// <summary>
/// 라이센스 Res
/// </summary>
public class GetLicenseInfoRes : DefaultPacketRes
{
}




#endregion

#region 파일함 목록 조회
public class GetCSAFFileboxPacketRes : DefaultPacketRes
{
    public CSAFFilebox[] fileboxes;
}

public class CSAFFilebox 
{
    public int id;
    public int boothId;
    public int fileBoxType;
    public string fileName;
    public string link;
    public string updatedAt;
}
#endregion

//========== [인앱결제 패킷] ==============//

#region 인앱결제
//인앱결제 영수증 원본
public class IAP_GoogleReceiptOrigin
{
    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
    public int quantity;
    public bool acknowledged;
}

//인앱결제 영수증 전송용
public class IAP_GoogleReceiptReq
{
    public string packageName;
    public string productId;
    public string purchaseToken;
}

public class IAP_GoogleReceiptRes
{
    public string purchaseTimeMillis;
    public int purchaseState;
    public int consumptionState;
    public string developerPayload;
    public string orderId;
    public int purchaseType;
    public int acknowledgementState;
    public string kind;
    public string regionCode;
}
#endregion 

//========== [패킷] ==============//

#region 오피
public class Office_CreateOfficeReservReq
{
    public int modeType;
    public string name;
    public int topicType;
    public int alarmType;
    public string description;
    public string password;
    public int spaceInfoId;
    public int runningTime;
    public int personnel;
    public int observer;
    public string reservationAt;
    public int startTime;
    public int repeatDay;
    public int isAdvertising;
    public int isWaitingRoom;

    public Office_CreateOfficeReservReq()
    {
    }
}

public class Office_OfficeInfo
{
    public string password;
    public int runningTime;
    public int personnel;
}

public class Office_CreateOfficeReservRes : DefaultPacketRes
{
    public OfficeReservationInfo memberReservationInfo;
}


public class ExpositionReservationInfo : BaseReservationInfo
{

}
public class BaseReservationInfo : DefaultPacketRes
{
    public string roomName;
    public int topicType;
    public string description;
    public int spaceInfoId;
}


/// <summary>
/// 룸 예약정보 가져오기
/// </summary>
[Serializable]
public class OfficeReservationInfo : BaseReservationInfo
{
    public string roomCode;
    public int modeType;
    public int runningTime;
    public int personnel;
    public int alarmType;
    public int isAdvertising;
    public int isWaitingRoom;
    public int observer;
    public string thumbnail;
    public string reservationAt;
    public int repeatDay;
    public int startTime;
    public int isDelete;
    public string nickName;
    public string memberCode;
    public string password;
    public int isPassword;

    public OfficeReservationInfo()
    {

    }
    public OfficeReservationInfo(OfficeModeType eOfficeModeType)
    {
        db.OfficeMode officeMode = Single.MasterData.dataOfficeMode.GetData((int)eOfficeModeType);

        this.roomName = "";
        this.roomCode = "";
        this.modeType = officeMode.modeType;
        this.topicType = 0;
        this.description = "";
        this.runningTime = officeMode.minGap;
        this.spaceInfoId = 0;
        //this.personnel = 6;
        this.personnel = 0;
        this.alarmType = 0;
        this.isAdvertising = 0;
        this.isWaitingRoom = 0;
        this.observer = 0;
        this.thumbnail = "";
        this.reservationAt = DateTime.Today.ToString("yyyy-MM-dd");
        this.repeatDay = 0;
        this.startTime = Util.HourMin2Min(DateTime.Now.Hour, DateTime.Now.Minute + 5);
        this.password = "";
        this.nickName = LocalPlayerData.NickName;
        //this.isDelete = 1;

        //this.memberCode = memberCode;
    }
}

public class OfficeRoomReservationWebInfo
{
    public OfficeRoomInfo officeRoomInfo;
}

public class OfficeRoomInfo
{
    public string roomCode;
    public string roomName;
    public int modeType;
    public int topicType;
    public string description;
    public int runningTime;
    public int spaceInfoId;
    public int personnel;
    public int alarmType;
    public int isAdvertising;
    public int isWaitingRoom;
    public int observer;
    public string thumbnail;
    public string reservationAt;
    public int repeatDay;
    public int startTime;
    public string nickName;
    public int isPassword;
    public string memberCode;
    public int isDelete;
}

public class Office_CreateRoomCodeRes
{
    public string roomCode;
}

public class Office_WaitOfficeReservReq
{
    public string roomCode;
}

public class Office_CheckRoomPassword
{
    public string roomCode;
    public string password;
}


public class Office_WaitOfficeReservRes : DefaultPacketRes
{
}

public class Office_GetReservationInfo : DefaultPacketRes
{
    public OfficeReservationInfo[] myReservations;
}

public class AdvertisingOfficeListRes : DefaultPacketRes
{
    public OfficeReservationInfo[] advertisingOfficeList;
}

public class Office_GetReservationWaitInfo : DefaultPacketRes
{
    public OfficeReservationInfo[] myWaitings;
}

public class Office_CheckRoomCodePasswordReq
{
    public string roomCode;
    public string password;
}

public class Office_CheckRoomCodePasswordRes : DefaultPacketRes
{
}



#endregion

#region 심심이_SimSImi
public class ErrorSimSimi
{
    public ErrorSimSimiArray[] errors;
}
public class ErrorSimSimiArray
{
    public string msg;
    public string param;
    public string location;
}
public class ReqSimSimi
{
    public string utext;
    public string lang;

    public ReqSimSimi(string utext, string lang)
    {
        this.utext = utext;
        this.lang = lang;
    }
}
public class ResSimSimi
{
    public int status;
    public string statusMessage;
    public Request request;
    public string atext;
    public string lang;
}
public class Request
{
    public string utext;
    public string lang;
}
#endregion

#region ChatGPT

public class ReqChatGPT
{
    public string model;
    public List<Message> messages;
    public float temperature;
    public int max_tokens;
    public float top_p;

    public ReqChatGPT(string message)
    {
        model = "gpt-3.5-turbo-1106";
        LocalPlayerData.currentMessage.Add(new Message("user", message));
        messages = LocalPlayerData.currentMessage;
        temperature = 0;
        max_tokens = 200;
        top_p = 1;
    }
}
public class Message
{
    public string role;
    public string content;

    public Message(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}
public class ResChatGPT
{

    public string id;
    public string @object;
    public long created;
    public string model;
    public Choice[] choices;
    public Usage usage;
    public string systemFingerprint;
}

public class Choice
{
    public int index;
    public Message message;
    public string finishReason;
}

public class Usage
{
    public int promptTokens;
    public int completionTokens;
    public int totalTokens;
}

#endregion

#region 선물함
public class PostboxItem
{
    public int appendType;
    public int appendValue;
    public int count;
    public int orderNum;
}
public class Postbox
{
    public int id;
    public string subject;
    public string summary;
    public string content;
    public string postalTypeName;
    public int period;
    public string createdAt;
    public string sendedAt;
    public PostboxItem item;
}
public class PostboxRes : DefaultPacketRes
{
    public List<Postbox> postboxes;
}
public class ReceivedItem
{
    public int itemId;
    public int num;
    public int itemType;
}
public class PostboxRecieveRes : DefaultPacketRes
{
    public int id;
    public ReceivedItem[] receivedItems;
}
public class PostboxReceiveAllRes : DefaultPacketRes
{
    public int[] ids;
    public ReceivedItem[] receivedItems;
}
#endregion

#region 숏링크 
public class CreateShortLink
{
    public int linkType;
    public string roomCode;
    public string mobileDynamicLink;
    public string pcDynamicLink;
}

public class GetShortLink
{
    public string dynamicLink;
}
#endregion

#region 재화, 코인
public class ADContentsRes : DefaultPacketRes
{
    public int contentsId;
    public int moneyType;
    public int count;
}
public class GetMoneyInfoRes : DefaultPacketRes
{
    public MoneyInfos[] moneyInfos;
}
#endregion




public class NoticeMember
{
    public DateTime lastCheckedDate;
    public eNoticeExposureType noticeExposureType;
    public eNoticeExposureState noticeExposureState;
    public NoticeMember(int noticeExposureType)
    {
        this.lastCheckedDate = DateTime.Now;
        this.noticeExposureType = (eNoticeExposureType)noticeExposureType;
        this.noticeExposureState = eNoticeExposureState.reset;
    }
}