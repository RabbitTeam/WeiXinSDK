namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 卡券被核销时事件。
    /// </summary>
    public sealed class CardEventUserConsumeCardMessage : CardEventBase
    {
        /// <summary>
        /// 卡券核销源。
        /// </summary>
        public enum CardConsumeSource
        {
            /// <summary>
            /// API核销（FROM_API）。
            /// </summary>
            Api = 0,

            /// <summary>
            /// 公众平台核销（FROM_MP）。
            /// </summary>
            Mp = 1,

            /// <summary>
            /// 卡券商户助手核销（核销员微信号）（FROM_MOBILE_HELPER）。
            /// </summary>
            MobileHelper = 2
        }

        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_User_Consume; } }

        #endregion Overrides of EventMessageBase

        /// <summary>
        /// code序列号。自定义code及非自定义code的卡券被领取后都支持事件推送。
        /// </summary>
        public string UserCardCode { get; set; }

        /// <summary>
        /// 核销来源。
        /// </summary>
        public CardConsumeSource ConsumeSource { get; set; }
    }
}