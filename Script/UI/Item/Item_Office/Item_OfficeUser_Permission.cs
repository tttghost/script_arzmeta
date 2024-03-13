/***************************************************************************************
 * 
 *      오피스룸 안에 참여자 목록 / 관전자 목록 / 대기자 목록을 보여주기 위한 스크롤 아이템
  * 
 *      <Partial 클래스 구조>
 *      Item_OfficeUser.cs
 *          Item_OfficeUser_Permission.cs
 *          Item_OfficeUser_Observer.cs
 *          Item_OfficeUser_WaitPlayer.cs
 * 
 ***************************************************************************************/
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using FrameWork.UI;
using Newtonsoft.Json;

public partial class Item_OfficeUser : UIBase
{
    private GameObject go_group_Permission;
    private TMP_Text txtmp_PlayerName;
    private Button btn_SetAuthority;
    private Image img_Thumbnail;
    private Image img_Authority;
    private TogglePlus togplus_Screen;
    private TogglePlus togplus_Chat;
    private TogglePlus togplus_Voice;
    private TogglePlus togplus_Video;
    private Button btn_KickOut;

    private View_OfficeUserInfo viewOfficeUserInfo;
    private View_OfficeWaitPlayers viewOfficeWaitPlayers;

    #region 오피스룸 참여자 권한정보 관련 함수
    // 오피스룸 관전자 UI Component 설정
    private void SetMemberUI_Permission()
    {
        Popup_OfficeUserInfo popup = SceneLogic.instance.GetPopup<Popup_OfficeUserInfo>();
        if ( popup != null )
        {
            viewOfficeUserInfo = popup.viewOfficeUserInfo;
            viewOfficeWaitPlayers = popup.viewOfficeWaitePlayers;
        }

        go_group_Permission = GetChildGObject( nameof( go_group_Permission ) );

        txtmp_PlayerName = GetUI_TxtmpMasterLocalizing( nameof( txtmp_PlayerName ) );
        img_Authority = GetUI_Img( nameof( img_Authority ) );

        // 권한설정 버튼
        btn_SetAuthority = GetUI_Button( nameof( btn_SetAuthority ), OnClick_SetAuthority );

        togplus_Screen = GetUI<TogglePlus>( nameof( togplus_Screen ) );
        if ( togplus_Screen != null )
        {

            togplus_Screen.SetToggleAction( ( isOn ) =>
            {
                if ( isOn )
                {
                    if ( viewOfficeUserInfo != null )
                        viewOfficeUserInfo.SetPermissionToggleState_All( permissionInfo, false );
                }

                permissionInfo.ScreenPermission = isOn;
            } );
        }

        togplus_Chat = GetUI<TogglePlus>( nameof( togplus_Chat ) );
        if ( togplus_Chat != null )
        {
            togplus_Chat.SetToggleAction( ( isOn ) =>
            {
                permissionInfo.ChatPermission = isOn;

            } );
        }

        togplus_Voice = GetUI<TogglePlus>( nameof( togplus_Voice ) );
        if ( togplus_Voice != null )
        {
            togplus_Voice.SetToggleAction( ( isOn ) =>
            {
                permissionInfo.VoicePermission = isOn;
            } );
        }

        togplus_Video = GetUI<TogglePlus>( nameof( togplus_Video ) );
        if ( togplus_Video != null )
        {
            togplus_Video.SetToggleAction( ( isOn ) =>
            {   
                permissionInfo.VideoPermission = isOn;
            } );
        }

        btn_KickOut = GetUI_Button( nameof( btn_KickOut ), OnClick_KickObserver );
    }

