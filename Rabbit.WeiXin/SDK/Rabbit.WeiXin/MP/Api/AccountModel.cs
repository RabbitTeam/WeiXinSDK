using System;

namespace Rabbit.WeiXin.MP.Api
{
    /// <summary>
    /// 公众号信息。
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 公众号应用密钥。
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 获取调用接口凭证。
        /// </summary>
        public Func<string> GetAccessToken { get; set; }
    }
}