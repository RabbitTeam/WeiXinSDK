using Xunit;
using Rabbit.WeiXin.MP.Api.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Tests
{
    
    public class UserGroupServiceTest : ApiTestBase
    {
        #region Field

        private readonly IUserGroupService _userGroupService;

        #endregion Field

        #region Constructor

        public UserGroupServiceTest()
        {
            _userGroupService = new UserGroupService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [Fact]
        public void CreateTest()
        {
            var result = _userGroupService.Create("test");
            Assert.True(result.Id > 0);
            Assert.Equal("test", result.Name);
        }

        [Fact]
        public void GetList()
        {
            var list = _userGroupService.GetList();
            Assert.True(list.Any());
        }

        [Fact]
        public void GetGroupByOpenId()
        {
            _userGroupService.GetGroupByOpenId(OpenId);
        }

        [Fact]
        public void ModifyGroupName()
        {
            var result = _userGroupService.Create("test");
            var newName = Guid.NewGuid().ToString("n").Substring(0, 30);
            Task.Delay(100).Wait();
            _userGroupService.ModifyGroupName(result.Id, newName);
            Task.Delay(300).Wait();
            Assert.True(_userGroupService.GetList().Any(i => i.Name == newName));
        }

        [Fact]
        public void MoveUserGroup()
        {
            var result = _userGroupService.Create("test");
            Task.Delay(300).Wait();
            _userGroupService.MoveUserGroup(OpenId, result.Id);
            Task.Delay(300).Wait();
            Assert.Equal(result.Id, _userGroupService.GetGroupByOpenId(OpenId));
        }

        [Fact]
        public void MoveUsersGroup()
        {
            var result = _userGroupService.Create("test");
            _userGroupService.MoveUsersGroup(new[] { OpenId }, result.Id);
            Assert.Equal(result.Id, _userGroupService.GetGroupByOpenId(OpenId));
        }

        [Fact]
        public void DeleteGroupTest()
        {
            Func<int> getCount = () => _userGroupService.GetList().Count();

            var outCount = getCount();
            var result = _userGroupService.Create("test");
            _userGroupService.Delete(result.Id);
            var newCount = getCount();
            Assert.Equal(outCount - 1, newCount);
        }

        #endregion Test Method
    }
}