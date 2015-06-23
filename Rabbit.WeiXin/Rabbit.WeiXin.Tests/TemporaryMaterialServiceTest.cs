using Rabbit.WeiXin.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.Material;
using System;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class TemporaryMaterialServiceTest : ApiTestBase
    {
        #region Field

        private readonly ITemporaryMaterialService _materialService;

        #endregion Field

        #region Constructor

        public TemporaryMaterialServiceTest()
        {
            _materialService = new TemporaryMaterialService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void AddJpgTemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Image);
            Assert.AreEqual(MaterialType.Image, result.Type);
            Assert.IsNotNull(result.MediaId);
            Assert.AreEqual(DateTime.Now.Year, result.CreateTime.Year);
        }

        [TestMethod]
        public void AddArmTemporary()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetAmrBytes(), MaterialType.Voice);
            Assert.AreEqual(MaterialType.Voice, result.Type);
            Assert.IsNotNull(result.MediaId);
            Assert.AreEqual(DateTime.Now.Year, result.CreateTime.Year);
        }

        [TestMethod]
        public void AddMp3TemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetMp3Bytes(), MaterialType.Voice);
            Assert.AreEqual(MaterialType.Voice, result.Type);
            Assert.IsNotNull(result.MediaId);
            Assert.AreEqual(DateTime.Now.Year, result.CreateTime.Year);
        }

        [TestMethod]
        public void AddMp4TemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetMp4Bytes(), MaterialType.Video);
            Assert.AreEqual(MaterialType.Video, result.Type);
            Assert.IsNotNull(result.MediaId);
            Assert.AreEqual(DateTime.Now.Year, result.CreateTime.Year);
        }

        [TestMethod]
        public void GetTemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Image);
            var model = _materialService.GetTemporary(result.MediaId);

            Assert.AreEqual("image/jpeg", model.ContentType);
            Assert.IsNotNull(model.FileName);
            Assert.IsTrue(model.Data.Length > 0);
        }

        #endregion Test Method
    }
}