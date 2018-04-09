using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.MP.Messages.Response;
using Rabbit.WeiXin.MP.Serialization.Providers.Request;
using System;
using System.Collections.Generic;
using response = Rabbit.WeiXin.MP.Serialization.Providers.Response;

namespace Rabbit.WeiXin.MP.Serialization
{
    /// <summary>
    /// 一个抽象的消息格式化器工厂。
    /// </summary>
    public interface IMessageFormatterFactory
    {
        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="requestMessageType">请求消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">请求消息的类型不被支持。</exception>
        IMessageFormatter GetFormatter(RequestMessageType requestMessageType);

        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="responseMessageType">响应消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">响应消息的类型不被支持。</exception>
        IMessageFormatter GetFormatter(ResponseMessageType responseMessageType);
    }

    /// <summary>
    /// 消息格式化器工厂。
    /// </summary>
    internal sealed class MessageFormatterFactory : IMessageFormatterFactory
    {
        #region Field

        private static readonly IDictionary<RequestMessageType, IMessageFormatter> RequestMessageFormatterDictionary = new Dictionary<RequestMessageType, IMessageFormatter>
        {
            {RequestMessageType.Text, new TextMessageFormatter()},
            {RequestMessageType.Image, new ImageMessageFormatter()},
            {RequestMessageType.Link, new LinkMessageFormatter()},
            {RequestMessageType.Location, new LocationMessageFormatter()},
            {RequestMessageType.ShortVideo, new ShortVideoMessageFormatter()},
            {RequestMessageType.Video, new VideoMessageFormatter()},
            {RequestMessageType.Voice, new VoiceMessageFormatter()},
            {RequestMessageType.Event, new EventRequestMessageFormatter()}
        };

        private static readonly IDictionary<ResponseMessageType, IMessageFormatter> ResponseMessageFormatterDictionary = new Dictionary<ResponseMessageType, IMessageFormatter>
        {
            {ResponseMessageType.Image, new response.ImageMessageFormatter()},
            {ResponseMessageType.Music, new response.MusicMessageFormatter()},
            {ResponseMessageType.News, new response.NewsMessageFormatter()},
            {ResponseMessageType.Text, new response.TextMessageFormatter()},
            {ResponseMessageType.TransferCustomerServic, new response.TransferCustomerServiceMessageFormatter()},
            {ResponseMessageType.Video, new response.VideoMessageFormatter()},
            {ResponseMessageType.Voice, new response.VoiceMessageFormatter()},
            {ResponseMessageType.Empty, new response.EmptyMessageFormatter()}
        };

        #endregion Field

        #region Implementation of IMessageFormatterFactory

        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="requestMessageType">请求消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">请求消息的类型不被支持。</exception>
        public IMessageFormatter GetFormatter(RequestMessageType requestMessageType)
        {
            var formatter = RequestMessageFormatterDictionary[requestMessageType];

            if (formatter == null)
                throw new NotSupportedException(string.Format("不支持的请求消息类型：{0}。", requestMessageType));

            return formatter;
        }

        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="responseMessageType">响应消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">响应消息的类型不被支持。</exception>
        public IMessageFormatter GetFormatter(ResponseMessageType responseMessageType)
        {
            var formatter = ResponseMessageFormatterDictionary[responseMessageType];

            if (formatter == null)
                throw new NotSupportedException(string.Format("不支持的响应消息类型：{0}。", responseMessageType));

            return formatter;
        }

        #endregion Implementation of IMessageFormatterFactory
    }
}