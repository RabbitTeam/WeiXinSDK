using Microsoft.AspNetCore.Mvc;
using Rabbit.WeiXin.AspNetCoreSample.Handlers;
using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.MvcExtension.Results;
using System;
using System.IO;
using System.Text;

namespace Rabbit.WeiXin.AspNetCoreSample.Controllers
{
    public class WeChatConfig
    {
        public static WeChatConfig Instance { get; } = new WeChatConfig
        {
            AppId = "appId",
            EncodingAesKey = "encodingAesKey",
            Token = "token"
        };

        public string AppId { get; set; }
        public string Token { get; set; }
        public string EncodingAesKey { get; set; }
    }

    public class MutualController : Controller
    {
        private readonly WeChatConfig _weChatConfig = WeChatConfig.Instance;

        [HttpGet]
        public string Index(string signature, string timestamp, string nonce, string echostr)
        {
            var signatureService = DefaultDependencyResolver.Instance.GetService<ISignatureService>();
            if (signatureService.Check(signature, timestamp, nonce, _weChatConfig.Token))
                return echostr;

            throw new Exception("非法请求。");
        }

        [HttpPost]
        public ActionResult Index()
        {
            IHandlerBuilder builder = new HandlerBuilder();

            builder
                .Use<SignatureCheckHandlerMiddleware>() //验证签名中间件。
                .Use<CreateRequestMessageHandlerMiddleware>() //创建消息中间件（内置消息解密逻辑）。
                .Use<SessionSupportHandlerMiddleware>() //会话支持中间件。
                .Use<IgnoreRepeatMessageHandlerMiddleware>() //忽略重复的消息中间件。
                .Use<TestMessageHandlerMiddleware>() //测试消息处理中间件。
                .Use<GenerateResponseXmlHandlerMiddleware>(); //生成相应XML处理中间件（内置消息加密逻辑）。
                                                              //                            .Use<AgentHandlerMiddleware>(new AgentRequestModel(new Uri("http://localhost:22479/Mutual")));

            string content;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            var context = new HandlerContext(content);

            //设置基本信息。
            context
                .SetMessageHandlerBaseInfo(new MessageHandlerBaseInfo(
                    _weChatConfig.AppId,
                    _weChatConfig.EncodingAesKey,
                    _weChatConfig.Token));

            IWeiXinHandler weiXinHandler = new DefaultWeiXinHandler(builder);
            weiXinHandler.Execute(context);

            return new WeiXinResult(context);
        }
    }
}