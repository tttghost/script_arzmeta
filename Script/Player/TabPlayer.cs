using UnityEngine;
using FrameWork.UI;
using FrameWork.Network;
using System.Collections;
using MEC;
using System.Collections.Generic;

[RequireComponent(typeof(TouchInteractable))]
public class TabPlayer : MonoBehaviour
{
    #region 변수
    private TouchInteractable touchInteractable;

    private float distance = 50f;
    private int layerMask;
    #endregion

    #region 초기화
    private void Start()
    {
        touchInteractable = GetComponent<TouchInteractable>();
        if (touchInteractable != null)
        {
            touchInteractable.interactDistance = distance;
            touchInteractable.AddEvent(TabPlayerEvent);
        }

        layerMask = 1 << LayerMask.NameToLayer("OtherPlayer");
    }
    #endregion

    #region TabPlayer
    /// <summary>
    /// 탭한 Player의 MemberID 받아서 넘기기
    /// </summary>
    private void TabPlayerEvent()
    {
        switch (SceneLogic.instance.GetSceneType())
        {
            case SceneName.Scene_Room_JumpingMatching:
            case SceneName.Scene_Room_OXQuiz: 
                return;
            default: break;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, distance, layerMask))
        {
            GameObject player = GameObject.Find("Players/" + hit.transform.name);
            if (player != null)
            {
                Util.RunCoroutine(Co_OpenPlayerInfo(player.GetComponent<NetworkTransform>().clientId));
            }
        }
    }

    private IEnumerator<float> Co_OpenPlayerInfo(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var popup_PlayerInfo = SceneLogic.instance.GetPopup<Popup_PlayerInfo>();

            popup_PlayerInfo.SetPlayerInfo(OTHERINFO_TYPE.MEMBERCODE, id);
            yield return Timing.WaitUntilTrue(()=> popup_PlayerInfo.isInitUI);

            SceneLogic.instance.PushPopup<Popup_PlayerInfo>();
        }
    }

    #endregion
}
