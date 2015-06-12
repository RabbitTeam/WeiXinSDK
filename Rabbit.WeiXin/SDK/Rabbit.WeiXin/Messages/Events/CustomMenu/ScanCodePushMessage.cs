namespace Rabbit.WeiXin.Messages.Events.CustomMenu
{
    /// <summary>
    /// 扫描类型。
    /// </summary>
    public enum ScanType
    {
        /// <summary>
        /// 二维码。
        /// </summary>
        QrCode = 0
    }

    /// <summary>
    /// 扫码事件消息。
    /// </summary>
    public sealed class ScanCodePushMessage : CustomMenuEventKeyMessageBase
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
            get { return EventType.ScanCode_Push; }
        }

        #endregion Overrides of EventMessageBase
    }
}