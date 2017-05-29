using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.MP.Messages;
using System.Threading.Tasks;
using Tencent;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 创建请求消息模型中间件（如果消息被加密则先进行解密操作）。
    /// </summary>
    public sealed class CreateRequestMessageHandlerMiddleware : HandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public CreateRequestMessageHandlerMiddleware(HandlerMiddleware next)
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
            var dependencyResolver = context.GetDependencyResolver();
            var requestMessageFactory = dependencyResolver.GetService<IRequestMessageFactory>();

            var content = context.Content;
            var parameters = context.GetRequestParameters();

            #region Decrypt

            if (parameters.ContainsKey("encrypt_type"))
            {
                var nonce = parameters["nonce"];
                var signature = parameters["msg_signature"];
                var timestamp = parameters["timestamp"];

                var baseInfo = context.GetMessageHandlerBaseInfo();
                var appId = baseInfo.AppId;
                var encodingAesKey = baseInfo.EncodingAesKey;
                var token = baseInfo.Token;

                var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAesKey, appId);
                wxBizMsgCrypt.DecryptMsg(signature, timestamp, nonce, content, ref content);
            }

            #endregion Decrypt

            context.SetRequestMessage(requestMessageFactory.CreateRequestMessage(content));

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware
    }
}