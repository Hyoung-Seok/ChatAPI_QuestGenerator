using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Application = UnityEngine.Device.Application;

public class ItemSpawner : MonoBehaviour
{
    private AssetBundle _loadedBundle;
    private readonly string PATH = Application.streamingAssetsPath;

    private void Start()
    {
        LoadItem("연구일지").Forget();
    }

    public async UniTask LoadItem(string itemName)
    {
        itemName = itemName.Replace(" ", "");
        var bundlePath = Path.Combine(PATH, "item." + itemName);
        _loadedBundle = await LoadAssetBundleAsync(bundlePath);
        
        if (_loadedBundle == null)
        {
            Debug.LogError("Bundle Is NULL");
            return;
        }

        await LoadAndInstantiateAsset("researchlog");
    }

    private async UniTask<AssetBundle> LoadAssetBundleAsync(string path)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);

        await bundleLoadRequest.ToUniTask();

        return bundleLoadRequest.assetBundle;
    }

    private async UniTask LoadAndInstantiateAsset(string assetName)
    {
        var asset = await _loadedBundle.LoadAssetAsync<GameObject>(assetName).ToUniTask();
        
        if (asset != null)
        {
            Instantiate(asset);
        }
        else
        {
            Debug.LogError($"Asset '{assetName}' not found in AssetBundle.");
        }
    }
}
