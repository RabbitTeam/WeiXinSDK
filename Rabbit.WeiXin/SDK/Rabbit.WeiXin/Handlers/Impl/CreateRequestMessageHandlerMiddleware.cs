using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Messages;
using Rabbit.WeiXin.Utility.Extensions;
using System.Text;
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
            var request = context.Request;
            var dependencyResolver = context.Get<IDependencyResolver>("DependencyResolver");
            var requestMessageFactory = dependencyResolver.GetService<IRequestMessageFactory>();

            var content = Encoding.UTF8.GetString(request.InputStream.ReadBytes());

            #region Decrypt

            var encryptType = request.QueryString["encrypt_type"];

            if (encryptType != null)
            {
                var nonce = request.QueryString["nonce"];
                var signature = request.QueryString["msg_signature"];
                var timestamp = request.QueryString["timestamp"];
                var appId = context.Get<string>("AppId");
                var encodingAesKey = context.Get<string>("EncodingAesKey");
                var token = context.Get<string>("Token");

                var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAesKey, appId);
                wxBizMsgCrypt.DecryptMsg(signature, timestamp, nonce, content, ref content);
            }

            #endregion Decrypt

            context.Set("RequestMessage", requestMessageFactory.CreateRequestMessage(content));

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware
    }
}