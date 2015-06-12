namespace Rabbit.WeiXin.Messages.Events.CustomService
{
    /// <summary>
    /// 接入会话通知消息。
    /// </summary>
    public sealed class CloseSessionMessage : EventMessageBase
    {
        /// <summary>
        /// 客服账号。
        /// </summary>
        public string Account { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.KF_Create_Session; }
        }

        #endregion Overrides of EventMessageBase
    }
}