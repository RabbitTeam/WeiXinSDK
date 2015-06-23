using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.CustomService;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CustomServiceServiceTest : ApiTestBase, IDisposable
    {
        #region Field

        private readonly ICustomServiceService _customServiceService;

        #endregion Field

        #region Constructor

        public CustomServiceServiceTest()
        {
            _customServiceService = new CustomServiceService(AccountModel);

            Reset();
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void AddAccountTest()
        {
            _customServiceService.AddAccount("test@chunsun_cc", "E10ADC3949BA59ABBE56E057F20F883E", "test");
            Assert.IsTrue(_customServiceService.GetAccounts().Any(i => i.Account == "test@chunsun_cc"));
            _customServiceService.DeleteAccount("test@chunsun_cc");
        }

        [TestMethod]
        public void ModifyAccountTest()
        {
            _customServiceService.AddAccount("test@chunsun_cc", "E10ADC3949BA59ABBE56E057F20F883E", "test");
            _customServiceService.ModifyAccount("test@chunsun_cc", "E10ADC3949BA59ABBE56E057F20F883E", "test_new");
            var info = _customServiceService.GetAccounts().First(i => i.Account == "test@chunsun_cc");
            Assert.AreEqual("test_new", info.NickName);
            _customServiceService.DeleteAccount("test@chunsun_cc");
        }

        [TestMethod]
        public void DeleteAccountTest()
        {
            Func<int> getCount = () => _customServiceService.GetAccounts().Length;
            _customServiceService.AddAccount("test@chunsun_cc", "E10ADC3949BA59ABBE56E057F20F883E", "test");
            var count = getCount();
            _customServiceService.DeleteAccount("test@chunsun_cc");
            Assert.IsTrue(count > getCount());
        }

        [TestMethod]
        public void SetAccountHeadPicture()
        {
            _customServiceService.AddAccount("test@chunsun_cc", "E10ADC3949BA59ABBE56E057F20F883E", "test");
            _customServiceService.SetAccountHeadPicture("test@chunsun_cc", ApiTestHelper.GetJpgBytes());
            var headPictureUrl = _customServiceService.GetAccounts().First(i => i.Account == "test@chunsun_cc").HeadPictureUrl;
            Assert.IsNotNull(headPictureUrl);
        }

        [TestMethod]
        public void GetAccountsTest()
        {
            var accounts = _customServiceService.GetAccounts();
            Assert.IsNotNull(accounts);
        }

        [TestMethod]
        public void GetOnlineListTest()
        {
            var list = _customServiceService.GetOnlineList();
            Assert.IsNotNull(list);
        }

        [TestMethod]
        public void GetRecordsTest()
        {
            var records = _customServiceService.GetRecords(DateTime.Now, DateTime.Now.AddDays(1));
            Assert.IsNotNull(records);
        }

        #endregion Test Method

        #region Private Method

        private void Reset()
        {
            if (_customServiceService.GetAccounts().Any(i => i.Account == "test@chunsun_cc"))
            {
                _customServiceService.DeleteAccount("test@chunsun_cc");
            }
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetMd5(string encypStr, string charset)
        {
            using (var m5 = new MD5CryptoServiceProvider())
            {
                //创建md5对象
                byte[] inputBye;

                //使用GB2312编码方式把字符串转化为字节数组．
                try
                {
                    inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
                }
                catch
                {
                    inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
                }
                var outputBye = m5.ComputeHash(inputBye);

                var retStr = BitConverter.ToString(outputBye);
                retStr = retStr.Replace("-", "").ToUpper();
                return retStr;
            }
        }

        #endregion Private Method

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Reset();
        }

        #endregion Implementation of IDisposable
    }
}