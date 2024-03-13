using UnityEngine;
using Cinemachine;
using Lean.Touch;
using FrameWork.UI;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using MEC;

[System.Serializable]
public class TPSCamera
{
	#region Members

	[Space(5f), Tooltip("Default")]
	[Range(0f, 25f)] public float speed = 11f;
	[Range(0f, 1f)] public const float threshold = 0.01f;

	[Space(10f), Tooltip("Control")]
	public bool cameraLock = false;
	public bool cameraReset = false;
	public bool rotateMovement = false;

	[Space(10f), Tooltip("Sensitivity")]
	[Range(0f, 1f)] public float yawSensitivity = 0.215f;
	[Range(0f, 1f)] public float pitchSensitivity = 0.215f;

	[SerializeField] public float yaw = 0f;
	[SerializeField] public float pitch = 0f;

	[HideInInspector] public Vector2 yawMinMax = new Vector2(float.MinValue, float.MaxValue);
	[HideInInspector] public Vector2 pitchMinMax = new Vector2(-30f, 70f);

	[HideInInspector] public float currentPitch;
	[HideInInspector] public float currentYaw;

	[HideInInspector] public float distanceOrigin = 0f;
	[HideInInspector] public float distanceCurrent = 0f;
	[HideInInspector] public Vector3 parentOriginPosition;
	[HideInInspector] public Quaternion parentOriginRotation;

	Camera mainCam;
	GameObject mainCamera;
	Camera subCam;
	GameObject subCamera;
	Camera stackCam;

	GameObject followTarget;
	GameObject parent;

	CinemachineBrain brain;
	CinemachineVirtualCamera virtualCam;
	Cinemachine3rdPersonFollow followCam;
	CinemachineCollider virtualCamCollider;

	CameraShake cameraShake;
	LeanPinchCameraCustom_Cinemachine leanPinch;

	public Camera MainCam { get => mainCam; set => mainCam = value; }
	public GameObject MainCamera { get => mainCamera; set => mainCamera = value; }
	public Camera SubCam { get => subCam; set => subCam = value; }
	public GameObject SubCamera { get => subCamera; }
	public Camera StackCam { get => stackCam; }

	public GameObject FollowTarget { get => followTarget; set => followTarget = value; }
	public float Threshold { get => threshold; }

	public CinemachineBrain Brain { get => brain; set => brain = value; }
	public CinemachineVirtualCamera VirtualCam { get => virtualCam; set => virtualCam = value; }
	public Cinemachine3rdPersonFollow FollowCam { get => followCam; set => followCam = value; }
	public CinemachineCollider VirtualCamCollider { get => virtualCamCollider; set => virtualCamCollider = value; }

	public CameraShake CameraShake { get => cameraShake; }
	public LeanPinchCameraCustom_Cinemachine LeanPinch { get => leanPinch; set => leanPinch = value; }

	#endregion

	#region Initialize

	public void Awake()
	{
		followTarget = MyPlayer.instance.Childrens[Cons.PLAYER_CAMERAROOT];

		mainCam = MyPlayer.instance.Childrens[Cons.MAINCAMERA].GetComponent<Camera>();
		mainCamera = mainCam.gameObject;

		parent = MyPlayer.instance.Childrens[Cons.CAMERAPARENT];
		parentOriginPosition = parent.transform.localPosition;
		parentOriginRotation = parent.transform.localRotation;

		brain = parent.GetComponentInChildren<CinemachineBrain>();
		virtualCam = parent.GetComponentInChildren<CinemachineVirtualCamera>();
		followCam = virtualCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
		virtualCamCollider = virtualCam.GetComponent<CinemachineCollider>();

		Panel_HUD hud = SceneLogic.instance.GetPanel<Panel_HUD>();
		leanPinch = Util.Search<LeanPinchCameraCustom_Cinemachine>(hud.gameObject, Cons.LeanPinchCamera);
		leanPinch.virtualCam = virtualCam;

		parent.AddComponent<CameraNoise>();
		parent.AddComponent<CameraShake>();
		this.cameraShake = parent.GetComponent<CameraShake>();

		CommonUtils.CreateCam(Define.SubCamera, ref subCamera);
		CommonUtils.InitCam(mainCam, ref subCamera);

		subCam = subCamera.GetComponent<Camera>();
		subCam.nearClipPlane = 0.5f;
		subCam.farClipPlane = 20000f;
		subCam.cullingMask = mainCam.cullingMask;

		AddStackCamera();
	}

	public void Start()
	{
		distanceOrigin = GetCameraDistnace();
		distanceCurrent = GetCameraDistnace();
	}

