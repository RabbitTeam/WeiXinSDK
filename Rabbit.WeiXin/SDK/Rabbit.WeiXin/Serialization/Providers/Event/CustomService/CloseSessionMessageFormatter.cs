using Rabbit.WeiXin.Messages.Events.CustomService;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event.CustomService
{
    internal sealed class CloseSessionMessageFormatter : XmlMessageFormatterBase<CloseSessionMessage>
    {
        #region Overrides of XmlMessageFormatterBase<CustomServiceCloseSessionMessage>

        public override CloseSessionMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new CloseSessionMessage
            {
                Account = GetValue(container, "KfAccount")
            });
        }

        public override string Serialize(CloseSessionMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<CustomServiceCloseSessionMessage>
    }
}