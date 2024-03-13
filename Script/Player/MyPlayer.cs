using System;
using FrameWork.Network;
using FrameWork.UI;
using UnityEngine;
using StarterAssets;
using Vector3 = UnityEngine.Vector3;
using MEC;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.InputSystem;
using TMPro;

public enum PlayerType { Single, Multi, Observer, }

public class MyPlayer : MonoBehaviour
{
	#region 맴버변수

	public static MyPlayer instance;

	[SerializeField] PlayerType playerType;

	SerializableDictionary<string, GameObject> childrens;
	SerializableDictionary<string, MonoBehaviour> controllers;

	CharacterController characterController;
	ThirdPersonController thirdPersonController;
	AvatarPartsController avatarPartsController;

	NetworkObserver networkObserver;

	PlayerSound playerSound;
	HUDParent hudParent;
	UIEventChecker eventChecker;

	[HideInInspector] public string clientId;

	bool isTeleport = false;
	[HideInInspector] public bool isFirstPersonView = false;

	Transform CameraParent;
	Transform PlayerParent;
	Transform Shadow;
	Transform HUDParent;

	public Action changeNickName;

	[HideInInspector] public float refreshDelay = 0.075f;
	[HideInInspector] public CameraView cameraView = CameraView.Back;

	#endregion


	#region 프로퍼티

	public SerializableDictionary<string, GameObject> Childrens { get => childrens; }
	public SerializableDictionary<string, MonoBehaviour> Controllers { get => controllers; }
	public ThirdPersonController TPSController { get => thirdPersonController; set => thirdPersonController = value; }
	public AvatarPartsController PartsController { get => avatarPartsController; set => avatarPartsController = value; }
	public NetworkObserver NetworkObserver { get => networkObserver; }
	public PlayerSound PlayerSound { get => playerSound; }
	public HUDParent HudParent { get => hudParent; }
	public UIEventChecker EventChecker { get => eventChecker; set => eventChecker = value; }
	public bool IsTeleport { get => isTeleport; }
	public string MySessionID { get => networkObserver.clientId; }
	public CharacterController CharacterController { get => characterController; }

	#endregion


	#region 초기화

	private void Awake()
	{
		GetComponents();
		teleportFadeSpeed = 2f;
	}

	private void Start()
	{
		SetComponents();

		this.gameObject.AddComponent<ShortcutManager>();
	}

	private void OnValidate()
	{

		CameraParent = transform.Search(nameof(CameraParent));
		HUDParent = transform.Search(nameof(HUDParent));
		PlayerParent = transform.Search(nameof(PlayerParent));
		Shadow = transform.Search(nameof(Shadow));

		if (!Application.isPlaying) return;

		GetComponents();
	}

	#region 컴포넌트 캐싱

	private void GetComponents()
	{
		switch (playerType)
		{
			case PlayerType.Single:
				GetSinglePlayerComponents();
				break;
			case PlayerType.Multi:
				GetMultiPlayerComponents();
				break;
			case PlayerType.Observer:
				GetObserverPlayerComponents();
				break;
		}
	}



	/// <summary>
	/// 싱글플레이
	/// </summary>
	private void GetSinglePlayerComponents()
	{
		CacheChildrens();
		CacheControllers();

		GetComponent<AvatarPartsController>().SetAvatarParts(LocalPlayerData.AvatarInfo);

		var hud = FindObjectOfType<Panel_HUD>(true);
		hud.dontSetActiveFalse = true;
		hud.gameObject.SetActive(true);
	}

	/// <summary>
	/// 멀티플레이
	/// </summary>
	private void GetMultiPlayerComponents()
	{
		CacheChildrens();
		CacheControllers();

		var hud = FindObjectOfType<Panel_HUD>(true);
		eventChecker = hud.GetComponent<UIEventChecker>();
		networkObserver = GetComponentInChildren<NetworkObserver>();
	}

