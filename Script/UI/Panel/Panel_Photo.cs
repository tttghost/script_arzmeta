using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using TMPro;
using MEC;
using FrameWork.Network;

public class Panel_Photo : PanelBase
{
	[SerializeField] List<Define.CameraPoseInteraction> cameraPose;
	List<Button> btn_poses = new List<Button>();

	Image img_Flash;
	Image img_Picture;

	Button btn_Back;
	Button btn_Capture;

	Button btn_CaptureBack;
	Button btn_Share;
	Button btn_Save;

	Button btn_ShowNickName;
	Button btn_Pose;

	Image img_ShowNickname;

	TMP_Text txtmp_Date;
	TMP_Text txtmp_Time;

	TMP_Text txtmp_Share;
	TMP_Text txtmp_Save;

	Transform cameraHolder;
	Transform captureHolder;

	CanvasGroup canvasGroupCamera;

	Clock clock;

	Sprite sprite_hide;
	Sprite sprite_show;

	ScrollRect scrollRect_Pose;
	Transform pose_content;

	public float captureSpeed = 1f;
	public float flashSpeed = 1f;

	public bool initialized;

	int hour;
	int minutes;
	int seconds;

	bool isClosed = false;
	bool isCaputreEnd = false;

	bool showNickname = true;
	bool showPose = false;

	public bool isInteract = false;

	public Action callback_BackAction = null;



	private void OnDestroy()
	{
		Util.KillCoroutine(nameof(Co_RefreshClock));
		Util.KillCoroutine(nameof(Co_PlayEmotion));
		Util.KillCoroutine(nameof(Co_Capture));
		Util.KillCoroutine(nameof(Co_CaptureEvent));
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		showPose = false;
		showNickname = true;

		BackAction_Custom = OnClick_Back;

		MyPlayer.instance.TPSController.PlayerInput.enabled = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		RefreshClock();

		BackAction_Custom = OnClick_Back;

		MyPlayer.instance.TPSController.PlayerInput.enabled = false;
	}

