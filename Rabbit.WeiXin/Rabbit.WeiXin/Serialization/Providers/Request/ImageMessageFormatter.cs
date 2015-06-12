using Rabbit.WeiXin.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    internal sealed class ImageMessageFormatter : XmlMessageFormatterBase<RequestMessageImage>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageImage>

        public override RequestMessageImage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageImage
            {
                PicUrl = GetUri(container, "PicUrl")
            });
        }

        public override string Serialize(RequestMessageImage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageImage>
    }
}