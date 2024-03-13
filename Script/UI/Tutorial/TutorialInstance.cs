using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.UI;

public class TutorialInstance : MonoBehaviour
{
    private void Start()
    {
        Create();
    }

    protected virtual void Create()
    {
        if (SceneLogic.instance is Scene_OfficeRoom logic)
        {
            // LoadTutorial("Tutorial_OfficeRoomHostFirstTime");
            // // if(logic.AmIHost()) LoadTutorial("Tutorial_OfficeRoomHostFirstTime");
            //
            // OfficeTopButton officeTopButton = FindObjectOfType<OfficeTopButton>(true);
            //
            // if (officeTopButton)
            // {
            //     var btn_UserInfo = officeTopButton.transform.Search<Button>("btn_UserInfo");
            //     var tutorial_OfficeRoomUserInfo = LoadTutorial("Tutorial_OfficeRoomUserInfo");
            //     
            //     btn_UserInfo.onClick.AddListener(() =>
            //     {
            //         if (tutorial_OfficeRoomUserInfo)
            //         {
            //             tutorial_OfficeRoomUserInfo.gameObject.SetActive(true);
            //         }
            //     });
            // }
            //
            // Popup_OfficeRoomSave popup_OfficeRoomSave = SceneLogic.instance.GetPopup<Popup_OfficeRoomSave>(nameof(Popup_OfficeRoomSave));
            //
            // if (popup_OfficeRoomSave)
            // {
            //     var tutorial_OfficeRoomInfo = LoadTutorial("Tutorial_OfficeRoomInfo");
            //     
            //     popup_OfficeRoomSave.SetOpenStartCallback(() =>
            //     {
            //         if (tutorial_OfficeRoomInfo)
            //         {
            //             tutorial_OfficeRoomInfo.gameObject.SetActive(true);
            //         }
            //     });
            // }
        }
        else
        {
            LoadTutorial("TutorialSwitch");
        }
    }

    public virtual Transform LoadTutorial(string name)
    {
        var prefab = Resources.Load<GameObject>("Addressable/Prefab/UI/Tutorial/" + name);
        var canvas = FindObjectOfType<MasterCanvas>();
        var instance = Instantiate(prefab, canvas.transform);
        
        instance.transform.SetAsLastSibling();

        return instance.transform;
    }
}
