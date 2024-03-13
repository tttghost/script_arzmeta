using Azure.Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : Singleton<StorageManager>
{
    public AzureBlobStorageService azureBlobStorageService;
    protected override void AWAKE()
    {
        base.AWAKE();
        azureBlobStorageService = gameObject.AddComponent<AzureBlobStorageService>();
    }

    public override void SingletonInit()
    {
        base.SingletonInit();
    }
}
