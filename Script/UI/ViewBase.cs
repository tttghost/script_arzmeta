#region 220810버전
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.Localization.Components;
//using UnityEngine.UI;


//namespace FrameWork.UI
//{
//    public class ViewBase : MonoBehaviour
//    {
//        protected virtual void Awake()
//        {
//            if (!isInit)
//            {
//                Initialize();
//            }
//        }

//        #region Module

//        public void Initialize()
//        {
//            isInit = true;
//            _myTransform = transform;
//            _myGameObject = gameObject;
//            AddUI(_myTransform);
//            SetMemberUI();

//            //if(_myGameObject != null)
//            //{
//            //    _myGameObject.SetActive(false);
//            //}
//        }

//        protected virtual void Start() { }
//        protected Transform _myTransform = default;
//        protected GameObject _myGameObject = default;

//        // 자식 하이라키에 있는 UI 보관
//        private Dictionary<string, MonoBehaviour> _dicUI = new Dictionary<string, MonoBehaviour>();

//        // 자식 하이라키에 있는 GameObject 보관 : "go_" 로 시작하는 GameObject
//        private Dictionary<string, GameObject> _dicGameObject = new Dictionary<string, GameObject>();
//        private bool isInit = false;

//        public virtual void SetMemberUI() { }
//        private void AddUI(Transform parent)
//        {
//            Transform child = null;
//            for (int i = 0; i < parent.childCount; ++i)
//            {
//                child = parent.GetChild(i);
//                string[] splits = child.name.Split('_');

//                MonoBehaviour childUI = null;

//                switch (splits[0].ToLower())
//                {
//                    // UGUI View
//                    case "view":
//                        childUI = child.GetComponent<ViewBase>();
//                        break;

//                    // UGUI Text
//                    case "txt":
//                        childUI = child.GetComponent<Text>();
//                        break;

//                    // TextMeshPro
//                    case "txtmp":
//                        childUI = child.GetComponent<TMP_Text>();
//                        break;

//                    // UGUI Image
//                    case "img":
//                        childUI = child.GetComponent<Image>();
//                        break;

//                    // UGUI Button
//                    case "btn":
//                        childUI = child.GetComponent<Button>();
//                        break;

//                    // UGUI Toggle
//                    case "tog":
//                        {
//                            childUI = child.GetComponent<Toggle>();
//                            Transform tempTransform = child.transform.parent;
//                            if (tempTransform != null)
//                            {
//                                string tempName = tempTransform.name.ToLower();
//                                if (tempName.Contains("togg"))
//                                {
//                                    ToggleGroup togGroup = tempTransform.GetComponent<ToggleGroup>();
//                                    if (togGroup != null)
//                                    {
//                                        Toggle toggle = (Toggle)childUI;
//                                        toggle.group = togGroup;
//                                    }
//                                }
//                            }
//                        }
//                        break;

//                    // UGUI ToggleGroup
//                    case "togg":
//                        {
//                            childUI = child.GetComponent<ToggleGroup>();
//                            if (childUI == null)
//                            {
//                                childUI = child.gameObject.AddComponent<ToggleGroup>();
//                            }
//                        }
//                        break;

//                    // UGUI Slider
//                    case "sld":
//                        childUI = child.GetComponent<Slider>();
//                        break;

//                    // UGUI InputFielder
//                    case "input":
//                        {
//                            if (child.GetComponent<InputField>())
//                            {
//                                childUI = child.GetComponent<InputField>();
//                            }
//                            else if (child.GetComponent<TMP_InputField>())
//                            {
//                                childUI = child.GetComponent<TMP_InputField>();
//                            }
//                        }
//                        break;

//                    // UGUI Scrollbar
//                    case "scrollbar":
//                        childUI = child.GetComponent<Scrollbar>();
//                        break;

//                    // GameObject
//                    case "go":
//                        {
//                            GameObject goTemp = child.gameObject;

//                            if (!_dicGameObject.ContainsKey(child.name))
//                            {
//                                _dicGameObject.Add(child.name, goTemp);
//                            }
//                        }
//                        break;

//                }

