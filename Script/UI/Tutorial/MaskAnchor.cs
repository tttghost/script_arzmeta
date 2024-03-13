using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// 타겟 이미지와 동일한 모양의 마스크 이미지를 생성하여 위치 세팅해줍니다.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MaskAnchor : MonoBehaviour
{
    [SerializeField, Tooltip("타겟 이름")] private string targetName;
    
    [SerializeField, Tooltip("타겟 이미지"), ReadOnly] private RectTransform target;
    [SerializeField, Tooltip("마스크 RectTransform"), ReadOnly] private RectTransform rectTransform;
    [SerializeField, Tooltip("마스크 이미지"), ReadOnly] private Image image;

    public bool update = false;

    public bool preventSize = true;
    
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        FindComponents();
    }

    private async void OnEnable()
    {
        FindTarget();

        await UniTask.NextFrame();
        
        CopyRectTransformData(target, preventSize);
    }

    private void Update()
    {
        if (!update) return;
        
        if(!target) FindTarget();
        else CopyRectTransformData(target, preventSize);
    }

    /// <summary>
    /// 타겟 이미지의 RectTransform 값과 이미지를 복사하여 마스크를 생성합니다.
    /// </summary>
    /// <param name="_rectTransform">타겟 이미지</param>
    /// <param name="preventSize">사이즈 재조정 불가 여부</param>
    public void SetTarget(RectTransform target, bool preventSize = false)
    {
        if (target)
        {
            if (string.IsNullOrEmpty(targetName)) targetName = target.GetPath();
            this.target = target;
        }
        else
        {
            FindTarget();
        }

        if (this.target)
        {
            GetMaskImage(this.target);
            CopyRectTransformData(this.target, preventSize);
        }
    }

    /// <summary>
    /// 타겟 이미지의 RectTransform을 찾아옵니다.
    /// </summary>
    public void FindTarget()
    {
        if (string.IsNullOrEmpty(targetName)) return;
        if (target) return;

        var rootName = targetName.Split('/')[0];
        var findPath = targetName.Substring(targetName.IndexOf('/') + 1);
        var rootGo = GameObject.Find(rootName);

        if (!rootGo)
        {
            Debug.Log($"rootName을 찾을 수 없습니다. ( {rootName} )");
            return;
        }
        
        var targetGo = rootGo.transform.Find(findPath);

        if (targetGo)
        {
            target = (RectTransform)targetGo.transform;
        }
    }
    
    /// <summary>
    /// 필요 컴포넌트를 가져옵니다.
    /// </summary>
    private void FindComponents()
    {
        if (!rectTransform) rectTransform = GetComponent<RectTransform>();
        if (!image) image = GetComponent<Image>();
    }

    /// <summary>
    /// 타겟 이미지의 RectTransform 값을 복사합니다.
    /// </summary>
    /// <param name="_rectTransform">타겟 이미지</param>
    /// <param name="preventSize">사이즈 재조정 불가 여부</param>
    public void CopyRectTransformData(RectTransform _rectTransform, bool preventSize = false)
    {
        if (target == null || rectTransform == null) return;

        if (_rectTransform.anchorMin == Vector2.zero && _rectTransform.anchorMax == Vector2.one)// 스트레치 상태이면 앵커를 가운데로
        {
            rectTransform.anchorMin = Vector2.one * 0.5f;
            rectTransform.anchorMax = Vector2.one * 0.5f;
            if (!preventSize)
                rectTransform.sizeDelta = new Vector2(_rectTransform.rect.width, _rectTransform.rect.height);
        }
        else
        {
            rectTransform.anchorMin = _rectTransform.anchorMin;
            rectTransform.anchorMax = _rectTransform.anchorMax;
            if(!preventSize) rectTransform.sizeDelta = _rectTransform.sizeDelta;
        }
        
        rectTransform.pivot = _rectTransform.pivot;
        rectTransform.anchoredPosition = _rectTransform.anchoredPosition;
        rectTransform.localScale = _rectTransform.localScale;
        rectTransform.position = _rectTransform.position;
        rectTransform.rotation = _rectTransform.rotation;
        
        // rectTransform.anchorMin = _rectTransform.anchorMin;
        // rectTransform.anchorMax = _rectTransform.anchorMax;
        // if(!preventSize) rectTransform.sizeDelta = _rectTransform.sizeDelta;
    }

    /// <summary>
    /// 마스크 이미지를 가져옵니다.
    /// </summary>
    /// <param name="_rectTransform"></param>
    private void GetMaskImage(RectTransform _rectTransform)
    {
        var hasImage = _rectTransform.TryGetComponent(out Image image);

        if (hasImage)
        {
            this.image.sprite = image.sprite;
            this.image.type = image.type;
            this.image.pixelsPerUnitMultiplier = image.pixelsPerUnitMultiplier;
            this.image.fillCenter = image.fillCenter;
        }
    }
}
