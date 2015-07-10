namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 用户在卡券里点击查看公众号进入会话时（需要用户已经关注公众号）事件消息。
    /// </summary>
    public sealed class CardEventUserEnterSessionMessage : CardEventBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_UserEnterSession; } }

        #endregion Overrides of EventMessageBase

        /// <summary>
        /// code序列号。自定义code及非自定义code的卡券被领取后都支持事件推送。
        /// </summary>
        public string UserCardCode { get; set; }
    }
}