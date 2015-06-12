using Rabbit.WeiXin.Messages.Request;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    internal sealed class VoiceMessageFormatter : XmlMessageFormatterBase<RequestMessageVoice>
    {
        #region Overrides of XmlMessageFormatterBase<RequestMessageVoice>

        public override RequestMessageVoice Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new RequestMessageVoice
            {
                Format = GetValue(container, "Format"),
                Recognition = GetValueOrDefault(container, "Recognition", null)
            });
        }

        public override string Serialize(RequestMessageVoice graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<RequestMessageVoice>
    }
}