using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// 튜토리얼 매니저입니다.
/// 인스펙터에서 세팅한 TutorialStepPreset 리스트 데이터를 기반으로 튜토리얼을 생성합니다.
/// 에디터에서 튜토리얼을 미리 생성하여 프리팹으로 저장한 후 사용 바랍니다.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField, Tooltip("스텝 프리셋 데이터 리스트"), NonReorderable] private List<TutorialStepPreset> stepPresets = new List<TutorialStepPreset>();
    [SerializeField, ReadOnly, Tooltip("생성된 스텝 리스트")] private List<TutorialStep> tutorialStepList = new List<TutorialStep>();

    [SerializeField, Tooltip("스텝 프리팹")] private Transform tutorialStepPrefab;
    [SerializeField, ReadOnly] private Transform content;
    [SerializeField, ReadOnly] private Transform dimPanel;
    [SerializeField, ReadOnly] private Button btn_Skip;

    private TutorialStep currentStep;
    private Queue<TutorialStep> tutorialStepQueue = new Queue<TutorialStep>();

    [SerializeField, Tooltip("완료 상태 PlayerPrefs 저장 여부")] private bool saveCompletedState = true;

    [SerializeField, Tooltip("마스터 데이터 사용 여부")] private bool useMasterString;

    [SerializeField, Tooltip("Start()시 튜토리얼 실행 여부")]
    private bool openThisOnStart = true;

    [FormerlySerializedAs("destroyWhenClose")] [SerializeField, Tooltip("튜토리얼 종료시 게임오브젝트 삭제 여부")]
    private bool destroyOnClose = true;

    [SerializeField, Tooltip("튜토리얼 종료시 종료 안내 팝업 여부")]
    private bool showClosePopup = true;

    private Canvas canvas;

    private void Awake()
    {
        if (HaveSeenTutorial())
        {
            Close();
        }
    }

    private async void Start()
    {
        await Init();

        content.gameObject.SetActive(false);

        if (openThisOnStart)
        {
            if (showClosePopup)
            {
                if (SceneLogic.instance)
                {
                    SceneLogic.instance.PushPopup<Popup_Basic>()
                        .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, new MasterLocalData("common_tutorial01_1"), new MasterLocalData("common_tutorial01_2")))
                        .ChainPopupAction(new PopupAction(() =>
                        {
                            // 진행 시작
                            content.gameObject.SetActive(true);
                            NextStep();
                        }, async () =>
                        {
                            // 진행 취소
                            SaveState();
                            await Task.Delay(200);

                            Close();
                        }));
                }
                else
                {
                    await UniTask.NextFrame();
                    content.gameObject.SetActive(true);
                    NextStep();
                }
            }
            else
            {
                await UniTask.NextFrame();
                content.gameObject.SetActive(true);
                NextStep();
            }
        }
        
        RefreshLocalizeString();
    }

    private void OnValidate()
    {
        if (!tutorialStepPrefab) tutorialStepPrefab = GetComponentInChildren<TutorialStep>(true).transform;
        if (!content) content = transform.GetChild(0);
        if (!dimPanel) dimPanel = content.GetChild(0);
        if (!btn_Skip) btn_Skip = transform.Search<Button>("btn_Skip");
    }
    
    /// <summary>
    /// 튜토리얼을 초기화 합니다.
    /// </summary>
    private async Task Init()
    {
        canvas = GetComponent<Canvas>();
        
        tutorialStepQueue.Clear();
        
        for (var i = 0; i < tutorialStepList.Count; i++)
        {
            var step = tutorialStepList[i];
            var stepPreset = stepPresets[i];
            step.SetEvent(stepPreset, NextStep);

            tutorialStepQueue.Enqueue(step);
            
            step.CloseStepWithNoEvent();
        }
        
        var processButton = dimPanel.GetComponent<Button>();
        
        processButton.onClick.AddListener(NextStep);

        btn_Skip.onClick.AddListener(Skip);
        
        if (!SceneLogic.instance) NextStep();

        await UniTask.NextFrame();
    }
    
    /// <summary>
    /// 스텝 프리셋 데이터(stepPresets)를 기반으로 튜토리얼을 세팅합니다.
    /// </summary>
    public void SetData()
    {
        if(stepPresets == null) return;

        GameObject cameraGo = null;
        
        if (!Camera.main)
        {
            Debug.LogWarning("마스크 위치 계산에 카메라를 사용합니다. 현재 Scene에 메인 카메라를 추가합니다.");
            cameraGo = new GameObject("Main Camera");
            cameraGo.transform.SetParent(this.transform);
            cameraGo.AddComponent<Camera>();
            cameraGo.tag = "MainCamera";
            //return;
        }

        foreach (var tutorialStep in tutorialStepList)
        {
            tutorialStep.gameObject.SetActive(false);
        }
        
        tutorialStepPrefab.gameObject.SetActive(true);
        
        tutorialStepList.Clear();
        
        foreach (var stepPreset in stepPresets)
        {
            var instance = Instantiate(tutorialStepPrefab, content);
            var step = instance.GetComponent<TutorialStep>();

            step.SetTutorialStep(stepPreset, useMasterString);

            tutorialStepList.Add(step);
        }

        tutorialStepPrefab.gameObject.SetActive(false);

        if (cameraGo)
        {
            Debug.LogWarning("임시로 생성한 메인 카메라를 Destroy합니다.");
            DestroyImmediate(cameraGo);
        }
    }

    /// <summary>
    /// 튜토리얼 설명 텍스트를 갱신합니다.
    /// </summary>
    public void RefreshLocalizeString()
    {
        for (var i = 0; i < tutorialStepList.Count; i++)
        {
            var step = tutorialStepList[i];
            var stepPreset = stepPresets[i];

            for (var index = 0; index < step.tutorialPoints.Count; index++)
            {
                step.SetTutorialText(step.tutorialPoints[index], stepPreset.pointPresets[index], useMasterString);
            }
        }
    }

    /// <summary>
    /// 다음 스텝으로 넘깁니다.
    /// </summary>
    public void NextStep()
    {
        Single.Sound.ClickPanel();
        
        if (tutorialStepQueue.Count == 0 || tutorialStepQueue == null)
        {
            currentStep.CloseStep();
            
            dimPanel.gameObject.SetActive(false);

            if (showClosePopup)
            {
                if (SceneLogic.instance)
                {
                    ShowCompletedPopup();
                }
                else
                {
                    Close();
                }
            }
            else
            {
                Close();
            }

            return;
        }
        
        if(currentStep != null) currentStep.CloseStep();
        
        var nextStep = tutorialStepQueue.Dequeue();
        
        nextStep.OpenStep();

        currentStep = nextStep;
    }

    public void Skip()
    {
        if (showClosePopup)
        {
            DisableCurrentStep();
            
            SceneLogic.instance.PushPopup<Popup_Basic>()
                .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.ConfirmCancel, new MasterLocalData("common_tutorial01_1"), new MasterLocalData("office_confirm_skiptutorial")))
                .ChainPopupAction(new PopupAction(async () =>
                 {
                     await Task.Delay(700);
                     ShowCompletedPopup();
                 }, 
                 EnableCurrentStep));
        }
        else
        {
            Close();
        }
    }

    public void EnableCurrentStep()
    {
        SetOrder(101);
        currentStep.gameObject.SetActive(true);
    }
    
    public void DisableCurrentStep()
    {
        SetOrder(99);
        currentStep.gameObject.SetActive(false);
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
    }
    
    public void Close()
    {
        if (destroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SetOrder(int order)
    {
        if (!canvas) canvas = GetComponent<Canvas>();

        canvas.sortingOrder = order;
    }

    public void ShowCompletedPopup()
    {
        SetOrder(99);

        SceneLogic.instance.PushPopup<Popup_Basic>()
            .ChainPopupData(new PopupData(POPUPICON.NONE, BTN_TYPE.Confirm, new MasterLocalData("common_tutorial01_1"), new MasterLocalData("common_tutorial01_9")))
            .ChainPopupAction(new PopupAction(async () =>
            {
                if (saveCompletedState)
                {
                    SaveState();
                }

                await Task.Delay(200);
                Close();
            }));
    }
    
    /// <summary>
    /// 튜토리얼 PlayerPrefs를 삭제합니다.
    /// </summary>
    public void DeleteTutorialSaveData()
    {
        Debug.Log($"Sav_{gameObject.name}_{LocalPlayerData.MemberCode} 키 삭제");
        PlayerPrefs.DeleteKey($"Sav_{gameObject.name}_{LocalPlayerData.MemberCode}");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 튜토리얼 PlayerPrefs를 생성합니다.
    /// </summary>
    public void SaveState()
    {
        if (!saveCompletedState) return;
        
        PlayerPrefs.SetInt($"Sav_{gameObject.name}_{LocalPlayerData.MemberCode}", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 튜토리얼 진행 여부를 체크합니다.
    /// </summary>
    /// <returns></returns>
    public bool HaveSeenTutorial()
    {
        return PlayerPrefs.GetInt($"Sav_{gameObject.name}_{LocalPlayerData.MemberCode}", 0) != 0;
    }
}
