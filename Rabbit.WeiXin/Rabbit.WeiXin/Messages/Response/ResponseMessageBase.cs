using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 响应消息类型。
    /// </summary>
    public enum ResponseMessageType
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
        /// 音乐消息。
        /// </summary>
        Music = 40,

        /// <summary>
        /// 图文。
        /// </summary>
        News = 41,

        /// <summary>
        /// 多客服消息。
        /// </summary>
        TransferCustomerServic = 42
    }

    public interface IResponseMessage : IMessageBase
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        new string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        new string FromUserName { get; set; }

        /// <summary>
        /// 消息类型。
        /// </summary>
        ResponseMessageType MessageType { get; }
    }

    /// <summary>
    /// 响应消息基础。
    /// </summary>
    public abstract class ResponseMessageBase : MessageBase, IResponseMessage
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        [Required]
        public override string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        [Required]
        public override string FromUserName { get; set; }

        #region Implementation of IResponseMessage

        /// <summary>
        /// 消息类型。
        /// </summary>
        public abstract ResponseMessageType MessageType { get; }

        #endregion Implementation of IResponseMessage
    }
}