using Rabbit.WeiXin.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 音乐响应消息。
    /// </summary>
    public sealed class ResponseMessageMusic : ResponseMessageBase
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的音乐响应消息。
        /// </summary>
        public ResponseMessageMusic()
        {
        }

        /// <summary>
        /// 初始化一个新的音乐响应消息。
        /// </summary>
        /// <param name="thumbMediaId">缩略图的媒体id，通过上传多媒体文件，得到的id</param>
        /// <param name="title">音乐标题</param>
        /// <param name="description">音乐描述</param>
        /// <param name="musicUrl">音乐链接</param>
        /// <param name="hqMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
        /// <exception cref="ArgumentNullException"><paramref name="thumbMediaId"/> 为空。</exception>
        public ResponseMessageMusic(string thumbMediaId, string title = null, string description = null, Uri musicUrl = null, Uri hqMusicUrl = null)
        {
            ThumbMediaId = thumbMediaId.NotEmptyOrWhiteSpace("thumbMediaId");
            Title = title;
            Description = description;
            MusicUrl = musicUrl;
            HqMusicUrl = hqMusicUrl;
        }

        #endregion Constructor

        /// <summary>
        /// 音乐标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 音乐描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 音乐链接
        /// </summary>
        public Uri MusicUrl { get; set; }

        /// <summary>
        /// 高质量音乐链接，WIFI环境优先使用该链接播放音乐
        /// </summary>
        public Uri HqMusicUrl { get; set; }

        /// <summary>
        /// 缩略图的媒体id，通过上传多媒体文件，得到的id
        /// </summary>
        [Required]
        public string ThumbMediaId { get; set; }

        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.Music; }
        }

        #endregion Overrides of ResponseMessageBase
    }
}