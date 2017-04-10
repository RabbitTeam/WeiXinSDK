using Xunit;
using Rabbit.WeiXin.MP.Api.QrCode;
using System;
using System.Text.Encodings.Web;

namespace Rabbit.WeiXin.Tests
{
    
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

        [Fact]
        public void CreateTemporaryTest()
        {
            var result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel(1));
            Assert.NotNull(result);
            Assert.NotNull(result.Ticket);
            Assert.Equal((uint)604800, result.ExpireSeconds);
            result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel(1, 61));
            Assert.Equal((uint)61, result.ExpireSeconds);
        }

        [Fact]
        public void CreateForeverTest()
        {
            var result = _qrCodeService.CreateForever(new CreateForeverQrCodeModel("abcd"));
            Assert.NotNull(result);
            Assert.NotNull(result.Ticket);
        }

        [Fact]
        public void GetTest()
        {
            var result = _qrCodeService.CreateTemporary(new CreateTemporaryQrCodeModel(1234));
            var qrCodeInfo = _qrCodeService.Get(result.Ticket);
            Assert.True(qrCodeInfo.ContentLength > 0);
            Assert.NotNull(qrCodeInfo.ContentType);
            Assert.True(qrCodeInfo.Data.Length > 0);
        }

        [Fact]
        public void GetUrlTest()
        {
            var ticket = Guid.NewGuid().ToString("n") + "@";
            ticket = UrlEncoder.Default.Encode(ticket);
            Assert.Equal("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticket, _qrCodeService.GetQrCodeUrl(ticket));
        }

        #endregion Test Method
    }
}