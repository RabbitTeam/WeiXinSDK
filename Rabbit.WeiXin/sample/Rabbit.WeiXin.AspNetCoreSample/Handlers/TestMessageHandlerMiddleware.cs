using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.AspNetCoreSample.Handlers
{
    public class TestMessageHandlerMiddleware : MessageHandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public TestMessageHandlerMiddleware(HandlerMiddleware next) : base(next)
        {
        }

        #region Overrides of MessageHandlerMiddleware

        /// <summary>
        /// 文字类型请求
        /// </summary>
        public override IResponseMessage OnTextRequest(RequestMessageText requestMessage)
        {
            switch (requestMessage.Content)
            {
                case "news":
                    return new ResponseMessageNews(Enumerable.Range(1, 5).Select(i =>
                        new ResponseMessageNews.Article
                        {
                            Description = "文章描述" + i,
                            Title = "文章标题" + i,
                            PicUrl =
                                new Uri(
                                    "http://www.rabbithub.com/icon.png"),
                            Url = new Uri("http://www.rabbithub.com")
                        }).ToArray());

                default:
                    return new ResponseMessageText(requestMessage.Content);
            }
        }

        /// <summary>
        /// Event事件类型请求之subscribe
        /// </summary>
        public override IResponseMessage OnEvent_SubscribeRequest(SubscribeEventMessage requestMessage)
        {
            var builder = new StringBuilder();
            builder
                .AppendLine("欢迎关注 RabbitHub：")
                .AppendLine("发送 \"news\" 文本，系统将会发送一条图文消息。")
                .AppendLine("发送其它文本，系统将会回复一样的内容。");
            return new ResponseMessageText(builder.ToString());
        }

        #endregion Overrides of MessageHandlerMiddleware
    }
}