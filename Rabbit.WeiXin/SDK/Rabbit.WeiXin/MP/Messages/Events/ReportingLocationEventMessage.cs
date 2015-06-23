namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 上报地理位置事件消息。
    /// </summary>
    public sealed class ReportingLocationEventMessage : EventMessageBase
    {
        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public double Precision { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.Location; }
        }

        #endregion Overrides of EventMessageBase
    }
}