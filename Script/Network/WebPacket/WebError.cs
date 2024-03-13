using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Launching.DEV.Script.Framework.Network.WebPacket
{
    public enum WEBERROR
    {
        NET_E_SUCCESS = 200, // 성공
        NET_E_NOT_LOGINED = 202, //로그인 되지 않음
        NET_E_EMPTY_TOKEN = 204, // 토큰이 비어있음
        NET_E_EXPIRED_TOKEN = 206, // 토큰 만료
        NET_E_INVALID_TOKEN = 208, // 유효하지 않은 토큰

        NET_E_IS_DORMANT_ACCOUNT = 240, // 휴면 계정 입니다
        NET_E_DUPLICATE_LOGIN = 242, // 중복 로그인
        NET_E_ALREADY_DELETE_USER_ID = 244, // 계정 탈퇴한 사용자 아이디
        NET_E_ALREADY_EXIST_EMAIL = 246, // 이미 존재하는 이메일
        NET_E_ALREADY_EXIST_NICKNAME = 248, // 이미 존재하는 닉네임
        NET_E_ALREADY_MY_NICKNAME = 249, // 이미 나의 닉네임
        NET_E_NOT_MATCH_PASSWORD = 250, // 패스워드 불일치
        NET_E_NOT_EXIST_USER = 252, // 존재하지 않는 사용자
        NET_E_NOT_MATCH_EMAIL_AUTH_CODE = 254, // 존재하지 않는 이메일 인증 코드
        NET_E_NOT_EXIST_EMAIL = 256, // 존재하지 않는 이메일
        NET_E_NOT_AUTH_EMAIL = 258, // 인증 되지 않은 이메일
        NET_E_ALREADY_EXIST_EMAIL_FOR_ARZMETA_LOGIN = 260, //이미 자체 로그인 가입 된 이메일 계정 사용자
        NET_E_EMPTY_PASSWORD = 262, // 패스워드가 없음.
        NET_E_CANNOT_UPDATED_EMAIL = 264, // 이메일 업데이트 불가 (1달에 1번)
        NET_E_SAME_PREVIOUS_EMAIL = 266, // 이메일이 변경 되지 않았음 (기존 이메일과 같다)
        NET_E_SOCIAL_LOGIN_USER = 268, // 소셜 로그인 사용자 입니다.
        NET_E_INVALID_EMAIL = 270, // 유효하지 않은 이메일 입니다.
        NET_E_OVER_COUNT_EMAIL_AUTH = 272, // 이메일 인증 횟수를 초과 하였습니다.
        NET_E_ALREADY_PROVIDER_TYPELINKED_ACCOUNT = 274, // 이미 연동된 계정 입니다.
        NET_E_CANNOT_RELEASE_LINKED_ACCOUNT = 276, // 계정 연동 해제 불가.
        NET_E_MAX_OVER_BUSINESS_CARD = 278, // 비지니스 명함 갯수 초과  
        NET_E_ERROR_BUSINESS_CARD_ID = 280, // 비지니스 명함 아이디 에러
        NET_E_ALREADY_ACCOUNT = 282, // 이미 존재하는 계정  
        NET_E_ALREADY_LINKED_OTHER_ACCOUNT = 284, // 이미 다른계정에 연동되어 있습니다.
        NET_E_NOT_EXIST_BUSINESS_CARD = 286, // 존재하지 않는 비지니스 명함  
        NET_E_IS_WITHDRAWAL_MEMBER = 288, // 탈퇴 진행중인 계정입니다.

        NET_E_ALREADY_FRIEND = 302, // 이미 친구 입니다.
        NET_E_ALREADY_RECEIVED_FRIEND_REQUEST = 304, // 이미 친구 요청을 받았습니다.
        NET_E_ALREADY_SEND_FRIEND_REQUEST = 306, // 이미 친구 요청을 보냈습니다.
        NET_E_NOT_EXIST_RECEIVED_REQUEST = 308, // 받은 요청이 없습니다.
        NET_E_NOT_EXIST_REQUEST = 310, // 보낸 요청이 없습니다.
        NET_E_MEMBER_IS_BLOCK = 312, // 차단 된 사용자 입니다.
        NET_E_MY_FRIEND_MAX_COUNT = 314, // 나의 친구 수 초과  
        NET_E_TARGET_FRIEND_MAX_COUNT = 316, // 상대의 친구 수 초과
        NET_E_CANNOT_BLOCK_MYSELF = 318, // 자기 자신은 차단 할 수 없음.  
        NET_E_CANNOT_REQUEST_MYSELF = 320, // 자기 자신은 친구 요청을 보낼 수 없음.

        NET_E_NOT_HAVE_ITEM = 340, // 소유하지 않은 아이템
        NET_E_ITEM_OVER_COUNT = 342, // 아이템 갯수 초과
        NET_E_NOT_MATCH_ITEM = 344,  // 아이템이 일치하지 않습니다.
        NET_E_ITEM_NOT_REMOVABLE = 346,  // 아이템을 배치 해제 할 수 없다.

        NET_E_NOT_SET_RESERVATION_TIME = 350, // 예약 시간 설정이 되지 않았습니다.
        NET_E_DUPLICATE_RESERVATION_TIME = 352, // 예약 시간 설정이 중복 되었습니다.
        NET_E_WRONG_RESERVATION_TIME = 354, // 예약 시간 설정이 잘못 되었습니다.
        NET_E_OFFICE_GRADE_AUTHORITY = 356, // 오피스 권한이 잘못되었습니다.
        NET_E_OVER_CREATE_OFFICE_RESERVATION_COUNT = 358, // 더 이상 룸을 예약 할 수 없습니다.
        NET_E_ERROR_SELECT_OFFICE_ROOM_INFO = 360, // 오피스 룸 선택 오류
        NET_E_OVER_MAX_PERSONNEL = 362, // 최대 인원 초과
        NET_E_OVER_RUNNING_TIME = 364, // 진행 시간 초과
        NET_E_CANNOT_SET_THUMBNAIL = 366, // 썸네일 설정 불가
        NET_E_CANNOT_SET_ADVERTISING = 368, // 홍보노출 설정 불가
        NET_E_CANNOT_SET_WAITING_ROOM = 370, // 대기실 설정 불가
        NET_E_NOT_EXIST_OFFICE = 372, // 존재 하지 않는 오피스
        NET_E_NOT_EXIST_WAITING = 374, // 존재 하지 않는 대기실
        NET_E_OFFICE_CREATE_ME = 376, // 내가 만든 오피스
        NET_E_CANNOT_OFFICE_SET_OBSERVER = 378, // 관전 인원 설정 불가  
        NET_E_OVER_MAX_OFFICE_SET_OBSERVER = 380, // 관전 인원 설정 초과
        NET_E_OFFICE_ALREADY_WAITING_USER = 386, // 이미 대기중인 사용자 입니다.

        NET_E_ALREADY_EXIST_MYROOM_ITEM = 390, // 마이룸에 있는 아이템
        NET_E_NOT_EXIST_MYROOM_ITEM = 392, // 마이룸에 없는 아이템
        NET_E_NOT_EXIST_FURNITURE_INVEN_ITEM = 394, // 가구 인벤에 없는 아이템
        NET_E_CANNOT_DELETE_ITEM = 396, // 삭제 불가능 아이템

        NET_E_NOT_EXIST_NOTICE = 402, // 존재 하진 않는 공지사항 입니다.
        NET_E_BAD_PASSWORD = 404, // 잘못된 패스워드 형식 입니다.
        NET_E_CANNOT_VOTE = 426, // 투표 불가.
        NET_E_TOO_MANY_RESPONSE = 428, // 투표 응답 갯수가 너무 많습니다.
        NET_E_ALREADY_VOTE = 430, // 이미 투표를 했습니다.
        NET_E_WRONG_RESPONSE = 432, // 잘못된 응답입니다.
        NET_E_NOT_EXIST_VOTE = 434, // 존재하지 않는 투표입니다.
        NET_E_NOT_EXIST_PROGRESS_VOTE = 436, // 진행 중인 투표가 없습니다.

        NET_E_CANNOT_RECEIVED_POST = 450, // 우편을 수령할 수 없습니다.
        NET_E_NOT_EXIST_POST = 452, // 존재하지 않는 우편입니다..

        NET_E_NOT_EXIST_IMAGE_URL = 470, // 이미지 URL 이 없습니다.
        NET_E_NOT_EXIST_IMAGE_FILE = 472, // 이미지 파일이 없습니다.
        NET_E_BAD_IMAGE = 474, // 부적절한 이미지 입니다.

        NET_E_ALREADY_LINKED_SAME_WALLET_ADDR = 480, // 이미 연동된 지갑 주속와 같은 지갑 주소 입니다.
        NET_E_ALREADY_EXISTS_LINKED_WALLET_ADDR = 482, // 나의 계정에 이미 지갑이 연동 되어 있는 경우.
        NET_E_ALREADY_EXISTS_LINKED_ACCOUNT = 484, // 지갑 주소가 이미 다른 계정과 연동되어 있는 경우
        NET_E_NOT_EXISTS_LINKED_WALLET_ADDR = 486, // 연동된 지갑 주소가 없음.

        NET_E_NOT_EXISTS_BOOTH = 490, // 존재 하지 않는 부스 입니다.
        NET_E_NOT_EXISTS_EVENT = 491, // 존재 하지 않는 이벤트 입니다.
        NET_E_HAVE_NOT_LICENSE_MEMBER = 492, // 라이선스를 보유지 않은 회원 입니다.
        NET_E_NOT_EXIST_FILE = 494, // 파일이 없습니다.
        NET_E_NOT_EXIST_EVENT = 496, // 진행중인 행사가 없습니다.
        NET_E_UNAUTHORIZE_ADMIN = 498, // 권한이 없습니다.

        NET_E_DB_FAILED = 600, // DB 실패
        NET_E_SERVER_INACTIVATE = 700, // 서버 비활성
        NET_E_NEED_UPDATE = 710 // 업데이트 필요
    }
}

public static class WebError
{
    public static string ToString(int error)
    {
        return ((WEBERROR)error).ToString();
    }
}
