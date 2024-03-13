using FrameWork.UI;
using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_OfficeExit : Popup_Basic
{
    /// <summary>
    /// 오피스 나가는 타이머
    /// </summary>
    /// <returns></returns>
    /// 
    public void ExitOffice() => Timing.RunCoroutine(Co_OfficeExitTimer());

    public IEnumerator<float> Co_OfficeExitTimer()
    {
        go_Desc.SetActive(true);

        int timer = 10;
        while (timer > 0)
        {
            Util.SetMasterLocalizing(txtmp_Desc, new MasterLocalData("1162", timer.ToString()));
            timer--;
            yield return Timing.WaitForSeconds(1f);
        }

        Back();        
    }
}
