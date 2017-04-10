using Xunit;
using Rabbit.WeiXin.MP.Api.GroupMessage;
using Rabbit.WeiXin.MP.Api.Material;
using Rabbit.WeiXin.Tests.Utility;

namespace Rabbit.WeiXin.Tests
{
    
    public class GroupMessageServiceTest : ApiTestBase
    {
        #region Field

        private readonly IGroupMessageService _groupMessageService;
        private readonly IForeverMaterialService _foreverMaterialService;

        #endregion Field

        #region Constructor

        public GroupMessageServiceTest()
        {
            _groupMessageService = new GroupMessageService(AccountModel);
            _foreverMaterialService = new ForeverMaterialService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [Fact]
        public void SendTextByGroupTest()
        {
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageText("content"));
        }

        [Fact]
        public void SendImageByGroupTest()
        {
            var result = _foreverMaterialService.AddOther(ApiTestHelper.GetJpgBytes());
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageImage(result.MediaId));
        }

        [Fact]
        public void SendNewsByGroupTest()
        {
            var result = _foreverMaterialService.AddOther(ApiTestHelper.GetJpgBytes());
            var newsId = _foreverMaterialService.AddNews(new[]
            {
                new NewsArticleModel
                {
                    Author = "author",
                    Content = "content",
                    Description = "description",
                    IsShowConverPicture = true,
                    ThumbnailsMediaId = result.MediaId,
                    Title = "标题1",
                    Url = "http://cn.bing.com"
                },
                new NewsArticleModel
                {
                    Author = "author2",
                    Content = "content2",
                    Description = "description2",
                    IsShowConverPicture = true,
                    ThumbnailsMediaId = result.MediaId,
                    Title = "标题2",
                    Url = "http://cn.bing.com"
                }
            });
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageNews
            {
                MediaId = newsId
            });
        }

        [Fact]
        public void SendVoiceByGroupTest()
        {
            var result = _foreverMaterialService.AddOther(ApiTestHelper.GetMp3Bytes());
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageVoice(result.MediaId));
        }

        [Fact]
        public void SendVideoByGroupTest()
        {
            var result = _foreverMaterialService.AddOther(ApiTestHelper.GetMp4Bytes());
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageVideo(result.MediaId, "视频标题", "视频描述"));
        }

        [Fact]
        public void SendCardByGroupTest()
        {
            _groupMessageService.SendByGroup(new GroupFilter(), new GroupMessageCard("cardId"));
        }

        [Fact]
        public void SendTextByGroupFromOpenIdTest()
        {
            _groupMessageService.SendByUsers(new[] { OpenId }, new GroupMessageText("content"));
        }

        #endregion Test Method
    }
}