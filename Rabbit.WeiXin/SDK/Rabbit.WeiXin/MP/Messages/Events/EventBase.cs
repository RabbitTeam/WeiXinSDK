using Rabbit.WeiXin.MP.Messages.Request;

namespace Rabbit.WeiXin.MP.Messages.Events
{
    /// <summary>
    /// 事件类型。
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 订阅。
        /// </summary>
        Subscribe,

        /// <summary>
        /// 取消订阅。
        /// </summary>
        UnSubscribe,

        /// <summary>
        /// 用户已关注。
        /// </summary>
        Scan,

        /// <summary>
        /// 上报地理位置。
        /// </summary>
        Location,

        /// <summary>
        /// 点击菜单拉取消息事件。
        /// </summary>
        Click,

        /// <summary>
        /// 点击菜单跳转事件。
        /// </summary>
        View,

        /// <summary>
        /// 模板消息发送。
        /// </summary>
        TemplateSendJobFinish,

        /// <summary>
        /// 群发消息发送。
        /// </summary>
        MassSendJobFinish,

        /// <summary>
        /// 扫码推事件的事件推送。
        /// </summary>
        ScanCode_Push,

        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框的事件推送
        /// </summary>
        ScanCode_WaitMsg,

        /// <summary>
        /// 弹出系统拍照发图的事件推送。
        /// </summary>
        Pic_SysPhoto,

        /// <summary>
        /// 弹出拍照或者相册发图的事件推送。
        /// </summary>
        Pic_Photo_Or_Album,

        /// <summary>
        /// 弹出微信相册发图器的事件推送。
        /// </summary>
        Pic_WeiXin,

        /// <summary>
        /// 弹出地理位置选择器的事件推送。
        /// </summary>
        Location_Select,

        /// <summary>
        /// 创建会话。
        /// </summary>
        KF_Create_Session,

        /// <summary>
        /// 关闭会话。
        /// </summary>
        KF_Close_Session,

        /// <summary>
        /// 转接会话。
        /// </summary>
        KF_Switch_Session
    }

    /// <summary>
    /// 事件消息基础。
    /// </summary>
    public abstract class EventMessageBase : MessageBase, IRequestMessageBase
    {
        #region Overrides of MessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public RequestMessageType MessageType
        {
            get { return RequestMessageType.Event; }
        }

        #endregion Overrides of MessageBase

        /// <summary>
        /// 事件类型。
        /// </summary>
        public abstract EventType EventType { get; }
    }

    /// <summary>
    /// 常见的事件消息基类。
    /// </summary>
    public abstract class EventKeyMessageBase : EventMessageBase
    {
        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// </summary>
        public string EventKey { get; set; }
    }

    /// <summary>
    /// 二维码事件消息基础。
    /// </summary>
    public abstract class QrEventKeyMessageBase : EventKeyMessageBase
    {
        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; set; }
    }
}