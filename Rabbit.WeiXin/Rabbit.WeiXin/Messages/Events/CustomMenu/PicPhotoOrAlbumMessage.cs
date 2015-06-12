namespace Rabbit.WeiXin.Messages.Events.CustomMenu
{
    /// <summary>
    /// 弹出微信相册发图器的事件推送。
    /// </summary>
    public sealed class PicPhotoOrAlbumMessage : CustomMenuEventKeyMessageBase
    {
        /// <summary>
        /// 发送的图片数量。
        /// </summary>
        public ushort Count { get; set; }

        /// <summary>
        /// 图片Md5数组。
        /// </summary>
        public string[] PictureMd5List { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.Pic_Photo_Or_Album; }
        }

        #endregion Overrides of EventMessageBase
    }
}