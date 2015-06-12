using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Messages;
using Rabbit.WeiXin.Messages.Response;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class ResponseMessageFormatter
    {
        #region Field

        private readonly IResponseMessageFactory _responseMessageFactory = new ResponseMessageFactory();

        #endregion Field

        [TestMethod]
        public void ResponseMessageTextTest()
        {
            var content = _responseMessageFactory.GetXmlByReponseMessage(new ResponseMessageText
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun",
                Content = "test"
            });

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[Text]]></MsgType><Content><![CDATA[test]]></Content></xml>", content);
        }

        [TestMethod]
        public void ResponseMessageImageTest()
        {
            var content = _responseMessageFactory.GetXmlByReponseMessage(new ResponseMessageImage
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun",
                MediaId = "123456-78901234567"
            });

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[Image]]></MsgType><Image><MediaId><![CDATA[123456-78901234567]]></MediaId></Image></xml>", content);
        }

        [TestMethod]
        public void ResponseMessageVoiceTest()
        {
            var content = _responseMessageFactory.GetXmlByReponseMessage(new ResponseMessageVoice
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun",
                MediaId = "123456-78901234567"
            });

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[Voice]]></MsgType><Voice><MediaId><![CDATA[123456-78901234567]]></MediaId></Voice></xml>", content);
        }

        [TestMethod]
        public void ResponseMessageVideoTest()
        {
            var content = _responseMessageFactory.GetXmlByReponseMessage(new ResponseMessageVideo
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun",
                MediaId = "123456-78901234567",
                Title = "标题",
                Description = "说明"
            });

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[Video]]></MsgType><Video><MediaId><![CDATA[123456-78901234567]]></MediaId><Title><![CDATA[标题]]></Title><Description><![CDATA[说明]]></Description></Video></xml>", content);
        }

        [TestMethod]
        public void ResponseMessageMusicTest()
        {
            var content = _responseMessageFactory.GetXmlByReponseMessage(new ResponseMessageMusic
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun",
                Title = "标题",
                Description = "说明",
                HqMusicUrl = new Uri("http://www.chunsun.cc/1-hq.mp3"),
                MusicUrl = new Uri("http://www.chunsun.cc/1.mp3"),
                ThumbMediaId = "1234567890-12345678"
            });

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[Music]]></MsgType><Music><Title><![CDATA[标题]]></Title><Description><![CDATA[说明]]></Description><MusicUrl><![CDATA[http://www.chunsun.cc/1.mp3]]></MusicUrl><HQMusicUrl><![CDATA[http://www.chunsun.cc/1-hq.mp3]]></HQMusicUrl><ThumbMediaId><![CDATA[1234567890-12345678]]></ThumbMediaId></Music></xml>", content);
        }

        [TestMethod]
        public void ResponseMessageNewsTest()
        {
            var message = new ResponseMessageNews
            {
                CreateTime = new DateTime(2015, 5, 26, 13, 00, 20),
                FromUserName = "majian",
                ToUserName = "chunsun"
            };
            message.Append(new ResponseMessageNews.Article
            {
                Title = "文章标题",
                Description = "文章说明",
                PicUrl = new Uri("http://www.chunsun.cc/pic.jpg"),
                Url = new Uri("http://www.chunsun.cc/articles/1")
            });
            Assert.IsTrue(message.IsAdd());

            message.Append(Enumerable.Range(1, 10).Select(i => new ResponseMessageNews.Article
            {
                Title = "文章标题" + i,
                Description = "文章说明" + i,
                PicUrl = new Uri("http://www.chunsun.cc/pic" + i + ".jpg"),
                Url = new Uri("http://www.chunsun.cc/articles/" + i)
            }).ToArray());
            Assert.IsFalse(message.IsAdd());

            Assert.AreEqual("文章标题9", message.Articles[message.ArticleCount - 1].Title);

            var content = _responseMessageFactory.GetXmlByReponseMessage(message);

            Assert.AreEqual("<xml><ToUserName><![CDATA[chunsun]]></ToUserName><FromUserName><![CDATA[majian]]></FromUserName><CreateTime><![CDATA[1432645220]]></CreateTime><MsgType><![CDATA[News]]></MsgType><ArticleCount>10</ArticleCount><Articles><item><Title><![CDATA[文章标题]]></Title><Description><![CDATA[文章说明]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/1]]></Url></item><item><Title><![CDATA[文章标题1]]></Title><Description><![CDATA[文章说明1]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic1.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/1]]></Url></item><item><Title><![CDATA[文章标题2]]></Title><Description><![CDATA[文章说明2]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic2.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/2]]></Url></item><item><Title><![CDATA[文章标题3]]></Title><Description><![CDATA[文章说明3]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic3.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/3]]></Url></item><item><Title><![CDATA[文章标题4]]></Title><Description><![CDATA[文章说明4]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic4.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/4]]></Url></item><item><Title><![CDATA[文章标题5]]></Title><Description><![CDATA[文章说明5]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic5.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/5]]></Url></item><item><Title><![CDATA[文章标题6]]></Title><Description><![CDATA[文章说明6]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic6.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/6]]></Url></item><item><Title><![CDATA[文章标题7]]></Title><Description><![CDATA[文章说明7]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic7.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/7]]></Url></item><item><Title><![CDATA[文章标题8]]></Title><Description><![CDATA[文章说明8]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic8.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/8]]></Url></item><item><Title><![CDATA[文章标题9]]></Title><Description><![CDATA[文章说明9]]></Description><PicUrl><![CDATA[http://www.chunsun.cc/pic9.jpg]]></PicUrl><Url><![CDATA[http://www.chunsun.cc/articles/9]]></Url></item></Articles></xml>", content);
        }
    }
}