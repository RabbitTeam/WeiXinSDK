using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api.CustomService
{
    /// <summary>
    /// 一个抽象的客服消息服务接口。
    /// </summary>
    public interface ICustomServiceMessageService
    {
        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">客服消息。</param>
        void Send(CustomServiceMessage message);
    }

    /// <summary>
    /// 客服消息服务实现。
    /// </summary>
    public sealed class CustomServiceMessageService : ICustomServiceMessageService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的客服消息服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public CustomServiceMessageService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICustomServiceInterfaceService

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">客服消息。</param>
        public void Send(CustomServiceMessage message)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, message.GetJson());
        }

        #endregion Implementation of ICustomServiceInterfaceService
    }

    #region Help Class

    /// <summary>
    /// 客服消息类型。
    /// </summary>
    public enum CustomServiceMessageType
    {
        /// <summary>
        /// 文本。
        /// </summary>
        Text,

        /// <summary>
        /// 图片。
        /// </summary>
        Image,

        /// <summary>
        /// 语音。
        /// </summary>
        Voice,

        /// <summary>
        /// 视频。
        /// </summary>
        Video,

        /// <summary>
        /// 音乐。
        /// </summary>
        Music,

        /// <summary>
        /// 图文。
        /// </summary>
        News,

        /// <summary>
        /// 卡券。
        /// </summary>
        Card,

        /// <summary>
        /// 微信图文素材。
        /// </summary>
        MpNews
    }

    /// <summary>
    /// 客服消息基类。
    /// </summary>
    public abstract class CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的自定义客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        protected CustomServiceMessage(string toUserId)
        {
            ToUserId = toUserId;
        }

        /// <summary>
        /// 普通用户openid
        /// </summary>
        [Required]
        public string ToUserId { get; private set; }

        /// <summary>
        /// 消息类型。
        /// </summary>
        public abstract CustomServiceMessageType Type { get; }

        /// <summary>
        /// 客服账号。
        /// </summary>
        public string CustomServiceAccount { get; set; }

        /// <summary>
        /// 获取对象的Json文本。
        /// </summary>
        /// <returns></returns>
        internal abstract string GetJson();

        /// <summary>
        /// 获取基础的Json对象。
        /// </summary>
        /// <param name="appendType">是否追加消息类型。</param>
        /// <returns></returns>
        protected JObject GetBasicJsonObject(bool appendType = true)
        {
            var obj = JObject.Parse("{}");
            obj["touser"] = ToUserId;
            if (appendType)
            {
                obj["msgtype"] = Type.ToString().ToLower();
            }
            if (!string.IsNullOrWhiteSpace(CustomServiceAccount))
            {
                obj["customservice"] = new JObject()["kf_account"] = CustomServiceAccount;
            }
            return obj;
        }
    }

    /// <summary>
    /// 文本客服消息。
    /// </summary>
    public sealed class CustomServiceMessageText : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的文本客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageText(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 文本消息内容。
        /// </summary>
        [Required]
        public string Content { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Text; }
        }

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["text"] = JObject.FromObject(new { content = Content });
            return obj.ToString();
        }

        #endregion Overrides of CustomServiceMessage
    }

    /// <summary>
    /// 图片客服消息。
    /// </summary>
    public sealed class CustomServiceMessageImage : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的图片客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageImage(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 媒体ID。
        /// </summary>
        [Required]
        public string MediaId { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Image; }
        }

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["image"] = JObject.FromObject(new { media_id = MediaId });
            return obj.ToString();
        }

        #endregion Overrides of CustomServiceMessage
    }

    /// <summary>
    /// 语音客服消息。
    /// </summary>
    public sealed class CustomServiceMessageVoice : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的语音客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageVoice(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 媒体ID。
        /// </summary>
        [Required]
        public string MediaId { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Voice; }
        }

        #endregion Overrides of CustomServiceMessage

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["voice"] = JObject.FromObject(new { media_id = MediaId });
            return obj.ToString();
        }
    }

    /// <summary>
    /// 视频客服消息。
    /// </summary>
    public sealed class CustomServiceMessageVideo : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的视频客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageVideo(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 媒体ID。
        /// </summary>
        [Required]
        public string MediaId { get; set; }

        /// <summary>
        /// 缩略图的媒体ID
        /// </summary>
        [Required]
        public string ThumbnailsMediaId { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 说明。
        /// </summary>
        public string Description { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Video; }
        }

        #endregion Overrides of CustomServiceMessage

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["video"] = JObject.FromObject(new
            {
                media_id = MediaId,
                thumb_media_id = ThumbnailsMediaId,
                title = Title,
                description = Description
            });
            return obj.ToString();
        }
    }

    /// <summary>
    /// 音乐客服消息。
    /// </summary>
    public sealed class CustomServiceMessageMusic : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的音乐客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageMusic(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 缩略图的媒体ID
        /// </summary>
        [Required]
        public string ThumbnailsMediaId { get; set; }

        /// <summary>
        /// 音乐链接
        /// </summary>
        [Required]
        public string MusicUrl { get; set; }

        /// <summary>
        /// 高品质音乐链接，wifi环境优先使用该链接播放音乐
        /// </summary>
        [Required]
        public string HqMusicUrl { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Music; }
        }

        #endregion Overrides of CustomServiceMessage

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["music"] = JObject.FromObject(new
            {
                musicurl = MusicUrl,
                hqmusicurl = HqMusicUrl,
                thumb_media_id = ThumbnailsMediaId,
                title = Title,
                description = Description
            });
            return obj.ToString();
        }
    }

    /// <summary>
    /// 图文客服消息。
    /// </summary>
    public sealed class CustomServiceMessageNews : CustomServiceMessage
    {
        #region Field

        private const ushort ArticleMaxCount = 10;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的图文客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        /// <param name="articles">图文项。</param>
        public CustomServiceMessageNews(string toUserId, Article[] articles)
            : base(toUserId)
        {
            if (articles.NotNull("articles").Length > ArticleMaxCount)
                throw new ArgumentException(string.Format("文章数量不能大于 {0} 条。", ArticleMaxCount), "articles");

            Articles = articles;
        }

        #endregion Constructor

        /// <summary>
        /// 文章类型。
        /// </summary>
        public sealed class Article
        {
            /// <summary>
            /// 图文消息标题
            /// </summary>
            [JsonProperty("title")]
            public string Title { get; set; }

            /// <summary>
            /// 图文消息描述
            /// </summary>
            [JsonProperty("description")]
            public string Description { get; set; }

            /// <summary>
            /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// </summary>
            [JsonProperty("picurl")]
            public Uri PicUrl { get; set; }

            /// <summary>
            /// 点击图文消息跳转链接
            /// </summary>
            [JsonProperty("url")]
            public Uri Url { get; set; }
        }

        /// <summary>
        /// 文章项。
        /// </summary>
        public Article[] Articles { get; private set; }

        /// <summary>
        /// 文章数量。
        /// </summary>
        public ushort ArticleCount
        {
            get
            {
                return (ushort)Articles.Length;
            }
        }

        #region Public Method

        /// <summary>
        /// 追加文章，如果 <see cref="Articles"/> 数量大于10则忽略。
        /// </summary>
        /// <param name="articles">文章数组。</param>
        public void Append(params Article[] articles)
        {
            if (articles.NotNull("articles").Length > ArticleMaxCount)
                throw new ArgumentException(string.Format("文章数量不能大于 {0} 条。", ArticleMaxCount), "articles");

            Articles = Articles == null ? articles : Articles.Concat(articles.Take(ArticleMaxCount - Articles.Length)).ToArray();
        }

        /// <summary>
        /// 清空所有文章。
        /// </summary>
        public void Clear()
        {
            Articles = null;
        }

        /// <summary>
        /// 是否允许添加文章。
        /// </summary>
        /// <returns>如果还可以添加文章则返回 true，否则返回 false。</returns>
        public bool IsAdd()
        {
            return Articles == null || Articles.Length < ArticleMaxCount;
        }

        #endregion Public Method

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.News; }
        }

        #endregion Overrides of CustomServiceMessage

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["news"] = JObject.FromObject(new
            {
                articles = Articles
            });
            return obj.ToString();
        }
    }

    /// <summary>
    /// 自定义服务消息微信图文素材消息。
    /// </summary>
    public sealed class CustomServiceMessageMpNews : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的自定义客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        /// <param name="mediaId">素材Id。</param>
        public CustomServiceMessageMpNews(string toUserId, string mediaId) : base(toUserId)
        {
            MediaId = mediaId;
        }

        /// <summary>
        /// 素材Id。
        /// </summary>
        public string MediaId { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override CustomServiceMessageType Type { get; } = CustomServiceMessageType.MpNews;

        /// <summary>
        /// 获取对象的Json文本。
        /// </summary>
        /// <returns></returns>
        internal override string GetJson()
        {
            var obj = GetBasicJsonObject();
            obj["mpnews"] = JObject.FromObject(new
            {
                media_id = MediaId
            });
            return obj.ToString();
        }

        #endregion Overrides of CustomServiceMessage
    }

    /// <summary>
    /// 卡券客服消息。
    /// </summary>
    public sealed class CustomServiceMessageCard : CustomServiceMessage
    {
        /// <summary>
        /// 初始化一个新的卡券客服消息。
        /// </summary>
        /// <param name="toUserId">接收消息的用户Id。</param>
        public CustomServiceMessageCard(string toUserId)
            : base(toUserId)
        {
        }

        /// <summary>
        /// 标识。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 文本。
        /// </summary>
        public string Text { get; set; }

        #region Overrides of CustomServiceMessage

        /// <summary>
        /// 消息类型
        /// </summary>
        public override CustomServiceMessageType Type
        {
            get { return CustomServiceMessageType.Card; }
        }

        #endregion Overrides of CustomServiceMessage

        internal override string GetJson()
        {
            var obj = GetBasicJsonObject(false);
            obj["msgtype"] = "wxcard";
            obj["wxcard"] = JObject.FromObject(new
            {
                card_id = Id,
                card_ext = Text
            });
            return obj.ToString();
        }
    }

    #endregion Help Class
}