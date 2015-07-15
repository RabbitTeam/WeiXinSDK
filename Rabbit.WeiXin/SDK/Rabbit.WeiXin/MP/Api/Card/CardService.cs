using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Card
{
    /// <summary>
    /// 卡券服务实现。
    /// </summary>
    public sealed class CardService : ICardService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个卡券服务。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public CardService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICardService

        /// <summary>
        /// 上传卡券Logo。
        /// </summary>
        /// <param name="bytes">文件字节组（大小限制1MB，像素为300*300，支持JPG格式。）。</param>
        /// <returns></returns>
        public string UploadLogo(byte[] bytes)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + _accountModel.GetAccessToken();

            var createBytes = new CreateBytes();

            var data = createBytes.JoinBytes(new ArrayList
            {
                createBytes.CreateFieldData("buffer", FileHelper.GetRandomFileName(bytes),
                    FileHelper.GetContentType(bytes), bytes)
            });
            var json = WeiXinHttpHelper.PostString(url, data, createBytes.ContentType);

            return JObject.Parse(json)["url"].Value<string>();
        }

        /// <summary>
        /// 创建卡券。
        /// </summary>
        /// <param name="model">卡券模型。</param>
        /// <returns>卡券Id。</returns>
        public string Create(CardModel model)
        {
            var url = "https://api.weixin.qq.com/card/create?access_token=" + _accountModel.GetAccessToken();
            var type = GetCardTypeString(model.Type);
            var postData = new
            {
                card = new
                {
                    card_type = type
                }
            };
            var postJson = JsonConvert.SerializeObject(postData);
            var postObj = JObject.Parse(postJson);
            postObj["card"][type.ToLower()] = JObject.Parse(JsonConvert.SerializeObject(model));
            var content = postObj.ToString();

            var json = WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(content));

            return JObject.Parse(json)["card_id"].Value<string>();
        }

        /// <summary>
        /// 获取卡券可用颜色。
        /// </summary>
        /// <returns>可用颜色数组。</returns>
        public CardColorItem[] GetCardColors()
        {
            var url = "https://api.weixin.qq.com/card/getcolors?access_token=" + _accountModel.GetAccessToken();

            var json = WeiXinHttpHelper.GetString(url);
            return JsonConvert.DeserializeObject<CardColorItem[]>(JObject.Parse(json)["colors"].ToString());
        }

        /// <summary>
        /// 批量查询卡列表。
        /// </summary>
        /// <param name="skip">查询卡列表的起始偏移量，从0开始，即offset: 5是指从从列表里的第六个开始读取。</param>
        /// <param name="take">需要查询的卡片的数量（数量最大50）。</param>
        /// <param name="status">支持开发者拉出指定状态的卡券列表，例：仅拉出通过审核的卡券。</param>
        /// <returns>卡券信息。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="take"/> 等于0或者大于50。</exception>
        public CardListResult GetCardList(uint skip, ushort take, CardStatusEnum[] status = null)
        {
            if (take > 50 || take == 0)
                throw new ArgumentOutOfRangeException("take", "查询的数量必须大于0小等于50。");

            var url = "https://api.weixin.qq.com/card/batchget?access_token=" + _accountModel.GetAccessToken();

            object postData;
            if (status == null)
            {
                postData = new { offset = skip, count = take };
            }
            else
            {
                postData = new { offset = skip, count = take, status_list = status.Select(GetStatusString).ToArray() };
            }

            return WeiXinHttpHelper.PostResultByJson<CardListResult>(url, postData);
        }

        /// <summary>
        /// 根据卡券Id获取卡券信息。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <returns>卡券信息。</returns>
        public CardModel Get(string cardId)
        {
            var url = "https://api.weixin.qq.com/card/get?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.PostString(url, new { card_id = cardId });

            var cardObj = JObject.Parse(content)["card"];

            var type = cardObj.Value<string>("card_type");
            var contentObj = cardObj[type.ToLower()];
            var baseInfoObj = contentObj["base_info"];
            var cardType = GetCardByString(type);

            CardModel card;

            switch (cardType)
            {
                case CardType.Cash:
                    card = new CashCardModel
                    {
                        LeastCost = contentObj.Value<uint>("least_cost"),
                        ReduceCost = contentObj.Value<uint>("reduce_cost")
                    };
                    break;

                case CardType.Coupon:
                    card = new CouponCardModel
                    {
                        Detail = contentObj.Value<string>("deal_detail")
                    };
                    break;

                case CardType.Discount:
                    card = new DiscountCardModel
                    {
                        Discount = contentObj.Value<ushort>("discount")
                    };
                    break;

                case CardType.Gift:
                    card = new GiftCardModel
                    {
                        Gift = contentObj.Value<string>("gift")
                    };
                    break;

                case CardType.Group:
                    card = new GroupCardModel
                    {
                        Detail = contentObj.Value<string>("deal_detail")
                    };
                    break;

                default:
                    throw new NotSupportedException("不支持的卡券类型：" + cardType);
            }

            var baseInfo = new CardBaseInfo
            {
                LogoUrl = baseInfoObj.Value<string>("logo_url"),
                ViewTypeString = baseInfoObj.Value<string>("code_type"),
                Id = baseInfoObj.Value<string>("id"),
                BrandName = baseInfoObj.Value<string>("brand_name"),
                Title = baseInfoObj.Value<string>("title"),
                SubTitle = baseInfoObj.Value<string>("sub_title"),
                Color = baseInfoObj.Value<string>("color"),
                Notice = baseInfoObj.Value<string>("notice"),
                Description = baseInfoObj.Value<string>("description"),
                Product = new CardBaseInfoBase.ProductInfo
                {
                    Quantity = baseInfoObj.SelectToken("sku.quantity").Value<uint>()
                },
                UseTime = GetUseTime(baseInfoObj["date_info"]),
                UseCustomCode = baseInfoObj.Value<bool>("use_custom_code"),
                BindOpenid = baseInfoObj.Value<bool>("bind_openid"),
                ServicePhone = baseInfoObj.Value<string>("service_phone"),
                LocationIdList = ((JArray)baseInfoObj["location_id_list"]).Select(i => i.Value<long>()).ToArray(),
                Source = baseInfoObj.Value<string>("source"),
                CustomUrlName = baseInfoObj.Value<string>("custom_url_name"),
                CustomUrl = baseInfoObj.Value<string>("custom_url"),
                CustomUrlSubTitle = baseInfoObj.Value<string>("custom_url_sub_title"),
                PromotionUrlName = baseInfoObj.Value<string>("promotion_url_name"),
                PromotionUrl = baseInfoObj.Value<string>("promotion_url"),
                PromotionUrlSubTitle = baseInfoObj.Value<string>("promotion_url_sub_title"),
                GetLimit = baseInfoObj.Value<uint>("get_limit"),
                AllowShare = baseInfoObj.Value<bool>("can_share"),
                AllowGive = baseInfoObj.Value<bool>("can_give_friend"),
                Status = GetStatusByString(baseInfoObj.Value<string>("status"))
            };

            card.BaseInfo = baseInfo;

            return card;
        }

        /// <summary>
        /// 删除卡券。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        public void Delete(string cardId)
        {
            var url = "https://api.weixin.qq.com/card/delete?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.PostString(url, new { card_id = cardId });
        }

        /// <summary>
        /// 设置卡券失效。
        /// </summary>
        /// <param name="cardId">卡券ID。</param>
        /// <param name="code">设置失效的Code码。</param>
        /// <remarks>
        /// 为满足改票、退款等异常情况，可调用卡券失效接口将用户的卡券设置为失效状态。 注：设置卡券失效的操作不可逆，即无法将设置为失效的卡券调回有效状态，商家须慎重调用该接口。
        /// </remarks>
        public void Invalid(string cardId, string code)
        {
            var url = "https://api.weixin.qq.com/card/code/unavailable?access_token=" + _accountModel.GetAccessToken();

            object postData;
            if (string.IsNullOrWhiteSpace(cardId))
            {
                postData = new { code };
            }
            else
            {
                postData = new { code, card_id = cardId };
            }

            WeiXinHttpHelper.PostString(url, postData);
        }

        /// <summary>
        /// 查询code。
        /// </summary>
        /// <param name="code">单张卡券的唯一标识。</param>
        /// <param name="cardId">卡券ID代表一类卡券。</param>
        /// <remarks>调用查询code接口可获取code的有效性（非自定义code），该code对应的用户openid、卡券有效期等信息。 自定义code（use_custom_code为true）的卡券调用接口时，post数据中需包含card_id，非自定义code不需上报。</remarks>
        /// <returns>查询结果。</returns>
        public SearchCodeResult SearchCode(string code, string cardId = null)
        {
            var url = "https://api.weixin.qq.com/card/code/get?access_token=" + _accountModel.GetAccessToken();

            object postData;
            if (string.IsNullOrWhiteSpace(cardId))
            {
                postData = new { code };
            }
            else
            {
                postData = new { code, card_id = cardId };
            }
            var content = WeiXinHttpHelper.PostString(url, postData);
            var obj = JObject.Parse(content);

            var card = obj["card"];
            return new SearchCodeResult
            {
                BeginTime = DateTimeHelper.GetTimeByTimeStamp(card.Value<ulong>("begin_time")),
                CardId = card.Value<string>("card_id"),
                EndTime = DateTimeHelper.GetTimeByTimeStamp(card.Value<ulong>("end_time")),
                OpenId = obj.Value<string>("openid")
            };
        }

        /// <summary>
        /// 获取用户的卡券列表。
        /// </summary>
        /// <param name="openId">用户Id。</param>
        /// <param name="cardId">卡券Id。</param>
        /// <returns></returns>
        public UserCardModel[] GetUserCardList(string openId, string cardId = null)
        {
            var url = "https://api.weixin.qq.com/card/user/getcardlist?access_token=" + _accountModel.GetAccessToken();

            object postData;
            if (string.IsNullOrWhiteSpace(cardId))
            {
                postData = new { openid = openId };
            }
            else
            {
                postData = new { openid = openId, card_id = cardId };
            }

            var content = WeiXinHttpHelper.PostString(url, postData);
            var obj = JObject.Parse(content);
            return ((JArray)obj["card_list"]).Select(
                i => new UserCardModel { CardId = i.Value<string>("card_id"), Code = i.Value<string>("code") }).ToArray();
        }

        /// <summary>
        /// 修改库存。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="increaseValue">增加的值。</param>
        /// <param name="reduceValue">减少的值。</param>
        /// <remarks>调用修改库存接口增减某张卡券的库存。</remarks>
        public void UpdateStock(string cardId, uint? increaseValue, uint? reduceValue)
        {
            var url = "https://api.weixin.qq.com/card/modifystock?access_token=" + _accountModel.GetAccessToken();

            object postData = null;

            if (!increaseValue.HasValue && !reduceValue.HasValue)
                throw new ArgumentException("增加库存的值与减少库存的值不能都为null");
            if (!increaseValue.HasValue)
                increaseValue = 0;
            if (!reduceValue.HasValue)
                reduceValue = 0;

            if (increaseValue.Value == reduceValue.Value)
                throw new ArgumentException("增加库存的值不能等于减少库存的值，因为这样没有任何意义。");

            if (increaseValue.Value > 0 && reduceValue.Value > 0)
                postData = new { card_id = cardId, increase_stock_value = increaseValue.Value, reduce_stock_value = reduceValue.Value };
            else if (increaseValue.Value > 0)
                postData = new { card_id = cardId, increase_stock_value = increaseValue.Value };
            else if (reduceValue.Value > 0)
                postData = new { card_id = cardId, reduce_stock_value = reduceValue.Value };

            if (postData == null)
                throw new ArgumentException("无效的参数值，请检查。");

            WeiXinHttpHelper.PostString(url, postData);
        }

        /// <summary>
        /// 更新卡券。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="model">卡券更新模型。</param>
        /// <returns>此次更新是否需要审核。</returns>
        public bool Update(string cardId, CardUpdateModel model)
        {
            var url = "https://api.weixin.qq.com/card/update?access_token=" + _accountModel.GetAccessToken();
            var obj = new JObject();

            obj["card_id"] = cardId;
            obj["member_card"] = JObject.Parse(JsonConvert.SerializeObject(model));

            var content = WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(obj.ToString()));
            return JObject.Parse(content).Value<bool>("send_check");
        }

        /// <summary>
        /// 核销卡券。
        /// </summary>
        /// <param name="code">需核销的Code码。</param>
        /// <param name="cardId">卡券ID。创建卡券时use_custom_code填写true时必填。非自定义Code不必填写。</param>
        /// <returns>核销卡券结果。</returns>
        public ConsumeResult Consume(string code, string cardId = null)
        {
            var url = "https://api.weixin.qq.com/card/code/consume?access_token=" + _accountModel.GetAccessToken();

            object postData;

            if (string.IsNullOrWhiteSpace(cardId))
            {
                postData = new { code };
            }
            else
            {
                postData = new { code, card_id = cardId };
            }

            var content = WeiXinHttpHelper.PostString(url, postData);
            var obj = JObject.Parse(content);

            return new ConsumeResult
            {
                CardId = obj.SelectToken("card.card_id").Value<string>(),
                OpenId = obj.Value<string>("openid")
            };
        }

        /// <summary>
        /// 解码Code。
        /// </summary>
        /// <param name="encryptCode">编码的Code值。</param>
        /// <returns>解码的Code值。</returns>
        public string DecryptCode(string encryptCode)
        {
            var url = "https://api.weixin.qq.com/card/code/decrypt?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.PostString(url, new { encrypt_code = encryptCode });

            return JObject.Parse(content).Value<string>("code");
        }

        /// <summary>
        /// 更新卡券Code。
        /// </summary>
        /// <param name="code">需变更的Code码。</param>
        /// <param name="newCode">变更后的有效Code码。</param>
        /// <param name="cardId">卡券ID。自定义Code码卡券为必填。</param>
        public void UpdateCode(string code, string newCode, string cardId = null)
        {
            var url = "https://api.weixin.qq.com/card/code/update?access_token=" + _accountModel.GetAccessToken();
            object postData;
            if (string.IsNullOrWhiteSpace(cardId))
            {
                postData = new { code, new_code = newCode };
            }
            else
            {
                postData = new { card_id = cardId, code, new_code = newCode };
            }

            WeiXinHttpHelper.Post(url, postData);
        }

        /// <summary>
        /// 创建领取卡券的二维码。
        /// </summary>
        /// <param name="model">创建领取卡券二维码模型。</param>
        /// <returns>创建领取卡券二维码结果。</returns>
        public CreateCardQrCodeResult CreateQrCode(CreateCardQrCodeModel model)
        {
            var url = "https://api.weixin.qq.com/card/qrcode/create?access_token=" + _accountModel.GetAccessToken();

            dynamic cardData = new ExpandoObject();

            if (!string.IsNullOrWhiteSpace(model.OpenId))
                cardData.openid = model.OpenId;
            if (!string.IsNullOrWhiteSpace(model.Code))
                cardData.code = model.Code;
            cardData.card_id = model.CardId;
            if (model.ExpireSeconds.HasValue)
                cardData.expire_seconds = 60;
            cardData.is_unique_code = model.IsUniqueCode;
            cardData.outer_id = model.OuterId;

            var content = WeiXinHttpHelper.PostString(url, new
            {
                action_name = "QR_CARD",
                action_info = new
                {
                    card = cardData
                }
            });
            var ticket = JObject.Parse(content).Value<string>("ticket");
            return new CreateCardQrCodeResult
            {
                QrCodeUrl = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ticket,
                Ticket = ticket
            };
        }

        #endregion Implementation of ICardService

        #region Private Method

        private static CardBaseInfoBase.UseTimeInfo GetUseTime(JToken token)
        {
            var type = token.Value<string>("type");
            switch (GetTimeTypeByString(type))
            {
                case CardUseTimeType.FixedLong:
                    return new CardBaseInfoBase.UseTimeFixedLongInfo
                    {
                        FixedBeginTerm = token.Value<uint>("fixed_begin_term"),
                        FixedTerm = token.Value<uint>("fixed_term")
                    };

                case CardUseTimeType.FixedTimeSpan:
                    return new CardBaseInfoBase.UseTimeFixedTimeSpanInfo
                    {
                        BeginTimestamp = token.Value<ulong>("begin_timestamp"),
                        EndTimestamp = token.Value<ulong>("end_timestamp")
                    };
            }
            throw new NotSupportedException("不支持的时间类型：" + type);
        }

        private static CardUseTimeType GetTimeTypeByString(string type)
        {
            switch (type)
            {
                case "DATE_TYPE_FIX_TIME_RANGE":
                    return CardUseTimeType.FixedTimeSpan;

                case "DATE_TYPE_FIX_TERM":
                    return CardUseTimeType.FixedLong;
            }
            throw new NotSupportedException("不支持的时间类型：" + type);
        }

        private static CardStatusEnum GetStatusByString(string status)
        {
            switch (status)
            {
                case "CARD_STATUS_NOT_VERIFY":
                    return CardStatusEnum.NotVerify;

                case "CARD_STATUS_VERIFY_FALL":
                    return CardStatusEnum.VerifyFall;

                case "CARD_STATUS_VERIFY_OK":
                    return CardStatusEnum.VerifyOk;

                case "CARD_STATUS_USER_DELETE":
                case "CARD_STATUS_DELETE":
                    return CardStatusEnum.Delete;

                case "CARD_STATUS_USER_DISPATCH":
                    return CardStatusEnum.Dispatch;
            }
            throw new NotSupportedException("不支持的卡券状态类型：" + status);
        }

        private static string GetStatusString(CardStatusEnum status)
        {
            switch (status)
            {
                case CardStatusEnum.NotVerify:
                    return "CARD_STATUS_NOT_VERIFY";

                case CardStatusEnum.VerifyFall:
                    return "CARD_STATUS_VERIFY_FALL";

                case CardStatusEnum.VerifyOk:
                    return "CARD_STATUS_VERIFY_OK";

                case CardStatusEnum.Delete:
                    return "CARD_STATUS_USER_DELETE";

                case CardStatusEnum.Dispatch:
                    return "CARD_STATUS_USER_DISPATCH";
            }
            throw new NotSupportedException("不支持的卡券状态类型：" + status);
        }

        private static CardType GetCardByString(string type)
        {
            switch (type)
            {
                case "GROUPON":
                    return CardType.Group;

                case "CASH":
                    return CardType.Cash;

                case "DISCOUNT":
                    return CardType.Discount;

                case "GIFT":
                    return CardType.Gift;

                case "GENERAL_COUPON":
                    return CardType.Coupon;
            }
            throw new NotSupportedException("不支持的卡券类型：" + type);
        }

        private static string GetCardTypeString(CardType type)
        {
            switch (type)
            {
                case CardType.Group:
                    return "GROUPON";

                case CardType.Cash:
                    return "CASH";

                case CardType.Discount:
                    return "DISCOUNT";

                case CardType.Gift:
                    return "GIFT";

                case CardType.Coupon:
                    return "GENERAL_COUPON";
            }
            throw new NotSupportedException("不支持的卡券类型：" + type);
        }

        #endregion Private Method
    }
}