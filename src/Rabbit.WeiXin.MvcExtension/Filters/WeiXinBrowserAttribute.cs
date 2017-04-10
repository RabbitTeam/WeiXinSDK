using System;

#if NET
using System.Web.Mvc;
#else

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

#endif

namespace Rabbit.WeiXin.MvcExtension.Filters
{
    /// <summary>
    /// 过滤没有使用微信浏览器的请求。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class WeiXinBrowserAttribute : ActionFilterAttribute
    {
        #region Field

        private readonly string _message;

        #endregion Field

        #region Property

        /// <summary>
        /// 如果不是使用微信浏览器请求则跳转到该地址（如果设置了该地址则优先使用跳转地址）。
        /// </summary>
        public string RedirectUrl { get; set; }

        #endregion Property

        #region Constructor

        /// <summary>
        /// 初始化一个新捣微信浏览器过滤器。
        /// </summary>
        /// <param name="message">如果不是微信浏览器则显示该消息内容。</param>
        public WeiXinBrowserAttribute(string message)
        {
            _message = message;
        }

        #endregion Constructor

        #region Overrides of ActionFilterAttribute

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if NET
            var userAgent = filterContext.HttpContext.Request.UserAgent;
#else
            var userAgent = filterContext.HttpContext.Request.Headers["User-Agent"].ToString();
#endif
            if (string.IsNullOrEmpty(userAgent) || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
            {
                if (!string.IsNullOrWhiteSpace(RedirectUrl))
                {
                    filterContext.Result = new RedirectResult(RedirectUrl);
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = _message };
                }
            }
        }

        #endregion Overrides of ActionFilterAttribute
    }
}