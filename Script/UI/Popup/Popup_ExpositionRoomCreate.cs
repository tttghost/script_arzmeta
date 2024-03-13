using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using db;
using FrameWork.UI;
using MEC;
using Office;
using TMPro;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Popup_ExpositionRoomCreate : Popup_BaseRoomCreate
{
    private Booth boothData;

    private GameObject go_RoomCode;

    private Button btn_RoomCode;

    private TMP_Text txtmp_RoomCode;
    private TMP_Text txtmp_Target;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        go_RoomCode = GetChildGObject(nameof(go_RoomCode));

        btn_RoomCode = GetUI_Button(nameof(btn_RoomCode), OnClick_RoomCode);

        txtmp_RoomCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_RoomCode));
        txtmp_Target = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Target));

        Util.SetMasterLocalizing(txtmp_RoomTitle, new MasterLocalData("office_booth_set"));

        Util.SetMasterLocalizing(txtmp_RoomName, new MasterLocalData("office_booth_name"));
        Util.SetMasterLocalizing(input_RoomName.placeholder, new MasterLocalData("office_room_name_for_exhibition", LocalPlayerData.NickName));

        Util.SetMasterLocalizing(txtmp_RoomDesc, new MasterLocalData("office_booth_desc"));
        Util.SetMasterLocalizing(input_RoomDesc.placeholder, new MasterLocalData("office_room_desc_for_exhibition", LocalPlayerData.NickName));

        Util.SetMasterLocalizing(txtmp_CreateRoom, new MasterLocalData("common_save"));

        btn_RemoveRoom.gameObject.SetActive(false);

    }

    private void OnClick_RoomCode()
    {
        string shareMessage = $"{boothData.name} {Util.GetMasterLocalizing("office_booth_code")} -> {boothData.roomCode}";

        Util.CopyToClipboard(shareMessage);
        OpenToast<Toast_Basic>()
            .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("10301")));
    }

    public override void Back(int cnt = 1)
    {
        if (btn_CreateRoom.GetComponent<CanvasGroup>().interactable)
        {
            PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.WARNING, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("businesscard_confirm_exit_nosave")))
                .ChainPopupAction(new PopupAction(() =>
                {
                    SceneLogic.instance.isUILock = false;
                    PopPopup();
                }));
        }
        else
        {
            base.Back(cnt);
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Util.RefreshScrollView(gameObject, "sview_ExpositionRoomCreate");
        Util.RefreshLayout(gameObject, "go_Content");
    }




    /// <summary>
    /// 박람회부스 생성
    /// </summary>
    public void Create_ExpositionRoom()
    {
        Booth _booth = new Booth();
        SetData_ExpositionRoom(_booth);
    }

    /// <summary>
    /// 박람회부스 수정
    /// </summary>
    /// <param name="_booth"></param>
    public void Modify_ExpositionRoom(Booth _booth)
    {
        SetData_ExpositionRoom(_booth);
    }

    /// <summary>
    /// 박람회부스 데이터 셋업
    /// </summary>
    /// <param name="_booth"></param>
    private void SetData_ExpositionRoom(Booth _booth)
    {
        boothData = _booth;

        input_RoomName.text = _booth.name;

        input_RoomDesc.text = _booth.description;

        thumbnailPath = Cons.EMPTY;

        go_RoomCode.SetActive(_booth.roomCode != string.Empty);

        txtmp_RoomCode.text = _booth.roomCode;

        if (!string.IsNullOrEmpty(_booth.thumbnail))
        {
            txtmp_Target.text = _booth.thumbnail;
            LocalPlayerData.Method.Load_ExpositionThumbnail(_booth.id, _booth.thumbnail, (sprite) => spriteThumbnail = sprite);
        }
        else
        {
            txtmp_Target.text = string.Empty;
            spriteThumbnail = null;
        }
    }





    /// <summary>
    /// 전시회부스 생성/수정
    /// </summary>
    protected override void OnClick_CreateRoom()
    {
        base.OnClick_CreateRoom();

        if (boothData.roomCode == string.Empty)
        {
            CreateRoom();
        }
        else
        {
            ReservOrModifyRoom();
        }
    }

    /// <summary>
    /// 전시회부스 생성
    /// </summary>
    protected override void CreateRoom()
    {
        base.CreateRoom();
        CreateCSAFBoothReq req = new CreateCSAFBoothReq
        {
            name = string.IsNullOrEmpty(input_RoomName.text) ? Util.GetMasterLocalizing(new MasterLocalData("office_room_name_for_exhibition", LocalPlayerData.NickName)) : input_RoomName.text,
            topicType = boothData.topicType,
            description = string.IsNullOrEmpty(input_RoomDesc.text) ? Util.GetMasterLocalizing(new MasterLocalData("office_room_desc_for_exhibition", LocalPlayerData.NickName)) : input_RoomDesc.text,
            spaceInfoId = boothData.spaceInfoId,
        };

        Single.Web.CSAF.CreateCSAFBooth(tempTex, req, (res) =>
        {
            PopPopup();
            //GetPanel<Panel_Exposition>().ChangeView<View_Exposition_EnterRoom>();
            GetPanel<Panel_Exposition>().SelectView(nameof(View_Exposition_EnterRoom));
            GetPanel<Panel_Exposition>().RefreshList();

        }, IsSameThumbnailPath() ? string.Empty : thumbnailPath);
    }

    /// <summary>
    /// 전시회부스 수정
    /// </summary>
    protected override void ReservOrModifyRoom()
    {
        base.ReservOrModifyRoom();

        var boothId = !Util.UtilOffice.IsExposition()
                  ? GetPopup<Popup_ExpositionRoomInfo>().booth.id
                  : (SceneLogic.instance as Scene_Room_Exposition_Booth).GetBooth().id;

        int isDelete = thumbnailPath == string.Empty ? 1 : 0;

        EditCSAFBoothsReq req = new EditCSAFBoothsReq
        {
            name = string.IsNullOrEmpty(input_RoomName.text) ? Util.GetMasterLocalizing(new MasterLocalData("office_room_name_for_exhibition", LocalPlayerData.NickName)) : input_RoomName.text,
            topicType = boothData.topicType,
            description = string.IsNullOrEmpty(input_RoomDesc.text) ? Util.GetMasterLocalizing(new MasterLocalData("office_room_desc_for_exhibition", LocalPlayerData.NickName)) : input_RoomDesc.text,
            spaceInfoId = boothData.spaceInfoId,
            isDelete = isDelete,
        };

        Single.Web.CSAF.EditCSAFBooth(boothId, tempTex, req, (res) =>
        {
            PopPopup();

            GetPanel<Panel_Exposition>()?.RefreshList();
            GetPopup<Popup_ExpositionRoomInfo>()?.SetExpositionCardInfo(res.booth);

            OpenToast<Toast_Basic>()
                    .ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("10301")));

        }, IsSameThumbnailPath() ? string.Empty : thumbnailPath);
    }

    /// <summary>
    /// 전시회부스 삭제 (사용하진 않음)
    /// </summary>
    protected override void OnClick_RemoveRoom()
    {
        base.OnClick_RemoveRoom();
        var boothId = GetPopup<Popup_ExpositionRoomInfo>().booth.id;

        Single.Web.CSAF.DeleteCSAFBooth(boothId, (res) =>
        {
            DEBUG.LOG("부스를 삭제하였습니다 : " + boothId);

            SceneLogic.instance.PopPopup();

            if (SceneLogic.instance._stackPopups.Count > 0)
            {
                SceneLogic.instance.isUILock = false;
                SceneLogic.instance.PopPopup();
            }

            GetPanel<Panel_Exposition>().RefreshList();
        });
    }
    protected override void OnThumbnailPath(string value)
    {
        base.OnThumbnailPath(value);
        txtmp_Target.text = value;
    }
    protected override void IsSameRoom()
    {
        bool isSameRoom = IsSameThumbnailPath() && IsSameInputField();
        btn_CreateRoom.GetComponent<CanvasGroup>().interactable = !isSameRoom;
    }
    private bool IsSameThumbnailPath()
    {
        return thumbnailPath == Cons.EMPTY;
    }
    private bool IsSameInputField()
    {
        return boothData.name == input_RoomName.text
            && boothData.description == input_RoomDesc.text;
    }
}
