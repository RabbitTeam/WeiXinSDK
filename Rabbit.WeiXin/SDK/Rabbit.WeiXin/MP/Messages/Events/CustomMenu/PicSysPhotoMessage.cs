namespace Rabbit.WeiXin.MP.Messages.Events.CustomMenu
{
    /// <summary>
    /// 弹出系统拍照发图的事件推送消息。
    /// </summary>
    public sealed class PicSysPhotoMessage : CustomMenuEventKeyMessageBase
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
            get { return EventType.Pic_SysPhoto; }
        }

        #endregion Overrides of EventMessageBase
    }
}