// See https://aka.ms/new-console-template for more information
using Microsoft.Web.WebView2.Wpf;
using NCLCore;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

InfoManager infoManager=new InfoManager();
infoManager.PropertyChanged += (oo, ee) =>
{
    var logtmp = (oo as InfoManager).info;
    Console.WriteLine(logtmp.msg);
};


var re1 = HttpRequestHelper.getHttpTool("https://cursemaven.com/test/344787/3648990");
if (re1 != null)
{
    string re = re1.Result;
    if(re.Contains("Response: 200"))
    {
        re = re.Substring(re.IndexOf("Found: ")+7);
        System.Console.WriteLine(re.Substring(re.LastIndexOf("/") + 1));
    }
    else
    {

    }
}

