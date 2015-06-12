using System;
using System.Web.Mvc;

namespace Rabbit.WeiXin.MvcExtension.Results
{
    /// <summary>
    /// 微信结果。
    /// </summary>
    public class WeiXinResult : ContentResult
    {
        #region Constructor

        /// <summary>
        /// 初始化一个空的微信结果。
        /// </summary>
        internal WeiXinResult()
        {
            ContentType = "text/xml";
            Content = string.Empty;
        }

        /// <summary>
        /// 初始化一个新的微信结果。
        /// </summary>
        /// <param name="content">响应内容。</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> 为null。</exception>
        public WeiXinResult(string content)
            : this()
        {
            Content = content;
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// 一个空的结果。
        /// </summary>
        public static WeiXinResult Empty = new WeiXinResult();

        #endregion Public Method
    }
}