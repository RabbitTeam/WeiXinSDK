using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin;
using Rabbit.WeiXin.MP.Api.CustomMenu;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class CustomMenuServiceTest : ApiTestBase
    {
        #region Field

        private readonly ICustomMenuService _customMenuService;

        #endregion Field

        #region Constructor

        public CustomMenuServiceTest()
        {
            _customMenuService = new CustomMenuService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void SetTest()
        {
            var topMenu1 = new CustomMenuTopButton("菜单1");
            var topMenu2 = new CustomMenuTopButton("菜单2");
            var topMenu3 = new CustomMenuKeyButton("菜单3", CustomMenuType.PicSysphoto, "test");

            topMenu1.AppendChildMenus(new CustomMenuButton[]
            {
                new CustomMenuKeyButton("ScancodePush",CustomMenuType.ScancodePush, "test"),
                new CustomMenuKeyButton("ScancodeWaitmsg",CustomMenuType.ScancodeWaitmsg, "test"),
            });

            topMenu2.AppendChildMenus(new CustomMenuButton[]
            {
                new CustomMenuKeyButton("Click",CustomMenuType.Click, "test"),
                new CustomMenuViewButton("View", "http://cn.bing.com/"),
                new CustomMenuKeyButton("LocationSelect",CustomMenuType.LocationSelect, "test"),
                new CustomMenuKeyButton("PicPhotoOrAlbum",CustomMenuType.PicPhotoOrAlbum, "test"),
                new CustomMenuKeyButton("PicSysphoto",CustomMenuType.PicSysphoto, "test"),
                new CustomMenuKeyButton("PicWeixin",CustomMenuType.PicWeixin, "test")
            });

            Assert.AreEqual(5, topMenu2.Childs.Length);
            Assert.AreEqual("View", topMenu2.Childs.First().Name);

            _customMenuService.Set(new CustomMenuButtonBase[] { topMenu1, topMenu2, topMenu3 });
        }

        [TestMethod]
        public void GetTest()
        {
            var list = _customMenuService.GetList();
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Length > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(list.First().Name));
        }

        [TestMethod]
        public void DeleteTest()
        {
            _customMenuService.Delete();

            try
            {
                _customMenuService.GetList();
                Assert.Fail();
            }
            catch (WeiXinException exception)
            {
                Assert.AreEqual(46003, exception.ErrorCode);
            }
        }

        #endregion Test Method
    }
}