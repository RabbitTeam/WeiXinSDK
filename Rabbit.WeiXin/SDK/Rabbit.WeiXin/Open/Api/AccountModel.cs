using System;

namespace Rabbit.WeiXin.Open.Api
{
    /// <summary>
    /// 第三方平台信息。
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// 第三方平台appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 第三方平台appsecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 消息加解密密钥。
        /// </summary>
        public string EncodingAesKey { get; set; }

        /// <summary>
        /// 获取核实票据信息。
        /// </summary>
        public Func<string> GetVerifyTicket { get; set; }
    }
}