using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class WebviewShortcutPage : MonoBehaviour
{
    [SerializeField] private CanvasWebViewPrefab webViewPrefab;

    [SerializeField] private WebviewController webviewController;

    [SerializeField] private Popup_AddShortcut popup_AddShortcut;
    
    [SerializeField] private ShortcutItem itemPrefab;

    [SerializeField] private List<ShortcutItem> itemList = new List<ShortcutItem>();

    public Transform scrollContent;

    //[SerializeField] private Button button_ActiveAddHome;
    //[SerializeField] private Button button_ActiveAddShortcut;
    //[SerializeField] private Button button_ActiveDelete;
    [SerializeField] private Toggle toggle_ActiveAddHome;
    [SerializeField] private Toggle toggle_ActiveAddShortcut;
    [SerializeField] private Toggle toggle_ActiveDelete;

    [SerializeField] private Transform navi_ActiveDelete;
    [SerializeField] private Transform navi_ActiveAddHome;

    [SerializeField] private CanvasGroup group;

    private BrowserData browserData;

    private bool editMode = false;
    private bool selectMode = false;

    private bool shortcutIsOpened = true;
    
    private List<Texture2D> tempTex = new List<Texture2D>();
    
    private void OnValidate()
    {
        if (!webViewPrefab) webViewPrefab = transform.parent.GetComponentInChildren<CanvasWebViewPrefab>();
        if (!itemPrefab) itemPrefab = GetComponentInChildren<ShortcutItem>();
        
        if(!webviewController) webviewController = webViewPrefab.GetComponent<WebviewController>();

        // if(!button_ActiveAddHome) button_ActiveAddHome = transform.Search("Button_ActiveAddHome").GetComponent<Button>();
        // if(!button_ActiveAddShortcut) button_ActiveAddShortcut = transform.Search("Button_ActiveAddShortcut").GetComponent<Button>();
        // if(!button_ActiveDelete) button_ActiveDelete = transform.Search("Button_ActiveDelete").GetComponent<Button>();
        if(!toggle_ActiveAddHome) toggle_ActiveAddHome = transform.Search("Button_ActiveAddHome").GetComponent<Toggle>();
        if(!toggle_ActiveAddShortcut) toggle_ActiveAddShortcut = transform.Search("Button_ActiveAddShortcut").GetComponent<Toggle>();
        if(!toggle_ActiveDelete) toggle_ActiveDelete = transform.Search("Button_ActiveDelete").GetComponent<Toggle>();

        if (!group) group = GetComponent<CanvasGroup>();

        if (!popup_AddShortcut) popup_AddShortcut = FindObjectOfType<Popup_AddShortcut>(true);

        scrollContent = GetComponentInChildren<ScrollRect>().content;
    }

    private string dataPath => Application.persistentDataPath + "/browserData.bd";

    private async void Awake()
    {
        if (File.Exists(dataPath))
        {
            await LoadData();
        }
        else
        {
            await SetDefaultBrowserData();
        }

        webviewController.SetHomeURL(browserData.homeURL);
        
        SaveData();
        
        InitShortCutItem();
    }

    private async void Start()
    {
        AddButtonsHandler();

        await SetWebviewOption();

        AddToolBarButtonsHandler();

        popup_AddShortcut.onConfirm.AddListener(CreateShortCutItem);

        var panel_webviewSharing = FindObjectOfType<Panel_WebviewSharing>();

        panel_webviewSharing.onClose.AddListener(DisableMode);

        var home = FindHomeItem();
        if (home) home.SetHomeMark(true);

        OpenShortcut(true);
    }

    private void OnDisable()
    {
        SetDeleteMode(false);
        SetAddHomeMode(false);
        SaveData();
    }

    private void OnDestroy()
    {
        foreach (var tex in tempTex)
        {
            Destroy(tex);
        }
    }

    private void OnApplicationPause()
    {
        SaveData();
    }

    private void SaveData()
    {
        var json = JsonUtility.ToJson(browserData);
        File.WriteAllText(dataPath, json.Compress());
    }

    private async Task LoadData()
    {
        var stream = File.ReadAllText(dataPath, Encoding.UTF8);
        browserData = JsonUtility.FromJson<BrowserData>(stream.Decompress());

        foreach (var shortcut in browserData.shortcutDataList)
        {
            var replaced = string.Join("_", shortcut.title.Split(Path.GetInvalidFileNameChars()));
            var path = $"{Application.persistentDataPath}/Bookmark/{replaced}/thumbnail.png";
            if (File.Exists(path))
            {
                shortcut.favicon = ImageDownloadUtility.LoadPngToTexture("thumbnail", $"/Bookmark/{replaced}");
                tempTex.Add(shortcut.favicon);
            }
            else
            {
                await ImageDownloadUtility.DownloadTextureAsync(
                    $"http://www.google.com/s2/favicons?sz=256&domain={shortcut.url}",
                    receive =>
                    {
                        shortcut.favicon = receive as Texture2D;
                        tempTex.Add(shortcut.favicon);
                    });
                
                if (shortcut.favicon)
                {
                    ImageDownloadUtility.SaveTextureToPNG(shortcut.favicon, "thumbnail", $"/Bookmark/{replaced}");
                }
            }
        }
    }
    
    public void OpenShortcut(bool enable)
    {
        group.alpha = enable ? 1 : 0;
        group.interactable = enable;
        group.blocksRaycasts = enable;
        
        if (!enable)
        {
            SetDeleteMode(false);
            SaveData();
        }

        webviewController.toolBar.SetURLFavoriteActive(!enable);
        
        shortcutIsOpened = enable;

        if (!enable)
        {
            DisableMode();
        }
    }

    private void DisableMode()
    {
        ResetToggle();
        SaveData();
    }

    private void InitShortCutItem()
    {
        itemPrefab.gameObject.SetActive(false);
        
        itemList.Clear();

        foreach (var shortcut in browserData.shortcutDataList)
        {
            InstantiateItem(shortcut);
        }
    }

    private void AddButtonsHandler()
    {
        // button_ActiveDelete.onClick.AddListener(delegate
        // {
        //     SetDeleteMode(editMode = !editMode);
        // });
        //
        // button_ActiveAddShortcut.onClick.AddListener(delegate
        // {
        //     OpenAddShortcutPopup();
        // });
        //
        // button_ActiveAddHome.onClick.AddListener(delegate
        // {
        //     SetAddHomeMode(selectMode = !selectMode);
        // });
        //
        // navi_ActiveDelete = button_ActiveDelete.transform.Search("NavigationText");
        // navi_ActiveDelete.gameObject.SetActive(false);
        //
        // navi_ActiveAddHome = button_ActiveAddHome.transform.Search("NavigationText");
        // navi_ActiveAddHome.gameObject.SetActive(false);
        
        toggle_ActiveDelete.onValueChanged.AddListener(OnValueChanged_SetDeleteMode);

        toggle_ActiveAddShortcut.onValueChanged.AddListener(EnableAddShortcutPopup);
        
        toggle_ActiveAddHome.onValueChanged.AddListener(OnValueChanged_SetAddHomeMode);
        
        navi_ActiveDelete = toggle_ActiveDelete.transform.Search("NavigationText");
        navi_ActiveDelete.gameObject.SetActive(false);
        
        navi_ActiveAddHome = toggle_ActiveAddHome.transform.Search("NavigationText");
        navi_ActiveAddHome.gameObject.SetActive(false);

        popup_AddShortcut.onConfirm.AddListener(delegate { DisableMode(); });
        popup_AddShortcut.onDeny.AddListener(DisableMode);
    }

    private void AddToolBarButtonsHandler()
    {
        webviewController.toolBar.onShortcut.AddListener(delegate
        {
            OpenShortcut(true);
            // if (webViewPrefab.WebView.Url == string.Empty)
            // {
            //     OpenShortcut(true);
            // }
            // else
            // {
            //     OpenShortcut(!shortcutIsOpened);
            // }
        });
        
        webviewController.toolBar.onHome.AddListener(delegate
        {
            OpenShortcut(false);
        });
        
        webviewController.toolBar.onFavorite.AddListener(delegate
        {
            if (shortcutIsOpened) return;
            
            OpenAddShortcutPopup();
        });
    }

    private async System.Threading.Tasks.Task SetWebviewOption()
    {
        await webViewPrefab.WaitUntilInitialized();
        
        webViewPrefab.WebView.UrlChanged += delegate(object sender, UrlChangedEventArgs args)
        {
            OpenShortcut(false);
        };
    }

    public async void ResetBrowserData()
    {
        await SetDefaultBrowserData();
        SaveData();
    }
    
    private async Task SetDefaultBrowserData()
    {
        // browserData = new BrowserData();
        //
        // browserData.shortcutDataList.Add(new ShortcutData("한컴독스", "https://www.hancomdocs.com/", true));
        // browserData.shortcutDataList.Add(new ShortcutData("Ploonet Studio", "http://studio.ploonet.com/main/"));
        // browserData.shortcutDataList.Add(new ShortcutData("Google", "https://www.google.com/"));
        // browserData.shortcutDataList.Add(new ShortcutData("Naver", "https://www.naver.com/"));
        // browserData.shortcutDataList.Add(new ShortcutData("Youtube", "https://www.youtube.com/"));
        // browserData.homeURL = "https://www.hancomdocs.com/";

        var webBookMarkList = MasterDataManager.Instance.dataOfficeBookmark.GetList();
        
        browserData = new BrowserData();

        foreach (var webBookMark in webBookMarkList)
        {
            Texture2D tex = null;
            await ImageDownloadUtility.DownloadTextureAsync($"http://www.google.com/s2/favicons?sz=256&domain={webBookMark.url}",
                receive =>
                {
                    tex = receive as Texture2D;
                    tempTex.Add(tex);
                });

            if (tex)
            {
                ImageDownloadUtility.SaveTextureToPNG(tex, "thumbnail", $"/Bookmark/{webBookMark.name}");
            }

            browserData.shortcutDataList.Add(new ShortcutData(webBookMark.name, webBookMark.url, false, tex));
        }
        
        browserData.homeURL = browserData.shortcutDataList[0].url;
    }

    public void InstantiateItem(ShortcutData data)
    {
        var item = Instantiate(itemPrefab.gameObject, scrollContent);
        var itemComponent = item.GetComponent<ShortcutItem>();

        if (itemComponent)
        {
            itemComponent.Init(data, webViewPrefab);
            itemList.Add(itemComponent);
            item.SetActive(true);
        }
    }

    public async void CreateShortCutItem(string urlTitle, string url)
    {
        Texture2D tex = null;
        await ImageDownloadUtility.DownloadTextureAsync($"http://www.google.com/s2/favicons?sz=256&domain={url}",
            receive =>
            {
                tex = receive as Texture2D;
                tempTex.Add(tex);
            });

        if (tex)
        {
            var replaced = string.Join("_", urlTitle.Split(Path.GetInvalidFileNameChars()));
            ImageDownloadUtility.SaveTextureToPNG(tex, "thumbnail", $"/Bookmark/{replaced}");
        }
            
        AddItem(new ShortcutData(urlTitle, url, false, tex));
    }
    
    private void AddItem(ShortcutData data)
    {
        // BKK TODO: 데이터 저장 후 인스턴스 생성
        browserData.shortcutDataList.Add(data);
        //SaveData();
        InstantiateItem(data);
    }

    public void RemoveItem(ShortcutItem item)
    {
        itemList.Remove(item);
        Destroy(item.gameObject);
        
        browserData.shortcutDataList.Remove(item.data);
        //SaveData();
        // BKK TODO: 저장된 데이터를 찾아서 삭제하기 추가할것
    }

    public ShortcutItem FindHomeItem()
    {
        return itemList.Find(i => i.data.isHome);
    }

    public void SetHomeURL(string url)
    {
        browserData.homeURL = url;
        //SaveData();
        webviewController.SetHomeURL(url);
    }

    private void OnValueChanged_SetDeleteMode(bool enable)
    {
        editMode = enable;
        foreach (var item in itemList)
        {
            item.SetEditMode(enable);
        }
        
        if(navi_ActiveDelete) navi_ActiveDelete.gameObject.SetActive(enable);
        
        if(!enable) ResetToggle();
    }
    
    public void SetDeleteMode(bool enable)
    {
        toggle_ActiveDelete.isOn = enable;
    }

    private void OnValueChanged_SetAddHomeMode(bool enable)
    {
        if (editMode) return;

        selectMode = enable;

        foreach (var item in itemList)
        {
            item.SetSelectMode(enable);
        }
        
        //if(enable != toggle_ActiveAddHome.isOn)
        
        if(navi_ActiveAddHome) navi_ActiveAddHome.gameObject.SetActive(enable);
        
        if(!enable) ResetToggle();
    }
    
    public void SetAddHomeMode(bool enable)
    {
        toggle_ActiveAddHome.isOn = enable;
    }

    private void OpenAddShortcutPopup()
    {
        popup_AddShortcut.OpenPopup(webViewPrefab.WebView.Title, webViewPrefab.WebView.Url, true);
    }
    
    private void EnableAddShortcutPopup(bool enable)
    {
        popup_AddShortcut.OpenPopup(webViewPrefab.WebView.Title, webViewPrefab.WebView.Url, enable);
        
        if(!enable) ResetToggle();
    }

    public void ResetToggle()
    {
        toggle_ActiveDelete.isOn = false;
        toggle_ActiveAddHome.isOn = false;
        toggle_ActiveAddShortcut.isOn = false;
    }
}

[System.Serializable]
public class ShortcutData
{
    public string title;
    public string url;
    public bool isHome = false;

    public Texture2D favicon;

    public ShortcutData(string _title, string _url, bool _isHome = false, Texture2D _favicon = null)
    {
        title = _title;
        url = _url;
        isHome = _isHome;
        favicon = _favicon;
    }
}

[System.Serializable]
public class BrowserData
{
    public string homeURL; 
    public List<ShortcutData> shortcutDataList = new List<ShortcutData>();
}
