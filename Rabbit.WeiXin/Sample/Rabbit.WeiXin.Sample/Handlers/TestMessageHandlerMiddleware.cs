using Rabbit.WeiXin.Handlers;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.Messages.Events;
using Rabbit.WeiXin.Messages.Request;
using Rabbit.WeiXin.Messages.Response;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Sample.Handlers
{
    /// <summary>
    /// 测试用的消息处理中间件。
    /// </summary>
    public class TestMessageHandlerMiddleware : MessageHandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public TestMessageHandlerMiddleware(HandlerMiddleware next)
            : base(next)
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
                                    "http://www.chunsun.cc/Modules/ChunSun.Manager.UserInterface/Contents/images/logo.png"),
                            Url = new Uri("http://www.chunsun.cc")
                        }).ToArray());

                case "music":
                    return new ResponseMessageMusic("_aDrUW_x3f24Cye0HQdpslUX7Fqi7F6wchhI2aPTnZ8", "星辰泪", "董贞", new Uri("http://www.chunsun.cc/星辰泪.mp3"), new Uri("http://www.chunsun.cc/星辰泪.mp3"));

                default:
                    return new ResponseMessageText(requestMessage.Content);
            }
        }

        /// <summary>
        /// 图片类型请求
        /// </summary>
        public override IResponseMessage OnImageRequest(RequestMessageImage requestMessage)
        {
            return new ResponseMessageImage("umkRNs_LvCkQsbyAekQCUYD1Nlo48CHlq0uH5uXoe_o");
        }

        /// <summary>
        /// 视频类型请求
        /// </summary>
        public override IResponseMessage OnVideoRequest(RequestMessageVideo requestMessage)
        {
            return new ResponseMessageVideo("VrZjycvlrXKs_yYOjDbIg4Ivtjg3rFADXk6xaBqfnOI", "标题", "说明");
        }

        /// <summary>
        /// 语音类型请求
        /// </summary>
        public override IResponseMessage OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            return new ResponseMessageVoice("VrZjycvlrXKs_yYOjDbIg97AxjDGyDhe444f_MBWq7E");
        }

        /// <summary>
        /// Event事件类型请求之subscribe
        /// </summary>
        public override IResponseMessage OnEvent_SubscribeRequest(SubscribeEventMessage requestMessage)
        {
            return new ResponseMessageText("欢迎关注我！");
        }

        #endregion Overrides of MessageHandlerMiddleware
    }
}