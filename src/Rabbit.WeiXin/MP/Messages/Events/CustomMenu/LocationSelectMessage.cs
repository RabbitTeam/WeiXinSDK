namespace Rabbit.WeiXin.MP.Messages.Events.CustomMenu
{
    /// <summary>
    /// 弹出地理位置选择器的事件推送消息。
    /// </summary>
    public sealed class LocationSelectMessage : CustomMenuEventKeyMessageBase
    {
        /// <summary>
        /// X坐标信息。
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标信息。
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 精度，可理解为精度或者比例尺、越精细的话 scale越高。
        /// </summary>
        public uint Scale { get; set; }

        /// <summary>
        /// 地理位置的字符串信息。
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 朋友圈POI的名字，可能为空。
        /// </summary>
        public string Poiname { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.Location_Select; }
        }

        #endregion Overrides of EventMessageBase
    }
}