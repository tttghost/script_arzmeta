/*
 * 
 *			PanelProgress
 * 
 */
using UnityEngine;
using UnityEngine.UI;
using FrameWork.UI;
using TMPro;
using System.Collections;
using System.Text;

public class Panel_Progress : PanelBase
{
	[HideInInspector] public Image _imgGauge = default;
    [HideInInspector] public TMP_Text _txtTMPercent = default;
    [HideInInspector] public TMP_Text _txtTMPTitle = default;
    [HideInInspector] public float parentWidth = 0f;
    [HideInInspector] public float parentHeight = 0f;
    private string titleHead = ". ";
    private string titleTail = " .";
    protected override void SetMemberUI()
	{
		base.SetMemberUI();
        // 게이지 조절용 Image
        _imgGauge = GetUI<Image>( "img_Gauge" );
		RectTransform imageParentRt = _imgGauge.transform.parent.GetComponent<RectTransform>();
		parentHeight = imageParentRt.rect.height; 
		parentWidth = imageParentRt.rect.width;

        _imgGauge.rectTransform.sizeDelta = new Vector2(parentWidth, parentHeight);


        // 게이지 퍼센트 표시 : TextMeshProUGUI 
        _txtTMPercent = GetUI<TMP_Text>("txtmp_percent");
        _txtTMPTitle = GetUI<TMP_Text>("txtmp_Title");
        

        SetProgress( 0.0f );
		
	}
    Coroutine coroutine;
    IEnumerator TitleAction(string title)
    {
        while (true)
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.Append(title);
            for (int i = 0; i < 4; i++)
            {
                StringBuilder sb2 = new StringBuilder();
                if (i > 0) sb2.Append(titleHead);
                sb2.Append(sb1);
                if (i > 0) sb2.Append(titleTail);
                sb1 = sb2;
                _txtTMPTitle.text = sb2.ToString();
                yield return new WaitForSeconds(0.5f);
            }

        }
    }
    public void SetProgressTitle(string title, bool titleAct = false)
    {
        // 타이틀 텍스트를 null 또는 string.empty 로 지정할 경우
        // 타이틀 텍스트 표시 비활성화 시키기
        if (string.IsNullOrEmpty(title))
        {
            Image bgTitle = GetUI<Image>("img_BG_Title");
            if (bgTitle != null)
                bgTitle.gameObject.SetActive(false);

            return;
        }
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        if (!titleAct)
        {
            _txtTMPTitle.text = title;
        }
        else
        {
            coroutine = StartCoroutine(TitleAction(title));
        }
    }

    public void SetProgress(float progress)
    {
        if (_imgGauge != null)
        {
            _imgGauge.rectTransform.sizeDelta = new Vector2(progress * parentWidth < parentHeight ? parentHeight : progress * parentWidth, parentHeight);
        }

        if (_txtTMPercent != null)
        {
            _txtTMPercent.text = progress.ToString("P");
        }
    }
}
