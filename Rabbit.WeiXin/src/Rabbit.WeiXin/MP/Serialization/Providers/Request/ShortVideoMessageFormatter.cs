using Rabbit.WeiXin.MP.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Request
{
    internal sealed class ShortVideoMessageFormatter : XmlMessageFormatterBase<RequestMessageShortVideo>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageShortVideo>

        public override RequestMessageShortVideo Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageShortVideo
            {
                ThumbMediaId = GetValue(container, "ThumbMediaId")
            });
        }

        public override string Serialize(RequestMessageShortVideo graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageShortVideo>
    }
}