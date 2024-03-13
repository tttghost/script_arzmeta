using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Touch Interactable 컴포넌트가 있는 게임 오브젝트와 상호작용 할 수 있게 해주는 컴포넌트입니다.
/// BKK
/// 작성자: 변고경
/// 작성일자: 2021/8/5
/// </summary>
public sealed class MobileTouchInteracter : BaseTouchInteracter
{
    protected override void Awake()
    {
        base.Awake();

        // 포탈 터치 대응. 20220614 변고경
        triggerInteraction = QueryTriggerInteraction.Collide; 
    }

    protected override void OnClickPerformed(InputAction.CallbackContext callback)
    {
        base.OnClickPerformed(callback);
        if (!IsPointerOverUI())
        {
            Interact();
        }
    }

    /// <summary>
    /// Touch Interactable과 상호작용
    /// </summary>
    private void Interact()
    {
        var hit = GetWorldPositionHitData(pointPos);

        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out TouchInteractable interactable))
        {
            if (interactable.IsInteraction(hit.distance))
            {
                interactable.Interaction();
            }
        }
    }
}

