using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
public class CreateScrite : MonoBehaviour {

    enum TagState {
        UIGameObject,
        UITransform,
        UISprite,
        UILabel,
        UISlider,
        UIScroll
    }

    static string Top = "//=====================================由代码自动生成请勿修改=======================================";
    static string AttributeTag =     "//-------------------------------属性标记请勿删除------------------------------------";
    static string InitAttributeTag = "//-----------------------------初始化属性标记请勿删除--------------------------------";
    static string AddListenTag =     "//--------------------------添加button点击事件标记请勿删除---------------------------";
    static string ClickFunTag = "//------------------------------点击方法标记请勿删除---------------------------------";
    static string ViewFunTag = "//-------------------------------表现方法标记请勿删除---------------------------------";
    static string TargetName;
    static GameObject Target;
    static Dictionary<string, List<Transform>> MyTagTrans;
    //===============================================主函数==================================================
    //自动生成UI功能代码
    [MenuItem("Create/自动生成UI功能代码")]
    static void CreateUIScript()
    {
        InitList();
        if(!SetTarget())
            return;
        SetListByTag(Target.transform);
        CreateClassC();
        CreateClassM();
        CreateClassVM();
        CreateClassV();
        AssetDatabase.Refresh();
    }
    static void InitList() {
        MyTagTrans = new Dictionary<string, List<Transform>>();
    }
  
    static void CreateClassC()
    {
        ClassBody data = new ClassBody();
        data.topTag = Top;
        data.use = GetUsingC();
        data.name = GetClassNameC();
        data.div = "BaseMonoBehaviour";
        data.attributes.Add("private " + GetClassNameM() + " Model;");
        GetUIClickAttribute(data.attributes);

        data.funcs.Add(GetAwake(""));
        data.funcs.Add(GetStart(AddTab() + AddTab() + "Init();"));
        data.funcs.Add(GetOnEnable(""));
        data.funcs.Add(GetInitC());
        if (MyTagTrans.ContainsKey("UIClick"))
        {
            string initGameObjectBody = GetFindChildBody(MyTagTrans["UIClick"], TagState.UIGameObject);
            data.funcs.Add(GetInitGameObject(initGameObjectBody));
        }
        string addListenBody = GetAddListenBody();
        data.funcs.Add(GetAddListen(addListenBody));

        string myString = data.CreateClass();
        WriteByUTF8(myString, data.name);
    }
  
