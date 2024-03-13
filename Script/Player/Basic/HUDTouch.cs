using Lean.Touch;
using StarterAssets;
using UnityEngine;
using System;

[Serializable]
public class HUDTouch
{
    [SerializeField] public GameObject pinch;              // LeanPinchCamera
	[SerializeField] public GameObject zone;               // view_TouchZone

    [SerializeField] public UIVirtualTouchZone_Custom virtualTouch;
    [SerializeField] public UIEventChecker eventChecker;
    [SerializeField] public LeanPinchCameraCustom_Cinemachine pinchCustom;
    [SerializeField] public MobileTouchInteracter touchInteracter;

    public HUDTouch(Panel_HUD _panel)
    {
        zone = Util.Search(_panel.gameObject, Cons.ViewTouch).gameObject;
        pinch = Util.Search(_panel.gameObject, Cons.LeanPinchCamera).gameObject;

        eventChecker = zone.GetComponent<UIEventChecker>();
        virtualTouch = zone.GetComponent<UIVirtualTouchZone_Custom>();
        pinchCustom = zone.GetComponentInChildren<LeanPinchCameraCustom_Cinemachine>();

        touchInteracter = UnityEngine.Object.FindObjectOfType<MobileTouchInteracter>();
    }
}
