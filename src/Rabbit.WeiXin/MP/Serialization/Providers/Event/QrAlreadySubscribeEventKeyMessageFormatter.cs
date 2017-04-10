using Rabbit.WeiXin.MP.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event
{
    internal sealed class QrAlreadySubscribeEventKeyMessageFormatter : XmlMessageFormatterBase<QrAlreadySubscribeEventKeyMessage>
    {
        #region Overrides of XmlMessageFormatterBase<QrAlreadySubscribeEventKeyMessage>

        public override QrAlreadySubscribeEventKeyMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new QrAlreadySubscribeEventKeyMessage
            {
                EventKey = GetValue(container, "EventKey"),
                Ticket = GetValue(container, "Ticket")
            });
        }

        public override string Serialize(QrAlreadySubscribeEventKeyMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<QrAlreadySubscribeEventKeyMessage>
    }
}