	/// <summary>
	/// 옵저버
	/// </summary>
	private void GetObserverPlayerComponents()
	{
		CacheChildrens();
		CacheControllers();

		AddController(thirdPersonController = GetComponent<ThirdPersonController>());

		eventChecker = FindObjectOfType<UIEventChecker>();
		networkObserver = GetComponentInChildren<NetworkObserver>();
	}

	private void CacheChildrens()
	{
		childrens = new SerializableDictionary<string, GameObject>();

		Transform[] _childrens = gameObject.GetComponentsInChildren<Transform>();

		for (int i = 0; i < _childrens.Length; i++)
		{
			if (childrens.ContainsKey(_childrens[i].name))
			{
				_childrens[i].name += i;
			}

			childrens.Add(_childrens[i].name, _childrens[i].gameObject);
		}
	}

	private void CacheControllers()
	{
		controllers = new SerializableDictionary<string, MonoBehaviour>();
		characterController = GetComponent<CharacterController>();
		AddController(thirdPersonController = GetComponent<ThirdPersonController>());
		AddController(avatarPartsController = GetComponent<AvatarPartsController>());
		playerSound = GetComponentInChildren<PlayerSound>();
		hudParent = GetComponentInChildren<HUDParent>();
	}

	public void AddController<T>(T _controller) where T : MonoBehaviour
	{
		if(_controller == null)
        {
			return;
        }
		string name = _controller.GetType().Name;

		if (!controllers.ContainsKey(name))
		{
			controllers.Add(name, _controller);
		}
	}

	public void RemoveController(string _name)
	{
		if (controllers.ContainsKey(_name))
		{
			Destroy(controllers[_name]);
			controllers.Remove(_name);
		}
	}

	public T GetController<T>(string _name) where T : MonoBehaviour
	{
		return controllers[_name] as T;
	}

	#endregion

	#region 컴포넌트 셋팅

	private void SetComponents()
	{
		switch (playerType)
		{
			case PlayerType.Single:
				SetSinglePlayerComponents();
				break;
			case PlayerType.Multi:
				SetMultiplayerComponents();
				break;
			case PlayerType.Observer:
				SetObserverPlayerComponents();
				break;
		}
    }

	private void SetSinglePlayerComponents()
	{
		instance = this;
		thirdPersonController.Initialize();
		HudParent.SetNickName(LocalPlayerData.NickName);

		EnableInput(true);
	}

	private void SetMultiplayerComponents()
	{
		if (networkObserver.isMine)
		{
			Util.ChangeLayerMask(this.gameObject, Cons.Player);

			instance = this;
			thirdPersonController.Initialize();
			clientId = this.GetComponent<NetworkTransform>().clientId;
			
			EnableInput(true);
		}

		else
		{
			Util.ChangeLayerMask(this.gameObject, Cons.OtherPlayer);
			gameObject.AddComponent<Interaction_OtherPlayer>();

			this.gameObject.tag = Cons.OtherPlayer;
			this.playerSound.enabled = false;

			Destroy(childrens[Cons.CAMERAPARENT]);
			Destroy(thirdPersonController.StarterInputs);
			Destroy(thirdPersonController.PlayerInput);
			Destroy(thirdPersonController);
			Destroy(this);
		}
	}

	private void SetObserverPlayerComponents()
	{
		Util.ChangeLayerMask(this.gameObject, Cons.Player);

		instance = this;
		thirdPersonController.Initialize();

		EnableInput(true);
	}

    #endregion

    #endregion


    #region 텔레포트
    public bool IsTeleporting()
    {
		return handle_teleport.IsRunning;
	}
	public CoroutineHandle handle_teleport;

	[HideInInspector] public float teleportFadeSpeed = 1f;

	public void Teleport(Transform _transform, Action _endAction = null, bool _resetRotation = false, bool _isSpawned = false, bool _useEffect = true)
	{
		Teleport(_transform.position, _transform.eulerAngles, _endAction, _resetRotation, _isSpawned, _useEffect);
	}

