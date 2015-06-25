using Newtonsoft.Json;
using Rabbit.WeiXin.MP.Api.Utility;
using System;

namespace Rabbit.WeiXin.Open.Api
{
    /// <summary>
    /// 一个抽象的通用服务。
    /// </summary>
    public interface ICommonService
    {
        /// <summary>
        /// 获取授权Url。
        /// </summary>
        /// <param name="returnUrl">授权成功返回的Url。</param>
        /// <returns>授权的Url地址。</returns>
        string GetAuthorizeUrl(string returnUrl);

        /// <summary>
        /// 获取第三方平台的全局唯一票据。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>第三方用户唯一凭证密钥，即appsecret。</returns>
        AccessTokenModel GetAccessToken(bool ignoreCached = false);

        /// <summary>
        /// 获取授权码。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>授权码结果。</returns>
        AuthorizeCodeResult GetAuthorizeCode(bool ignoreCached = false);

        /// <summary>
        /// 获取公众账号授权信息。
        /// </summary>
        /// <param name="authorizationCode">授权码。</param>
        /// <returns>公众账号授权信息。</returns>
        PublicAccountAuthorizerInfo GetPublicAccountAuthorizerInfo(string authorizationCode);

        /// <summary>
        /// 获取公众账号信息。
        /// </summary>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>公众账号信息。</returns>
        PublicAccountInfo GetPublicAccountInfo(string authorizerAppId);

        /// <summary>
        /// 获取选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>选项值。</returns>
        string GetOptionValue(string optionName, string authorizerAppId);

        /// <summary>
        /// 设置选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="optionValue">选项值。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        void SetOptionValue(string optionName, string optionValue, string authorizerAppId);
    }

    /// <summary>
    /// 通用服务实现。
    /// </summary>
    public sealed class CommonService : ICommonService
    {
        #region Field

        private readonly AccountModel _accountModel;
        private AccessTokenModel _accessTokenModel;

        #endregion Field

        #region Constructor

        public CommonService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICommonService

        /// <summary>
        /// 获取授权Url。
        /// </summary>
        /// <param name="returnUrl">授权成功返回的Url。</param>
        /// <returns>授权的Url地址。</returns>
        public string GetAuthorizeUrl(string returnUrl)
        {
            return string.Format(
                "https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid=xxxx&pre_auth_code={0}&redirect_uri={1}",
                GetAuthorizeCode(), returnUrl);
        }

        /// <summary>
        /// 获取第三方平台的全局唯一票据。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>第三方用户唯一凭证密钥，即appsecret。</returns>
        public AccessTokenModel GetAccessToken(bool ignoreCached = false)
        {
            if (_accessTokenModel == null)
                return _accessTokenModel =
                    WeiXinHttpHelper.PostResultByJson<AccessTokenModel>(
                        "https://api.weixin.qq.com/cgi-bin/component/api_component_token",
                        new
                        {
                            component_appid = _accountModel.AppId,
                            component_appsecret = _accountModel.AppSecret,
                            component_verify_ticket = _accountModel.GetVerifyTicket()
                        });

            if (!_accessTokenModel.IsExpired())
                return _accessTokenModel;

            return _accessTokenModel = RefreshToken(_accessTokenModel.AccessToken);
        }

        /// <summary>
        /// 获取授权码。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>授权码结果。</returns>
        public AuthorizeCodeResult GetAuthorizeCode(bool ignoreCached = false)
        {
            return WeiXinHttpHelper.PostResultByJson<AuthorizeCodeResult>(
                "https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token=" +
                GetAccessToken(), new { component_appid = _accountModel.AppId });
        }

