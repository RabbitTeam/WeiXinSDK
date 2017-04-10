using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Response
{
    internal sealed class VoiceMessageFormatter : XmlMessageFormatterBase<ResponseMessageVoice>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageVoice>

        public override ResponseMessageVoice Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageVoice graph)
        {
            return SerializeAction(graph, builder => builder
                .Append("<Voice>")
                .AppendFormat("<MediaId><![CDATA[{0}]]></MediaId>", graph.MediaId)
                .Append("</Voice>"));
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageVoice>
    }
}