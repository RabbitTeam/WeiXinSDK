using Rabbit.WeiXin.MP.Messages;
using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Serialization.Providers.Event;
using Rabbit.WeiXin.MP.Serialization.Providers.Event.Card;
using Rabbit.WeiXin.MP.Serialization.Providers.Event.CustomMenu;
using Rabbit.WeiXin.MP.Serialization.Providers.Event.CustomService;
using Rabbit.WeiXin.MP.Serialization.Providers.Event.WiFi;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Request
{
    /// <summary>
    /// 事件请求消息格式化器。
    /// </summary>
    internal sealed class EventRequestMessageFormatter : IMessageFormatter
    {
        #region Field

        private static readonly IDictionary<string, IMessageFormatter> Mappings = new Dictionary<string, IMessageFormatter>
        {
            {EventType.UnSubscribe.ToString().ToLower(), new UnSubscribeEventMessageFormatter()},
            //            {EventType.Subscribe, new SubscribeEventMessageFormatter()},
            {EventType.Scan.ToString().ToLower(), new QrAlreadySubscribeEventKeyMessageFormatter()},
            {EventType.Location.ToString().ToLower(), new ReportingLocationEventMessageFormatter()},
            {EventType.Click.ToString().ToLower(), new ClickMessageFormatter()},
            {EventType.View.ToString().ToLower(), new ViewMessageFormatter()},
            {EventType.TemplateSendJobFinish.ToString().ToLower(), new TemplateMessageSendPushMessageFormatter()},
            {EventType.MassSendJobFinish.ToString().ToLower(), new MassSendPushMessageFormatter()},
            {EventType.ScanCode_Push.ToString().ToLower(), new ScanCodePushMessageFormatter()},
            {EventType.ScanCode_WaitMsg.ToString().ToLower(), new ScanCodeWaitMessageFormatter()},
            {EventType.Pic_SysPhoto.ToString().ToLower(), new PicSysPhotoMessageFormatter()},
            {EventType.Pic_Photo_Or_Album.ToString().ToLower(), new PicPhotoOrAlbumMessageFormatter()},
            {EventType.Pic_WeiXin.ToString().ToLower(), new PicWeiXinMessageFormatter()},
            {EventType.Location_Select.ToString().ToLower(), new LocationSelectMessageFormatter()},
            {EventType.KF_Create_Session.ToString().ToLower(), new CreateSessionMessageFormatter()},
            {EventType.KF_Close_Session.ToString().ToLower(), new CloseSessionMessageFormatter()},
            {EventType.KF_Switch_Session.ToString().ToLower(), new SwitchSessionMessageFormatter()},
            {"card_pass_check", new CardEventPassCheckMessageFormatter()},
            {"card_not_pass_check", new CardEventNotPassCheckMessageFormatter()},
            {"user_consume_card", new CardEventUserConsumeCardMessageFormatter()},
            {"user_del_card", new CardEventUserDeleteMessageFormatter()},
            {"user_enter_session_from_card", new CardEventUserEnterSessionMessageFormatter()},
            {"user_get_card", new CardEventUserGetMessageFormatter()},
            {"user_view_card", new CardEventUserViewMessageFormatter()},
            {EventType.WifiConnected.ToString().ToLower(),new ConnectedMessageFormatter() }
        };

        private static readonly SubscribeEventMessageFormatter SubscribeEventMessageFormatter = new SubscribeEventMessageFormatter();
        private static readonly QrSubscribeEventKeyMessageFormatter QrSubscribeEventKeyMessageFormatter = new QrSubscribeEventKeyMessageFormatter();

        #endregion Field

        #region Implementation of IMessageFormatter

        public IMessageBase Deserialize(XContainer container)
        {
            var eventType = GetValue(container, "Event").ToLower();
            switch (eventType)
            {
                case "subscribe":
                    if (container.Element("Ticket") == null)
                        return SubscribeEventMessageFormatter.Deserialize(container);
                    return QrSubscribeEventKeyMessageFormatter.Deserialize(container);
            }

            if (!Mappings.ContainsKey(eventType))
                throw new NotSupportedException(string.Format("不支持的事件类型：{0}。", eventType));

            return Mappings[eventType].Deserialize(container);
        }

        public string Serialize(IMessageBase graph)
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IMessageFormatter

        #region Private Method

        private static string GetValue(XContainer container, string elementName)
        {
            var typeElemment = container.Element(elementName);
            if (typeElemment == null)
                throw new ArgumentException("找不到元素 MsgType。");

            return typeElemment.Value;
        }

        #endregion Private Method
    }
}