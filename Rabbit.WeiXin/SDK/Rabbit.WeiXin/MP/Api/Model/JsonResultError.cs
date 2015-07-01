using Newtonsoft.Json;

namespace Rabbit.WeiXin.MP.Api.Model
{
    /// <summary>
    /// 表示json错误的结果值。
    /// </summary>
    public sealed class JsonResultError
    {
        /// <summary>
        /// 错误代码。
        /// </summary>
        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 错误信息。
        /// </summary>
        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
    }
}