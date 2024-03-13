using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Popup_AddShortcut : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField_URLTitle;
    [SerializeField] private TMP_InputField inputField_URL;

    [SerializeField] private Button button_Confirm;
    [SerializeField] private Button button_Deny;

    public UnityEvent<string, string> onConfirm = new UnityEvent<string, string>();
    public UnityEvent onDeny = new UnityEvent();

    private bool titleEdited = false;
    
    private void OnValidate()
    {
        if (!inputField_URL) inputField_URL = transform.Search("InputField_URL").GetComponentInChildren<TMP_InputField>();
        if (!inputField_URLTitle) inputField_URLTitle = transform.Search("InputField_URLTitle").GetComponentInChildren<TMP_InputField>();
        if (!button_Confirm) button_Confirm = transform.Search("Button_Confirm").GetComponent<Button>();
        if (!button_Deny) button_Deny = transform.Search("Button_Deny").GetComponent<Button>();
    }

    private void Awake()
    {
        button_Confirm.onClick.AddListener(delegate
        {
            if (inputField_URLTitle.text == string.Empty || inputField_URL.text == string.Empty) return;
            
            onConfirm.Invoke(inputField_URLTitle.text, inputField_URL.text);
            OpenPopup("", "", false);
        });
        
        button_Deny.onClick.AddListener(delegate
        {
            onDeny.Invoke();
            OpenPopup("", "", false);
        });
    }

    public void OpenPopup(string urlTitle, string url, bool enable)
    {
        inputField_URL.text = url;
        inputField_URLTitle.text = urlTitle;
        this.gameObject.SetActive(enable);
    }
}
