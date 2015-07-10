using Rabbit.WeiXin.MP.Messages.Events.Card;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.Card
{
    internal sealed class CardEventUserConsumeCardMessageFormatter : XmlMessageFormatterBase<CardEventUserConsumeCardMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CardEventUserConsumeCardMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override CardEventUserConsumeCardMessage Deserialize(XContainer container)
        {
            CardEventUserConsumeCardMessage.CardConsumeSource consumeSource;

            var consumeSourceString = GetValue(container, "ConsumeSource");
            switch (consumeSourceString)
            {
                case "(FROM_API)":
                    consumeSource = CardEventUserConsumeCardMessage.CardConsumeSource.Api;
                    break;

                case "(FROM_MP)":
                    consumeSource = CardEventUserConsumeCardMessage.CardConsumeSource.Mp;
                    break;

                case "(FROM_MOBILE_HELPER)":
                    consumeSource = CardEventUserConsumeCardMessage.CardConsumeSource.MobileHelper;
                    break;

                default:
                    throw new NotSupportedException(string.Format("不支持的消费源类型：{0}。", consumeSourceString));
            }

            return SetBaseInfo(container, new CardEventUserConsumeCardMessage
            {
                CardId = GetValue(container, "CardId"),
                UserCardCode = GetValue(container, "UserCardCode"),
                ConsumeSource = consumeSource
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(CardEventUserConsumeCardMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CardEventUserConsumeCardMessage>
    }
}