//                if (childUI != null)
//                {
//                    if (!_dicUI.ContainsKey(child.name))
//                    {
//                        _dicUI.Add(child.name, childUI);
//                    }
//                    if (childUI.TryGetComponent(out ViewBase viewBase))
//                    {
//                        DEBUG.LOG("viewBase.name : " + viewBase.name);
//                        viewBase.Initialize();
//                        viewList.Add(viewBase.gameObject);
//                        continue;
//                    }
//                }

//                if (child.childCount > 0) //자식이 있으면 재귀해서 서칭
//                {
//                    AddUI(child);
//                }
//            }
//        }

//        private List<GameObject> viewList = new List<GameObject>();

//        #region GetUI

//        /// <summary>
//        /// 자식 하이라키에서 이름으로 GameObject 찾기
//        /// </summary>
//        /// <param name="hierachyName"></param>
//        /// <returns></returns>
//        public GameObject GetChildGObject(string hierachyName)
//        {
//            GameObject goFind = default;

//            if (_dicGameObject.TryGetValue(hierachyName, out goFind) == false)
//            {
//                Debug.LogError("PanelBase.GetGameObject() : 게임오브젝트 몾찾음 - " + hierachyName);
//                return null;
//            }

//            return goFind;
//        }


//        /// <summary>
//        /// 자식 하이라키에서 이름으로 UGUI Object 얻기
//        /// </summary>
//        /// <typeparam name="T"> UGUI UI 타입지정 : Image, Button, etc...</typeparam>
//        /// <param name="hierachyName"> 씬 하이라키 이름 </param>
//        /// <returns></returns>
//        public T GetUI<T>(string hierachyName) where T : MonoBehaviour
//        {
//            MonoBehaviour childUI = default;

//            if (_dicUI.TryGetValue(hierachyName, out childUI) == false)
//            {
//                DEBUG.LOG("PanelBase PanelBase.GetUI() : UI 몾찾음 - " + hierachyName, eColorManager.UI);
//                return null;
//            }

//            return (T)childUI;
//        }

//        public Image GetUI_Img(string hierachyName, string str = "")
//        {
//            Image img = GetUI<Image>(hierachyName);
//            img.sprite = Single.Resources.LoadAD<Sprite>(Cons.Path_Image + str);
//            return img;
//        }

//        public TMP_Text GetUI_Txtmp(string hierachyName, string str = "")
//        {
//            TMP_Text txtmp = GetUI<TMP_Text>(hierachyName);
//            txtmp.LocalText(str);
//            return txtmp;
//        }

//        /// <summary>
//        /// TMP_Text를 로드함과 동시에 로컬라이제이션 셋업
//        /// </summary>
//        /// <param name="hierachyName"></param>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public TMP_Text GetUI_Txtmp(string hierachyName, string table, string entry, LocalData localData = null)
//        {
//            TMP_Text txtmp = GetUI<TMP_Text>(hierachyName);
//            txtmp.LocalText(table, entry, localData);
//            return txtmp;
//        }


//        public TMP_Text GetUI_Txtmp(string hierachyName,  LocalData localData)
//        {
//            TMP_Text txtmp = GetUI<TMP_Text>(hierachyName);
//            txtmp.LocalText(localData);
//            return txtmp;
//        }


//        public Text GetUI_Txt(string hierachyName, string str = "")
//        {
//            Text txt = GetUI<Text>(hierachyName);
//            txt.LocalText(str);
//            return txt;
//        }

//        /// <summary>
//        /// Text를 로드함과 동시에 로컬라이제이션 셋업
//        /// </summary>
//        /// <param name="hierachyName"></param>
//        /// <param name="data"></param>
//        /// <returns></returns>
//        public Text GetUI_Txt(string hierachyName, string table, string entry, LocalData localData = null)
//        {
//            Text txt = GetUI<Text>(hierachyName);
//            txt.LocalText(table, entry, localData);
//            return txt;
//        }

//        public Text GetUI_Txt(string hierachyName, LocalData localData)
//        {
//            Text txt = GetUI<Text>(hierachyName);
//            txt.LocalText(localData);
//            return txt;
//        }

