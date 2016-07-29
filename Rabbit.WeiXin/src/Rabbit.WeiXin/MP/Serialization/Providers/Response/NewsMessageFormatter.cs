using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Response
{
    internal sealed class NewsMessageFormatter : XmlMessageFormatterBase<ResponseMessageNews>
    {
        #region Overrides of XmlMessageFormatterBase<ResponseMessageNews>

        public override ResponseMessageNews Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageNews graph)
        {
            return SerializeAction(graph, builder =>
            {
                builder.AppendFormat("<ArticleCount>{0}</ArticleCount>", graph.ArticleCount);
                builder.Append("<Articles>");
                if (graph.Articles != null)
                    foreach (var article in graph.Articles)
                    {
                        builder.AppendFormat("<item>");
                        builder.AppendFormat("<Title><![CDATA[{0}]]></Title>", article.Title);
                        builder.AppendFormat("<Description><![CDATA[{0}]]></Description>", article.Description);
                        builder.AppendFormat("<PicUrl><![CDATA[{0}]]></PicUrl>", article.PicUrl);
                        builder.AppendFormat("<Url><![CDATA[{0}]]></Url>", article.Url);
                        builder.AppendFormat("</item>");
                    }
                builder.Append("</Articles>");
            });
        }

        #endregion Overrides of XmlMessageFormatterBase<ResponseMessageNews>
    }
}