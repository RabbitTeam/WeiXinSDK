using Rabbit.WeiXin.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Response
{
    internal sealed class ImageMessageFormatter : XmlMessageFormatterBase<ResponseMessageImage>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageImage>

        public override ResponseMessageImage Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageImage graph)
        {
            return SerializeAction(graph, builder => builder
                .Append("<Image>")
                .AppendFormat("<MediaId><![CDATA[{0}]]></MediaId>", graph.MediaId)
                .Append("</Image>"));
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageImage>
    }
}