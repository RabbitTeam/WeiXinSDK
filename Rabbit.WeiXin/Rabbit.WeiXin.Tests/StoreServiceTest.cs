using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.Store;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class StoreServiceTest : ApiTestBase
    {
        #region Field

        private readonly IStoreService _storeService;

        #endregion Field

        #region Constructor

        public StoreServiceTest()
        {
            _storeService = new StoreService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void UploadImageTest()
        {
            var url = _storeService.UploadImage(ApiTestHelper.GetJpgBytes());
            Assert.IsNotNull(url);
            Assert.IsTrue(url.StartsWith("http://"));
        }

        [TestMethod]
        public void CreateTest()
        {
            var url = _storeService.UploadImage(ApiTestHelper.GetJpgBytes());
            _storeService.Create(new CreateStoreModel
            {
                Address = "西洪路528号",
                AvgPrice = 50,
                BranchName = "大泽",
                Categories = new Collection<string> { "公司企业", "企业/工厂" },
                City = "福州市",
                CustomStoreId = "1000",
                District = "鼓楼区",
                EndTime = new TimeSpan(18, 0, 0),
                Introduction = "大泽技术",
                Latitude = 25.097486,
                Longitude = 115.32375,
                Name = "众事达",
                PhotoList = new[] { url },
                Province = "福建省",
                Recommend = "雨后春笋",
                Special = "免费wifi",
                StartTime = new TimeSpan(8, 30, 0),
                TelePhone = "0591-63323934"
            });
        }

        [TestMethod]
        public void GetTest()
        {
            var store = _storeService.Get(289123561);
            Assert.IsNotNull(store);
            Assert.IsNotNull(store.PhotoList);
            Assert.IsTrue(store.PhotoList.Any());
        }

        [TestMethod]
        public void GetListTest()
        {
            var stores = _storeService.GetList();
            Assert.IsNotNull(stores);
        }

        [TestMethod]
        public void UpdateTest()
        {
            _storeService.Update(new UpdateStoreModel { StoreId = 289123561, Introduction = "简介" });
        }

        [TestMethod]
        public void DeleteTest()
        {
            _storeService.Delete(289123561);
        }

        #endregion Test Method
    }
}