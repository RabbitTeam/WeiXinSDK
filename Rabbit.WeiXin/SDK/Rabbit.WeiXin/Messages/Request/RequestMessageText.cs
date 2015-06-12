namespace Rabbit.WeiXin.Messages.Request
{
    /// <summary>
    /// 文本请求消息。
    /// </summary>
    public class RequestMessageText : RequestMessageBase
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override RequestMessageType MessageType
        {
            get { return RequestMessageType.Text; }
        }

        #endregion Overrides of MessageBase
    }
}