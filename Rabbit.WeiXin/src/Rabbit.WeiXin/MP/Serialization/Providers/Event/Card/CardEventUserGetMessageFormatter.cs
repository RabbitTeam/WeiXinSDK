using Rabbit.WeiXin.MP.Messages.Events.Card;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.Card
{
    internal sealed class CardEventUserGetMessageFormatter : XmlMessageFormatterBase<CardEventUserGetMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CardEventUserGetMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override CardEventUserGetMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CardEventUserGetMessage
            {
                CardId = GetValue(container, "CardId"),
                FriendUserName = GetValue(container, "FriendUserName"),
                IsGiveByFriend = GetBoolean(container, "IsGiveByFriend"),
                UserCardCode = GetValue(container, "UserCardCode"),
                OldUserCardCode = GetValueOrDefault(container, "OldUserCardCode", null),
                OuterId = GetInt(container, "OuterId")
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(CardEventUserGetMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CardEventUserGetMessage>
    }
}