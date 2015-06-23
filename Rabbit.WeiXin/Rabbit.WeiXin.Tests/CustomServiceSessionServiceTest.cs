using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.CustomService;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CustomServiceSessionServiceTest : ApiTestBase, IDisposable
    {
        #region Field

        private readonly ICustomServiceSessionService _customServiceSessionService;
        private readonly ICustomServiceService _customServiceService;
        private const string Account = "test@chunsun_cc";

        #endregion Field

        #region Constructor

        public CustomServiceSessionServiceTest()
        {
            _customServiceSessionService = new CustomServiceSessionService(AccountModel);
            _customServiceService = new CustomServiceService(AccountModel);
            Reset();
            _customServiceService.AddAccount(Account, "E10ADC3949BA59ABBE56E057F20F883E", "test");
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void CreateTest()
        {
            _customServiceSessionService.Create(OpenId, Account, "创建了会话");
        }

        [TestMethod]
        public void CloseTest()
        {
            _customServiceSessionService.Close(OpenId, Account, "关闭了会话");
        }

        [TestMethod]
        public void GetTest()
        {
            var info = _customServiceSessionService.Get(OpenId);
            Assert.IsFalse(string.IsNullOrWhiteSpace(info.Account));
            Assert.AreEqual(DateTime.Now.Date, info.CreateTime.Date);
        }

        [TestMethod]
        public void GetListTest()
        {
            var list = _customServiceSessionService.GetList(Account);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Length > 0);
        }

        [TestMethod]
        public void GetWaitListTest()
        {
            var info = _customServiceSessionService.GetWaitList();
            Assert.IsNotNull(info);
        }

        #endregion Test Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }

        #endregion Implementation of IDisposable

        #region Private Method

        private void Reset()
        {
            if (_customServiceService.GetAccounts().Any(i => i.Account == "test@chunsun_cc"))
            {
                _customServiceService.DeleteAccount("test@chunsun_cc");
            }
        }

        #endregion Private Method
    }
}