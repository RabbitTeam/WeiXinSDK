using Rabbit.WeiXin.MP.Messages;
using Rabbit.WeiXin.MP.Messages.Response;
using Rabbit.WeiXin.Utility;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization
{
    /// <summary>
    /// 一个抽象的消息格式化器。
    /// </summary>
    public interface IMessageFormatter
    {
        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        IMessageBase Deserialize(XContainer container);

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        string Serialize(IMessageBase graph);
    }

    /// <summary>
    /// 一个泛型消息格式化器。
    /// </summary>
    /// <typeparam name="T">消息类型。</typeparam>
    public interface IMessageFormatter<T> : IMessageFormatter where T : IMessageBase
    {
        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        new T Deserialize(XContainer container);

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        string Serialize(T graph);
    }

    /// <summary>
    /// 一个Xml消息格式化器基类。
    /// </summary>
    /// <typeparam name="T">消息类型。</typeparam>
    internal abstract class XmlMessageFormatterBase<T> : IMessageFormatter<T> where T : class, IMessageBase
    {
        #region Implementation of IMessageFormatter<T>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public abstract T Deserialize(XContainer container);

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public abstract string Serialize(T graph);

        #endregion Implementation of IMessageFormatter<T>

        #region Implementation of IMessageFormatter

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        IMessageBase IMessageFormatter.Deserialize(XContainer container)
        {
            return Deserialize(container);
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        string IMessageFormatter.Serialize(IMessageBase graph)
        {
            return Serialize(graph as T);
        }

        #endregion Implementation of IMessageFormatter

        #region Protected Method

        protected static string GetValueOrDefault(XContainer container, string name, string defaultValue)
        {
            var ele = GetElement(container, name);
            return ele == null ? defaultValue : ele.Value;
        }

        protected static string GetValue(XContainer container, string name)
        {
            var ele = GetElement(container, name);

            if (ele == null)
                throw new ArgumentException(string.Format("找不到名称为 {0} 的元素。", name));
            return ele.Value;
        }

        protected static bool GetBoolean(XContainer container, string name)
        {
            var value = GetValue(container, name);

            #region By String

            if (value.Length > 1)
            {
                switch (value.ToLower())
                {
                    case "true":
                        return true;

                    case "false":
                        return false;

                    default:
                        throw new ArgumentException(string.Format("无法将 {0} 转换为Boolean类型。", value));
                }
            }

            #endregion By String

            #region By Number

            int number;
            if (!int.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换为Boolean类型。", value));
            switch (number)
            {
                case 0:
                    return false;

                case 1:
                    return true;

                default:
                    throw new ArgumentException(string.Format("无法将 {0} 转换为Boolean类型。", value));
            }

            #endregion By Number
        }

        protected static uint GetUInt(XContainer container, string name)
        {
            var value = GetValue(container, name);
            uint number;
            if (!uint.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成int。", value));

            return number;
        }

        protected static ushort GetUShort(XContainer container, string name)
        {
            var value = GetValue(container, name);
            ushort number;
            if (!ushort.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成int。", value));

            return number;
        }

        protected static int GetInt(XContainer container, string name)
        {
            var value = GetValue(container, name);
            int number;
            if (!int.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成int。", value));

            return number;
        }

        protected static double GetDouble(XContainer container, string name)
        {
            var value = GetValue(container, name);
            double number;
            if (!double.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成double。", value));

            return number;
        }

        protected static long GetLong(XContainer container, string name)
        {
            var value = GetValue(container, name);
            long number;
            if (!long.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成long。", value));

            return number;
        }

        protected static ulong GetULong(XContainer container, string name)
        {
            var value = GetValue(container, name);
            ulong number;
            if (!ulong.TryParse(value, out number))
                throw new ArgumentException(string.Format("无法将 {0} 转换成ulong。", value));

            return number;
        }

        protected static DateTime GetDataTime(XContainer container, string name)
        {
            return DateTimeHelper.GetTimeByTimeStamp(GetULong(container, "CreateTime"));
        }

        protected static Uri GetUri(XContainer container, string name)
        {
            var value = GetValue(container, name);
            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
                throw new ArgumentException(string.Format("无法将 {0} 转换成long。", value));
            return uri;
        }

        protected T SetBaseInfo(XContainer container, T message)
        {
            message.ToUserName = GetValue(container, "ToUserName");
            message.FromUserName = GetValue(container, "FromUserName");
            message.CreateTime = GetDataTime(container, "CreateTime");

            var messageIdentity = message as IMessageIdentity;
            if (messageIdentity != null)
                messageIdentity.MessageId = GetLong(container, "MsgId");

            var mediaMessage = message as IMediaMessage;
            if (mediaMessage != null)
                mediaMessage.MediaId = GetValue(container, "MediaId");

            return message;
        }

        protected string SerializeAction(T message, Action<StringBuilder> action = null)
        {
            var builder = new StringBuilder();
            builder
                .Append("<xml>")
                .AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", message.ToUserName)
                .AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", message.FromUserName)
                .AppendFormat("<CreateTime><![CDATA[{0}]]></CreateTime>",
                    DateTimeHelper.GetTimeStampByTime(message.CreateTime));

            var responseMessage = message as IResponseMessage;
            if (responseMessage != null)
                builder.AppendFormat("<MsgType><![CDATA[{0}]]></MsgType>", responseMessage.MessageType);

            if (action != null)
                action(builder);

            builder.Append("</xml>");

            return builder.ToString();
        }

        #endregion Protected Method

        #region Private Method

        private static XElement GetElement(XContainer container, string name)
        {
            //首先直接得到元素，如果为null则忽略大小写重新尝试获取（之所以这么做是因为直接获取元素性能比后者高）。
            return container.Element(name) ??
                      container.Elements().FirstOrDefault(i => i.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Private Method
    }
}