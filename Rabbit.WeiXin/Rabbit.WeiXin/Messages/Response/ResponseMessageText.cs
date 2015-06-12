using Rabbit.WeiXin.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 文本响应消息。
    /// </summary>
    public sealed class ResponseMessageText : ResponseMessageBase
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的文本响应消息。
        /// </summary>
        public ResponseMessageText()
        {
        }

        /// <summary>
        /// 初始化一个新的文本响应消息。
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> 为空。</exception>
        public ResponseMessageText(string content)
        {
            Content = content.NotEmptyOrWhiteSpace("content");
        }

        #endregion Constructor

        /// <summary>
        /// 文本消息内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.Text; }
        }

        #endregion Overrides of ResponseMessageBase
    }
}