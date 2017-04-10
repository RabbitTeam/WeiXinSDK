using Xunit;
using Rabbit.WeiXin.MP.Api.Material;
using Rabbit.WeiXin.Tests.Utility;
using System;

namespace Rabbit.WeiXin.Tests
{
    
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

        [Fact]
        public void AddJpgTemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Image);
            Assert.Equal(MaterialType.Image, result.Type);
            Assert.NotNull(result.MediaId);
            Assert.Equal(DateTime.Now.Year, result.CreateTime.Year);
        }

        [Fact]
        public void AddArmTemporary()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetAmrBytes(), MaterialType.Voice);
            Assert.Equal(MaterialType.Voice, result.Type);
            Assert.NotNull(result.MediaId);
            Assert.Equal(DateTime.Now.Year, result.CreateTime.Year);
        }

        [Fact]
        public void AddMp3TemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetMp3Bytes(), MaterialType.Voice);
            Assert.Equal(MaterialType.Voice, result.Type);
            Assert.NotNull(result.MediaId);
            Assert.Equal(DateTime.Now.Year, result.CreateTime.Year);
        }

        [Fact]
        public void AddMp4TemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetMp4Bytes(), MaterialType.Video);
            Assert.Equal(MaterialType.Video, result.Type);
            Assert.NotNull(result.MediaId);
            Assert.Equal(DateTime.Now.Year, result.CreateTime.Year);
        }

        [Fact]
        public void GetTemporaryTest()
        {
            var result = _materialService.AddTemporary(ApiTestHelper.GetJpgBytes(), MaterialType.Image);
            var model = _materialService.GetTemporary(result.MediaId);

            Assert.Equal("image/jpeg", model.ContentType);
            Assert.NotNull(model.FileName);
            Assert.True(model.Data.Length > 0);
        }

        #endregion Test Method
    }
}