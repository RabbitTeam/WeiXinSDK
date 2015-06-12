using ChunSun.PublicPlatform.Services.Utility;
using Rabbit.WeiXin.Messages.Response;
using System;
using System.Text;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Response
{
    internal sealed class TransferCustomerServiceMessageFormatter : XmlMessageFormatterBase<ResponseMessageTransferCustomerService>
    {
        #region Overrides of XmlMessageFormatterBase<ResponseMessageTransferCustomerService>

        public override ResponseMessageTransferCustomerService Deserialize(XContainer container)
        {
            throw new NotImplementedException();
        }

        public override string Serialize(ResponseMessageTransferCustomerService graph)
        {
            var builder = new StringBuilder();
            builder
                .Append("<xml>")
                .AppendFormat("<ToUserName><![CDATA[{0}]]></ToUserName>", graph.ToUserName)
                .AppendFormat("<FromUserName><![CDATA[{0}]]></FromUserName>", graph.FromUserName)
                .AppendFormat("<CreateTime><![CDATA[{0}]]></CreateTime>",
                    DateTimeHelper.GetTimeStampByTime(graph.CreateTime))
                .AppendFormat("<MsgType><![CDATA[transfer_customer_service]]></MsgType>")
                .Append("</xml>");

            return builder.ToString();
        }

        #endregion Overrides of XmlMessageFormatterBase<ResponseMessageTransferCustomerService>
    }
}