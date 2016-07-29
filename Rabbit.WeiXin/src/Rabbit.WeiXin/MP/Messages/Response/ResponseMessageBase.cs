using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.MP.Messages.Response
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

    /// <summary>
    /// 一个抽象的响应消息。
    /// </summary>
    public interface IResponseMessage : IMessageBase
    {
        /// <summary>
        /// 接收方帐号（收到的OpenID）。
        /// </summary>
        [Required]
        new string ToUserName { get; set; }

        /// <summary>
        /// 开发者微信号。
        /// </summary>
        [Required]
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
        #region Overrides of MessageBase

        /// <summary>
        /// 接收方帐号（收到的OpenID）。
        /// </summary>
        [Required]
        public override string ToUserName { get; set; }

        /// <summary>
        /// 开发者微信号。
        /// </summary>
        [Required]
        public override string FromUserName { get; set; }

        #endregion Overrides of MessageBase

        #region Implementation of IResponseMessage

        /// <summary>
        /// 消息类型。
        /// </summary>
        public abstract ResponseMessageType MessageType { get; }

        #endregion Implementation of IResponseMessage
    }
}