using Rabbit.WeiXin.MP.Api.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Rabbit.WeiXin.Tests
{
    public class UserTagServiceTest : ApiTestBase
    {
        #region Field

        private readonly IUserTagService _userTagService;

        #endregion Field

        #region Constructor

        public UserTagServiceTest()
        {
            _userTagService = new UserTagService(AccountModel);
        }

        #endregion Constructor


        #region Test Method

        [Fact]
        public void CreateTagTest()
        {
            var tagInfo = _userTagService.Create("Test");

            Assert.NotNull(tagInfo);
            Assert.Equal(tagInfo.Name, "Test");
            Assert.Equal(tagInfo.Count, 0);
        }

        [Fact]
        public void GetTagTest()
        {
            var tagInfo = _userTagService.Get();

            Assert.NotNull(tagInfo);
            Assert.True(tagInfo.Any());
        }

        [Fact]
        public void UpdateTagTest()
        {
            var tagInfo = _userTagService.Create("Test");
            Assert.NotNull(tagInfo);

            var updateTagInfo = new TagInfo { Id = tagInfo.Id, Name = "Test1" };
            _userTagService.Update(updateTagInfo);

            var updated = _userTagService.Get().FirstOrDefault(p => p.Id == tagInfo.Id);
            Assert.NotNull(updated);
            Assert.Equal(updated.Name, updateTagInfo.Name);
        }

        [Fact]
        public void DeleteTagTest()
        {
            var tagInfo = _userTagService.Create("TestAdd");
            var tagInfos = _userTagService.Get();
            var tt = tagInfos.FirstOrDefault(p => p.Name == tagInfo.Name);
            Assert.NotNull(tt);
            _userTagService.Delete(tt.Id);
            var tagInfos1 = _userTagService.Get();
            Assert.False(tagInfos1.Any(p => p.Name == tagInfo.Name));
        }

        [Fact]
        public void BatchTaggingTest()
        {
            var tags = _userTagService.Get();
            var tag = tags.FirstOrDefault();
            _userTagService.BatchTagging(tag.Id, new string[] { OpenId });
            var tt = _userTagService.Get(tag.Id);
            Assert.True(tt.Data.OpenIds.Contains(OpenId));
        }

        [Fact]
        public void BatchUntaggingTest()
        {
            var tags = _userTagService.Get();
            var tag = tags.FirstOrDefault();
            _userTagService.BatchTagging(tag.Id, new string[] { OpenId });
            var tt = _userTagService.Get(tag.Id);
            Assert.True(tt.Data.OpenIds.Contains(OpenId));

            _userTagService.BatchUntagging(tag.Id, new string[] { OpenId });

            tt = _userTagService.Get(tag.Id);
            Assert.False(tt.Data.OpenIds.Contains(OpenId));
        }

        [Fact]
        public void GetIdListTest()
        {
            var result = _userTagService.GetIdList(OpenId);
            Assert.NotEqual(result.Count, 0);
        }

        #endregion Test Method
    }
}
