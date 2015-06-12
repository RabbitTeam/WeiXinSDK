namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 多客服响应消息。
    /// </summary>
    public class ResponseMessageTransferCustomerService : ResponseMessageBase
    {
        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.TransferCustomerServic; }
        }

        #endregion Overrides of ResponseMessageBase
    }
}