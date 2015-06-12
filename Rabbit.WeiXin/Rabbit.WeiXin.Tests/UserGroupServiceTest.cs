using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Api.User;
using System;
using System.Linq;
using System.Threading;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
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

        [TestMethod]
        public void CreateTest()
        {
            var result = _userGroupService.Create("test");
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("test", result.Name);
        }

        [TestMethod]
        public void GetList()
        {
            var list = _userGroupService.GetList();
            Assert.IsTrue(list.Any());
        }

        [TestMethod]
        public void GetGroupByOpenId()
        {
            _userGroupService.GetGroupByOpenId(OpenId);
        }

        [TestMethod]
        public void ModifyGroupName()
        {
            var result = _userGroupService.Create("test");
            var newName = Guid.NewGuid().ToString("n").Substring(0, 30);
            Thread.Sleep(100);
            _userGroupService.ModifyGroupName(result.Id, newName);
            Thread.Sleep(300);
            Assert.IsTrue(_userGroupService.GetList().Any(i => i.Name == newName));
        }

        [TestMethod]
        public void MoveUserGroup()
        {
            var result = _userGroupService.Create("test");
            Thread.Sleep(300);
            _userGroupService.MoveUserGroup(OpenId, result.Id);
            Thread.Sleep(300);
            Assert.AreEqual(result.Id, _userGroupService.GetGroupByOpenId(OpenId));
        }

        [TestMethod]
        public void MoveUsersGroup()
        {
            var result = _userGroupService.Create("test");
            _userGroupService.MoveUsersGroup(new[] { OpenId }, result.Id);
            Assert.AreEqual(result.Id, _userGroupService.GetGroupByOpenId(OpenId));
        }

        [TestMethod]
        public void DeleteGroupTest()
        {
            Func<int> getCount = () => _userGroupService.GetList().Count();

            var outCount = getCount();
            var result = _userGroupService.Create("test");
            _userGroupService.Delete(result.Id);
            var newCount = getCount();
            Assert.AreEqual(outCount - 1, newCount);
        }

        #endregion Test Method
    }
}