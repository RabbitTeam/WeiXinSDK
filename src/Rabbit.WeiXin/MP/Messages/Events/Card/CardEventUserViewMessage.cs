namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 用户在进入会员卡时事件消息。
    /// </summary>
    public sealed class CardEventUserViewMessage : CardEventBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_User_View; } }

        #endregion Overrides of EventMessageBase

        /// <summary>
        /// code序列号。自定义code及非自定义code的卡券被领取后都支持事件推送。
        /// </summary>
        public string UserCardCode { get; set; }
    }
}