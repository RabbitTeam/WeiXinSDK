using Newtonsoft.Json;
using Rabbit.WeiXin.MP.Api.User;
using Rabbit.WeiXin.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Globalization;

namespace Rabbit.WeiXin.MP.Api.OAuth
{
    /// <summary>
    /// 一个抽象的OAuth服务。
    /// </summary>
    public interface IOAuthService
    {
        /// <summary>
        /// 获取OAuth授权的Url地址。
        /// </summary>
        /// <param name="url">需要跳转的目标地址。</param>
        /// <param name="scope">OAuth域。</param>
        /// <param name="state">状态参数。</param>
        /// <returns>绝对的Url地址。</returns>
        string GetAuthorizeUrl(string url, OAuthScope scope, string state = null);

        /// <summary>
        /// 获取访问票据。
        /// </summary>
        /// <param name="code">公众平台Code参数。</param>
        /// <returns>结果模型。</returns>
        GetOAuthAccessTokenResult GetAccessToken(string code);

        /// <summary>
        /// 刷新访问票据。
        /// </summary>
        /// <param name="refreshToken">刷新票据。</param>
        /// <returns>结果模型。</returns>
        GetOAuthAccessTokenResult RefreshToken(string refreshToken);

        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="oAuthAccessToken">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同。</param>
        /// <param name="openId">用户的唯一标识。</param>
        /// <param name="language">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语。</param>
        /// <returns>用户信息。</returns>
        OAuthUserInfo GetUserInfo(GetOAuthAccessTokenResult oAuthAccessToken, string openId, string language = null);
    }

    /// <summary>
    /// OAuth服务的实现。
    /// </summary>
    public sealed class OAuthService : IOAuthService
    {
        #region Field

        private readonly AccountModel _accountModel;
        private readonly Open.Api.AccountModel _openAccountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的授权服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public OAuthService(AccountModel accountModel)
            : this(accountModel, null)
        {
        }

        /// <summary>
        /// 初始化一个新的授权服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        /// <param name="openAccountModel">开放平台账号模型。</param>
        public OAuthService(AccountModel accountModel, Open.Api.AccountModel openAccountModel)
        {
            _accountModel = accountModel;
            _openAccountModel = openAccountModel;
        }

        #endregion Constructor

        #region Implementation of IOAuthService

        /// <summary>
        /// 获取OAuth授权的Url地址。
        /// </summary>
        /// <param name="url">需要跳转的目标地址。</param>
        /// <param name="scope">OAuth域。</param>
        /// <param name="state">状态参数。</param>
        /// <returns>绝对的Url地址。</returns>
        public string GetAuthorizeUrl(string url, OAuthScope scope, string state = null)
        {
            var appId = _accountModel.AppId;
            var redirectUrl =
                string.Format(
                    "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}",
                    appId, url, "snsapi_" + scope.ToString().ToLower(), state);

            if (_openAccountModel != null)
            {
                redirectUrl = redirectUrl + "&component_appid=" + _openAccountModel.AppId;
            }
            return redirectUrl + "#wechat_redirect";
        }

        /// <summary>
        /// 获取访问票据。
        /// </summary>
        /// <param name="code">公众平台Code参数。</param>
        /// <returns>结果模型。</returns>
        public GetOAuthAccessTokenResult GetAccessToken(string code)
        {
            var url = _openAccountModel != null ? string.Format("https://api.weixin.qq.com/sns/oauth2/component/access_token?appid={0}&code={1}&grant_type=authorization_code&component_appid={2}&component_access_token={3}", _accountModel.AppId, code, _openAccountModel.AppId, _openAccountModel.GetAccessToken()) : string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", _accountModel.AppId, _accountModel.AppSecret, code);

            return GetOAuthAccessTokenResult.CreateResult(WeiXinHttpHelper.GetString(url));
        }

        /// <summary>
        /// 刷新访问票据。
        /// </summary>
        /// <param name="refreshToken">刷新票据。</param>
        /// <returns>结果模型。</returns>
        public GetOAuthAccessTokenResult RefreshToken(string refreshToken)
        {
            var url = _openAccountModel == null ? string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}", _accountModel.AppId, refreshToken) : string.Format("https://api.weixin.qq.com/sns/oauth2/component/refresh_token?appid={0}&grant_type=refresh_token&component_appid={1}&component_access_token={2}&refresh_token={3}", _accountModel.AppId, _openAccountModel.AppId, _openAccountModel.GetAccessToken(), refreshToken);

            return GetOAuthAccessTokenResult.CreateResult(WeiXinHttpHelper.GetString(url));
        }

        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="oAuthAccessToken">网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同。</param>
        /// <param name="openId">用户的唯一标识。</param>
        /// <param name="language">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语。</param>
        /// <returns>用户信息。</returns>
        public OAuthUserInfo GetUserInfo(GetOAuthAccessTokenResult oAuthAccessToken, string openId, string language)
        {
            oAuthAccessToken.NotNull("accessToken");

            //票据过期则刷新票据。
            if (oAuthAccessToken.IsExpires())
                oAuthAccessToken = RefreshToken(oAuthAccessToken.RefreshToken);

            var url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang={2}",
                oAuthAccessToken.AccessToken, openId, language);

