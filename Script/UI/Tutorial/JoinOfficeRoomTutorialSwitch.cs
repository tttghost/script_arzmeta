using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using Office;
using UnityEngine;
using UnityEngine.UI;

public class JoinOfficeRoomTutorialSwitch : MonoBehaviour
{
    public Transform tutorial_MyRoom;
    public Transform tutorial_OfficeRoomEnter;
    public Transform tutorial_OfficeRoomCreate;
    // public Transform tutorial_OfficeRoomInfo;

    private void Start()
    {
        if (SceneLogic.instance is Scene_Logo ||
            SceneLogic.instance is Scene_Title ||
            SceneLogic.instance is Scene_Lobby)
        {
            return;
        }

        if (SceneLogic.instance is Scene_MyRoom)
        {
            OpenMyRoomTutorial();
        }

        // Panel_OfficeRoom panelOfficeRoom = SceneLogic.instance.GetPanel<Panel_OfficeRoom>(nameof(Panel_OfficeRoom));
        //
        // if (panelOfficeRoom)
        // {
        //     var tog_OfficeCreate = panelOfficeRoom.GetUI<Toggle>("tog_OfficeCreate");
        //     var tog_RoomEnter = panelOfficeRoom.GetUI<Toggle>("tog_RoomEnter");
        //
        //     tog_OfficeCreate.onValueChanged.AddListener((enable) =>
        //     {
        //         if (enable)
        //         {
        //             OpenOfficeRoomCreateTutorial();
        //         }
        //     });
        //
        //     panelOfficeRoom.SetOpenStartCallback(OpenOfficeRoomEnterTutorial);
        // }

        // if (roomEnterToggle.isOn)
        // {
        //     panelOfficeRoom.SetOpenStartCallback(OpenOfficeRoomEnterTutorial);
        //     return;
        // }
        //
        // roomEnterToggle.onValueChanged.AddListener((enable) =>
        // {
        //     if (enable)
        //     {
        //         OpenOfficeRoomEnterTutorial();
        //     }
        // });
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
    
    // public void OpenOfficeRoomInfoTutorial()
    // {
    //     if(tutorial_OfficeRoomInfo) tutorial_OfficeRoomInfo.gameObject.SetActive(true);
    // }
}