	public void Teleport(Vector3 _position, Vector3 _eulerAngle, Action _endAction = null, bool _resetRotation = false, bool _isSpawned = false, bool _useEffect = true)
	{
		Util.RunCoroutine(Co_Teleport(_position, _eulerAngle, _endAction, _resetRotation, _isSpawned, _useEffect), "Teleport" + this.GetHashCode(), Util.SceneCoroutine.Player);
	}

	private IEnumerator<float> Co_Teleport(Vector3 _position, Vector3 _eulerAngles, Action _endAction = null, bool _resetRotation = false, bool isSpawned = false, bool _useEffect = true)
	{
		isTeleport = true;

		yield return Timing.WaitUntilTrue(() => thirdPersonController);

		if (!isSpawned) Util.CreateEffect(Cons.EF_summon, this.transform.position);
		if (_resetRotation) thirdPersonController.Camera.cameraReset = true;

		yield return Timing.WaitForSeconds(0.01f);

		thirdPersonController.enabled = false;
		thirdPersonController.StarterInputs.enabled = false;

		thirdPersonController.Camera.VirtualCam.PreviousStateIsValid = false;
		this.transform.localPosition = _position;
		this.transform.localEulerAngles = _eulerAngles;

		yield return Timing.WaitForSeconds(0.01f);

		thirdPersonController.enabled = true;
		thirdPersonController.StarterInputs.enabled = true;

		yield return Timing.WaitForSeconds(0.01f);

		this.transform.position += Vector3.zero;

		thirdPersonController.SetAdditionalMove(Vector3.zero, 0);

		if (_resetRotation) thirdPersonController.Camera.cameraReset = false;

		if (_useEffect) Util.CreateEffect(Cons.EF_summon, this.transform.position);

		isTeleport = false;

		_endAction?.Invoke();
	}




	public void Teleport_New(Transform _transform, bool isFade = true)
	{
		if (handle_teleport.IsRunning) return;

		handle_teleport = Timing.RunCoroutine(Co_Teleport(_transform.position, _transform.rotation, isFade));
	}

	public void Teleport_New(Vector3 _position, Quaternion _rotation, bool isFade = true)
	{
		if (handle_teleport.IsRunning) return;

		handle_teleport = Timing.RunCoroutine(Co_Teleport(_position, _rotation, isFade));
	}

	private IEnumerator<float> Co_Teleport(Vector3 _position, Quaternion _rotation, bool isFade = true)
	{
		if (isFade)
		{
			thirdPersonController.VolumeController.SetBloom(20f, 1f, 2f);

			Single.Scene.SetFadeColor(Color.white);

			Single.Scene.FadeOut(teleportFadeSpeed);

			yield return Timing.WaitUntilTrue(() => Single.Scene.fadeOut);
		}

		var networkHandler = FindObjectOfType<NetworkHandler>();
		var camera = CreateTeleportCamera(_position, _rotation);

		networkHandler.RemoveObject();



		yield return Timing.WaitUntilTrue(() => networkHandler.isRemoved);

		var playerData = new PlayerData
		{
			position = _position,
			eulerAngle = _rotation.eulerAngles,
			cameraView = cameraView,
			isRandomSpawn = false
		};

		var player = new NetworkPlayer(playerData);



		yield return Timing.WaitUntilTrue(() => instance != null);

		Util.CreateEffect(Cons.EF_summon, _position);



		yield return Timing.WaitForSeconds(refreshDelay);

		Destroy(camera);

		if (isFade)
		{
			TPSController.VolumeController.SetBloom(.3f, .7f, 5f);

			Single.Scene.FadeIn(teleportFadeSpeed, Single.Scene.ResetFadeColor);
		}

		SceneLogic.instance.GetPanel<Panel_HUD>().Refresh();

		if (ArzMetaManager.Instance != null)
		{
			ArzMetaManager.Instance.PhoneController.Refresh();
		};

		FindObjectOfType<MobileTouchInteracter>().Refresh();

		LocalPlayerData.Method.handler_Teleported?.Invoke();

	}


