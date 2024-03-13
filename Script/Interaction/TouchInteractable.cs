using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum InteractionType
{
    [InspectorName("플레이어 거리")] PlayerDistance,
    [InspectorName("카메라 거리")] CameraDistance,
    [InspectorName("상호작용 영역")] Collision,
    [InspectorName("플레이어 거리 + 상호작용 영역")] PlayerDistanceAndCollision,
    [InspectorName("카메라 거리 + 상호작용 영역")] CameraDistanceAndCollision,
}

/// <summary>
/// 변고경 BKK
/// 콜라이더가 있는 게임 오브젝트를 터치시 등록한 이벤트를 호출해주는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class TouchInteractable : MonoBehaviour
{

    [Tooltip("터치시 이벤트 호출 타이밍")] 
    public float onTouchTiming = 0;

    [Tooltip("재터치 딜레이")] 
    public float onTouchDelay = 1;

    [Tooltip("상호작용 체크 방식")] 
    public InteractionType interactionType = InteractionType.PlayerDistance; 

    [Tooltip("상호작용 가능 거리")] 
    public float interactDistance = 8f;

    [Tooltip("상호작용 가능 영역")] 
    public InteractionArea interactionArea;

    [Tooltip("이 게임오브젝트를 터치시 호출합니다"), SerializeField] 
    private UnityEvent onTouch = new UnityEvent();
    
    private bool delayed = false;

    private void OnEnable()
    {
        delayed = false;
    }

    /// <summary>
    /// 상호작용 체크
    /// </summary>
    /// <param name="cameraDistance"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool IsInteraction(float cameraDistance = 0)
    {
        if (MyPlayer.instance == null)
        {
            return false;
        }

        float playerDistance = Vector3.Distance(MyPlayer.instance.transform.position, transform.position);

        bool isInteraction = false;

        bool isPlayer = playerDistance <= interactDistance;
        bool isCamera = cameraDistance <= interactDistance;
        bool isCollider = interactionArea && interactionArea.entered;

        switch (interactionType)
        {
            case InteractionType.PlayerDistance:                isInteraction = isPlayer; break;
            case InteractionType.CameraDistance:                isInteraction = isCamera; break;
            case InteractionType.Collision:                     isInteraction = isCollider; break;
            case InteractionType.PlayerDistanceAndCollision:    isInteraction = isPlayer && isCollider; break;
            case InteractionType.CameraDistanceAndCollision:    isInteraction = isCamera && isCollider; break;
        }

        return isInteraction;
    }

    /// <summary>
    /// 상호작용
    /// </summary>
    public void Interaction() => Util.RunCoroutine(Co_Interaction());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator<float> Co_Interaction()
    {
        if (delayed || !enabled)
        {
            yield break;
        }

        delayed = true;

        yield return Timing.WaitForSeconds(onTouchTiming);

        Single.Sound.PlayEffect("effect_poshit_2");

        onTouch?.Invoke();

        yield return Timing.WaitForSeconds(onTouchDelay);

        delayed = false;
    }

    #region 이벤트

    /// <summary>
    /// OnTouch 이벤트를 등록합니다.
    /// </summary>
    /// <param name="action"></param>
    public void AddEvent(UnityAction action)
    {
        onTouch.AddListener(action);
    }

    /// <summary>
    /// OnTouch 이벤트를 해제합니다.
    /// </summary>
    /// <param name="action"></param>
    public void RemoveEvent(UnityAction action)
    {
        onTouch.RemoveListener(action);
    }

    /// <summary>
    /// 모든 OnTouch 이벤트를 해제합니다.
    /// </summary>
    public void RemoveAllEvent()
    {
        onTouch.RemoveAllListeners();
    }

    /// <summary>
    /// 실행중인 모든 OnTouch 이벤트를 중단합니다.
    /// </summary>
    public void StopAllEvent()
    {
        StopAllCoroutines();
    }

    #endregion
}
