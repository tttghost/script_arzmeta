using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class ShortcutItem : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text titleFirst;
    [SerializeField] private RawImage favicon;
    [SerializeField] private Button button_Load;
    [SerializeField] private Button button_Delete;
    //[SerializeField] private Button button_SelectOutline;
    [SerializeField] private Image selectOutline;
    [SerializeField] private Transform homeMark;

    //private string url;

    public ShortcutData data;

    public bool canEdit { get; protected set; }
    public bool canSelect { get; protected set; }

    private void Awake()
    {
        SetEditMode(false);
        SetSelectMode(false);
        //SetHomeMark(false);
    }

    private void OnValidate()
    {
        if(!title) title = transform.Search("Title").GetComponent<TMP_Text>();
        if(!titleFirst) titleFirst = transform.Search("TitleFirst").GetComponent<TMP_Text>();
        if (!favicon) favicon = transform.Search("Favicon").GetComponent<RawImage>();
        if (!button_Load) button_Load = GetComponent<Button>();
        if (!button_Delete) button_Delete = transform.Search("Button_Delete").GetComponent<Button>();
        if (!selectOutline) selectOutline = transform.Search("SelectOutline").GetComponent<Image>();
        if (!homeMark) homeMark = transform.Search("img_homemark_Bg");
        //if (!button_SelectOutline) button_SelectOutline = selectOutline.GetComponent<Button>();
    }

    public void Init(ShortcutData data, CanvasWebViewPrefab _webViewPrefab)
    {
        this.data = data;
        
        title.text = this.data.title;
        if(this.data.title.Length > 0) titleFirst.text = this.data.title.Substring(0, 1).ToUpper();

        SetFavicon(data.favicon);

        button_Load.onClick.AddListener(delegate
        {
            if (!canEdit && !canSelect)
            {
                var shortcutPage = GetComponentInParent<WebviewShortcutPage>();
                shortcutPage.OpenShortcut(false);
                _webViewPrefab.WebView.LoadUrl(this.data.url);
            }
            
            if (canSelect)
            {
                var shortcutPage = GetComponentInParent<WebviewShortcutPage>();
                var currentHome = shortcutPage.FindHomeItem();
                
                if(currentHome) currentHome.SetHomeMark(false);
                
                shortcutPage.SetHomeURL(this.data.url);
                SetHomeMark(true);
                shortcutPage.SetAddHomeMode(false);
            }
        });
        
        button_Delete.onClick.AddListener(delegate
        {
            if (canEdit)
            {
                var shortcutPage = GetComponentInParent<WebviewShortcutPage>();
                
                shortcutPage.RemoveItem(this);
            }
        });

        SetHomeMark(this.data.isHome);

        // button_SelectOutline.onClick.AddListener(delegate
        // {
        //     if (canSelect)
        //     {
        //         var shortcutPage = GetComponentInParent<WebviewShortcutPage>();
        //         shortcutPage.SetHomeURL(this.data.url);
        //         shortcutPage.SetAddHomeMode(false);
        //     }
        // });
    }

    public void SetEditMode(bool enable)
    {
        canEdit = enable;
        button_Delete.gameObject.SetActive(enable);
    }

    public void SetSelectMode(bool enable)
    {
        canSelect = enable;
        selectOutline.gameObject.SetActive(enable);
    }

    public void SetHomeMark(bool enable)
    {
        this.data.isHome = enable;
        homeMark.gameObject.SetActive(enable);
    }

    private void SetFavicon(bool enable)
    {
        titleFirst.enabled = !enable;

        if (enable && data != null && data.favicon != null)
        {
            favicon.texture = data.favicon;
        }
        
        favicon.enabled = enable;
    }
}
