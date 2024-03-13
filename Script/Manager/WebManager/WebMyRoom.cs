using FrameWork.Network;
using FrameWork.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WebMyRoom
{
    private string ContentServerUrl => Single.Web.ContentServerUrl;

    /// <summary>
    /// 플레이모드와 에딧모드 비교
    /// </summary>
    /// <returns></returns>
    public bool EqualPlaymodeAndEditmode()
    {
        return Util.EqualMyRoomList(LocalPlayerData.Method.PlayMyRoomList, LocalPlayerData.Method.EditMyRoomList);
    }

    /// <summary>
    /// 에딧모드와 그리드모드 비교
    /// </summary>
    /// <returns></returns>
    public bool EqualEditmodeAndGrid()
    {
        return Util.EqualMyRoomList(LocalPlayerData.Method.EditMyRoomList, MyRoomManager.Instance.gridSystem.GetGridData2MyRoomList());
    }


    /// <summary>
    /// 마이룸 정보 저장
    /// </summary>
    /// <param name="myRoomList"></param>
    public void MyRoomCreate_Req()
    {
        MyRoomList[] PlayMyRoomList = LocalPlayerData.Method.PlayMyRoomList;
        MyRoomList[] EditMyRoomList = LocalPlayerData.Method.EditMyRoomList;

        List<MyRoomList> createMyRoomDatas = new List<MyRoomList>();
        List<MyRoomList> updateMyRoomDatas = new List<MyRoomList>();
        List<DeleteMyRoomList> deleteMyRoomDatas = new List<DeleteMyRoomList>();


        foreach (var EditMyRoom in EditMyRoomList)
        {
            int num = EditMyRoom.num;
            MyRoomList existMyRoom = PlayMyRoomList.SingleOrDefault(x => x.num == num);
            if (existMyRoom == null)
            {
                createMyRoomDatas.Add(EditMyRoom); //C
            }
            else
            {
                if (!Util.EqualMyRoomList(existMyRoom, EditMyRoom))
                {
                    updateMyRoomDatas.Add(EditMyRoom); //U
                }
            }
        }

        foreach (var PlayMyRoom in PlayMyRoomList)
        {
            int num = PlayMyRoom.num;
            MyRoomList existMyRoom = EditMyRoomList.SingleOrDefault(x => x.num == num);
            if (existMyRoom == null)
            {
                deleteMyRoomDatas.Add(new DeleteMyRoomList(PlayMyRoom)); //D
            }
        }

        var myRoomCreateReq = new
        {
            createMyRoomDatas,
            updateMyRoomDatas,
            deleteMyRoomDatas,
        };

        Single.Web.SendRequest<MyRoomCreateRes>(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.MyRoomCreate_Req, myRoomCreateReq), MyRoomCreate_Res, Single.Web.Error_Res);
    }

    private void MyRoomCreate_Res(MyRoomCreateRes myRoomCreateRes)
    {
        {
            bool isPlayMode = Scene.MyRoom.myRoomMode == eMyRoomMode.PLAYMODE;
            MyRoomManager.Instance.myroomModuleHandler.C_MYROOM_END_EDIT(isPlayMode);
        }
    }

    /// <summary>
    /// 다른사람 마이룸 불러오기
    /// </summary>
    /// <param name="callback"></param>
    public void MyRoomOthersRoomList_Req(string roomCode, Action<MyRoomOthersRoomListRes> callback, bool isDim = true)
    {
        //Debug.Log("roomCode : " + roomCode);
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.GET, ContentServerUrl, WebPacket.MyRoomOthersRoomList_Req(roomCode), dim: isDim), callback, Single.Web.Error_Res);
    }


    /// <summary>
    /// 마이룸 상태 타입 변경 1: 공개 public , 2: 비공개 private
    /// </summary>
    /// <param name="myRoomStateType"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void MyRoomChangeState(int myRoomStateType, Action<MyRoomState> _res, Action<DefaultPacketRes> _error = null)
    {
        var packet = new
        {
            myRoomStateType
        };
        Single.Web.SendRequest(new SendRequestData(eHttpVerb.PUT, ContentServerUrl, WebPacket.MyRoomChangeState, packet), _res, Single.Web.Error_Res);
    }

    /// <summary>
    /// 마이룸 이미지 업로드
    /// </summary>
    public void MyRoomFrameImageUpload(MyRoomFrameImage myRoomFrameImage, Texture2D iosTex = null, Action<MyRoomFrameImage_Res> _res = null, Action<DefaultPacketRes> _error = null)
    {
        switch ((FRAMEIMAGEAPPEND_TYPE)myRoomFrameImage.uploadType)
        {
            case FRAMEIMAGEAPPEND_TYPE.로컬이미지:
                {
                    var data = new
                    {
                        myRoomFrameImage.itemId,
                        myRoomFrameImage.num,
                        myRoomFrameImage.uploadType,
                    };
                    Single.Web.SendWebRequest_MultiPartFormData(new SendRequestData_MultiPartForm(eHttpVerb.POST, ContentServerUrl, WebPacket.MyRoomFrameImageUpload, data, thumbnailPath: myRoomFrameImage.imageName, iosTex: iosTex), _res, _error);
                }
                break;
            case FRAMEIMAGEAPPEND_TYPE.URL이미지:
                {
                    var data = new
                    {
                        itemId = myRoomFrameImage.itemId.ToString(),
                        num = myRoomFrameImage.num.ToString(),
                        uploadType = myRoomFrameImage.uploadType.ToString(),
                        imageUrl = myRoomFrameImage.imageName, //이부분 다름
                    };
                    Single.Web.SendRequest(new SendRequestData(eHttpVerb.POST, ContentServerUrl, WebPacket.MyRoomFrameImageUpload, data), _res, _error);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 마이룸 이미지 삭제
    /// </summary>
    /// <param name="num"></param>
    /// <param name="_res"></param>
    /// <param name="_error"></param>
    public void MyRoomFrameImageDelete(MyRoomFrameImage myRoomFrameImage, Action<MyRoomFrameImage_Res> _res = null, Action<DefaultPacketRes> _error = null)
    {
        var tempPacket = new[]
        {
                new
                {
                myRoomFrameImage.itemId,
                myRoomFrameImage.num
                }
            };

        string json = JsonConvert.SerializeObject(tempPacket);

        Single.Web.SendRequest(new SendRequestData(eHttpVerb.DELETE, ContentServerUrl, WebPacket.MyRoomFrameImageDelete(json), dim: false), _res, _error);
    }
}
