using Rabbit.WeiXin.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    internal sealed class LocationMessageFormatter : XmlMessageFormatterBase<RequestMessageLocation>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageLocation>

        public override RequestMessageLocation Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageLocation
            {
                X = GetDouble(container, "Location_X"),
                Y = GetDouble(container, "Location_Y"),
                Scale = GetInt(container, "Scale"),
                Label = GetValue(container, "Label")
            });
        }

        public override string Serialize(RequestMessageLocation graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageLocation>
    }
}