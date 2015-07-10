using Rabbit.WeiXin.MP.Messages.Events.Card;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.Card
{
    internal sealed class CardEventNotPassCheckMessageFormatter : XmlMessageFormatterBase<CardEventNotPassCheckMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CardEventNotPassCheckMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override CardEventNotPassCheckMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CardEventNotPassCheckMessage
            {
                CardId = GetValue(container, "CardId")
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(CardEventNotPassCheckMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CardEventNotPassCheckMessage>
    }
}