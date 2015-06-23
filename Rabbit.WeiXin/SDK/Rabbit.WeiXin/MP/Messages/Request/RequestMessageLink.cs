using System;

namespace Rabbit.WeiXin.MP.Messages.Request
{
    /// <summary>
    /// 链接请求消息。
    /// </summary>
    public class RequestMessageLink : RequestMessageBase
    {
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 消息链接
        /// </summary>
        public Uri Url { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override RequestMessageType MessageType
        {
            get { return RequestMessageType.Link; }
        }

        #endregion Overrides of MessageBase
    }
}