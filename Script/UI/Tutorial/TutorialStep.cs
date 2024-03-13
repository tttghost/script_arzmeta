using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// 튜토리얼의 스텝 클래스입니다.
/// Tutorial Manager에 의해 관리되는 대상이며, 한 스텝은 안내 한 페이지에 해당합니다.
/// </summary>
public class TutorialStep : MonoBehaviour
{
    [SerializeField, Tooltip("튜토리얼 포인트 프리팹\n스텝 하위 첫번째 자식 트랜스폼")] private Transform tutorialPointPrefab;

    [SerializeField, Tooltip("캔버스 그룹.\n반투명 효과 줄때 사용")] private CanvasGroup canvasGroup;

    public UnityEvent onOpen = new UnityEvent();
    public UnityEvent onClose = new UnityEvent();

    [Tooltip("튜토리얼 포인트 목록")]public List<TutorialPoint> tutorialPoints = new List<TutorialPoint>();

    private bool nextStepByTargetButton = false;
    public bool enableDelayStart = false;
    [Range(0, 10)] public float startDelay = 0.25f;

    private CanvasGroup dim;

    private void Awake()
    {
        dim = transform.parent.Find("DimPanel").GetComponent<CanvasGroup>();
    }

    private void OnValidate()
    {
        if(Application.isPlaying) return;
        
        if (!tutorialPointPrefab) tutorialPointPrefab = transform.GetChild(0);
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();

        if (!Application.isPlaying)
        {
            tutorialPoints = GetComponentsInChildren<TutorialPoint>().ToList();
        }
    }

    /// <summary>
    /// TutorialStepPreset 데이터에 의거해 튜토리얼 스텝을 세팅합니다.
    /// </summary>
    /// <param name="stepPreset">스텝 데이터</param>
    /// <param name="useMaster">마스터 데이터 사용 여부</param>
    /// <param name="preventSize">사이즈 재조정 불가 여부</param>
    public void SetTutorialStep(TutorialStepPreset stepPreset, bool useMaster = false, bool preventSize = false)
    {
        tutorialPoints.Clear();

        startDelay = stepPreset.startDelay;
        enableDelayStart = stepPreset.enableDelayStart;

        foreach (var pointPreset in stepPreset.pointPresets)
        {
            var instance = Instantiate(tutorialPointPrefab, transform);
            
            var point = instance.GetComponent<TutorialPoint>();
            point.SetAnchorTarget(pointPreset.target, preventSize);// 마스크 이미지의 앵커 타겟을 지정

            if (pointPreset.interactableBehindUI)// 뒤에 UI에 상호작용해야한다면 RaycastStinger를 추가하여 타겟 UI를 지정
            {
                if (!point.TryGetComponent(out RaycastStinger stinger))
                {
                    stinger = point.gameObject.AddComponent<RaycastStinger>();
                }
            
                stinger.targetName = pointPreset.target.name.Substring(pointPreset.target.name.LastIndexOf('/') + 1);
            }

            if (useMaster)// 마스터 데이터 사용 여부 체크해서 텍스트 세팅
            {
                point.SetDescriptionFromMaster(pointPreset.masterId, pointPreset.fontSize);
            }
            else
            {
                point.SetDescription(pointPreset.description, pointPreset.fontSize);
            }

            tutorialPoints.Add(point);
        }

        if (stepPreset.nextStepByTargetButton)// 다른 버튼으로 다음 스텝을 넘겨야할 경우 별도 예외 처리
        {
            nextStepByTargetButton = stepPreset.nextStepByTargetButton;
            
            var index = stepPreset.nextStepPointIndex;
            
            var targetPoint = tutorialPoints[index];

            if (!targetPoint.TryGetComponent(out RaycastStinger stinger))
            {
                stinger = targetPoint.gameObject.AddComponent<RaycastStinger>();
            }

            // if (otherNextButtonClickEvent != null) stinger.onClick.AddListener(otherNextButtonClickEvent.Invoke);
        }

        tutorialPointPrefab.gameObject.SetActive(false); // 세팅 완료 되었으면 비활성화
    }
    
