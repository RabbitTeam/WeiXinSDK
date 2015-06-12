using Rabbit.WeiXin.Messages.Events.CustomService;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event.CustomService
{
    internal sealed class CreateSessionMessageFormatter : XmlMessageFormatterBase<CreateSessionMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CustomServiceCreateSessionMessage>

        public override CreateSessionMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CreateSessionMessage
            {
                Account = GetValue(container, "KfAccount")
            });
        }

        public override string Serialize(CreateSessionMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CustomServiceCreateSessionMessage>
    }
}