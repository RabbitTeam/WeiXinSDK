using Rabbit.WeiXin.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event
{
    internal sealed class ReportingLocationEventMessageFormatter : XmlMessageFormatterBase<ReportingLocationEventMessage>
    {
        #region Overrides of XmlMessageFormatterBase<ReportingLocationEventMessage>

        public override ReportingLocationEventMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new ReportingLocationEventMessage
            {
                Latitude = GetDouble(container, "Latitude"),
                Longitude = GetDouble(container, "Longitude"),
                Precision = GetDouble(container, "Precision")
            });
        }

        public override string Serialize(ReportingLocationEventMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<ReportingLocationEventMessage>
    }
}