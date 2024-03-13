using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_OtherPlayer : MonoBehaviour
{
    Outline outline;

    private void Start()
    {
        //outline = this.GetComponentInChildren<Outline>();
        //outline.enabled = false;

        StartCoroutine(SetInteractionArea());
    }

    private IEnumerator SetInteractionArea()
    {
        yield return Timing.WaitUntilTrue(() => MyPlayer.instance);

        while (MyPlayer.instance)
        {
            if (Vector3.Distance(transform.position, MyPlayer.instance.transform.position) <= 10)
            {
                // outline.enabled = true;

                if (GetComponentInChildren<TabPlayer>() == null)
                    gameObject.AddComponent<TabPlayer>();
                // Debug.Log(Vector3.Distance(transform.position, MyPlayer.instance.transform.position));
            }
            else
            {
                // outline.enabled = false;
                if (GetComponentInChildren<TabPlayer>() != null)
                    Destroy(gameObject.GetComponentInChildren<TabPlayer>());
                // Debug.Log("근방에 존재하는 플레이어 없음");
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}
