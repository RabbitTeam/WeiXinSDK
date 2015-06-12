using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Api.QrCode;
using System;
using System.Web;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class QrCodeServiceTest : ApiTestBase
    {
        #region Field

        private readonly IQrCodeService _qrCodeService;

        #endregion Field

        #region Constructor

        public QrCodeServiceTest()
        {
            _qrCodeService = new QrCodeService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void CreateTemporaryTest()
        {
            var result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Ticket);
            Assert.AreEqual((uint)604800, result.ExpireSeconds);
            result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel { ExpireSeconds = 61 });
            Assert.AreEqual((uint)61, result.ExpireSeconds);
        }

        [TestMethod]
        public void CreateForeverTest()
        {
            var result = _qrCodeService.CreateForever(new CreateForeverQrCodeModel(1234));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Ticket);
        }

        [TestMethod]
        public void GetTest()
        {
            var result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel(1234));
            var qrCodeInfo = _qrCodeService.Get(result.Ticket);
            Assert.IsTrue(qrCodeInfo.ContentLength > 0);
            Assert.IsNotNull(qrCodeInfo.ContentType);
            Assert.IsTrue(qrCodeInfo.Data.Length > 0);
        }

        [TestMethod]
        public void GetUrlTest()
        {
            var ticket = Guid.NewGuid().ToString("n") + "@";
            Assert.AreEqual("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + HttpUtility.UrlEncode(ticket), _qrCodeService.GetQrCodeUrl(ticket));
        }

        #endregion Test Method
    }
}