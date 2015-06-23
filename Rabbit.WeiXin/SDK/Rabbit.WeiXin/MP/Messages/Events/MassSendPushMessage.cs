namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 群发消息发送状态。
    /// </summary>
    public enum MassSendStatus
    {
        /// <summary>
        /// 成功。
        /// </summary>
        Success,

        /// <summary>
        /// 失败。
        /// </summary>
        Fail
    }

    /// <summary>
    /// 群发消息发送推送消息。
    /// </summary>
    public sealed class MassSendPushMessage : EventMessageBase, IMessageIdentity
    {
        /// <summary>
        /// 发送状态。
        /// </summary>
        public MassSendStatus Status { get; set; }

        /// <summary>
        /// group_id下粉丝数；或者openid_list中的粉丝数
        /// </summary>
        public ulong TotalCount { get; set; }

        /// <summary>
        /// 过滤（过滤是指特定地区、性别的过滤、用户设置拒收的过滤，用户接收已超4条的过滤）后，准备发送的粉丝数，原则上，FilterCount = SentCount + ErrorCount
        /// </summary>
        public ulong FilterCount { get; set; }

        /// <summary>
        /// 发送成功的粉丝数
        /// </summary>
        public ulong SentCount { get; set; }

        /// <summary>
        /// 发送失败的粉丝数
        /// </summary>
        public ulong ErrorCount { get; set; }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.MassSendJobFinish; }
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