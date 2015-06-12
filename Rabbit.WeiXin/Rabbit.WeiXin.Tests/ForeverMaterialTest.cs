using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Api.Material;
using System;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class ForeverMaterialTest : ApiTestBase
    {
        #region Field

        private readonly IForeverMaterialService _materialService;
        private readonly ITemporaryMaterialService _temporaryMaterialService;

        #endregion Field

        #region Constructor

        public ForeverMaterialTest()
        {
            _materialService = new ForeverMaterialService(AccountModel);
            _temporaryMaterialService = new TemporaryMaterialService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void AddThumbnailsTest()
        {
            var result = _materialService.AddOtherThumbnails(ApiTestHelper.GetJpgBytes());
            Assert.IsNotNull(result.MediaId);
            Assert.IsTrue(Uri.IsWellFormedUriString(result.Url, UriKind.Absolute));
        }

        [TestMethod]
        public void AddJpgTest()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetJpgBytes());
            Assert.IsNotNull(result.MediaId);
            Assert.IsTrue(Uri.IsWellFormedUriString(result.Url, UriKind.Absolute));
        }

        [TestMethod]
        public void AddMp4Test()
        {
            var result = _materialService.AddVoide(ApiTestHelper.GetMp4Bytes(), new AddVoideMaterialModel
            {
                Title = "视频标题",
                Description = "视频说明"
            });
            Assert.IsNotNull(result.MediaId);
            //            Assert.IsTrue(Uri.IsWellFormedUriString(result.Url, UriKind.Absolute));
        }

        [TestMethod]
        public void AddAmrTest()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetAmrBytes());
            Assert.IsNotNull(result.MediaId);
            //            Assert.IsTrue(Uri.IsWellFormedUriString(result.Url, UriKind.Absolute));
        }

        [TestMethod]
        public void AddMp3Test()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetMp3Bytes());
            Assert.IsNotNull(result.MediaId);
            //            Assert.IsTrue(Uri.IsWellFormedUriString(result.Url, UriKind.Absolute));
        }

        [TestMethod]
        public void AddNewsTest()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetJpgBytes());
            _materialService.AddNews(new[]
            {
                new NewsArticleModel
                {
                    Author="author",
                    Content = "content",
                    Description = "description",
                    IsShowConverPicture = true,
                    ThumbnailsMediaId = result.MediaId,
                    Title = "标题1",
                    Url = "http://cn.bing.com"
                },
                new NewsArticleModel
                {
                    Author="author2",
                    Content = "content2",
                    Description = "description2",
                    IsShowConverPicture = true,
                    ThumbnailsMediaId = result.MediaId,
                    Title = "标题2",
                    Url = "http://cn.bing.com"
                },
            });
        }

        [TestMethod]
        public void GetOtherTest()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetJpgBytes());
            var data = _materialService.GetOther(result.MediaId);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
        }

        [TestMethod]
        public void GetMediaCountTest()
        {
            var result = _materialService.GetMaterialCount(GetAccessToken());
            Assert.IsTrue(result.ImageCount > 0);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var result = _materialService.AddOther(ApiTestHelper.GetJpgBytes());
            _materialService.Delete(result.MediaId);
            try
            {
                _materialService.GetOther(result.MediaId);
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void GetListTest()
        {
            var list = _materialService.GetList(new GetMaterialListFilter { Take = 20, Skip = 0, Type = MaterialType.Image });

            Assert.IsTrue(list.TotalCount > 0);
        }

        #endregion Test Method
    }
}