using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.CustomService;
using Rabbit.WeiXin.MP.Api.Material;
using System;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CustomServiceMessageServiceTest : ApiTestBase
    {
        #region Field

        private readonly ICustomServiceMessageService _customServiceMessageService;
        private readonly ITemporaryMaterialService _temporaryMaterialService;

        #endregion Field

        #region Constructor

        public CustomServiceMessageServiceTest()
        {
            _customServiceMessageService = new CustomServiceMessageService(AccountModel);
            _temporaryMaterialService = new TemporaryMaterialService(AccountModel);
        }

        #endregion Constructor

        #region TestMethod

        [TestMethod]
        public void SendTextMessageTest()
        {
            _customServiceMessageService.Send(new CustomServiceMessageText(OpenId) { Content = "test" });
        }

        [TestMethod]
        public void SendImageMessageTest()
        {
            var result = _temporaryMaterialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Image);
            _customServiceMessageService.Send(new CustomServiceMessageImage(OpenId) { MediaId = result.MediaId });
        }

        [TestMethod]
        public void SendMusicMessageTest()
        {
            var result = _temporaryMaterialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Thumb);
            _customServiceMessageService.Send(new CustomServiceMessageMusic(OpenId)
            {
                Description = "董贞",
                HqMusicUrl = ApiTestHelper.HqMusicUrl,
                MusicUrl = ApiTestHelper.MusicUrl,
                ThumbnailsMediaId = result.MediaId,
                Title = "星辰泪"
            });
        }

        [TestMethod]
        public void SendVoiceMessageTest()
        {
            var result = _temporaryMaterialService.AddTemporary(ApiTestHelper.GetAmrBytes(), MaterialType.Voice);
            _customServiceMessageService.Send(new CustomServiceMessageVoice(OpenId)
            {
                MediaId = result.MediaId
            });
        }

        [TestMethod]
        public void SendVideoMessageTest()
        {
            var thumbnailsMedia = _temporaryMaterialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Thumb);
            var result = _temporaryMaterialService.AddTemporary(ApiTestHelper.GetMp4Bytes(), MaterialType.Video);
            _customServiceMessageService.Send(new CustomServiceMessageVideo(OpenId)
            {
                Description = "视频说明",
                MediaId = result.MediaId,
                ThumbnailsMediaId = thumbnailsMedia.MediaId,
                Title = "视频标题"
            });
        }

        [TestMethod]
        public void SendNewsMessageTest()
        {
            _customServiceMessageService.Send(new CustomServiceMessageNews(OpenId, new[]
            {
                new CustomServiceMessageNews.Article
                {
                    Title = "文章1",
                    Description = "文章1说明",
                    Url = new Uri("http://cn.bing.com"),
                    PicUrl = new Uri("http://www.chunsun.cc/Modules/ChunSun.Manager.UserInterface/Contents/images/logo.png")
                },
                new CustomServiceMessageNews.Article
                {
                    Title = "文章2",
                    Description = "文章2说明",
                    Url = new Uri("http://cn.bing.com"),
                    PicUrl = new Uri("http://www.chunsun.cc/Modules/ChunSun.Manager.UserInterface/Contents/images/logo.png")
                },
            }));
        }

        [TestMethod]
        public void SendCardMessageTest()
        {
            _customServiceMessageService.Send(new CustomServiceMessageCard(OpenId));
        }

        #endregion TestMethod
    }
}