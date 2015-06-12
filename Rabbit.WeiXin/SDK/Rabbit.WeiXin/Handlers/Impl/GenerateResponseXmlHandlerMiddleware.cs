using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Messages;
using System.Threading.Tasks;
using Tencent;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 生成响应Xml中间件（如果需要加密则先进行加密）。
    /// </summary>
    public sealed class GenerateResponseXmlHandlerMiddleware : HandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public GenerateResponseXmlHandlerMiddleware(HandlerMiddleware next)
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
            var responseMessage = context.GetResponseMessage();

            var dependencyResolver = context.GetDependencyResolver();
            var responseMessageFactory = dependencyResolver.GetService<IResponseMessageFactory>();
            var content = responseMessage == null ? string.Empty : responseMessageFactory.GetXmlByReponseMessage(responseMessage);

            #region Encrypt

            if (!string.IsNullOrWhiteSpace(content))
            {
                var request = context.Request;
                var encryptType = request.QueryString["encrypt_type"];

                if (encryptType != null)
                {
                    var nonce = request.QueryString["nonce"];
                    var timestamp = request.QueryString["timestamp"];

                    var baseInfo = context.GetMessageHandlerBaseInfo();
                    var appId = baseInfo.AppId;
                    var encodingAesKey = baseInfo.EncodingAesKey;
                    var token = baseInfo.Token;

                    var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAesKey, appId);
                    wxBizMsgCrypt.EncryptMsg(content, timestamp, nonce, ref content);
                }
            }

            #endregion Encrypt

            context.ResponseXml = content;

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware
    }
}