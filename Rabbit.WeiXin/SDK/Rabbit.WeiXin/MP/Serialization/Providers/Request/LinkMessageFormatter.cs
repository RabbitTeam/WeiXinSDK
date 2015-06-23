using Rabbit.WeiXin.MP.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Request
{
    internal sealed class LinkMessageFormatter : XmlMessageFormatterBase<RequestMessageLink>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageLink>

        public override RequestMessageLink Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageLink
            {
                Title = GetValue(container, "Title"),
                Description = GetValue(container, "Description"),
                Url = GetUri(container, "Url")
            });
        }

        public override string Serialize(RequestMessageLink graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageLink>
    }
}