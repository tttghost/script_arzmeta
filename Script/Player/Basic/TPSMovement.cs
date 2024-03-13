using StarterAssets;
using UnityEngine;

[System.Serializable]
public class TPSMovement
{
	[Space(5f), Tooltip("Default")]
	[Range(0f, 20f)] public float moveSpeed = 1.52f;
	[Range(0f, 30f)] public float sprintSpeed = 4.7f;
	[HideInInspector] public float moveSmoothRate = 10.0f;                            // Speed change rate (Acceleration and deceleration)
	[HideInInspector] public float rotateSmoothRate = 0.07f;                         // How fast the character turns to face movement direction

	[Space(10), Tooltip("Joystick")]
	public bool joystickLock = false;
	public bool joystickSprint = true;                                                             // Run When Joystick Fully Pulled, 조이스틱 감도에 따른 속도 조절 - 20220118 변고경 연구원

	[Space(10), Tooltip("Gravity")]
	public float gravity = -15f;                                                                       // 자체 중력값 (기본값은 -9.81f)
	public float fallSpeed;                                                                             // 낙하 속도
	public float fallSpeedLimit = -20f;                                                           // 낙하 속도 제한

	[Space(10), Tooltip("Jump")]
	[Range(0f, 50f)] public float jumpHeight = 1.2f;                                      // 점프 높이
	[HideInInspector] public float jumpInterval = 0.506f;                           // 다시 점프하기까지 필요한 시간. 즉시 다시 점프하려면 0f로 설정
	[HideInInspector] public float fallInterval = 0.15f;                                // 하강 상태에 들어가기 전에 필요한 시간.	 (계단을 내려갈 때 유용)
	[HideInInspector] public float jumpDeltaTime;                                       // 점프 후 누적 시간
	[HideInInspector] public float fallDeltaTime;                                         // 하강 후 누적 시간

	[HideInInspector] public bool isJumping = false;                                    // 점프를 하는 중 인지에 대한 여부
	[HideInInspector] public bool isGravity = true;                                      // 중력을 사용할 것인지에 대한여부
	[HideInInspector] public bool isUpdate = false;                                      // 중력을 사용할 것인지에 대한여부

	[Space(10), Tooltip("Grounded")]
	public bool grounded = true;                                                                    // 캐릭터가 접지되었는지 여부. 기본 확인에 내장된 CharacterController의 일부가 아님
	public LayerMask groundLayer;                                                               // 캐릭터가 그라운드로 사용하는 레이어
	public float groundedRange = 0.28f;                         // 접지된 수표의 반경입니다. CharacterController의 반경과 일치해야 함
	public float groundedOffset = -0.14f;                      // 거친 땅에 유용
	[HideInInspector] public Vector3 groundPosition;

	[HideInInspector] public bool checkSpeed = false;
	[HideInInspector] public float currentSpeed;
	[HideInInspector] public float targetSpeed;
	[HideInInspector] public float targetRotation;
	[HideInInspector] public float rotationVelocity;
	[HideInInspector] public float terminalVelocity = 53.0f;

	float sprintSpeedOrigin = 0f;

	StarterAssetsInputs input;

	public void Awake()
	{
		input = MyPlayer.instance.TPSController.StarterInputs;
	}

	public void Start()
	{
		jumpDeltaTime = jumpInterval;
		fallDeltaTime = fallInterval;
		sprintSpeedOrigin = sprintSpeed;
	}

	public bool IsGrounded()
	{
		return grounded;
	}

	public bool IsMoving()
	{
		return input.move != Vector2.zero;
	}

	public bool IsJumping()
	{
		return isJumping;
	}

	public bool IsRunning()
	{
		return (targetSpeed == sprintSpeed) ? true : false;
	}

	public bool IsFallSpeedNearLimit()
	{
		return fallSpeed < fallSpeedLimit + 1f;
	}

	public void SetSprintSpeed(float _speed)
	{
		sprintSpeed = _speed;
	}

	public void ResetSprintSpeed()
	{
		sprintSpeed = sprintSpeedOrigin;
	}

	public void EnableJumpInput(bool _enable)
	{
		input.blockJump = _enable ? false : true;
	}

	public void EnableDashInput(bool _enable)
	{
		input.blockSprint = _enable ? false : true;
	}
}
