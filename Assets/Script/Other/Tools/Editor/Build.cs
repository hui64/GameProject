﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
public class Build
{
    [MenuItem("Build/Create AssetsAndVersion")]
    static void CreateAssets()
    {
        Dictionary<string, string> fileData = new Dictionary<string, string>();
        //添加依赖文件路径到version中
        AssetPath.AddDepToVersion(fileData);

        if (Directory.Exists(AssetPath.StreamingPath))
            Directory.Delete(AssetPath.StreamingPath, true);

        Directory.CreateDirectory(AssetPath.StreamingPath);
        AssetPath.NewText(AssetPath.VersionPath);
        List<string> assetFullPaths = AssetPath.GetNeedBuildPath();
        foreach (string fullPath in assetFullPaths)
        {
            Debug.Log(fullPath);
            AssetPath.GetFilePathsAndMD5(fullPath, fileData);
        }
        AssetPath.WriteVersion(AssetPath.VersionPath, fileData);             //写入版本配置文件
        AssetDatabase.Refresh();
        AssetBundleBuild[] buildMap = new AssetBundleBuild[fileData.Count];
        List<string> paths = new List<string>();
        foreach (string path in fileData.Keys)
        {
            paths.Add(path);
        }
        for (int i = 0; i < buildMap.Length; i++)
        {
            //此资源包下文件
            string[] path = new string[1];
            path[0] = @"Assets\" + paths[i];
            buildMap[i].assetNames = path;
            buildMap[i].assetBundleName = AssetPath.ChangPathEnd(paths[i], AssetPath.EndPath);   //打包的资源包名称
            Debug.Log(path[0]);
            Debug.Log(buildMap[i].assetBundleName);
        }
        Debug.Log(AssetPath.StreamingPath);
        #if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(AssetPath.StreamingPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        #elif UNITY_IPHONE
        BuildPipeline.BuildAssetBundles(AssetPath.StreamingPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);
        #endif
        AssetDatabase.Refresh();                                             //刷新编辑器
    }
}