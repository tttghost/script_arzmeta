using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using MEC;

public class Panel_Phone : PanelBase
{
	#region Members

	Button btn_Back;

	Button btn_Background;

	[HideInInspector] public bool initialized = false;
	[HideInInspector] public bool isBack = false;

	#endregion


	#region Initialize

	private void OnDestroy()
	{
		btn_Back.onClick.RemoveAllListeners();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		BackAction_Custom += OnClick_Back;

		Single.Web.webPostbox.PostboxReq(null);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		isBack = true;

		BackAction_Custom -= OnClick_Back;
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		this.GetComponent<ArzMetaPhone>().Init();

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		Debug.Log(this.transform.Search(nameof(btn_Background)));

		btn_Background = GetUI_Button(nameof(btn_Background), OnClick_Back);
		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
	}

	protected override IEnumerator<float> Co_OpenEndAct()
	{
		yield return Timing.WaitUntilTrue(() => this.canvasGroup.alpha >= 1f);

		yield return Timing.WaitForSeconds(.5f);

		GetComponent<ArzMetaPhone>().SmartPhone.applicationList[Define.SmartPhoneApp.Mail].CheckNewMail();

		isBack = false;
	}

	public void OnClick_Back()
	{
		if (isBack) return;

		ArzMetaManager.Instance.PhoneController.ExitPhone();

		SceneLogic.instance.PopPanel();

		float delay = ArzMetaManager.Instance.PhoneController.isSit ? 0f : 1f;

		Util.RunCoroutine(Co_EnableInput().Delay(delay));

		isBack = true;		
	}

	IEnumerator<float> Co_EnableInput()
	{
		MyPlayer.instance.EnableInput(true);

		GetPanel<Panel_HUD>().Show(true);
		GetPanel<Panel_HUD>().Joystick.ShowItem(true);

		GetPanel<Panel_HUD>().Joystick.left.SetActive(true);

		yield return Timing.WaitUntilTrue(() => GetPanel<Panel_HUD>().canvasGroup.alpha >= .99f);

		GetPanel<Panel_HUD>().Joystick.virtualJoystick.GetComponent<UIVirtualJoystick>().enabled = true;
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		MyPlayer.instance.EnableInput(false);

		Show(true);

		GetPanel<Panel_HUD>().Show(false);
		GetPanel<Panel_HUD>().Joystick.ShowItem(false);
		GetPanel<Panel_HUD>().Joystick.virtualJoystick.GetComponent<UIVirtualJoystick>().enabled = false;

		yield return Timing.WaitForOneFrame;
	}

	protected override IEnumerator<float> Co_SetCloseStartAct()
	{
		Show(false);

		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha <= 0);
	}

	public void ShowAlphaOnly(bool _show, float _delay = 0f, bool _instant = false)
	{
		FadeUtils.SetCanvasGroupAlphaOnly(this.GetComponent<CanvasGroup>(), this.GetHashCode(), _show, _delay, _instant);
	}

	#endregion
}
