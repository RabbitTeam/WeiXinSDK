namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 用户摇一摇事件。
    /// </summary>
    public sealed class ShakearoundUserShakeMessage : EventMessageBase
    {
        /// <summary>
        /// Beacon设备的信息。
        /// </summary>
        public sealed class BeaconInfo
        {
            /// <summary>
            /// 通用唯一识别码。
            /// </summary>
            public string Uuid { get; set; }

            /// <summary>
            /// 主要的。
            /// </summary>
            public string Major { get; set; }

            /// <summary>
            /// 次要的。
            /// </summary>
            public string Minor { get; set; }

            /// <summary>
            /// 距离（米）。
            /// </summary>
            public double Distance { get; set; }
        }

        /// <summary>
        /// 选中的 Beacon 设备信息。
        /// </summary>
        public BeaconInfo ChosenBeacon { get; set; }

        /// <summary>
        /// 周围的 Beacon 设备信息。
        /// </summary>
        public BeaconInfo[] AroundBeacons { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.ShakearoundUserShake; } }

        #endregion Overrides of EventMessageBase
    }
}