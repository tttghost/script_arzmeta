using System.Text;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Google.Protobuf;
using FrameWork.Network;
using FrameWork.UI;
using Newtonsoft.Json;

public partial class Item_OfficeUser : UIBase
{
    #region 관전자 UI
    private GameObject go_group_Observer;

    private Toggle tog_SetAudience;
    private TMP_Text txtmp_Observer;
    private TMP_Text txtmp_Audience;
    private TogglePlus togplus_ChatPermission;
    private Button btn_KickObserver;
    #endregion

    private void SetMemberUI_Observer()
    {
        go_group_Observer = GetChildGObject( nameof( go_group_Observer ) );

        txtmp_Observer = GetUI_TxtmpMasterLocalizing( nameof( txtmp_Observer ), new MasterLocalData( "office_participant_type_observer" ) );
        txtmp_Audience = GetUI_TxtmpMasterLocalizing( nameof( txtmp_Audience ), new MasterLocalData( "office_participant_type_audience" ) );

        btn_KickObserver = GetUI_Button( nameof( btn_KickObserver ), OnClick_KickObserver );
        tog_SetAudience = GetUI_Toggle( nameof( tog_SetAudience ), OnValueChanged_SetAudience );

        togplus_ChatPermission = GetUI<TogglePlus>(nameof(togplus_ChatPermission));
        togplus_ChatPermission.SetToggleAction(OnValueChanged_SetChatAuthority);
    }

    public void SetData_Observer( OfficeUserInfo permissioninfo, string nickname, OfficeAuthority myAuthority )
    {
        permissionInfo = permissioninfo;
        nickName = nickname;

        go_ThumbNail.SetActive( false );
        go_group_Permission.SetActive( false );
        go_group_Observer.SetActive( true );
        go_group_Wait.SetActive( false );

        txtmp_Observer.gameObject.SetActive( false );
        txtmp_Audience.gameObject.SetActive( true );

        SetUI_NickName( permissioninfo.Authority, nickname );

        // 내가 관리자 or 부관리자일 경우
        switch ( myAuthority )
        {
        case OfficeAuthority.관리자:
        case OfficeAuthority.부관리자:
            {
                if (tog_SetAudience != null)
                    tog_SetAudience.interactable = true;

                if (togplus_ChatPermission != null)
                {
                    togplus_ChatPermission.tog.interactable = true;
                    togplus_ChatPermission.SetToggleIsOn(permissioninfo.ChatPermission);
                }

                if (btn_KickObserver != null)
                    btn_KickObserver.interactable = true;

                if ( go_AdminBg != null )
                    go_AdminBg.SetActive( true );

                if ( go_GuestBg != null )
                    go_GuestBg.SetActive( false );

                if (txtmp_Observer != null)
                    txtmp_Observer.gameObject.SetActive(true);
                if (txtmp_Audience != null)
                    txtmp_Audience.gameObject.SetActive(false);
            }
            break;

        default:
            {
                if (tog_SetAudience != null)
                    tog_SetAudience.interactable = false;

                if (togplus_ChatPermission != null)
                {
                    togplus_ChatPermission.tog.interactable = false;
                    togplus_ChatPermission.SetToggleIsOn(permissioninfo.ChatPermission);
                }

                if (btn_KickObserver != null)
                    btn_KickObserver.interactable = false;

                if ( go_AdminBg != null )
                    go_AdminBg.SetActive( false );
                if ( go_GuestBg != null )
                    go_GuestBg.SetActive( true );
            }
            break;
        }
    }

    // 관전자를 청중으로 변경을 서버에 요청하는 토글 콜백
    private void OnValueChanged_SetAudience( bool isOn )
    {
        if ( isOn )
        {
            permissionInfo.Authority = (int)OfficeAuthority.청중;
            txtmp_Observer.gameObject.SetActive( false );
            txtmp_Audience.gameObject.SetActive( true );
        }
        else
        {
            permissionInfo.Authority = (int)OfficeAuthority.관전자;
            txtmp_Observer.gameObject.SetActive( true );
            txtmp_Audience.gameObject.SetActive( false );
        }

        SetUI_NickName( permissionInfo.Authority, nickName );
    }

    // 채팅 권한 변경 후 서버에 요청
    private void OnValueChanged_SetChatAuthority( bool isOn )
    {
        permissionInfo.ChatPermission = isOn;
    }

    public void SetChatToggleState( bool isOn )
    {
        togplus_ChatPermission.SetToggleIsOn(isOn);
    }
}
