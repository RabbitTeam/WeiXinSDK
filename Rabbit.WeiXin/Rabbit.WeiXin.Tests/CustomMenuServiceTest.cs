using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.MP.Api.CustomMenu;
using Rabbit.WeiXin.MP.Api.User;
using System;
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
        public void SetPersonaliseBySexTest()
        {
            var menu1 = new CustomMenuTopButton("男生可见");
            menu1.AppendChildMenus(new CustomMenuKeyButton("ScancodePush", CustomMenuType.ScancodePush, "test"), new CustomMenuKeyButton("ScancodeWaitmsg", CustomMenuType.ScancodeWaitmsg, "test"));
            var menu2 = new CustomMenuTopButton("女生可见");
            menu2.AppendChildMenus(new CustomMenuKeyButton("ScancodePush", CustomMenuType.ScancodePush, "test"), new CustomMenuKeyButton("ScancodeWaitmsg", CustomMenuType.ScancodeWaitmsg, "test"));

            var result = _customMenuService.Set(new CustomMenuButtonBase[] { menu1 }, new CustomMeunMatchRule
            {
                Sex = SexEnum.Male
            });
            Assert.IsTrue(result.HasValue);

            result = _customMenuService.Set(new CustomMenuButtonBase[] { menu2 }, new CustomMeunMatchRule
            {
                Sex = SexEnum.Female
            });
            Assert.IsTrue(result.HasValue);

            Assert.AreEqual(menu1.Name, _customMenuService.GetList(OpenId).First().Name);
        }

        [TestMethod]
        public void GetOutTest()
        {
            var test = new Action<CustomMenuButtonBase[]>(list =>
            {
                Assert.IsNotNull(list);
                Assert.IsTrue(list.Length > 0);
                Assert.IsFalse(string.IsNullOrWhiteSpace(list.First().Name));
            });
            test(_customMenuService.GetList());
            test(_customMenuService.GetList(OpenId));
        }

        [TestMethod]
        public void GetTest()
        {
            var model = _customMenuService.Get();
            Assert.IsNotNull(model);
            Assert.IsNotNull(model.DefaultMenu);
            Assert.IsNull(model.DefaultMenu.MatchRule);
            Assert.IsNotNull(model.DefaultMenu.MenuId);
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

        [TestMethod]
        public void DeleteByMenuIdTest()
        {
            var menuModel = _customMenuService.Get();
            if (menuModel.ConditionalMenus == null || !menuModel.ConditionalMenus.Any())
                return;
            var menu = menuModel.ConditionalMenus.First();
            _customMenuService.DeleteByMenuId(menu.MenuId);

            var menus = _customMenuService.Get().ConditionalMenus;
            if (menus != null && menus.Any(i => i.MenuId == menu.MenuId))
                Assert.Fail();
        }

        #endregion Test Method
    }
}