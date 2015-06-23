using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.CustomMenu
{
    internal sealed class PicPhotoOrAlbumMessageFormatter : XmlMessageFormatterBase<PicPhotoOrAlbumMessage>
    {
        #region Overrides of XmlMessageFormatterBase<PicPhotoOrAlbumMessage>

        public override PicPhotoOrAlbumMessage Deserialize(XContainer container)
        {
            var info = container.Element("SendPicsInfo");
            if (info == null)
                throw new ArgumentException("找不到 SendPicsInfo 元素。");

            return SetBaseInfo(container, new PicPhotoOrAlbumMessage
            {
                EventKey = GetValue(container, "EventKey"),
                Count = GetUShort(info, "Count"),
                PictureMd5List = info.Element("PicList").Elements().Select(i => GetValue(i, "PicMd5Sum")).ToArray()
            });
        }

        public override string Serialize(PicPhotoOrAlbumMessage graph)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<PicPhotoOrAlbumMessage>
    }
}