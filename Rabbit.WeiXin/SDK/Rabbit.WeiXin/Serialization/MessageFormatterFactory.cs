using Rabbit.WeiXin.Messages.Request;
using Rabbit.WeiXin.Messages.Response;
using Rabbit.WeiXin.Serialization.Providers.Request;
using System;
using System.Collections.Generic;
using response = Rabbit.WeiXin.Serialization.Providers.Response;

namespace Rabbit.WeiXin.Serialization
{
    /// <summary>
    /// 消息格式化器工厂。
    /// </summary>
    public class MessageFormatterFactory
    {
        #region Field

        private static readonly MessageFormatterFactory Instance = new MessageFormatterFactory();

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
            {ResponseMessageType.Voice, new response.VoiceMessageFormatter()}
        };

        public static MessageFormatterFactory Factory { get { return Instance; } }

        #endregion Field

        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="requestMessageType">请求消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">请求消息的类型不被支持。</exception>
        public IMessageFormatter GetFormatter(RequestMessageType requestMessageType)
        {
            if (!RequestMessageFormatterDictionary.ContainsKey(requestMessageType))
                throw new NotSupportedException(string.Format("不支持的请求消息类型：{0}。", requestMessageType));
            return RequestMessageFormatterDictionary[requestMessageType];
        }

        /// <summary>
        /// 根据请求消息类型得到一个对应的格式化器。
        /// </summary>
        /// <param name="responseMessageType">响应消息类型。</param>
        /// <returns>一个可用消息格式化器。</returns>
        /// <exception cref="NotSupportedException">响应消息的类型不被支持。</exception>
        public IMessageFormatter GetFormatter(ResponseMessageType responseMessageType)
        {
            if (!ResponseMessageFormatterDictionary.ContainsKey(responseMessageType))
                throw new NotSupportedException(string.Format("不支持的响应消息类型 {0}。", responseMessageType));
            return ResponseMessageFormatterDictionary[responseMessageType];
        }
    }
}