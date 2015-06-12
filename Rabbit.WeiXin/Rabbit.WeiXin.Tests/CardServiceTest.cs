using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Api.Card;
using System;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CardServiceTest : ApiTestBase
    {
        #region Field

        readonly private ICardService _cardService;

        #endregion Field

        #region Constructor

        public CardServiceTest()
        {
            _cardService = new CardService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void UploadLogoTest()
        {
            var url = _cardService.UploadLogo(ApiTestHelper.GetJpgBytes());
            Assert.IsTrue(Uri.IsWellFormedUriString(url, UriKind.Absolute));
        }

        [TestMethod]
        public void CreateTest()
        {
            var cardId = _cardService.Create(new GroupCardModel
            {
                BaseInfo = new CardBaseInfo
                {
                    LogoUrl = "http://mmbiz.qpic.cn/",
                    ViewType = CardViewType.QrCode,
                    BrandName = "海底捞",
                    Title = "双人套餐100元兑换券",
                    SubTitle = "鸳鸯锅底+牛肉1份+土豆一份",
                    Color = "Color010",
                    Notice = "请出示二维码核销卡券",
                    Description = "不可与其他优惠同享/n如需团购券发票，请向店员提出要求。",
                    Product = new CardBaseInfo.ProductInfo
                    {
                        Quantity = 0
                    },
                    UseTime = new CardBaseInfo.UseTimeFixedLongInfo
                    {
                        FixedBeginTerm = 0,
                        FixedTerm = 0
                    }
                },
                Detail = "双人套餐\n -进口红酒一支。\n孜然牛肉一份。"
            });
            Assert.IsNotNull(cardId);
        }

        #endregion Test Method
    }
}