using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RaycastStinger : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IDragHandler,
    IBeginDragHandler, 
    IEndDragHandler, 
    IScrollHandler
{
    private GameObject newTarget;

    public string targetName;

    public UnityEvent onClick = new UnityEvent();

    public void StingEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> eventHandler, Action afterAction = null)
        where T : IEventSystemHandler
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        if (targetName != string.Empty)
        {
            newTarget = raycastResults.Find((result) => result.gameObject.name == targetName).gameObject;
        }
        else
        {
            if (raycastResults.Count > 1)
            {
                newTarget = raycastResults[1].gameObject;
            }
        }

        if (newTarget)
        {
            ExecuteEvents.Execute(newTarget, eventData, eventHandler);
        
            afterAction?.Invoke();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.pointerClickHandler, onClick.Invoke);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    public void OnDrag(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.dragHandler);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.beginDragHandler);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.endDragHandler);
    }

    public void OnScroll(PointerEventData eventData)
    {
        StingEvent(eventData, ExecuteEvents.scrollHandler);
    }
}
