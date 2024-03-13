using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using Office;
using UnityEngine;

public class InOfficeRoomTutorialSwitch : MonoBehaviour
{
    public Transform tutorial_MyRoom;
    public Transform tutorial_OfficeRoomEnter;
    public Transform tutorial_OfficeRoomCreate;

    private void Start()
    {
        
    }

    public void OpenMyRoomTutorial()
    {
        if(tutorial_MyRoom) tutorial_MyRoom.gameObject.SetActive(true);
    }
    
    public void OpenOfficeRoomEnterTutorial()
    {
        if(tutorial_OfficeRoomEnter) tutorial_OfficeRoomEnter.gameObject.SetActive(true);
    }
    
    public void OpenOfficeRoomCreateTutorial()
    {
        if(tutorial_OfficeRoomCreate) tutorial_OfficeRoomCreate.gameObject.SetActive(true);
    }
}