    public GameObject CreateTeleportCamera(Vector3 _position, Quaternion _rotation)
	{
		DeleteReflection();

		var mainCamera = thirdPersonController.Camera.MainCamera;
		var brain = thirdPersonController.Camera.Brain;
		brain.enabled = false;

		var position = mainCamera.transform.position + _position;
		var rotation = mainCamera.transform.rotation * _rotation;

		return Instantiate(mainCamera, position, rotation);
	}

	void DeleteReflection()
	{
		GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

		foreach (var rootObject in rootObjects)
		{
			if (rootObject.name.Contains("Reflection Camera for MainCamera"))
			{
				Destroy(rootObject);
			}
		}
	}

	#endregion


	#region 카메라

	//[InspectorButton("1인칭 전환")]
	public void SetFirstPersonView()
	{
		isFirstPersonView = true;

		var body = thirdPersonController.Camera.VirtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

		if (body is Cinemachine3rdPersonFollow follow)
		{
			follow.CameraDistance = 1;
		}

		SetMyPlayerVisible(false);

		thirdPersonController.Camera.ResetCameraBack();
	}

	//[InspectorButton("3인칭 전환")]
	public void SetThirdPersonView()
	{
		isFirstPersonView = false;

		var body = thirdPersonController.Camera.VirtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

		if (body is Cinemachine3rdPersonFollow follow)
		{
			follow.CameraDistance = 5;
		}

		SetMyPlayerVisible(true);

		thirdPersonController.Camera.ResetCameraBack();
	}

	#endregion


	#region Input 활성화/비활성화

	/// <summary>
	/// 키보드 입력 제어
	/// </summary>
	/// <param name="_enable"></param>
	public void EnableInput(bool _enable)
	{
		this.GetComponent<ThirdPersonController>().PlayerInput.enabled = _enable;
	}

	/// <summary>
	/// 조이스틱(이동) 입력 제어
	/// </summary>
	/// <param name="_enable"></param>
	public void EnableJoystick(bool _enable)
	{
		SceneLogic.instance.GetPanel<Panel_HUD>().Joystick.virtualJoystick.enabled = _enable;
	}


	/// <summary>
	/// HUD제어
	/// </summary>
	/// <param name="_show"></param>a
	/// <param name="_lerpspeed"></param>
	/// <param name="startAct"></param>
	/// <param name="endAct"></param>
	public void EnableController(bool _show, float _lerpspeed = 1f, Action startAct = null, Action endAct = null)
	{
		float target = _show ? 1f : 0f;
		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();

		thirdPersonController.Animation.Blend = 0f;
		//thirdPersonController.PlayerInput.enabled = _show;
		EnableInput(_show);
		thirdPersonController.Camera.LeanPinch.enabled = _show;

		FadeUtils.FadeCanvasGroup(hud.canvasGroup, target, _lerpspeed, 0f, startAct, endAct);

		hud.Touch.virtualTouch.enabled = _show;
	}

	#endregion


	#region HUD 활성화/비활성화

	public void EnableHUD(bool _show, float _lerpspeed = 1f, float _delay = 0f, bool _instant = false, Action startAct = null, Action endAct = null)
	{
		float target = _show ? 1f : 0f;

		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();

		hud.Touch.eventChecker.enabled = _show;
		hud.Touch.virtualTouch.enabled = _show;
		hud.Joystick.virtualJoystick.enabled = _show;

		thirdPersonController.Camera.LeanPinch.enabled = _show;

		if (_instant)
		{
			hud.canvasGroup.alpha = target;
			return;
		}

		FadeUtils.FadeCanvasGroup(hud.canvasGroup, target, _lerpspeed, _delay, startAct, endAct);
	}

