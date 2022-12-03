
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCLCore;

public class HttpRequestHelper
{
   static HttpClientHandler httpClientHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
    };
    private static HttpClient webClient = new HttpClient(httpClientHandler);
    private static readonly ILog log = LogManager.GetLogger("HttpRequestHelper");
    public static string DownLoad(string uri, string localFileName)
    {
        log.Debug("开始请求:Url:"+uri);
        var server = new Uri(uri);
        var p = Path.GetDirectoryName(localFileName);
        if (!Directory.Exists(p)) Directory.CreateDirectory(p);

        // 发起请求并异步等待结果
        var httpClient = new HttpClient();
        var responseMessage = httpClient.GetAsync(server).Result;
        if (responseMessage.IsSuccessStatusCode)
        {
            using (var fs = File.Create(localFileName))
            {
                // 获取结果，并转成 stream 保存到本地。
                var streamFromService = responseMessage.Content.ReadAsStreamAsync().Result;
                streamFromService.CopyTo(fs);
                return "true";
            }
        }
        else
            return responseMessage.Content.ToString();
    }
    public static  async Task<string> httpTool(string url, JObject keyValues)
    {
        HttpContent content = new StringContent(JsonConvert.SerializeObject(keyValues),Encoding.ASCII, "application/json");
        var reponse = await webClient.PostAsync(url, content);
        String result =  reponse.Content.ReadAsStringAsync().Result;
        log.Debug(result);
        return result;
        
        return null;
    }
    public static async Task<string> getHttpTool(string url)
    {
        
        var reponse =  webClient.GetAsync(url);
        webClient.DefaultRequestHeaders.Add("Accept", "application/json");
        webClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization");
        String result = reponse.Result.Content.ReadAsStringAsync().Result;
        log.Debug(result);
        return result;

        return null;
    }
    public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
    {
        var cout = 0;
        while (cout < 10)
        {
            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version11;
            }
                
            else
                request = WebRequest.Create(url) as HttpWebRequest;

            //request.Method = "POST";
            request.ContentType = "application/json";
           // request.Headers.Add("x-api-key", System.Environment.GetEnvironmentVariable("CURSE_API_KEY"));
            //设置代理UserAgent和超时
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36 Edg/99.0.1150.52";
            //request.Timeout = timeout;

            //if (cookies != null)
            //{
            //    request.CookieContainer = new CookieContainer();
            //    request.CookieContainer.Add(cookies);
            //}
            //发送POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                request.Method = "POST";
                //System.Console.WriteLine(JsonConvert.SerializeObject(parameters));
                var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(parameters));
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            var values = request.Headers.GetValues("Content-Type");

            return request.GetResponse() as HttpWebResponse;
        }

        return null;
    }

    /// <summary>
    ///     获取请求的数据
    /// </summary>
    public static string GetResponseString(HttpWebResponse webresponse)
    {
        using (var s = webresponse.GetResponseStream())
        {
            var reader = new StreamReader(s, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }

    /// <summary>
    ///     验证证书
    /// </summary>
    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
        SslPolicyErrors errors)
    {
        if (errors == SslPolicyErrors.None)
            return true;
        return false;
    }
}