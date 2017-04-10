namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 卡券通过审核事件消息。
    /// </summary>
    public sealed class CardEventPassCheckMessage : CardEventBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType { get { return EventType.Card_Pass_Check; } }

        #endregion Overrides of EventMessageBase
    }
}