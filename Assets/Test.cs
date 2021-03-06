using UnityEngine;
using System.Collections;
using System.IO;

public class Test : MonoBehaviour
{
	public GUISkin skin;
	Texture texture;

	
	void Update ()
	{
        
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
  		{
   			Application.Quit();
   			
  		}
	}
	
	void OnGUI()
	{
		 GUI.skin = skin;
		
		if(GUILayout.Button("打开手机相册"))
		{
			 //调用我们制作的Android插件打开手机相册
			 AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      	 	 AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        	 jo.Call("TakePhoto","takeSave");
		}
		if(GUILayout.Button("打开手机摄像机"))
		{
			 //调用我们制作的Android插件打开手机摄像机
			 AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      	 	 AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        	 jo.Call("TakePhoto","takePhoto");

		}
		if(texture != null)
		{
			//注意！ 我们在这里绘制Texture对象，该对象是通过
			//我们制作的Android插件得到的，当这个对象不等于空的时候
			//直接绘制。
			GUI.DrawTexture(new Rect(100,300,300,300),texture);
		}
	}
	
	void  messgae(string str)
	{
		//在Android插件中通知Unity开始去指定路径中找图片资源
		StartCoroutine(LoadTexture(str));
	
	}
	
	

  IEnumerator LoadTexture(string name)
  {
	//注解1
	string path  =  "file://" + Application.persistentDataPath +"/" + name;
	
	WWW www = new WWW(path);
	while (!www.isDone)
	{
			
	}
	yield return www;
	//为贴图赋值
	texture = www.texture;
  }  
}