	protected override void SetMemberUI()
	{
		showStart += () =>
		{
			ShowCameraUI(true);

			SceneLogic.instance.SetScreenTouchParticle(false);
		};

		hideEnd += () =>
		{
			isClosed = true;

			ShowCameraUI(false);

			SceneLogic.instance.SetScreenTouchParticle(true);
		};

		this.canvasGroup.alpha = 0f;

		sprite_hide = CommonUtils.Load<Sprite>("Sprites/icon_hide");
		sprite_show = CommonUtils.Load<Sprite>("Sprites/icon_see");

		cameraHolder = Util.Search(this.gameObject, Define.CameraUI);
		captureHolder = Util.Search(this.gameObject, Define.CaptureUI);
		canvasGroupCamera = cameraHolder.GetComponent<CanvasGroup>();

		scrollRect_Pose = this.transform.Search(nameof(scrollRect_Pose)).GetComponent<ScrollRect>();
		pose_content = this.transform.Search(Define.Content);

		img_Flash = GetUI_Img(nameof(img_Flash));
		img_Flash.color = new Color(img_Flash.color.r, img_Flash.color.g, img_Flash.color.b, 0f);
		img_Picture = GetUI_Img(nameof(img_Picture));
		img_ShowNickname = GetUI_Img(nameof(img_ShowNickname));
		img_ShowNickname.sprite = sprite_hide;

		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
		btn_Capture = GetUI_Button(nameof(btn_Capture), OnClick_Capture);
		btn_CaptureBack = GetUI_Button(nameof(btn_CaptureBack), OnClick_CaptureBack);
		btn_Share = GetUI_Button(nameof(btn_Share), OnClick_Share);
		btn_Save = GetUI_Button(nameof(btn_Save), OnClick_Save);
		btn_ShowNickName = GetUI_Button(nameof(btn_ShowNickName), () => ShowNickname(showNickname = !showNickname));
		btn_Pose = GetUI_Button(nameof(btn_Pose), () => scrollRect_Pose.GetComponent<Animator>().SetBool(Define.Show, showPose = !showPose));

		txtmp_Time = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Time));
		txtmp_Share = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Share), new MasterLocalData("common_share"));
		txtmp_Save = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Save), new MasterLocalData("common_save"));
		txtmp_Date = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Date));

		MakePoseButton();
		EnablePoseToggle(true);
	}



	protected override IEnumerator<float> Co_OpenStartAct()
	{
		Show(true);

		yield return Timing.WaitForOneFrame;
	}

	protected override IEnumerator<float> Co_SetCloseEndAct()
	{
		Show(false);

		yield return Timing.WaitForOneFrame;
	}




	private void OnClick_Back()
	{
		PopPanel();

		ShowNickname(true);

		GetPanel<Panel_HUD>().Joystick.ShowItem(true);

		callback_BackAction?.Invoke();
	}

	private void BackButtonEvent_Interaction()
	{
		PopPanel();

		GetPanel<Panel_HUD>().Show(true);
		GetPanel<Panel_HUD>().Joystick.ShowItem(true);
		GetPanel<Panel_HUD>().Joystick.left.SetActive(true);
		GetPanel<Panel_HUD>().Joystick.right.SetActive(true);

		btn_Back.onClick.RemoveAllListeners();
		btn_Back.onClick.AddListener(OnClick_Back);

		showEnd -= () => GetPanel<Panel_HUD>().canvasGroup.blocksRaycasts = false;

		isInteract = false;
	}

	public void SetBackEvent()
	{
		showEnd += () => GetPanel<Panel_HUD>().canvasGroup.blocksRaycasts = true;

		btn_Back.onClick.RemoveAllListeners();
		btn_Back.onClick.AddListener(BackButtonEvent_Interaction);

		isInteract = true;
	}

	public void ShowCameraUI(bool _show)
	{
		if (canvasGroupCamera == null)
		{
			canvasGroupCamera = this.transform.Search("CameraUI").GetComponent<CanvasGroup>();
		}

		canvasGroupCamera.alpha = _show ? 1f : 0f;
		canvasGroupCamera.blocksRaycasts = _show;
	}






	#region Share and Save

	private void OnClick_Share()
	{
		Util.RunCoroutine(Co_CaptureEvent(ArzMetaScreenShotController.Instance.ShareContents), nameof(Co_CaptureEvent));
	}

	private void OnClick_Save()
	{
		btn_Save.interactable = false;

		Util.RunCoroutine(Co_CaptureEvent(ArzMetaScreenShotController.Instance.SaveContents), nameof(Co_CaptureEvent));
	}

	private void OnClick_Capture() => Util.RunCoroutine(Co_Capture(), nameof(Co_CaptureEvent));

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

	#endregion





	#region Basic Methods

	private IEnumerator<float> Co_Capture()
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

		if(!isInteract)
		{
			FadeUtils.FadeCanvasGroupAlphaOnly(OpenPanel<Panel_HUD>().canvasGroup, 1f, 2f, .25f);
		}
		
		GetPanel<Panel_HUD>().Joystick.ShowItem(false);

		isCaputreEnd = false;
	}

	private IEnumerator<float> Co_CaptureEvent(Action _action = null)
	{
		yield return Timing.WaitForOneFrame;

		Single.Sound.Click();

		_action?.Invoke();
	}




	private void MakePoseButton()
	{
		var prefab = CommonUtils.Load<GameObject>(Define.PathButtonPose);

		for (int i = 0; i < cameraPose.Count; i++)
		{
			var poseObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, pose_content);
			poseObject.transform.localPosition = Vector3.zero;
			poseObject.transform.localRotation = Quaternion.identity;
			poseObject.gameObject.name = cameraPose[i].ToString();

			btn_poses.Add(poseObject.GetComponent<Button>());
		}

		for (int i = 0; i < btn_poses.Count; i++)
		{
			int index = i;

			btn_poses[i].transform.GetChild(0).GetComponent<Image>().sprite = GetPoseSprite(cameraPose[index]);
			btn_poses[i].GetComponent<Button>().onClick.AddListener(() => SitPose(cameraPose[index]));
		}
	}

	public void SetPoseButton(bool _isSit)
	{
		if(_isSit)
		{
			for (int i = 0; i < btn_poses.Count; i++)
			{
				int index = i;
				btn_poses[i].GetComponent<Button>().onClick.RemoveAllListeners();
				btn_poses[i].GetComponent<Button>().onClick.AddListener(() => SitPose(cameraPose[index]));
			}
		}

		else
		{
			for (int i = 0; i < btn_poses.Count; i++)
			{
				int index = i;
				btn_poses[i].GetComponent<Button>().onClick.RemoveAllListeners();
				btn_poses[i].GetComponent<Button>().onClick.AddListener(() => StandPose(cameraPose[index]));
			}
		}
	}

	private Sprite GetPoseSprite(Define.CameraPoseInteraction _cameraPose)
	{
		string path = string.Empty;

		switch (_cameraPose)
		{
			case Define.CameraPoseInteraction.None:
				path = "Addressable/ArzPhone/Icon/Emoji/None";
				break;
			case Define.CameraPoseInteraction.Cheer:
				path = "Addressable/ArzPhone/Icon/Emoji/Cheer";
				break;
			case Define.CameraPoseInteraction.Clap:
				path = "Addressable/ArzPhone/Icon/Emoji/Clap";
				break;
			case Define.CameraPoseInteraction.Cute:
				path = "Addressable/ArzPhone/Icon/Emoji/Cute";
				break;
			case Define.CameraPoseInteraction.Encourage:
				path = "Addressable/ArzPhone/Icon/Emoji/Encourage";
				break;
			case Define.CameraPoseInteraction.Hi:
				path = "Addressable/ArzPhone/Icon/Emoji/Hi";
				break;
			case Define.CameraPoseInteraction.Surprise:
				path = "Addressable/ArzPhone/Icon/Emoji/Surprise";
				break;
		}

		return CommonUtils.Load<Sprite>(path);
	}

	private void SitPose(Define.CameraPoseInteraction _cameraPose)
	{
		string animation = string.Empty;

		switch (_cameraPose)
		{
			case Define.CameraPoseInteraction.None:
				animation = Define.Action_Idle;
				break;
			case Define.CameraPoseInteraction.Cheer:
				animation = Define.Sit_Emote_Cheer;
				break;
			case Define.CameraPoseInteraction.Clap:
				animation = Define.Sit_Emote_Clap;
				break;
			case Define.CameraPoseInteraction.Cute:
				animation = Define.Sit_Emote_Cute;
				break;
			case Define.CameraPoseInteraction.Encourage:
				animation = Define.Sit_Emote_Encourage;
				break;
			case Define.CameraPoseInteraction.Hi:
				animation = Define.Sit_Emote_Hi;
				break;
			case Define.CameraPoseInteraction.Surprise:
				animation = Define.Sit_Emote_Surprise;
				break;
		}

		Util.RunCoroutine(Co_PlayEmotion(animation), nameof(Co_PlayEmotion));
	}

	private void StandPose(Define.CameraPoseInteraction _cameraPose)
	{
		string animation = string.Empty;

		switch (_cameraPose)
		{
			case Define.CameraPoseInteraction.None:
				animation = Define.Action_Idle;
				break;
			case Define.CameraPoseInteraction.Cheer:
				animation = Define.Emote_Cheer;
				break;
			case Define.CameraPoseInteraction.Clap:
				animation = Define.Emote_Clap;
				break;
			case Define.CameraPoseInteraction.Cute:
				animation = Define.Emote_Cute;
				break;
			case Define.CameraPoseInteraction.Encourage:
				animation = Define.Emote_Encourage;
				break;
			case Define.CameraPoseInteraction.Hi:
				animation = Define.Emote_Hi;
				break;
			case Define.CameraPoseInteraction.Surprise:
				animation = Define.Emote_Surprise;
				break;
		}

		Util.RunCoroutine(Co_PlayEmotion(animation), "EmotionSequance", Util.SceneCoroutine.Player);
	}

	private IEnumerator<float> Co_PlayEmotion(string _parameter)
	{
		if (MyPlayer.instance.TPSController.Movement.IsMoving() ||
			MyPlayer.instance.TPSController.DashController.isDashing ||
			!MyPlayer.instance.TPSController.Movement.grounded
		) yield break;

		MyPlayer.instance.TPSController.MotionController.PlayMotion(_parameter);
	}




	private void EnablePoseToggle(bool _show)
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

		img_ShowNickname.sprite = showNickname ? sprite_hide : sprite_show;
	}

	private void RefreshClock() => Util.RunCoroutine(Co_RefreshClock(), nameof(Co_RefreshClock));
	
	private IEnumerator<float> Co_RefreshClock()
	{
		clock = new Clock();

		txtmp_Date.text = clock.GetCurrentDate();

		while (this.gameObject.activeInHierarchy)
		{
			txtmp_Time.text = clock.GetCurrentTime(true);

			yield return Timing.WaitForOneFrame;
		}
	}


	#endregion
}
