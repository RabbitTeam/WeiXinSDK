using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 视频响应消息。
    /// </summary>
    public sealed class ResponseMessageVideo : ResponseMessageBase, IMediaMessage
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的视频响应消息。
        /// </summary>
        public ResponseMessageVideo()
        {
        }

        /// <summary>
        /// 初始化一个新的视频响应消息。
        /// </summary>
        /// <param name="mediaId">媒体id，可以调用多媒体文件下载接口拉取数据。</param>
        /// <param name="title">视频消息的标题</param>
        /// <param name="description">视频消息的描述</param>
        /// <exception cref="ArgumentNullException"><paramref name="mediaId"/> 不能为空。</exception>
        public ResponseMessageVideo(string mediaId, string title = null, string description = null)
        {
            MediaId = mediaId.NotEmptyOrWhiteSpace("mediaId");
            Title = title;
            Description = description;
        }

        #endregion Constructor

        /// <summary>
        /// 视频消息的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频消息的描述
        /// </summary>
        public string Description { get; set; }

        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.Video; }
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