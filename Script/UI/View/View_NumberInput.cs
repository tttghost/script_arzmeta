using FrameWork.UI;
using Office;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_NumberInput : UIBase
{
    private GameObject go_NumberInput;

    private TMP_Text txtmp_Confirm;

    private Button btn_Exit;
    private Button btn_Exit2;
    
    private Action callbackAct;

    [HideInInspector]
    public List<Item_NumberInput> item_NumberInputList = new List<Item_NumberInput>();

    protected override void SetMemberUI()
    {
        base.SetMemberUI();
        go_NumberInput = GetChildGObject(nameof(go_NumberInput));

        txtmp_Confirm = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Confirm), new MasterLocalData("001"));

        btn_Exit = GetUI_Button(nameof(btn_Exit), OnClick_Exit);
        btn_Exit2 = GetUI_Button(nameof(btn_Exit2), ()=> gameObject.SetActive(false));
    }

    private void OnClick_Exit()
    {
        gameObject.SetActive(false);
        //Popup_OfficeRoomCreate popup_OfficeRoomCreate = SceneLogic.instance.GetPopup<Popup_OfficeRoomCreate>(Cons.Popup_OfficeRoomCreate);
        //popup_OfficeRoomCreate.ChangeView("");
        callbackAct?.Invoke();
    }

    /// <summary>
    /// 셋 데이터
    /// </summary>
    /// <param name="items"></param>
    public void SetData(Action callbackAct, params MinNumMax[] minNumMax)
    {
        this.callbackAct = callbackAct;
        for (int i = 0; i < item_NumberInputList.Count; i++) //전체 비활성화
        {
            item_NumberInputList[i].gameObject.SetActive(false);
        }

        for (int i = item_NumberInputList.Count; i < minNumMax.Length; i++) //모자란개수만큼 추가
        {
            Item_NumberInput item_NumberInput = Single.Resources.Instantiate<Item_NumberInput>(Cons.Path_Prefab_Item + nameof(item_NumberInput), go_NumberInput.transform);
            item_NumberInputList.Add(item_NumberInput);
        }

        for (int i = 0; i < minNumMax.Length; i++) //활성화
        {
            Item_NumberInput item_NumberInput = item_NumberInputList[i];
            item_NumberInput.SetData(minNumMax[i]);
            item_NumberInput.gameObject.SetActive(true);
        }
        Util.RefreshLayout(gameObject, "go_Root");
    }
}

public class MinNumMax
{
    public int num;
    public int minNum;
    public int maxNum;
    public string format;
    public Action<int> act;
    public MinNumMax(int num, int minNum, int maxNum, string format, Action<int> act)
    {
        this.num = num;
        this.minNum = minNum;
        this.maxNum = maxNum;
        this.format = format;
        this.act = act;
    }
}