            return WeiXinHttpHelper.GetResultByJson<OAuthUserInfo>(url);
        }

        #endregion Implementation of IOAuthService
    }

    #region Help Class

    /// <summary>
    /// 公众平台OAuth服务扩展方法。
    /// </summary>
    public static class PublicPlatformOAuthServiceExtensions
    {
        /// <summary>
        /// 获取用户信息。
        /// </summary>
        /// <param name="authService">OAuth服务。</param>
        /// <param name="code">公众平台Code参数。</param>
        /// <returns>用户信息。</returns>
        public static OAuthUserInfo GetUserInfo(this IOAuthService authService, string code)
        {
            code.NotEmptyOrWhiteSpace("code");
            var result = authService.NotNull("authService").GetAccessToken(code);
            if (result == null)
                throw new ArgumentException("根据 Code 获取访问票据失败。", "code");
            return authService.GetUserInfo(result, result.OpenId);
        }
    }

    /// <summary>
    /// OAuth域。
    /// </summary>
    /// <remarks>
    /// 1、以snsapi_base为scope发起的网页授权，是用来获取进入页面的用户的openid的，并且是静默授权并自动跳转到回调页的。用户感知的就是直接进入了回调页（往往是业务页面）
    /// 2、以snsapi_userinfo为scope发起的网页授权，是用来获取用户的基本信息的。但这种授权需要用户手动同意，并且由于用户同意过，所以无须关注，就可在授权后获取该用户的基本信息。
    /// 3、用户管理类接口中的“获取用户基本信息接口”，是在用户和公众号产生消息交互或关注后事件推送后，才能根据用户OpenID来获取用户基本信息。这个接口，包括其他微信接口，都是需要该用户（即openid）关注了公众号后，才能调用成功的。
    /// </remarks>
    public enum OAuthScope
    {
        /// <summary>
        /// 基本。
        /// </summary>
        Base = 0,

        /// <summary>
        /// 用户信息。
        /// </summary>
        UserInfo = 1
    }

    /// <summary>
    /// 获取网页授权访问票据的结果。
    /// </summary>
    public sealed class GetOAuthAccessTokenResult
    {
        #region Field

        private DateTime _createTime;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的实例。
        /// </summary>
        public GetOAuthAccessTokenResult()
        {
            _createTime = DateTime.Now;
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 访问票据。
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 过期时间（单位：秒）。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 刷新票据。
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// OpenId。
        /// </summary>
        [JsonProperty("openid")]
        public string OpenId { get; set; }

        /// <summary>
        /// 域。
        /// </summary>
        [JsonProperty("scope")]
        public string ScopeString { get; set; }

        /// <summary>
        /// 域。
        /// </summary>
        [JsonIgnore]
        public OAuthScope Scope
        {
            get
            {
                switch (ScopeString)
                {
                    case "snsapi_base":
                        return OAuthScope.Base;

                    case "snsapi_userinfo":
                        return OAuthScope.UserInfo;
                }
                return default(OAuthScope);
            }
        }

        #endregion Property

        #region Public Method

        /// <summary>
        /// 票据是否过期。
        /// </summary>
        /// <returns>如果过期返回true，否则返回false。</returns>
        public bool IsExpires()
        {
            return _createTime.AddSeconds(ExpiresIn) <= DateTime.Now;
        }

        #endregion Public Method

        #region Internal Method

        /// <summary>
        /// 根据Json内容创建模型。
        /// </summary>
        /// <param name="jsonContent">Json内容。</param>
        /// <returns>结果。</returns>
        internal static GetOAuthAccessTokenResult CreateResult(string jsonContent)
        {
            if (string.IsNullOrWhiteSpace(jsonContent) || jsonContent.Contains("errcode"))
                return null;
            return JsonConvert.DeserializeObject<GetOAuthAccessTokenResult>(jsonContent);
        }

        #endregion Internal Method
    }

    /// <summary>
    /// 通过网页授权得到的用户信息。
    /// </summary>
    public sealed class OAuthUserInfo
    {
        /// <summary>
        /// OpenId。
        /// </summary>
        [JsonProperty("openid")]
        public string OpenId { get; set; }

        /// <summary>
        /// 昵称。
        /// </summary>
        [JsonProperty("nickname")]
        public string NickName { get; set; }

        /// <summary>
        /// 性别（0：未知，1：男，2：女）
        /// </summary>
        [JsonProperty("sex")]
        internal int SexNumber { get; set; }

        /// <summary>
        /// 省份。
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市。
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// 国家。
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        [JsonProperty("headimgurl")]
        public string HeadPictureUrl { get; set; }

        /// <summary>
        /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）。
        /// </summary>
        [JsonProperty("privilege")]
        public string[] Privilege { get; set; }

        /// <summary>
        /// UnionId。
        /// </summary>
        /// <remarks>
        /// 1、请注意，网页授权获取用户基本信息也遵循UnionID机制。即如果开发者有在多个公众号，或在公众号、移动应用之间统一用户帐号的需求，需要前往微信开放平台（open.weixin.qq.com）绑定公众号后，才可利用UnionID机制来满足上述需求。
        /// 2、UnionID机制的作用说明：如果开发者拥有多个移动应用、网站应用和公众帐号，可通过获取用户基本信息中的unionid来区分用户的唯一性，因为同一用户，对同一个微信开放平台下的不同应用（移动应用、网站应用和公众帐号），unionid是相同的。
        /// </remarks>
        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        /// <summary>
        /// 性别。
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
    }

    #endregion Help Class
}
