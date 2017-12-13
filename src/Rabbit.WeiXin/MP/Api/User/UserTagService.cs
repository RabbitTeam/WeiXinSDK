using Newtonsoft.Json;
using Rabbit.WeiXin.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.User
{
    /// <summary>
    /// 用户标签管理接口的接口。
    /// </summary>
    public interface IUserTagService
    {
        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="name">标签名(30个字符以内)</param>
        /// <returns>返回标签编号</returns>
        TagInfo Create(string name);

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns>标签集合</returns>
        List<TagInfo> Get();

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="tagInfo">编辑的标签</param>
        void Update(TagInfo tagInfo);

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId">删除标签的编号</param>
        void Delete(int tagId);

        /// <summary>
        /// 获取标签下所有用户openid
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="nextOpenId">第一个拉取的openid, 默认重头开始</param>
        /// <returns>标签下粉丝列表</returns>
        GetTagUserResultModel Get(int tagId, string nextOpenId = "");

        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openIds">粉丝openid列表</param>
        void BatchTagging(int tagId, IEnumerable<string> openIds);

        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openIds">粉丝openid列表</param>
        void BatchUntagging(int tagId, IEnumerable<string> openIds);

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openId">粉丝OpenId</param>
        /// <returns>标签列表集合</returns>
        List<int> GetIdList(string openId);
    }

    /// <summary>
    /// 用户标签管理接口的实现。
    /// </summary>
    public class UserTagService : IUserTagService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的用户服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public UserTagService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of UserTagService

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="name">标签名(30个字符以内)</param>
        /// <returns>返回标签编号</returns>
        public TagInfo Create(string name)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/create?access_token={_accountModel.GetAccessToken()}";
            var result = WeiXinHttpHelper.PostResultByJson<TagWarp>(url, new { tag = new { name } });
            return result.Tag;
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="tagId">删除标签的编号</param>
        public void Delete(int tagId)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/delete?access_token={_accountModel.GetAccessToken()}";
            WeiXinHttpHelper.Post(url, new { tag = new { id = tagId } });
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns>标签集合</returns>
        public List<TagInfo> Get()
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/get?access_token={_accountModel.GetAccessToken()}";
            var result = WeiXinHttpHelper.GetResultByJson<TagWarp>(url);
            return result.Tags;
        }

        /// <summary>
        /// 获取标签下所有用户openid
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="nextOpenId">第一个拉取的openid, 默认重头开始</param>
        /// <returns>标签下粉丝列表</returns>
        public GetTagUserResultModel Get(int tagId, string nextOpenId = "")
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/user/tag/get?access_token={_accountModel.GetAccessToken()}";
            return WeiXinHttpHelper.PostResultByJson<GetTagUserResultModel>(url, new { tagid = tagId, next_openid = nextOpenId });
        }

        /// <summary>
        /// 编辑标签
        /// </summary>
        /// <param name="tagInfo">编辑的标签</param>
        public void Update(TagInfo tagInfo)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/update?access_token={_accountModel.GetAccessToken()}";
            WeiXinHttpHelper.Post(url, new { tag = new { id = tagInfo.Id, name = tagInfo.Name } });
        }

        /// <summary>
        /// 批量为用户打标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openIds">粉丝openid列表</param>
        public void BatchTagging(int tagId, IEnumerable<string> openIds)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/members/batchtagging?access_token={_accountModel.GetAccessToken()}";
            WeiXinHttpHelper.Post(url, new { tagid = tagId, openid_list = openIds });
        }

        /// <summary>
        /// 批量为用户取消标签
        /// </summary>
        /// <param name="tagId">标签编号</param>
        /// <param name="openIds">粉丝openid列表</param>
        public void BatchUntagging(int tagId, IEnumerable<string> openIds)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/tags/members/batchuntagging?access_token={_accountModel.GetAccessToken()}";
            WeiXinHttpHelper.Post(url, new { tagid = tagId, openid_list = openIds });
        }

        /// <summary>
        /// 获取用户身上的标签列表
        /// </summary>
        /// <param name="openId">粉丝OpenId</param>
        /// <returns>标签列表集合</returns>
        public List<int> GetIdList(string openId)
        {
            string url = $" https://api.weixin.qq.com/cgi-bin/tags/getidlist?access_token={_accountModel.GetAccessToken()}";
            GetIdListModel result = WeiXinHttpHelper.PostResultByJson<GetIdListModel>(url, new { openid = openId });
            return result?.TagIds ?? new List<int>(0);
        }

        #endregion Implementation of UserTagService
    }

    #region Help Class

    /// <summary>
    /// 标签序列化包装对象
    /// </summary>
    public sealed class TagWarp
    {
        /// <summary>
        /// 标签
        /// </summary>
        [JsonProperty("tag")]
        public TagInfo Tag { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        [JsonProperty("tags")]
        public List<TagInfo> Tags { get; set; }
    }

    /// <summary>
    /// 标签信息
    /// </summary>
    public sealed class TagInfo
    {
        /// <summary>
        /// 标签编号
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 此标签下粉丝数
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }
    }


    /// <summary>
    /// 获取标签下粉丝列表模型
    /// </summary>
    public sealed class GetTagUserResultModel
    {

        /// <summary>
        /// 该次获取的粉丝数量
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }


        /// <summary>
        /// 拉取列表最后一个用户的openid
        /// </summary>
        [JsonProperty("next_openid")]
        public string NextOpenId { get; set; }

        /// <summary>
        /// 粉丝列表OpenId
        /// </summary>
        [JsonProperty("data")]
        public TagUserItem Data { get; set; } = new TagUserItem();
    }

    /// <summary>
    /// OpenId项
    /// </summary>
    public sealed class TagUserItem
    {
        /// <summary>
        ///粉丝OpenId
        /// </summary>
        [JsonProperty("openid")]
        public List<string> OpenIds { get; set; } = new List<string>();
    }

    /// <summary>
    /// 用户身上的标签列表模型
    /// </summary>
    sealed class GetIdListModel
    {
        /// <summary>
        /// 被置上的标签列表
        /// </summary>
        [JsonProperty("tagid_list")]
        public List<int> TagIds { get; set; }
    }

    #endregion Help Class
}
