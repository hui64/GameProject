using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using UnityEngine.UI;

public class UpLoadToSever : BaseMonoBehaviour {
    // Use this for initialization
    void Start () {
        UpLoad();
	}
    /*
       推送文件到资源服务器
   */
    private void UpLoad()
    {
        print(AssetPath.AssetBundlePath);
        StartCoroutine(ClearSeverData(delegate()
        {
            LoadVersion(delegate (List<string> paths) {
                StartCoroutine(UpLoadYied(paths));
            });
        }));
    }
    private IEnumerator UpLoadYied(List<string> paths) {
        foreach (string info in paths)
        {
            string path = AssetPath.GetWWWPath(AssetPath.StreamingPath + AssetPath.ChangPathEnd(info));
            print(path);
            WWW www = new WWW(path);
            //生成版本资源更新
            yield return www;
            WWWForm form = new WWWForm();
            byte[] infos = www.bytes;
            print(www.bytes.Length);
            form.AddBinaryData("aaaa", www.bytes, "/" + AssetPath.ChangPathEnd(info));
            WWW upLoad = new WWW(AssetPath.UpURL, form);
            yield return upLoad;
        }
    }
    private IEnumerator ClearSeverData(GameTools.Finish finish) {
        print("====");
        WWWForm form = new WWWForm();
        form.AddField("clear", "true");
        WWW upLoad = new WWW(AssetPath.UpURL, form);
        yield return upLoad;
        print("startLoad");
        finish();
    }
    private void LoadVersion(GameTools.Finish<List<string>> finish)
    {
        List<string> paths = new List<string>();
        DownLoad(AssetPath.GetWWWPath(AssetPath.VersionPath), delegate (WWW www) {
            string content = www.text;
            print("加载本地版本文件: " + content);
            string[] items = content.Split(new char[] { '\n' });
            foreach (string item in items)
            {
                string[] info = item.Split(new char[] { ',' });
                if (info != null && info.Length == 2)
                {
                    paths.Add(info[0]);
                }
            }
            finish(paths);
        });
    }
}
