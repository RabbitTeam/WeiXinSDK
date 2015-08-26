using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Rabbit.WeiXin.Utility;

namespace Rabbit.WeiXin.MP.Api.TemplateMessage
{
    /// <summary>
    /// 一个抽象的模板消息服务。
    /// </summary>
    public interface ITemplateMessageService
    {
        /// <summary>
        /// 设置行业。
        /// </summary>
        /// <param name="mainIndustryId">主行业Id。</param>
        /// <param name="deputyIndustryid">副行业Id。</param>
        /// <remarks>行业Id参考：http://mp.weixin.qq.com/wiki/17/304c1885ea66dbedf7dc170d84999a9d.html#toc。</remarks>
        void SetIndustry(uint mainIndustryId, uint deputyIndustryid);

        /// <summary>
        /// 获取发送模板的Id。
        /// </summary>
        /// <param name="templateShortId">模板短Id。</param>
        /// <returns>发送模板的Id。</returns>
        string GetTemplateId(string templateShortId);

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

        /// <summary>
        /// 初始化一个新的模板消息服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public TemplateMessageService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ITemplateMessageService

        /// <summary>
        /// 设置行业。
        /// </summary>
        /// <param name="mainIndustryId">主行业Id。</param>
        /// <param name="deputyIndustryid">副行业Id。</param>
        /// <remarks>行业Id参考：http://mp.weixin.qq.com/wiki/17/304c1885ea66dbedf7dc170d84999a9d.html#toc。</remarks>
        public void SetIndustry(uint mainIndustryId, uint deputyIndustryid)
        {
            var postUrl = "https://api.weixin.qq.com/cgi-bin/template/api_set_industry?access_token=" + _accountModel.GetAccessToken();
            WeiXinHttpHelper.Post(postUrl, new { industry_id1 = mainIndustryId, industry_id2 = deputyIndustryid });
        }

        /// <summary>
        /// 获取发送模板的Id。
        /// </summary>
        /// <param name="templateShortId">模板短Id。</param>
        /// <returns>发送模板的Id。</returns>
        public string GetTemplateId(string templateShortId)
        {
            var postUrl = "https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token=" + _accountModel.GetAccessToken();
            var content = WeiXinHttpHelper.PostString(postUrl, new { template_id_short = templateShortId });
            return JObject.Parse(content).Value<string>("template_id");
        }

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
        /// <summary>
        /// 初始化一个新的模板消息字段数据项。
        /// </summary>
        /// <param name="value">字段值。</param>
        public TemplateMessageFieldDataItem(string value)
            : this(value, "#000000")
        {
        }

        /// <summary>
        /// 初始化一个新的模板消息字段数据项。
        /// </summary>
        /// <param name="color">字段颜色。</param>
        /// <param name="value">字段值。</param>
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