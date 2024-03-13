using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_OfficeUpgrade : MonoBehaviour
{
    [SerializeField] private Transform item_FeaturePrefab;
    [SerializeField] private Transform item_PurchasePrefab;

    [SerializeField] private Transform contents_Feature;
    [SerializeField] private Transform contents_PurchaseButtons;
    [SerializeField] private TMP_Text text_ItemTitle;

    [SerializeField] private Button button_MoreInfo;
    
    private void OnValidate()
    {
        if (!item_FeaturePrefab) item_FeaturePrefab = transform.Search("Item_Feature");
        if (!item_PurchasePrefab) item_PurchasePrefab = transform.Search("Item_Purchase");

        if (!contents_Feature) contents_Feature = transform.Search("Contents_Feature");
        if (!contents_PurchaseButtons) contents_PurchaseButtons = transform.Search("Contents_PurchaseButtons");
        
        if (!button_MoreInfo) button_MoreInfo = transform.Search<Button>("Button_MoreInfo");
        if (!text_ItemTitle) text_ItemTitle = transform.Search<TMP_Text>("Text_ItemTitle");
    }

    public void SetData(OfficeUpgradeInfo upgradeInfo)
    {
        // BKK TODO: For문 돌려서 Feature와 Purchase를 생성
        
        item_FeaturePrefab.gameObject.SetActive(true);
        item_PurchasePrefab.gameObject.SetActive(true);

        foreach (var featureInfo in upgradeInfo.featureInfos)
        {
            var instance = Instantiate(item_FeaturePrefab, contents_Feature);
            var feature = instance.GetComponent<Item_Feature>();

            feature.SetData(featureInfo);
        }
        
        foreach (var priceInfo in upgradeInfo.priceInfos)
        {
            var instance = Instantiate(item_PurchasePrefab, contents_PurchaseButtons);
            var purchase = instance.GetComponent<Item_Purchase>();

            purchase.SetData(priceInfo);
        }
        
        item_FeaturePrefab.gameObject.SetActive(false);
        item_PurchasePrefab.gameObject.SetActive(false);

        text_ItemTitle.text = upgradeInfo.title;
        
        button_MoreInfo.onClick.AddListener(() =>
        {
            Application.OpenURL(upgradeInfo.moreInfoUrl);
        });
    }
}
