using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
using Rabbit.WeiXin.Utility;
using System;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api.CustomService
{
    /// <summary>
    /// 一个抽象的多客服会话服务接口。
    /// </summary>
    public interface ICustomServiceSessionService
    {
        /// <summary>
        /// 创建会话。
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="text">附加信息，文本会展示在客服人员的多客服客户端</param>
        /// <remarks>开发者可以使用本接口，为多客服的客服工号创建会话，将某个客户直接指定给客服工号接待，需要注意此接口不会受客服自动接入数以及自动接入开关限制。只能为在线的客服（PC客户端在线，或者已绑定多客服助手）创建会话。</remarks>
        void Create(string openId, string account, string text = null);

        /// <summary>
        /// 关闭会话。
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="text">附加信息，文本会展示在客服人员的多客服客户端</param>
        /// <remarks>开发者可以使用本接口，关闭一个会话。</remarks>
        void Close(string openId, string account, string text = null);

        /// <summary>
        /// 获取客户的会话状态.
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <returns>会话信息。</returns>
        UserSessionInfo Get(string openId);

        /// <summary>
        /// 获取客服的会话列表。
        /// </summary>
        /// <param name="account">客服账号。</param>
        /// <returns>客服会话信息数组。</returns>
        CustomServiceSessionInfo[] GetList(string account);

        /// <summary>
        /// 获取未接入会话列表。
        /// </summary>
        /// <returns>等待服务的信息。</returns>
        /// <remarks>开发者可以通过本接口获取当前正在等待队列中的会话列表，此接口最多返回最早进入队列的100个未接入会话。</remarks>
        WaitServiceInfo GetWaitList();
    }

    /// <summary>
    /// 多客服会话服务实现。
    /// </summary>
    public class CustomServiceSessionService : ICustomServiceSessionService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public CustomServiceSessionService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICustomSessionService

        /// <summary>
        /// 创建会话。
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="text">附加信息，文本会展示在客服人员的多客服客户端</param>
        /// <remarks>开发者可以使用本接口，为多客服的客服工号创建会话，将某个客户直接指定给客服工号接待，需要注意此接口不会受客服自动接入数以及自动接入开关限制。只能为在线的客服（PC客户端在线，或者已绑定多客服助手）创建会话。</remarks>
        public void Create(string openId, string account, string text = null)
        {
            var url = "https://api.weixin.qq.com/customservice/kfsession/create?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new
            {
                kf_account = account,
                openid = openId,
                text
            });
        }

        /// <summary>
        /// 关闭会话。
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="text">附加信息，文本会展示在客服人员的多客服客户端</param>
        /// <remarks>开发者可以使用本接口，关闭一个会话。</remarks>
        public void Close(string openId, string account, string text = null)
        {
            var url = "https://api.weixin.qq.com/customservice/kfsession/close?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new
            {
                kf_account = account,
                openid = openId,
                text
            });
        }

        /// <summary>
        /// 获取客户的会话状态.
        /// </summary>
        /// <param name="openId">客户openid</param>
        /// <returns>会话信息。</returns>
        public UserSessionInfo Get(string openId)
        {
            var url = string.Format("https://api.weixin.qq.com/customservice/kfsession/getsession?access_token={0}&openid={1}", _accountModel.GetAccessToken(), openId);

            return WeiXinHttpHelper.GetResultByJson<UserSessionInfo>(url);
        }

        /// <summary>
        /// 获取客服的会话列表。
        /// </summary>
        /// <param name="account">客服账号。</param>
        /// <returns>客服会话信息数组。</returns>
        public CustomServiceSessionInfo[] GetList(string account)
        {
            var url = string.Format("https://api.weixin.qq.com/customservice/kfsession/getsessionlist?access_token={0}&kf_account={1}", _accountModel.GetAccessToken(), account);

            var content = WeiXinHttpHelper.GetString(url);
            var array = (JArray)JObject.Parse(content)["sessionlist"];

            return array.Select(i => new CustomServiceSessionInfo
            {
                CreateTimestamp = i.Value<ulong>("createtime"),
                OpenId = i.Value<string>("openid")
            }).ToArray();
        }

        /// <summary>
        /// 获取未接入会话列表。
        /// </summary>
        /// <returns>等待服务的信息。</returns>
        /// <remarks>开发者可以通过本接口获取当前正在等待队列中的会话列表，此接口最多返回最早进入队列的100个未接入会话。</remarks>
        public WaitServiceInfo GetWaitList()
        {
            var url = string.Format("https://api.weixin.qq.com/customservice/kfsession/getwaitcase?access_token={0}", _accountModel.GetAccessToken());

            return WeiXinHttpHelper.GetResultByJson<WaitServiceInfo>(url);
        }

        #endregion Implementation of ICustomSessionService
    }

    #region Help Class

    /// <summary>
    /// 等待服务的信息。
    /// </summary>
    public sealed class WaitServiceInfo
    {
        /// <summary>
        /// 等待服务的项。
        /// </summary>
        public sealed class WaitServiceItem
        {
            [JsonProperty("createtime")]
            internal ulong CreateTimestamp { get; set; }

            /// <summary>
            /// 用户来访时间。
            /// </summary>
            [JsonIgnore]
            public DateTime CreateTime
            {
                get { return DateTimeHelper.GetTimeByTimeStamp(CreateTimestamp); }
            }

            /// <summary>
            /// 客户openid。
            /// </summary>
            public string OpenId { get; set; }

            /// <summary>
            /// 正在接待的客服，为空表示没有人在接待。
            /// </summary>
            [JsonProperty("kf_account")]
            public string Account { get; set; }
        }

        /// <summary>
        /// 未接入会话数量。
        /// </summary>
        public ushort Count { get; set; }

        /// <summary>
        /// 未接入会话列表，最多返回100条数据。
        /// </summary>
        [JsonProperty("waitcaselist")]
        public WaitServiceItem[] List { get; set; }
    }

    /// <summary>
    /// 客服的会话信息。
    /// </summary>
    public sealed class CustomServiceSessionInfo
    {
        internal ulong CreateTimestamp { get; set; }

        /// <summary>
        /// 会话接入的时间。
        /// </summary>
        public DateTime CreateTime
        {
            get { return DateTimeHelper.GetTimeByTimeStamp(CreateTimestamp); }
        }

        /// <summary>
        /// 客户openid。
        /// </summary>
        public string OpenId { get; set; }
    }

    /// <summary>
    /// 用户的客服会话信息。
    /// </summary>
    public sealed class UserSessionInfo
    {
        [JsonProperty("createtime")]
        internal ulong CreateTimestamp { get; set; }

        /// <summary>
        /// 会话接入的时间。
        /// </summary>
        public DateTime CreateTime
        {
            get { return DateTimeHelper.GetTimeByTimeStamp(CreateTimestamp); }
        }

        /// <summary>
        /// 正在接待的客服，为空表示没有人在接待。
        /// </summary>
        [JsonProperty("kf_account")]
        public string Account { get; set; }
    }

    #endregion Help Class
}