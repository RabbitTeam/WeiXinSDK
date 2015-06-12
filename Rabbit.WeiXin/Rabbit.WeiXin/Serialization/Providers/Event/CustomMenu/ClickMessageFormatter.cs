using Rabbit.WeiXin.Messages.Events.CustomMenu;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event.CustomMenu
{
    internal sealed class ClickMessageFormatter : XmlMessageFormatterBase<ClickMessage>
    {
        #region Overrides of XmlMessageFormatterBase<ClickCustomMenuEventKeyMessage>

        public override ClickMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new ClickMessage
            {
                EventKey = GetValue(container, "EventKey")
            });
        }

        public override string Serialize(ClickMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<ClickCustomMenuEventKeyMessage>
    }
}