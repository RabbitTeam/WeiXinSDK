using Newtonsoft.Json;

namespace Rabbit.WeiXin.MP.Api.Model
{
    public class JsonResultError : IJsonResult
    {
        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
    }
}