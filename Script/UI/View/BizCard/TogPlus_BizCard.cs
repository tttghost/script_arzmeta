using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class TogPlus_BizCard : UIBase, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    #region 변수
    public TogglePlus TogPlus { get; private set; }
    public Toggle Tog => TogPlus.tog;

    private TMP_Text txtmp_Index;
    private GameObject go_Checkmark;
    #endregion

    protected override void SetMemberUI()
    {
        #region TMP_Text
        txtmp_Index = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Index));
        #endregion

        #region etc
        go_Checkmark = GetChildGObject(nameof(go_Checkmark));
        TogPlus = GetComponent<TogglePlus>();
        #endregion
    }

    #region 초기화 및 데이터 갱신
    public void SetData(int index, bool isDefault, Action<bool> action)
    {
        SetIndex(index);
        SetActiveDefault(isDefault);
        AddListner(action);
    }

    /// <summary>
    /// 인덱스 세팅
    /// </summary>
    /// <param name="index"></param>
    public void SetIndex(int index)
    {
        if (txtmp_Index != null)
        {
            string str = index >= 0 ? index.ToString() : "+";
            Util.SetMasterLocalizing(txtmp_Index, str);
        }
    }

    /// <summary>
    /// 대표 아이콘 비/활성화
    /// </summary>
    /// <param name="b"></param>
    public void SetActiveDefault(bool b)
    {
        if (go_Checkmark != null)
        {
            go_Checkmark.SetActive(b);
        }
    }

    public void AddListner(Action<bool> action)
    {
        if (TogPlus != null)
        {
            TogPlus.SetToggleAction(action);
        }
    }
    #endregion

    #region OnDrag 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        GetComponentInParent<ScrollRect>().OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
        canvasGroup.blocksRaycasts = true;
    }
    #endregion
}
