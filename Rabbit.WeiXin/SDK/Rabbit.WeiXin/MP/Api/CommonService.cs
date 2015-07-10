using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api
{
    /// <summary>
    /// 一个通用的服务接口。
    /// </summary>
    public interface ICommonService
    {
        /// <summary>
        /// 获取公众号的全局唯一票据。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>第三方用户唯一凭证密钥，即appsecret。</returns>
        AccessTokenModel GetAccessToken(bool ignoreCached = false);

        /// <summary>
        /// 将一个url地址转换成一个短地址。
        /// </summary>
        /// <param name="url">url地址。</param>
        /// <returns>短地址Url。</returns>
        string GenerateShortAddress(string url);

        /// <summary>
        /// 获取服务器Ip地址列表。
        /// </summary>
        /// <returns>服务器Ip地址列表。</returns>
        string[] GetServerIpList();
    }

    /// <summary>
    /// 通用的服务接口实现。
    /// </summary>
    public sealed class CommonService : ICommonService
    {
        #region Field

        private readonly AccountModel _accountModel;
        private static readonly ConcurrentDictionary<string, AccessTokenModel> Dictionary = new ConcurrentDictionary<string, AccessTokenModel>();

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的通用服务实例。
        /// </summary>
        /// <param name="accountModel"></param>
        public CommonService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICommonService

        /// <summary>
        /// 获取公众号的全局唯一票据。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>第三方用户唯一凭证密钥，即appsecret。</returns>
        public AccessTokenModel GetAccessToken(bool ignoreCached = false)
        {
            const string grantType = "client_credential";

            var appId = _accountModel.AppId;
            var appSecret = _accountModel.AppSecret;

            Func<AccessTokenModel> get = () =>
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type={0}&appid={1}&secret={2}", grantType, appId, appSecret);
                return WeiXinHttpHelper.GetResultByJson<AccessTokenModel>(url);
            };

            return ignoreCached ? Dictionary.AddOrUpdate(appId, key => get(), (k, m) => get()) : Dictionary.GetOrAdd(appId, key => get());
        }

        /// <summary>
        /// 将一个url地址转换成一个短地址。
        /// </summary>
        /// <param name="url">url地址。</param>
        /// <returns>短地址Url。</returns>
        public string GenerateShortAddress(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                throw new ArgumentException(url + " 不是一个有效的Url。", "url");

            if (new[] { "http", "https", "weixin" }.All(i => !i.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Url 中的 Scheme必需是（http、https、weixin）当中的一种，而当前是：" + uri.Scheme, "url");

            var postUrl = "https://api.weixin.qq.com/cgi-bin/shorturl?access_token=" + _accountModel.GetAccessToken();
            var content = WeiXinHttpHelper.PostString(postUrl, new { action = "long2short", long_url = url });
            var shortAddress = JObject.Parse(content)["short_url"].Value<string>();
            return shortAddress;
        }

        /// <summary>
        /// 获取服务器Ip地址列表。
        /// </summary>
        /// <returns>服务器Ip地址列表。</returns>
        public string[] GetServerIpList()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + _accountModel.GetAccessToken();

            var json = WeiXinHttpHelper.GetString(url);
            var array = (JArray)JObject.Parse(json)["ip_list"];
            return array.Select(i => i.Value<string>()).ToArray();
        }

        #endregion Implementation of ICommonService
    }

    #region Help Class

    /// <summary>
    /// 获取公众号的全局唯一票据结果模型。
    /// </summary>
    public sealed class AccessTokenModel
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的公众号的全局唯一票据。
        /// </summary>
        public AccessTokenModel()
        {
            CreateTime = DateTime.Now;
        }

        #endregion Constructor

        /// <summary>
        /// 公众号的全局唯一票据。
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 凭证有效时间（秒）。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 是否过期。
        /// </summary>
        /// <returns>如果过期返回true，否则返回false。</returns>
        public bool IsExpired()
        {
            return CreateTime.AddSeconds(ExpiresIn - 20/*不采用最后的期限作为判断，防止在很少的时间内到期导致后续的逻辑无法执行*/) <= DateTime.Now;
        }
    }

    #endregion Help Class
}