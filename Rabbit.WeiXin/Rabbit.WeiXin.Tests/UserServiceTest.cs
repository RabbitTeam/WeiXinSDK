using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.User;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
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

        [TestMethod]
        public void GetUserTest()
        {
            var userInfo = _userService.GetUser(OpenId);

            Assert.IsNotNull(userInfo);
            Assert.IsNotNull(userInfo.OpenId);
        }

        [TestMethod]
        public void GetUserListTest()
        {
            var list = _userService.GetUserList();

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.TotalCount > 0);
            Assert.IsNotNull(list.OpenIds);
            Assert.IsTrue(list.OpenIds.Any());

            var userList = _userService.GetUserList(OpenId);

            Assert.IsTrue(userList.Count < list.Count);
        }

        [TestMethod]
        public void SetRemarkTest()
        {
            var remarkName = "remark_majian" + "_" + new Random().Next(0, 9999);
            _userService.SetRemark(OpenId, remarkName);

            Assert.AreEqual(remarkName, _userService.GetUser(OpenId).Remark);
        }

        #endregion Test Method
    }
}