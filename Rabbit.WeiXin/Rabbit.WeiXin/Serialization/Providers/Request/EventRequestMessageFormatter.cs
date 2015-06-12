using Rabbit.WeiXin.Messages;
using Rabbit.WeiXin.Messages.Events;
using Rabbit.WeiXin.Serialization.Providers.Event;
using Rabbit.WeiXin.Serialization.Providers.Event.CustomMenu;
using Rabbit.WeiXin.Serialization.Providers.Event.CustomService;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Rabbit.WeiXin.Serialization.Providers.Request
{
    /// <summary>
    /// 事件请求消息格式化器。
    /// </summary>
    internal sealed class EventRequestMessageFormatter : IMessageFormatter
    {
        #region Field

        private static readonly IDictionary<EventType, IMessageFormatter> Mappings = new Dictionary<EventType, IMessageFormatter>
        {
            {EventType.UnSubscribe, new UnSubscribeEventMessageFormatter()},
//            {EventType.Subscribe, new SubscribeEventMessageFormatter()},
            {EventType.Scan, new QrAlreadySubscribeEventKeyMessageFormatter()},
            {EventType.Location, new ReportingLocationEventMessageFormatter()},
            {EventType.Click, new ClickMessageFormatter()},
            {EventType.View, new ViewMessageFormatter()},
            {EventType.TemplateSendJobFinish, new TemplateMessageSendPushMessageFormatter()},
            {EventType.MassSendJobFinish, new MassSendPushMessageFormatter()},
            {EventType.ScanCode_Push, new ScanCodePushMessageFormatter()},
            {EventType.ScanCode_WaitMsg, new ScanCodeWaitMessageFormatter()},
            {EventType.Pic_SysPhoto, new PicSysPhotoMessageFormatter()},
            {EventType.Pic_Photo_Or_Album, new PicPhotoOrAlbumMessageFormatter()},
            {EventType.Pic_WeiXin, new PicWeiXinMessageFormatter()},
            {EventType.Location_Select, new LocationSelectMessageFormatter()},
            {EventType.KF_Create_Session, new CreateSessionMessageFormatter()},
            {EventType.KF_Close_Session, new CloseSessionMessageFormatter()},
            {EventType.KF_Switch_Session, new SwitchSessionMessageFormatter()}
        };

        private static readonly SubscribeEventMessageFormatter SubscribeEventMessageFormatter = new SubscribeEventMessageFormatter();
        private static readonly QrSubscribeEventKeyMessageFormatter QrSubscribeEventKeyMessageFormatter = new QrSubscribeEventKeyMessageFormatter();

        #endregion Field

        #region Implementation of IMessageFormatter

        public IMessageBase Deserialize(XContainer container)
        {
            var eventType = GetEventType(container);
            switch (eventType)
            {
                case EventType.Subscribe:
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

        private static EventType GetEventType(XContainer container)
        {
            return GetType<EventType>(container, "Event");
        }

        private static T GetType<T>(XContainer container, string elementName) where T : struct
        {
            var typeElemment = container.Element(elementName);
            if (typeElemment == null)
                throw new ArgumentException("找不到元素 MsgType。");

            var type = typeElemment.Value;

            T value;
            if (!Enum.TryParse(type, true, out value))
                throw new NotSupportedException(string.Format("无法将 {0} 转换为指定的类型 {1}。", type, typeof(T).FullName));

            return value;
        }

        #endregion Private Method
    }
}