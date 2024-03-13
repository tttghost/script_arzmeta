using FrameWork.Network;
using FrameWork.UI;
using MEC;
using SimpleFileBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Panel_Camera : PanelBase
{
	#region Members

	Transform cameraHolder;
	Transform captureHolder;
	Transform poseHolder;

	Button btn_Back;
	Button btn_Capture;
	Button btn_Switch;
	Button btn_ShowNickName;
	Button btn_Pose;

	Button btn_Share;
	Button btn_Save;
	Button btn_CaptureBack;

	ScrollRect scrollRect_Pose;

	Image img_Flash;
	Image img_Picture;
	Image img_ShowNickname;

	TMP_Text txtmp_Date;
	TMP_Text txtmp_Time;

	TMP_Text txtmp_Share;
	TMP_Text txtmp_Save;

	public Sprite sprite_hide;
	public Sprite sprite_show;

	Clock clock;

	[SerializeField] List<Define.CameraPose> cameraPose;
	List<Button> btn_poses = new List<Button>();

	[Range(0f, 10f)] public float flashSpeed = 1f;

	public bool isTPSmode = false;
	bool isShowNickname = true;
	bool isShowPose = false;
	bool isFadeOut = false;
	bool isCaputreEnd = false;
	bool isUpdate = false;
	public bool isBack = false;
	public bool isSwitching = false;

	public Action cameraOn;
	public Action cameraOff;

	CoroutineHandle handle_lensSwitch;

	#endregion



	#region Initialize

	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_Initialize) + this.GetHashCode());
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		BackAction_Custom = OnClick_Back;

		RefreshClock();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		OpenPanel<Panel_HUD>().canvasGroup.interactable = true;

		ShowNickname(isShowNickname = true);

		RefreshPoseToggle();

		scrollRect_Pose.GetComponent<Animator>().SetBool(Define.Show, isShowPose = false);

		MyPlayer.instance.TPSController.Movement.ResetSprintSpeed();
		MyPlayer.instance.TPSController.Movement.EnableJumpInput(true);
		MyPlayer.instance.TPSController.Movement.EnableDashInput(true);

		ArzMetaManager.Instance.PhoneController.Phone.Camera.FirstPersonCamera(false);

		isTPSmode = false;
		isBack = true;

		FindObjectOfType<ShortcutManager>().ExitCamera();
	}



	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		clock = new Clock();

		showInstant += () => ClosePanel<Panel_Camera>();
		showStart += () =>
		{
			cameraOn?.Invoke();
			SceneLogic.instance.SetScreenTouchParticle(false);
		};
		hideEnd += () =>
		{
			cameraOn?.Invoke();

			GetPanel<Panel_HUD>().Joystick.ShowItem(true);
			SceneLogic.instance.SetScreenTouchParticle(true);
		};

		this.GetComponent<CanvasGroup>().alpha = 0f;
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		Util.RunCoroutine(Co_Initialize(), nameof(Co_Initialize) + this.GetHashCode());
	}

	IEnumerator<float> Co_Initialize()
	{
		yield return Timing.WaitUntilTrue(() => ArzMetaManager.Instance && MyPlayer.instance);

		sprite_hide = CommonUtils.Load<Sprite>("Sprites/icon_hide");
		sprite_show = CommonUtils.Load<Sprite>("Sprites/icon_see");

		scrollRect_Pose = Util.Search<ScrollRect>(this.gameObject, Define.ScrollView_Pose);

		cameraHolder = Util.Search(this.gameObject, Define.CameraUI);
		captureHolder = Util.Search(this.gameObject, Define.CaptureUI);
		poseHolder = Util.Search(scrollRect_Pose.gameObject, Define.Content);

		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
		btn_Capture = GetUI_Button(nameof(btn_Capture), OnClick_Capture);
		btn_Switch = GetUI_Button(nameof(btn_Switch), OnClick_LensSwitch);
		btn_CaptureBack = GetUI_Button(nameof(btn_CaptureBack), OnClick_CaptureBack);
		btn_Share = GetUI_Button(nameof(btn_Share), OnClick_Share);
		btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);
		btn_ShowNickName = GetUI_Button(nameof(btn_ShowNickName), OnClick_ShowNickname);
		btn_Pose = GetUI_Button(nameof(btn_Pose), OnClick_Pose);

		img_Flash = GetUI_Img(nameof(img_Flash));
		img_Picture = GetUI_Img(nameof(img_Picture));
		img_ShowNickname = GetUI_Img(nameof(img_ShowNickname));

		txtmp_Date = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Date));
		txtmp_Time = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Time));
		txtmp_Share = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Share), new MasterLocalData("common_share"));
		txtmp_Save = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Save), new MasterLocalData("common_save"));

		img_ShowNickname.sprite = sprite_show;
		img_Flash.color = new Color(img_Flash.color.r, img_Flash.color.g, img_Flash.color.b, 0f);

		CreatePoseButtons();
		RefreshClock();
		RefreshPoseToggle();

		MyPlayer.instance.TPSController.moveEvent.AddListener(MoveLensEvent);

