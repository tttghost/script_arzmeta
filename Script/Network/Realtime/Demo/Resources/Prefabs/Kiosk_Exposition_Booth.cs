using FrameWork.Interaction;
using FrameWork.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiosk_Exposition_Booth : MonoBehaviour
{
    private void Start()
    {
        GetComponentInChildren<Contact>().Action_Contact = () => SceneLogic.instance.PushPanel<Panel_ExpositionFileBox>();
    }
}
