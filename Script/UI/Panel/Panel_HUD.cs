using UnityEngine;
using FrameWork.UI;
using System.Collections.Generic;
using System;
using MEC;
using UnityEngine.Events;

public class Panel_HUD : PanelBase
{
	#region Members

	[SerializeField] private HUDTouch touch;
	[SerializeField] private HUDMenu menu;
	[SerializeField] private HUDJoystick joystick;
	[SerializeField] private View_Chat chat;

	public View_HUD_TopLeft viewHudTopLeft { get; private set; } = null;
	public View_HUD_TopCenter viewHudTopCenter { get; private set; } = null;
	public View_HUD_TopRight viewHudTopRight { get; private set; } = null;

	public View_Chat viewChat { get; private set; } = null;

	[HideInInspector] public Canvas canvas;

	[HideInInspector] public bool cached = false;

	CoroutineHandle handle_initialize;

	#endregion

	#region Properties

	public HUDTouch Touch { get => touch; }
	public HUDMenu Menu { get => menu; }
	public HUDJoystick Joystick { get => joystick; }
	public View_Chat Chat { get => chat; }

	#endregion

	#region Initialize

	private void OnDestroy()
	{
		RemoveListeners();

		Timing.KillCoroutines(nameof(Panel_HUD));
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		touch = new HUDTouch(this);
		menu = new HUDMenu(this);
		joystick = new HUDJoystick(this);
		chat = GetView<View_Chat>();

		viewHudTopLeft = GetView<View_HUD_TopLeft>();
		viewHudTopCenter = GetView<View_HUD_TopCenter>();
		viewHudTopRight = GetView<View_HUD_TopRight>();
		viewChat = GetView<View_Chat>();

		handle_initialize = Util.RunCoroutine(Co_Initialize(), nameof(Panel_HUD));
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		if (cached) return;

		if (viewHudTopLeft != null)
			viewHudTopLeft.gameObject.SetActive(true);

		if (viewHudTopCenter != null)
			viewHudTopCenter.gameObject.SetActive(true);

		if (viewHudTopRight != null)
			viewHudTopRight.gameObject.SetActive(true);
	}

	private IEnumerator<float> Co_Initialize()
	{
		yield return Timing.WaitUntilTrue(() => MyPlayer.instance != null);

		joystick.zone = this.transform.Search(Cons.ViewJoyStickZone).gameObject;
		joystick.virtualJoystick = this.transform.Search(Cons.JoyStickMove).GetComponent<UIVirtualJoystick>();
		joystick.jump = this.transform.Search(Cons.JoyStickJump).GetComponent<UIVirtualButton>();
		joystick.dash = this.transform.Search(Cons.JoyStickDash).GetComponent<UIVirtualButton>();

		canvas = this.gameObject.transform.root.GetComponent<Canvas>();

		CacheListeners();

#if UNITY_STANDALONE || UNITY_EDITOR
		joystick.right.GetComponent<CanvasGroup>().alpha = 0f;
		joystick.right.GetComponent<CanvasGroup>().blocksRaycasts = false;
		joystick.left.GetComponent<CanvasGroup>().alpha = 0f;
		joystick.left.GetComponent<CanvasGroup>().blocksRaycasts = false;
#endif

		cached = true;
	}

	public void CacheListeners()
	{
		if (MyPlayer.instance == null) return;

		touch.virtualTouch.touchZoneOutputEvent.AddListener(MyPlayer.instance.TPSController.StarterInputs.LookInput);
		joystick.virtualJoystick.joystickOutputEvent.AddListener(MyPlayer.instance.TPSController.StarterInputs.MoveInput);
		joystick.jump.buttonStateOutputEvent.AddListener(MyPlayer.instance.TPSController.StarterInputs.JumpInput);
		joystick.dash.buttonClickOutputEvent.AddListener(MyPlayer.instance.TPSController.DashController.DashInput);
	}

	public void RemoveListeners()
	{
		if (MyPlayer.instance == null) return;

		touch.virtualTouch.touchZoneOutputEvent.RemoveListener(MyPlayer.instance.TPSController.StarterInputs.LookInput);
		joystick.virtualJoystick.joystickOutputEvent.RemoveListener(MyPlayer.instance.TPSController.StarterInputs.MoveInput);
		joystick.jump.buttonStateOutputEvent.RemoveListener(MyPlayer.instance.TPSController.StarterInputs.JumpInput);
		joystick.dash.buttonClickOutputEvent.RemoveListener(MyPlayer.instance.TPSController.DashController.DashInput);
	}

	public void Refresh()
	{
		RemoveListeners();

		handle_initialize = Util.RunCoroutine(Co_Initialize(), nameof(Panel_HUD));

		joystick.emoji.GetComponent<EmotionController>().Refresh();

#if UNITY_STANDALONE || UNITY_EDITOR
		joystick.right.GetComponent<CanvasGroup>().alpha = 0f;
		joystick.right.GetComponent<CanvasGroup>().blocksRaycasts = false;
		joystick.left.GetComponent<CanvasGroup>().alpha = 0f;
		joystick.left.GetComponent<CanvasGroup>().blocksRaycasts = false;
#endif
	}






	public void CacheJumpSprintEvent(UnityAction<bool> _jump, UnityAction<bool> _dash)
	{
		joystick.jump.buttonStateOutputEvent.AddListener(_jump);
		joystick.dash.buttonStateOutputEvent.AddListener(_dash);
	}

	public void DeleteJumpSprintEvent()
	{
		joystick.jump.buttonClickOutputEvent.RemoveAllListeners();
		joystick.jump.buttonStateOutputEvent.RemoveAllListeners();

		joystick.dash.buttonClickOutputEvent.RemoveAllListeners();
		joystick.dash.buttonStateOutputEvent.RemoveAllListeners();
	}



	#endregion

	#region Update

	private void Update()
	{
		if (!cached) return;

		joystick.Update();
		//chat.Update();
	}

	#endregion


	public void EnableOnlyLeftJoystick(bool _active, bool _instant = false)
	{
		if (_instant)
		{
			joystick.left.SetActive(_active);
			joystick.right.SetActive(!_active);

			return;
		}

		float alpha = _active ? 0f : 1f;

		FadeUtils.FadeCanvasGroup(joystick.left.GetComponent<CanvasGroup>(), 1 - alpha);
		FadeUtils.FadeCanvasGroup(joystick.right.GetComponent<CanvasGroup>(), alpha);
		FadeUtils.FadeCanvasGroup(chat.GetComponent<CanvasGroup>(), alpha);
	}

	public void EnableControllerUI(bool _active)
	{
		joystick.left.SetActive(_active);
		joystick.right.SetActive(_active);
	}

	public void Show(bool _show = true, float _delay = 0f, Action _start = null, Action _end = null)
	{
		float target = _show ? 1f : 0f;

		FadeUtils.FadeCanvasGroup(GetComponent<CanvasGroup>(), target, 1f, _delay, _start, _end);
	}
}