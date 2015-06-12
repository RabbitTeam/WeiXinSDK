namespace Rabbit.WeiXin.Messages.Events.CustomMenu
{
    /// <summary>
    /// 扫码事件消息。
    /// </summary>
    public sealed class ScanCodeWaitMessage : CustomMenuEventKeyMessageBase
    {
        /// <summary>
        /// 扫描类型。
        /// </summary>
        public ScanType Type { get; set; }

        /// <summary>
        /// 扫描结果，即二维码对应的字符串信息。
        /// </summary>
        public string Result { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.ScanCode_WaitMsg; }
        }

        #endregion Overrides of EventMessageBase
    }
}