#if UNITY_STANDALONE || UNITY_EDITOR
		btn_Share.gameObject.SetActive(false);
#endif
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		var sceneName = Single.Scene.GetSceneId();

		if (Single.RealTime.roomType.current == RoomType.Meeting || Single.RealTime.roomType.current == RoomType.Lecture)
		{
			GetPanel<Panel_HUD>().viewHudTopLeft.gameObject.SetActive(false);
		}

		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().viewHudTopCenter.canvasGroup, 0f, 2f);
		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().viewHudTopRight.canvasGroup, 0f, 2f);

		GetPanel<Panel_HUD>().Menu.viewHudTopLeft.gameObject.SetActive(false);
		GetPanel<Panel_HUD>().Joystick.right.SetActive(false);

		MyPlayer.instance.TPSController.Camera.SetYawPitchInstant(MyPlayer.instance.transform.localEulerAngles.y, 45f);
		MyPlayer.instance.Childrens[Cons.PLAYER_CAMERAROOT].transform.localPosition = new Vector3(0f, 1.5f, 0f);

		yield return Timing.WaitUntilTrue(() => Vector3.Distance(MyPlayer.instance.Childrens[Cons.PLAYER_CAMERAROOT].transform.localPosition, new Vector3(0f, 1.5f, 0f)) < 0.001f);

		MyPlayer.instance.TPSController.MotionController.PlayAnimation("phone_cameraOn");

		MyPlayer.instance.TPSController.Camera.pitch = 0f;
		MyPlayer.instance.TPSController.Camera.yaw = MyPlayer.instance.transform.localEulerAngles.y;
		MyPlayer.instance.TPSController.Camera.SetCameraDistance(-1f, _instant: true);

		MyPlayer.instance.TPSController.Movement.SetSprintSpeed(2.5f);
		MyPlayer.instance.TPSController.Movement.EnableJumpInput(false);
		MyPlayer.instance.TPSController.Movement.EnableDashInput(false);

		MyPlayer.instance.EnableInput(true);

		GetPanel<Panel_HUD>().canvasGroup.blocksRaycasts = true;
		GetPanel<Panel_HUD>().canvasGroup.alpha = 1f;

		GetPanel<Panel_HUD>().Touch.pinchCustom.enabled = true;
		GetPanel<Panel_HUD>().Touch.eventChecker.enabled = true;
		GetPanel<Panel_HUD>().Joystick.virtualJoystick.enabled = true;

		GetPanel<Panel_HUD>().Joystick.ShowItem(false);
		GetPanel<Panel_HUD>().Joystick.left.SetActive(true);
		GetPanel<Panel_HUD>().Joystick.virtualJoystick.GetComponent<UIVirtualJoystick>().enabled = true;

		GetPanel<Panel_HUD>().Chat.gameObject.SetActive(false);

		Show(true);

		EnableTouchInteractables(false);
	}

	protected override IEnumerator<float> Co_OpenEndAct()
	{
		yield return Timing.WaitUntilTrue(() => this.canvasGroup.alpha >= 1f);

		yield return Timing.WaitForSeconds(.5f);

		isBack = false;
	}

	protected override IEnumerator<float> Co_SetCloseStartAct()
	{
		Show(false);

		EnableTouchInteractables(true);

		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha <= 0);

		isTPSmode = false;

		MyPlayer.instance.TPSController.Camera.SetEaseSpeed(.5f);

		cameraHolder.GetComponent<CanvasGroup>().alpha = 1f;
		cameraHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().viewHudTopCenter.canvasGroup, 1f);
		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().viewHudTopRight.canvasGroup, 1f);
	}

	protected override IEnumerator<float> Co_SetCloseEndAct()
	{
		yield return Timing.WaitForOneFrame;

		MyPlayer.instance.TPSController.Camera.SetYawPitchInstant(MyPlayer.instance.gameObject.transform.localEulerAngles.y, 0f);

		if (Single.RealTime.roomType.current == RoomType.Meeting || Single.RealTime.roomType.current == RoomType.Lecture)
		{
			GetPanel<Panel_HUD>().viewHudTopLeft.gameObject.SetActive(true);
		}

		Util.ChangeLayerMask(MyPlayer.instance.Childrens[Cons.AVATARPARTS], Define.Invisible);
		Util.ChangeLayerMask(MyPlayer.instance.Childrens[Cons.HUDPARENT], Define.Invisible);

		MyPlayer.instance.TPSController.Camera.SetCameraDistance(6f, _instant: true);
		MyPlayer.instance.Childrens[Cons.PLAYER_CAMERAROOT].transform.localPosition = new Vector3(0f, 1.3f, 0f);
		MyPlayer.instance.SetFirstPersonView();

		GetPanel<Panel_HUD>().Menu.viewHudTopLeft.gameObject.SetActive(true);
		GetPanel<Panel_HUD>().Joystick.right.SetActive(true);
		GetPanel<Panel_HUD>().Chat.gameObject.SetActive(true);
	}


	public void CameraUICanvasGroup(bool _show)
	{
		if (cameraHolder == null) return;

		cameraHolder.GetComponent<CanvasGroup>().blocksRaycasts = _show;
		cameraHolder.GetComponent<CanvasGroup>().interactable = _show;
	}

	private void EnableTouchInteractables(bool _enabled)
	{
		GetPanel<Panel_HUD>().Touch.touchInteracter.Distance = _enabled ? 100f : 0f;	}

	#endregion



	#region Methods

	#region Camera Pose

	private void CreatePoseButtons()
	{
		GameObject posePrefab = CommonUtils.Load<GameObject>(Define.PathButtonPose);

		for (int i = 0; i < cameraPose.Count; i++)
		{
			GameObject pose = Instantiate(posePrefab, Vector3.zero, Quaternion.identity, poseHolder);
			pose.transform.localPosition = Vector3.zero;
			pose.transform.localRotation = Quaternion.identity;
			pose.gameObject.name = cameraPose[i].ToString();

			btn_poses.Add(pose.GetComponent<Button>());
		}

		for (int i = 0; i < btn_poses.Count; i++)
		{
			int index = i;
			btn_poses[i].transform.GetChild(0).GetComponent<Image>().sprite = SetPoseIcon(cameraPose[index]);
			btn_poses[i].GetComponent<Button>().onClick.AddListener(() => SetPoseEvent(cameraPose[index]));
		}
	}

	private Sprite SetPoseIcon(Define.CameraPose _cameraPose)
	{
		string path = string.Empty;

		switch (_cameraPose)
		{
			case Define.CameraPose.None:
				path = "Addressable/ArzPhone/Icon/Emoji/None";
				break;
			case Define.CameraPose.Cheer:
				path = "Addressable/ArzPhone/Icon/Emoji/Cheer";
				break;
			case Define.CameraPose.Clap:
				path = "Addressable/ArzPhone/Icon/Emoji/Clap";
				break;
			case Define.CameraPose.Cute:
				path = "Addressable/ArzPhone/Icon/Emoji/Cute";
				break;
			case Define.CameraPose.Encourage:
				path = "Addressable/ArzPhone/Icon/Emoji/Encourage";
				break;
			case Define.CameraPose.Hi:
				path = "Addressable/ArzPhone/Icon/Emoji/Hi";
				break;
			case Define.CameraPose.Surprise:
				path = "Addressable/ArzPhone/Icon/Emoji/Surprise";
				break;
		}

		return CommonUtils.Load<Sprite>(path);
	}

	private void SetPoseEvent(Define.CameraPose _cameraPose)
	{
		string animation = string.Empty;

		switch (_cameraPose)
		{
			case Define.CameraPose.None:
				animation = Define.Action_Idle;
				break;
			case Define.CameraPose.Cheer:
				animation = Define.Emote_Cheer;
				break;
			case Define.CameraPose.Clap:
				animation = Define.Emote_Clap;
				break;
			case Define.CameraPose.Cute:
				animation = Define.Emote_Cute;
				break;
			case Define.CameraPose.Encourage:
				animation = Define.Emote_Encourage;
				break;
			case Define.CameraPose.Hi:
				animation = Define.Emote_Hi;
				break;
			case Define.CameraPose.Surprise:
				animation = Define.Emote_Surprise;
				break;
		}

		OnClick_Pose();

		Timing.KillCoroutines("EmotionSequance");

		Util.RunCoroutine(Co_Emotion(animation), "EmotionSequance", Util.SceneCoroutine.Player);
	}

	private IEnumerator<float> Co_Emotion(string _parameter)
	{
		if (MyPlayer.instance.TPSController.Movement.IsMoving() ||
			MyPlayer.instance.TPSController.DashController.isDashing ||
			!MyPlayer.instance.TPSController.Movement.grounded
		) yield break;

		MyPlayer.instance.TPSController.MotionController.PlayMotion(_parameter);
	}

	#endregion




	#region Nickname

	private void RefreshPoseToggle(bool _show = false)
	{
		float alpha = _show ? 1f : .25f;

		btn_Pose.GetComponent<CanvasGroup>().alpha = alpha;
		btn_Pose.GetComponent<CanvasGroup>().blocksRaycasts = _show;
	}

	private void ShowNickname(bool _show)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i] == null || players[i].gameObject.activeInHierarchy == false) continue;

			FadeUtils.FadeCanvasGroup(
				players[i].GetComponentInChildren<HUDParent>().GetComponent<CanvasGroup>(),
				_show ? 1f : 0f, 2f
			);
		}

		img_ShowNickname.sprite = isShowNickname ? sprite_show : sprite_hide;
	}

	#endregion




	#region Share and Save

	private void OnClick_Share()
	{
		Util.RunCoroutine(Co_CaptureEvent(ArzMetaScreenShotController.Instance.ShareContents), nameof(Co_CaptureEvent));
	}

	private async void OnClick_Save()
	{
#if UNITY_STANDALONE || UNITY_EDITOR

		string fileName = MyPlayer.instance.gameObject.name;
        string path = string.Empty;
        path = await SceneLogic.instance.PushPopup<Popup_FileBrowser>().OpenFileBrowser_ShowSaveDialog(FILEBROWSER_SETFILTER.IMAGE);

        if (path != string.Empty)
        {
			OnClick_CaptureBack();

			path = CheckAndFixExtension(path);

            Texture2D texture = ArzMetaScreenShotController.Instance.ScreenShot.texture;
			Util.Tex2Image(path, texture);

			SceneLogic.instance.OpenToast<Toast_Basic>().ChainToastData(new ToastData_Basic(TOASTICON.Current, new MasterLocalData("5201")));
		}
#else
		Util.RunCoroutine(Co_CaptureEvent(ArzMetaScreenShotController.Instance.SaveContents), nameof(Co_CaptureEvent));
#endif
	}

	public static string CheckAndFixExtension(string fileName)
	{
		string extension = Path.GetExtension(fileName);

		if (extension == ".jpg" || extension == ".png")
		{
			return fileName;
		}
		else 
		{ 
			return fileName + ".png";
		}
	}

	private void OnClick_CaptureBack()
	{
		btn_Save.interactable = true;

		Util.RunCoroutine(Co_CaptureEvent(() =>
		{
			ArzMetaScreenShotController.Instance.ClearScreenShot();
			isCaputreEnd = true;
		}
		));
	}

	private void OnClick_Pose()
	{
		scrollRect_Pose.GetComponent<Animator>().SetBool(Define.Show, isShowPose = !isShowPose);

		btn_Pose.GetComponent<CanvasGroup>().blocksRaycasts = false;

		Util.RunCoroutine(Co_OnClick_Pose());
	}

	private IEnumerator<float> Co_OnClick_Pose()
	{
		yield return Timing.WaitForSeconds(.5f);

		btn_Pose.GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	private void OnClick_ShowNickname() => ShowNickname(isShowNickname = !isShowNickname);


#endregion




#region Camera Lens Switch

	IEnumerator<float> Co_LensSwitch()
	{
		isSwitching = true;

		Single.Scene.FadeOut(6f, () => isFadeOut = true);

		MyPlayer.instance.EnableInput(false);

		CameraUICanvasGroup(false);

		yield return Timing.WaitUntilTrue(() => isFadeOut);

		isTPSmode = !isTPSmode;

		if (isTPSmode)
		{
			ArzMetaManager.Instance.PhoneController.Phone.Camera.ThirdPersonCamera();
			RefreshPoseToggle(true);

			SceneLogic.instance.GetPanel<Panel_HUD>().Touch.pinchCustom.Zoom = 50f;
		}

		else
		{
			ArzMetaManager.Instance.PhoneController.Phone.Camera.FirstPersonCamera();
			RefreshPoseToggle();

			scrollRect_Pose.GetComponent<Animator>().SetBool(Define.Show, isShowPose = false);
		}

		SceneLogic.instance.GetPanel<Panel_HUD>().Touch.pinchCustom.Zoom = 50f;

		yield return Timing.WaitForSeconds(.25f);

		Single.Scene.FadeIn(6f, () => isFadeOut = false);

		if (isTPSmode)
		{
			MyPlayer.instance.EnableInput(false);

			yield return Timing.WaitForSeconds(.5f);

			CameraUICanvasGroup(true);
		}

		else
		{
			yield return Timing.WaitForSeconds(.5f);

			MyPlayer.instance.EnableInput(true);

			CameraUICanvasGroup(true);
		}

		yield return Timing.WaitUntilTrue(() => cameraHolder.GetComponent<CanvasGroup>().interactable);

		isSwitching = false;
	}

	public void MoveLensEvent()
	{
		if (MyPlayer.instance.TPSController.StarterInputs.move == Vector2.zero)
		{
			if (!isUpdate)
			{
				FadeUtils.FadeCanvasGroup(btn_Switch.GetComponent<CanvasGroup>(), 1f, 1f, 0f, null, () => btn_Switch.GetComponent<CanvasGroup>().blocksRaycasts = true);

				isUpdate = true;
			}
		}

		else
		{
			if (isUpdate)
			{
				FadeUtils.FadeCanvasGroup(btn_Switch.GetComponent<CanvasGroup>(), .75f, 1f, 0f, () => btn_Switch.GetComponent<CanvasGroup>().blocksRaycasts = false);

				isUpdate = false;
			}
		}
	}

#endregion




#region Screenshot

	IEnumerator<float> Co_Capture()
	{
		this.GetComponent<CanvasGroup>().blocksRaycasts = false;

		cameraHolder.GetComponent<CanvasGroup>().alpha = 0f;
		cameraHolder.GetComponent<CanvasGroup>().blocksRaycasts = false;
		OpenPanel<Panel_HUD>().canvasGroup.alpha = 0f;

		ArzMetaScreenShotController.Instance.TakeScreenShot();

		yield return Timing.WaitUntilTrue(() => ArzMetaScreenShotController.Instance.ScreenShot != null);
		yield return Timing.WaitUntilTrue(() => ArzMetaScreenShotController.Instance.ScreenShot.texture != null);

		img_Picture.sprite = Sprite.Create(ArzMetaScreenShotController.Instance.ScreenShot.texture, new Rect(0.0f, 0.0f, Screen.width, Screen.height), new Vector2(0.5f, 0.5f), 100.0f);

		FadeUtils.FadeInMaskableGraphic(img_Flash, 1f, flashSpeed * 100f);

		yield return Timing.WaitUntilTrue(() => img_Flash.GetComponent<Image>().color.a >= 1f);

		captureHolder.GetComponent<Animator>().SetBool(Define.Capture, true);

		FadeUtils.FadeOutMaskableGraphic(img_Flash, 0f, flashSpeed * 100f);

		yield return Timing.WaitUntilTrue(() => img_Flash.GetComponent<Image>().color.a <= 0f);

		this.GetComponent<CanvasGroup>().blocksRaycasts = true;

		yield return Timing.WaitUntilTrue(() => isCaputreEnd);
		captureHolder.GetComponent<Animator>().SetBool(Define.Capture, false);

		FadeUtils.FadeCanvasGroup(cameraHolder.GetComponent<CanvasGroup>(), 1f, 2f, .25f);
		FadeUtils.FadeCanvasGroupAlphaOnly(OpenPanel<Panel_HUD>().canvasGroup, 1f, 2f, .25f);

		isCaputreEnd = false;
	}

	IEnumerator<float> Co_CaptureEvent(Action _action = null)
	{
		yield return Timing.WaitForSeconds(0f);

		Single.Sound.Click();

		_action?.Invoke();
	}

#endregion




#region Clock

	private void RefreshClock()
	{
		Util.SetMasterLocalizing(txtmp_Date, clock.GetCurrentDate());

		Util.RunCoroutine(Co_RefreshClock());
	}

	IEnumerator<float> Co_RefreshClock()
	{
		while (this.gameObject.activeInHierarchy)
		{
			Util.SetMasterLocalizing(txtmp_Time, clock.GetCurrentTime(true));

			yield return Timing.WaitForOneFrame;
		}
	}

#endregion

#endregion



#region Button Events

	private void OnClick_Back()
	{
		if (isBack || isSwitching) return;

		ArzMetaManager.Instance.PhoneController.Phone.Home();

		MyPlayer.instance.EnableInput(false);
		MyPlayer.instance.TPSController.MotionController.PlayAnimation("phone_cameraOff");

		SceneLogic.instance.GetPanel<Panel_Phone>().ShowAlphaOnly(true, .5f);
		SceneLogic.instance.GetPanel<Panel_Phone>().GetComponent<CanvasGroup>().blocksRaycasts = true;

		SceneLogic.instance.GetPanel<Panel_HUD>().GetComponent<CanvasGroup>().blocksRaycasts = false;
		SceneLogic.instance.GetPanel<Panel_HUD>().Touch.pinchCustom.enabled = true;
		SceneLogic.instance.GetPanel<Panel_HUD>().Touch.pinchCustom.virtualCam = MyPlayer.instance.TPSController.Camera.VirtualCam;

		SceneLogic.instance.PopPanel();

		SceneLogic.instance.GetPanel<Panel_HUD>().Show(
		false, 0f, null,
		() =>
		{
			SceneLogic.instance.GetPanel<Panel_HUD>().EnableOnlyLeftJoystick(false, true);
			MyPlayer.instance.TPSController.Camera.SetEaseSpeed(1f);
		});

		isBack = true;
	}

	private void OnClick_Capture() => Util.RunCoroutine(Co_Capture());

	public void OnClick_LensSwitch() => handle_lensSwitch = Util.RunCoroutine(Co_LensSwitch());

#endregion
}