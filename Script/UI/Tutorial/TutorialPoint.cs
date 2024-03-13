using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

/// <summary>
/// 튜토리얼 스텝 내 포인트 클래스입니다.
/// Tutorial Manager에 의해 Tutorial Step이 생성될때 같이 생성되며, 한 포인트에는 하나의 Mask Anchor(강조 마스크)를 포함합니다. 
/// </summary>
public class TutorialPoint : MonoBehaviour
{
    [SerializeField, Tooltip("마스크 이미지")] private MaskAnchor anchor;

    [SerializeField, Tooltip("설명")] private TMP_Text description;
    [SerializeField, Tooltip("설명 텍스트 루트 트랜스폼")] private RectTransform descriptionRoot;

    private void OnValidate()
    {
        FindComponents();
    }

    private void Awake()
    {
        FindComponents();
    }

    /// <summary>
    /// 필요 컴포넌트를 가져옵니다.
    /// </summary>
    private void FindComponents()
    {
        if (!anchor) anchor = GetComponent<MaskAnchor>();
        if (!description) description = GetComponentInChildren<TMP_Text>();
        if (!descriptionRoot) descriptionRoot = transform.Search<RectTransform>("img_Description_Bg");
    }

    /// <summary>
    /// 타겟 이미지의 RectTransform 값과 이미지를 복사하여 마스크를 생성합니다.
    /// </summary>
    /// <param name="target">타겟 이미지</param>
    /// <param name="preventSize">사이즈 재조정 불가 여부</param>
    public void SetAnchorTarget(RectTransform target, bool preventSize = false)
    {
        anchor.SetTarget(target, preventSize);
    }

    /// <summary>
    /// 설명 텍스트를 세팅합니다.
    /// </summary>
    /// <param name="text">설명</param>
    /// <param name="fontSize">글자 크기</param>
    public void SetDescription(string text, int fontSize = 30)
    {
        if (string.IsNullOrEmpty(text) || fontSize == 0) description.enabled = false;
        
        SetText(text);
        description.fontSize = fontSize;

        CalculateDescriptionPosition();
    }

    /// <summary>
    /// 마스터 데이터 전용
    /// 마스터 데이터에 존재하는 번역 텍스트를 가져와서 설명 텍스트를 세팅합니다.
    /// </summary>
    /// <param name="masterId">마스터 아이디</param>
    /// <param name="fontSize">글자 크기</param>
    /// <param name="args">마스터 번역 텍스트에 포함될 매개변수</param>
    public void SetDescriptionFromMaster(string masterId, int fontSize = 30, params object[] args)
    {
        if (!Application.isPlaying) return;

        if (string.IsNullOrEmpty(masterId) || fontSize == 0) description.enabled = false;
        
        if (args.Length != 0)
        {
            Util.SetMasterLocalizing(description, new MasterLocalData(masterId, args));
        }
        else
        {
            Util.SetMasterLocalizing(description, new MasterLocalData(masterId));
        }
        
        description.fontSize = fontSize;
    }
    
    /// <summary>
    /// 유니티 로컬라이제이션 패키지 전용
    /// 로컬라이제이션 테이블에 존재하는 번역 텍스트를 가져와서 설명 텍스트를 세팅합니다.
    /// </summary>
    /// <param name="stringTableName">테이블 이름</param>
    /// <param name="key">키 이름</param>
    /// <param name="fontSize">글자 크기</param>
    public async void SetDescription(string stringTableName, string key, int fontSize = 30)
    {
        await LocalizationSettings.InitializationOperation;
        await LocalizationSettings.StringDatabase.PreloadOperation;
        
        var stringEvent = description.GetComponent<LocalizeStringEvent>();

        if (stringEvent)
        {
            stringEvent.StringReference.SetReference(stringTableName, key);
            stringEvent.OnUpdateString.AddListener(SetText);
            stringEvent.RefreshString();
        }
        else
        {
            Debug.LogWarning($"[{this.gameObject.name}]LocalizeStringEvent가 존재하지 않습니다.");
        }
        
        description.fontSize = fontSize;
        
        CalculateDescriptionPosition();
    }

    /// <summary>
    /// 설명 텍스트를 세팅합니다.
    /// </summary>
    /// <param name="str"></param>
    public void SetText(string str)
    {
        description.text = str;
    }

    /// <summary>
    /// 설명 텍스트의 Pivot을 타겟 이미지 위치에 맞춰 계산합니다.
    /// </summary>
    public void CalculateDescriptionPosition()
    {
        float xPos = 0, yPos = 0;
        float pivotX = 0, pivotY = 0;
        float screenWidth = Camera.main.pixelWidth;
        float screenHeight = Camera.main.pixelHeight;

        var currentPos = ((RectTransform) transform).position;
        var imageRect = (RectTransform) anchor.transform;

        if (currentPos.x < (screenWidth / 2.0f))// 스크린 중앙 기준 왼쪽에 있으면
        {
            description.alignment = TextAlignmentOptions.MidlineLeft;
            xPos = imageRect.rect.width / 2.0f;
            pivotX = 0;
        }
        else if (Mathf.Round(currentPos.x) == Mathf.Round(screenWidth / 2.0f))// 중앙에 있으면
        {
            description.alignment = TextAlignmentOptions.Midline;
            xPos = 0;
            pivotX = 0.5f;
        }
        else// 오른쪽에 있으면
        {
            description.alignment = TextAlignmentOptions.MidlineRight;
            xPos = -imageRect.rect.width / 2.0f;
            pivotX = 1;
        }

        if (currentPos.y < (screenHeight / 2.0f))// 스크린 중앙 기준 아래쪽에 있으면
        {
            yPos = imageRect.rect.height / 2.0f;
            pivotY = 0;
        }
        else if (Mathf.Round(currentPos.y) == Mathf.Round(screenHeight / 2.0f))
        {
            yPos = 0;
            pivotY = 0.5f;
        }
        else// 위쪽에 있으면
        {
            yPos = -imageRect.rect.height / 2.0f;
            pivotY = 1;
        }

        descriptionRoot.pivot = new Vector2(pivotX, pivotY);
        descriptionRoot.localPosition = new Vector3(xPos, yPos, 0);
    }
}

[Serializable]
public class TutorialPointPreset
{
    public RectTransform target;
    public string description;
    public string masterId;
    public int fontSize = 30;

    public bool interactableBehindUI = false;
    
    public TutorialPointPreset()
    {
        masterId = "000";
        fontSize = 30;
    }
}