    static void CreateClassM()
    {
        string cache = "";
        //Component script = Target.GetComponent(Type.GetType(GetClassNameC()));
        string path = Application.dataPath + "/" + "Script/" + GetClassNameM() + ".cs";
        if (File.Exists(path))
        {
            cache = ReadByFile(path);
            string clickFuncOld = GetCache(cache, ClickFunTag);
            string clickFuncNew = GetClickFuncs(clickFuncOld);
            Debug.Log(clickFuncOld);
            Debug.Log(clickFuncNew);
            cache = cache.Replace(clickFuncOld, clickFuncNew);
            WriteByUTF8(cache, GetClassNameM());
            return;
        }
        ClassBody data = new ClassBody();
        data.use = GetUsingM();
        data.name = GetClassNameM();
        data.attributes.Add("private " + GetClassNameVM() + " ViewModel;");
        data.funcs.Add(GetInitM());
        data.funcs.Add(ClickFunTag);
        data.funcs.Add(GetClickFuncs(cache) + ClickFunTag);
        string myString = data.CreateClass();
        WriteByUTF8(myString, data.name);
    }
    static void CreateClassVM()
    {
        ClassBody data = new ClassBody();
        data.use = GetUsingVM();
        data.name = GetClassNameVM();
        data.attributes.Add("private " + GetClassNameV() + " View;");
        GetVMAllAttribute(data.attributes, data.funcs);
        data.attributes.Add(GetInitVM());
        string myString = data.CreateClass();
        WriteByUTF8(myString, data.name);
    }
    static void CreateClassV()
    {
        string cache = "";
        //Component script = Target.GetComponent(Type.GetType(GetClassNameC()));
        string path = Application.dataPath + "/" + "Script/" + GetClassNameV() + ".cs";
        if (File.Exists(path))
        {
            cache = ReadByFile(path);
            string viewFuncOld = GetCache(cache, ViewFunTag);
            string viewFuncNew = GetViewFuncs(viewFuncOld);
            cache = cache.Replace(viewFuncOld, viewFuncNew);
            WriteByUTF8(cache, GetClassNameV());
            return;
        }
        ClassBody data = new ClassBody();
        data.use = GetUsingV();
        data.name = GetClassNameV();
        data.attributes.Add("private " + GetClassNameVM() + " ViewModel;");
        data.attributes.Add("private BaseUI baseUI;");
        GetViewAllAttribute(data.attributes);
        data.funcs.Add(GetFunction("public " + GetClassNameV() + "(Transform myTran)", AddTab() + AddTab()
            + "InitGameObject(myTran);" + AddLineAndTab() + AddTab() + "baseUI = GameTools.GetBaseUI();"));
        data.funcs.Add(GetFunction("void InitGameObject(Transform myTran)", GetAllFindChildBody()));
        data.funcs.Add(ViewFunTag);
        data.funcs.Add(GetViewFuncs(cache) + ViewFunTag);
        string myString = data.CreateClass();
        WriteByUTF8(myString, data.name);
    }
    //Component script = target.GetComponent(Type.GetType(name));
    //if (!script)
    static bool SetTarget() {
        Target = GetChooseGameObject();
        if (!Target)
            return false;
        TargetName = ChangeName(Target.name);
        Target.tag = "UIRoot";
        return true;
    }
    static GameObject GetChooseGameObject()
    {
        GameObject[] targets = Selection.gameObjects;
        if (targets.Length == 0)
        {
            Debug.Log("请选择要生成代码的物体");
            return null;
        }
        if (targets.Length > 1)
        {
            Debug.Log("只能选中一个物体");
            return null;
        }
        return targets[0];
    }
    static string ChangeName(string name) {
        string startName = name.Substring(0, 2);
        Debug.Log(startName);
        if (startName.Equals("UI"))
            return FirstUp(name);
        return "UI" + FirstUp(name);
    }
    static void SetListByTag(Transform go)
    {
        for (int i = 0; i < go.childCount; i++)
        {
            Transform target = go.GetChild(i);
            AddListByTag(target.tag, target);
            SetListByTag(target);
        }
    }
    static void AddListByTag(string tag, Transform myTran)
    {
        if (!MyTagTrans.ContainsKey(tag))
            MyTagTrans.Add(tag, new List<Transform>());
        MyTagTrans[tag].Add(myTran);
    }
    static string GetTargePath(Transform myTran)
    {
        if (myTran.name.Equals(Selection.gameObjects[0].name))
            return "";
        return GetTargePath(myTran.parent) + "/" + myTran.name;
    }
  
    //============================================ ClassComponent ==================================================
    static string GetUsingC() {
        List<string> usingStrings = new List<string>() {
            "UnityEngine",
            "System.Collections",
            "System.Collections.Generic",
            "UnityEngine.UI"
        };
        return GetUsing(usingStrings);
    }
    static string GetUsingM()
    {
        List<string> usingStrings = new List<string>() {
            "System.Collections",
            "System.Collections.Generic",
        };
        return GetUsing(usingStrings);
    }
    static string GetUsingVM()
    {
        List<string> usingStrings = new List<string>() {
            "UnityEngine",
            "System.Collections",
            "System.Collections.Generic",
        };
        return GetUsing(usingStrings);
    }
    static string GetUsingV()
    {
        List<string> usingStrings = new List<string>() {
            "UnityEngine",
            "System.Collections",
            "System.Collections.Generic",
        };
        return GetUsing(usingStrings);
    }
    static string GetUsing(List<string> usingStrings)
    {
        string myString = "";
        for (int i = 0; i < usingStrings.Count; i++)
        {
            string info = "using " + usingStrings[i] + ";";
            myString += info + "\n";
        }
        return myString;
    }

    static string GetClassNameC()
    {
        return TargetName + "C";
    }
    static string GetClassNameM()
    {
        return TargetName + "M";

    }
    static string GetClassNameVM()
    {
        return TargetName + "VM";
    }
    static string GetClassNameV()
    {
        return TargetName + "V";
    }

