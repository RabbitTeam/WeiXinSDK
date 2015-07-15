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
        /// 消息接收方用户名称。
        /// </summary>
        /// <remarks>如果是请求消息则是开发者微信号，如果是响应消息则是用户的OpenId。</remarks>
        [Required]
        string ToUserName { get; set; }

        /// <summary>
        /// 消息发送方用户名称。
        /// </summary>
        /// <remarks>如果是请求消息则是用户的OpenId，如果是响应消息则是开发者微信号。</remarks>
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
        /// 消息接收方用户名称。
        /// </summary>
        /// <remarks>如果是请求消息则是开发者微信号，如果是响应消息则是用户的OpenId。</remarks>
        [Required]
        public virtual string ToUserName { get; set; }

        /// <summary>
        /// 消息发送方用户名称。
        /// </summary>
        /// <remarks>如果是请求消息则是用户的OpenId，如果是响应消息则是开发者微信号。</remarks>
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