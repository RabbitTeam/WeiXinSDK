using Newtonsoft.Json;
using Rabbit.WeiXin.MP.Api.Model;
using System.Text;

namespace Rabbit.WeiXin.Utility
{
    internal static class ResultHelper
    {
        public static void TryThrowError(byte[] data)
        {
            //一般情况错误信息不会大于200，为了提升性能这边加入这个判断。
            if (data.Length > 200)
                return;
            var jsonContent = Encoding.UTF8.GetString(data);

            TryThrowError(jsonContent);
        }

        public static void TryThrowError(string jsonContent)
        {
            var error = IsError(jsonContent) ? TryGetError(jsonContent) : null;
            if (error != null)
                throw new WeiXinException(error.ErrorCode, error.ErrorMessage);
        }

        public static JsonResultError TryGetError(string jsonContent)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<JsonResultError>(jsonContent);
                return (model.ErrorCode == 0 && (string.IsNullOrWhiteSpace(model.ErrorMessage) || model.ErrorMessage == "no error" || model.ErrorMessage == "ok" || model.ErrorMessage == "send job submission success")) ? null : model;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsError(string jsonContent)
        {
            var first = jsonContent.StartsWith("{\"errcode\"");
            return first || jsonContent.Replace("\n", "").Replace(" ", "").StartsWith("{\"errcode\"");
        }
    }
}