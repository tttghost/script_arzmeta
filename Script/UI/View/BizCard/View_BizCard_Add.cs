using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FrameWork.UI;
using UnityEngine.UI;
using UnityEngine.Events;

public class View_BizCard_Add : UIBase
{
    #region 변수
    private Button btn_Add;
    #endregion

    protected override void SetMemberUI()
    {
        #region Button
        btn_Add = GetUI_Button(nameof(btn_Add));
        #endregion
    }

    #region 초기화
    public void AddListener(UnityAction unityAction = null)
    {
        if (btn_Add != null)
        {
            btn_Add.onClick.RemoveAllListeners();
            btn_Add.onClick.AddListener(unityAction);
        }
    }
    #endregion
}
