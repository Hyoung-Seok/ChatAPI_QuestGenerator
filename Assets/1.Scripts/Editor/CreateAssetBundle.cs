using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Assets/Build All Asset Bundles")]
    private static void BuildAllAssetBundles()
    {
        var assetBundleDir = Application.streamingAssetsPath;

        if (Directory.Exists(Application.streamingAssetsPath) == false)
        {
            Directory.CreateDirectory(assetBundleDir);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDir, BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);
    }
}
