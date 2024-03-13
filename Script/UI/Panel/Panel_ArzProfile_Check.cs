using UnityEngine;
using FrameWork.UI;
using TMPro;

/// <summary>
/// 회원가입 시 최초 생성하는 아즈메타 명함 확인 패널
/// </summary>
public class Panel_ArzProfile_Check : PanelBase
{
    #region 변수
    private TMP_Text txtmp_Nickname;
    private TMP_Text txtmp_MemberCode;
    private TMP_Text txtmp_StateMessage;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Nickname = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Nickname));
        txtmp_MemberCode = GetUI_TxtmpMasterLocalizing(nameof(txtmp_MemberCode));
        txtmp_StateMessage = GetUI_TxtmpMasterLocalizing(nameof(txtmp_StateMessage));

        GetUI_TxtmpMasterLocalizing("txtmp_Main", new MasterLocalData("3069"));
        GetUI_TxtmpMasterLocalizing("txtmp_Sub", new MasterLocalData("3062"));
        GetUI_TxtmpMasterLocalizing("txtmp_MemberCodeTitle", new MasterLocalData("014"));
        GetUI_TxtmpMasterLocalizing("txtmp_Residence", new MasterLocalData("3013"));
        #endregion

        #region Button
        GetUI_Button("btn_Next", () => Single.Web.account.SetEnter());
        #endregion

        BackAction_Custom = () => { return; }; // 물리 백버튼 눌렀을 시 막는 버튼
    }

    protected override void OnEnable()
    {
        if (txtmp_Nickname != null)
        {
            Util.SetMasterLocalizing(txtmp_Nickname, LocalPlayerData.NickName);
        }

        if (txtmp_MemberCode != null)
        {
            Util.SetMasterLocalizing(txtmp_MemberCode, LocalPlayerData.MemberCode);
        }

        if (txtmp_StateMessage != null)
        {
            if (!string.IsNullOrEmpty(LocalPlayerData.StateMessage))
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, LocalPlayerData.StateMessage);
            }
            else
            {
                Util.SetMasterLocalizing(txtmp_StateMessage, new MasterLocalData("3066", LocalPlayerData.NickName));
            }
        }
    }
}
