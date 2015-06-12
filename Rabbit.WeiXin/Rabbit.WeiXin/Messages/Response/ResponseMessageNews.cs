using Rabbit.WeiXin.Utility;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Messages.Response
{
    /// <summary>
    /// 图文响应消息。
    /// </summary>
    public sealed class ResponseMessageNews : ResponseMessageBase
    {
        #region Field

        private const ushort ArticleMaxCount = 10;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的图文响应消息实例。
        /// </summary>
        public ResponseMessageNews()
        {
            Articles = new Article[0];
        }

        /// <summary>
        /// 初始化一个新的图文响应消息实例。
        /// </summary>
        /// <param name="articles">文章信息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="articles"/> 为 null。</exception>
        /// <exception cref="ArgumentException"><paramref name="articles"/> 长度大于 <see cref="ArticleMaxCount"/> 10。</exception>
        public ResponseMessageNews(Article[] articles)
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
            public string Title { get; set; }

            /// <summary>
            /// 图文消息描述
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// </summary>
            public Uri PicUrl { get; set; }

            /// <summary>
            /// 点击图文消息跳转链接
            /// </summary>
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

        #region Overrides of ResponseMessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType
        {
            get { return ResponseMessageType.News; }
        }

        #endregion Overrides of ResponseMessageBase

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
    }
}