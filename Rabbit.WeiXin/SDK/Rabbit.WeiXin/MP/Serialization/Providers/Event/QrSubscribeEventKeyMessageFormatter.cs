using Rabbit.WeiXin.MP.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event
{
    internal sealed class QrSubscribeEventKeyMessageFormatter : XmlMessageFormatterBase<QrSubscribeEventKeyMessage>
    {
        #region Overrides of XmlMessageFormatterBase<QrSubscribeEventKeyMessage>

        public override QrSubscribeEventKeyMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new QrSubscribeEventKeyMessage
            {
                EventKey = GetValue(container, "EventKey"),
                Ticket = GetValue(container, "Ticket")
            });
        }

        public override string Serialize(QrSubscribeEventKeyMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<QrSubscribeEventKeyMessage>
    }
}