using Newtonsoft.Json;
using Rabbit.WeiXin.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rabbit.WeiXin.Utility.Extensions;

namespace Rabbit.WeiXin.MP.Api.Js
{

    /// <summary>
    /// Js配置生成生成服务
    /// </summary>
    public interface IJsConfigService
    {
        /// <summary>
        /// 生成微信JsSDK配置信息
        /// </summary>
        /// <param name="currentUrl"></param>
        /// <returns></returns>
        JsSdkConfigModel Create(string currentUrl);
    }


    /// <summary>
    /// Js配置生成生成服务
    /// </summary>
    public class JsConfigService : IJsConfigService
    {
        private readonly AccountModel _accountModel;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="accountModel"></param>
        public JsConfigService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        public JsSdkConfigModel Create(string currentUrl)
        {
            JsSdkConfigModel result = new JsSdkConfigModel();
            result.Timestamp = DateTimeHelper.GetTimeStampByTime(DateTime.Now).ToString();
            result.AppId = _accountModel.AppId;
            result.NonceStr = StringHelper.GetRandomString(32);
             

            return result;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class JsSdkConfigModel
    {
        /// <summary>
        /// AppId
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        [JsonProperty("nonceStr")]
        public string NonceStr { get; set; }

        /// <summary>
        /// Signature
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
