
using System;
using System.Collections.Generic;
using UnityEngine;

#region 보류
/// <summary>
/// 공지사항 (뉴스) 아이템 데이터
/// </summary>
public class Item_FAQData : DynamicScrollData
{
    public int id;

    public int notcieType;
    public int viewCount;
    public string title;
    public string contents;

    public bool isExpanded = false;
    public float collapsedSize;
    public float expandedSize;

    public float Size
    {
        get { return isExpanded ? expandedSize : collapsedSize; }
    }
}

/// <summary>
/// 공지사항 아이템 데이터
/// </summary>
public class Item_NoticeData : DynamicScrollData
{
    public int id;
    public string subject;
    public string content;
    public string createdAt;
    public string updatedAt;
    public int viewCount;
    public int notcieType;
    public bool isTopFix;
    public bool isSearch;

    public bool isExpanded = false;
    public float collapsedSize;
    public float expandedSize;

    public float Size
    {
        get { return isExpanded ? expandedSize : collapsedSize; }
    }
}
#endregion

/// <summary>
/// 아이템 데이터 베이스 클래스 
/// </summary>
public class Item_Data { }

/// <summary>
/// 아바타 아이템
/// </summary>
public class Item_CostumeData : Item_Data
{
    public int id;
    public bool isReset;
    public bool isSpecial; // NFT 스페셜인지
    public string thumbnail;
    public int purchaseType;
    public int salespurchaseType; // NFT 구매판매 타입
    public Vector2 cellSize;

    public Action<int> action;
}

/// <summary>
/// 커머스 존 아이템
/// </summary>
public class Item_CommerceData : Item_Data
{
    public int id;
    public StoreAvatarParts costum;
}

/// <summary>
/// 아바타 프리셋 아이템
/// </summary>
public class Item_AvatarPresetData : Item_Data
{
    public int presetId;
    public RenderTexture texture;
    public object leanManualRotate;
    public string localId;
}

/// <summary>
/// 친구 목록 아이템
/// </summary>
public class Item_FriendData : Item_Data
{
    public string memberId;
    public string memberCode;
    public string nickname;
    public string message;
    public string createdAt;
    public Dictionary<string, int> avatarInfos;
    public bool isOnline;

    // 고정 여부
    public bool bookmark;
    // 고정일시
    public string bookmarkedAt;

    public Action updateAction; 
}

/// <summary>
/// 신규 차단, 친구 검색, 차단 목록, 친구 요청 받은 목록, 친구 요청 보낸 목록 아이템
/// </summary>
public class Item_ManageFriendData : Item_Data
{
    public string memberCode;
    public string nickname;
    public string message;
    public string createdAt;
    public Dictionary<string, int> avatarInfos;
    public FRIEND_TYPE type;

    public Action updateAction;
}

/// <summary>
/// 비즈니스 명함 아이템
/// </summary>
public class BusinessCardData
{
    public int id; // 아이디
    public int purchaseType; // 구매 타입
    public int price; // 가격
    public string thumbnailName; // 썸네일 이름

    public string name; // 이름
    public int phone; // 전화번호
    public string email; // 이메일
    public string addr; // 주소
    public string fax; // 팩스
    public string job; // 직업
    public string position; // 직책
    public string intro; // 소개
}

public class MeetingItemData : DynamicScrollData
{
    public RoomInfoRes ServerInfo;

    public MeetingItemData()
    {
        ServerInfo = new RoomInfoRes();
    }
}

public class MeetingGuestUiUserItemData : DynamicScrollData
{
    public string ClientId;
    public string NickName;
}

public class OfficeItem : DynamicScrollData
{
    public eOpenType openType;

    public OfficeRoomInfoRes roomInfo = new OfficeRoomInfoRes();
    public OfficeRoomReservationInfo reservationInfo = new OfficeRoomReservationInfo();
}

public class OfficeReservationItem : DynamicScrollData
{
    public string roomCode;
}

public class ItemGiftMailItemData : Item_Data
{
    public Postbox postbox;
}

public class ItemGiftMailGetItemData : Item_Data
{
    public int id;
    public int count;
}

public class Item_ExpositionBooth : Item_Data
{
	public Booth boothInfo = new Booth();
}

/// <summary>
/// 국제 유학박람회 파일함 아이템
/// </summary>
public class Item_ExpositionFileData : Item_Data
{
    public int boothId;
    public int id;
    public int fileBoxType;
    public string fileName;
    public string link;
    public string updatedAt;

    public bool IsAdmin;
}