using Newtonsoft.Json;
using Rabbit.WeiXin.Utility.Extensions;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Utility
{
    internal static class WeiXinHttpHelper
    {
        public static TResult GetResultByJson<TResult>(string url)
        {
            var data = HttpHelper.Get(url);
            return GetResultByJson<TResult>(data);
        }

        public static TResult PostResultByJson<TResult>(string url, byte[] postData, string contentType = "application/x-www-form-urlencoded")
        {
            var data = HttpHelper.Post(url, postData, contentType);
            return GetResultByJson<TResult>(data);
        }

        public static TResult PostResultByJson<TResult>(string url, object postJsonData, string contentType = "application/x-www-form-urlencoded")
        {
            return PostResultByJson<TResult>(url, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postJsonData)), contentType);
        }

        public static byte[] Post(string url, string data, string contentType = "application/x-www-form-urlencoded")
        {
            var result = HttpHelper.Post(url, Encoding.UTF8.GetBytes(data), contentType);
            ResultHelper.TryThrowError(result);
            return result;
        }

        public static byte[] Post(string url, object postJsonData, string contentType = "application/x-www-form-urlencoded")
        {
            return Post(url, JsonConvert.SerializeObject(postJsonData), contentType);
        }

        public static string PostString(string url, object postJsonData)
        {
            var data = Post(url, postJsonData);
            var content = Encoding.UTF8.GetString(data);
            ResultHelper.TryThrowError(content);
            return content;
        }

        public static string PostString(string url, byte[] data, string contentType = "application/x-www-form-urlencoded")
        {
            var result = HttpHelper.Post(url, data, contentType);
            ResultHelper.TryThrowError(result);
            return Encoding.UTF8.GetString(result);
        }

        public static string GetString(string url)
        {
            var data = HttpHelper.Get(url);
            var content = Encoding.UTF8.GetString(data);
            ResultHelper.TryThrowError(content);
            return content;
        }

        #region Private Method

        private static TResult GetResultByJson<TResult>(byte[] data)
        {
            var content = Encoding.UTF8.GetString(data);
            ResultHelper.TryThrowError(content);
            return JsonConvert.DeserializeObject<TResult>(content);
        }

        #endregion Private Method
    }

    internal static class HttpHelper
    {
        public static byte[] Get(string url)
        {
            WebHeaderCollection responseHeaders;
            return Get(url, out responseHeaders);
        }

        public static string GetString(string url)
        {
            return Encoding.UTF8.GetString(Get(url));
        }

        public static byte[] Get(string url, out WebHeaderCollection responseHeaders)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
#if NET
                        using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    responseHeaders = response.Headers;
                    return responseStream.ReadBytes();
                }
            }
#else
            var result = Task.Run(async () =>
             {
                 using (var response = await request.GetResponseAsync())
                 {
                     using (var responseStream = response.GetResponseStream())
                     {
                         return new KeyValuePair<byte[], WebHeaderCollection>(responseStream.ReadBytes(), response.Headers);
                     }
                 }
             }).Result;

            responseHeaders = result.Value;
            return result.Key;
#endif
        }

        public static byte[] Post(string url, byte[] postData, string contentType = "application/x-www-form-urlencoded")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
#if NET
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postData, 0, postData.Length);
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            return responseStream.ReadBytes();
                        }
                    }
                }
#else
            return Task.Run(async () =>
            {
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    requestStream.Write(postData, 0, postData.Length);
                    using (var response = await request.GetResponseAsync())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            return responseStream.ReadBytes();
                        }
                    }
                }
            }).Result;
#endif
        }
    }
}