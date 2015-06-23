using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
using System;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.User
{
    /// <summary>
    /// 一个抽象的用户分组服务。
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// 创建分组。
        /// </summary>
        /// <param name="name">分组名称（30字以内）</param>
        /// <returns>创建分组结果模型。</returns>
        CreateGroupResultModel Create(string name);

        /// <summary>
        /// 获取所有分组。
        /// </summary>
        /// <returns>分组项集合。</returns>
        GetGroupListItem[] GetList();

        /// <summary>
        /// 根据OpenId获取分组Id。
        /// </summary>
        /// <param name="openId">用户唯一标识符。</param>
        /// <returns>分组Id。</returns>
        ulong GetGroupByOpenId(string openId);

        /// <summary>
        /// 修改分组名称。
        /// </summary>
        /// <param name="groupId">分组Id。</param>
        /// <param name="name">分组名称（30字以内）</param>
        void ModifyGroupName(ulong groupId, string name);

        /// <summary>
        /// 移动用户分组。
        /// </summary>
        /// <param name="openId">用户唯一标识符。</param>
        /// <param name="toGroupId">新分组的Id。</param>
        void MoveUserGroup(string openId, ulong toGroupId);

        /// <summary>
        /// 批量移动用户分组。
        /// </summary>
        /// <param name="openIds">用户唯一标识符openid的列表（size不能超过50）</param>
        /// <param name="toGroupId">新分组的Id。</param>
        void MoveUsersGroup(string[] openIds, ulong toGroupId);

        /// <summary>
        /// 删除分组（删除分组后，所有该分组内的用户自动进入默认分组）。
        /// </summary>
        /// <param name="groupId">分组Id。</param>
        void Delete(ulong groupId);
    }

    /// <summary>
    /// 用户分组服务的实现。
    /// </summary>
    public sealed class UserGroupService : IUserGroupService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public UserGroupService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IUserGroupService

        /// <summary>
        /// 创建分组。
        /// </summary>

        /// <param name="name">分组名称（30字以内）</param>
        /// <returns>创建分组结果模型。</returns>
        public CreateGroupResultModel Create(string name)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/create?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.PostString(url, new { group = new { name } });
            return CreateGroupResultModel.Create(JObject.Parse(content));
        }

        /// <summary>
        /// 获取所有分组。
        /// </summary>

        /// <returns>分组项集合。</returns>
        public GetGroupListItem[] GetList()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/get?access_token=" + _accountModel.GetAccessToken();

            var content = HttpHelper.GetString(url);
            ResultHelper.TryThrowError(content);
            var arrayContent = JObject.Parse(content)["groups"].ToString();
            return JsonConvert.DeserializeObject<GetGroupListItem[]>(arrayContent);
        }

        /// <summary>
        /// 根据OpenId获取分组Id。
        /// </summary>

        /// <param name="openId">用户唯一标识符。</param>
        /// <returns>分组Id。</returns>
        public ulong GetGroupByOpenId(string openId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/getid?access_token=" + _accountModel.GetAccessToken();

            var data = WeiXinHttpHelper.Post(url, new { openid = openId });
            var content = Encoding.UTF8.GetString(data);
            return JObject.Parse(content).Value<ulong>("groupid");
        }

        /// <summary>
        /// 修改分组名称。
        /// </summary>

        /// <param name="groupId">分组Id。</param>
        /// <param name="name">分组名称（30字以内）</param>
        public void ModifyGroupName(ulong groupId, string name)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/update?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { group = new { id = groupId, name } });
        }

        /// <summary>
        /// 移动用户分组。
        /// </summary>

        /// <param name="openId">用户唯一标识符。</param>
        /// <param name="toGroupId">新分组的Id。</param>
        public void MoveUserGroup(string openId, ulong toGroupId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { openid = openId, to_groupid = toGroupId });
        }

        /// <summary>
        /// 批量移动用户分组。
        /// </summary>

        /// <param name="openIds">用户唯一标识符openid的列表（size不能超过50）</param>
        /// <param name="toGroupId">新分组的Id。</param>
        public void MoveUsersGroup(string[] openIds, ulong toGroupId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { openid_list = openIds, to_groupid = toGroupId });
        }

        /// <summary>
        /// 删除分组（删除分组后，所有该分组内的用户自动进入默认分组）。
        /// </summary>

        /// <param name="groupId">分组Id。</param>
        public void Delete(ulong groupId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/groups/delete?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { group = new { id = groupId } });
        }

        #endregion Implementation of IUserGroupService
    }

    #region Help Class

    /// <summary>
    /// 创建分组结果模型。
    /// </summary>
    public sealed class CreateGroupResultModel
    {
        /// <summary>
        /// 分组Id。
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// 分组名称。
        /// </summary>
        public string Name { get; set; }

        internal static CreateGroupResultModel Create(JObject obj)
        {
            var groupObj = obj["group"] as JObject;
            if (groupObj == null)
                throw new NotSupportedException("不支持的分组格式。");
            return new CreateGroupResultModel
            {
                Id = groupObj.Value<ulong>("id"),
                Name = groupObj.Value<string>("name")
            };
        }
    }

    /// <summary>
    /// 分组集合项。
    /// </summary>
    public sealed class GetGroupListItem
    {
        /// <summary>
        /// 分组Id。
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 分组名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户数。
        /// </summary>
        [JsonProperty("count")]
        public uint UserCount { get; set; }
    }

    #endregion Help Class
}