using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Launching.DEV.Script.Framework.Network.WebSocket
{
    public enum WebSocketError
    {
        // Chatting
        NOT_EXIST_USER = 2100,              // 귓속말(월드, 1:1 ) 대상이 존재하지 않을 경우
        OFFLINE_USER = 2101,                // 귓속말(월드, 1:1 ) 대상이 현재 오프라인 상태일 경우
        SEND_MYSELF = 2102,                 // 귓속말(월드, 1:1 )을 본인에게 보냈을 경우
        
        DROP_PLAYER = 10001,                // 최신 세션아이디가 아닌 유저일 경우
        DUPLICATE = 10000,                // 최신 세션아이디가 아닌 유저일 경우

        // Office
        OFFICE_WAIT_SUCCESS = 30000,        // 회의실예약 대기 성공
        OFFICE_NOT_EXIST_USER = 30001,      // 존재하지 않는 유저
        OFFICE_ALREADY_WAIT = 30002,        // 이미 예약 대기중인 회의실 일때
        OFFICE_WAIT_OVERFLOW = 30003,       // 예약 인원이 초과 되었을때
        OFFICE_NOT_EXIST_ROOM = 30004,      // 존재하지 않는 회의실일 경우
        OFFICE_WAIT_EXIT = 30005,           // 회의실 예약 대기 나가기
        OFFICE_NOTICE_CREATE_ROOM = 30006,  // 회의실 방 생성 알림
        OFFICE_NOTICE_DELETE_ROOM = 30007,  // 회의실 방 삭제 알림

        ENTER_ALREADY_CONNECTED = 31000,    // 방 입장시, 이미 접속중인 방 일때

        // Friend
        NOT_FRIEND = 40001,                 // 친구가 아닐 경우
        OFFLINE_FRIEND = 40002,             // 친구가 오프라인일 경우
        NOT_EXIST_USER_FRIEND = 40003,      // 존재하지 않는 친구 사용자인 경우
        FOLLOW_SUCCESS = 400010,            // 친구 따라가기 성공 시 
        BRING_SUCCESS = 40011,              // 친구 불러오기 성공 시 
        DO_NOT_ENTER_SCENE = 40012,         // 입장할 수 없는 씬일 경우

        RECONNECT_FAILED = 100000,          // 클라이언트 재연결 시도 후에도 연결이 안 될 때
    }
}
