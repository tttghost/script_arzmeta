using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;
using System;

public class Item_FAQ : DynamicItemBase
{
    #region 변수
    private TMP_Text txtmp_Title;
    private TMP_Text txtmp_Contents;

    private Item_FAQData data;
    #endregion

    #region 초기화
    protected override void SetMemberUI()
    {
        base.SetMemberUI();

        currentSize = GetComponent<RectTransform>().sizeDelta;

        #region Button
        GetUI_Button("btn_Item", OnClick_Select);
        #endregion

        #region TMP_Text
        txtmp_Title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Title));
        txtmp_Contents = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Contents));
        #endregion
    }
    #endregion

    public override void UpdateData(DynamicScrollData scrollData)
    {
        data = (Item_FAQData)scrollData;
        ChangeContentSize(data.Size);

        base.UpdateData(scrollData);
    }

    /// <summary>
    /// 프리팹에 데이터 세팅
    /// </summary>
    protected override void SetContent()
    {
        // 제목
        if (txtmp_Title != null)
        {
            Util.SetMasterLocalizing(txtmp_Title, data.title);
        }
        // 내용
        if (txtmp_Contents != null)
        {
            Util.SetMasterLocalizing(txtmp_Contents, data.contents);
            txtmp_Contents.enabled = true;
        }
    }
}
