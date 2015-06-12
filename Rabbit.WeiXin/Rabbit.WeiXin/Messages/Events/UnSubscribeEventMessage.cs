namespace Rabbit.WeiXin.Messages.Events
{
    /// <summary>
    /// 取消订阅事件消息。
    /// </summary>
    public sealed class UnSubscribeEventMessage : EventMessageBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.UnSubscribe; }
        }

        #endregion Overrides of EventMessageBase
    }
}