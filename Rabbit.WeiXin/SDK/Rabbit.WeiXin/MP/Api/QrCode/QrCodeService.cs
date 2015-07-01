using Newtonsoft.Json;
using Rabbit.WeiXin.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;

namespace Rabbit.WeiXin.MP.Api.QrCode
{
    /// <summary>
    /// 一个抽象的二维码服务接口。
    /// </summary>
    public interface IQrCodeService
    {
        /// <summary>
        /// 创建一个二维码。
        /// </summary>
        /// <param name="model">创建二维码模型。</param>
        /// <returns>创建二维码的结果模型。</returns>
        CreateQrCodeResultModel Create(CreateQrCodeModel model);

        /// <summary>
        /// 根据 <paramref name="ticket"/> 得到一个二维码信息。
        /// </summary>
        /// <param name="ticket">获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。</param>
        /// <returns>获取二维码的结果模型。</returns>
        GetQrCodeResultModel Get(string ticket);

        /// <summary>
        /// 获取二维码访问的Url。
        /// </summary>
        /// <param name="ticket">获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。</param>
        /// <returns>二维码Url地址。</returns>
        string GetQrCodeUrl(string ticket);
    }

    /// <summary>
    /// 二维码服务实现。
    /// </summary>
    public sealed class QrCodeService : IQrCodeService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public QrCodeService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IQrCodeService

        /// <summary>
        /// 创建一个二维码。
        /// </summary>
        /// <param name="model">创建二维码模型。</param>
        /// <returns>创建二维码的结果模型。</returns>
        public CreateQrCodeResultModel Create(CreateQrCodeModel model)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + _accountModel.GetAccessToken();

