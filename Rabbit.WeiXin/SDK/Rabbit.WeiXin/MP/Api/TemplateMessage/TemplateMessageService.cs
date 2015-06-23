using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Rabbit.WeiXin.MP.Api.Utility;

namespace Rabbit.WeiXin.MP.Api.TemplateMessage
{
    /// <summary>
    /// 一个抽象的模板消息服务。
    /// </summary>
    public interface ITemplateMessageService
    {
        /// <summary>
        /// 发送模板消息。
        /// </summary>
        /// <param name="openId">用户Id。</param>
        /// <param name="templateId">模板Id。</param>
        /// <param name="url">跳转的url。</param>
        /// <param name="topColor">顶部颜色。</param>
        /// <param name="data">模板数据。</param>
        /// <returns>消息Id。</returns>
        ulong Send(string openId, string templateId, string url, string topColor, object data);
    }

    /// <summary>
    /// 模板消息服务实现。
    /// </summary>
    public sealed class TemplateMessageService : ITemplateMessageService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public TemplateMessageService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ITemplateMessageService

        /// <summary>
        /// 发送模板消息。
        /// </summary>
        /// <param name="openId">用户Id。</param>
        /// <param name="templateId">模板Id。</param>
        /// <param name="url">跳转的url。</param>
        /// <param name="topColor">顶部颜色。</param>
        /// <param name="data">模板数据。</param>
        /// <returns>消息Id。</returns>
        public ulong Send(string openId, string templateId, string url, string topColor, object data)
        {
            var postUrl = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + _accountModel.GetAccessToken();
            var json = WeiXinHttpHelper.PostString(postUrl, new
            {
                touser = openId,
                template_id = templateId,
                url,
                topcolor = topColor,
                data
            });
            return JObject.Parse(json).Value<ulong>("msgid");
        }

        #endregion Implementation of ITemplateMessageService
    }

    #region Help Class

    /// <summary>
    /// 模板消息字段数据项。
    /// </summary>
    public sealed class TemplateMessageFieldDataItem
    {
        public TemplateMessageFieldDataItem(string value)
            : this(value, "#000000")
        {
        }

        public TemplateMessageFieldDataItem(string value, string color)
        {
            Value = value;
            Color = color;
        }

        /// <summary>
        /// 值。
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// 颜色。
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }
    }

    #endregion Help Class
}