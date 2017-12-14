namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 订阅事件消息。
    /// </summary>
    public sealed class QrSubscribeEventKeyMessage : QrEventKeyMessageBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.Subscribe; }
        }

        /// <summary>
        /// 扫码的Secene值
        /// </summary>
        public string SeceneValue => EventKey.Substring(8);

        #endregion Overrides of EventMessageBase
    }
}