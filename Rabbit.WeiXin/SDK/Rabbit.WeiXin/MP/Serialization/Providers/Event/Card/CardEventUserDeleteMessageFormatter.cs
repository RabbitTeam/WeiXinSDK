using Rabbit.WeiXin.MP.Messages.Events.Card;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.Card
{
    internal sealed class CardEventUserDeleteMessageFormatter : XmlMessageFormatterBase<CardEventUserDeleteMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CardEventUserDeleteMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override CardEventUserDeleteMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CardEventUserDeleteMessage
            {
                CardId = GetValue(container, "CardId"),
                UserCardCode = GetValue(container, "UserCardCode")
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(CardEventUserDeleteMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CardEventUserDeleteMessage>
    }
}