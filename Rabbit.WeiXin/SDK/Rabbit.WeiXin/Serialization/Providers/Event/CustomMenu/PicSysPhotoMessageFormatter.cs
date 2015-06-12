using Rabbit.WeiXin.Messages.Events.CustomMenu;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Event.CustomMenu
{
    internal sealed class PicSysPhotoMessageFormatter : XmlMessageFormatterBase<PicSysPhotoMessage>
    {
        #region Overrides of XmlMessageFormatterBase<PopSystemPhotoCustomMenuEventMessage>

        public override PicSysPhotoMessage Deserialize(XContainer container)
        {
            var info = container.Element("SendPicsInfo");
            if (info == null)
                throw new ArgumentException("找不到 SendPicsInfo 元素。");

            return SetBaseInfo(container, new PicSysPhotoMessage
            {
                EventKey = GetValue(container, "EventKey"),
                Count = GetUShort(info, "Count"),
                PictureMd5List = info.Element("PicList").Elements().Select(i => GetValue(i, "PicMd5Sum")).ToArray()
            });
        }

        public override string Serialize(PicSysPhotoMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<PopSystemPhotoCustomMenuEventMessage>
    }
}