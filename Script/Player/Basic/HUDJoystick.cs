using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using MEC;
using System;
using static EasingFunction;
using UnityEngine.Events;
using FrameWork.UI;

[Serializable]
public class HUDJoystick
{
	[SerializeField] public GameObject zone;

	[SerializeField] public UIVirtualJoystick virtualJoystick;
	[SerializeField] public UIVirtualButton jump;
	[SerializeField] public UIVirtualButton dash;

	[SerializeField] public GameObject handle;
	[SerializeField] public Image img_HandlePivot;

	[SerializeField] public GameObject left;
	[SerializeField] public GameObject right;

	[SerializeField] public GameObject chat;

	[SerializeField] public GameObject emoji;
	[SerializeField] public GameObject emojiList;
	[SerializeField] public GameObject phone;
	[SerializeField] public GameObject phone_alarm;
	[SerializeField] public GameObject perspective;
	[SerializeField] public GameObject group_menu;
	[SerializeField] public GameObject view_HUD_TopLeft;
	[SerializeField] public GameObject image_Handle;

	[SerializeField] public List<GameObject> joystick_items = new List<GameObject>();

	private Vector2 origin = Vector2.zero;
	private Vector2 normalized = Vector2.zero;

	private bool isPointerDown = true;
	public bool isUsingJoystick = false;

	Vector3 joystickPosition;
	Vector3 prevJoystickPosition;
	
	[HideInInspector] public Transform joystick_vehicle;
	[HideInInspector] public Transform joystick_Vehicle_Right;
	[HideInInspector] public Transform joystick_Vehicle_Left;
	[HideInInspector] public Transform joystick_Accelerate;
	[HideInInspector] public Transform joystick_Break;
	[HideInInspector] public Transform joystick_Boost;

	[HideInInspector] public Transform img_BoostCoolTime;
	public bool isCoolDown = false;

	public HUDJoystick(Panel_HUD _panel)
	{
		img_HandlePivot = _panel.GetUI<Image>(Define.img_HandlePivot);

		handle = _panel.transform.Search(Define.JoystickMove).gameObject;
		left = _panel.transform.Search(Define.joystick_left).gameObject;
		right = _panel.transform.Search(Define.joystick_right).gameObject;

		emoji = _panel.transform.Search(Define.hud_Emoji).gameObject;
		phone = _panel.transform.Search(Define.hud_Phone).gameObject;
		phone_alarm = _panel.transform.Search(Define.go_PhoneNew).gameObject;
		emojiList = _panel.transform.Search(Define.EmojiList).gameObject;
		perspective = _panel.transform.Search(Define.Perspective).gameObject;
		group_menu = _panel.transform.Search(Define.group_menu).gameObject;


		view_HUD_TopLeft = _panel.transform.Search(nameof(view_HUD_TopLeft)).gameObject;
		image_Handle = _panel.transform.Search(nameof(image_Handle)).gameObject;

		phone_alarm.SetActive(false);

		joystick_vehicle = _panel.transform.Search(nameof(joystick_vehicle));
		joystick_Vehicle_Right = _panel.transform.Search(nameof(joystick_Vehicle_Right));
		joystick_Vehicle_Left = _panel.transform.Search(nameof(joystick_Vehicle_Left));
		joystick_Accelerate = _panel.transform.Search(nameof(joystick_Accelerate));
		joystick_Break = _panel.transform.Search(nameof(joystick_Break));
		joystick_Boost = _panel.transform.Search(nameof(joystick_Boost));
		img_BoostCoolTime = _panel.transform.Search(nameof(img_BoostCoolTime));
	}

	public void Update()
	{
		if (MyPlayer.instance.TPSController.StarterInputs == null || img_HandlePivot == null) return;

		origin = MyPlayer.instance.TPSController.StarterInputs.move;
		normalized = MyPlayer.instance.TPSController.StarterInputs.move.normalized;

		float x = Mathf.Abs(origin.x) > Mathf.Abs(normalized.x) ? normalized.x : origin.x;
		float y = Mathf.Abs(origin.y) > Mathf.Abs(normalized.y) ? normalized.y : origin.y;
		float distance = Mathf.Pow(x, 2) + Mathf.Pow(y, 2);

		if (distance > 0f)
		{
			if (img_HandlePivot.transform.localEulerAngles.z != 270f)
			{
				if (!isPointerDown) return;

                float scale = handle.GetComponent<RectTransform>().sizeDelta.x / 240f;
                Util.RunCoroutine(JoystickSequance(scale, 2f, 3f, Ease.EaseOutBack), nameof(JoystickSequance));

                isPointerDown = false;
			}
		}

		else
		{
			if (isPointerDown) return;

            float scale = handle.GetComponent<RectTransform>().sizeDelta.x / 120f;
            Util.RunCoroutine(JoystickSequance(scale, 1.2f, 1f, Ease.EaseOutQuint), nameof(JoystickSequance));

            isPointerDown = true;
		}
	}

