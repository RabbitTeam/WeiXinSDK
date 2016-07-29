using Xunit;
using Rabbit.WeiXin.MP.Api;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Tests
{
    public class CommonServiceTest : ApiTestBase
    {
        #region Test Method

        [Fact]
        public void GetAccessTokenTest()
        {
            Func<bool, AccessTokenModel> get =
                ignoreCached => CommonService.GetAccessToken(ignoreCached);

            var model = get(false);

            Assert.NotNull(model.AccessToken);

            Assert.False(model.IsExpired());

            var model2 = get(false);
            Assert.Equal(model.AccessToken, model2.AccessToken);
            Assert.False(model2.IsExpired());

            model = get(true);
            Assert.NotEqual(model.AccessToken, model2.AccessToken);
        }

        [Fact]
        public void GenerateShotAddressTest()
        {
            const string url = "http://cn.bing.com/search?q=windows10&go=%E6%8F%90%E4%BA%A4&qs=n&form=QBLH&pq=windows10&sc=8-9&sp=-1&sk=&ghc=1&cvid=6d333afcfbec4834bbf3ce592b699f66";
            var shortAddress = CommonService.GenerateShortAddress(url);

            Assert.NotNull(shortAddress);

            var request = (HttpWebRequest)WebRequest.Create(shortAddress);
            //request.AllowAutoRedirect = false;
            Task.Run(async () =>
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (response.GetResponseStream())
                    {
                        Assert.Equal(url, response.Headers["Location"]);
                        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
                    }
                }
            }).Wait();
        }

        [Fact]
        public void GetServerIpListTest()
        {
            var list = CommonService.GetServerIpList();
            Assert.NotNull(list);
            Assert.True(list.Length > 0);
        }

        #endregion Test Method
    }
}