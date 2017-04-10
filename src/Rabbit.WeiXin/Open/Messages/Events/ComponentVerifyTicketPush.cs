using Rabbit.WeiXin.Utility;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Open.Messages.Events
{
    /// <summary>
    /// 推送component_verify_ticket协议。
    /// </summary>
    public class ComponentVerifyTicketPush
    {
        /// <summary>
        /// 初始化一个新的开放平台平台组件验证事件消息。
        /// </summary>
        public ComponentVerifyTicketPush()
        {
        }

        /// <summary>
        /// 初始化一个新的开放平台平台组件验证事件消息。
        /// </summary>
        /// <param name="xml">xml内容。</param>
        public ComponentVerifyTicketPush(string xml)
        {
            var document = XDocument.Parse(xml);
            Init(document.Element("xml"));
        }

        /// <summary>
        /// 初始化一个新的开放平台平台组件验证事件消息。
        /// </summary>
        /// <param name="container">xml容器。</param>
        public ComponentVerifyTicketPush(XContainer container)
        {
            Init(container);
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
        /// Ticket内容
        /// </summary>
        public string ComponentVerifyTicket { get; set; }

        #region Private Method

        private void Init(XContainer container)
        {
            AppId = container.Element("AppId").Value;
            CreateTime = DateTimeHelper.GetTimeByTimeStampString(container.Element("CreateTime").Value);
            ComponentVerifyTicket = container.Element("ComponentVerifyTicket").Value;
        }

        #endregion Private Method
    }
}