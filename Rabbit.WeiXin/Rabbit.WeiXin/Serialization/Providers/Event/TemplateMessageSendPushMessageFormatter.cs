using Rabbit.WeiXin.Messages.Events;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event
{
    internal sealed class TemplateMessageSendPushMessageFormatter : XmlMessageFormatterBase<TemplateMessageSendPushMessage>
    {
        #region Overrides of XmlMessageFormatterBase<TemplateMessageSendPushMessage>

        public override TemplateMessageSendPushMessage Deserialize(XContainer container)
        {
            var model = SetBaseInfo(container, new TemplateMessageSendPushMessage());

            var status = GetValue(container, "Status");
            switch (status)
            {
                case "success":
                    model.Status = TemplateMessageSendStatus.Success;
                    break;

                case "failed:user block":
                    model.Status = TemplateMessageSendStatus.UserBlock;
                    break;

                case "failed: system failed":
                    model.Status = TemplateMessageSendStatus.SystemFailed;
                    break;

                default:
                    throw new NotSupportedException("不支持的类型：" + status);
            }

            return model;
        }

        public override string Serialize(TemplateMessageSendPushMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<TemplateMessageSendPushMessage>
    }
}