    private void OnClick_SetAuthority()
    {
        // 권한설정 버튼을 눌렀을 때, Popup_OfficeSetAuthority 열어주기
        Popup_OfficeSetAuthority popup = SceneLogic.instance.GetPopup<Popup_OfficeSetAuthority>();
        if ( popup != null )
        {
            popup.SetToggleState(
                (OfficeAuthority)scene_OfficeRoom.myPermission.Authority,
                (OfficeAuthority)permissionInfo.Authority );

            PushPopup<Popup_OfficeSetAuthority>();
            popup.SetCloseEndCallback( () =>
            {
                // 선택한 유저의 권한 프리셋 변경이 있었다면
                if ( (OfficeAuthority)permissionInfo.Authority != popup.userAuthority )
                {
                    // 관리자를 변경한 경우 현재 관리자 처리
                    // 회의실의 경우, 현재 관리자 -> 일반참가자로 전환
                    // 강의실의 경우, 현재 관리자 -> 청중         전환
                    if ( popup.userAuthority == OfficeAuthority.관리자 )
					{
                        if ( Util.UtilOffice.IsLectureRoom() )
                            viewOfficeUserInfo.itemMasterUser.permissionInfo.Authority = (int)OfficeAuthority.청중;
                        else
                            viewOfficeUserInfo.itemMasterUser.permissionInfo.Authority = (int)OfficeAuthority.일반참가자;

                        viewOfficeUserInfo.itemMasterUser.RefreshUI_Permission();
                    }

                    // 권한 프리셋 변경에 따른 Permission  Value 설정
                    db.OfficeDefaultOption defaultOption
                    = Single.MasterData.dataOfficeDefaultOption.GetData( permissionInfo.Authority );

                    permissionInfo.Authority = (int)popup.userAuthority;
                    permissionInfo.ScreenPermission = Convert.ToBoolean( defaultOption.webShare );
                    permissionInfo.ChatPermission = Convert.ToBoolean( defaultOption.chat );
                    permissionInfo.VoicePermission = Convert.ToBoolean( defaultOption.voiceChat );
                    permissionInfo.VideoPermission = Convert.ToBoolean( defaultOption.videoChat );

                    RefreshUI_Permission();
                }
            } );
        }
    }

