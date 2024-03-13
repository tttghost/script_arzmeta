using FrameWork.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel_MedicineRoomSelect : PanelBase
{
    TMP_Text txtmp_MedicineRoomTitle;

    GameObject go_Content;
    GameObject go_MedicineRoom;

    int imgCount = 0;
    int minCount = 1;
    int maxCount = 3;
    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        Button btn_Close = GetUI_Button("btn_Close", BtnClose);
        Button btn_LeftSelect = GetUI_Button("btn_LeftSelect", BtnLeftSelect);
        Button btn_RightSelect = GetUI_Button("btn_RightSelect", BtnRightSelect);

        go_Content = GetChildGObject("go_Content");
        go_MedicineRoom = GetChildGObject("go_MedicineRoom");

    }
    protected override void Start()
    {
        base.Start();
    }
    /// <summary>
    /// 패널 종료
    /// </summary>
    private void BtnClose()
    {
        SceneLogic.instance.PopPanel();
    }
    /// <summary>
    /// 왼쪽 이동
    /// </summary>
    private void BtnLeftSelect()
    {

    }
    /// <summary>
    /// 오른쪽 이동
    /// </summary>
    private void BtnRightSelect()
    {

    }

}
