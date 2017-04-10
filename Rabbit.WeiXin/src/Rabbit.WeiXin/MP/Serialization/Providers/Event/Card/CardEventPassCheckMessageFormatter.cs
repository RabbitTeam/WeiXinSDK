using Rabbit.WeiXin.MP.Messages.Events.Card;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.Card
{
    internal sealed class CardEventPassCheckMessageFormatter : XmlMessageFormatterBase<CardEventPassCheckMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CardEventPassCheckMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override CardEventPassCheckMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CardEventPassCheckMessage
            {
                CardId = GetValue(container, "CardId")
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(CardEventPassCheckMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CardEventPassCheckMessage>
    }
}