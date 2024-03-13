using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Popup_BrandList : Popup_Basic
{
    Button btn_Exit;
	TMP_Text txtmp_title;

	private void OnDestroy()
	{
		btn_Exit.onClick.RemoveListener(BackEvent);
	}

	protected override void Awake()
	{
		btn_Exit = this.transform.Search("btn_Exit").GetComponent<Button>();
		btn_Exit.onClick.AddListener(BackEvent);
	}

    protected override void SetMemberUI()
    {
        popupAnimator = GetComponent<Animator>();

		txtmp_title = GetUI_TxtmpMasterLocalizing(nameof(txtmp_title), new MasterLocalData(""));
	}

    private void BackEvent()
	{
		SceneLogic.instance.PopPopup();
	}
}
