using System.Collections.Generic;
using FrameWork.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using BitBenderGames;
using UnityEngine.EventSystems;
using FrameWork.Network;
using System.Linq;
using UnityEngine.Rendering.Universal;

public enum GameWorld
{
	ArzMetaWorld,
	BusanWorld,
}

[System.Serializable]
public class MinimapTeleport
{
	public int masterId;

	public SceneName roomType;
	public Vector3 focusPoint;

	public float posX;
	public float posY;
	public float posZ;

	public float eulerY;

	public string name;
	public string description;
	public string image;

	public int mapInfoType;
	public int worldId;

	public int sort;
}

public class Panel_Map : PanelBase
{
	#region Members

	[SerializeField] GameWorld world;
	[SerializeField] MinimapTeleport[] minimapTeleports;

	ArzMetaManager arzmeta;

	RectTransform locationMark;

	TouchInputController touchInputController;
	MobileTouchCamera mobileTouchCamera;

	Camera minimapCam;

	Transform spawnPoint;
	Transform go_MapList;
	Transform content_Region;
	Transform content_Brand;

	GameObject players;

	TMP_Text txtmp_world;
	TMP_Text txtmp_Region;
	TMP_Text txtmp_Brand;
	TMP_Text txtmp_Back;

	Button btn_Handle;
	Button btn_Back;

	Image img_Arrow;

	TogglePlus togplus_Region;
	TogglePlus togplus_Brand;

	ScrollRect sview_MapList;

	Vector3 hidePoint;
	Vector3 showPoint;

	bool isHandleOut = false;
	bool isMinimapView = false;

	bool isPinching = false;
	bool isDragging = false;
	[HideInInspector] public bool isBack = false;

	int currentId;

	CoroutineHandle handle_update;

	//Camera stackCam;

	#endregion



	#region Initialize

