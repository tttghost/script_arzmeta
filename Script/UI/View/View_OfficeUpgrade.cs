using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_OfficeUpgrade : UIBase
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform item_OfficeUpgradePrefab;
    [SerializeField] private Button button_Back;

    private List<OfficeUpgradeInfo> upgradeInfos = new List<OfficeUpgradeInfo>();

    public UnityEvent onClose = new UnityEvent();

    private void OnValidate()
    {
        if (!scrollRect) scrollRect = transform.Search<ScrollRect>("Scroll View");
        if (!item_OfficeUpgradePrefab) item_OfficeUpgradePrefab = scrollRect.content.GetChild(0);
        if (!button_Back) button_Back = transform.Search<Button>("btn_Back");
    }

    protected override void Start()
    {
        base.Start();

        button_Back.onClick.AddListener(CloseView);
        
        SetExampleData();
    }

    public void CloseView()
    {
        onClose?.Invoke();
    }

    public void SetData()
    {
        item_OfficeUpgradePrefab.gameObject.SetActive(true);
        
        foreach (var upgradeInfo in upgradeInfos)
        {
            var instance = Instantiate(item_OfficeUpgradePrefab, scrollRect.content);
            var item = instance.GetComponent<Item_OfficeUpgrade>();
            item.SetData(upgradeInfo);
        }

        item_OfficeUpgradePrefab.gameObject.SetActive(false);
        scrollRect.horizontalScrollbar.value = 0;
    }

    private void SetExampleData()
    {
        OfficeUpgradeInfo info_free = new OfficeUpgradeInfo
        {
            title = "Free",
            moreInfoUrl = "https://www.google.com/",
            featureInfos = new List<OfficeFeatureInfo>
            {
                new OfficeFeatureInfo
                {
                    description = "6명 이내의 자유로운 모임",
                    icon = null,
                },
                new OfficeFeatureInfo
                {
                    description = "45분 이내 무제한 회의",
                    icon = null,
                },
                new OfficeFeatureInfo
                {
                    description = "나의 예약일정 등록",
                    icon = null,
                },
            },

            priceInfos = new List<OfficePriceInfo>
            {
                new OfficePriceInfo
                {
                    price = 0,
                    isFree = true,
                    purchaseUrl = "https://www.google.com/",
                },
            }
        };
        
        OfficeUpgradeInfo info_Basic = new OfficeUpgradeInfo
        {
            title = "Basic",
            moreInfoUrl = "https://www.google.com/",
            featureInfos = new List<OfficeFeatureInfo>
            {
                new OfficeFeatureInfo
                {
                    description = "공간 별 자유로운 인원 설정",
                    icon = null,
                },
                new OfficeFeatureInfo
                {
                    description = "최대 12시간 회의 진행",
                    icon = null,
                },
                new OfficeFeatureInfo
                {
                    description = "개수 제한 없이 일정 예약",
                    icon = null,
                },
            },

            priceInfos = new List<OfficePriceInfo>
            {
                new OfficePriceInfo
                {
                    price = 9900,
                    isFree = false,
                    cultureInfo = "ko-KR",
                    billingCycle = BillingCycle.Monthly,
                    purchaseUrl = "https://www.google.com/",
                },
                new OfficePriceInfo
                {
                    price = 94800,
                    isFree = false,
                    cultureInfo = "ko-KR",
                    billingCycle = BillingCycle.Annually,
                    annuallyDiscountPercentage = 20,
                    purchaseUrl = "https://www.google.com/",
                },
            }
        };

        upgradeInfos = new List<OfficeUpgradeInfo>
        {
            info_free,
            info_Basic,
        };

        SetData();
    }
}

[Serializable]
public class OfficeUpgradeInfo
{
    public string title;

    public string moreInfoUrl;
    
    public List<OfficeFeatureInfo> featureInfos = new List<OfficeFeatureInfo>();

    public List<OfficePriceInfo> priceInfos = new List<OfficePriceInfo>();
}

[Serializable]
public class OfficeFeatureInfo
{
    public Texture2D icon;

    public string description;

    ~OfficeFeatureInfo()// 소멸자
    {
        UnityEngine.Object.Destroy(icon);// 메모리에서 Texture2D 삭제
    }
}

[Serializable]
public class OfficePriceInfo
{
    public int price;
    public string cultureInfo;
    public bool isFree;
    public int specialDiscountPercentage;
    public int annuallyDiscountPercentage;
    public BillingCycle billingCycle;
    public string purchaseUrl;
}

public enum BillingCycle
{
    None = 0,
    Monthly = 1,
    Annually = 2,
}
