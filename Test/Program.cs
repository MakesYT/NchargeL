// See https://aka.ms/new-console-template for more information
using Microsoft.Web.WebView2.Wpf;
using NCLCore;
using System.Collections.Generic;

InfoManager infoManager=new InfoManager();
infoManager.PropertyChanged += (oo, ee) =>
{
    var logtmp = (oo as InfoManager).info;
    Console.WriteLine(logtmp.msg);
};


HttpClient webClient = new HttpClient();
var result = webClient.Send(new HttpRequestMessage(HttpMethod.Head, "https://download.ncserver.top:8000/NCL/clients/Ncharge/1.0.2.zip"));
Console.WriteLine(result.Content.Headers.ContentLength);
