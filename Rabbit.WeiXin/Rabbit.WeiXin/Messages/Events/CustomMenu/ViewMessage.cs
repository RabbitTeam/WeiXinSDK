namespace Rabbit.WeiXin.Messages.Events.CustomMenu
{
    /// <summary>
    /// 自定义菜单跳转消息。
    /// </summary>
    public sealed class ViewMessage : CustomMenuEventKeyMessageBase
    {
        #region Overrides of EventMessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public override EventType EventType
        {
            get { return EventType.View; }
        }

        #endregion Overrides of EventMessageBase
    }
}