	public void AddStackCamera()
	{
		//stackCam = GameObject.Find(Define.STACKCAMERA).GetComponent<Camera>();
		stackCam = SceneLogic.instance.stackCamera;
		List<Camera> mainCameraStack = mainCam.GetUniversalAdditionalCameraData().cameraStack;
		mainCameraStack.Clear();
		mainCameraStack.Add(stackCam);

		List<Camera> subCameraStack = subCam.GetUniversalAdditionalCameraData().cameraStack;
		subCameraStack.Clear();
		subCameraStack.Add(stackCam);
	}

	#endregion

	public float GetSensitivity()
	{
		if (mainCam != null)
		{
			// adjust sensitivity by FOV?
			if (mainCam.orthographic == false)
			{
				return mainCam.fieldOfView / 90.0f;
			}
		}

		return 1.0f;
	}

	public float GetDampen(float damping, float elapsed)
	{
		if (damping < 0.0f)
		{
			return 1.0f;
		}

#if UNITY_EDITOR
		if (Application.isPlaying == false)
		{
			return 1.0f;
		}
#endif

		return 1.0f - Mathf.Exp(-damping * elapsed);
	}

	public void SetYawPitchInstant(float _yaw, float _pitch)
	{
		yaw = _yaw;
		pitch = _pitch;

		currentYaw = yaw;
		currentPitch = pitch;

		followTarget.transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
	}

	public void ResetCameraBack()
	{
		yaw = MyPlayer.instance.transform.localEulerAngles.y;
		pitch = 0;

		currentYaw = yaw;
		currentPitch = pitch;

		followTarget.transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
	}

	public void ResetCameraFront()
	{
		yaw = MyPlayer.instance.transform.localEulerAngles.y - 180f;
		pitch = 0;

		currentYaw = yaw;
		currentPitch = pitch;

		followTarget.transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
	}

	public void SetCameraRotation(float _pitch, float _yaw)
	{
		pitch = _pitch;
		yaw = _yaw;

		currentPitch = 0f;
		currentYaw = 0f;

		followTarget.transform.rotation = Quaternion.Euler(0f, 0f, 0.0f);
	}

	public void SetCameraTarget(Vector3 _target, float _lerpspeed, bool _instant = false)
	{
		if (_instant)
		{
			followTarget.transform.localPosition = _target;
			return;
		}

		Util.RunCoroutine(FadeCameraTarget(_target, _lerpspeed), nameof(FadeCameraTarget));
	}

	public void SetCameraParentPosition(Vector3 _position)
	{
		parent.transform.position = _position;
	}

	public void SetCameraParentOrigin()
	{
		parent.transform.localPosition = parentOriginPosition;
	}

	public void SetCameraDistance(float _distance, float _lerpspeed = 1f, bool _instant = false)
	{
		if (_instant)
		{
			distanceOrigin = followCam.CameraDistance;
			followCam.CameraDistance = _distance;
			return;
		}

		Util.RunCoroutine(Co_CameraDistanceOrigin(followCam, _distance, _lerpspeed), nameof(Co_CameraDistanceOrigin), Util.SceneCoroutine.Player);
	}

	public void SetCameraParentHeight(float _height)
	{
		var tranform = MyPlayer.instance.Childrens[Cons.CAMERAPARENT].transform;

		tranform.localPosition = new Vector3(0f, _height, 0f);
	}



	public void SetCameraSholuderOffset(float _height, float _lerpSpeed = 1f, bool _instant = false)
	{
		if (_instant)
		{
			followCam.ShoulderOffset = new Vector3(0f, _height, 0f);
			return;
		}

		Util.RunCoroutine(Co_CameraSholuderOffset(_height, _lerpSpeed));
	}

