using System;

namespace Rabbit.WeiXin
{
    /// <summary>
    /// 微信异常信息。
    /// </summary>
    public class WeiXinException : Exception
    {
        /// <summary>
        /// 初始化一个新的微信异常。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误消息。</param>
        public WeiXinException(int errorCode, string message)
            : base(string.Format("调用接口失败，错误码：{0}，错误信息：{1}", errorCode, message))
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode { get; set; }
    }
}