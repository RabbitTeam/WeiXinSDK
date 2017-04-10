using Rabbit.WeiXin.MP.Messages.Request;

namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 事件消息基础。
    /// </summary>
    public abstract class EventMessageBase : MessageBase, IRequestMessageBase
    {
        #region Overrides of MessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public RequestMessageType MessageType
        {
            get { return RequestMessageType.Event; }
        }

        #endregion Overrides of MessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public abstract EventType EventType { get; }
    }

    /// <summary>
    /// 常见的事件消息基类。
    /// </summary>
    public abstract class EventKeyMessageBase : EventMessageBase
    {
        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string EventKey { get; set; }
    }

    /// <summary>
    /// 二维码事件消息基础。
    /// </summary>
    public abstract class QrEventKeyMessageBase : EventKeyMessageBase
    {
        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; set; }
    }
}