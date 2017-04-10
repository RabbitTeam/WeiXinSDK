using Rabbit.WeiXin.Utility;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Open.Messages.Events
{
    /// <summary>
    /// 取消授权消息。
    /// </summary>
    public class CancelAuthorizeMessage
    {
        /// <summary>
        /// 初始化一个新的开放平台取消授权事件消息。
        /// </summary>
        /// <param name="container">xml容器。</param>
        public CancelAuthorizeMessage(XContainer container)
        {
            AppId = container.Element("AppId").Value;
            CreateTime = DateTimeHelper.GetTimeByTimeStampString(container.Element("CreateTime").Value);
            AuthorizerAppId = container.Element("AuthorizerAppid").Value;
        }

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
        public string AuthorizerAppId { get; set; }
    }
}