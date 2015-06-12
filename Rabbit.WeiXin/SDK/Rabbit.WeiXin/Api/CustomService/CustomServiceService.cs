using ChunSun.PublicPlatform.Services.Utility;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Api.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace Rabbit.WeiXin.Api.CustomService
{
    /// <summary>
    /// 一个抽象的客户服务接口。
    /// </summary>
    public interface ICustomServiceService
    {
        /// <summary>
        /// 添加客服账号（每个公众号最多添加10个客服账号）。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
        /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
        void AddAccount(string account, string password, string nickName);

        /// <summary>
        /// 修改客服账号。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
        /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
        void ModifyAccount(string account, string password, string nickName);

        /// <summary>
        /// 删除客服账号。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        void DeleteAccount(string account);

        /// <summary>
        /// 设置客服账号的头像。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="pictureData">客服人员的头像，头像图片文件必须是jpg格式，推荐使用640*640大小的图片以达到最佳效果</param>
        void SetAccountHeadPicture(string account, byte[] pictureData);

        /// <summary>
        /// 获取所有客服账号。
        /// </summary>
        /// <returns>客服账号信息数组。</returns>
        CustomServiceAccountInfo[] GetAccounts();

        /// <summary>
        /// 获取在线客服接待信息。
        /// </summary>
        /// <returns>在线客服信息数组。</returns>
        OnlineCustomServiceInfo[] GetOnlineList();

        /// <summary>
        /// 获取聊天记录。
        /// </summary>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间，每次查询不能跨日查询</param>
        /// <param name="pageSize">每页大小，每页最多拉取50条</param>
        /// <param name="pageIndex">查询第几页，从1开始</param>
        /// <param name="openId">普通用户的标识，对当前公众号唯一</param>
        /// <returns>服务记录数组。</returns>
        ServiceRecord[] GetRecords(DateTime startTime, DateTime endTime, ushort pageSize = 50, ushort pageIndex = 1, string openId = null);
    }

    /// <summary>
    /// 多客服服务的实现。
    /// </summary>
    public sealed class CustomServiceService : ICustomServiceService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public CustomServiceService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICustomService

        /// <summary>
        /// 添加客服账号（每个公众号最多添加10个客服账号）。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
        /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
        public void AddAccount(string account, string password, string nickName)
        {
            account.NotEmptyOrWhiteSpace("account");
            password.NotEmptyOrWhiteSpace("password");

            var url = "https://api.weixin.qq.com/customservice/kfaccount/add?access_token=" + _accountModel.GetAccessToken();
            WeiXinHttpHelper.Post(url, new
            {
                kf_account = account,
                nickname = nickName,
                password
            });
        }

        /// <summary>
        /// 修改客服账号。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="password">客服账号登录密码，格式为密码明文的32位加密MD5值。该密码仅用于在公众平台官网的多客服功能中使用，若不使用多客服功能，则不必设置密码</param>
        /// <param name="nickName">客服昵称，最长6个汉字或12个英文字符</param>
        public void ModifyAccount(string account, string password, string nickName)
        {
            account.NotEmptyOrWhiteSpace("account");
            password.NotEmptyOrWhiteSpace("password");

            var url = "https://api.weixin.qq.com/customservice/kfaccount/update?access_token=" + _accountModel.GetAccessToken();
            WeiXinHttpHelper.Post(url, new
            {
                kf_account = account,
                nickname = nickName,
                password
            });
        }

        /// <summary>
        /// 删除客服账号。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        public void DeleteAccount(string account)
        {
            var url = string.Format("https://api.weixin.qq.com/customservice/kfaccount/del?access_token={0}&kf_account={1}", _accountModel.GetAccessToken(), account);
            WeiXinHttpHelper.GetString(url);
        }

        /// <summary>
        /// 设置客服账号的头像。
        /// </summary>
        /// <param name="account">完整客服账号，格式为：账号前缀@公众号微信号</param>
        /// <param name="pictureData">客服人员的头像，头像图片文件必须是jpg格式，推荐使用640*640大小的图片以达到最佳效果</param>
        public void SetAccountHeadPicture(string account, byte[] pictureData)
        {
            if (!FileHelper.IsJpg(pictureData))
                throw new NotSupportedException("头像图片文件必须是jpg格式。");

            var url = string.Format("http://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token={0}&kf_account={1}", _accountModel.GetAccessToken(), account);

            var createBytes = new CreateBytes();
            var list = new ArrayList
            {
                createBytes.CreateFieldData("media", FileHelper.GetRandomFileName(pictureData), FileHelper.GetContentType(pictureData), pictureData),
            };
            var data = createBytes.JoinBytes(list);

            WeiXinHttpHelper.Post(url, data, createBytes.ContentType);
        }

        /// <summary>
        /// 获取所有客服账号。
        /// </summary>
        /// <returns>客服账号信息数组。</returns>
        public CustomServiceAccountInfo[] GetAccounts()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.GetString(url);
            var list = (JArray)JObject.Parse(content)["kf_list"];
            return list.Select(i => new CustomServiceAccountInfo
            {
                Account = i.Value<string>("kf_account"),
                HeadPictureUrl = i.Value<string>("kf_headimgurl"),
                Id = i.Value<ulong>("kf_id"),
                NickName = i.Value<string>("kf_nick"),
            }).ToArray();
        }

        /// <summary>
        /// 获取在线客服接待信息。
        /// </summary>
        /// <returns>在线客服信息数组。</returns>
        public OnlineCustomServiceInfo[] GetOnlineList()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/customservice/getonlinekflist?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.GetString(url);
            var list = (JArray)JObject.Parse(content)["kf_online_list"];
            return list.Select(i => new OnlineCustomServiceInfo
            {
                Account = i.Value<string>("kf_account"),
                Id = i.Value<ulong>("kf_id"),
                MaxAutoAcceptNumber = i.Value<uint>("auto_accept"),
                ServingNumber = i.Value<uint>("accepted_case"),
                StatusNumber = i.Value<ushort>("status")
            }).ToArray();
        }

        /// <summary>
        /// 获取聊天记录。
        /// </summary>
        /// <param name="startTime">查询开始时间</param>
        /// <param name="endTime">查询结束时间，每次查询不能跨日查询</param>
        /// <param name="pageSize">每页大小，每页最多拉取50条</param>
        /// <param name="pageIndex">查询第几页，从1开始</param>
        /// <param name="openId">普通用户的标识，对当前公众号唯一</param>
        /// <returns>服务记录数组。</returns>
        public ServiceRecord[] GetRecords(DateTime startTime, DateTime endTime, ushort pageSize = 50, ushort pageIndex = 1,
            string openId = null)
        {
            if ((endTime - startTime) > TimeSpan.FromDays(1))
                throw new ArgumentException("查询的结束时间过长，每次查询不能跨日查询。", "endTime");
            if (pageIndex <= 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, "查询页最小为1。");
            if (pageSize <= 0 || pageSize > 50)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页的数据必需在1~50之间（含50）。");

            var url = "https://api.weixin.qq.com/customservice/msgrecord/getrecord?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.PostString(url, new
            {
                openid = openId,
                starttime = DateTimeHelper.GetTimeStampByTime(startTime),
                endtime = DateTimeHelper.GetTimeStampByTime(endTime),
                pagesize = pageSize,
                pageindex = pageIndex
            });

            var array = (JArray)JObject.Parse(content)["recordlist"];

            return array.Select(i => new ServiceRecord
            {
                OpenId = i.Value<string>("openid"),
                OperCode = i.Value<ushort>("opercode"),
                Text = i.Value<string>("text"),
                Timestamp = i.Value<ulong>("time"),
                Account = i.Value<string>("worker")
            }).ToArray();
        }

        #endregion Implementation of ICustomService
    }

    #region Help Class

    /// <summary>
    /// 服务记录。
    /// </summary>
    public sealed class ServiceRecord
    {
        /// <summary>
        /// 客服账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 操作ID（会话状态），具体说明见下文
        /// </summary>
        /// <remarks>
        /// 1000	创建未接入会话
        /// 1001	接入会话
        /// 1002	主动发起会话
        /// 1004	关闭会话
        /// 1005	抢接会话
        /// 2001	公众号收到消息
        /// 2002	客服发送消息
        /// 2003	客服收到消息
        /// </remarks>
        public ushort OperCode { get; set; }

        internal ulong Timestamp { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Time
        {
            get { return DateTimeHelper.GetTimeByTimeStamp(Timestamp); }
        }

        /// <summary>
        /// 聊天记录
        /// </summary>
        public string Text { get; set; }
    }

    /// <summary>
    /// 客服在线状态。
    /// </summary>
    public enum CustomServiceOnlineStatus
    {
        /// <summary>
        /// PC在线。
        /// </summary>
        Pc = 1,

        /// <summary>
        /// 手机在线。
        /// </summary>
        Mobile = 2,

        /// <summary>
        /// PC和手机同时在线。
        /// </summary>
        PcAndMobile = 3
    }

    /// <summary>
    /// 在线客服信息。
    /// </summary>
    public sealed class OnlineCustomServiceInfo
    {
        /// <summary>
        /// 完整客服账号，格式为：账号前缀@公众号微信号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 客服工号
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 客服在线状态 1：pc在线，2：手机在线。若pc和手机同时在线则为 1+2=3
        /// </summary>
        internal ushort StatusNumber { get; set; }

        /// <summary>
        /// 客服在线状态。
        /// </summary>
        public CustomServiceOnlineStatus Status
        {
            get
            {
                CustomServiceOnlineStatus value;
                Enum.TryParse(StatusNumber.ToString(CultureInfo.InvariantCulture), out value);
                return value;
            }
            set { StatusNumber = (ushort)value; }
        }

        /// <summary>
        /// 客服设置的最大自动接入数。
        /// </summary>
        public uint MaxAutoAcceptNumber { get; set; }

        /// <summary>
        /// 客服当前正在接待的会话数。
        /// </summary>
        public uint ServingNumber { get; set; }
    }

    /// <summary>
    /// 多客服账号信息。
    /// </summary>
    public sealed class CustomServiceAccountInfo
    {
        /// <summary>
        /// 完整客服账号，格式为：账号前缀@公众号微信号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 客服昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 客服工号
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 客服头像。
        /// </summary>
        public string HeadPictureUrl { get; set; }
    }

    #endregion Help Class
}