using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 语音响应消息。
    /// </summary>
    public sealed class ResponseMessageVoice : ResponseMessageBase, IMediaMessage
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的语音响应消息。
        /// </summary>
        public ResponseMessageVoice()
        {
        }

        /// <summary>
        /// 初始化一个新的语音响应消息。
        /// </summary>
        /// <param name="mediaId">媒体id，可以调用多媒体文件下载接口拉取数据。</param>
        /// <exception cref="ArgumentNullException"><paramref name="mediaId"/> 为空。</exception>
        public ResponseMessageVoice(string mediaId)
        {
            MediaId = mediaId.NotEmptyOrWhiteSpace("mediaId");
        }

        #endregion Constructor

        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.Voice; }
        }

        #endregion Overrides of ResponseMessageBase

        #region Implementation of IMediaMessage

        /// <summary>
        /// 媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        [Required]
        public string MediaId { get; set; }

        #endregion Implementation of IMediaMessage
    }
}