    private void OnClick_KickObserver()
    {
        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("1180")))
            .ChainPopupAction(new PopupAction(() =>
            {
                C_OFFICE_KICK packet = new C_OFFICE_KICK { ClientId = permissionInfo.ClientId };
                Single.RealTime.Send(packet);
            }, null));
    }

    public void SetData_UserPermission( UserData userdata, OfficeUserInfo userinfo,
                                        OfficeAuthority myAuthority, Dictionary<string, int> avatarDatas )
    {
        permissionInfo = userinfo;
        nickName = userdata.Nickname;

        go_ThumbNail.SetActive( true );
        go_group_Permission.SetActive( true );
        go_group_Observer.SetActive( false );
        go_group_Wait.SetActive( false );

        // 닉네임 설정
        SetUI_NickName( userinfo.Authority, userdata.Nickname );

        // 권한 아이콘 sprite 설정
        img_Authority.sprite = Util.UtilOffice.GetAuthoritySprite( userinfo.Authority );

        // Thumbnail sprite 설정
        LocalPlayerData.Method.GetAvatarSprite( userdata.OwnerId, avatarDatas, OnThumbnailSprite );

        // 서버에서 보내준 권한대로 Check 상태 설정하기
        
        togplus_Screen.SetToggleIsOn(userinfo.ScreenPermission);
        togplus_Chat.SetToggleIsOn(userinfo.ChatPermission);
        togplus_Voice.SetToggleIsOn(userinfo.VoicePermission);
        togplus_Video.SetToggleIsOn(userinfo.VideoPermission);

        // 내가 관리자 or 부관리자일 경우
        switch ( myAuthority )
        {
        case OfficeAuthority.관리자:
			{
                go_AdminBg.SetActive(true);
                go_GuestBg.SetActive(false);

                switch ((OfficeAuthority)userinfo.Authority)
				{
                case OfficeAuthority.관리자:
                    btn_SetAuthority.interactable = false;
                    togplus_Screen.tog.interactable = true;
                    togplus_Chat.tog.interactable = true;
                    togplus_Voice.tog.interactable = true;
                    togplus_Video.tog.interactable = true;
                    btn_KickOut.interactable = false;
                    break;

                default:
                    btn_SetAuthority.interactable = true;
                    togplus_Screen.tog.interactable = true;
                    togplus_Chat.tog.interactable = true;
                    togplus_Voice.tog.interactable = true;
                    togplus_Video.tog.interactable = true;
                    btn_KickOut.interactable = true;
                    break;
				}
			}
            break;

        case OfficeAuthority.부관리자:
            {
                go_AdminBg.SetActive(true);
                go_GuestBg.SetActive(false);

                switch ((OfficeAuthority)userinfo.Authority)
                {
                case OfficeAuthority.관리자:
                    {
                        btn_SetAuthority.interactable = false;
                        togplus_Screen.tog.interactable = true;
                        togplus_Chat.tog.interactable = false;
                        togplus_Voice.tog.interactable = false;
                        togplus_Video.tog.interactable = false;
                        btn_KickOut.interactable = false;
                    }
                    break;

                case OfficeAuthority.부관리자:
                    {
                        btn_SetAuthority.interactable = false;
                        togplus_Screen.tog.interactable = true;
                        togplus_Chat.tog.interactable = true;
                        togplus_Voice.tog.interactable = true;
                        togplus_Video.tog.interactable = true;
                        btn_KickOut.interactable = false;
                    }
                    break;

                default:
                    {
                        btn_SetAuthority.interactable = true;
                        togplus_Screen.tog.interactable = true;
                        togplus_Chat.tog.interactable = true;
                        togplus_Voice.tog.interactable = true;
                        togplus_Video.tog.interactable = true;
                        btn_KickOut.interactable = true;
                    }
                    break;
                }
            }
            break;

        default:
            {
                go_AdminBg.SetActive(false);
                go_GuestBg.SetActive(true);

                btn_SetAuthority.interactable = false;
                togplus_Screen.tog.interactable = false;
                togplus_Chat.tog.interactable = false;
                togplus_Voice.tog.interactable = false;
                togplus_Video.tog.interactable = false;
                btn_KickOut.interactable = false;
            }
            break;
        }
    }

    public void RefreshUI_Permission()
	{
        SetUI_NickName( permissionInfo.Authority, nickName );

        // 권한 아이콘 sprite 설정
        img_Authority.sprite = Util.UtilOffice.GetAuthoritySprite( permissionInfo.Authority );

        // 서버에서 보내준 권한대로 Check 상태 설정하기
        togplus_Screen.SetToggleIsOn(permissionInfo.ScreenPermission);
        togplus_Chat.SetToggleIsOn(permissionInfo.ChatPermission);
        togplus_Voice.SetToggleIsOn(permissionInfo.VoicePermission);
        togplus_Video.SetToggleIsOn(permissionInfo.VideoPermission);
    }

	private void OnThumbnailSprite( Sprite sprite )
	{
        img_Thumbnail = GetUI_Img( nameof( img_Thumbnail ) );
        if ( img_Thumbnail != null )
            img_Thumbnail.sprite = sprite;
    }

	public void SetPermissionToggleState( eOfficePermissionMaster permissionMaster, bool isOn )
    {
        switch ( permissionMaster )
        {
        case eOfficePermissionMaster.CHAT:
            {
                if ( togplus_Chat.tog.IsInteractable() )
                    {
                        togplus_Chat.SetToggleIsOn(isOn);
                        permissionInfo.ChatPermission = isOn;
                }
            }
            break;

        case eOfficePermissionMaster.VOICE:
            {
                if ( togplus_Voice.tog.IsInteractable() )
                {
                        togplus_Voice.SetToggleIsOn(isOn);
                        permissionInfo.VoicePermission = isOn;
                }
            }
            break;

        case eOfficePermissionMaster.VIDEO:
            {
                if ( togplus_Video.tog.IsInteractable() )
                {
                        togplus_Video.SetToggleIsOn(isOn);
                        permissionInfo.VideoPermission = isOn;
                }
            }
            break;

        case eOfficePermissionMaster.SCREEN:
        default:
            {
                //if ( tog_Screen.IsInteractable() )
                {
                    togplus_Screen.SetToggleIsOn(isOn);
                    permissionInfo.ScreenPermission = isOn;
                }
            }
            break;
        }
    }
    #endregion
}