	IEnumerator<float> Co_CameraSholuderOffset(float _target, float _lerpspeed = 1f)
	{
		float lerpvalue = 0f;
		
		while (Mathf.Abs(followCam.ShoulderOffset.y - _target) > 0.01f)
		{

			followCam.ShoulderOffset = Vector3.Lerp(followCam.ShoulderOffset, new Vector3(0f, _target, 0f), lerpvalue += _lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		followCam.ShoulderOffset = new Vector3(0f, _target, 0f);
	}



	IEnumerator<float> FadeCameraTarget(Vector3 _target, float _lerpspeed = 1f, float _delay = 0f)
	{
		float lerpvalue = 0f;

		yield return Timing.WaitForSeconds(_delay);

		while (Vector3.Distance(followTarget.transform.localPosition, _target) > 0.001f)
		{
			followTarget.transform.localPosition = Vector3.Lerp(followTarget.transform.localPosition, _target, lerpvalue += _lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}
		followTarget.transform.localPosition = _target;
	}



	public void SetCameraDistanceOrigin(float _lerpspeed = 1f, bool _instant = true)
	{
		if (_instant)
		{
			followCam.CameraDistance = distanceOrigin;
			return;
		}

		Util.RunCoroutine(Co_CameraDistanceOrigin(followCam, distanceOrigin, _lerpspeed), "SetCameraDistanceOrigin");
	}

	public float GetCameraDistnace()
	{
		return followCam.CameraDistance;
	}

	IEnumerator<float> Co_CameraDistanceOrigin(Cinemachine3rdPersonFollow _follow, float _target, float _lerpspeed = 1f, float _delay = 0f)
	{
		yield return Timing.WaitForSeconds(_delay);

		float lerpvalue = 0f;

		while (Mathf.Abs(_follow.CameraDistance - _target) > 0.01f)
		{

			_follow.CameraDistance = Mathf.LerpUnclamped(_follow.CameraDistance, _target, lerpvalue += _lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		_follow.CameraDistance = _target;
	}

	public void SetCameraZoom(float _distance, float _lerpspeed, float _delay = 0f, bool _instant = false)
	{
		if (_instant)
		{
			leanPinch.Zoom = _distance;
			return;
		}

		Util.RunCoroutine(FadeCameraZoom(_distance, _lerpspeed, _delay), "FadeCameraZoom");
	}

	public float GetZoomDistance()
	{
		return leanPinch.Zoom;
	}

	public IEnumerator<float> FadeCameraZoom(float _distance, float _lerpspeed, float _delay)
	{
		yield return Timing.WaitForSeconds(_delay);

		float lerpvalue = 0f;

		while (Mathf.Abs(leanPinch.Zoom - _distance) > 0.001f)
		{
			leanPinch.Zoom = Mathf.Lerp(leanPinch.Zoom, _distance, lerpvalue += _lerpspeed * Time.deltaTime);

			yield return Timing.WaitForOneFrame;
		}

		leanPinch.Zoom = _distance;
	}

	public void SetBlendStyle(CinemachineBlendDefinition.Style _style, float _speed = 1f)
	{
		brain.m_DefaultBlend = new CinemachineBlendDefinition(_style, _speed);
	}

	public void SetEaseSpeed(float _speed)
	{
		brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, _speed);
	}

	public void SetCameraRoll(float _yaw, float _pitch, bool _reset = false)
	{
		if (_reset)
		{
			yaw = 270f; pitch = 0f;
			return;
		}

		yaw = _yaw; pitch = _pitch;
	}

	public void ClampYaw(float _clampX, float _clampY, bool _reset = false)
	{
		if (_reset)
		{
			yawMinMax = new Vector2(float.MinValue, float.MaxValue);
			return;
		}

		yawMinMax.x = _clampX; yawMinMax.y = _clampY;
	}

	public void ClampPitch(float _clampX, float _clampY, bool _reset = false)
	{
		if (_reset)
		{
			pitchMinMax = new Vector2(-30f, 70f);
			return;
		}

		pitchMinMax.x = _clampX; yawMinMax.y = _clampY;
	}

	public Camera GetHighestPriorityCamera()
	{
		List<Camera> camerasList = new List<Camera>();

		Camera[] cameras = Object.FindObjectsOfType<Camera>();
		Camera camera = cameras[Random.Range(0, camerasList.Count)];

		for (int i = 0; i <cameras.Length;i++)
		{
			if (cameras[i] == stackCam) continue;

			camerasList.Add(cameras[i]);
		}

		for(int i = 0; i < camerasList.Count; i++)
		{
			if (camera.depth >= camerasList[i].depth) continue;

			camera = camerasList[i];
		}

		return camera;
	}

	public bool IsCinemachineBlendEnd(Transform _virtualCam)
	{		
		return 
			Vector3.Distance(mainCam.transform.position, _virtualCam.position) < 0.001f && 
			Quaternion.Dot(mainCam.transform.rotation, _virtualCam.rotation) > 0.001f;
	}

	public bool IsSubCamBlendEnd()
	{
		return 
			Vector3.Distance(mainCam.transform.position, subCam.transform.position) < 0.01f && 
			Quaternion.Dot(mainCam.transform.rotation, subCam.transform.rotation) > 0.01f;
	}

	public float SubCamBlendDistance()
	{
		return Vector3.Distance(mainCam.transform.position, subCam.transform.position);
	}

	public void SetVirtualCamNearClipPlane(float _value)
	{
		virtualCam.m_Lens.NearClipPlane = _value;
	}

	public void InitYawPitch()
	{
		yaw = MyPlayer.instance.transform.eulerAngles.y;
		pitch = 0;
	}

	public void SetYawPitch(float yaw, float pitch)
	{
		this.yaw = yaw;
		this.pitch = pitch;
	}
}
