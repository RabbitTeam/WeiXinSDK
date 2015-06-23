using System;

namespace Rabbit.WeiXin.Open.Messages.Events
{
    /// <summary>
    /// 推送component_verify_ticket协议。
    /// </summary>
    public class ComponentVerifyTicketPush
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
        /// Ticket内容
        /// </summary>
        public string ComponentVerifyTicket { get; set; }
    }
}