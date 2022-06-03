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

DownloadManagerV2 downloadManagerV2 = new DownloadManagerV2(infoManager);
var downloadItems = new List<DownloadItem>();
downloadItems.Add(new DownloadItem("http://download.ncserver.top:8000/NCL/clients/Ncharge/1.0.2.zip", "C:\\Users\\13540\\Downloads\\1.txt"));
downloadItems.Add(new DownloadItem("http://download.ncserver.top:8000/NCL/2.txt", "C:\\Users\\13540\\Downloads\\2.txt"));
downloadManagerV2.Start(downloadItems, 2);