	public void EnableHUDAlphaOnly(bool _show, float _lerpspeed = 1f, bool _instant = false, Action startAct = null, Action endAct = null)
	{
		float target = _show ? 1f : 0f;
		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();

		if (_instant)
		{
			hud.canvasGroup.alpha = target;
			return;
		}

		FadeUtils.FadeCanvasGroupAlphaOnly(hud.canvasGroup, target);
	}

	public bool IsHUDActive()
	{
		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();

		return hud.canvasGroup.alpha == 1f ? true : false;
	}

	#endregion


	#region HUD 비활성화 후 카메라 회전만 활성화

	public void EnableOnlyTouchInput(bool _show, float _lerpspeed = 1f, float _delay = 0f, bool _instant = false, Action startAct = null, Action endAct = null)
	{
		float target = _show ? 0f : 1f;
		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();

		if (_instant)
		{
			hud.canvasGroup.alpha = target;
			hud.canvasGroup.blocksRaycasts = true;
			hud.Joystick.zone.SetActive(!_show);

			return;
		}

		FadeUtils.FadeCanvasGroupAlphaOnly(hud.canvasGroup, target, _lerpspeed, _delay,
			() =>
			{
				if (_show) hud.Joystick.zone.SetActive(false);
				else hud.Joystick.zone.SetActive(true);
			},
			() =>
			{
				hud.canvasGroup.blocksRaycasts = true;
			}
		);
	}

	#endregion


	#region  Visible 활성화/비활성화

	public void SetMyPlayerVisible(bool _show, bool _playEffect = false)
	{
		if (_show)
		{
			Util.ChangeLayerMask(Childrens[Cons.AVATARPARTS], Define.Player);
			Util.ChangeLayerMask(Childrens[Cons.HUDPARENT], Define.Player);
		}

		else
		{
			Util.ChangeLayerMask(Childrens[Cons.AVATARPARTS], Define.Invisible);
			Util.ChangeLayerMask(Childrens[Cons.HUDPARENT], Define.Invisible);
		}

		if (_playEffect)
		{
			Util.Pool(Define.EF_Puff, this.transform.position, Quaternion.identity, Vector3.one * 1.5f);
		}
	}

