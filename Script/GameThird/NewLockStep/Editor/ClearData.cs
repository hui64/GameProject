using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ClearData : MonoBehaviour {
    [MenuItem("Clear/ClearData")]
    static void ClearCacheData()
    {
        if (Directory.Exists(Application.persistentDataPath))
            Directory.Delete(Application.persistentDataPath, true);
        Directory.CreateDirectory(Application.persistentDataPath);
        AssetDatabase.Refresh();
    }
}
