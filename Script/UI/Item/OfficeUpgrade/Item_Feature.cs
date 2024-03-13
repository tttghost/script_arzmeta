using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Feature : MonoBehaviour
{
    [SerializeField] private Image image_FeatureIcon;
    [SerializeField] private TMP_Text text_Description;
    
    private void OnValidate()
    {
        if (!image_FeatureIcon) image_FeatureIcon = transform.Search<Image>("Image_FeatureIcon");
        if (!text_Description) text_Description = transform.Search<TMP_Text>("Text_Description");
    }

    public void SetData(Sprite icon, string description)
    {
        text_Description.text = description;

        image_FeatureIcon.sprite = icon;
    }

    public void SetData(Texture2D icon, string description)
    {
        Rect texRect = new Rect(0, 0, icon.width, icon.height);
        Sprite iconSprite = Sprite.Create(icon, texRect, Vector2.one * 0.5f);
        SetData(iconSprite, description);
    }
    
    public void SetData(OfficeFeatureInfo featureInfo)
    {
        if (!featureInfo.icon)
        {
            SetData((Sprite) null, featureInfo.description);
            return;
        }
        
        Rect texRect = new Rect(0, 0, featureInfo.icon.width, featureInfo.icon.height);
        Sprite iconSprite = Sprite.Create(featureInfo.icon, texRect, Vector2.one * 0.5f);
        SetData(iconSprite, featureInfo.description);
    }
}
