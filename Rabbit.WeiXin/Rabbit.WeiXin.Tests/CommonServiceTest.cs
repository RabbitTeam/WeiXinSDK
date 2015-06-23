using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api;
using System;
using System.Net;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CommonServiceTest : ApiTestBase
    {
        #region Test Method

        [TestMethod]
        public void GetAccessTokenTest()
        {
            Func<bool, AccessTokenModel> get =
                ignoreCached => CommonService.GetAccessToken(ignoreCached);

            var model = get(false);

            Assert.IsNotNull(model.AccessToken);

            Assert.IsFalse(model.IsExpired());

            var model2 = get(false);
            Assert.AreEqual(model.AccessToken, model2.AccessToken);
            Assert.IsFalse(model2.IsExpired());

            model = get(true);
            Assert.AreNotEqual(model.AccessToken, model2.AccessToken);
        }

        [TestMethod]
        public void GenerateShotAddressTest()
        {
            const string url = "http://cn.bing.com/search?q=windows10&go=%E6%8F%90%E4%BA%A4&qs=n&form=QBLH&pq=windows10&sc=8-9&sp=-1&sk=&ghc=1&cvid=6d333afcfbec4834bbf3ce592b699f66";
            var shortAddress = CommonService.GenerateShortAddress(url);

            Assert.IsNotNull(shortAddress);

            var request = (HttpWebRequest)WebRequest.Create(shortAddress);
            request.AllowAutoRedirect = false;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (response.GetResponseStream())
                {
                    Assert.AreEqual(url, response.Headers["Location"]);
                    Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
                }
            }
        }

        [TestMethod]
        public void GetServerIpListTest()
        {
            var list = CommonService.GetServerIpList();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Length > 0);
        }

        #endregion Test Method
    }
}