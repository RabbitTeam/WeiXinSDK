using Rabbit.WeiXin.MP.Api.Card;
using Rabbit.WeiXin.Tests.Utility;
using Rabbit.WeiXin.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Rabbit.WeiXin.Tests
{
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

        [Fact]
        public void UploadLogoTest()
        {
            var url = _cardService.UploadLogo(ApiTestHelper.GetJpgBytes());
            Assert.True(Uri.IsWellFormedUriString(url, UriKind.Absolute));
        }

        [Fact]
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
            Assert.NotNull(cardId);

            cardId = _cardService.Create(new CashCardModel
            {
                BaseInfo = baseInfo,
                LeastCost = 10,
                ReduceCost = 1
            });
            Assert.NotNull(cardId);

            cardId = _cardService.Create(new DiscountCardModel
            {
                BaseInfo = baseInfo,
                Discount = 30
            });
            Assert.NotNull(cardId);

            cardId = _cardService.Create(new GiftCardModel
            {
                BaseInfo = baseInfo,
                Gift = "iPhone6"
            });
            Assert.NotNull(cardId);

            cardId = _cardService.Create(new CouponCardModel
            {
                BaseInfo = baseInfo,
                Detail = "全场酒水免费畅饮"
            });
            Assert.NotNull(cardId);
        }

        [Fact]
        public void GetListTest()
        {
            var list = _cardService.GetCardList();
            Assert.NotNull(list);
            if (list.Count > 1)
            {
                Assert.Equal(_cardService.GetCardList(1, 1).List.First(), list.List.Skip(1).First());
            }
        }

        [Fact]
        public void GetTest()
        {
            var id = _cardService.GetCardList().List.First();
            var model = _cardService.Get(id);
            Assert.NotNull(model);
        }

        [Fact]
        public void DeleteTest()
        {
            var cardId = CreateGroupCard();
            var model = _cardService.Get(cardId);
            Assert.Equal(CardStatusEnum.NotVerify, ((CardBaseInfo)model.BaseInfo).Status);
            _cardService.Delete(cardId);
            for (var i = 0; i < 3; i++)
            {
                Task.Delay(500).Wait();
                model = _cardService.Get(cardId);
                if (((CardBaseInfo)model.BaseInfo).Status == CardStatusEnum.Delete)
                {
                    Assert.Equal(CardStatusEnum.Delete, ((CardBaseInfo)model.BaseInfo).Status);
                    return;
                }
            }
            Assert.True(false);
        }

        [Fact]
        public void GetUserCardList()
        {
            var list = _cardService.GetUserCardList(OpenId);

            Assert.NotNull(list);
            if (!list.Any())
            {
                Assert.True(false, "没有可用的用户卡券。");
                //                Assert.Inconclusive("没有可用的用户卡券。");
            }
            var cardId = list.First().CardId;
            var list2 = _cardService.GetUserCardList(OpenId, cardId);
            Assert.Equal(list.Count(i => i.CardId == cardId), list2.Count());
        }

        [Fact]
        public void IncreaseStockTest()
        {
            var cardId = CreateGroupCard();
            var model = _cardService.Get(cardId);
            _cardService.IncreaseStock(cardId, 5);
            for (var i = 0; i < 3; i++)
            {
                Task.Delay(500).Wait();
                var model2 = _cardService.Get(cardId);
                if (model.BaseInfo.Product.Quantity == model2.BaseInfo.Product.Quantity)
                    continue;
                Assert.Equal(model.BaseInfo.Product.Quantity + 5, model2.BaseInfo.Product.Quantity);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void ReduceStockTest()
        {
            var cardId = CreateGroupCard();
            var model = _cardService.Get(cardId);
            _cardService.ReduceStock(cardId, 5);
            for (var i = 0; i < 3; i++)
            {
                Task.Delay(500).Wait();
                var model2 = _cardService.Get(cardId);
                if (model.BaseInfo.Product.Quantity == model2.BaseInfo.Product.Quantity)
                    continue;
                Assert.Equal(model.BaseInfo.Product.Quantity - 5, model2.BaseInfo.Product.Quantity);
                return;
            }

            Assert.True(false);
        }

        [Fact]
        public void UpdateCodeTest()
        {
            var list = _cardService.GetUserCardList(OpenId);

            var userCard = list.FirstOrDefault(i =>
            {
                try
                {
                    _cardService.SearchCode(i.Code);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
            if (userCard == null)
            {
                Assert.True(false, "没有可用的用户卡券。");
                //                Assert.Inconclusive("没有可用的用户卡券。");
            }

            const string newCode = "chusun1231231";
            _cardService.UpdateCode(userCard.Code, newCode, userCard.CardId);

            Assert.True(_cardService.GetUserCardList(OpenId, userCard.CardId).Any(i => i.Code == newCode));
        }

        [Fact]
        public void SearchCodeTest()
        {
            var list = _cardService.GetUserCardList(OpenId);
            if (!list.Any())
                return;

            var userCard = list.Last();

            var result = _cardService.SearchCode(userCard.Code);
            Assert.Equal(OpenId, result.OpenId);
            Assert.True(result.BeginTime > DateTimeHelper.StartTime);
            Assert.True(result.EndTime > DateTimeHelper.StartTime);
        }

        [Fact]
        public void InvalidTest()
        {
            var list = _cardService.GetUserCardList(OpenId);

            var userCard = list.FirstOrDefault(i =>
              {
                  try
                  {
                      _cardService.SearchCode(i.Code);
                      return true;
                  }
                  catch
                  {
                      return false;
                  }
              });
            if (userCard == null)
            {
                Assert.True(false, "没有可用的用户卡券。");
                //                Assert.Inconclusive("没有可用的用户卡券。");
            }

            _cardService.Invalid(userCard.CardId, userCard.Code);
            try
            {
                _cardService.SearchCode(userCard.Code);
                Assert.True(false);
            }
            catch (WeiXinException)
            {
            }
        }

        [Fact]
        public void UpdateTest()
        {
            /*            var cardId=_cardService.GetCardList().List.FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(cardId))
                        {
                            Assert.Inconclusive("没有可用的卡券。");
                        }
                        _cardService.Update(cardId, new CardUpdateModel
                        {
                        });*/
        }

        [Fact]
        public void DecryptCodeTest()
        {
            var code = _cardService.DecryptCode("XXIzTtMqCxwOaawoE91+VJdsFmv7b8g0VZIZkqf4GWA60Fzpc8ksZ/5ZZ0DVkXdE");

            Assert.Equal("992718526867", code);
        }

        [Fact]
        public void ConsumeTest()
        {
            var list = _cardService.GetUserCardList(OpenId);

            var userCard = list.FirstOrDefault(i =>
            {
                try
                {
                    _cardService.SearchCode(i.Code);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
            if (userCard == null)
            {
                Assert.True(false, "没有可用的用户卡券。");
                //                Assert.Inconclusive("没有可用的用户卡券。");
            }
            var result = _cardService.Consume(userCard.Code);
            Assert.Equal(OpenId, result.OpenId);
            Assert.Equal(userCard.CardId, result.CardId);
        }

        [Fact]
        public void CreateQrCodeTest()
        {
            var cardId = _cardService.GetCardList().List.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(cardId))
                Assert.True(false, "没有可用的用户卡券。");
            //                Assert.Inconclusive("没有卡券信息。");

            var result = _cardService.CreateQrCode(new CreateCardQrCodeModel(cardId));
            Assert.NotNull(result);
            Assert.NotNull(result.Ticket);
            Assert.NotNull(result.QrCodeUrl);
        }

        #endregion Test Method

        #region Private Method

        private string CreateGroupCard()
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
            return _cardService.Create(new GroupCardModel
            {
                BaseInfo = baseInfo,
                Detail = "双人套餐\n -进口红酒一支。\n孜然牛肉一份。"
            });
        }

        #endregion Private Method
    }
}