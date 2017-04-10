using Rabbit.WeiXin.MP.Api.CustomMenu;
using Rabbit.WeiXin.MP.Api.User;
using System;
using System.Linq;
using Xunit;

namespace Rabbit.WeiXin.Tests
{
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

        [Fact]
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

            Assert.Equal(5, topMenu2.Childs.Length);
            Assert.Equal("View", topMenu2.Childs.First().Name);

            _customMenuService.Set(new CustomMenuButtonBase[] { topMenu1, topMenu2, topMenu3 });
        }

        [Fact]
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
            Assert.True(result.HasValue);

            result = _customMenuService.Set(new CustomMenuButtonBase[] { menu2 }, new CustomMeunMatchRule
            {
                Sex = SexEnum.Female
            });
            Assert.True(result.HasValue);

            Assert.Equal(menu1.Name, _customMenuService.GetList(OpenId).First().Name);
        }

        [Fact]
        public void GetOutTest()
        {
            var test = new Action<CustomMenuButtonBase[]>(list =>
            {
                Assert.NotNull(list);
                Assert.True(list.Length > 0);
                Assert.False(string.IsNullOrWhiteSpace(list.First().Name));
            });
            test(_customMenuService.GetList());
            test(_customMenuService.GetList(OpenId));
        }

        [Fact]
        public void GetTest()
        {
            var model = _customMenuService.Get();
            Assert.NotNull(model);
            Assert.NotNull(model.DefaultMenu);
            Assert.Null(model.DefaultMenu.MatchRule);
            Assert.NotNull(model.DefaultMenu.MenuId);
        }

        [Fact]
        public void DeleteTest()
        {
            _customMenuService.Delete();

            try
            {
                _customMenuService.GetList();
                Assert.True(false);
            }
            catch (WeiXinException exception)
            {
                Assert.Equal(46003, exception.ErrorCode);
            }
        }

        [Fact]
        public void DeleteByMenuIdTest()
        {
            var menuModel = _customMenuService.Get();
            if (menuModel.ConditionalMenus == null || !menuModel.ConditionalMenus.Any())
                return;
            var menu = menuModel.ConditionalMenus.First();
            _customMenuService.DeleteByMenuId(menu.MenuId);

            var menus = _customMenuService.Get().ConditionalMenus;
            if (menus != null && menus.Any(i => i.MenuId == menu.MenuId))
                Assert.True(false);
        }

        #endregion Test Method
    }
}