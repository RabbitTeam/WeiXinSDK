namespace Rabbit.WeiXin.MP.Messages.Request
{
    /// <summary>
    /// 消息类型。
    /// </summary>
    public enum RequestMessageType
    {
        /// <summary>
        /// 文本消息。
        /// </summary>
        Text = 0,

        /// <summary>
        /// 图片消息。
        /// </summary>
        Image = 1,

        /// <summary>
        /// 语音消息。
        /// </summary>
        Voice = 2,

        /// <summary>
        /// 视频消息。
        /// </summary>
        Video = 3,

        /// <summary>
        /// 短视频消息。
        /// </summary>
        ShortVideo = 20,

        /// <summary>
        /// 地理位置消息。
        /// </summary>
        Location = 21,

        /// <summary>
        /// 链接消息。
        /// </summary>
        Link = 22,

        /// <summary>
        /// 事件。
        /// </summary>
        Event = 23
    }

    /// <summary>
    /// 一个抽象的请求消息基础。
    /// </summary>
    public interface IRequestMessageBase : IMessageBase
    {
        /// <summary>
        /// 消息类型。
        /// </summary>
        RequestMessageType MessageType { get; }
    }

    /// <summary>
    /// 一个抽象的请求消息。
    /// </summary>
    public interface IRequestMessage : IRequestMessageBase, IMessageIdentity
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        new string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        new string FromUserName { get; set; }
    }

    /// <summary>
    /// 请求消息基类。
    /// </summary>
    public abstract class RequestMessageBase : MessageBase, IRequestMessage
    {
        #region Implementation of IRequestMessage

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// 消息类型。
        /// </summary>
        public abstract RequestMessageType MessageType { get; }

        #endregion Implementation of IRequestMessage
    }
}