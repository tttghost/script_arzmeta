using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 보류됨
/// </summary>
public class View_ArzTalk : UIBase
{
    #region 변수
    private GameObject go_TextRig;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        GetUI_TxtmpMasterLocalizing("txtmp_NoUser", new MasterLocalData("40000"));
        #endregion

        #region etc
        go_TextRig = GetChildGObject(nameof(go_TextRig));
        #endregion
    }

    #region 초기화
    protected override void OnEnable()
    {
        if (go_TextRig != null)
        {
            go_TextRig.SetActive(true); // 임시로 계속 켜놓음
        }
    }
    #endregion
}
