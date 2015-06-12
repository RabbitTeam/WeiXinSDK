using Rabbit.WeiXin.Utility.Extensions;
using System;

namespace Rabbit.WeiXin.Handlers
{
    /// <summary>
    /// 消息处理的基本信息。
    /// </summary>
    public sealed class MessageHandlerBaseInfo
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的消息处理基本信息。
        /// </summary>
        /// <param name="appId">应用Id。</param>
        /// <param name="encodingAesKey">消息加解密密钥。</param>
        /// <param name="token">令牌。</param>
        /// <exception cref="ArgumentNullException"><paramref name="appId"/> 为空。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="encodingAesKey"/> 为空。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="token"/> 为空。</exception>
        public MessageHandlerBaseInfo(string appId, string encodingAesKey, string token)
        {
            AppId = appId.NotEmptyOrWhiteSpace("appId");
            EncodingAesKey = encodingAesKey.NotEmptyOrWhiteSpace("encodingAesKey");
            Token = token.NotEmptyOrWhiteSpace("token");
        }

        #endregion Constructor

        /// <summary>
        /// 应用ID。
        /// </summary>
        public string AppId { get; private set; }

        /// <summary>
        /// 消息加解密密钥。
        /// </summary>
        public string EncodingAesKey { get; private set; }

        /// <summary>
        /// 令牌。
        /// </summary>
        public string Token { get; private set; }
    }
}