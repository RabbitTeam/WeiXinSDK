using Rabbit.WeiXin.Messages.Request;
using Rabbit.WeiXin.Serialization;
using Rabbit.WeiXin.Utility;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Messages
{
    /// <summary>
    /// 一个抽象的请求消息工厂。
    /// </summary>
    public interface IRequestMessageFactory
    {
        /// <summary>
        /// 创建请求消息实例。
        /// </summary>
        /// <param name="xmlContent">xml内容。</param>
        /// <returns>请求消息。</returns>
        IRequestMessageBase CreateRequestMessage(string xmlContent);
    }

    /// <summary>
    /// 请求消息工厂实现。
    /// </summary>
    public sealed class RequestMessageFactory : IRequestMessageFactory
    {
        #region Implementation of IRequestMessageFactory

        /// <summary>
        /// 创建请求消息实例。
        /// </summary>
        /// <param name="xmlContent">xml内容。</param>
        /// <returns>请求消息。</returns>
        public IRequestMessageBase CreateRequestMessage(string xmlContent)
        {
            Func<string, XContainer> getRoot = content =>
            {
                var document = XDocument.Parse(content);
                var container = document.Element("xml");
                if (container == null)
                    throw new ArgumentException("找不到根元素 xml。");
                return container;
            };

            var root = getRoot(xmlContent);

            var requestMessageType = GetRequestMessageType(root);
            return (IRequestMessageBase)MessageFormatterFactory.Factory.GetFormatter(requestMessageType).Deserialize(root);
        }

        #endregion Implementation of IRequestMessageFactory

        #region Private Method

        private static RequestMessageType GetRequestMessageType(XContainer container)
        {
            return GetType<RequestMessageType>(container, "MsgType");
        }

        private static T GetType<T>(XContainer container, string elementName) where T : struct
        {
            var typeElemment = container.Element(elementName);
            if (typeElemment == null)
                throw new ArgumentException("找不到元素 MsgType。");

            var type = typeElemment.Value;

            T value;
            if (!Enum.TryParse(type, true, out value))
                throw new NotSupportedException(string.Format("无法将 {0} 转换为指定的类型 {1}。", type, typeof(T).FullName));

            return value;
        }

        #endregion Private Method
    }

    /// <summary>
    /// 请求消息工厂扩展方法。
    /// </summary>
    public static class RequestMessageFactoryExtensions
    {
        /// <summary>
        /// 创建请求消息实例。
        /// </summary>
        /// <typeparam name="T">请求消息类型。</typeparam>
        /// <param name="requestMessageFactory">请求消息工厂。</param>
        /// <param name="xmlContent">xml内容。</param>
        /// <returns>请求消息。</returns>
        public static T CreateRequestMessage<T>(this IRequestMessageFactory requestMessageFactory, string xmlContent) where T : class,IMessageBase
        {
            return requestMessageFactory.CreateRequestMessage(xmlContent) as T;
        }

        /// <summary>
        /// 创建请求消息实例。
        /// </summary>
        /// <param name="requestMessageFactory">请求消息工厂。</param>
        /// <param name="stream">数据流。</param>
        /// <returns>请求消息。</returns>
        public static IMessageBase CreateRequestMessage(this IRequestMessageFactory requestMessageFactory, Stream stream)
        {
            stream.NotNull("stream");
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            var content = Encoding.UTF8.GetString(bytes);

            return requestMessageFactory.CreateRequestMessage(content);
        }
    }
}