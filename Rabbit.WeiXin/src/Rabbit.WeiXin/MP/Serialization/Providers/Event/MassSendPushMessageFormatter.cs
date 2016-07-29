using Rabbit.WeiXin.MP.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event
{
    internal sealed class MassSendPushMessageFormatter : XmlMessageFormatterBase<MassSendPushMessage>
    {
        #region Overrides of XmlMessageFormatterBase<GroupMessageSendPushMessage>

        public override MassSendPushMessage Deserialize(XContainer container)
        {
            var model = SetBaseInfo(container, new MassSendPushMessage
            {
                TotalCount = GetULong(container, "TotalCount"),
                FilterCount = GetULong(container, "FilterCount"),
                SentCount = GetULong(container, "SentCount"),
                ErrorCount = GetULong(container, "ErrorCount")
            });

            var status = GetValue(container, "Status");
            switch (status)
            {
                case "sendsuccess":
                    model.Status = MassSendStatus.Success;
                    break;

                //                case "send fail":
                default:
                    model.Status = MassSendStatus.Fail;
                    break;
            }
            return model;
        }

        public override string Serialize(MassSendPushMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<GroupMessageSendPushMessage>
    }
}