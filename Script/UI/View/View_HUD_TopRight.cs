/************************************************************************************************************
 * 
 *          View_HUD_TopRight class
 *              - 오피스카메라 / 참여자 정보 / 룸정보 / 나가기
 * 
 ************************************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using FrameWork.UI;
using FrameWork.Cache;

public class View_HUD_TopRight : UIBase
{
    [SerializeField] private List<Sprite> imgOfficeViewList;

    public delegate void Handler_OfficeCamera(eOfficeCameraType eOfficeCameraType);
    public event Handler_OfficeCamera handler_OfficeCamera;

    private GameObject go_TopRight_btn;
    private eOfficeCameraType officeCameraType = eOfficeCameraType.NONE;

    private Button btn_UserInfo = null;
    private Button btn_RoomInfo = null;
    private Button btn_OfficeCamera = null;
    private Button btn_KtmfVote = null;
    private Image imgOfficeCamera = null;

    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        // 오피스 UI Root GameObject
        go_TopRight_btn = GetChildGObject(nameof(go_TopRight_btn));

        btn_OfficeCamera = GetUI_Button(nameof(btn_OfficeCamera), OnClick_ChangeOfficeCamera);

        btn_KtmfVote = GetUI_Button(nameof(btn_KtmfVote), () => LocalPlayerData.Method.KTMFVote());

        imgOfficeCamera = GetUI<Image>("img_OfficeCamera_icon");

        // 참여자 정보 버튼 OnClick Callback 등록
        btn_UserInfo = GetUI<Button>(nameof(btn_UserInfo));
        if (btn_UserInfo != null)
        {
            btn_UserInfo.onClick.AddListener(() =>
           SceneLogic.instance.PushPopup<Popup_OfficeUserInfo>()
            );
        }

        // 룸정보 버튼 OnClick Callback 등록
        btn_RoomInfo = GetUI<Button>(nameof(btn_RoomInfo));
        if (btn_RoomInfo != null)
        {
            btn_RoomInfo.onClick.AddListener(() => OnClick_RoomInfo());
        }

        Button btnTemp = null;
        // 나가기 버튼 OnClick Callback 등록
        btnTemp = GetUI<Button>("btn_ExitRoom");
        if (btnTemp != null)
        {
            btnTemp.onClick.AddListener(() =>
           SceneLogic.instance.Back()
            );
        }
    }

    protected override void Start()
    {
        base.Start();

        if (Util.UtilOffice.IsOffice())
        {
            go_TopRight_btn.SetActive(true);

            // 강의실에서만 오피스 카메라 변경 버튼 활성화
            if (btn_OfficeCamera != null)
                btn_OfficeCamera.gameObject.SetActive(Util.UtilOffice.IsLectureRoom());

        }
        else if (Util.UtilOffice.IsExposition())
        {
            // 유학박람회 부스에서만 입장 인원 정보와 방 설정 버튼 비활성화
            if (btn_OfficeCamera != null)
                btn_OfficeCamera.gameObject.SetActive(false);
            if (btn_UserInfo != null)
                btn_UserInfo.gameObject.SetActive(false);
        }
        else
        {
            go_TopRight_btn.SetActive(false);
        }

        // 진행중 투표 없으면 버튼 끄기
        switch (SceneLogic.instance.GetSceneType())
        {
            case SceneName.Scene_Room_JumpingMatching:
            case SceneName.Scene_Room_OXQuiz:
                break;
            default:
                { 
                    Single.Web.selectVote.GetKTMFVoteInfo((res) =>
                    {
                        switch (SceneLogic.instance.GetSceneType())
                        {
                            case SceneName.Scene_Room_JumpingMatching:
                            case SceneName.Scene_Room_OXQuiz:
                            case SceneName.Scene_Room_Lecture:
                            case SceneName.Scene_Room_Lecture_22Christmas:
                            case SceneName.Scene_Room_Meeting:
                            case SceneName.Scene_Room_Meeting_22Christmas:
                            case SceneName.Scene_Room_Meeting_Office:
                            case SceneName.Scene_Room_Consulting:
                                SetActiveKTMFVote(false); break;
                            default: SetActiveKTMFVote(true); break;
                        }
                    },
                    (error) =>
                    {
                        SetActiveKTMFVote(false);
                    },
                    false);
                }
                break;
        }

    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    #region TOP_RIGHT: 오피스 카메라시점 변경 버튼 콜백함수
    private void OnClick_ChangeOfficeCamera()
    {
        officeCameraType += 1;
        if (officeCameraType > eOfficeCameraType.SCREEN)
            officeCameraType = eOfficeCameraType.NONE;

        handler_OfficeCamera?.Invoke(officeCameraType);

        // 오피스 카메라 상태에 따른 Image Sprite 변경
        if (imgOfficeViewList.Count > 0 &&
             imgOfficeViewList.Count >= (int)officeCameraType)
        {
            if (imgOfficeCamera != null)
                imgOfficeCamera.sprite = imgOfficeViewList[(int)officeCameraType];
        }
    }

    #endregion

    public void SetActiveKTMFVote(bool active)
    {
        if (btn_KtmfVote != null)
        {
            btn_KtmfVote.gameObject.SetActive(active);
        }
    }

    private void OnClick_RoomInfo()
    {
        if (Util.UtilOffice.IsExposition())
        {
            var scene_Booth = SceneLogic.instance as Scene_Room_Exposition_Booth;
            if (scene_Booth.IsMyBooth())
            {
                //scene_Booth.GetBoothDetail(()=> GetPopup<Popup_ExpositionRoomCreate>().Modify_ExpositionRoom(scene_Booth.GetBooth()));

                //PushPopup<Popup_ExpositionRoomCreate>();
                PushPopup<Popup_ExpositionRoomCreate>().Modify_ExpositionRoom(scene_Booth.GetBooth());
                return;
            }
        }

        SceneLogic.instance.PushPopup<Popup_OfficeRoomSave>();
    }
}
