using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.Card;
using Rabbit.WeiXin.Tests.Utility;
using System;
using System.Linq;
using System.Threading;

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
            var baseInfo = new CreateCardBaseInfo
            {
                LogoUrl =
                    "http://mmbiz.qpic.cn/mmbiz/YclRzpaicL4h155d9lAt2icgjn2TyYDjzjdM9dur1EzL7qM7OqMdHKrYhGhppvcibaicNoxylRTkwN6OL5qpuHTl2Q/0",
                ViewType = CardViewType.QrCode,
                BrandName = "海底捞",
                Title = "双人套餐100元兑换券",
                LocationIdList = new long[] { 282428816 },
                SubTitle = "鸳鸯锅底+牛肉1份+土豆一份",
                Color = "Color010",
                Notice = "请出示二维码核销卡券",
                Description = "不可与其他优惠同享/n如需团购券发票，请向店员提出要求。",
                Product = new CardBaseInfoBase.ProductInfo
                {
                    Quantity = 20
                },
                UseTime = new CardBaseInfoBase.UseTimeFixedLongInfo
                {
                    FixedBeginTerm = 2,
                    FixedTerm = 10
                },
                ServicePhone = "88080222"
            };
            var cardId = _cardService.Create(new GroupCardModel
            {
                BaseInfo = baseInfo,
                Detail = "双人套餐\n -进口红酒一支。\n孜然牛肉一份。"
            });
            Assert.IsNotNull(cardId);

            cardId = _cardService.Create(new CashCardModel
            {
                BaseInfo = baseInfo,
                LeastCost = 10,
                ReduceCost = 1
            });
            Assert.IsNotNull(cardId);

            cardId = _cardService.Create(new DiscountCardModel
            {
                BaseInfo = baseInfo,
                Discount = 30
            });
            Assert.IsNotNull(cardId);

            cardId = _cardService.Create(new GiftCardModel
            {
                BaseInfo = baseInfo,
                Gift = "iPhone6"
            });
            Assert.IsNotNull(cardId);

            cardId = _cardService.Create(new CouponCardModel
            {
                BaseInfo = baseInfo,
                Detail = "全场酒水免费畅饮"
            });
            Assert.IsNotNull(cardId);
        }

        [TestMethod]
        public void GetListTest()
        {
            var list = _cardService.GetCardList();
            Assert.IsNotNull(list);
            if (list.Count > 1)
            {
                Assert.AreEqual(_cardService.GetCardList(1, 1).List.First(), list.List.Skip(1).First());
            }
        }

        [TestMethod]
        public void GetTest()
        {
            var id = _cardService.GetCardList().List.First();
            var model = _cardService.Get(id);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void DeleteTest()
        {
            var baseInfo = new CreateCardBaseInfo
            {
                LogoUrl =
                    "http://mmbiz.qpic.cn/mmbiz/YclRzpaicL4h155d9lAt2icgjn2TyYDjzjdM9dur1EzL7qM7OqMdHKrYhGhppvcibaicNoxylRTkwN6OL5qpuHTl2Q/0",
                ViewType = CardViewType.QrCode,
                BrandName = "海底捞",
                Title = "双人套餐100元兑换券",
                LocationIdList = new long[] { 282428816 },
                SubTitle = "鸳鸯锅底+牛肉1份+土豆一份",
                Color = "Color010",
                Notice = "请出示二维码核销卡券",
                Description = "不可与其他优惠同享/n如需团购券发票，请向店员提出要求。",
                Product = new CardBaseInfoBase.ProductInfo
                {
                    Quantity = 20
                },
                UseTime = new CardBaseInfoBase.UseTimeFixedLongInfo
                {
                    FixedBeginTerm = 2,
                    FixedTerm = 10
                },
                ServicePhone = "88080222"
            };
            var cardId = _cardService.Create(new GroupCardModel
            {
                BaseInfo = baseInfo,
                Detail = "双人套餐\n -进口红酒一支。\n孜然牛肉一份。"
            });

            var model = _cardService.Get(cardId);
            Assert.AreEqual(CardStatusEnum.NotVerify, ((CardBaseInfo)model.BaseInfo).Status);
            _cardService.Delete(cardId);
            for (var i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                model = _cardService.Get(cardId);
                if (((CardBaseInfo)model.BaseInfo).Status == CardStatusEnum.Delete)
                {
                    Assert.AreEqual(CardStatusEnum.Delete, ((CardBaseInfo)model.BaseInfo).Status);
                    return;
                }
            }
            Assert.Fail();
        }

        [TestMethod]
        public void GetUserCardList()
        {
            var list = _cardService.GetUserCardList(OpenId);

            Assert.IsNotNull(list);
            if (!list.Any())
                return;
            var cardId = list.First().CardId;
            var list2 = _cardService.GetUserCardList(OpenId, cardId);
            Assert.AreEqual(list.Count(i => i.CardId == cardId), list2.Count());
        }

        #endregion Test Method
    }
}