using FrameWork.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionCustomObj : MonoBehaviour
{
    private GameObject go_Parent; //기준 오브젝트

    private InteractionCustom interaction; //데이터


    //터치 인터렉터블
    private TouchInteractable TouchInteractable;
    
    //활성화 영역
    private InteractionArea InteractionArea;


    [SerializeField] private float interactionAreaDistance = 4f;

    private bool initHandler = false;

    private void Awake()
    {
        TouchInteractable = GetComponentInChildren<TouchInteractable>(true);
        InteractionArea = GetComponentInChildren<InteractionArea>(true);
    }
    public void SetData(InteractionCustom interaction)
    {
        this.interaction = interaction;

        InitData();

        AddHandler();

        OnInteractableArea(false);
    }
    private void InitData()
    {
        go_Parent = Util.Search(transform, nameof(go_Parent)).gameObject;

        TouchInteractable.transform.localPosition = interaction.localPosition_Clicker;
        TouchInteractable.interactDistance = interactionAreaDistance;
        TouchInteractable.AddEvent(Sit);

        InteractionArea.transform.rotation = Quaternion.identity;
    }
    private void AddHandler()
    {
        initHandler = true;
        if (!InteractionCustomManager.Instance)
        {
            return;
        }

        InteractionCustomManager.Instance.handlerInteraction += OnInteraction;
        InteractionCustomManager.Instance.handlerMyRoomModeChange += OnMyRoomModeChange;
        
        InteractionArea._ontriggerEnter.AddListener((col) => OnInteractableArea(true));
        InteractionArea._ontriggerExit.AddListener((col) => OnInteractableArea(false));
    }
    private void OnDestroy()
    {
        RemoveHandler();
    }
    private void RemoveHandler()
    {
        if(!initHandler)
        {
            return;
        }
        if(!InteractionCustomManager.Instance)
        {
            return;
        }

        InteractionCustomManager.Instance.handlerInteraction -= OnInteraction;
        InteractionCustomManager.Instance.handlerMyRoomModeChange -= OnMyRoomModeChange;
        
        InteractionArea._ontriggerEnter.RemoveListener((col) => OnInteractableArea(true));
        InteractionArea._ontriggerExit.RemoveListener((col) => OnInteractableArea(false));
    }


    public void Sit()
    {
        InteractionCustomManager.Instance.interaction = interaction;

        InteractionCustomManager.Instance.target = transform;

        InteractionCustomManager.Instance.StartInteraction();
    }

    public void Stand()
    {
        InteractionCustomManager.Instance.EndInteraction();
    }


    /// <summary>
    /// 인터렉터블 영역 enter/exit 콜백 -> 인터렉션UI 활성/비활성
    /// </summary>
    /// <param name="enable"></param>
    public void OnInteractableArea(bool enable)
    {
        TouchInteractable.gameObject.SetActive(enable);
    }

    /// <summary>
    /// 인터렉션버튼 클릭시 콜백 -> 인터렉션요소 활성/비활성 (재사용)
    /// </summary>
    public void OnInteraction(bool enable)
    {
        OnMyRoomModeChange(enable ? eMyRoomMode.PLAYMODE : eMyRoomMode.EDITMODE);
    }

    /// <summary>
    /// 마이룸모드 체인지 콜백 -> 인터렉션요소 활성/비활성 
    /// </summary>
    /// <param name="myRoomMode"></param>
    private void OnMyRoomModeChange(eMyRoomMode myRoomMode)
    {
        switch (myRoomMode)
        {
            case eMyRoomMode.PLAYMODE:
                go_Parent.SetActive(true);
                break;
            case eMyRoomMode.EDITMODE:
                go_Parent.SetActive(false);
                OnInteractableArea(false);
                break;
            default:
                break;
        }
    }
}
