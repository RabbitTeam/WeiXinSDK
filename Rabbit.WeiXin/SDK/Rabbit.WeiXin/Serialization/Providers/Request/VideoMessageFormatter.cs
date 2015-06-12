using Rabbit.WeiXin.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    internal sealed class VideoMessageFormatter : XmlMessageFormatterBase<RequestMessageVideo>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageVideo>

        public override RequestMessageVideo Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageVideo
            {
                ThumbMediaId = GetValue(container, "ThumbMediaId")
            });
        }

        public override string Serialize(RequestMessageVideo graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageVideo>
    }
}