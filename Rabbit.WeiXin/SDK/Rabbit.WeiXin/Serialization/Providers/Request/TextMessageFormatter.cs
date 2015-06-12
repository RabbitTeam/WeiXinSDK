using Rabbit.WeiXin.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    internal sealed class TextMessageFormatter : XmlMessageFormatterBase<RequestMessageText>
    {
        #region Overrides of MessageFormatterBase<RequestMessageText>

        public override RequestMessageText Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageText
            {
                Content = GetValue(container, "Content")
            });
        }

        public override string Serialize(RequestMessageText graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of MessageFormatterBase<RequestMessageText>
    }
}