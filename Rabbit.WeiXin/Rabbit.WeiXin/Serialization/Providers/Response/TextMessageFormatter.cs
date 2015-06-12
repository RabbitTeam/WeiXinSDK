using Rabbit.WeiXin.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Response
{
    internal sealed class TextMessageFormatter : XmlMessageFormatterBase<ResponseMessageText>
    {
        #region Overrides of MessageFormatterBase<RequestMessageText>

        public override ResponseMessageText Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageText graph)
        {
            return SerializeAction(graph, builder => builder
                .AppendFormat("<Content><![CDATA[{0}]]></Content>", graph.Content));
        }

        #endregion Overrides of MessageFormatterBase<RequestMessageText>
    }
}