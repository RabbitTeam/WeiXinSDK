using Xunit;
using Rabbit.WeiXin.MP.Api.User;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    
    public class UserServiceTest : ApiTestBase
    {
        #region Field

        private readonly IUserService _userService;

        #endregion Field

        #region Constructor

        public UserServiceTest()
        {
            _userService = new UserService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [Fact]
        public void GetUserTest()
        {
            var userInfo = _userService.GetUser(OpenId);

            Assert.NotNull(userInfo);
            Assert.NotNull(userInfo.OpenId);
        }

        [Fact]
        public void GetUserListTest()
        {
            var list = _userService.GetUserList();

            Assert.True(list.Count > 0);
            Assert.True(list.TotalCount > 0);
            Assert.NotNull(list.OpenIds);
            Assert.True(list.OpenIds.Any());

            var userList = _userService.GetUserList(OpenId);

            Assert.True(userList.Count < list.Count);
        }

        [Fact]
        public void SetRemarkTest()
        {
            var remarkName = "remark_majian" + "_" + new Random().Next(0, 9999);
            _userService.SetRemark(OpenId, remarkName);

            Assert.Equal(remarkName, _userService.GetUser(OpenId).Remark);
        }

        [Fact]
        public void GetUserInfoListTest()
        {
            var list = _userService.GetUserInfoList(OpenId);

            Assert.True(list.Any());
        }

        #endregion Test Method
    }
}