namespace Rabbit.WeiXin.MP.Messages.Events.Card
{
    /// <summary>
    /// 卡券事件基础。
    /// </summary>
    public abstract class CardEventBase : EventMessageBase
    {
        /// <summary>
        /// 卡券Id。
        /// </summary>
        public string CardId { get; set; }
    }
}