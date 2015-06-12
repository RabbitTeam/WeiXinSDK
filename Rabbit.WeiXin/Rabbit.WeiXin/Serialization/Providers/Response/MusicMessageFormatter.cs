using Rabbit.WeiXin.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Response
{
    internal sealed class MusicMessageFormatter : XmlMessageFormatterBase<ResponseMessageMusic>
    {
        #region Overrides of XmlMessageFormatterBase<ResponseMessageMusic>

        public override ResponseMessageMusic Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageMusic graph)
        {
            return SerializeAction(graph, builder => builder
                .Append("<Music>")
                .AppendFormat("<Title><![CDATA[{0}]]></Title>", graph.Title)
                .AppendFormat("<Description><![CDATA[{0}]]></Description>", graph.Description)
                .AppendFormat("<MusicUrl><![CDATA[{0}]]></MusicUrl>", graph.MusicUrl)
                .AppendFormat("<HQMusicUrl><![CDATA[{0}]]></HQMusicUrl>", graph.HqMusicUrl)
                .AppendFormat("<ThumbMediaId><![CDATA[{0}]]></ThumbMediaId>", graph.ThumbMediaId)
                .Append("</Music>"));
        }

        #endregion Overrides of XmlMessageFormatterBase<ResponseMessageMusic>
    }
}