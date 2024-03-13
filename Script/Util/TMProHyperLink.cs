using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using FrameWork.UI;

/// <summary>
/// 
/// Tmp_Text 사용 시 <link="링크"></link>의
/// "링크"를 찾아옴
/// 
/// </summary>
public class TMProHyperLink : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI m_TextMeshPro;
    private Camera m_Camera;
    private Canvas m_Canvas;

    void Start()
    {
        m_Canvas = gameObject.GetComponentInParent<Canvas>();
        m_Camera = m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : m_Canvas.worldCamera;

        m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.ForceMeshUpdate();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
            Debug.Log("linkInfo : " + linkInfo.GetLinkID());
        }
    }
}
