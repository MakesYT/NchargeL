// See https://aka.ms/new-console-template for more information
using Microsoft.Web.WebView2.Wpf;
using NCLCore;


Console.WriteLine(System.Environment.GetEnvironmentVariable("CURSE_API_KEY"));

//var hwr = HttpRequestHelper.CreatePostHttpResponse(
//               "https://files.xmdhs.com/curseforge/info?id=238222",
//               new Dictionary<string, string>());

//var re1 = HttpRequestHelper.GetResponseString(hwr);
//Console.WriteLine(re1);
//int begin = re1.IndexOf("https://www.curseforge.com/minecraft/mc-mods/");
//Console.WriteLine(begin);
//int end = re1.IndexOf("\"", begin);
//Console.WriteLine(re1.Substring(begin, end - begin));
//System.Diagnostics.Process.Start("explorer.exe", "https://www.curseforge.com/minecraft/mc-mods/jei/download/3681294/file");
WebView2 webView = new WebView2();
webView.Source = new Uri("https://www.curseforge.com/minecraft/mc-mods/jei/download/3681294/file");
