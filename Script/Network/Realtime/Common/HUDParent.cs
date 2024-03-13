/****************************************************************

기능 : 플레이어 위에 NickName, Chat Text 관리하는 Script

*****************************************************************/

using TMPro;
using UnityEngine;
using FrameWork.UI;
using Vector3 = UnityEngine.Vector3;
using System.Collections.Generic;
using MEC;
using Unity.Linq;
using System.Linq;

/// <summary>
/// 동기화 중인 플레이어의 NickName, Chat 관리
/// </summary>
public class HUDParent : MonoBehaviour
{
    private UIBase uIBase;
    private TMP_Text txtmp_NickName = null;
    private TMP_Text txtmp_SpeechBubble = null;

    private Animator go_SpeechBubble;

    private Transform targetTransform;

    [Range(0.00f, 10.00f), SerializeField] private float offset = 0.8f;

    private const string isChatBubble = "isChatBubble";
    private void Awake()
    {
        uIBase = GetComponent<UIBase>();

        txtmp_NickName = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_NickName));
        txtmp_SpeechBubble = uIBase.GetUI_TxtmpMasterLocalizing(nameof(txtmp_SpeechBubble));
        txtmp_SpeechBubble.text = "";

        go_SpeechBubble = uIBase.GetChildGObject(nameof(go_SpeechBubble)).GetComponent<Animator>();

        targetTransform = transform.parent.Search("Bip001 Head");
    }

    private void Update()
    {
        UpdatePosition();
        UpdateScale();
    }

    /// <summary>
    /// 내 거리와 상대방 거리에 따른 스피치버블 크기 조절
    /// </summary>
    private void UpdateScale()
    {
        return;
        if(MyPlayer.instance == null || MyPlayer.instance.HudParent == this || MyPlayer.instance.HudParent == null)
        {
            return;
        }

        float speechBubbleScale = Vector3.Distance(MyPlayer.instance.HudParent.transform.position, transform.position);

        transform.localScale = Vector3.one * Util.LerpInverseLerp(minA, maxA, minB, maxB, speechBubbleScale);
    }
    public float minA = 1f;
    public float maxA = 2f;
    public float minB = 0.01f;
    public float maxB = 0.02f;

    /// <summary>
    /// 머리위 스피치버블 위치
    /// </summary>
    private void UpdatePosition()
    {
        if (!targetTransform) return;
        transform.position = targetTransform.position + Vector3.up * offset;
    }

    // 패킷에서 새로운 닉네임을 받았을 때 호출하여 적용 
    public void SetNickName( string nickName )
    {
        txtmp_NickName.text = nickName;
        transform.parent.name = nickName;
    }

    /// <summary>
    /// 말풍선 말하기
    /// </summary>
    /// <param name="text"></param>
    public void SpeechBubble( string text )
    {
        go_SpeechBubble.Rebind();

        txtmp_SpeechBubble.text = text;

        Util.RunCoroutine(Co_SpeechBubble(), _tag: GetInstanceID() + "chat");
    }

    private const float SpeechBubbleDuration = 5f;
    private IEnumerator<float> Co_SpeechBubble()
    {
        go_SpeechBubble.SetBool(isChatBubble, true);

        yield return Timing.WaitForSeconds(SpeechBubbleDuration);

        go_SpeechBubble.SetBool(isChatBubble, false);
    }
}
