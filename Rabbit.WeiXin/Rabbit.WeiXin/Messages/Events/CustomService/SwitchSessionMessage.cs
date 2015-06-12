namespace Rabbit.WeiXin.Messages.Events.CustomService
{
    /// <summary>
    /// 转接会话通知消息。
    /// </summary>
    public sealed class SwitchSessionMessage : EventMessageBase
    {
        /// <summary>
        /// 来自客服账号。
        /// </summary>
        public string FromAccount { get; set; }

        /// <summary>
        /// 转接客服的账号。
        /// </summary>
        public string ToAccount { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.KF_Switch_Session; }
        }

        #endregion Overrides of EventMessageBase
    }
}