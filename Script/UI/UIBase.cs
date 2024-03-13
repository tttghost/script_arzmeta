
/**********************************************************************************************
 * 
 *                  UIBase.cs
 *                  
 *                      - UI를 캐싱하는 클래스
 *                      - 자식 UI(Button, Text...ect) 등을 보관 및 가져오기
 * 
 **********************************************************************************************/
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace FrameWork.UI
{

    public class UIBase : MonoBehaviour
    {
        private bool isInit = false;//1회성 이니셜라이즈용 bool값
        private bool isInitLate = false;

        protected GameObject _myGameObject = default;
        private Dictionary<string, MonoBehaviour> _dicUI = new Dictionary<string, MonoBehaviour>();       // 자식 하이라키에 있는 UI 보관
        private Dictionary<string, GameObject> _dicGO = new Dictionary<string, GameObject>();  // 자식 하이라키에 있는 GameObject 보관 : "go_" 로 시작하는 GameObject

        public bool dontSetActiveFalse = false;
        [HideInInspector] public CanvasGroup canvasGroup;
        protected string uiName;
        public Action BackAction_Custom { get; set; }

        #region Base


        protected virtual void Awake()
        {
            Initialize();
            InitializeLate();
        }

        /// <summary>
        /// Panel, Popup 이니셜라이즈
        /// </summary>
        public virtual void Initialize()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;

            uiName = gameObject.name.Replace("(Clone)", "");

            if (!GetComponent<CanvasGroup>())
            {
                gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup = GetComponent<CanvasGroup>();

            AddUI(transform);
            SetMemberUI();

            _myGameObject = gameObject;
            if (!dontSetActiveFalse)
            {
                _myGameObject.SetActive(false);
            }
        }

        protected virtual void SetMemberUI() { }


        /// <summary>
        /// Panel이나 Popup을 미리 캐스팅 하는 함수
        /// </summary>
        public void InitializeLate()
        {
            if (isInitLate)
            {
                return;
            }
            isInitLate = true;

            SetMemberUILate();
        }
        protected virtual void SetMemberUILate()
        {

        }


        protected virtual void Start() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        private void OnDestroy()
        {
            RemoveDropDownLocalizeHandler();
        }
        #endregion

        #region View
        private List<UIBase> viewList = new List<UIBase>();
        private string curViewName = string.Empty;
        private UIBase curActiveView = null;

        /// <summary>
        /// 체인지뷰
        /// </summary>
        /// <param name="changeViewName"></param>
        public UIBase ChangeView(string changeViewName = "", bool leave = false)
        {
            return ChangeView<UIBase>(changeViewName, leave);
        }
        public virtual T ChangeView<T>(bool leave = false) where T : UIBase
        {
            return ChangeView<T>(typeof(T).Name, leave);
        }
        private T ChangeView<T>(string changeViewName, bool leave = false) where T : UIBase
        {
            T t = default;
            string oldViewName = curViewName;

            for (int i = 0; i < viewList.Count; i++)
            {
                UIBase view = viewList[i];
                if (view.name == oldViewName) //올드뷰 이름일때
                {
                    if (leave)
                    {
                        continue;
                    }
                    else
                    {
                        view.gameObject.SetActive(false);
                    }
                }
                if (view.name == changeViewName) //바꿀뷰 이름일때
                {
                    view.gameObject.SetActive(true);
                    curActiveView = view;
                    view.TryGetComponent(out t);
                    curViewName = changeViewName;
                    continue;
                }

                view.gameObject.SetActive(false);

            }
            return t;
        }

        /// <summary>
        /// 뷰 가져오기
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public GameObject GetView(string viewName)
        {
            return GetView<UIBase>(viewName).gameObject;
        }
        public T GetView<T>()
        {
            return GetView<T>(typeof(T).Name);
        }
        public T GetView<T>(string viewName)
        {
            for (int i = 0; i < viewList.Count; i++)
            {
                if (viewName.ToLower() == viewList[i].name.ToLower())
                {
                    return viewList[i].GetComponent<T>();
                }
            }
            return default;
        }

        public UIBase GetActiveView()
        {
            return curActiveView;
        }
        #endregion



        #region Set UI

        /// <summary>
        /// 하위 하이라키를 탐색하면서, 미리 정해진 UI 이름과 비교해 UI Component 찾기
        /// </summary>
        /// <param name="parent"></param>
        private void AddUI(Transform parent)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                Transform child = parent.GetChild(i);
                string[] splits = child.name.Split('_');
                string preName = splits[0].ToLower();

                bool isContinue = SetUI(preName, child); //코어코드

                if (isContinue && child.childCount > 0)
                {
                    AddUI(child);
                }
            }
        }

        /// <summary>
        /// UI 딕셔너리에 셋업
        /// </summary>
        /// <param name="preName"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        protected virtual bool SetUI(string preName, Transform child)
        {
            MonoBehaviour childUI = null;
            switch (preName)
            {
                case "view":
                    childUI = child.GetComponent<UIBase>();
                    break;

                // UGUI Text
                case "txt":
                    childUI = child.GetComponent<Text>();
                    break;

                // TextMeshPro
                case "txtmp":
                    childUI = child.GetComponent<TMP_Text>();
                    break;

                // UGUI Image
                case "img":
                    childUI = child.GetComponent<Image>();
                    break;

                // UGUI Button
                case "btn":
                    childUI = child.GetComponent<Button>();
                    break;

                // UGUI Toggle
                case "tog":
                    {
                        childUI = child.GetComponent<Toggle>();
                        Transform tempTransform = child.parent;
                        if (tempTransform != null)
                        {
                            string tempName = tempTransform.name.ToLower();
                            if (tempName.Contains("togg")) //부모오브젝트에 토글그룹이 있으면
                            {
                                ToggleGroup togGroup = tempTransform.GetComponent<ToggleGroup>();
                                if (togGroup != null)
                                {
                                    Toggle toggle = (Toggle)childUI;
                                    toggle.group = togGroup;
                                }
                            }
                        }
                    }
                    break;

                // UGUI ToggleGroup
                case "togg":
                    {
                        childUI = child.GetComponent<ToggleGroup>();
                        if (childUI == null)
                        {
                            childUI = child.gameObject.AddComponent<ToggleGroup>();
                        }
                    }
                    break;

                // UGUI TogglePlus
                case "togplus":
                    {
                        childUI = child.GetComponent<TogglePlus>();
                    }
                    break;

                // UGUI Slider
                case "sld":
                case "slider":
                    childUI = child.GetComponent<Slider>();
                    break;

                // UGUI InputFielder
                case "input":
                    {
                        if (child.GetComponent<InputField>())
                        {
                            childUI = child.GetComponent<InputField>();
                        }
                        else if (child.GetComponent<TMP_InputField>())
                        {
                            childUI = child.GetComponent<TMP_InputField>();
                        }
                    }
                    break;

                // UGUI Scrollview
                case "sview":
                case "scrollview":
                    childUI = child.GetComponent<ScrollRect>();
                    break;

                // UGUI Scrollbar
                case "sbar":
                case "scrollbar":
                    childUI = child.GetComponent<Scrollbar>();
                    break;

                // GameObject
                case "go":
                    {
                        GameObject goTemp = child.gameObject;

                        if (!_dicGO.ContainsKey(child.name))
                        {
                            _dicGO.Add(child.name, goTemp);
                        }
                    }
                    break;

                // UGUI TMP_Dropdown
                case "txtmpdrop":
                case "tmpdrop":
                case "dropdown":
                    {
                        if (child.GetComponent<Dropdown>())
                        {
                            childUI = child.GetComponent<Dropdown>();
                        }
                        if (child.GetComponent<TMP_Dropdown>())
                        {
                            childUI = child.GetComponent<TMP_Dropdown>();
                        }
                    }
                    break;

            }

            if (childUI != null)
            {
                if (!_dicUI.ContainsKey(child.name))
                {
                    _dicUI.Add(child.name, childUI);
                }
                if (childUI.TryGetComponent(out UIBase viewBase))
                {
                    viewBase.Initialize();
                    viewList.Add(viewBase);
                    return false;
                }
            }
            return true;
        }
        #endregion



        #region Get UI


        //게임오브젝트
        /// <summary>
        /// 자식 하이라키에서 이름으로 GameObject 찾기
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <returns></returns>
        public GameObject GetChildGObject(string hierachyName)
        {
            GameObject goFind = default;

            if (_dicGO.TryGetValue(hierachyName, out goFind) == false)
            {
                Debug.LogWarning("GetGameObject() : 게임오브젝트 몾찾음 - " + hierachyName);
                return null;
            }

            return goFind;
        }


        //공통
        /// <summary>
        /// 자식 하이라키에서 이름으로 UGUI Object 얻기
        /// </summary>
        /// <typeparam name="T"> UGUI UI 타입지정 : Image, Button, etc...</typeparam>
        /// <param name="hierachyName"> 씬 하이라키 이름 </param>
        /// <returns></returns>
        public T GetUI<T>(string hierachyName) where T : MonoBehaviour
        {
            if (_dicUI.TryGetValue(hierachyName, out MonoBehaviour childUI) == false)
            {
                DEBUG.LOGWARNING("GetUI() : UI 몾찾음 - " + hierachyName, eColorManager.UI);
                return null;
            }

            return (T)childUI;
        }

        // 드롭다운
        private TMP_Dropdown dropdown;
        private List<MasterLocalData> masterLocalData;
        /// <summary>
        /// TMP_Dropdown 찾으면서 액션 셋업
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="valueChangedAction"></param>
        /// <returns></returns>
        public TMP_Dropdown GetUI_TMPDropdown(string hierachyName, UnityAction<int> valueChangedAction = null, List<MasterLocalData> masterLocalData = null)
        {
            dropdown = GetUI<TMP_Dropdown>(hierachyName);

            //사운드 실행
            dropdown.onValueChanged.AddListener((idx) => Single.Sound.PlayEffect(Cons.click));

            if (valueChangedAction != null)
            {
                dropdown.onValueChanged.AddListener((idx) => valueChangedAction?.Invoke(idx));
            }

            if (masterLocalData != null)
            {
                this.masterLocalData = masterLocalData;
                DropDownLocalizeHandler();
                AddDropDownLocalizeHandler();
            }

            return dropdown;
        }

        private void DropDownLocalizeHandler()
        {
            Localizing_TMPDropdownOption(dropdown, masterLocalData);
        }

        private void AddDropDownLocalizeHandler() => AppGlobalSettings.Instance.OnChangeLanguage.AddListener(DropDownLocalizeHandler);
        private void RemoveDropDownLocalizeHandler() => AppGlobalSettings.Instance.OnChangeLanguage.RemoveListener(DropDownLocalizeHandler);

        /// <summary>
        /// 드롭다운 로컬라이징 (MasterLocalData)
        /// OnEnable에서 사용
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="masterLocalData"></param>
        private void Localizing_TMPDropdownOption(TMP_Dropdown dropdown, List<MasterLocalData> masterLocalData = null)
        {
            if (masterLocalData == null || masterLocalData.Count == 0) return;

            int selectValue = dropdown.value;

            dropdown.Hide();

            dropdown.ClearOptions();
            int count = masterLocalData.Count;
            for (int i = 0; i < count; i++)
            {
                TMP_Dropdown.OptionData newData = new TMP_Dropdown.OptionData { text = Util.GetMasterLocalizing(masterLocalData[i]) };
                dropdown.options.Add(newData);
            }
            dropdown.value = selectValue;

            if (dropdown.IsExpanded)
            {
                dropdown.Show();
            }

            Util.SetMasterLocalizing(dropdown.captionText, masterLocalData[dropdown.value]);
        }

        //이미지
        /// <summary>
        /// Image 찾으면서 Sprite 셋업
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public Image GetUI_Img(string hierachyName, string str = "")
        {
            Image img = GetUI<Image>(hierachyName);
            if (str != "")
            {
                img.sprite = Single.Resources.Load<Sprite>(Cons.Path_Image + str);
            }
            return img;
        }

        /// <summary>
        /// TMP_Text 찾고 로컬라이징 마스터데이터로 설정
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="id"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public TMP_Text GetUI_TxtmpMasterLocalizing(string hierachyName, string str = "")
        {
            TMP_Text txtmp = GetUI<TMP_Text>(hierachyName);
            if (!string.IsNullOrEmpty(str))
            {
                Util.SetMasterLocalizing(txtmp, str);
            }
            return txtmp;
        }

        /// <summary>
        /// TMP_Text 찾고 로컬라이징 마스터데이터로 설정
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="masterLocalData"></param>
        /// <returns></returns>
        public TMP_Text GetUI_TxtmpMasterLocalizing(string hierachyName, MasterLocalData masterLocalData)
        {
            TMP_Text txtmp = GetUI<TMP_Text>(hierachyName);
            if (masterLocalData != null)
            {
                Util.SetMasterLocalizing(txtmp, masterLocalData);
            }
            return txtmp;
        }

        //버튼
        /// <summary>
        /// Button 찾으면서 액션 셋업
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="unityAction"></param>
        /// <returns></returns>
        public Button GetUI_Button(string hierachyName, UnityAction unityAction = null, string soundName = "")
        {
            Button btn = GetUI<Button>(hierachyName);

            if (!btn)
            {
                Debug.LogWarning($"{hierachyName} 버튼을 찾을 수 없습니다.");
                return null;
            }

            btn.onClick.RemoveAllListeners();

            //사운드 실행
            btn.onClick.AddListener(() => Single.Sound.PlayEffect(soundName ?? Cons.click));

            //버튼이벤트 있으면 실행
            if (unityAction != null)
            {
                btn.onClick.AddListener(unityAction);
            }

            return btn;
        }


        //인풋필드
        /// <summary>
        /// TMP_InputField 캐싱, submit과 placeholder 설정가능
        /// </summary>
        /// <param name="hierachyName"></param>
        /// <param name="unityAction"></param>
        /// <returns></returns>
        public TMP_InputField GetUI_TMPInputField(string hierachyName, UnityAction<string> valueChangedAction = null, UnityAction<string> submitAction = null, MasterLocalData masterLocalData = null)
        {
            TMP_InputField input = GetUI<TMP_InputField>(hierachyName);
            if (valueChangedAction != null)
            {
                input.onValueChanged.AddListener(valueChangedAction);
            }
            if (submitAction != null)
            {
                input.onSubmit.AddListener(submitAction);
            }
            if (masterLocalData != null)
            {
                Util.SetMasterLocalizing(input.placeholder, masterLocalData);
            }

            return input;
        }


        //토글
        private Dictionary<string, UnityAction> toggleOnAction = new Dictionary<string, UnityAction>();
        private Dictionary<string, UnityAction> toggleOffAction = new Dictionary<string, UnityAction>();
        private Dictionary<string, UnityAction<bool>> toggleAction = new Dictionary<string, UnityAction<bool>>();

        /// <summary>
        /// Toggle 찾으면서 체인지벨류 액션 셋업. 단, 토글이름이 겹치면 이벤트실행 에러
        /// </summary>
        /// <param name="togName"></param>
        /// <param name="toggleOnAction"></param>
        /// <returns></returns>
        public Toggle GetUI_Toggle(string togName, UnityAction toggleOnAction = null, UnityAction toggleOffAction = null)
        {
            Toggle tog = GetUI<Toggle>(togName);
            SetToggleIsOn(tog, toggleOnAction);
            SetToggleIsOff(tog, toggleOffAction);
            return tog;
        }

        /// <summary>
        /// Toggle 찾으면서 체인지벨류 액션 셋업. 단, 토글이름이 겹치면 이벤트실행 에러
        /// </summary>
        /// <param name="togName"></param>
        /// <param name="toggleOn"></param>
        /// <returns></returns>
        public Toggle GetUI_Toggle(string togName, UnityAction<bool> toggleAction)
        {
            Toggle tog = GetUI<Toggle>(togName);
            if (!this.toggleAction.ContainsKey(togName))
            {
                this.toggleAction.Add(togName, toggleAction);
            }

            tog.onValueChanged.AddListener((b) =>
            {
                if (b)
                {
                    Single.Sound.PlayEffect(Cons.click);
                    ToggleIsOn(tog);
                }
                else
                {
                    ToggleIsOff(tog);
                }
            });

            return tog;
        }

        /// <summary>
        /// 토글 셋업
        /// </summary>
        /// <param name="tog"></param>
        /// <param name="toggleOnAction"></param>
        public void SetToggleIsOn(Toggle tog, UnityAction toggleOnAction)
        {
            string hierachyName = tog.name;
            if (toggleOnAction != null)
            {
                if (!this.toggleOnAction.ContainsKey(hierachyName))
                {
                    this.toggleOnAction.Add(hierachyName, toggleOnAction);
                }
                else
                {
                    this.toggleOnAction[hierachyName] = toggleOnAction;
                }
            }
            tog.onValueChanged.AddListener((b) =>
            {
                if (b)
                {
                    Single.Sound.PlayEffect(Cons.click);
                    ToggleIsOn(tog);
                }
            });
        }

        /// <summary>
        /// 토글 셋업
        /// </summary>
        /// <param name="tog"></param>
        /// <param name="toggleOffAction"></param>
        public void SetToggleIsOff(Toggle tog, UnityAction toggleOffAction)
        {
            string hierachyName = tog.name;
            if (toggleOffAction != null)
            {
                if (!this.toggleOffAction.ContainsKey(hierachyName))
                {
                    this.toggleOffAction.Add(hierachyName, toggleOffAction);
                }
                else
                {
                    this.toggleOffAction[hierachyName] = toggleOffAction;
                }
            }

            tog.onValueChanged.AddListener((b) =>
            {
                if (!b)
                {
                    ToggleIsOff(tog);
                }
            });
        }

        /// <summary>
        /// 토글 셋업
        /// </summary>
        /// <param name="tog"></param>
        /// <param name="toggleAction"></param>
        public void SetToggleAction(Toggle tog, UnityAction<bool> toggleAction)
        {
            string hierachyName = tog.name;

            if (toggleAction != null)
            {

                if (!this.toggleAction.ContainsKey(hierachyName))
                {
                    this.toggleAction.Add(hierachyName, toggleAction);
                }
                else
                {
                    this.toggleAction[hierachyName] = toggleAction;
                }
            }

            tog.onValueChanged.AddListener((b) =>
            {
                if (b) ToggleIsOn(tog);
                else ToggleIsOff(tog);
            });
        }
        public void ToggleIsOn(string togName) => ToggleIsOnOff(togName, true);
        public void ToggleIsOn(Toggle tog) => ToggleIsOnOff(tog, true);
        public void ToggleIsOff(string togName) => ToggleIsOnOff(togName, false);
        public void ToggleIsOff(Toggle tog) => ToggleIsOnOff(tog, false);
        public void ToggleIsOnOff(string togName, bool isOn)
        {
            ToggleIsOnOff(GetUI<Toggle>(togName), isOn);
        }
        public void ToggleIsOnOff(Toggle tog, bool isOn)
        {
            string togName = tog.name;
            tog.SetIsOnWithoutNotify(isOn);
            if (isOn && toggleOnAction.ContainsKey(togName)) toggleOnAction[togName]?.Invoke();
            if (!isOn && toggleOffAction.ContainsKey(togName)) toggleOffAction[togName]?.Invoke();
            if (toggleAction.ContainsKey(togName)) toggleAction[togName]?.Invoke(isOn);
        }


        //토글플러스
        public TogglePlus GetUI_TogglePlus(string togName) => GetUI<TogglePlus>(togName);
        #endregion



        #region Callback Action
        private Action _openStartCallback = default;
        private Action _openEndCallback = default;
        private Action _closeStartCallback = default;
        private Action _closeEndCallback = default;
        /// <summary>
        /// 오픈 스타트 콜백 셋팅
        /// </summary>
        /// <param name="callback"></param>
        public UIBase SetOpenStartCallback(Action callback)
        {
            _openStartCallback = callback;
            return this;
        }

        /// <summary>
        /// 오픈 엔드 콜백 셋팅
        /// </summary>
        /// <param name="callback"></param>
        public UIBase SetOpenEndCallback(Action callback)
        {
            _openEndCallback = callback;
            return this;
        }

        /// <summary>
        /// 클로즈 스타트 콜백 셋팅
        /// </summary>
        /// <param name="callback"></param>
        public UIBase SetCloseStartCallback(Action callback)
        {
            _closeStartCallback = callback;
            return this;
        }

        /// <summary>
        /// 클로즈 엔드 콜백 셋팅
        /// </summary>
        /// <param name="callback"></param>
        public UIBase SetCloseEndCallback(Action callback)
        {
            _closeEndCallback = callback;
            return this;
        }

        /// <summary>
        /// 오픈 시작할때 액션
        /// </summary>
        public virtual void OpenStartAct()
        {
            _openStartCallback?.Invoke();
            _openStartCallback = null;
        }

        /// <summary>
        /// 오픈 끝낼때 액션
        /// </summary>
        public virtual void OpenEndAct()
        {
            _openEndCallback?.Invoke();
            _openEndCallback = null;
        }

        /// <summary>
        /// 클로즈 시작할때 액션
        /// </summary>
        public virtual void CloseStartAct()
        {
            _closeStartCallback?.Invoke();
            _closeStartCallback = null;
        }

        /// <summary>
        /// 클로즈 끝낼때 액션
        /// </summary>
        public virtual void CloseEndAct()
        {
            _closeEndCallback?.Invoke();
            _closeEndCallback = null;
        }

        /// <summary>
        /// 코루틴 오픈 스타트할때 액션
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> Co_OpenStartAct()
        {
            yield return Timing.WaitForOneFrame;
        }

        /// <summary>
        /// 코루틴 오픈 엔드할때 액션
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> Co_OpenEndAct()
        {
            yield return Timing.WaitForOneFrame;
        }

        /// <summary>
        /// 코루틴 클로즈 스타트할때 액션
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> Co_SetCloseStartAct()
        {
            yield return Timing.WaitForOneFrame;
        }

        /// <summary>
        /// 코루틴 클로즈 엔드할때 액션
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator<float> Co_SetCloseEndAct()
        {
            yield return Timing.WaitForOneFrame;
        }

        #endregion



        #region Panel,Popup

        /// <summary>
        /// 패널 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPanel<T>() where T : PanelBase
        {
            return SceneLogic.instance.GetPanel<T>();
        }

        /// <summary>
        /// 패널 쌓기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isShowAnimationNewPanel"></param>
        /// <param name="curPanelShow"></param>
        /// <param name="newPanelShow"></param>
        public void StackPanel<T>(bool isShowAnimationNewPanel = true, bool curPanelShow = false, bool newPanelShow = true) where T : PanelBase
        {
            SceneLogic.instance.StackPanel<T>(isShowAnimationNewPanel, curPanelShow, newPanelShow);
        }

        /// <summary>
        /// 패널 열기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T OpenPanel<T>() where T : PanelBase
        {
            return SceneLogic.instance.OpenPanel<T>();
        }

        /// <summary>
        /// 패널 닫기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ClosePanel<T>() where T : PanelBase
        {
            return SceneLogic.instance.ClosePanel<T>();
        }

        /// <summary>
        /// 패널 푸쉬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isShowAnimation"></param>
        /// <returns></returns>
        public T PushPanel<T>(bool isShowAnimation = true) where T : PanelBase
        {
            return SceneLogic.instance.PushPanel<T>(isShowAnimation);
        }
        /// <summary>
        /// 패널 팝
        /// </summary>
        /// <param name="cnt"></param>
        public void PopPanel(int cnt = 1)
        {
            SceneLogic.instance.PopPanel(cnt);
        }


        /// <summary>
        /// 이름, 타입명으로 팝업 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public T GetPopup<T>() where T : PopupBase
        {
            return SceneLogic.instance.GetPopup<T>();
        }



        /// <summary>
        /// 이름, 타입명으로 팝업 푸쉬
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public virtual T PushPopup<T>() where T : PopupBase
        {
            return SceneLogic.instance.PushPopup<T>();
        }
        public void PopPopup()
        {
            SceneLogic.instance.PopPopup();
        }


        #region 확장기능

        /// <summary>
        /// 팝업이 열려있는지 반환
        /// </summary>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public bool IsPopupOpen(string popupName)
        {
            return SceneLogic.instance._stackPopups.FirstOrDefault(x => x.uiName == popupName);
        }

        public bool IsPopupOpen()
        {
            if (_myGameObject == null)
                return false;

            return _myGameObject.activeSelf;
        }
        #endregion

        #region ToastPopup

        #region GetToast
        /// <summary>
        /// 타입명으로 토스트 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toastName"></param>
        /// <returns></returns>
        public T OpenToast<T>() where T : ToastBase
        {
            return SceneLogic.instance.OpenToast<T>();
        }
        #endregion

        #endregion


        /// <summary>
        /// Back 버튼
        /// </summary>
        public void Back()
        {
            Back(1);
        }

        /// <summary>
        /// Back 버튼
        /// </summary>
        /// <param name="cnt"></param>
        public virtual void Back(int cnt = 1)
        {
            SceneLogic.instance.Back(cnt);
        }
        #endregion



        #region ETC
        //public void Unload()
        //{
        //    _dicUI.Clear();
        //    _dicGameObject.Clear();
        //}
        #endregion
    }
}