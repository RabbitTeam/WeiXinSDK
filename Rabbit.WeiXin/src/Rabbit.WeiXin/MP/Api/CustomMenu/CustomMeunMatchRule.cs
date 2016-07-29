using Newtonsoft.Json;
using Rabbit.WeiXin.MP.Api.User;

namespace Rabbit.WeiXin.MP.Api.CustomMenu
{
    /// <summary>
    /// 自定义菜单匹配模型。
    /// </summary>
    public sealed class CustomMeunMatchRule
    {
        /// <summary>
        /// 用户分组id，可通过用户分组管理接口获取。
        /// </summary>
        [JsonProperty("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// 性别：男（1）女（2），不填则不做匹配
        /// </summary>
        [JsonIgnore]
        public SexEnum? Sex
        {
            get
            {
                if (!SexNumber.HasValue)
                    return null;
                switch (SexNumber.Value)
                {
                    case 1:
                        return SexEnum.Male;

                    case 2:
                        return SexEnum.Female;

                    default:
                        return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    SexNumber = (int)value.Value;
                }
                else
                {
                    SexNumber = null;
                }
            }
        }

        [JsonProperty("sex")]
        internal int? SexNumber { get; set; }

        /// <summary>
        /// 客户端版本，当前只具体到系统型号：IOS(1), Android(2),Others(3)，不填则不做匹配
        /// </summary>
        [JsonProperty("client_platform_type")]
        public PlatformTypeEnum ClientPlatformType { get; set; }

        /// <summary>
        /// 国家信息，是用户在微信中设置的地区，具体请参考地区信息表。
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// 省份信息，是用户在微信中设置的地区，具体请参考地区信息表。
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市信息，是用户在微信中设置的地区，具体请参考地区信息表。
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }
    }
}