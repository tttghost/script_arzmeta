using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Common;
using Lean.Touch;
using UnityEngine;
using UnityEngine.EventSystems;
using MEC;

public class LeanDragPanel : MonoBehaviour, IDragHandler
{
    public LeanManualRotate rotate;

    public RotateDirection type = RotateDirection.A;

    [Range(0.0f, 1.0f)] public float damping = 0.1f;

    private void Awake()
    {
        Timing.RunCoroutine(Co_SetData());

        //rotate.AxisA = new Vector3(0, -1, 0);
        //rotate.AxisB = new Vector3(1, 0, 0);
    }

    IEnumerator<float> Co_SetData()
    {
        yield return Timing.WaitUntilTrue(() => rotate != null);

        rotate.AxisA = new Vector3(0, -1, 0);
        rotate.AxisB = new Vector3(1, 0, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (LeanTouch.Fingers.Count > 1) return;

        switch (type)
        {
            case RotateDirection.A:
                rotate.RotateA(eventData.delta.magnitude * (eventData.delta.x > 0 ? 1 : -1) * damping);
                break;
            case RotateDirection.B:
                rotate.RotateB(eventData.delta.magnitude * (eventData.delta.y > 0 ? 1 : -1) * damping);
                break;
            case RotateDirection.AB:
                rotate.RotateAB(eventData.delta * damping);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ResetRotate()
    {
        rotate.transform.localRotation = Quaternion.identity;
    }
}

public enum RotateDirection
{
    A,
    B,
    AB
}