            return WeiXinHttpHelper.PostResultByJson<CreateQrCodeResultModel>(url, model);
        }

        /// <summary>
        /// 根据 <paramref name="ticket"/> 得到一个二维码信息。
        /// </summary>
        /// <param name="ticket">获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。</param>
        /// <returns>获取二维码的结果模型。</returns>
        public GetQrCodeResultModel Get(string ticket)
        {
            WebHeaderCollection responseHeaders;
            var data = HttpHelper.Get(GetQrCodeUrl(ticket), out responseHeaders);

            return new GetQrCodeResultModel
            {
                ContentLength = long.Parse(responseHeaders["Content-Length"]),
                ContentType = responseHeaders["Content-Type"],
                Data = data
            };
        }

        /// <summary>
        /// 获取二维码访问的Url。
        /// </summary>
        /// <param name="ticket">获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。</param>
        /// <returns>二维码Url地址。</returns>
        public string GetQrCodeUrl(string ticket)
        {
            return "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + HttpUtility.UrlEncode(ticket);
        }

        #endregion Implementation of IQrCodeService
    }

    #region Help Class

    /// <summary>
    /// 二维码服务扩展方法。
    /// </summary>
    public static class QrCodeServiceExtensions
    {
        /// <summary>
        /// 创建一个临时二维码。
        /// </summary>
        /// <param name="qrCodeService">二维码服务。</param>
        /// <param name="model">创建二维码模型。</param>
        /// <returns>创建二维码的结果模型。</returns>
        public static CreateQrCodeResultModel CreateTemporary(this IQrCodeService qrCodeService, CreateTemporaryQrCodeModel model)
        {
            return qrCodeService.NotNull("qrCodeService").Create(model);
        }

        /// <summary>
        /// 创建一个永久二维码。
        /// </summary>
        /// <param name="qrCodeService">二维码服务。</param>
        /// <param name="model">创建二维码模型。</param>
        /// <returns>创建二维码的结果模型。</returns>
        public static CreateQrCodeResultModel CreateForever(this IQrCodeService qrCodeService, CreateForeverQrCodeModel model)
        {
            return qrCodeService.NotNull("qrCodeService").Create(model);
        }
    }

    /// <summary>
    /// 二维码类型。
    /// </summary>
    public enum QrCodeType
    {
        /// <summary>
        /// 临时。
        /// </summary>
        Temporary = 0,

        /// <summary>
        /// 永久。
        /// </summary>
        Forever = 1,

        /// <summary>
        /// 永久的字符串参数值。
        /// </summary>
        ForeverString = 2
    }

    /// <summary>
    /// 创建二维码模型基类。
    /// </summary>
    public abstract class CreateQrCodeModel
    {
        #region Field

        /// <summary>
        /// 二维码类型映射。
        /// </summary>
        public static readonly IDictionary<QrCodeType, string> QrCodeTypeMappings = new Dictionary<QrCodeType, string>
        {
            {QrCodeType.Temporary, "QR_SCENE"},
            {QrCodeType.Forever, "QR_LIMIT_SCENE"},
            {QrCodeType.ForeverString, "QR_LIMIT_STR_SCENE"},
        };

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的创建二维码模型。
        /// </summary>
        protected CreateQrCodeModel()
        {
        }

        /// <summary>
        /// 初始化一个新的创建二维码模型。
        /// </summary>
        /// <param name="qrCodeType">二维码类型。</param>
        protected CreateQrCodeModel(QrCodeType qrCodeType)
        {
            Type = qrCodeType;
        }

        /// <summary>
        /// 初始化一个新的创建二维码模型。
        /// </summary>
        /// <param name="qrCodeType">二维码类型。</param>
        /// <param name="sceneId">数字型的场景Id。</param>
        protected CreateQrCodeModel(QrCodeType qrCodeType, uint sceneId)
            : this(qrCodeType)
        {
            SceneId = sceneId;
        }

        #endregion Constructor

        /// <summary>
        /// 二维码类型。
        /// </summary>
        public QrCodeType Type
        {
            get
            {
                var item = QrCodeTypeMappings.SingleOrDefault(i => i.Value.Equals(ActionName));
                if (item.Value == null)
                    throw new NotSupportedException("不支持的二维码类型：" + ActionName);
                return item.Key;
            }
            set
            {
                ActionName = QrCodeTypeMappings[value];
            }
        }

        /// <summary>
        /// 二维码类型，QR_SCENE为临时,QR_LIMIT_SCENE为永久,QR_LIMIT_STR_SCENE为永久的字符串参数值
        /// </summary>
        [JsonProperty("action_name")]
        internal string ActionName { get; set; }

        /// <summary>
        /// 二维码详细信息。
        /// </summary>
        [JsonProperty("action_info")]
        public string ActionInfo { get; set; }

        private uint? _sceneId;

        /// <summary>
        /// 场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）
        /// </summary>
        [Range(1, 100000), JsonProperty("scene_id")]
        public uint? SceneId
        {
            get { return _sceneId; }
            set
            {
                if (!value.HasValue)
                {
                    _sceneId = null;
                    return;
                }
                if (value <= 0 || value > 100000)
                    throw new ArgumentException("SceneId 必需在 1~100000 之间。");
                _sceneId = value;
            }
        }
    }

    /// <summary>
    /// 创建临时二维码模型。
    /// </summary>
    public sealed class CreateTemporaryQrCodeModel : CreateQrCodeModel
    {
        /// <summary>
        /// 初始化一个新的创建临时二维码模型。
        /// </summary>
        public CreateTemporaryQrCodeModel()
        {
            Type = QrCodeType.Temporary;
            ExpireSeconds = 604800;
        }

        /// <summary>
        /// 初始化一个新的创建临时二维码模型。
        /// </summary>
        /// <param name="sceneId">数字型的场景Id。</param>
        /// <param name="expireSeconds">该二维码有效时间，以秒为单位。 最大不超过604800（即7天）。</param>
        public CreateTemporaryQrCodeModel(uint sceneId = 0, uint expireSeconds = 604800)
            : base(QrCodeType.Temporary, sceneId)
        {
            ExpireSeconds = expireSeconds;
        }

        private uint _expireSeconds;

        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过604800（即7天）。
        /// </summary>
        [JsonProperty("expire_seconds"), Range(60, 604800)]
        public uint ExpireSeconds
        {
            get { return _expireSeconds; }
            set
            {
                if (value < 60 || value > 604800)
                    throw new ArgumentOutOfRangeException("value", value, "有效时间不能小于0秒且不大于604800秒。");
                _expireSeconds = value;
            }
        }
    }

    /// <summary>
    /// 创建永久二维码模型。
    /// </summary>
    public sealed class CreateForeverQrCodeModel : CreateQrCodeModel
    {
        /// <summary>
        /// 初始化一个新的创建永久二维码模型。
        /// </summary>
        public CreateForeverQrCodeModel()
        {
            Type = QrCodeType.Forever;
        }

        /// <summary>
        /// 初始化一个新的创建永久二维码模型。
        /// </summary>
        /// <param name="sceneId">数字型的场景Id。</param>
        public CreateForeverQrCodeModel(uint sceneId)
            : base(QrCodeType.Forever, sceneId)
        {
            SceneId = sceneId;
            Type = QrCodeType.Forever;
        }

        /// <summary>
        /// 初始化一个新的创建永久二维码模型。
        /// </summary>
        /// <param name="sceneString">字符串型的场景Id。</param>
        public CreateForeverQrCodeModel(string sceneString)
            : base(QrCodeType.Forever)
        {
            SceneString = sceneString;
            Type = QrCodeType.Forever;
        }

        /// <summary>
        /// 场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段。
        /// </summary>
        [JsonProperty("scene_str")]
        public string SceneString { get; set; }
    }

    /// <summary>
    /// 创建二维码结果模型。
    /// </summary>
    public sealed class CreateQrCodeResultModel
    {
        /// <summary>
        /// 获取的二维码ticket，凭借此ticket可以在有效时间内换取二维码。
        /// </summary>
        public string Ticket { get; set; }

        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过604800（即7天）。
        /// </summary>
        [JsonProperty("expire_seconds"), Range(0, 604800)]
        public uint ExpireSeconds { get; set; }

        /// <summary>
        /// 二维码图片解析后的地址，开发者可根据该地址自行生成需要的二维码图片.
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 获取二维码结果模型。
    /// </summary>
    public sealed class GetQrCodeResultModel
    {
        /// <summary>
        /// 内容长度。
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// 内容类型。
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 二维码数据。
        /// </summary>
        public byte[] Data { get; set; }
    }

    #endregion Help Class
}