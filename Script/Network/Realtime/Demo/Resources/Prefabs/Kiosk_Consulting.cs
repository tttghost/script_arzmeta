using FrameWork.Interaction;
using FrameWork.UI;
using UnityEngine;

public class Kiosk_Consulting : MonoBehaviour
{    
    private void Start()
    {
        GetComponentInChildren<Contact>().Action_Contact = () => SceneLogic.instance.PushPanel<Panel_Consulting>();
    }
}
