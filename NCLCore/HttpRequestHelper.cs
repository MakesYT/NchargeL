using Newtonsoft.Json;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NCLCore
{
    public class HttpRequestHelper
    {
        public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            int cout = 0;
            while (cout < 10)
            {
                HttpWebRequest request = null;
                //如果是发送HTTPS请求 
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    //request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                //request.Method = "POST";
                request.ContentType = "application/json";

                //设置代理UserAgent和超时
                //request.UserAgent = userAgent;
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
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(parameters));
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                string[] values = request.Headers.GetValues("Content-Type");
                try
                {
                    return request.GetResponse() as HttpWebResponse;
                }
                catch (WebException we)
                {
                    // return (HttpWebResponse)we.Response;
                }
            }
            return null;

        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();

            }
        }

        /// <summary>
        /// 验证证书
        /// </summary>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }
    }

}