        /// <summary>
        /// 获取公众账号授权信息。
        /// </summary>
        /// <param name="authorizationCode">授权码。</param>
        /// <returns>公众账号授权信息。</returns>
        public PublicAccountAuthorizerInfo GetPublicAccountAuthorizerInfo(string authorizationCode)
        {
            var content = WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token=" + GetAccessToken(),
                new
                {
                    component_appid = _accountModel.AppId,
                    authorization_code = authorizationCode
                });
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取公众账号信息。
        /// </summary>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>公众账号信息。</returns>
        public PublicAccountInfo GetPublicAccountInfo(string authorizerAppId)
        {
            var content = WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token=" +
                GetAccessToken(), new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId
                });
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>选项值。</returns>
        public string GetOptionValue(string optionName, string authorizerAppId)
        {
            var content = WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/ api_get_authorizer_option?component_access_token=" +
                GetAccessToken(), new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId,
                    option_name = optionName
                });

            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="optionValue">选项值。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        public void SetOptionValue(string optionName, string optionValue, string authorizerAppId)
        {
            var content = WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/ api_set_authorizer_option?component_access_token=" +
                GetAccessToken(), new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId,
                    option_name = optionName,
                    option_value = optionValue
                });
            throw new NotImplementedException();
        }

        #endregion Implementation of ICommonService

        #region Private Method

        private AccessTokenModel RefreshToken(string accessToken)
        {
            return
                WeiXinHttpHelper.PostResultByJson<AccessTokenModel>(
                    "https:// api.weixin.qq.com /cgi-bin/component/api_authorizer_token?component_access_token=" + accessToken,
                    new
                    {
                        component_appid = _accountModel.AppId,
                        component_appsecret = _accountModel.AppSecret,
                        component_verify_ticket = _accountModel.GetVerifyTicket()
                    });
        }

        #endregion Private Method
    }

    #region Help Class

    /// <summary>
    /// 获取第三方平台的全局唯一票据结果模型。
    /// </summary>
    public sealed class AccessTokenModel
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的第三方平台的全局唯一票据。
        /// </summary>
        internal AccessTokenModel()
        {
            CreateTime = DateTime.Now;
        }

        #endregion Constructor

        /// <summary>
        /// 第三方平台access_token。
        /// </summary>
        [JsonProperty("component_access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 凭证有效时间（秒）。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 是否过期。
        /// </summary>
        /// <returns>如果过期返回true，否则返回false。</returns>
        public bool IsExpired()
        {
            return CreateTime.AddSeconds(ExpiresIn - 20/*不采用最后的期限作为判断，防止在很少的时间内到期导致后续的逻辑无法执行*/) <= DateTime.Now;
        }
    }

    /// <summary>
    /// 获取授权码结果。
    /// </summary>
    public sealed class AuthorizeCodeResult
    {
        #region Constructor

        internal AuthorizeCodeResult()
        {
            CreateTime = DateTime.Now;
        }

        #endregion Constructor

        /// <summary>
        /// 授权码。
        /// </summary>
        [JsonProperty("pre_auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 凭证有效时间（秒）。
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { get; private set; }

        /// <summary>
        /// 是否过期。
        /// </summary>
        /// <returns>如果过期返回true，否则返回false。</returns>
        public bool IsExpired()
        {
            return CreateTime.AddSeconds(ExpiresIn - 20/*不采用最后的期限作为判断，防止在很少的时间内到期导致后续的逻辑无法执行*/) <= DateTime.Now;
        }
    }

    /// <summary>
    /// 授权信息。
    /// </summary>
    public sealed class AuthorizationInfo
    {
        /// <summary>
        /// 授权方appid
        /// </summary>
        [JsonProperty("authorizer_appid")]
        public string AppId { get; set; }

        /// <summary>
        /// 授权方令牌（在授权的公众号具备API权限时，才有此返回值）
        /// </summary>
        [JsonProperty("authorizer_access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 有效期（在授权的公众号具备API权限时，才有此返回值）
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 刷新令牌（在授权的公众号具备API权限时，才有此返回值），刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌
        /// </summary>
        [JsonProperty("authorizer_refresh_token")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// 权限集枚举。
    /// </summary>
    public enum RightEnum
    {
        /// <summary>
        /// 消息与菜单权限集。
        /// </summary>
        MessageAndMenu = 1,

        /// <summary>
        /// 用户管理权限集。
        /// </summary>
        UserManager = 2,

        /// <summary>
        /// 帐号管理权限集。
        /// </summary>
        AccountManager = 3,

        /// <summary>
        /// 网页授权权限集。
        /// </summary>
        WebPageAuthorizer = 4,

        /// <summary>
        /// 微信小店权限集。
        /// </summary>
        WeiXinShop = 5,

        /// <summary>
        /// 多客服权限集。
        /// </summary>
        TransferCustomerService = 6,

        /// <summary>
        /// 业务通知权限集。
        /// </summary>
        BusinessNotification = 7,

        /// <summary>
        /// 微信卡券权限集。
        /// </summary>
        WeiXinCard
    }

    /// <summary>
    /// 公众号授权信息。
    /// </summary>
    public sealed class PublicAccountAuthorizerInfo
    {
        /// <summary>
        /// 授权信息。
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// 权限集。
        /// </summary>
        public RightEnum[] Rights { get; set; }
    }

    /// <summary>
    /// 公众账号信息。
    /// </summary>
    public sealed class PublicAccountInfo
    {
        /// <summary>
        /// 公众号类型。
        /// </summary>
        public enum ServiceTypeEnum
        {
            /// <summary>
            /// 订阅号。
            /// </summary>
            Subscription = 0,

            /// <summary>
            /// 由历史老帐号升级后的订阅号。
            /// </summary>
            OutSubscription = 1,

            /// <summary>
            /// 服务号。
            /// </summary>
            Service = 2
        }

        /// <summary>
        /// 认证类型。
        /// </summary>
        public enum AuthenticateTypeEnum
        {
            /// <summary>
            /// 未认证。
            /// </summary>
            None = -1,

            /// <summary>
            /// 微信认证。
            /// </summary>
            WeiXin = 0,

            /// <summary>
            /// 新浪微博。
            /// </summary>
            SinaWeibo = 1,

            /// <summary>
            /// 腾讯微博。
            /// </summary>
            TencentWeibo = 2,

            /// <summary>
            /// 其它。
            /// </summary>
            Other = 99
        }

        /// <summary>
        /// 授权信息。
        /// </summary>
        public sealed class AuthorizationInfo
        {
            /// <summary>
            /// 授权方appid
            /// </summary>
            public string AppId { get; set; }

            /// <summary>
            /// 权限集。
            /// </summary>
            public RightEnum[] Rights { get; set; }
        }

        /// <summary>
        /// 授权者信息。
        /// </summary>
        public sealed class AuthorizerInfo
        {
            [JsonProperty("nick_name")]
            public string NickName { get; set; }

            [JsonProperty("service_type_info")]
            public ServiceTypeEnum ServiceType { get; set; }

            [JsonProperty("verify_type_info")]
            public AuthenticateTypeEnum AuthenticateType { get; set; }

            [JsonProperty("head_img")]
            public string HeadPicture { get; set; }

            [JsonProperty("user_name")]
            public string UserName { get; set; }

            [JsonProperty("alias")]
            public string Alias { get; set; }
        }

        /// <summary>
        /// 授权信息。
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// 授权者信心。
        /// </summary>
        public AuthorizerInfo Authorizer { get; set; }

        [JsonProperty("qrcode_url")]
        public string QrCode { get; set; }
    }

    #endregion Help Class
}