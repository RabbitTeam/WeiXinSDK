using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.MvcExtension.Results;
using Rabbit.WeiXin.Sample.Handlers;
using System;
using System.Web.Mvc;

namespace Rabbit.WeiXin.Sample.Controllers
{
    public class MutualController : Controller
    {
        [HttpGet]
        public string Index(string signature, string timestamp, string nonce, string echostr)
        {
            var signatureService = DefaultDependencyResolver.Instance.GetService<ISignatureService>();
            if (signatureService.Check(signature, timestamp, nonce, "weixin"))
                return echostr;

            throw new Exception("非法请求。");
        }

        [HttpPost]
        public ActionResult Index(string signature, string timestamp, string nonce)
        {
            IHandlerBuilder builder = new HandlerBuilder();

            builder
                .Use<SignatureCheckHandlerMiddleware>()             //验证签名中间件。
                .Use<CreateRequestMessageHandlerMiddleware>()   //创建消息中间件。
                .Use<TestMessageHandlerMiddleware>()                //测试消息处理中间件。
                .Use<GenerateResponseXmlHandlerMiddleware>();   //生成相应XML处理中间件。

            var context = new HandlerContext(Request);

            context.Set("DependencyResolver", DefaultDependencyResolver.Instance);
            context.Set("AppId", "wxa4ab3e636e2eb702");
            context.Set("EncodingAesKey", "zhRjo19GUoReVbuC4b3HrWADbTcQlOnt7qe1wmDmg4T");
            context.Set("Token", "weixin");

            IWeiXinHandler weiXinHandler = new DefaultWeiXinHandler(builder);
            weiXinHandler.Execute(context);

            return new WeiXinResult(context);
        }
    }
}