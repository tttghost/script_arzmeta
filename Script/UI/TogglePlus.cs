using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle), typeof(Image))]
public class TogglePlus : MonoBehaviour
{
    #region 변수
    private Action onAction;
    private Action offAction;
    private Action<bool> boolAction;
    public GameObject go_On;
    public GameObject go_Off;
    public Toggle tog;
    public bool Interactable
    {
        get => tog.interactable;
        set => tog.interactable = value;
    }
    public Image img;
    [HideInInspector] public bool isSound = true;
    #endregion

    #region 함수
    #region 초기화
    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        Init();
        SetOnOff(GetToggleIsOn());
        tog.onValueChanged.AddListener((isOn) =>
        {
            if(isSound && (!tog.group || isOn))
            {
                Single.Sound.PlayEffect(Cons.click);
            }
            isSound = true;
            SetOnOff(isOn);
            ToggleActions(isOn);
        });
    }

    /// <summary>
    /// 인스펙터 값 변경시 호출 초기값 셋팅
    /// </summary>
    private async void OnValidate()
    {
        if (Application.isPlaying) return;
        await CreateObj(nameof(go_On));
        await CreateObj(nameof(go_Off));

        Init();

        //DestroyImmediate(this);
    }

    /// <summary>
    /// 초기값 셋팅
    /// </summary>
    private void Init()
    {
        tog = GetComponent<Toggle>();
        img = GetComponent<Image>();

        tog.targetGraphic = img;
        tog.toggleTransition = Toggle.ToggleTransition.None;
        img.color = Color.clear;

        go_On = Util.Search(gameObject, nameof(go_On)).gameObject;
        go_Off = Util.Search(gameObject, nameof(go_Off)).gameObject;

    }

    /// <summary>
    /// go_On, go_Off 생성
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    private  async Task<GameObject> CreateObj(string objName)
    {
        GameObject newObj;
        Transform tr = transform.Find(objName);
        if (!tr)
        {
            await Task.Delay(1);
            newObj = new GameObject(objName);
            newObj.transform.SetParent(transform);
            newObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            newObj = tr.gameObject;
        }
        return newObj;
    }
    #endregion


    #region 토글상태 변경, 토글그룹

    /// <summary>
    /// 토글그룹 가져오기
    /// </summary>
    /// <returns></returns>
    public ToggleGroup GetToggleGroup()
    {
        return tog.group;
    }
    /// <summary>
    /// 토글그룹 셋팅
    /// </summary>
    /// <param name="togg"></param>
    public void SetToggleGroup(ToggleGroup togg)
    {
        tog.group = togg;
    }

    /// <summary>
    /// 토글 상태 가져오기
    /// </summary>
    /// <returns></returns>
    public bool GetToggleIsOn()
    {
        return tog.isOn;
    }

    /// <summary>
    /// 토글 상태 변경
    /// </summary>
    /// <param name="isOn"></param>
    public TogglePlus SetToggleIsOn(bool isOn, bool isSound = false)
    {
        this.isSound = isSound;
        if (tog.isOn == isOn)
        {
            tog.onValueChanged?.Invoke(isOn);
        }
        tog.isOn = isOn;
        return this;
    }

    /// <summary>
    /// 토글 상태 변경(액션 실행 x)
    /// </summary>
    /// <param name="isOn"></param>
    public TogglePlus SetToggleIsOnWithoutNotify(bool isOn)
    {
        SetOnOff(isOn);
        tog.SetIsOnWithoutNotify(isOn);
        return this;
    }

    /// <summary>
    /// 토글 callback만 실행
    /// </summary>
    /// <param name="isOn"></param>
    //public void SetToggleIsOnWithoutValueChange(bool isOn)
    //{
    //    tog.onValueChanged?.Invoke(isOn);
    //    if(isOn)onAction?.Invoke();
    //    else offAction?.Invoke();
    //}

    /// <summary>
    /// 토글 액션 모음
    /// </summary>
    /// <param name="isOn"></param>
    private void ToggleActions(bool isOn)
    {
        if (isOn) onAction?.Invoke();
        else offAction?.Invoke();
        boolAction?.Invoke(isOn);
    }
    #endregion


    #region 토글액션 - 입맛대로 넣어 쓰세요
    public TogglePlus SetToggleAction(Action onAction, Action offAction)
    {
        SetToggleOnAction(onAction);
        SetToggleOffAction(offAction);
        return this;
    }
    public TogglePlus SetToggleOnAction(Action onAction)
    {
        this.onAction += onAction;
        return this;
    }
    public TogglePlus SetToggleOffAction(Action offAction)
    {
        this.offAction += offAction;
        return this;
    }
    public TogglePlus SetToggleAction(Action<bool> boolAction)
    {
        this.boolAction += boolAction;
        return this;
    }

    /// <summary>
    /// 온오프 오브젝트
    /// </summary>
    /// <param name="active"></param>
    private void SetOnOff(bool active)
    {
        go_On.SetActive(active);
        go_Off.SetActive(!active);
    }
    #endregion
    #endregion
}