//        /// <summary>
//        /// Button을 로드함과 동시에 액션 셋업
//        /// </summary>
//        /// <param name="hierachyName"></param>
//        /// <param name="unityAction"></param>
//        /// <returns></returns>
//        public Button GetUI_Button(string hierachyName, UnityAction unityAction = null)
//        {
//            Button btn = GetUI<Button>(hierachyName);
//            if (unityAction != null)
//            {
//                btn.onClick.AddListener(() => Single.Sound.PlayEffect(Cons.click));
//                btn.onClick.AddListener(unityAction);
//            }
//            return btn;
//        }

//        /// <summary>
//        /// TMP_InputField 로드함과 동시에 액션, 로컬라이제이션 셋업
//        /// </summary>
//        /// <param name="hierachyName"></param>
//        /// <param name="unityAction"></param>
//        /// <returns></returns>
//        public TMP_InputField GetUI_TMPInputField(string hierachyName, UnityAction<string> valueChangedAction = null, UnityAction<string> submitAction = null)
//        {
//            TMP_InputField input = GetUI<TMP_InputField>(hierachyName);
//            if (valueChangedAction != null)
//            {
//                input.onValueChanged.AddListener(valueChangedAction);
//            }
//            if (submitAction != null)
//            {
//                input.onSubmit.AddListener(submitAction);
//            }
//            return input;
//        }


//        public Toggle GetUI_Toggle(string hierachyName, UnityAction unityAction = null)
//        {
//            Toggle tog = GetUI<Toggle>(hierachyName);
//            if (unityAction != null)
//            {
//                tog.onValueChanged.AddListener((b) => { if(b) Single.Sound.PlayEffect(Cons.click); });
//                tog.onValueChanged.AddListener((b) =>{ if (b) unityAction(); });
//            }
//            return tog;
//        }

//        #endregion
//        #endregion


//    }
//}
#endregion


/**********************************************************************************************
 * 
 *                  ViewBase.cs
 *                  
 *                      - UI Base Class
 *                      - 자식 UI(Button, Text...ect) 등을 보관 및 가져오기
 * 
 **********************************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UI
{
    public class ViewBase : UIBase
    {
        //protected List<GameObject> viewList = new List<GameObject>();

        ///// <summary>
        ///// 현재 뷰 이름
        ///// </summary>
        ///// <returns></returns>
        //public string GetCurrentViewName()
        //{
        //    string viewName = string.Empty;
        //    for (int i = 0; i < viewList.Count; i++)
        //    {
        //        GameObject view = viewList[i];
        //        if (view.activeSelf == true)
        //        {
        //            viewName = view.name;
        //        }
        //    }
        //    return viewName;
        //}

        ///// <summary>
        ///// 체인지뷰
        ///// </summary>
        ///// <param name="viewName"></param>
        //public virtual void ChangeView(string viewName = "")
        //{
        //    for (int i = 0; i < viewList.Count; i++)
        //    {
        //        GameObject view = viewList[i];
        //        view.SetActive(viewName == view.name ? true : false);
        //    }
        //}


        ///// <summary>
        ///// 뷰 가져오기
        ///// </summary>
        ///// <param name="viewName"></param>
        ///// <returns></returns>
        //public UIBase GetView(string viewName)
        //{
        //    for (int i = 0; i < viewList.Count; i++)
        //    {
        //        if (viewName == viewList[i].name)
        //        {
        //            return viewList[i].GetComponent<UIBase>();
        //        }
        //    }
        //    return null;
        //}
        //protected override bool SetUI(string preName, Transform child)
        //{
        //    base.SetUI(preName, child);

        //    bool isContinue = true;
        //    Debug.Log("preName: " + preName);
        //    MonoBehaviour childUI = null;
        //    // UGUI View
        //    switch (preName)
        //    {
        //        case "view":
        //            childUI = child.GetComponent<UIBase>();
        //            break;
        //        default:
        //            break;
        //    }
        //    if (childUI != null)
        //    {
        //        if (!_dicUI.ContainsKey(child.name))
        //        {
        //            _dicUI.Add(child.name, childUI);
        //        }
        //        if (childUI.TryGetComponent(out UIBase viewBase))
        //        {
        //            isContinue = false;
        //            viewBase.Initialize();
        //            viewList.Add(viewBase.gameObject);
        //        }
        //    }
        //    return isContinue;
        //}
    }
}