	private void OnDestroy()
	{
		Timing.KillCoroutines(handle_update);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void OnEnable()
	{
		base.OnEnable();

		var rect = go_MapList.GetComponent<RectTransform>();
		rect.anchoredPosition = hidePoint;

		minimapCam.GetComponent<TouchInputController>().enabled = true;
		minimapCam.GetComponent<MobileTouchCamera>().enabled = true;

		locationMark.anchoredPosition = Vector3.zero;
				
		handle_update = Util.RunCoroutine(Co_Update(), nameof(Co_Update));

		isBack = true;
	}

	public override void Back(int cnt = 1)
	{
		OnClick_Back();
	}

	protected override void SetMemberUI()
	{
		base.SetMemberUI();

		world = Single.Scene.GetSceneId() == SceneName.Scene_Land_Arz.ToString() ? GameWorld.ArzMetaWorld : GameWorld.BusanWorld;

		arzmeta = ArzMetaManager.Instance;
		minimapCam = arzmeta.transform.Search("MinimapCamera").GetComponent<Camera>();

		spawnPoint = arzmeta.transform.Search(Define.SpawnPoint);

		go_MapList = this.transform.Search(nameof(go_MapList));
		sview_MapList = this.transform.Search<ScrollRect>(nameof(sview_MapList));
		content_Region = sview_MapList.transform.Search(nameof(content_Region));
		content_Brand = sview_MapList.transform.Search(nameof(content_Brand));

		txtmp_Region = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Region), new MasterLocalData("map_infotype_landmark"));
		txtmp_Brand = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Brand), new MasterLocalData("map_infotype_building"));

		img_Arrow = GetUI_Img(nameof(img_Arrow));

		togplus_Region = GetUI<TogglePlus>(nameof(togplus_Region));
		togplus_Region.SetToggleOnAction(OnClick_Region);

		togplus_Brand = GetUI<TogglePlus>(nameof(togplus_Brand));
		togplus_Brand.SetToggleOnAction(OnClick_Brand);

		btn_Handle = GetUI_Button(nameof(btn_Handle), OnClick_Handle);

		btn_Back = GetUI_Button(nameof(btn_Back), OnClick_Back);
		//txtmp_Back = GetUI_TxtmpMasterLocalizing(nameof(txtmp_Back), new MasterLocalData("common_back"));

		var masterId = world == GameWorld.ArzMetaWorld ? "common_arzland" : "common_busanland";
		txtmp_world = GetUI_TxtmpMasterLocalizing(nameof(txtmp_world), new MasterLocalData(masterId));

		var rect = go_MapList.GetComponent<RectTransform>();
		hidePoint = new Vector2(rect.anchoredPosition.x - rect.rect.width, rect.anchoredPosition.y);
		showPoint = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);

		locationMark = this.transform.Search("go_LocationMark").GetComponent<RectTransform>();
		locationMark.GetComponent<CanvasGroup>().alpha = 0f;

		//stackCam = GameObject.Find(Define.STACKCAMERA).GetComponent<Camera>();
		//stackCam = SceneLogic.instance.stackCamera;
	}


	protected override void Start()
	{
		base.Start();

		players = GameObject.Find("Players");

		go_MapList.GetComponent<CanvasGroup>().alpha = 0f;
		go_MapList.GetComponent<CanvasGroup>().blocksRaycasts = false;

		int index = 0;
		var data = Single.MasterData.dataMapExposulInfo.GetDictionary_int();

		minimapTeleports = new MinimapTeleport[data.Count];

		foreach (var element in data)
		{
			minimapTeleports[index] = new MinimapTeleport();

			minimapTeleports[index].masterId = element.Value.id;
			minimapTeleports[index].name = element.Value.name;
			minimapTeleports[index].description = element.Value.description;
			minimapTeleports[index].image = element.Value.image;

			minimapTeleports[index].posX = element.Value.positionX * 0.01f;
			minimapTeleports[index].posY = element.Value.positionY * 0.01f;
			minimapTeleports[index].posZ = element.Value.positionZ * 0.01f;

			minimapTeleports[index].eulerY = element.Value.rotationY;
			minimapTeleports[index].mapInfoType = element.Value.mapInfoType;
			minimapTeleports[index].worldId = element.Value.landType;
			minimapTeleports[index].sort = element.Value.sort;

			index++;
		}
		
		minimapTeleports = minimapTeleports.OrderBy(teleport => teleport.sort).ToArray();

		MakeMinimapList();

		List<Camera> mainCameraStack = minimapCam.GetUniversalAdditionalCameraData().cameraStack;
		mainCameraStack.Clear();
		mainCameraStack.Add(SceneLogic.instance.stackCamera);
	}

	protected override IEnumerator<float> Co_OpenStartAct()
	{
		isBack = true;

		yield return Timing.WaitUntilTrue(() => Single.Scene.fadeOut);

		Single.Scene.FadeIn();

		togplus_Region.SetToggleIsOn(true);

		this.GetComponent<CanvasGroup>().blocksRaycasts = false;
		this.GetComponent<CanvasGroup>().alpha = 1f;

		ArzMetaManager.Instance.PhoneController.isPhone = false;
		ArzMetaManager.Instance.MinimapController.MinimapStart();

		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().Chat.GetComponent<CanvasGroup>(), 0f, 1.5f, 0f);

		MyPlayer.instance.SetThirdPersonView();
		MyPlayer.instance.TPSController.Camera.MainCam.enabled = false;
		MyPlayer.instance.TPSController.Camera.SetEaseSpeed(.75f);
		MyPlayer.instance.TPSController.MotionController.PlayAnimation("phone_takeIn", _isResetWeight: true);

		minimapCam.gameObject.SetActive(true);
		minimapCam.enabled = true;
		minimapCam.transform.localPosition = new Vector3(0f, 0f, 50f);

		touchInputController = minimapCam.GetComponent<TouchInputController>();
		mobileTouchCamera = minimapCam.GetComponent<MobileTouchCamera>();

		MyPlayer.instance.TPSController.Camera.SubCam.transform.SetParent(minimapCam.transform);
		MyPlayer.instance.TPSController.Camera.SubCam.transform.localPosition = Vector3.zero;
		MyPlayer.instance.TPSController.Camera.SubCam.transform.localRotation = Quaternion.identity;
		MyPlayer.instance.TPSController.Camera.SubCam.fieldOfView = minimapCam.fieldOfView;

		Util.ChangeLayerMask(MyPlayer.instance.gameObject, Define.Player);
	}

	protected override IEnumerator<float> Co_OpenEndAct()
	{
		yield return Timing.WaitForOneFrame;

		players.SetActive(false);

		isMinimapView = true;
		this.GetComponent<CanvasGroup>().blocksRaycasts = true;

		var canvasgroup = locationMark.GetComponent<CanvasGroup>();
		FadeUtils.FadeCanvasGroup(canvasgroup, 1f);
		locationMark.gameObject.SetActive(true);

		Util.RunCoroutine(Co_UpdateLocationMark(new Vector3(0f, 0f, 0f)), nameof(Co_UpdateLocationMark));

		MyPlayer.instance.SetEntirePlayerVisible(true);

		go_MapList.GetComponent<CanvasGroup>().alpha = 1f;
		go_MapList.GetComponent<CanvasGroup>().blocksRaycasts = true;

		Util.RunCoroutine(Co_ShowUI(true), nameof(Co_ShowUI));

		yield return Timing.WaitUntilTrue(() => this.GetComponent<CanvasGroup>().alpha >= 1f);
		yield return Timing.WaitForSeconds(.25f);

		isBack = false;
	}

	protected override IEnumerator<float> Co_SetCloseStartAct()
	{
		yield return Timing.WaitForOneFrame;

		MyPlayer.instance.EnableHeadDisplay(true);

		GetPanel<Panel_HUD>().Joystick.group_menu.SetActive(true);
		GetPanel<Panel_HUD>().transform.Search("btn_SwitchPerspective").gameObject.SetActive(true);
	}

	protected override IEnumerator<float> Co_SetCloseEndAct()
	{
		yield return Timing.WaitForOneFrame;

		FadeUtils.FadeCanvasGroup(GetPanel<Panel_HUD>().Chat.GetComponent<CanvasGroup>(), 1f, 1.5f, 0f);

		MyPlayer.instance.TPSController.Camera.SubCam.enabled = false;
		MyPlayer.instance.TPSController.Camera.MainCam.enabled = true;

		ArzMetaManager.Instance.MinimapController.MinimapEnd();

		spawnPoint.transform.localPosition = Vector3.zero;
		spawnPoint.transform.localRotation = Quaternion.identity;

		minimapCam.transform.localPosition = new Vector3(0f, 0f, 50f);

		OnClick_Region();

		Timing.KillCoroutines(nameof(Co_UpdateLocationMark));
	}

	private IEnumerator<float> Co_Update()
	{
		while (true)
		{
			yield return Timing.WaitUntilTrue(() => isMinimapView);

			if (mobileTouchCamera.IsDragging && !isDragging)
			{
				isDragging = true;
			}

			if (mobileTouchCamera.IsPinching && !isPinching)
			{
				isPinching = true;
			}

			if (IsTouchOverUI())
			{
				touchInputController.enabled = false;
			}

			if (!IsTouchOverUI())
			{
				touchInputController.enabled = true;

				Util.RunCoroutine(Co_EndInteraction());
			}

			yield return Timing.WaitForOneFrame;
		}
	}

	private IEnumerator<float> Co_EndInteraction()
	{
		yield return Timing.WaitForOneFrame;

		isDragging = false;
		isPinching = false;
	}

	#endregion

	public List<RectTransform> target = new List<RectTransform>();

	#region Core Methods

	bool isFocusing = false;

	public void FocusCameraOnPoint(MinimapTeleport _minimapTeleport)
	{
		if (isFocusing) return;

		Util.RunCoroutine(Co_FocusCameraOnPoint(_minimapTeleport), nameof(Co_FocusCameraOnPoint));

		isFocusing = true;
	}

	IEnumerator<float> Co_FocusCameraOnPoint(MinimapTeleport _minimapTeleport)
	{
		yield return Timing.WaitForOneFrame;

		if (SceneLogic.instance._stackPopups.Count > 0 && currentId != _minimapTeleport.masterId)
		{
			locationMark.gameObject.SetActive(false);
			var canvasgroup = locationMark.GetComponent<CanvasGroup>();
			canvasgroup.alpha = 0f;

			SceneLogic.instance.isUILock = false;

			SceneLogic.instance.PopPopup();

			yield return Timing.WaitForSeconds(.25f);
		}

		currentId = _minimapTeleport.masterId;

		bool isEnough = false;

		isDragging = false;
		isPinching = false;

		touchInputController.enabled = false;
		mobileTouchCamera.enabled = false;

		float lerpvalue = 0f;
		float lerpspeed = 1f;

		var current = minimapCam.transform.localPosition;
		var target = new Vector3(_minimapTeleport.focusPoint.x, (int)minimapCam.transform.localPosition.y, _minimapTeleport.focusPoint.z);

		while (Vector3.Distance(current, target) >= 0.001f)
		{
			current = Vector3.Lerp(current, target, lerpvalue += lerpspeed * Time.deltaTime);

			minimapCam.transform.localPosition = current;

			if (Vector3.Distance(current, target) <= 0.25f && !isEnough)
			{
				OpenPopup(_minimapTeleport);

				isEnough = true;
			}

			yield return Timing.WaitForOneFrame;
		}

		if (!isEnough) OpenPopup(_minimapTeleport);

		touchInputController.enabled = true;
		mobileTouchCamera.enabled = true;
	}


	#endregion



	#region Utils

	private void MakeMinimapList()
	{
		var worldId = (world == GameWorld.ArzMetaWorld) ? 1001 : 1002;
		var prefab = Resources.Load<GameObject>("Addressable/Prefab/Item/item_Location");

		for (int i = 0; i < minimapTeleports.Length; i++)
		{
			if (minimapTeleports[i].worldId != worldId) continue;

			minimapTeleports[i].focusPoint = new Vector3(minimapTeleports[i].posX, minimapTeleports[i].posY, minimapTeleports[i].posZ + 45f);

			var parent = minimapTeleports[i].mapInfoType == 1 ? content_Region : content_Brand;
			var location = Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);

			location.transform.localPosition = Vector3.zero;
			location.transform.localRotation = Quaternion.identity;
			location.name = minimapTeleports[i].name;

			location.GetComponent<Item_Location>().Init(minimapTeleports[i]);
		}

		content_Brand.gameObject.SetActive(false);
	}

	public void SetTeleportPoint(Vector3 _position, Vector3 _euler)
	{
		spawnPoint.transform.localPosition = _position;
		spawnPoint.transform.localEulerAngles = _euler;
	}



	public void Teleport() => Util.RunCoroutine(Co_Teleport(), nameof(Co_Teleport));

	IEnumerator<float> Co_Teleport()
	{
		players.SetActive(true);

		MyPlayer.instance.Teleport_New(spawnPoint, false);

		yield return Timing.WaitForSeconds(.35f);

		yield return Timing.WaitUntilTrue(() => MyPlayer.instance != null);
		
		players.SetActive(true);

		arzmeta.MinimapController.GoToPlayer();

		minimapCam.enabled = false;
		minimapCam.GetComponent<TouchInputController>().enabled = false;
		minimapCam.GetComponent<MobileTouchCamera>().enabled = false;


		Timing.KillCoroutines(nameof(Co_UpdateLocationMark));

		locationMark.anchoredPosition = Vector3.zero;
		locationMark.gameObject.SetActive(false);
		locationMark.GetComponent<CanvasGroup>().alpha = 0f;
		

		Util.RunCoroutine(Co_ShowUI(false), nameof(Co_ShowUI));

		isMinimapView = false;

		if (SceneLogic.instance._stackPopups.Count > 0)
		{
			SceneLogic.instance.PopPopup();
		}

		Util.RunCoroutine(Co_Back());
	}

	private bool IsTouchOverUI()
	{
		if (Input.touchCount > 0)
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return true;
			}
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				if (EventSystem.current.IsPointerOverGameObject())
				{
					return true;
				}
			}
		}

		return false;
	}

	private void OpenPopup(MinimapTeleport _minimapTeleport)
	{
		var popup = GetPopup<Popup_Map>();

		popup.Init(_minimapTeleport);

		PushPopup<Popup_Map>();

		var position = new Vector3(_minimapTeleport.posX, _minimapTeleport.posY, _minimapTeleport.posZ);

		Util.RunCoroutine(Co_UpdateLocationMark(position), nameof(Co_UpdateLocationMark));
	}

	IEnumerator<float> Co_UpdateLocationMark(Vector3 _position)
	{
		var canvasgroup = locationMark.GetComponent<CanvasGroup>();
		canvasgroup.alpha = 0f;
		FadeUtils.FadeCanvasGroup(canvasgroup, 1f);

		locationMark.gameObject.SetActive(true);

		var position = new Vector3(_position.x, _position.y, _position.z);


		yield return Timing.WaitUntilTrue(() => canvasGroup.alpha >= 1f);

		isFocusing = false;

		while (true)
		{
			locationMark.anchoredPosition =
			new Vector2(
				minimapCam.WorldToScreenPoint(position).x - (Screen.width * .5f),
				minimapCam.WorldToScreenPoint(position).y - (Screen.height * .5f)
				) / MasterCanvas.instance.GetComponent<Canvas>().scaleFactor;

			yield return Timing.WaitForOneFrame;
		}
	}

	#endregion



	#region Button Events

	private void OnClick_Region()
	{
		bool _isEnable = true;

		content_Region.gameObject.SetActive(_isEnable);
		content_Brand.gameObject.SetActive(!_isEnable);
	}

	private void OnClick_Brand()
	{
		bool _isEnable = true;

		content_Region.gameObject.SetActive(!_isEnable);
		content_Brand.gameObject.SetActive(_isEnable);
	}

	private void OnClick_Handle()
	{
		Util.RunCoroutine(Co_ShowUI(!isHandleOut), nameof(Co_ShowUI));

		if (isHandleOut)
		{
			if (SceneLogic.instance._stackPopups.Count > 0)
			{
				SceneLogic.instance.PopPopup();
			}
		}
	}

	IEnumerator<float> Co_ShowUI(bool _show)
	{
		float lerpvalue = 0f;
		float lerpspeed = 1f;
		float dir = _show ? 1f : -1f;

		var target = _show ? showPoint : hidePoint;

		var current = go_MapList.GetComponent<RectTransform>().anchoredPosition;

		while (Vector2.Distance(current, target) > 0.001f)
		{
			current = Vector2.Lerp(current, target, lerpvalue += lerpspeed * Time.deltaTime);

			go_MapList.GetComponent<RectTransform>().anchoredPosition = current;

			yield return Timing.WaitForOneFrame;
		}

		img_Arrow.GetComponent<RectTransform>().localScale = Vector3.one * dir;

		isHandleOut = _show;
	}

	public void OnClick_Back()
	{
		if (isBack) return;

		Util.RunCoroutine(Co_ShowUI(false), nameof(Co_ShowUI));

		Util.RunCoroutine(Co_OnClick_Back(), nameof(Co_OnClick_Back));

		isBack = true;
	}

	IEnumerator<float> Co_OnClick_Back()
	{
		Single.Scene.FadeOut();
		Single.Scene.SetDimOn();

		players.SetActive(true);

		yield return Timing.WaitUntilTrue(() => Single.Scene.fadeOut);
		yield return Timing.WaitForSeconds(.5f);

		Timing.KillCoroutines(nameof(Co_UpdateLocationMark));

		locationMark.anchoredPosition = Vector3.zero;
		locationMark.gameObject.SetActive(false);
		locationMark.GetComponent<CanvasGroup>().alpha = 0f;

		isMinimapView = false;

		if (SceneLogic.instance._stackPopups.Count > 0)
		{
			SceneLogic.instance.PopPopup();
		}

		Util.RunCoroutine(Co_ShowUI(false), nameof(Co_ShowUI));

		Util.RunCoroutine(Co_Back());

		MyPlayer.instance.TPSController.Camera.SubCam.transform.SetParent(MyPlayer.instance.TPSController.Camera.MainCam.transform);
		MyPlayer.instance.TPSController.Camera.SubCam.transform.localPosition = Vector3.zero;
		MyPlayer.instance.TPSController.Camera.SubCam.transform.localRotation = Quaternion.identity;
		MyPlayer.instance.TPSController.Camera.SubCam.transform.localScale = Vector3.one;

		MyPlayer.instance.TPSController.Camera.SubCam.enabled = false;
		MyPlayer.instance.TPSController.Camera.MainCam.enabled = true;

		minimapCam.enabled = false;
		minimapCam.GetComponent<TouchInputController>().enabled = false;
		minimapCam.GetComponent<MobileTouchCamera>().enabled = false;

		GetPanel<Panel_HUD>().Joystick.virtualJoystick.enabled = true;

		yield return Timing.WaitUntilTrue(() => MyPlayer.instance.TPSController.Camera.MainCam.enabled);

		MyPlayer.instance.EnableController(true);

        Single.Scene.FadeIn();

        Single.Scene.SetDimOff(endCallback: FindObjectOfType<ShortcutManager>().ExitMinimap);


    }

    IEnumerator<float> Co_Back()
    {
		yield return Timing.WaitUntilTrue(() => !isHandleOut);

		FadeUtils.FadeCanvasGroup(this.GetComponent<CanvasGroup>(), 0f, 1.5f, 0f);

		yield return Timing.WaitUntilTrue(() => MyPlayer.instance.TPSController.Camera.MainCam.enabled);

		GetPanel<Panel_HUD>().Show(true);
		GetPanel<Panel_HUD>().Joystick.ShowItem(true);

		SceneLogic.instance.isUILock = false;

		SceneLogic.instance.PopPanel();

		Timing.KillCoroutines(handle_update);
	}

	public Vector3 GetFocusPoint(int _masterId)
	{
		var position = Vector3.zero;

		switch (_masterId)
		{
			case 10001:
				position = new Vector3(0f, 0f, 50f);
				break;
			case 10002:
				position = new Vector3(96f, 0f, 33f);
				break;
			case 10003:
				position = new Vector3(169, 0f, 0f);
				break;
			case 10004:
				position = new Vector3(215, 0f, -2f);
				break;
			case 10005:
				position = new Vector3(214f, 0f, -13f);
				break;
			case 10006:
				position = new Vector3(214f, 0f, 77f);
				break;
			case 10007:
				position = new Vector3(-156.56f, 0f, 11f);
				break;
			case 10008:
				position = new Vector3(-7f, 0f, 43f);
				break;
			case 10009:
				position = new Vector3(6f, 0f, 99f);
				break;
			case 10010:
				position = new Vector3(-120f, 0f, 150f);
				break;
		}

		return position;
	}

	#endregion
}