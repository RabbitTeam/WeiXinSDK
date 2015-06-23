using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Card
{
    /// <summary>
    /// 一个抽象的卡券服务。
    /// </summary>
    public interface ICardService
    {
        /// <summary>
        /// 上传卡券Logo。
        /// </summary>
        /// <param name="bytes">文件字节组（大小限制1MB，像素为300*300，支持JPG格式。）。</param>
        /// <returns></returns>
        string UploadLogo(byte[] bytes);

        /// <summary>
        /// 创建卡券。
        /// </summary>
        /// <param name="model">卡券模型。</param>
        /// <returns>卡券Id。</returns>
        string Create(CardModel model);

        /// <summary>
        /// 获取卡券可用颜色。
        /// </summary>
        /// <returns>可用颜色数组。</returns>
        CardColorItem[] GetCardColors();
    }

    /// <summary>
    /// 卡券服务实现。
    /// </summary>
    public sealed class CardService : ICardService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

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
            postObj[type] = JObject.Parse(JsonConvert.SerializeObject(model));

            var json = WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(postObj.ToString()));

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

        #endregion Implementation of ICardService

        #region Private Method

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

    #region Help Class

    /// <summary>
    /// 卡券颜色项。
    /// </summary>
    public sealed class CardColorItem
    {
        /// <summary>
        /// 颜色名称。
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 颜色值（如：#55bd47）。
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    /// <summary>
    /// 卡券类型。
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 团购。
        /// </summary>
        Group = 0,

        /// <summary>
        /// 代金券。
        /// </summary>
        Cash = 1,

        /// <summary>
        /// 折扣。
        /// </summary>
        Discount = 2,

        /// <summary>
        /// 礼品。
        /// </summary>
        Gift = 3,

        /// <summary>
        /// 优惠券。
        /// </summary>
        Coupon = 4
    }

    /// <summary>
    /// 卡券展示类型。
    /// </summary>
    public enum CardViewType
    {
        /// <summary>
        /// 文本。
        /// </summary>
        Text = 0,

        /// <summary>
        /// 一维码。
        /// </summary>
        BarCode = 1,

        /// <summary>
        /// 二维码。
        /// </summary>
        QrCode = 2,

        /// <summary>
        /// 二维码无Code。
        /// </summary>
        OnlyQrCode = 3,

        /// <summary>
        /// 一维码无Code。
        /// </summary>
        OnlyBarCode = 4
    }

    /// <summary>
    /// 卡券使用时间类型。
    /// </summary>
    public enum CardUseTimeType
    {
        /// <summary>
        /// 固定日期区间。
        /// </summary>
        FixedTimeSpan = 1,

        /// <summary>
        /// 固定时常。
        /// </summary>
        FixedLong = 2
    }

    public sealed class CardBaseInfo
    {
        /// <summary>
        /// 商品信息。
        /// </summary>
        public sealed class ProductInfo
        {
            /// <summary>
            /// 卡券库存的数量，不支持填写0，上限为100000000。
            /// </summary>
            [Range(0, 100000), JsonProperty("quantity")]
            public uint? Quantity { get; set; }
        }

        public abstract class UseTimeInfo
        {
            /// <summary>
            /// 使用时间的类型。
            /// </summary>
            [JsonProperty("type")]
            public abstract CardUseTimeType Type { get; }
        }

        public sealed class UseTimeFixedTimeSpanInfo : UseTimeInfo
        {
            /// <summary>
            /// Type为FixedTimeSpan时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）
            /// </summary>
            [JsonProperty("begin_timestamp")]
            internal ulong BeginTimestamp { get; set; }

            /// <summary>
            /// Type为FixedTimeSpan时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）
            /// </summary>
            [JsonProperty("end_timestamp")]
            internal ulong EndTimestamp { get; set; }

            #region Overrides of UseTimeInfo

            /// <summary>
            /// 使用时间的类型。
            /// </summary>
            public override CardUseTimeType Type
            {
                get { return CardUseTimeType.FixedTimeSpan; }
            }

            #endregion Overrides of UseTimeInfo
        }

        public sealed class UseTimeFixedLongInfo : UseTimeInfo
        {
            /// <summary>
            /// Type为FixedLong时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）
            /// </summary>
            [JsonProperty("fixed_term")]
            public uint FixedTerm { get; set; }

            /// <summary>
            /// Type为FixedLong时专用，表示自领取后多少天开始生效。（单位为天）
            /// </summary>
            [JsonProperty("fixed_begin_term")]
            public uint FixedBeginTerm { get; set; }

            #region Overrides of UseTimeInfo

            /// <summary>
            /// 使用时间的类型。
            /// </summary>
            public override CardUseTimeType Type
            {
                get { return CardUseTimeType.FixedLong; }
            }

            #endregion Overrides of UseTimeInfo
        }

        /// <summary>
        /// 卡券的商户logo，建议像素为300*300。
        /// </summary>
        [Required, StringLength(128), JsonProperty("logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// 展示类型。
        /// </summary>
        public CardViewType ViewType { get; set; }

        [JsonProperty("code_type")]
        internal string ViewTypeString
        {
            get
            {
                switch (ViewType)
                {
                    case CardViewType.Text:
                        return "CODE_TYPE_TEXT";

                    case CardViewType.BarCode:
                        return "CODE_TYPE_BARCODE";

                    case CardViewType.OnlyBarCode:
                        return "CODE_TYPE_ONLY_BARCODE";

                    case CardViewType.OnlyQrCode:
                        return "CODE_TYPE_ONLY_QRCODE";

                    case CardViewType.QrCode:
                        return "CODE_TYPE_QRCODE";
                }
                throw new NotSupportedException("不支持的类型：" + ViewType);
            }
            set
            {
                switch (value)
                {
                    case "CODE_TYPE_TEXT":
                        ViewType = CardViewType.Text;
                        break;

                    case "CODE_TYPE_BARCODE":
                        ViewType = CardViewType.BarCode;
                        break;

                    case "CODE_TYPE_ONLY_BARCODE":
                        ViewType = CardViewType.OnlyBarCode;
                        break;

                    case "CODE_TYPE_ONLY_QRCODE":
                        ViewType = CardViewType.OnlyQrCode;
                        break;

                    case "CODE_TYPE_QRCODE":
                        ViewType = CardViewType.QrCode;
                        break;
                }
                throw new NotSupportedException("不支持的类型：" + value);
            }
        }

        /// <summary>
        /// 商户名字,字数上限为12个汉字。
        /// </summary>
        [Required, StringLength(36), JsonProperty("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。
        /// </summary>
        [Required, StringLength(27), JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 券名，字数上限为18个汉字。
        /// </summary>
        [StringLength(54), JsonProperty("sub_title")]
        public string SubTitle { get; set; }

        /// <summary>
        /// 券颜色。按色彩规范标注填写Color010-Color100。
        /// </summary>
        /// <remarks>可用颜色介绍：[http://mp.weixin.qq.com/wiki/8/b7e310e7943f7763450eced91fa793b0.html]</remarks>
        [Required, StringLength(16), JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// 卡券使用提醒，字数上限为16个汉字。
        /// </summary>
        [Required, StringLength(48), JsonProperty("notice")]
        public string Notice { get; set; }

        /// <summary>
        /// 卡券使用说明，字数上限为1024个汉字。
        /// </summary>
        [Required, StringLength(3072), JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 商品信息。
        /// </summary>
        [JsonProperty("sku")]
        public ProductInfo Product { get; set; }

        /// <summary>
        /// 使用日期，有效期的信息。
        /// </summary>
        [Required, JsonProperty("date_info")]
        public UseTimeInfo UseTime { get; set; }

        /// <summary>
        /// 是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。
        /// </summary>
        [JsonProperty("use_custom_code")]
        public bool UseCustomCode { get; set; }

        /// <summary>
        /// 是否指定用户领取，填写true或false。默认为false。
        /// </summary>
        [JsonProperty("bind_openid")]
        public bool BindOpenid { get; set; }

        /// <summary>
        /// 客服电话。
        /// </summary>
        [StringLength(24), JsonProperty("service_phone")]
        public string ServicePhone { get; set; }

        /// <summary>
        /// 门店位置ID。调用POI门店管理接口获取门店位置ID。
        /// </summary>
        [JsonProperty("location_id_list")]
        public double[] LocationIdList { get; set; }

        /// <summary>
        /// 第三方来源名，例如同程旅游、大众点评。
        /// </summary>
        [StringLength(36), JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// 自定义跳转外链的入口名字。
        /// </summary>
        [StringLength(15), JsonProperty("custom_url_name")]
        public string CustomUrlName { get; set; }

        /// <summary>
        /// 自定义跳转的URL。
        /// </summary>
        [StringLength(128), JsonProperty("custom_url")]
        public string CustomUrl { get; set; }

        /// <summary>
        /// 显示在入口右侧的提示语。
        /// </summary>
        [StringLength(18), JsonProperty("custom_url_sub_title")]
        public string CustomUrlSubTitle { get; set; }

        /// <summary>
        /// 营销场景的自定义入口名称。
        /// </summary>
        [StringLength(15), JsonProperty("promotion_url_name")]
        public string PromotionUrlName { get; set; }

        /// <summary>
        /// 入口跳转外链的地址链接。
        /// </summary>
        [StringLength(128), JsonProperty("promotion_url")]
        public string PromotionUrl { get; set; }

        /// <summary>
        /// 显示在营销入口右侧的提示语。
        /// </summary>
        [StringLength(18), JsonProperty("promotion_url_sub_title")]
        public string PromotionUrlSubTitle { get; set; }

        /// <summary>
        /// 每人可领券的数量限制。
        /// </summary>
        [JsonProperty("get_limit")]
        public uint GetLimit { get; set; }

        /// <summary>
        /// 卡券领取页面是否可分享。
        /// </summary>
        [JsonProperty("can_share")]
        public bool AllowShare { get; set; }

        /// <summary>
        /// 卡券是否可转赠。
        /// </summary>
        [JsonProperty("can_give_friend")]
        public bool AllowGive { get; set; }
    }

    public abstract class CardModel
    {
        /// <summary>
        /// 卡券基本信息。
        /// </summary>
        [JsonProperty("base_info")]
        public CardBaseInfo BaseInfo { get; set; }

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public abstract CardType Type { get; }
    }

    public sealed class GroupCardModel : CardModel
    {
        /// <summary>
        /// 团购券专用，团购详情。
        /// </summary>
        [Required, StringLength(24), JsonProperty("deal_detail")]
        public string Detail { get; set; }

        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public override CardType Type
        {
            get { return CardType.Group; }
        }

        #endregion Overrides of CardModel
    }

    public sealed class CashCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public override CardType Type
        {
            get { return CardType.Cash; }
        }

        #endregion Overrides of CardModel

        /// <summary>
        /// 表示起用金额。（单位为分）
        /// </summary>
        [JsonProperty("least_cost")]
        public uint LeastCost { get; set; }

        /// <summary>
        /// 表示减免金额。（单位为分）
        /// </summary>
        [JsonProperty("reduce_cost")]
        public uint ReduceCost { get; set; }
    }

    public sealed class DiscountCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public override CardType Type
        {
            get { return CardType.Discount; }
        }

        #endregion Overrides of CardModel

        /// <summary>
        /// 折扣券专用，表示打折额度（百分比）。填30就是七折。
        /// </summary>
        [JsonProperty("discount")]
        public ushort Discount { get; set; }
    }

    public sealed class GiftCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public override CardType Type
        {
            get { return CardType.Gift; }
        }

        #endregion Overrides of CardModel

        /// <summary>
        /// 填写礼品的名称。
        /// </summary>
        [Required, StringLength(3072), JsonProperty("gift")]
        public string Gift { get; set; }
    }

    public sealed class CouponCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        public override CardType Type
        {
            get { return CardType.Coupon; }
        }

        #endregion Overrides of CardModel

        /// <summary>
        /// 填写优惠详情。
        /// </summary>
        [Required, StringLength(3072), JsonProperty("default_detail")]
        public string Detail { get; set; }
    }

    #endregion Help Class
}