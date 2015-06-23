using System;

namespace Rabbit.WeiXin.Open.Messages.Events
{
    /// <summary>
    /// 取消授权消息。
    /// </summary>
    public class CancelAuthorizeMessage
    {
        /// <summary>
        /// 第三方平台appid。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 取消授权的公众号。
        /// </summary>
        public string AuthorizerAppid { get; set; }
    }
}