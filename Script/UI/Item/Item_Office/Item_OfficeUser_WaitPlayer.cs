using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using Protocol;

public partial class Item_OfficeUser : UIBase
{
    private GameObject go_group_Wait;

    private List<ButtonTweenComponent> btnTweenComponents = new List<ButtonTweenComponent>();

    private void SetMemberUI_Waitting()
    {
        go_group_Wait = GetChildGObject( nameof( go_group_Wait ) );

        Button btn = null;
        ButtonTweenComponent component = null;

        btn = GetUI_Button( "btn_Accept", OnClicdk_Accept );
        if ( btn != null )
        {
            component = btn.GetComponent<ButtonTweenComponent>();
            btnTweenComponents.Add( component );
        }

        btn = GetUI_Button( "btn_Reject", OnClicdk_Reject );
        if ( btn != null )
        {
            component = btn.GetComponent<ButtonTweenComponent>();
            btnTweenComponents.Add( component );
        }

        GetUI_TxtmpMasterLocalizing("txtmp_masterAccept", new MasterLocalData("common_accept"));
        GetUI_TxtmpMasterLocalizing("txtmp_masterReject", new MasterLocalData("common_reject"));
    }

    private void OnClicdk_Accept()
    {
        C_OFFICE_ACCEPT_WAIT message = new C_OFFICE_ACCEPT_WAIT();
        message.IsAccepted = true;
        message.ClientId.Add( waitingUserClientId );
        Single.RealTime.Send( message );

        viewOfficeWaitPlayers.REQUEST_NFO();
    }

    private void OnClicdk_Reject()
    {
        C_OFFICE_ACCEPT_WAIT message = new C_OFFICE_ACCEPT_WAIT();
        message.IsAccepted = false;
        message.ClientId.Add( waitingUserClientId );
        Single.RealTime.Send( message );

        viewOfficeWaitPlayers.REQUEST_NFO();
    }

    // 대기 유저 스크롤아이템 데이터 설정
    public void SetData_OfficeUserWaiting( string clientid, string nickname )
    {
        go_ThumbNail.SetActive( false );
        go_group_Permission.SetActive( false );
        go_group_Observer.SetActive( false );
        go_group_Wait.SetActive( true );

        waitingUserClientId = clientid;
        nickName = nickname;

        // 닉네임 설정
        if ( txtmp_PlayerName != null )
            txtmp_PlayerName.text = nickname;
    }

    // 닉네임 설정
    private void SetUI_NickName( int authority, string nickname )
    {
        if ( txtmp_PlayerName == null )
            return;

        var localAuthority = Util.UtilOffice.GetMasterLocal_OfficeAutority( authority );
        sbTemp.Clear();
        sbTemp.Append( localAuthority );
        sbTemp.Append( " : " );
        sbTemp.Append( nickname );
        txtmp_PlayerName.text = sbTemp.ToString();
    }

    public void StopButtonTweenComponent()
    {
        Util.KillCoroutine("buttontween");
    }
}
