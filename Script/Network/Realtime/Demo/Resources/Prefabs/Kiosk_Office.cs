using FrameWork.Interaction;
using FrameWork.UI;
using UnityEngine;

public class Kiosk_Office : MonoBehaviour
{
    private void Start()
    {
        GetComponentInChildren<Contact>().Action_Contact = () => SceneLogic.instance.PushPanel<Panel_Office>();
    }
}