	public void SetOtherPlayerVisible(string _sessionID, bool _show, bool _playEffect = false)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].clientId == MySessionID || players[i].clientId != _sessionID) continue;

			if (_show) Util.ChangeLayerMask(players[i].gameObject, Define.OtherPlayer);
			else Util.ChangeLayerMask(players[i].gameObject, Define.Invisible);

			if (_playEffect)
			{
				Util.Pool(Define.EF_Puff, players[i].transform, Vector3.up, Quaternion.identity, Vector3.one * 1.5f);
			}
		}
	}

	public void SetOtherPlayersVisible(bool _show, bool _playEffect = false)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].clientId == MySessionID) continue;

			if (_show) Util.ChangeLayerMask(players[i].gameObject, Define.OtherPlayer);
			else Util.ChangeLayerMask(players[i].gameObject, Define.Invisible);

			if (_playEffect)
			{
				Util.Pool(Define.EF_Puff, players[i].transform, Vector3.up, Quaternion.identity, Vector3.one * 1.5f);
			}
		}
	}

	public void SetEntirePlayerVisible(bool _show, bool _playEffect = false)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (_show) Util.ChangeLayerMask(players[i].gameObject, Define.OtherPlayer);
			else Util.ChangeLayerMask(players[i].gameObject, Define.Invisible);

			if (_playEffect)
			{
				Util.Pool(Define.EF_Puff, players[i].transform, Vector3.up, Quaternion.identity, Vector3.one * 1.5f);
			}
		}

		if (_show) Util.ChangeLayerMask(this.gameObject, Define.Player);
	}

	public void SetEntirePlayerVisibleExceptMyPlayer(bool _show, bool _playEffect = false)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].clientId == networkObserver.clientId) continue;

			if (_show) Util.ChangeLayerMask(players[i].gameObject, Define.OtherPlayer);
			else Util.ChangeLayerMask(players[i].gameObject, Define.Invisible);

			if (_playEffect)
			{
				Util.Pool(Define.EF_Puff, players[i].transform, Vector3.up, Quaternion.identity, Vector3.one * 1.5f);
			}
		}
	}

	public void SetOtherPlayersLayerMask(string _layermask)
	{
		NetworkTransform[] players = FindObjectsOfType<NetworkTransform>();

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].clientId == MySessionID) continue;

			Util.ChangeLayerMask(players[i].gameObject, _layermask);
		}
	}

	#endregion


	#region 닉네임 활성화/비활성화

	public void EnableHeadDisplay(bool _show, float _lerpspeed = 1f, bool _instant = false, Action startAct = null, Action endAct = null)
	{
		float target = _show ? 1f : 0f;

		if (_instant)
		{
			HudParent.GetComponent<CanvasGroup>().alpha = target;
			return;
		}

		FadeUtils.FadeCanvasGroupAlphaOnly(HudParent.GetComponent<CanvasGroup>(), target, _lerpspeed, 0f, startAct, endAct);
	}

	#endregion


	#region 싱글플레이/멀티플레이

	public bool IsOfflinePlayer()
	{
		return playerType == PlayerType.Single;
	}

	#endregion


	#region 관전자모드
	public void SetInspectorMode(bool enable)
	{
		if (enable)
		{
			SetFirstPersonView();

			thirdPersonController.Movement.gravity = 0;
			thirdPersonController.Movement.fallSpeed = 0;
			thirdPersonController.Movement.joystickLock = false;
			thirdPersonController.Camera.rotateMovement = true;
			PlayerParent.gameObject.SetActive(false);
			Shadow.gameObject.SetActive(false);
			HUDParent.gameObject.SetActive(false);
		}
		else
		{
			thirdPersonController.Movement.gravity = -15;
			thirdPersonController.Movement.fallSpeed = 0;
			thirdPersonController.Movement.joystickLock = false;
			thirdPersonController.Camera.rotateMovement = false;
			PlayerParent.gameObject.SetActive(true);
			Shadow.gameObject.SetActive(true);
			HUDParent.gameObject.SetActive(true);

			SetThirdPersonView();
		}

		MasterCanvas.instance.SetControllerUI(true);
	}

	public void SetInspectorMode(bool enable, Transform tr)
	{
		if (enable)
		{
			SetFirstPersonView();

			thirdPersonController.Movement.gravity = 0;
			thirdPersonController.Movement.fallSpeed = 0;
			thirdPersonController.Movement.joystickLock = true;
			thirdPersonController.Camera.rotateMovement = true;
			PlayerParent.gameObject.SetActive(false);
			Shadow.gameObject.SetActive(false);
			HUDParent.gameObject.SetActive(false);

			this.transform.SetPositionAndRotation(tr.position, tr.rotation);
		}
		else
		{
			thirdPersonController.Movement.gravity = -15;
			thirdPersonController.Movement.fallSpeed = 0;
			thirdPersonController.Movement.joystickLock = false;
			thirdPersonController.Camera.rotateMovement = false;
			PlayerParent.gameObject.SetActive(true);
			Shadow.gameObject.SetActive(true);
			HUDParent.gameObject.SetActive(true);

			SetThirdPersonView();
		}

		MasterCanvas.instance.SetControllerUI(!enable);
	}

	//[InspectorButton("관전자 모드 활성화")]
	public void EnableInspectorMode()
	{
		SetInspectorMode(true);
	}

	//[InspectorButton("관전자 모드 비활성화")]
	public void DisableInspectorMode()
	{
		SetInspectorMode(false);
	}

    #endregion


}