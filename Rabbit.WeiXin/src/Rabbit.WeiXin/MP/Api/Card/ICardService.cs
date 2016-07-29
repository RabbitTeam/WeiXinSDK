using Newtonsoft.Json;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// 批量查询卡列表。
        /// </summary>
        /// <param name="skip">查询卡列表的起始偏移量，从0开始，即offset: 5是指从从列表里的第六个开始读取。</param>
        /// <param name="take">需要查询的卡片的数量（数量最大50）。</param>
        /// <param name="status">支持开发者拉出指定状态的卡券列表，例：仅拉出通过审核的卡券。</param>
        /// <returns>卡券信息。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="take"/> 等于0或者大于50。</exception>
        CardListResult GetCardList(uint skip = 0, ushort take = 50, CardStatusEnum[] status = null);

        /// <summary>
        /// 根据卡券Id获取卡券信息。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <returns>卡券信息。</returns>
        CardModel Get(string cardId);

        /// <summary>
        /// 删除卡券。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        void Delete(string cardId);

        /// <summary>
        /// 设置卡券失效。
        /// </summary>
        /// <param name="cardId">卡券ID。</param>
        /// <param name="code">设置失效的Code码。</param>
        /// <remarks>
        /// 为满足改票、退款等异常情况，可调用卡券失效接口将用户的卡券设置为失效状态。 注：设置卡券失效的操作不可逆，即无法将设置为失效的卡券调回有效状态，商家须慎重调用该接口。
        /// </remarks>
        void Invalid(string cardId, string code);

        /// <summary>
        /// 查询code。
        /// </summary>
        /// <param name="code">单张卡券的唯一标识。</param>
        /// <param name="cardId">卡券ID代表一类卡券。</param>
        /// <remarks>调用查询code接口可获取code的有效性（非自定义code），该code对应的用户openid、卡券有效期等信息。 自定义code（use_custom_code为true）的卡券调用接口时，post数据中需包含card_id，非自定义code不需上报。</remarks>
        /// <returns>查询结果。</returns>
        SearchCodeResult SearchCode(string code, string cardId = null);

        /// <summary>
        /// 获取用户的卡券列表。
        /// </summary>
        /// <param name="openId">用户Id。</param>
        /// <param name="cardId">卡券Id。</param>
        /// <returns></returns>
        UserCardModel[] GetUserCardList(string openId, string cardId = null);

        /// <summary>
        /// 修改库存。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="increaseValue">增加的值。</param>
        /// <param name="reduceValue">减少的值。</param>
        /// <remarks>调用修改库存接口增减某张卡券的库存。</remarks>
        void UpdateStock(string cardId, uint? increaseValue, uint? reduceValue);

        /// <summary>
        /// 更新卡券。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="model">卡券更新模型。</param>
        /// <returns>此次更新是否需要审核。</returns>
        bool Update(string cardId, CardUpdateModel model);

        /// <summary>
        /// 核销卡券。
        /// </summary>
        /// <param name="code">需核销的Code码。</param>
        /// <param name="cardId">卡券ID。创建卡券时use_custom_code填写true时必填。非自定义Code不必填写。</param>
        /// <returns>核销卡券结果。</returns>
        ConsumeResult Consume(string code, string cardId = null);

        /// <summary>
        /// 解码Code。
        /// </summary>
        /// <param name="encryptCode">编码的Code值。</param>
        /// <returns>解码的Code值。</returns>
        string DecryptCode(string encryptCode);

        /// <summary>
        /// 更新卡券Code。
        /// </summary>
        /// <param name="code">需变更的Code码。</param>
        /// <param name="newCode">变更后的有效Code码。</param>
        /// <param name="cardId">卡券ID。自定义Code码卡券为必填。</param>
        void UpdateCode(string code, string newCode, string cardId = null);

        /// <summary>
        /// 创建领取卡券的二维码。
        /// </summary>
        /// <param name="model">创建领取卡券二维码模型。</param>
        /// <returns>创建领取卡券二维码结果。</returns>
        CreateCardQrCodeResult CreateQrCode(CreateCardQrCodeModel model);
    }

    /// <summary>
    /// 卡券服务扩展方法。
    /// </summary>
    public static class CardServiceExtensions
    {
        /// <summary>
        /// 添加库存。
        /// </summary>
        /// <param name="cardService">卡券服务。</param>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="value">增加的值。</param>
        public static void IncreaseStock(this ICardService cardService, string cardId, uint value)
        {
            cardService.NotNull("cardService").UpdateStock(cardId, value, null);
        }

        /// <summary>
        /// 减少库存。
        /// </summary>
        /// <param name="cardService">卡券服务。</param>
        /// <param name="cardId">卡券Id。</param>
        /// <param name="value">减少的值。</param>
        public static void ReduceStock(this ICardService cardService, string cardId, uint value)
        {
            cardService.NotNull("cardService").UpdateStock(cardId, null, value);
        }
    }

    #region Help Class

    /// <summary>
    /// 创建领取卡券二维码结果。
    /// </summary>
    public sealed class CreateCardQrCodeResult
    {
        /// <summary>
        /// 获取的二维码ticket。
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// 二维码访问地址。
        /// </summary>
        public string QrCodeUrl { get; set; }
    }

    /// <summary>
    /// 创建领取卡券二维码模型。
    /// </summary>
    public sealed class CreateCardQrCodeModel
    {
        #region Constructor

        /// <summary>
        /// 创建一个新的创建领取卡券二维码模型。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        public CreateCardQrCodeModel(string cardId)
        {
            CardId = cardId;
        }

        #endregion Constructor

        /// <summary>
        /// 卡券ID。
        /// </summary>
        [JsonProperty("card_id")]
        public string CardId { get; set; }

        /// <summary>
        /// use_custom_code字段为true的卡券必须填写，非自定义code不必填写。
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// 指定领取者的openid，只有该用户能领取。bind_openid字段为true的卡券必须填写，非指定openid不必填写。
        /// </summary>
        [JsonProperty("openid")]
        public string OpenId { get; set; }

        /// <summary>
        /// 指定二维码的有效时间，范围是60 ~ 1800秒。不填默认为永久有效。
        /// </summary>
        [JsonProperty("expire_seconds")]
        public ushort? ExpireSeconds { get; set; }

        /// <summary>
        /// 指定下发二维码，生成的二维码随机分配一个code，领取后不可再次扫描。填写true或false。默认false。
        /// </summary>
        [JsonProperty("is_unique_code")]
        public bool IsUniqueCode { get; set; }

        /// <summary>
        /// 领取场景值，用于领取渠道的数据统计，默认值为0，字段类型为整型，长度限制为60位数字。用户领取卡券后触发的事件推送中会带上此自定义场景值。
        /// </summary>
        [JsonProperty("outer_id")]
        public int OuterId { get; set; }
    }

    /// <summary>
    /// 消费结果。
    /// </summary>
    public sealed class ConsumeResult
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 卡券Id。
        /// </summary>
        public string CardId { get; set; }
    }

    /// <summary>
    /// 卡券更新模型。
    /// </summary>
    public sealed class CardUpdateModel
    {
        /// <summary>
        /// 卡券基础信息。
        /// </summary>
        public sealed class BaseInfo
        {
            /// <summary>
            /// 卡券的商户logo，建议像素为300*300。
            /// </summary>
            [Required, StringLength(128), JsonProperty("logo_url")]
            public string LogoUrl { get; set; }

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
            /// 客服电话。
            /// </summary>
            [StringLength(24), JsonProperty("service_phone")]
            public string ServicePhone { get; set; }

            /// <summary>
            /// 卡券使用说明，字数上限为1024个汉字。
            /// </summary>
            [Required, StringLength(3072), JsonProperty("description")]
            public string Description { get; set; }

            /// <summary>
            /// 门店位置ID。调用POI门店管理接口获取门店位置ID。
            /// </summary>
            [JsonProperty("location_id_list")]
            public long[] LocationIdList { get; set; }
        }

        /// <summary>
        /// 基础信息。
        /// </summary>
        [JsonProperty("base_info")]
        public BaseInfo Base { get; set; }

        /// <summary>
        /// 积分清零规则。
        /// </summary>
        [JsonProperty("bonus_cleared")]
        public string BonusCleared { get; set; }

        /// <summary>
        /// 积分规则。
        /// </summary>
        [JsonProperty("bonus_rules")]
        public string BonusRules { get; set; }

        /// <summary>
        /// 特权。
        /// </summary>
        [JsonProperty("prerogative")]
        public string Prerogative { get; set; }
    }

    /// <summary>
    /// 用户领取的卡券模型。
    /// </summary>
    public sealed class UserCardModel
    {
        /// <summary>
        /// 卡券代码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 卡券Id。
        /// </summary>
        public string CardId { get; set; }
    }

    /// <summary>
    /// 查询Code结果。
    /// </summary>
    public sealed class SearchCodeResult
    {
        /// <summary>
        /// 用户ID。
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 卡券Id。
        /// </summary>
        public string CardId { get; set; }

        /// <summary>
        /// 卡券起始使用时间。
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 卡券结束时间。
        /// </summary>
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// 卡券列表结果。
    /// </summary>
    public sealed class CardListResult
    {
        /// <summary>
        /// 卡券Id列表。
        /// </summary>
        [JsonProperty("card_id_list")]
        public string[] List { get; set; }

        /// <summary>
        /// 该商户名下卡券ID总数。
        /// </summary>
        [JsonProperty("total_num")]
        public long Count { get; set; }
    }

    /// <summary>
    /// 卡券状态。
    /// </summary>
    public enum CardStatusEnum
    {
        /// <summary>
        /// 待审核。
        /// </summary>
        NotVerify = 0,

        /// <summary>
        /// 审核失败。
        /// </summary>
        VerifyFall = 1,

        /// <summary>
        /// 通过审核。
        /// </summary>
        VerifyOk = 2,

        /// <summary>
        /// 卡券被用户删除。
        /// </summary>
        Delete = 3,

        /// <summary>
        /// 在公众平台投放过的卡券。
        /// </summary>
        Dispatch = 4
    }

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

    /// <summary>
    /// 券的基础信息。
    /// </summary>
    public abstract class CardBaseInfoBase
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

        /// <summary>
        /// 卡券使用时间信息基类。
        /// </summary>
        public abstract class UseTimeInfo
        {
            /// <summary>
            /// 使用时间的类型。
            /// </summary>
            [JsonIgnore]
            public abstract CardUseTimeType Type { get; }

            [JsonProperty("type")]
            internal string TypeString
            {
                get
                {
                    switch (Type)
                    {
                        case CardUseTimeType.FixedLong:
                            return "DATE_TYPE_FIX_TERM";

                        case CardUseTimeType.FixedTimeSpan:
                            return "DATE_TYPE_FIX_TIME_RANGE";
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// 固定日期区间的使用时间信息。
        /// </summary>
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
            [JsonIgnore]
            public override CardUseTimeType Type
            {
                get { return CardUseTimeType.FixedTimeSpan; }
            }

            #endregion Overrides of UseTimeInfo
        }

        /// <summary>
        /// 固定时常的使用时间信息。
        /// </summary>
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
            [JsonIgnore]
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
        [JsonIgnore]
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
                        return;

                    case "CODE_TYPE_BARCODE":
                        ViewType = CardViewType.BarCode;
                        return;

                    case "CODE_TYPE_ONLY_BARCODE":
                        ViewType = CardViewType.OnlyBarCode;
                        return;

                    case "CODE_TYPE_ONLY_QRCODE":
                        ViewType = CardViewType.OnlyQrCode;
                        return;

                    case "CODE_TYPE_QRCODE":
                        ViewType = CardViewType.QrCode;
                        return;
                }
                throw new NotSupportedException("不支持的类型：" + value);
            }
        }

        /// <summary>
        /// 卡券Id。
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

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
        public long[] LocationIdList { get; set; }

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

    /// <summary>
    /// 创建卡券的基础信息模型。
    /// </summary>
    public sealed class CreateCardBaseInfo : CardBaseInfoBase
    {
    }

    /// <summary>
    /// 获取卡券的基础信息模型。
    /// </summary>
    public sealed class CardBaseInfo : CardBaseInfoBase
    {
        /// <summary>
        /// 卡券状态。
        /// </summary>
        public CardStatusEnum Status { get; set; }
    }

    /// <summary>
    /// 券的基础模型。
    /// </summary>
    public abstract class CardModel
    {
        /// <summary>
        /// 卡券基本信息。
        /// </summary>
        [JsonProperty("base_info")]
        public CardBaseInfoBase BaseInfo { get; set; }

        /// <summary>
        /// 卡券类型。
        /// </summary>
        [JsonIgnore]
        public abstract CardType Type { get; }
    }

    /// <summary>
    /// 团购券。
    /// </summary>
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
        [JsonIgnore]
        public override CardType Type
        {
            get { return CardType.Group; }
        }

        #endregion Overrides of CardModel
    }

    /// <summary>
    /// 代金券。
    /// </summary>
    public sealed class CashCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        [JsonIgnore]
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

    /// <summary>
    /// 打折券。
    /// </summary>
    public sealed class DiscountCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        [JsonIgnore]
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

    /// <summary>
    /// 礼品券。
    /// </summary>
    public sealed class GiftCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        [JsonIgnore]
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

    /// <summary>
    /// 优惠券。
    /// </summary>
    public sealed class CouponCardModel : CardModel
    {
        #region Overrides of CardModel

        /// <summary>
        /// 卡券类型。
        /// </summary>
        [JsonIgnore]
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