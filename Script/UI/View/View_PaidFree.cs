using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class View_PaidFree : UIBase
{
    private GameObject  go_Paid;
    private GameObject  go_Free;
    //private TMP_Text    txtmp_Paid;
    //private TMP_Text    txtmp_Free;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        dontSetActiveFalse = true;

        go_Paid = GetChildGObject(nameof(go_Paid));
        go_Paid.SetActive(false);
        go_Free = GetChildGObject(nameof(go_Free));
        go_Free.SetActive(false);

        //txtmp_Paid = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Paid));
        //txtmp_Free = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Free));

        //AddHandler();

        gameObject.SetActive(false); //예외처리 : 상점들어가기전 비활성화
    }
    //private void AddHandler()
    //{
    //    if (!MyRoomManager.Instance) return;
    //    //MyRoomManager.Instance.handlerPaidMoney += OnPaidMoney;
    //    //MyRoomManager.Instance.handlerFreeMoney += OnFreeMoney;
    //}
    //private void RemoveHandler()
    //{
    //    if (!MyRoomManager.Instance) return;
    //    //MyRoomManager.Instance.handlerPaidMoney -= OnPaidMoney;
    //    //MyRoomManager.Instance.handlerFreeMoney -= OnFreeMoney;
    //}
    //private void OnDestroy()
    //{
    //    RemoveHandler();
    //}
    //private void OnPaidMoney(int paidMoney)
    //{
    //    go_Paid.SetActive(paidMoney != 0);
    //    txtmp_Paid.text = paidMoney.ToString();
    //}
    //private void OnFreeMoney(int freeMoney)
    //{
    //    go_Free.SetActive(freeMoney != 0);
    //    txtmp_Free.text = freeMoney.ToString();
    //}

}
