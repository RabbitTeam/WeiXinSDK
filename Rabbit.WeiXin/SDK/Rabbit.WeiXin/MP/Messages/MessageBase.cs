using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.MP.Messages
{
    /// <summary>
    /// 一个抽象的消息接口。
    /// </summary>
    public interface IMessageBase
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        [Required]
        string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        [Required]
        string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        [Required]
        DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 消息基类。
    /// </summary>
    public abstract class MessageBase : IMessageBase
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        [Required]
        public virtual string ToUserName { get; set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        [Required]
        public virtual string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 一个抽象的媒体消息接口。
    /// </summary>
    public interface IMediaMessage
    {
        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        string MediaId { get; set; }
    }

    /// <summary>
    /// 一个抽象的消息标识接口。
    /// </summary>
    public interface IMessageIdentity
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        long MessageId { get; set; }
    }
}