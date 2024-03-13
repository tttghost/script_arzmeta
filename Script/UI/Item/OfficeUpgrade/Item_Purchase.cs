using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Purchase : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Cycle;
    [SerializeField] private TMP_Text text_Currency;
    [SerializeField] private TMP_Text text_Price;
    [SerializeField] private TMP_Text text_Free;

    [SerializeField] private TMP_Text text_Discount;
    [SerializeField] private Image image_Discount;

    [SerializeField] private Transform group_Price;

    [SerializeField] private Button button;

    private void OnValidate()
    {
        if (!text_Cycle) text_Cycle = transform.Search<TMP_Text>("Text_Cycle");
        if (!text_Currency) text_Currency = transform.Search<TMP_Text>("Text_Currency");
        if (!text_Price) text_Price = transform.Search<TMP_Text>("Text_Price");
        if (!text_Free) text_Free = transform.Search<TMP_Text>("Text_Free");
        if (!text_Discount) text_Discount = transform.Search<TMP_Text>("Text_Discount");
        if (!image_Discount) image_Discount = transform.Search<Image>("Image_Discount");
        
        if (!group_Price) group_Price = transform.Search("Group_Price");

        if (!button) button = GetComponent<Button>();
    }

    public void SetData(string price, string cultureInfo, string cycle, Action onClick)
    {
        if (int.Parse(price) == 0)
        {
            text_Free.gameObject.SetActive(true);
            text_Cycle.gameObject.SetActive(false);
            group_Price.gameObject.SetActive(false);
        }
        else
        {
            NumberFormatInfo numberFormatInfo = new CultureInfo(cultureInfo).NumberFormat;
            string localizedPrice = Convert.ToInt64(price).ToString("c", numberFormatInfo)
                .Replace(numberFormatInfo.CurrencySymbol, "");
            
            text_Cycle.text = cycle;
            text_Currency.text = numberFormatInfo.CurrencySymbol;
            text_Price.text = localizedPrice;
            
            text_Free.gameObject.SetActive(false);
            text_Cycle.gameObject.SetActive(true);
            group_Price.gameObject.SetActive(true);
        }

        button.onClick.AddListener(() => onClick?.Invoke());
    }
    
    public void SetData(OfficePriceInfo priceInfo)
    {
        if (priceInfo.isFree)
        {
            text_Free.gameObject.SetActive(true);
            text_Cycle.gameObject.SetActive(false);
            group_Price.gameObject.SetActive(false);
            image_Discount.gameObject.SetActive(false);
            text_Discount.gameObject.SetActive(false);
        }
        else
        {
            NumberFormatInfo numberFormatInfo = new CultureInfo(priceInfo.cultureInfo).NumberFormat;
            string localizedPrice = priceInfo.price.ToString("c", numberFormatInfo)
                .Replace(numberFormatInfo.CurrencySymbol, "");
            
            text_Cycle.text = priceInfo.billingCycle.ToString();
            text_Currency.text = numberFormatInfo.CurrencySymbol;
            text_Price.text = localizedPrice;

            switch (priceInfo.billingCycle)
            {
                case BillingCycle.Monthly:
                    text_Cycle.text = "월간";// BKK TODO: 국가별로 언어 바꿀수 있게 할 것(서버측에 해당 언어 데이터를 요청해서 Set?)
                    text_Cycle.gameObject.SetActive(true);
                    break;
                case BillingCycle.Annually:
                    text_Discount.text = "연간";// BKK TODO: 국가별로 언어 바꿀수 있게 할 것(서버측에 해당 언어 데이터를 요청해서 Set?)
                    text_Cycle.gameObject.SetActive(true);
                    break;
                case BillingCycle.None:
                default:
                    text_Cycle.gameObject.SetActive(false);
                    break;
            }

            if (priceInfo.annuallyDiscountPercentage != 0)
            {
                text_Discount.text = $"월 {priceInfo.annuallyDiscountPercentage}% 할인";// BKK TODO: 국가별로 언어 바꿀수 있게 할 것(서버측에 해당 언어 데이터를 요청해서 Set?)
                
                image_Discount.gameObject.SetActive(true);
                text_Discount.gameObject.SetActive(true);
            }
            else
            {
                image_Discount.gameObject.SetActive(false);
                text_Discount.gameObject.SetActive(false);
            }

            text_Free.gameObject.SetActive(false);
            group_Price.gameObject.SetActive(true);
        }

        button.onClick.AddListener(() =>
        {
            Purchase(priceInfo.purchaseUrl);
        });
    }

    public void Purchase(string url)
    {
        // BKK TODO: 여기서 현재 오피스 등급 체크할 것
        Application.OpenURL(url);
    }
}