	IEnumerator<float> JoystickSequance(float _start, float _end, float _easespeed, Ease _ease)
	{
		float lerpvalue = 0f;

		while (lerpvalue <= 1f)
		{
			Function function = GetEasingFunction(_ease);

			float value = function(_start, _end, lerpvalue += _easespeed * Time.deltaTime);

			handle.GetComponent<RectTransform>().sizeDelta = (Vector2.one * 120f) * value;

			yield return Timing.WaitForOneFrame;
		}
	}

	public void ShowItem(bool _enable)
	{
		group_menu.SetActive(_enable);
		perspective.SetActive(_enable);
		view_HUD_TopLeft.SetActive(_enable);

		for(int i = 0; i < joystick_items.Count; i++)
		{
			joystick_items[i].SetActive(_enable);
		}
	}

	public void AddItem()
	{
		joystick_items.Add(left);
		joystick_items.Add(right);
		joystick_items.Add(chat);
	}



	public void EnableRightJoystick(bool _show)
	{
		var canvasGroup = right.GetComponent<CanvasGroup>();

		var alpha = _show ? 1f : 0f;

		FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
	}

	public void EnableLeftJoystick(bool _show)
	{
		var canvasGroup = left.GetComponent<CanvasGroup>();

		var alpha = _show ? 1f : 0f;

		FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
	}

	public void EnableVehicleJoystick(bool _show)
	{
		var canvasGroup = joystick_vehicle.GetComponent<CanvasGroup>();

		var alpha = _show ? 1f : 0f;

		FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
	}

	public void EnableBoost(bool _show)
	{
		var canvasGroup = joystick_Boost.GetComponent<CanvasGroup>();

		var alpha = _show ? 1f : .5f;

		if (_show)
		{
			FadeUtils.FadeCanvasGroup(canvasGroup, alpha);
		}

		else
		{
			FadeUtils.FadeCanvasGroup(canvasGroup, alpha, _start: () => canvasGroup.blocksRaycasts = false);
		}
	}

	public void HandleVehicleButtonEvent(VehicleButton _name, UnityAction<bool> _action, bool _isAddListener)
	{
		var vehicleButton = GetVehicleButton(_name);

		if (vehicleButton != null)
		{
			var button = vehicleButton.GetComponent<UIVirtualButton>();

			if (_isAddListener)
			{
				button.buttonStateOutputEvent.AddListener(_action);
			}

			else
			{
				button.buttonStateOutputEvent.RemoveListener(_action);
			}
		}
	}

	public Transform GetVehicleButton(VehicleButton _name)
	{
		switch (_name)
		{
			case VehicleButton.Accelerate:
				return joystick_Accelerate;

			case VehicleButton.Break:
				return joystick_Break;

			case VehicleButton.Boost:
				return joystick_Boost;

			case VehicleButton.Right:
				return joystick_Vehicle_Right;

			case VehicleButton.Left:
				return joystick_Vehicle_Left;

			default:
				return null;
		}
	}

	public void BoostCoolDown(float _time)
	{
		if (isCoolDown) return;

		Util.RunCoroutine(Co_BoostCoolDown(_time), nameof(Co_BoostCoolDown));
	}

	private IEnumerator<float> Co_BoostCoolDown(float _time)
	{
		isCoolDown = true;

		var elapsedTime = 0f;
		var color = img_BoostCoolTime.GetComponent<Image>().color;
		var alpha = color.a;

		joystick_Boost.GetComponent<CanvasGroup>().blocksRaycasts = false;

		while (img_BoostCoolTime.GetComponent<Image>().fillAmount < 1f)
		{
			joystick_Boost.GetComponent<CanvasGroup>().blocksRaycasts = false;

			elapsedTime += Time.deltaTime;

			img_BoostCoolTime.GetComponent<Image>().fillAmount = elapsedTime / _time;

			yield return Timing.WaitForOneFrame;
		}

		img_BoostCoolTime.GetComponent<Image>().fillAmount = 1f;



		elapsedTime = 0f;

		while (Mathf.Abs(alpha - 0f) >= 0.001f)
		{
			joystick_Boost.GetComponent<CanvasGroup>().blocksRaycasts = false;

			alpha = Mathf.Lerp(alpha, 0f, elapsedTime += Time.deltaTime);

			img_BoostCoolTime.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);

			yield return Timing.WaitForOneFrame;
		}

		img_BoostCoolTime.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0f);

		isCoolDown = false;

		yield return Timing.WaitForOneFrame;

		img_BoostCoolTime.GetComponent<Image>().fillAmount = 0f;
		img_BoostCoolTime.GetComponent<Image>().color = color;
		joystick_Boost.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public void KillBoostCoolDown() => Util.KillCoroutine(nameof(Co_BoostCoolDown));
}

public enum VehicleButton
{
	None,
	Right,
	Left,
	Accelerate,
	Break,
	Boost
}
