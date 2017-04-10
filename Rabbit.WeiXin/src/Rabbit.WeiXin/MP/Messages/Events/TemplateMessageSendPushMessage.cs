namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 模板消息发送状态。
    /// </summary>
    public enum TemplateMessageSendStatus
    {
        /// <summary>
        /// 发送成功。
        /// </summary>
        Success = 0,

        /// <summary>
        /// 用户拒绝接收。
        /// </summary>
        UserBlock = 1,

        /// <summary>
        /// 发送状态为发送失败（非用户拒绝）。
        /// </summary>
        SystemFailed = 2
    }

    /// <summary>
    /// 模板消息发送事件推送消息。
    /// </summary>
    public class TemplateMessageSendPushMessage : EventMessageBase, IMessageIdentity
    {
        /// <summary>
        /// 发送状态。
        /// </summary>
        public TemplateMessageSendStatus Status { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.TemplateSendJobFinish; }
        }

        #endregion Overrides of EventMessageBase

        #region Implementation of IMessageIdentity

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public long MessageId { get; set; }

        #endregion Implementation of IMessageIdentity
    }
}