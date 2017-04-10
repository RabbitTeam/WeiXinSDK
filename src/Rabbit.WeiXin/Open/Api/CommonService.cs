using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System;
using System.Collections.Concurrent;
using System.Linq;

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
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>公众账号授权信息。</returns>
        PublicAccountAuthorizerInfo GetPublicAccountAuthorizerInfo(string authorizationCode, bool ignoreCached = false);

        /// <summary>
        /// 获取公众账号信息。
        /// </summary>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>公众账号信息。</returns>
        PublicAccountInfo GetPublicAccountInfo(string authorizerAppId);

        /// <summary>
        /// 获取选项值。
        /// </summary>
        /// <param name="optionName">
        /// 选项名称。
        /// location_report(地理位置上报选项)（0	无上报 1	进入会话时上报2	每5s上报）
        /// voice_recognize（语音识别开关选项）（0	关闭语音识别 1	开启语音识别）
        /// customer_service（客服开关选项）（0	关闭多客服1	开启多客服）
        /// </param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>选项值。</returns>
        /// <remarks>
        /// location_report(地理位置上报选项)（0	无上报 1	进入会话时上报2	每5s上报）
        /// voice_recognize（语音识别开关选项）（0	关闭语音识别 1	开启语音识别）
        /// customer_service（客服开关选项）（0	关闭多客服1	开启多客服）
        /// </remarks>
        string GetOptionValue(string optionName, string authorizerAppId);

        /// <summary>
        /// 设置选项值。
        /// </summary>
        /// <param name="optionName">
        /// 选项名称。
        /// location_report(地理位置上报选项)（0	无上报 1	进入会话时上报2	每5s上报）
        /// voice_recognize（语音识别开关选项）（0	关闭语音识别 1	开启语音识别）
        /// customer_service（客服开关选项）（0	关闭多客服1	开启多客服）
        /// </param>
        /// <param name="optionValue">选项值。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        void SetOptionValue(string optionName, string optionValue, string authorizerAppId);

        /// <summary>
        /// 刷新授权公众号令牌。
        /// </summary>
        /// <param name="authorizerAppId">授权方appid。</param>
        /// <param name="authorizerRefreshToken">授权方的刷新令牌，刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌。</param>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>刷新令牌模型。</returns>
        RefreshAccessToken RefreshToken(string authorizerAppId, string authorizerRefreshToken, bool ignoreCached = false);
    }

    /// <summary>
    /// 通用服务实现。
    /// </summary>
    public sealed class CommonService : ICommonService
    {
        #region Field

        private readonly AccountModel _accountModel;
        private AccessTokenModel _accessTokenModel;
        private AuthorizeCodeResult _authorizeCodeResult;
        private readonly ConcurrentDictionary<string, object> _objectCacheDictionary = new ConcurrentDictionary<string, object>();

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的通用服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
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
                "https://mp.weixin.qq.com/cgi-bin/componentloginpage?component_appid={0}&pre_auth_code={1}&redirect_uri={2}",
                _accountModel.AppId, GetAuthorizeCode(true).AuthCode, returnUrl);
        }

        /// <summary>
        /// 获取第三方平台的全局唯一票据。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>第三方用户唯一凭证密钥，即appsecret。</returns>
        public AccessTokenModel GetAccessToken(bool ignoreCached = false)
        {
            Func<AccessTokenModel> get = () => WeiXinHttpHelper.PostResultByJson<AccessTokenModel>(
                "https://api.weixin.qq.com/cgi-bin/component/api_component_token",
                new
                {
                    component_appid = _accountModel.AppId,
                    component_appsecret = _accountModel.AppSecret,
                    component_verify_ticket = _accountModel.GetVerifyTicket()
                });

            //是否需要重新获取（无效、忽略缓存、过期）
            Func<bool> needGet = () => _accessTokenModel == null || ignoreCached || _accessTokenModel.IsExpired();

            if (needGet())
            {
                lock (this)
                {
                    if (needGet())
                    {
                        /*                        var newModel = get();
                                                if (_accessTokenModel != null && _accessTokenModel.AccessToken == newModel.AccessToken)
                                                    return _accessTokenModel;
                                                return _accessTokenModel = newModel;*/
                        return _accessTokenModel = get();
                    }
                }
            }

            return _accessTokenModel;
        }

        /// <summary>
        /// 获取授权码。
        /// </summary>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>授权码结果。</returns>
        public AuthorizeCodeResult GetAuthorizeCode(bool ignoreCached = false)
        {
            Func<string, AuthorizeCodeResult> get = accessToken => WeiXinHttpHelper.PostResultByJson<AuthorizeCodeResult>(
                 "https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token=" +
                 accessToken, new { component_appid = _accountModel.AppId });

            //是否需要重新获取（无效、忽略缓存、过期）
            Func<bool> needGet = () => _authorizeCodeResult == null || ignoreCached || _authorizeCodeResult.IsExpired();

            if (needGet())
            {
                lock (this)
                {
                    if (needGet())
                    {
                        AuthorizeCodeResult newModel;
                        try
                        {
                            newModel = get(GetAccessToken().AccessToken);
                        }
                        catch (WeiXinException exception)
                        {
                            if (exception.ErrorCode == 40001 ||
                                exception.Message == "invalid credential, access_token is invalid or not latest")
                            {
                                newModel = get(GetAccessToken(true).AccessToken);
                            }
                            else
                            {
                                throw;
                            }
                        }
                        /*if (_authorizeCodeResult != null && _authorizeCodeResult.AuthCode == newModel.AuthCode)
                            return _authorizeCodeResult;*/
                        return _authorizeCodeResult = newModel;
                    }
                }
            }

            return _authorizeCodeResult;
        }

        /// <summary>
        /// 获取公众账号授权信息。
        /// </summary>
        /// <param name="authorizationCode">授权码。</param>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>公众账号授权信息。</returns>
        public PublicAccountAuthorizerInfo GetPublicAccountAuthorizerInfo(string authorizationCode, bool ignoreCached = false)
        {
            var key = "GetPublicAccountAuthorizerInfo_" + authorizationCode;

            lock (this)
            {
                object value;
                var newValue = new Lazy<PublicAccountAuthorizerInfo>(() => InternalGetPublicAccountAuthorizerInfo(authorizationCode));

                if (_objectCacheDictionary.TryGetValue(key, out value))
                {
                    var model = (PublicAccountAuthorizerInfo)value;
                    //忽略缓存、过期，则重新获取。
                    if (ignoreCached || model.IsExpired())
                    {
                        _objectCacheDictionary.TryUpdate(key, newValue.Value, value);
                        return newValue.Value;
                    }
                    //缓存有效则直接返回。
                    return model;
                }

                //添加新缓存。
                _objectCacheDictionary.TryAdd(key, newValue.Value);
                return newValue.Value;
            }
        }

        /// <summary>
        /// 获取公众账号信息。
        /// </summary>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>公众账号信息。</returns>
        public PublicAccountInfo GetPublicAccountInfo(string authorizerAppId)
        {
            Func<string, string> getContent = accessToken => WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token=" +
                accessToken, new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId
                });

            string content;

            try
            {
                content = getContent(GetAccessToken().AccessToken);
            }
            catch (WeiXinException exception)
            {
                if (exception.ErrorCode == 40001 ||
                    exception.Message == "invalid credential, access_token is invalid or not latest")
                {
                    content = getContent(GetAccessToken(true).AccessToken);
                }
                else
                {
                    throw;
                }
            }

            var obj = JObject.Parse(content);
            var authorizerInfo = obj["authorizer_info"];
            var authorizationInfo = obj["authorization_info"];
            var model = new PublicAccountInfo
            {
                Authorizer =
                    JsonConvert.DeserializeObject<PublicAccountInfo.AuthorizerInfo>(authorizerInfo.ToString()),
                Authorization =
                    JsonConvert.DeserializeObject<PublicAccountInfo.AuthorizationInfo>(authorizationInfo.ToString()),
            };

            model.Authorization.Rights = GetRights(authorizationInfo);
            model.Authorizer.ServiceType =
                (PublicAccountInfo.ServiceTypeEnum)
                    Enum.Parse(typeof(PublicAccountInfo.ServiceTypeEnum),
                        authorizerInfo.SelectToken("service_type_info.id").Value<string>());
            model.Authorizer.AuthenticateType =
                (PublicAccountInfo.AuthenticateTypeEnum)
                    Enum.Parse(typeof(PublicAccountInfo.AuthenticateTypeEnum),
                        authorizerInfo.SelectToken("verify_type_info.id").Value<string>());

            return model;
        }

        /// <summary>
        /// 获取选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <returns>选项值。</returns>
        /// <remarks>
        /// location_report(地理位置上报选项)（0	无上报 1	进入会话时上报2	每5s上报）
        /// voice_recognize（语音识别开关选项）（0	关闭语音识别 1	开启语音识别）
        /// customer_service（客服开关选项）（0	关闭多客服1	开启多客服）
        /// </remarks>
        public string GetOptionValue(string optionName, string authorizerAppId)
        {
            Func<string, string> get = accessToken => WeiXinHttpHelper.PostString(
                "https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_option?component_access_token=" +
                accessToken, new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId,
                    option_name = optionName
                });
            string content;

            try
            {
                content = get(GetAccessToken().AccessToken);
            }
            catch (WeiXinException exception)
            {
                if (exception.ErrorCode == 40001 ||
                    exception.Message == "invalid credential, access_token is invalid or not latest")
                {
                    content = get(GetAccessToken(true).AccessToken);
                }
                else
                {
                    throw;
                }
            }

            return JObject.Parse(content).Value<string>("option_value");
        }

        /// <summary>
        /// 设置选项值。
        /// </summary>
        /// <param name="optionName">选项名称。</param>
        /// <param name="optionValue">选项值。</param>
        /// <param name="authorizerAppId">授权者AppId。</param>
        /// <remarks>
        /// location_report(地理位置上报选项)（0	无上报 1	进入会话时上报2	每5s上报）
        /// voice_recognize（语音识别开关选项）（0	关闭语音识别 1	开启语音识别）
        /// customer_service（客服开关选项）（0	关闭多客服1	开启多客服）
        /// </remarks>
        public void SetOptionValue(string optionName, string optionValue, string authorizerAppId)
        {
            Action<string> set = accessToken =>
            {
                WeiXinHttpHelper.PostString(
                    "https://api.weixin.qq.com/cgi-bin/component/api_set_authorizer_option?component_access_token=" +
                    accessToken, new
                    {
                        component_appid = _accountModel.AppId,
                        authorizer_appid = authorizerAppId,
                        option_name = optionName,
                        option_value = optionValue
                    });
            };

            try
            {
                set(GetAccessToken().AccessToken);
            }
            catch (WeiXinException exception)
            {
                if (exception.ErrorCode == 40001 ||
                    exception.Message == "invalid credential, access_token is invalid or not latest")
                {
                    set(GetAccessToken(true).AccessToken);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 刷新授权公众号令牌。
        /// </summary>
        /// <param name="authorizerAppId">授权方appid。</param>
        /// <param name="authorizerRefreshToken">授权方的刷新令牌，刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌。</param>
        /// <param name="ignoreCached">是否忽略缓存。</param>
        /// <returns>刷新令牌模型。</returns>
        public RefreshAccessToken RefreshToken(string authorizerAppId, string authorizerRefreshToken, bool ignoreCached = false)
        {
            var key = "RefreshToken_" + authorizerAppId;

            lock (this)
            {
                object value;
                var newValue = new Lazy<RefreshAccessToken>(() => InternalRefreshToken(authorizerAppId, authorizerRefreshToken));

                if (_objectCacheDictionary.TryGetValue(key, out value))
                {
                    var model = (RefreshAccessToken)value;
                    //忽略缓存、过期，则重新获取。
                    if (ignoreCached || model.IsExpired())
                    {
                        _objectCacheDictionary.TryUpdate(key, newValue.Value, value);
                        return newValue.Value;
                    }
                    //缓存有效则直接返回。
                    return model;
                }

                //添加新缓存。
                _objectCacheDictionary.TryAdd(key, newValue.Value);
                return newValue.Value;
            }
        }

        #endregion Implementation of ICommonService

        #region Private Method

        private PublicAccountAuthorizerInfo InternalGetPublicAccountAuthorizerInfo(string authorizationCode)
        {
            var get = new Func<string, PublicAccountAuthorizerInfo>(accessToken =>
               {
                   var content = WeiXinHttpHelper.PostString(
                       "https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token=" +
                       accessToken,
                       new
                       {
                           component_appid = _accountModel.AppId,
                           authorization_code = authorizationCode
                       });
                   var obj = JObject.Parse(content)["authorization_info"];

                   var model = JsonConvert.DeserializeObject<PublicAccountAuthorizerInfo>(obj.ToString());
                   model.Rights = GetRights(obj);

                   return model;
               });

            try
            {
                return get(GetAccessToken().AccessToken);
            }
            catch (WeiXinException exception)
            {
                if (exception.ErrorCode == 40001 ||
                    exception.Message == "invalid credential, access_token is invalid or not latest")
                {
                    return get(GetAccessToken(true).AccessToken);
                }
                throw;
            }
        }

        private RefreshAccessToken InternalRefreshToken(string authorizerAppId, string authorizerRefreshToken)
        {
            Func<string, RefreshAccessToken> get = accessToken => WeiXinHttpHelper.PostResultByJson<RefreshAccessToken>(
                "https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token=" +
                accessToken,
                new
                {
                    component_appid = _accountModel.AppId,
                    authorizer_appid = authorizerAppId,
                    authorizer_refresh_token = authorizerRefreshToken
                });

            try
            {
                return get(GetAccessToken().AccessToken);
            }
            catch (WeiXinException exception)
            {
                if (exception.ErrorCode == 40001 ||
                    exception.Message == "invalid credential, access_token is invalid or not latest")
                {
                    return get(GetAccessToken(true).AccessToken);
                }
                throw;
            }
        }

        private static RightEnum[] GetRights(JToken obj)
        {
            return obj["func_info"].Select(i =>
            {
                var value = i.SelectToken("funcscope_category.id").Value<string>();
                return (RightEnum)Enum.Parse(typeof(RightEnum), value);
            }).ToArray();
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
    /// 刷新访问票据模型。
    /// </summary>
    public sealed class RefreshAccessToken
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的第三方平台的全局唯一票据。
        /// </summary>
        internal RefreshAccessToken()
        {
            CreateTime = DateTime.Now;
        }

        #endregion Constructor

        /// <summary>
        /// 授权方令牌。
        /// </summary>
        [JsonProperty("authorizer_access_token")]
        public string AuthorizerAccessToken { get; set; }

        /// <summary>
        /// 刷新令牌。
        /// </summary>
        [JsonProperty("authorizer_refresh_token")]
        public string AuthorizerRefreshToken { get; set; }

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
    /// 权限集枚举。
    /// </summary>
    public enum RightEnum
    {
        /// <summary>
        /// 消息与菜单权限集（帮助公众号向用户回复消息、创建菜单）
        /// </summary>
        MessageAndMenu = 1,

        /// <summary>
        /// 用户管理权限集（帮助公众号进行用户管理）
        /// </summary>
        UserManager = 2,

        /// <summary>
        /// 帐号管理权限集（帮助公众号进行帐号管理）
        /// </summary>
        AccountManager = 3,

        /// <summary>
        /// 网页授权权限集（帮助公众号进行网页开发）
        /// </summary>
        WebPageAuthorizer = 4,

        /// <summary>
        /// 微信小店权限集（帮助公众号管理微信小店）
        /// </summary>
        WeiXinShop = 5,

        /// <summary>
        /// 多客服权限集（帮助公众号管理多客服系统）
        /// </summary>
        TransferCustomerService = 6,

        /// <summary>
        /// 业务通知权限集（帮助公众号通知用户）
        /// </summary>
        BusinessNotification = 7,

        /// <summary>
        /// 微信卡券权限集（帮助公众号管理微信卡券）
        /// </summary>
        WeiXinCard = 8,

        /// <summary>
        /// 素材管理权限集（帮助公众号管理图文消息、图片等素材）。
        /// </summary>
        MaterialManager = 9,

        /// <summary>
        /// 摇一摇周边权限集（帮助公众号管理摇一摇周边）。
        /// </summary>
        ShakePeriphery = 10,

        /// <summary>
        /// 线下门店权限集（帮助公众号管理线下门店数据）
        /// </summary>
        OfflineStore = 11,

        /// <summary>
        /// 微信连WIFI权限集（帮助公众号配置管理WIFI）
        /// </summary>
        WeiXinConnectionWifi = 12,
    }

    /// <summary>
    /// 公众号授权信息。
    /// </summary>
    public sealed class PublicAccountAuthorizerInfo
    {
        /// <summary>
        /// 初始化一个新的公众号授权信息。
        /// </summary>
        public PublicAccountAuthorizerInfo()
        {
            CreateTime = DateTime.Now;
        }

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

        /// <summary>
        /// 刷新令牌（在授权的公众号具备API权限时，才有此返回值），刷新令牌主要用于公众号第三方平台获取和刷新已授权用户的access_token，只会在授权时刻提供，请妥善保存。 一旦丢失，只能让用户重新授权，才能再次拿到新的刷新令牌
        /// </summary>
        [JsonProperty("authorizer_refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 权限集。
        /// </summary>
        [JsonIgnore]
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
            [JsonProperty("authorizer_appid")]
            public string AppId { get; set; }

            /// <summary>
            /// 权限集。
            /// </summary>
            [JsonIgnore]
            public RightEnum[] Rights { get; set; }
        }

        /// <summary>
        /// 授权者信息。
        /// </summary>
        public sealed class AuthorizerInfo
        {
            /// <summary>
            /// 昵称。
            /// </summary>
            [JsonProperty("nick_name")]
            public string NickName { get; set; }

            /// <summary>
            /// 服务类型。
            /// </summary>
            [JsonIgnore]
            public ServiceTypeEnum ServiceType { get; set; }

            /// <summary>
            /// 授权类型。
            /// </summary>
            [JsonIgnore]
            public AuthenticateTypeEnum AuthenticateType { get; set; }

            /// <summary>
            /// 头像Url。
            /// </summary>
            [JsonProperty("head_img")]
            public string HeadPicture { get; set; }

            /// <summary>
            /// 用户名。
            /// </summary>
            [JsonProperty("user_name")]
            public string UserName { get; set; }

            /// <summary>
            /// 别名。
            /// </summary>
            [JsonProperty("alias")]
            public string Alias { get; set; }

            /// <summary>
            /// 二维码图片Url。
            /// </summary>
            [JsonProperty("qrcode_url")]
            public string QrCode { get; set; }
        }

        /// <summary>
        /// 授权信息。
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// 授权者信心。
        /// </summary>
        public AuthorizerInfo Authorizer { get; set; }
    }

    #endregion Help Class
}