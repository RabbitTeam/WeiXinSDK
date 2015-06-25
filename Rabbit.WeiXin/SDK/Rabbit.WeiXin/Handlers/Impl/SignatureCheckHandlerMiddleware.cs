using Rabbit.WeiXin.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 签名验证中间件。
    /// </summary>
    public sealed class SignatureCheckHandlerMiddleware : HandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public SignatureCheckHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            var request = context.Request;
            var signature = request.QueryString["signature"];
            var timestamp = request.QueryString["timestamp"];
            var nonce = request.QueryString["nonce"];
            var token = context.GetMessageHandlerBaseInfo().Token;

            var dependencyResolver = context.GetDependencyResolver();
            var signatureService = dependencyResolver.GetService<ISignatureService>();
            if (!signatureService.Check(signature, timestamp, nonce, token))
                throw new Exception("非法请求。");

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware
    }
}