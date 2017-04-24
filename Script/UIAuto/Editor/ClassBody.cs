using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ClassBody  {
    public string topTag;
    public string use;
    public string name;
    public string div;  //继承
    public List<string> attributes = new List<string>();
    public List<string> funcs = new List<string>();
    public string CreateClass() {
        string myString = "";
        if(!string.IsNullOrEmpty(topTag))
            myString += topTag + AddLine();
        myString += use;
        myString += GetClassInfo(name, GetDeriverName(div));
        string body = GetAttributeString();
        body += GetFunString();
        myString += AddBody(body);
        return myString;
    }
    public string GetClassInfo(string className, string derName = "")
    {
        return "public class " + className + derName;
    }
    public string GetDeriverName(string div)
    {  //获取继承
        if (string.IsNullOrEmpty(div))
            return "";
        return ": " + div;
    }
    public string GetAttributeString() {
        string myString = "";
        foreach (var str in attributes) {
            myString += AddTab() + str + AddLine();
        }
        return myString;
    }
    public string GetFunString()
    {
        string myString = "";
        foreach (var str in funcs)
        {
            myString += AddTab() + str + AddLine();
        }
        return RemoveEmptyLine(myString);
    }
   
    //============================================ 符号 =============================================
    public string AddLine()
    {
        return "\n";
    }
    public string AddTab()
    {
        return "    ";
    }
    public string AddLineAndTab()
    {
        return AddLine() + AddTab();
    }
    public string AddBody(string body, string tab0 = "", string tab1 = "", string tab2 = "", string tab3 = "")
    {
        string info = tab0 + tab1 + tab2 + tab3;
        return "{" + AddLine() + body + AddLine() + info + "}";
    }
    static string RemoveEmptyLine(string myString)
    {
        if (string.IsNullOrEmpty(myString))
            return "";
        return myString.Substring(0, myString.Length - 1);
    }
}
