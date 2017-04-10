using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using System;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.CustomMenu
{
    internal sealed class ScanCodePushMessageFormatter : XmlMessageFormatterBase<ScanCodePushMessage>
    {
        #region Overrides of XmlMessageFormatterBase<ScanCodePushCustomMenuEventMessage>

        public override ScanCodePushMessage Deserialize(XContainer container)
        {
            var info = container.Element("ScanCodeInfo");
            var model = SetBaseInfo(container, new ScanCodePushMessage
            {
                EventKey = GetValue(container, "EventKey"),
                Result = GetValue(info, "ScanResult")
            });
            var typeString = GetValue(info, "ScanType");
            switch (typeString)
            {
                case "qrcode":
                    model.Type = ScanType.QrCode;
                    break;

                default:
                    throw new NotSupportedException("不支持的扫码类型：" + typeString);
            }
            return model;
        }

        public override string Serialize(ScanCodePushMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<ScanCodePushCustomMenuEventMessage>
    }
}