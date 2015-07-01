using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System;
using System.Globalization;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api.User
{
    /// <summary>
    /// 一个抽象的用户服务接口。
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 设置用户备注。
        /// </summary>
        /// <param name="openId">用户标识</param>
        /// <param name="remark">新的备注名，长度必须小于30字符</param>
        void SetRemark(string openId, string remark);

        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="openId">普通用户的标识，对当前公众号唯一</param>
        /// <param name="language">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <returns>用户信息。</returns>
        UserInfo GetUser(string openId, string language = null);

        /// <summary>
        /// 获取用户列表。
        /// </summary>
        /// <param name="startOpenId">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns>获取用户列表结果模型。</returns>
        GetUserListResultModel GetUserList(string startOpenId = null);
    }

    /// <summary>
    /// 用户服务接口的实现。
    /// </summary>
    public class UserService : IUserService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public UserService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IUserService

        /// <summary>
        /// 设置用户备注。
        /// </summary>
        /// <param name="openId">用户标识</param>
        /// <param name="remark">新的备注名，长度必须小于30字符</param>
        public void SetRemark(string openId, string remark)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { openid = openId, remark });
        }

        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="openId">普通用户的标识，对当前公众号唯一</param>
        /// <param name="language">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <returns>用户信息。</returns>
        public UserInfo GetUser(string openId, string language = null)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}",
                _accountModel.GetAccessToken(), openId);
            if (!string.IsNullOrWhiteSpace(language))
                url = url + "&lang=" + language;

            return WeiXinHttpHelper.GetResultByJson<UserInfo>(url);
        }

        /// <summary>
        /// 获取用户列表。
        /// </summary>
        /// <param name="startOpenId">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns>获取用户列表结果模型。</returns>
        public GetUserListResultModel GetUserList(string startOpenId = null)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + _accountModel.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(startOpenId))
                url = url + "&next_openid=" + startOpenId;

            var content = WeiXinHttpHelper.GetString(url);
            var obj = JObject.Parse(content);

            return GetUserListResultModel.Create(obj);
        }

        #endregion Implementation of IUserService
    }

    #region Help Class

    /// <summary>
    /// 性别枚举。
    /// </summary>
    public enum SexEnum
    {
        /// <summary>
        /// 未知。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 男性。
        /// </summary>
        Male = 1,

        /// <summary>
        /// 女性。
        /// </summary>
        Female = 2
    }

    /// <summary>
    /// 用户信息。
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        [JsonProperty("subscribe")]
        public bool IsSubscribe { get; set; }

        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 用户的昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        [JsonProperty("sex")]
        internal ushort SexNumber { get; set; }

        /// <summary>
        /// 用户的性别。
        /// </summary>
        public SexEnum Sex
        {
            get
            {
                SexEnum value;
                Enum.TryParse(SexNumber.ToString(CultureInfo.InvariantCulture), out value);
                return value;
            }
        }

        /// <summary>
        /// 用户的语言，简体中文为zh_CN
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 用户所在城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 用户所在省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 用户所在国家
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        [JsonProperty("headimgurl")]
        public string HeadPictureUrl { get; set; }

        /// <summary>
        /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        /// </summary>
        [JsonProperty("subscribe_time")]
        public string SubscribeTimestamp { get; set; }

        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 用户所在的分组ID
        /// </summary>
        public ulong GroupId { get; set; }
    }

    /// <summary>
    /// 获取用户列表结果模型。
    /// </summary>
    public sealed class GetUserListResultModel
    {
        /// <summary>
        /// 关注该公众账号的总用户数
        /// </summary>
        public ulong TotalCount { get; set; }

        /// <summary>
        /// 拉取的OPENID个数，最大值为10000
        /// </summary>
        public ushort Count { get; set; }

        /// <summary>
        /// 列表数据，OPENID的列表
        /// </summary>
        public string[] OpenIds { get; set; }

        /// <summary>
        /// 拉取列表的后一个用户的OPENID
        /// </summary>
        public string LastOpenId { get; set; }

        internal static GetUserListResultModel Create(JObject obj)
        {
            var array = (obj["data"]["openid"] as JArray);
            return new GetUserListResultModel
            {
                TotalCount = obj.Value<ulong>("total"),
                Count = obj.Value<ushort>("count"),
                LastOpenId = obj.Value<string>("next_openid"),
                OpenIds = array == null ? new string[0] : array.Select(i => i.Value<string>()).ToArray(),
            };
        }
    }

    #endregion Help Class
}