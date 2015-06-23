using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api.CustomMenu
{
    /// <summary>
    /// 一个抽象的自定义菜单服务。
    /// </summary>
    public interface ICustomMenuService
    {
        /// <summary>
        /// 设置自定义菜单。
        /// </summary>
        /// <param name="menus">自定义菜单数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="menus"/> 为null。</exception>
        /// <exception cref="ArgumentException"><paramref name="menus"/> 长度超过3。</exception>
        void Set(CustomMenuButtonBase[] menus);

        /// <summary>
        /// 获取自定义菜单信息。
        /// </summary>
        /// <returns>自定义菜单数组。</returns>
        CustomMenuButtonBase[] GetList();

        /// <summary>
        /// 删除自定义菜单。
        /// </summary>
        void Delete();
    }

    /// <summary>
    /// 自定义菜单服务实现。
    /// </summary>
    public sealed class CustomMenuService : ICustomMenuService
    {
        #region Field

        private readonly AccountModel _accountModel;

        private static readonly IDictionary<CustomMenuType, string> CustomMenuTypeMappings = new Dictionary<CustomMenuType, string>
        {
            {CustomMenuType.Click, "click"},
            {CustomMenuType.View, "view"},
            {CustomMenuType.ScancodePush, "scancode_push"},
            {CustomMenuType.ScancodeWaitmsg, "scancode_waitmsg"},
            {CustomMenuType.PicSysphoto, "pic_sysphoto"},
            {CustomMenuType.PicPhotoOrAlbum, "pic_photo_or_album"},
            {CustomMenuType.PicWeixin, "pic_weixin"},
            {CustomMenuType.LocationSelect, "location_select"},
            {CustomMenuType.MediaId, "media_id"},
            {CustomMenuType.ViewLimited, "view_limited"}
        };

        #endregion Field

        #region Constructor

        public CustomMenuService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of ICustomMenuService

        /// <summary>
        /// 设置自定义菜单。
        /// </summary>
        /// <param name="menus">自定义菜单数组。</param>
        /// <exception cref="ArgumentNullException"><paramref name="menus"/> 为null。</exception>
        /// <exception cref="ArgumentException"><paramref name="menus"/> 长度超过3。</exception>
        public void Set(CustomMenuButtonBase[] menus)
        {
            if (menus.NotNull("menus").Length > 3)
                throw new ArgumentException("顶级菜单项不能超过3个。", "menus");

            var url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new { button = menus.Select(GetMenuItem) });
        }

        /// <summary>
        /// 获取自定义菜单信息。
        /// </summary>
        /// <returns>自定义菜单数组。</returns>
        public CustomMenuButtonBase[] GetList()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" + _accountModel.GetAccessToken();

            var content = WeiXinHttpHelper.GetString(url);

            var buttons = (JArray)JObject.Parse(content)["menu"]["button"];

            return buttons.Select(token => GetMenuItemByWeiXin((JObject)token)).ToArray();
        }

        /// <summary>
        /// 删除自定义菜单。
        /// </summary>
        public void Delete()
        {
            WeiXinHttpHelper.GetString("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token=" +
                                       _accountModel.GetAccessToken());
        }

        #endregion Implementation of ICustomMenuService

        #region Private Method

        private static CustomMenuButtonBase GetMenuItemByWeiXin(JObject obj)
        {
            var name = obj.Value<string>("name");
            var typeString = obj.Value<string>("type");
            var customMenuType = string.IsNullOrWhiteSpace(typeString) ? null : new CustomMenuType?(CustomMenuTypeMappings.Single(i => i.Value.Equals(typeString)).Key);
            var subButtons = (JArray)obj["sub_button"];
            if (!customMenuType.HasValue)
            {
                var menu = new CustomMenuTopButton(name);
                menu.AppendChildMenus(subButtons.Select(token => GetMenuItemByWeiXin((JObject)token)).OfType<CustomMenuButton>().ToArray());
                return menu;
            }

            var type = customMenuType.Value;
            switch (type)
            {
                case CustomMenuType.Click:
                case CustomMenuType.ScancodePush:
                case CustomMenuType.ScancodeWaitmsg:
                case CustomMenuType.PicSysphoto:
                case CustomMenuType.PicPhotoOrAlbum:
                case CustomMenuType.PicWeixin:
                case CustomMenuType.LocationSelect:
                    {
                        return new CustomMenuKeyButton(name, type, obj.Value<string>("key"));
                    }
                case CustomMenuType.View:
                    {
                        return new CustomMenuViewButton(name, obj.Value<string>("url"));
                    }
                case CustomMenuType.MediaId:
                case CustomMenuType.ViewLimited:
                    {
                        return new CustomMenuMediaButton(name, type, obj.Value<string>("media_id"));
                    }
                default:
                    throw new NotSupportedException("不支持的类型：" + type);
            }
        }

        private static object GetMenuItem(CustomMenuButtonBase menuButtonBase)
        {
            var customMenuTopButton = menuButtonBase as CustomMenuTopButton;
            if (customMenuTopButton != null)
            {
                var menu = customMenuTopButton;
                return new
                {
                    name = menu.Name,
                    sub_button = menu.Childs.Select(GetMenuItem).ToArray()
                };
            }

            var menuButton = (CustomMenuButton)menuButtonBase;
            switch (menuButton.Type)
            {
                case CustomMenuType.Click:
                case CustomMenuType.ScancodePush:
                case CustomMenuType.ScancodeWaitmsg:
                case CustomMenuType.PicSysphoto:
                case CustomMenuType.PicPhotoOrAlbum:
                case CustomMenuType.PicWeixin:
                case CustomMenuType.LocationSelect:
                    {
                        var menu = (CustomMenuKeyButton)menuButton;
                        return new
                        {
                            type = CustomMenuTypeMappings[menu.Type],
                            name = menu.Name,
                            key = menu.Key
                        };
                    }
                case CustomMenuType.View:
                    {
                        var menu = (CustomMenuViewButton)menuButton;
                        return new
                        {
                            type = CustomMenuTypeMappings[menu.Type],
                            name = menu.Name,
                            url = menu.Url
                        };
                    }
                case CustomMenuType.MediaId:
                case CustomMenuType.ViewLimited:
                    {
                        var menu = (CustomMenuMediaButton)menuButton;
                        return new
                        {
                            type = CustomMenuTypeMappings[menu.Type],
                            name = menu.Name,
                            media_id = menu.MediaId
                        };
                    }
                default:
                    throw new NotSupportedException("不支持的类型：" + menuButton.Type);
            }
        }

        #endregion Private Method
    }

    #region Help Class

    /// <summary>
    /// 自定义菜单按钮基类。
    /// </summary>
    public abstract class CustomMenuButtonBase
    {
        /// <summary>
        /// 初始化一个新的自定义菜单。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        protected CustomMenuButtonBase(string name)
        {
            Name = name.NotEmptyOrWhiteSpace("name");
            if (this is CustomMenuTopButton)
            {
                if (Name.Length > 16)
                    throw new ArgumentException("顶级菜单标题长度不能超过16个字节。", "name");
            }
            else
            {
                if (Name.Length > 40)
                    throw new ArgumentException("子级菜单标题长度不能超过40个字节。", "name");
            }
        }

        /// <summary>
        /// 菜单标题，不超过16个字节，子菜单不超过40个字节
        /// </summary>
        [Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// 顶级自定义菜单按钮。
    /// </summary>
    public sealed class CustomMenuTopButton : CustomMenuButtonBase
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的自定义菜单。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        public CustomMenuTopButton(string name)
            : base(name)
        {
        }

        #endregion Constructor

        /// <summary>
        /// 二级菜单数组，个数应为1~5个
        /// </summary>
        public CustomMenuButton[] Childs { get; private set; }

        #region Public Method

        /// <summary>
        /// 清空所有子级菜单。
        /// </summary>
        public void ClearChildMenus()
        {
            Childs = null;
        }

        /// <summary>
        /// 追加子级菜单（子级菜单最多为5个，如果超出则替换掉旧的数据）。
        /// </summary>
        /// <param name="menus">菜单项。</param>
        public void AppendChildMenus(params CustomMenuButton[] menus)
        {
            menus.NotNull("menus");

            var result = Childs == null ? menus : Childs.Concat(menus).ToArray();

            var count = result.Count();
            const ushort maxCount = 5;

            //如果大于5则删掉前几条数据保证最后能取出5条数据，否则直接获取所有的。
            Childs = count > maxCount ? result.Skip(count - maxCount).Take(maxCount).ToArray() : result.ToArray();
        }

        #endregion Public Method
    }

    /// <summary>
    /// 一个基础的自定义菜单按钮。
    /// </summary>
    public abstract class CustomMenuButton : CustomMenuButtonBase
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的自定义菜单。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        protected CustomMenuButton(string name)
            : base(name)
        {
        }

        #endregion Constructor

        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public abstract CustomMenuType Type { get; }
    }

    /// <summary>
    /// 带有事件Key的自定义菜单按钮。
    /// </summary>
    public sealed class CustomMenuKeyButton : CustomMenuButton
    {
        #region Field

        private readonly CustomMenuType _type;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的带有事件Key的自定义菜单按钮。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <param name="type">菜单类型。</param>
        /// <param name="key">事件key。</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> 为 <see cref="CustomMenuType.View"/> 或 <see cref="CustomMenuType.MediaId"/> 或 <see cref="CustomMenuType.ViewLimited"/>。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为空。</exception>
        public CustomMenuKeyButton(string name, CustomMenuType type, string key)
            : base(name)
        {
            switch (type)
            {
                case CustomMenuType.View:
                case CustomMenuType.MediaId:
                case CustomMenuType.ViewLimited:
                    throw new NotSupportedException("不支持的类型：" + type);
            }
            _type = type;
            Key = key.NotEmptyOrWhiteSpace("key");
        }

        #endregion Constructor

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节
        /// </summary>
        [Required, StringLength(128)]
        public string Key { get; set; }

        #region Overrides of CustomMenuButton

        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public override CustomMenuType Type { get { return _type; } }

        #endregion Overrides of CustomMenuButton
    }

    /// <summary>
    /// 跳转行为的自定义菜单按钮。
    /// </summary>
    public sealed class CustomMenuViewButton : CustomMenuButton
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的跳转行为的自定义菜单按钮。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <param name="url">网页链接，用户点击菜单可打开链接，不超过256字节。</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> 为空 或 超过 256 个字节 或者不是一个有效的url。</exception>
        public CustomMenuViewButton(string name, string url)
            : base(name)
        {
            Url = url.NotEmptyOrWhiteSpace("url");
            if (url.Length > 256)
                throw new ArgumentException("url 的长度不能超过256个字节。", "url");
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException(url + " 不是一个有效的url地址。", "url");
        }

        #endregion Constructor

        /// <summary>
        /// 网页链接，用户点击菜单可打开链接，不超过256字节
        /// </summary>
        [Required, StringLength(256)]
        public string Url { get; set; }

        #region Overrides of CustomMenuButton

        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public override CustomMenuType Type { get { return CustomMenuType.View; } }

        #endregion Overrides of CustomMenuButton
    }

    /// <summary>
    /// 使用永久素材的自定义菜单按钮。
    /// </summary>
    public sealed class CustomMenuMediaButton : CustomMenuButton
    {
        #region Field

        private readonly CustomMenuType _type;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的使用永久素材的自定义菜单按钮。
        /// </summary>
        /// <param name="name">菜单标题，不超过16个字节，子菜单不超过40个字节</param>
        /// <param name="type">菜单类型。</param>
        /// <param name="mediaId">调用新增永久素材接口返回的合法media_id。</param>
        /// <exception cref="ArgumentException">顶级菜单标题长度不能超过16个字节。</exception>
        /// <exception cref="ArgumentException">子级菜单标题长度不能超过40个字节。</exception>
        /// <exception cref="NotSupportedException"><paramref name="type"/> 不为 <see cref="CustomMenuType.MediaId"/> 或 <see cref="CustomMenuType.ViewLimited"/>。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="mediaId"/> 为空。</exception>
        public CustomMenuMediaButton(string name, CustomMenuType type, string mediaId)
            : base(name)
        {
            switch (type)
            {
                case CustomMenuType.MediaId:
                case CustomMenuType.ViewLimited:
                    break;

                default:
                    throw new NotSupportedException("不支持的类型：" + type);
            }
            _type = type;
            MediaId = mediaId.NotEmptyOrWhiteSpace("mediaId");
        }

        #endregion Constructor

        /// <summary>
        /// 调用新增永久素材接口返回的合法media_id
        /// </summary>
        [Required]
        public string MediaId { get; set; }

        #region Overrides of CustomMenuButton

        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        public override CustomMenuType Type { get { return _type; } }

        #endregion Overrides of CustomMenuButton
    }

    /// <summary>
    /// 自定义菜单类型。
    /// </summary>
    public enum CustomMenuType
    {
        /// <summary>
        /// 点击。
        /// </summary>
        /// <remarks>
        /// 用户点击click类型按钮后，微信服务器会通过消息接口推送消息类型为event	的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互；
        /// </remarks>
        Click = 0,

        /// <summary>
        /// 跳转Url。
        /// </summary>
        /// <remarks>
        /// 用户点击view类型按钮后，微信客户端将会打开开发者在按钮中填写的网页URL，可与网页授权获取用户基本信息接口结合，获得用户基本信息。
        /// </remarks>
        View = 1,

        /// <summary>
        /// 扫描二维码。
        /// </summary>
        /// <remarks>
        /// 用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后显示扫描结果（如果是URL，将进入URL），且会将扫码的结果传给开发者，开发者可以下发消息。
        /// </remarks>
        ScancodePush = 2,

        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框
        /// </summary>
        /// <remarks>
        /// 用户点击按钮后，微信客户端将调起扫一扫工具，完成扫码操作后，将扫码的结果传给开发者，同时收起扫一扫工具，然后弹出“消息接收中”提示框，随后可能会收到开发者下发的消息。
        /// </remarks>
        ScancodeWaitmsg = 3,

        /// <summary>
        /// 弹出系统拍照发图
        /// </summary>
        /// <remarks>
        /// 用户点击click类型按钮后，微信服务器会通过消息接口推送消息类型为event	的结构给开发者（参考消息接口指南），并且带上按钮中开发者填写的key值，开发者可以通过自定义的key值与用户进行交互；
        /// </remarks>
        PicSysphoto = 4,

        /// <summary>
        /// 弹出拍照或者相册发图
        /// </summary>
        /// <remarks>
        /// 用户点击按钮后，微信客户端将调起系统相机，完成拍照操作后，会将拍摄的相片发送给开发者，并推送事件给开发者，同时收起系统相机，随后可能会收到开发者下发的消息。
        /// </remarks>
        PicPhotoOrAlbum = 5,

        /// <summary>
        /// 弹出微信相册发图器
        /// </summary>
        /// <remarks>
        /// 用户点击按钮后，微信客户端将调起微信相册，完成选择操作后，将选择的相片发送给开发者的服务器，并推送事件给开发者，同时收起相册，随后可能会收到开发者下发的消息。
        /// </remarks>
        PicWeixin = 6,

        /// <summary>
        /// 弹出地理位置选择器
        /// </summary>
        /// <remarks>
        /// 用户点击按钮后，微信客户端将调起地理位置选择工具，完成选择操作后，将选择的地理位置发送给开发者的服务器，同时收起位置选择工具，随后可能会收到开发者下发的消息。
        /// </remarks>
        LocationSelect = 7,

        /// <summary>
        /// 下发消息（除文本消息）
        /// </summary>
        /// <remarks>
        /// 用户点击media_id类型按钮后，微信服务器会将开发者填写的永久素材id对应的素材下发给用户，永久素材类型可以是图片、音频、视频、图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id。
        /// </remarks>
        MediaId = 8,

        /// <summary>
        /// 跳转图文消息URL
        /// </summary>
        /// <remarks>
        /// 用户点击view_limited类型按钮后，微信客户端将打开开发者在按钮中填写的永久素材id对应的图文消息URL，永久素材类型只支持图文消息。请注意：永久素材id必须是在“素材管理/新增永久素材”接口上传后获得的合法id。
        /// </remarks>
        ViewLimited = 9
    }

    #endregion Help Class
}