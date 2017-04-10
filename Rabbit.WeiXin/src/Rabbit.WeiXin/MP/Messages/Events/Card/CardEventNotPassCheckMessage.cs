namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 卡券审核不通过事件消息。
    /// </summary>
    public sealed class CardEventNotPassCheckMessage : CardEventBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_Not_Pass_Check; } }

        #endregion Overrides of EventMessageBase
    }
}