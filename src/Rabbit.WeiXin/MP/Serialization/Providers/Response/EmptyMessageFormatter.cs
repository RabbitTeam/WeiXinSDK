using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Response
{
    internal sealed class EmptyMessageFormatter : XmlMessageFormatterBase<ResponseMessageEmpty>
    {

        #region Overrides of XmlMessageFormatterBase<RequestMessageVoice>

        public override ResponseMessageEmpty Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageEmpty graph)
        {
            return "success";
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageVoice>
    }
}