    /// <summary>
    /// TutorialPoint에 설명 텍스트를 세팅합니다.
    /// </summary>
    /// <param name="point">튜토리얼 포인트</param>
    /// <param name="pointPreset">포인트 데이터</param>
    /// <param name="useMaster">마스터 데이터 사용 여부</param>
    public void SetTutorialText(TutorialPoint point, TutorialPointPreset pointPreset, bool useMaster = false)
    {
        if (useMaster)
        {
            point.SetDescriptionFromMaster(pointPreset.masterId, pointPreset.fontSize);
        }
        else
        {
            point.SetDescription(pointPreset.description, pointPreset.fontSize);
        }
    }

    /// <summary>
    /// TutorialStepPreset 데이터에 있는 두 이벤트를 등록합니다.
    /// 해당 두 이벤트는 각각 열릴 때와 닫힐 때 호출합니다.
    /// </summary>
    /// <param name="stepPreset">스텝 데이터</param>
    public void SetEvent(TutorialStepPreset stepPreset, Action otherNextButtonClickEvent = null)
    {
        onOpen.AddListener(()=>
        {
            var target = GameObject.Find(stepPreset.onOpenData.gameObjectPath);

            if (target && !string.IsNullOrEmpty(stepPreset.onOpenData.methodName))
            {
                target.SendMessage(stepPreset.onOpenData.methodName);
            }
            
            stepPreset.onOpen.Invoke();
        });
        onClose.AddListener(()=>
        {
            var target = GameObject.Find(stepPreset.onCloseData.gameObjectPath);

            if (target && !string.IsNullOrEmpty(stepPreset.onCloseData.methodName))
            {
                target.SendMessage(stepPreset.onCloseData.methodName);
            }
            
            stepPreset.onClose.Invoke();
        });

        foreach (var point in tutorialPoints)
        {
            if (point.TryGetComponent(out RaycastStinger stinger))
            {
                if (otherNextButtonClickEvent != null) stinger.onClick.AddListener(otherNextButtonClickEvent.Invoke);
            }
        }
    }

    /// <summary>
    /// 스텝을 활성화 합니다.
    /// </summary>
    public void OpenStep()
    {
        onOpen?.Invoke();

        if (enableDelayStart)
        {
            gameObject.SetActive(true);
            StartCoroutine(DelayedOpen(startDelay));
        }
        else
        {
            dim.interactable = !nextStepByTargetButton;
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 스텝을 비활성화 합니다.
    /// </summary>
    public void CloseStep()
    {
        onClose?.Invoke();

        gameObject.SetActive(false);
        
    }

    private IEnumerator DelayedOpen(float delay)
    {
        dim.alpha = 0;
        dim.interactable = false;
        
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        
        gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);
        
        float t = 0;
        while (t < 0.5f)
        {
            var lerp = Mathf.Lerp(0f, 1f, t / 0.5f);
            
            dim.alpha = lerp;
            canvasGroup.alpha = lerp;
            yield return endOfFrame;
            t += Time.deltaTime;
        }
        
        dim.alpha = 1;
        canvasGroup.alpha = 1;
        
        canvasGroup.interactable = !nextStepByTargetButton;
        dim.interactable = !nextStepByTargetButton;
    }
    
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();

    /// <summary>
    /// onClose 이벤트 호출 없이 스텝을 비활성화 합니다.
    /// </summary>
    public void CloseStepWithNoEvent()
    {
        gameObject.SetActive(false);
    }
}

[Serializable]
public class TutorialStepPreset
{
    [NonReorderable] public List<TutorialPointPreset> pointPresets = new List<TutorialPointPreset>();

    public UnityEvent onOpen = new UnityEvent();
    public UnityEvent onClose = new UnityEvent();

    public TutorialStepEventData onOpenData = new TutorialStepEventData();
    public TutorialStepEventData onCloseData = new TutorialStepEventData();

    public bool enableDelayStart = false;
    [FormerlySerializedAs("delayTime")] [Range(0, 10)] public float startDelay = 0.5f;
    
    public bool nextStepByTargetButton = false;

    public int nextStepPointIndex = 0;
}

[Serializable]
public class TutorialStepEventData
{
    public string gameObjectPath;
    public string methodName;
} 
