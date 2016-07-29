using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Response
{
    internal sealed class VideoMessageFormatter : XmlMessageFormatterBase<ResponseMessageVideo>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageVideo>

        public override ResponseMessageVideo Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageVideo graph)
        {
            return SerializeAction(graph, builder => builder
                .Append("<Video>")
                .AppendFormat("<MediaId><![CDATA[{0}]]></MediaId>", graph.MediaId)
                .AppendFormat("<Title><![CDATA[{0}]]></Title>", graph.Title)
                .AppendFormat("<Description><![CDATA[{0}]]></Description>", graph.Description)
                .Append("</Video>"));
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageVideo>
    }
}