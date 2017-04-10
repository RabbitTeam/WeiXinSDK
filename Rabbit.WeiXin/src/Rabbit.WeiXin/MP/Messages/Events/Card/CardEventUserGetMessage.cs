namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 用户在领取卡券时事件消息。
    /// </summary>
    public sealed class CardEventUserGetMessage : CardEventBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_User_Get; } }

        #endregion Overrides of EventMessageBase

        /// <summary>
        /// 是否为转赠。
        /// </summary>
        public bool IsGiveByFriend { get; set; }

        /// <summary>
        /// code序列号。自定义code及非自定义code的卡券被领取后都支持事件推送。
        /// </summary>
        public string UserCardCode { get; set; }

        /// <summary>
        /// 转赠前的code序列号。
        /// </summary>
        public string OldUserCardCode { get; set; }

        /// <summary>
        /// 领取场景值。
        /// </summary>
        public int OuterId { get; set; }

        /// <summary>
        /// 赠送方账号（一个OpenID），<see cref="IsGiveByFriend"/> 为true时填写该参数。
        /// </summary>
        public string FriendUserName { get; set; }
    }
}