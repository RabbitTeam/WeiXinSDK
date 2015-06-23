using Rabbit.WeiXin.MP.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event
{
    internal sealed class UnSubscribeEventMessageFormatter : XmlMessageFormatterBase<UnSubscribeEventMessage>
    {
        #region Overrides of XmlMessageFormatterBase<UnSubscribeEventMessage>

        public override UnSubscribeEventMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new UnSubscribeEventMessage());
        }

        public override string Serialize(UnSubscribeEventMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<UnSubscribeEventMessage>
    }
}