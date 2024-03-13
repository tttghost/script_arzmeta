using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


/// <summary>
/// Overlay UI를 제외한 오브젝트들에 대한 터치 조작을 가능하게 해주는 베이스 클래스입니다.
/// 
/// 작성자: 변고경
/// 작성일자: 2021/8/5
/// </summary>
public class BaseTouchInteracter : MonoBehaviour
{
    //public UIEventChecker eventChecker;

    [Tooltip("콜라이더 트리거에 대한 상호작용 여부")]
    [SerializeField]
    protected QueryTriggerInteraction triggerInteraction;

    [Tooltip("상호작용 처리에서 무시할 레이어")]
    [SerializeField]
    protected LayerMask ignoreLayerMask;

    [Tooltip("레이캐스트 거리")] [SerializeField] private float collideMaxDistance = 100f;

    protected static Vector2 pointPos;

    protected Camera overlayCamera;
    
    protected readonly RaycastHit emptyHit = new RaycastHit();

    public Camera OveralyCam { get => overlayCamera; set => overlayCamera = value; }
    public float Distance { set => collideMaxDistance = value; }

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        InitInputs();
        FindCamera();
    }

    public void Refresh()
	{
        DEBUG.LOG("BASE TOUCH INTERACTER IS REFRESHED.", eColorManager.UI);
        RemoveInputs();

        InitInputs();
        FindCamera();
    }

    /// <summary>
    /// InputSystem 기반의 FrontisInputSystem의 입력 방식을 초기화합니다.
    /// </summary>
    protected void InitInputs()
    {
        FrontisInputSystem.inputs.Touch.SingleTap.started += OnClickStarted;
        FrontisInputSystem.inputs.Touch.SingleTap.canceled += OnClickCancled;
        FrontisInputSystem.inputs.Touch.SingleTap.performed += OnClickPerformed;

        FrontisInputSystem.inputs.Touch.Point.performed += OnLookPerformed;
    }

    private void OnLookPerformed(InputAction.CallbackContext obj)
    {
        pointPos = obj.ReadValue<Vector2>();
    }

    private void RemoveInputs()
	{
        FrontisInputSystem.inputs.Touch.SingleTap.started -= OnClickStarted;
        FrontisInputSystem.inputs.Touch.SingleTap.canceled -= OnClickCancled;
        FrontisInputSystem.inputs.Touch.SingleTap.performed -= OnClickPerformed;

        FrontisInputSystem.inputs.Player.Look.performed -= OnLookPerformed;
    }

    protected void FindCamera() => Timing.RunCoroutine(Co_FindCamera());
    
    private IEnumerator<float> Co_FindCamera()
    {
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance != null);
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance.TPSController.Camera.MainCam != null);

        overlayCamera = MyPlayer.instance.TPSController.Camera.MainCam;
    }

    protected virtual void OnClickStarted(InputAction.CallbackContext callback)
    {
    }

    protected virtual void OnClickCancled(InputAction.CallbackContext callback)
    {
    }

    protected virtual void OnClickPerformed(InputAction.CallbackContext callback)
    {
        
    }

    //protected virtual void Update()
    //{
    //    if (!FrontisInputSystem.finishSet) return;

    //    pointPos = FrontisInputSystem.inputs.Touch.Point.ReadValue<Vector2>();
    //}

    /// <summary>
    /// 터치 / 마우스 클릭 입력시 입력 포지션에 대응하는 3D 포지션에 레이캐스트 히트 데이터를 리턴
    /// </summary>
    /// <param name="screenPosition">터치 / 마우스 좌표</param>
    /// <returns>입력 포지션에 대응하는 3D 포지션에 레이캐스트 히트 데이터</returns>
    public RaycastHit GetWorldPositionHitData(Vector3 screenPosition)
    {
        if (!overlayCamera) return emptyHit;

        if (FrontisInputSystem.inputs.Touch.SecondaryTouch.triggered) return emptyHit;
        
        var ray = overlayCamera.ScreenPointToRay(screenPosition);

        Physics.Raycast(ray, out RaycastHit hit, collideMaxDistance, ~ignoreLayerMask, triggerInteraction);

        return hit;
    }
    
    /// <summary>
    /// 터치한 포인트가 UI 위에 있는지 여부
    /// </summary>
    /// <returns></returns>
    public static bool IsPointerOverUI()
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = pointPos,
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Any(result => result.gameObject.layer == LayerMask.NameToLayer("UI"));
    }
}

