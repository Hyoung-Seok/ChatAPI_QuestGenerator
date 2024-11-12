using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;

public class ItemSpawnManager : MonoBehaviour
{
    [Header("Setting")] 
    [SerializeField] private float checkRadius = 2.0f;
    [SerializeField] private LayerMask layerMask;

    [Header("Position Data")]
    [SerializeField] private List<ItemSpawnPos> itemSpawnPosList;
    
    private AssetBundle _loadedBundle;
    private readonly string PATH = Path.Combine(Application.streamingAssetsPath, "Item");

    public void Init()
    {
        foreach (Transform child in transform)
        {
            itemSpawnPosList.Add(child.gameObject.GetComponent<ItemSpawnPos>());
        }
    }
    
    public async UniTask LoadItem(string itemName, int count)
    {
        itemName = itemName.Replace(" ", "");
        var bundlePath = Path.Combine(PATH, "item." + itemName);
        
        _loadedBundle = await LoadAssetBundleAsync(bundlePath);
        
        if (_loadedBundle == null)
        {
            return;
        }

        await LoadAndInstantiateAsset(itemName, count);
    }

    private async UniTask<AssetBundle> LoadAssetBundleAsync(string path)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
        await bundleLoadRequest.ToUniTask();
        return bundleLoadRequest.assetBundle;
    }

    private async UniTask LoadAndInstantiateAsset(string assetName, int count)
    {
        var asset = await _loadedBundle.LoadAssetAsync<GameObject>(assetName).ToUniTask() as GameObject;

        if (asset == null)
        {
            return;
        }

        for (var i = 0; i < count; ++i)
        {
            var obj = Instantiate(asset, transform);
            obj.transform.SetPositionAndRotation(GetRandomPositionInRange(), GetRandomRotation());
        }
        
        _loadedBundle.Unload(false);
        _loadedBundle = null;
    }
    
    private Vector3 GetRandomPositionInRange()
    {
        var result = Vector3.zero;
        var index = Random.Range(0, itemSpawnPosList.Count);

        var ld = itemSpawnPosList[index].LeftDown;
        var rt = itemSpawnPosList[index].RightTop;
        
        for (var i = 0; i < 10; ++i)
        {
            var randomX = Random.Range(ld.x, rt.x);
            var randomZ = Random.Range(ld.y, rt.y);
            result = new Vector3(randomX, 0, randomZ);

            if (Physics.OverlapSphere(result, checkRadius, layerMask).Length <= 0)
            {
                break;
            }
        }

        result.y = itemSpawnPosList[index].SpawnHeight;
        return result;
    }

    private Quaternion GetRandomRotation()
    {
        var rot = Random.Range(0, 360f);
        return Quaternion.Euler(0, rot, 0);
    }
}
