using Assets._Launching.DEV.Script.Framework.Network.WebPacket;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GamePotUnity;

public class Panel_LeaveAccount : PanelBase
{
    private ToggleGroup togg_LeaveGroup; //토글 그룹 
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        GetUI_TxtmpMasterLocalizing("txtmp_Title", new MasterLocalData("9104"));  // 탈퇴사유 타이틀 
        GetUI_TxtmpMasterLocalizing("txtmp_Leave1", new MasterLocalData("9105")); //콘텐츠 등 이용 서비스 부족
        GetUI_TxtmpMasterLocalizing("txtmp_Leave2", new MasterLocalData("9106")); //사용빈도 낮음
        GetUI_TxtmpMasterLocalizing("txtmp_Leave3", new MasterLocalData("9107")); //오류발생 등 이용 불편
        GetUI_TxtmpMasterLocalizing("txtmp_Leave4", new MasterLocalData("9108")); //재가입 목적
        GetUI_TxtmpMasterLocalizing("txtmp_Leave5", new MasterLocalData("9109")); //기타 사유 
        togg_LeaveGroup = GetUI<ToggleGroup>(nameof(togg_LeaveGroup)); //탈퇴사유 토글 그룹(5가지)       
        GetUI_Button("btn_Back", Back); //뒤로가기 버튼
        GetUI_Button("btn_Next", Next); //다음 버튼( 팝업 ) 
    }
    private void Start()
    {
        int count = togg_LeaveGroup.transform.childCount; //토글 그룹 하위 자식들 찾아서
        for (int i = 0; i < count; ++i)
        {
            Toggle tog = togg_LeaveGroup.transform.GetChild(i).GetComponent<Toggle>(); //찾아진 토글
            tog.onValueChanged.AddListener((bisCheckLeaveAccount) =>
            {
                if (bisCheckLeaveAccount == true)
                {
                    Single.Sound.PlayEffect(Cons.click);
                }
            });
        }
    }
    /// <summary>
    /// 뒤로 가기 버튼
    /// </summary>
    private void Back()
    {
        SceneLogic.instance.Back(); //팝업있으면 팝업부터 닫고 popPanel
    }
    /// <summary>
    /// 탈퇴 진행 버튼
    /// </summary>
    private void Next()
    {
        PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, null, new MasterLocalData("9110")))
            .ChainPopupAction(new PopupAction(() =>
            {
                //Single.Web.Resign("1234556", (res) =>
                // {
                //     FrontisNetwork.DisconnectAllServer();
                //     Single.Scene.LoadScene(Cons.Scene_Title); //계정 탈퇴 성공하면 타이틀 페이지로 
                //     LocalPlayerData.ResetData();
                // });

                Single.Web.member.Withdrawal((res) =>
                {
                    Util.RunCoroutine(Co_PopPopup());
                });
            }));
    }

    private IEnumerator<float> Co_PopPopup()
    {
        yield return GetPopup<Popup_Basic>().Co_ClosePopup().WaitUntilDone(); //클로즈

        Single.Scene.FadeOut(1f, () =>
        {
            RealtimeUtils.Disconnect();

            LocalPlayerData.ResetData();
            GamePot.deleteMember();
            Single.Scene.LoadScene(SceneName.Scene_Base_Title);
        });
    }
}