    static string GetDeriverName(string div)
    {  //获取继承
        return ": " + div;
    }
    static string GetAttribute(string attribute, string name) {
        return  attribute + FirstUp(name) + ";";
    }
    static string GetFunction(string func, string body) {
        return func + AddBody(body, AddTab());
    }
    static string GetClickFuncs(string cache) {
        foreach (var info in MyTagTrans["UIClick"]) {
            if (cache.IndexOf("Click" + FirstUp(info.name)) == -1)
            {
                cache += GetFunction("public void Click" + FirstUp(info.name)
                    + "(string buttonName)", AddTab() + AddTab() + "GameLog.Log(\"click\" + buttonName);" + AddLineAndTab() + AddTab()
                    + "//ViewModel.ButtonIsClicked(buttonName);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string GetViewFuncs(string cache) {
        foreach (var dic in MyTagTrans)
        {
            switch (dic.Key)
            {
                case "UIGameObjectActive":
                    cache = SetActiveViewFun(dic.Value, cache);
                    continue;
                case "UITranMovePos":
                    cache = SetMovePosViewFun(dic.Value, cache);
                    continue;
                case "UITranMoveAroundOnce":
                    cache = SetMoveAroundOnceViewFun(dic.Value, cache);
                    continue;
                case "UITranChangeScan":
                    cache = SetChangeScanViewFun(dic.Value, cache);
                    continue;
                case "UITranChangeScanAroundOnce":
                   cache = SetChangeScanAroundOnceViewFun(dic.Value, cache);
                   continue;
                case "UILabel":
                    cache = SetUILabelViewFun(dic.Value, cache);
                    continue;
                case "UISprite":
                    cache = SetUISpriteViewFun(dic.Value, cache);
                    continue;
                case "UISlider":
                    cache = SetUISliderViewFun(dic.Value, cache);
                    continue;
                case "UIScroll":
                    cache = SetScrollViewFun(dic.Value, cache);
                    continue;
            }
        }
        return RemoveEmptyLine(cache);
    }
    static string SetActiveViewFun(List<Transform> myTrans, string cache) {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(bool active)",
                    AddTab() + AddTab() + FirstUp(myTran.name) + ".SetActive(active);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetMovePosViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(Vector3 targetPos, float speed, GameTools.Finish finish)",
                    AddTab() + AddTab() +  "AssetTool.Instance.MoveToward(" + FirstUp(myTran.name) + ", targetPos, speed, finish);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetMoveAroundOnceViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(Vector3 targetPos, float speed, GameTools.Finish finish)",
                    AddTab() + AddTab() + "AssetTool.Instance.MoveAroundOnce(" + FirstUp(myTran.name) + ", targetPos, speed, finish);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetChangeScanViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(float scan, float speed, GameTools.Finish finish)",
                    AddTab() + AddTab() + "AssetTool.Instance.ChangeScan(" + FirstUp(myTran.name) + ", scan, speed, finish);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetChangeScanAroundOnceViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(float scan, float speed, GameTools.Finish finish)",
                    AddTab() + AddTab() + "AssetTool.Instance.ChangeScanAroundOnce(" + FirstUp(myTran.name) + ", scan, speed, finish);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetUILabelViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(string text)",
                    AddTab() + AddTab() + "baseUI.SetText(" + FirstUp(myTran.name) + ", text);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetUISpriteViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(string spriteName)",
                    AddTab() + AddTab() + "baseUI.SetImage(" + FirstUp(myTran.name) + ", spriteName);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetUISliderViewFun(List<Transform> myTrans, string cache)
    {
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(float value)",
                    AddTab() + AddTab() + "baseUI.SetSlider(" + FirstUp(myTran.name) + ", value);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string SetScrollViewFun(List<Transform> myTrans, string cache)
    {
        print("--------------");
        foreach (var myTran in myTrans)
        {
            if (cache.IndexOf(GetFuncName(myTran)) == -1)
            {
                cache += GetFunction("public void " + GetFuncName(myTran) + "(BaseItem[] datas, int oneLineNum, Vector2 ditance)",
                    AddTab() + AddTab() +  "baseUI.SetScroll(" + FirstUp(myTran.name) + ", datas, oneLineNum, ditance);") + AddLineAndTab();
            }
        }
        return cache;
    }
    static string GetFuncName(Transform myTran) {
        return "Set" + myTran.tag + FirstUp(myTran.name);
    }
    //=======================================ClassBody=======================================
    /*
     * 属性定义区
     */
  
    static void GetUIClickAttribute(List<string> dataAttributes)
    {
        if (MyTagTrans.ContainsKey("UIClick"))
        {
            foreach (var myTran in MyTagTrans["UIClick"])
            {
                dataAttributes.Add(GetAttribute("private GameObject ", FirstUp(myTran.name)));
            }
        }
    }
    static void GetViewAllAttribute(List<string> dataAttributes)
    {
        foreach (var dic in MyTagTrans) {
            switch (dic.Key) {
                case "UIClick":
                case "UIGameObjectActive":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private GameObject ",FirstUp(myTran.name)));
                    }
                    continue;
                case "UITranMovePos":
                case "UITranMoveAroundOnce":
                case "UITranChangeScan":
                case "UITranChangeScanAroundOnce":
                case "UIScroll":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private Transform ",FirstUp(myTran.name)));
                    }
                    continue;
            }
            foreach (var myTran in dic.Value)
            {
                dataAttributes.Add(GetAttribute("private Object ", FirstUp(myTran.name)));
            }
        }
    }
    static string GetAndSetString(string attributeName, string funcName) {
        return AddTab() + AddTab() + "get { return " + attributeName + "; }" + AddLineAndTab() + AddTab() +
            "set {" + AddLineAndTab() + AddTab() + AddTab() + "if(" + attributeName + " == value)" + AddLineAndTab() + AddTab() + AddTab() + AddTab() +
            "return;" + AddLineAndTab() + AddTab() + AddTab() + attributeName + " = value;" + AddLineAndTab() + AddTab() + AddTab() +
            "View." + funcName + "(" + attributeName + ");" + AddLineAndTab() + AddTab() + "}";
    }
    static string GetAndSetMoveString(string attributeName, string funcName, string speed, string delegateString)
    {
        return AddTab() + AddTab() + "get { return " + attributeName + "; }" + AddLineAndTab() + AddTab() +
            "set {" + AddLineAndTab() + AddTab() + AddTab() + "if(" + attributeName + " == value)" + AddLineAndTab() + AddTab() + AddTab() + AddTab() +
            "return;" + AddLineAndTab() + AddTab() + AddTab() + attributeName + " = value;" + AddLineAndTab() + AddTab() + AddTab() +
            "View." + funcName + "(" + attributeName + "," + speed + "," + delegateString + ");" + AddLineAndTab() + AddTab() + "}";
    }
    static void GetVMAllAttribute(List<string> dataAttributes, List<string> dataFuncs)
    {
        foreach (var dic in MyTagTrans)
        {
            switch (dic.Key)
            {
                case "UIGameObjectActive":
                     foreach (var myTran in dic.Value)
                     {
                         dataAttributes.Add(GetAttribute("private bool _active", FirstUp(myTran.name)));
                         dataFuncs.Add(GetFunction("public bool Active" + FirstUp(myTran.name), GetAndSetString("_active" + FirstUp(myTran.name),
                            GetFuncName(myTran))));
                     }
                    continue;
                case "UILabel":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private string _string", FirstUp(myTran.name)));
                        dataFuncs.Add(GetFunction("public string String" + FirstUp(myTran.name), GetAndSetString("_string" + FirstUp(myTran.name),
                            GetFuncName(myTran))));
                    }
                    continue;
                case "UISprite":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private string _sprite", FirstUp(myTran.name)));
                        dataFuncs.Add(GetFunction("public string Sprite" + FirstUp(myTran.name), GetAndSetString("_sprite" + FirstUp(myTran.name),
                            GetFuncName(myTran))));
                    }
                    continue;
                case "UISlider":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private float _slider", FirstUp(myTran.name)));
                        dataFuncs.Add(GetFunction("public float Slider" + FirstUp(myTran.name), GetAndSetString("_slider" + FirstUp(myTran.name),
                            GetFuncName(myTran))));
                    }
                    continue;
                case "UITranMovePos":
                case "UITranMoveAroundOnce":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private Vector3 _pos", FirstUp(myTran.name)));
                        dataAttributes.Add(GetAttribute("private float Speed", FirstUp(myTran.name)));
                        dataAttributes.Add(GetAttribute("private GameTools.Finish Delegate", FirstUp(myTran.name)));
                        dataFuncs.Add(GetFunction("public Vector3 Pos" + FirstUp(myTran.name), GetAndSetMoveString("_pos" + FirstUp(myTran.name),
                            GetFuncName(myTran), "Speed" + FirstUp(myTran.name), "Delegate" + FirstUp(myTran.name))));
                    }
                    continue;
                case "UITranChangeScan":
                case "UITranChangeScanAroundOnce":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private float _scan", FirstUp(myTran.name)));
                        dataAttributes.Add(GetAttribute("private float Speed", FirstUp(myTran.name)));
                        dataAttributes.Add(GetAttribute("private GameTools.Finish Delegate", FirstUp(myTran.name)));
                        dataFuncs.Add(GetFunction("public float Scan" + FirstUp(myTran.name), GetAndSetMoveString("_scan" + FirstUp(myTran.name),
                            GetFuncName(myTran), "Speed" + FirstUp(myTran.name), "Delegate" + FirstUp(myTran.name))));
                    }
                    continue;
                case "UIScroll":
                    foreach (var myTran in dic.Value)
                    {
                        dataAttributes.Add(GetAttribute("private bool _", myTran.name));
                       //dataFuncs.Add(GetFunction("public void "));
                    }
                    continue;
            }
            //foreach (var myTran in dic.Value)
            //{
            //    dataAttributes.Add(GetAttribute("private bool _", myTran.name));
            //    //dataFuncs.Add(GetFunction("public void "));
            //}
        }
    }
    /*
     * Awake
     */
    static string GetAwake(string body) {
        return GetFunction("void Awake()", body);
    }
    /*
     * Start
     */
    static string GetStart(string body)
    {
        return GetFunction("void Start()", body);
    }
    /*
     * OnEnable
     */
    static string GetOnEnable(string body)
    {
        return GetFunction("void OnEnable()", body);
    }
    /*
     * InitGameObject
     */
    static string GetInitGameObject(string body)
    {
        return GetFunction("void InitGameObject(Transform myTran)", body);
    }
    /*
     * AddListen
     */
    static string GetAddListen(string body)
    {
        return GetFunction("void AddListen()", body);
    }
    static string GetInitC() {
        string body = AddTab() + AddTab() + "InitGameObject(transform);" + AddLineAndTab() + AddTab()
           + "AddListen();" + AddLineAndTab() + AddTab()
           + "Model = new " + GetClassNameM() + "();" + AddLineAndTab() + AddTab()
           + GetClassNameV() + " view = new " + GetClassNameV() + "(transform);"+ AddLineAndTab() + AddTab()
           + "Model.Init(view);";
        return GetFunction("public void Init()", body);
    }
    static string GetInitM()
    {
        string body = AddTab() + AddTab() + "ViewModel = new " + GetClassNameVM() + "();" + AddLineAndTab() + AddTab() + "ViewModel.Init(view);";
        return GetFunction("public void Init(" + GetClassNameV() + " view" + ")", body);
    }
    static string GetInitVM()
    {
        string body = AddTab() + AddTab() + "View = view;";
        return GetFunction("public void Init(" + GetClassNameV() + " view" + ")", body);
    }
    /*
     * InitGameObject Body
     */
    static string GetCInitGameObjectBody() {
        string myString = "";
       
        return myString;
    }
    static string GetAllFindChildBody() {
        string myString = "";
        foreach (var dic in MyTagTrans)
        {
            switch (dic.Key)
            {
                case "UIClick":
                case "UIGameObjectActive":
                case "UIScrollItem":
                    myString += GetFindChildBody(dic.Value, TagState.UIGameObject);
                    continue;
                case "UITranMovePos":
                case "UITranMoveAroundOnce":
                case "UITranChangeScan":
                case "UITranChangeScanAroundOnce":
                    myString += AddLine() + GetFindChildBody(dic.Value, TagState.UITransform);
                    continue;
                case "UILabel":
                    myString += AddLine() + GetFindChildBody(dic.Value, TagState.UILabel);
                    continue;
                case "UISprite":
                    myString += AddLine() + GetFindChildBody(dic.Value, TagState.UISprite);
                    continue;
                case "UISlider":
                    myString += AddLine() + GetFindChildBody(dic.Value, TagState.UISlider);
                    continue;
            }
        }
        return myString;
    }
    static string GetFindChildBody(List<Transform> myTrans, TagState endState) {
        string myString = "";
        foreach (var myTran in myTrans)
        {
            string path = GetTargePath(myTran);
            myString += AddTab() + AddTab() + GetFindChild(myTran.name, path, endState);
        }
        return RemoveEmptyLine(myString);
    }
    static string GetFindChild(string name ,string path, TagState endState) {
        path = path.Substring(1);
        switch (endState) {
            case TagState.UIGameObject :
                return FirstUp(name) + " = myTran.FindChild(\"" + path + "\").gameObject;\n";
            case TagState.UITransform:
                return FirstUp(name) + " = myTran.FindChild(\"" + path + "\");\n";
            case TagState.UISprite:
                return FirstUp(name) + " = baseUI.GetImage(myTran.FindChild(\"" + path + "\"));\n";
            case TagState.UILabel:
                return FirstUp(name) + " = baseUI.GetText(myTran.FindChild(\"" + path + "\"));\n";
            case TagState.UISlider:
                return FirstUp(name) + " = baseUI.GetSlider(myTran.FindChild(\"" + path + "\"));\n";
        }
        return "";
    }
    /*
     * AddListen Body
     */
    static string GetAddListenBody() {
        string myString = "";
        foreach (var myTran in MyTagTrans["UIClick"]) {
            myString += AddTab() + AddTab() + "EventTriggerListener.Get(" + FirstUp(myTran.name) + ").onClick = Model.Click" + FirstUp(myTran.name) + ";\n";
        }
        return RemoveEmptyLine(myString);
    }
    //============================================ 符号 =============================================
    static string AddLine()
    {
        return "\n";
    }
    static string AddTab()
    {
        return "    ";
    }
    static string AddLineAndTab()
    {
        return AddLine() + AddTab();
    }
    static string AddBody(string body, string tab0 = "", string tab1 = "", string tab2 = "", string tab3 = "")
    {
        string info = tab0 + tab1 + tab2 + tab3;
        return "{" + AddLine() + body + AddLine() + info + "}";
    }
    static string RemoveEmptyLine(string myString)
    {
        return myString.Substring(0, myString.Length - 1);
    }
    //--------------------------------------------------------end----------------------------------------------------
    //获取从begin开始的字符串不包括begin
    public static string GetCache(string myString, string begin)
    {
        Debug.Log(begin);
        if (myString.IndexOf(begin) == -1)
            return myString;
        int i = myString.IndexOf(begin) + begin.Length;
        myString = myString.Substring(i + 3);
        i = myString.IndexOf(begin);
        return myString.Substring(0, i);
    }
    public static string FirstUp(string name)
    {
        return name.Substring(0, 1).ToUpper() + name.Substring(1);
    }
    //写入文本并且变为UTF8格式
    public static void WriteByUTF8(string myString, string name)
    {
        string path = Application.dataPath + "/" + "Script/" + name + ".cs";
        StreamWriter stream = new StreamWriter(path, false, Encoding.UTF8);
        UTF8Encoding utf8 = new UTF8Encoding(); // Create a UTF-8 encoding.
        byte[] bytes = utf8.GetBytes(myString.ToCharArray());
        string EnUserid = utf8.GetString(bytes);
        stream.WriteLine(EnUserid);
        stream.Flush();
        stream.Close();
    }
    //读取txt
    public static string ReadByFile(string path)
    {
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        StringBuilder sb = new StringBuilder();
        while ((line = sr.ReadLine()) != null)
        {
            sb.AppendLine(line);
        }
        sr.Close();
        return sb.ToString();
    }
}

