using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Application = UnityEngine.Device.Application;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [Header("Component")] 
    [SerializeField] private Transform leftDown;
    [SerializeField] private Transform rightTop;
    [SerializeField] private float checkRadius = 2.0f;
    [SerializeField] private LayerMask layerMask;
    
    private AssetBundle _loadedBundle;
    private readonly string PATH = Path.Combine(Application.streamingAssetsPath, "Item");
    
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
    }
    
    private Vector3 GetRandomPositionInRange()
    {
        var result = Vector3.zero;
        
        for (var i = 0; i < 10; ++i)
        {
            var randomX = Random.Range(leftDown.position.x, rightTop.position.x);
            var randomZ = Random.Range(leftDown.position.z, rightTop.position.z);
            result = new Vector3(randomX, 0, randomZ);

            if (Physics.OverlapSphere(result, checkRadius, layerMask).Length <= 0)
            {
                break;
            }
        }

        return result;
    }

    private Quaternion GetRandomRotation()
    {
        var rot = Random.Range(0, 360f);
        return Quaternion.Euler(0, rot, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var rightDown = new Vector3(rightTop.position.x, 0, leftDown.position.z);
        var leftTop = new Vector3(leftDown.position.x, 0, rightTop.position.z);
        
        Gizmos.DrawLine(leftDown.position, rightDown);
        Gizmos.DrawLine(rightDown, rightTop.position);
        Gizmos.DrawLine(rightTop.position, leftTop);
        Gizmos.DrawLine(leftTop, leftDown.position);
    }
}
