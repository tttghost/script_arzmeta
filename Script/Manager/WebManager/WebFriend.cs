using FrameWork.Network;
using System;

public class WebFriend 
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    #region Friend 목록들 가져와서 로컬에 바로 저장하기
    /// <summary>
    /// 친구 목록 가져와서 로컬에 바로 저장하기
    /// </summary>
    public void GetFriendsToLocal(Action action = null)
    {
        GetFriends((res) => { LocalPlayerData.Method.friends = res.friends; action?.Invoke(); });
    }

    /// <summary>
    /// 친구 요청 보낸 목록 가져와서 로컬에 바로 저장하기
    /// </summary>
    public void GetRequestFriendsToLocal(Action action = null)
    {
        GetRequestFriends((res) => { LocalPlayerData.Method.requestFriends = res.myRequestList; action?.Invoke(); });
    }

    /// <summary>
    /// 친구 요청 받은 목록 가져와서 로컬에 바로 저장하기
    /// </summary>
    public void GetReceiveFriendsToLocal(Action action = null)
    {
        GetReceiveFriends((res) => { LocalPlayerData.Method.receivefriends = res.myReceivedList; action?.Invoke(); });
    }

    /// <summary>
    /// 친구 차단 목록 가져와서 로컬에 바로 저장하기
    /// </summary>
    public void GetBlockFriendsToLocal(Action action = null)
    {
        GetBlockFriends((res) => { LocalPlayerData.Method.blockfriends = res.blockMembers; action?.Invoke(); });
    }
    #endregion

    /// <summary>
    /// 친구 목록 가져오기
    /// </summary>
    public void GetFriends(Action<GetFriendsPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetFriends, dim: false), _res, _error);
    }

    /// <summary>
    /// 친구 요청 하기 1: nickname, 2: memberCode 
    /// </summary>
    public void RequestFriend(string friendId, int requestType, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            friendId,
            requestType
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.RequestFriend, packet), _res, _error);
    }

    /// <summary>
    /// 친구 요청 보낸 목록 가져오기
    /// </summary>
    public void GetRequestFriends(Action<GetRequestFriendsPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetRequestFriends, dim: false), _res, _error);
    }

    /// <summary>
    /// 친구 요청 받은 목록 가져오기
    /// </summary>
    public void GetReceiveFriends(Action<GetReceiveFriendsPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetReceiveFriends, dim: false), _res, _error);
    }

    /// <summary>
    /// 친구 수락 하기
    /// </summary>
    public void AcceptFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.AcceptFriend(friendMemeberCode)), _res, _error);
    }

    /// <summary>
    /// 친구 요청 취소하기
    /// </summary>
    public void CancelRequestFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.CancelRequestFriend(friendMemeberCode)), _res, _error);
    }

    /// <summary>
    /// 친구 요청 거절 하기
    /// </summary>
    public void RefusalRequestFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.RefusalRequestFriend(friendMemeberCode)), _res, _error);
    }

    /// <summary>
    /// 친구 차단 하기
    /// </summary>
    public void BlockFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            friendMemeberCode
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.BlockFriend, packet), _res, _error);
    }

    /// <summary>
    /// 친구 삭제 하기
    /// </summary>
    public void DeleteFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.DeleteFriend(friendMemeberCode)), _res, _error);
    }

    /// <summary>
    /// 친구 차단 해제 하기
    /// </summary>
    public void ReleaseBlockFriend(string friendMemeberCode, Action<DefaultPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.ReleaseBlockFriend(friendMemeberCode)), _res, _error);
    }

    /// <summary>
    /// 친구 차단 목록 가져오기
    /// </summary>
    public void GetBlockFriends(Action<GetBlockFriendsPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.GetBlockFriends, dim: false), _res, _error);
    }

    /// <summary>
    /// 친구 찾기 1: nickname, 2: memberCode 
    /// </summary>
    public void FindFriend(int requestType, string friendId, Action<FindFriendPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.FindFriend(requestType, friendId), dim: false), _res, _error);
    }

    /// <summary>
    /// 친구 즐겨찾기
    /// </summary>
    public void BookmarkFriend(string friendMemeberCode, Action<BookmarkFriendPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            friendMemeberCode
        };

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.BookmarkFriend, packet), _res, _error);
    }

    /// <summary>
    /// 친구 룸아이디 조회
    /// 친구 따라가기 시 상대방이 있는 장소 알아낼 때 사용
    /// </summary>
    public void FindFriendRoomId(string friendMemeberCode, Action<FindRoomIdPacketRes> _res, Action<DefaultPacketRes> _error = null)
    {
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.FindFriendRoomId(friendMemeberCode), dim: false), _res, _error);
    }

}
