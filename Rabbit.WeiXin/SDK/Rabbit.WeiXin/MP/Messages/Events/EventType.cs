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
        KF_Switch_Session,

        /// <summary>
        /// 卡券通过审核。
        /// </summary>
        Card_Pass_Check,

        /// <summary>
        /// 卡券不通过审核。
        /// </summary>
        Card_Not_Pass_Check,

        /// <summary>
        /// 用户在领取卡券时。
        /// </summary>
        Card_User_Get,

        /// <summary>
        /// 用户在删除卡券时。
        /// </summary>
        Card_User_Delete,

        /// <summary>
        /// 卡券被核销时。
        /// </summary>
        Card_User_Consume,

        /// <summary>
        /// 用户在进入会员卡时。
        /// </summary>
        Card_User_View,

        /// <summary>
        /// 用户在卡券里点击查看公众号进入会话时（需要用户已经关注公众号）。
        /// </summary>
        Card_UserEnterSession
    }
}