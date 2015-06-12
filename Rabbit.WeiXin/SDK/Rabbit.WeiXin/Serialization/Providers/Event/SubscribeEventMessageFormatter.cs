using Rabbit.WeiXin.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event
{
    internal sealed class SubscribeEventMessageFormatter : XmlMessageFormatterBase<SubscribeEventMessage>
    {
        #region Overrides of XmlMessageFormatterBase<SubscribeEventMessage>

        public override SubscribeEventMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new SubscribeEventMessage());
        }

        public override string Serialize(SubscribeEventMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<SubscribeEventMessage>
    }
}