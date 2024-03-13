using FrameWork.UI;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;



public class Item_OfficeUserInfoWait : UIBase
{
    //public OfficeWaitUserData userData = new OfficeWaitUserData();
    private View_OfficeWaitPlayers viewWaitPlayers;

    private TMP_Text txtmp_PlayerName;

    private Button btn_accept;
    private Button btn_reject;


    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        txtmp_PlayerName = GetUI_TxtmpMasterLocalizing(nameof(txtmp_PlayerName));

        View_OfficeUserInfo _viewHostUI = FindObjectOfType<View_OfficeUserInfo>();
        //viewWaitPlayers = _viewHostUI.view_WaitPlayers;

        Util.RunCoroutine(InitUI());

    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> InitUI()
    {
        yield return Timing.WaitUntilTrue(() => viewWaitPlayers != null);
        
        // 강퇴 버튼 
        btn_accept = Util.Search<Button>(gameObject, nameof(btn_accept));
        btn_accept.onClick.AddListener(OnClickAcceptPlayer);

        btn_reject = Util.Search<Button>(gameObject, nameof(btn_reject));
        btn_reject.onClick.AddListener(OnClickRejectPlayer);
    }

    private void OnClickAcceptPlayer()
    {
        //viewWaitPlayers.SendAcceptWaitPacket(userData.clientData.clientId, (int)userData.permissionType, true);
    }

    private void OnClickRejectPlayer()
    {
        //viewWaitPlayers.SendAcceptWaitPacket(userData.clientData.clientId, (int)userData.permissionType, false);
    }


    public void SetItem(/* OfficeWaitUserData data */)
    {
        gameObject.SetActive(true); // 생성하자마자 보이도록

        //userData = data; // 데이터 설정 

        //txtmp_PlayerName.LocalText(userData.clientData.nickname);
    }
}

