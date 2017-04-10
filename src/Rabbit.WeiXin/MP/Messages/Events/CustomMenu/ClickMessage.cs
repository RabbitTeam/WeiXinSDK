namespace Rabbit.WeiXin.MP.Messages.Events.CustomMenu
{
    /// <summary>
    /// 自定义菜单点击消息。
    /// </summary>
    public sealed class ClickMessage : CustomMenuEventKeyMessageBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.Click; }
        }

        #endregion Overrides of EventMessageBase
    }
}