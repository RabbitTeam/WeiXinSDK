using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using Rabbit.WeiXin.MP.Messages.Events.CustomService;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.MP.Messages.Response;
using System;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 一个抽象的消息处理中间件。
    /// </summary>
    public abstract class MessageHandlerMiddleware : HandlerMiddleware
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        protected MessageHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        #endregion Constructor

        #region Property

        /// <summary>
        /// 处理上下文。
        /// </summary>
        protected IHandlerContext Context { get; private set; }

        #endregion Property

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            Context = context;

            var requestMessage = context.GetRequestMessage();
             
            IResponseMessage responseMessage;
            switch (requestMessage.MessageType)
            {
                case RequestMessageType.Event:
                    responseMessage = OnEventRequest(requestMessage as EventMessageBase);
                    break;

                case RequestMessageType.Image:
                    responseMessage = OnImageRequest(requestMessage as RequestMessageImage);
                    break;

                case RequestMessageType.Link:
                    responseMessage = OnLinkRequest(requestMessage as RequestMessageLink);
                    break;

                case RequestMessageType.Location:
                    responseMessage = OnLocationRequest(requestMessage as RequestMessageLocation);
                    break;

                case RequestMessageType.ShortVideo:
                    responseMessage = OnShortVideoRequest(requestMessage as RequestMessageShortVideo);
                    break;

                case RequestMessageType.Text:
                    responseMessage = OnTextRequest(requestMessage as RequestMessageText);
                    break;

                case RequestMessageType.Video:
                    responseMessage = OnVideoRequest(requestMessage as RequestMessageVideo);
                    break;

                case RequestMessageType.Voice:
                    responseMessage = OnVoiceRequest(requestMessage as RequestMessageVoice);
                    break;

                default:
                    throw new NotSupportedException("不支持的请求消息类型：" + requestMessage.MessageType);
            }

            if (responseMessage != null)
            {
                //基本信息初始化。
                responseMessage.CreateTime = DateTime.Now;
                responseMessage.FromUserName = requestMessage.ToUserName;
                responseMessage.ToUserName = requestMessage.FromUserName;
            } 

            context.SetResponseMessage(responseMessage);

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware

        #region Receive Message
         
        /// <summary>
        /// 文字类型请求
        /// </summary>
        public virtual IResponseMessage OnTextRequest(RequestMessageText requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 位置类型请求
        /// </summary>
        public virtual IResponseMessage OnLocationRequest(RequestMessageLocation requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 图片类型请求
        /// </summary>
        public virtual IResponseMessage OnImageRequest(RequestMessageImage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 语音类型请求
        /// </summary>
        public virtual IResponseMessage OnVoiceRequest(RequestMessageVoice requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 视频类型请求
        /// </summary>
        public virtual IResponseMessage OnVideoRequest(RequestMessageVideo requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 链接消息类型请求
        /// </summary>
        public virtual IResponseMessage OnLinkRequest(RequestMessageLink requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 链接消息类型请求
        /// </summary>
        public virtual IResponseMessage OnShortVideoRequest(RequestMessageShortVideo requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求
        /// </summary>
        public virtual IResponseMessage OnEventRequest(EventMessageBase requestMessage)
        {
            IResponseMessage responseMessage;
            switch (requestMessage.EventType)
            {
                case EventType.Click:
                    responseMessage = OnEvent_ClickRequest(requestMessage as ClickMessage);
                    break;

                case EventType.KF_Close_Session:
                    responseMessage = OnEvent_Kf_Close_SessionRequest(requestMessage as CloseSessionMessage);
                    break;

                case EventType.KF_Create_Session:
                    responseMessage = OnEvent_Kf_Create_SessionRequest(requestMessage as CreateSessionMessage);
                    break;

                case EventType.KF_Switch_Session:
                    responseMessage = OnEvent_Kf_Switch_SessionRequest(requestMessage as SwitchSessionMessage);
                    break;

                case EventType.Location:
                    responseMessage = OnEvent_LocationRequest(requestMessage as ReportingLocationEventMessage);
                    break;

                case EventType.Location_Select:
                    responseMessage = OnEvent_LocationSelectRequest(requestMessage as LocationSelectMessage);
                    break;

                case EventType.MassSendJobFinish:
                    responseMessage = OnEvent_MassSendJobFinishRequest(requestMessage as MassSendPushMessage);
                    break;

                case EventType.Pic_Photo_Or_Album:
                    responseMessage = OnEvent_PicPhotoOrAlbumRequest(requestMessage as PicPhotoOrAlbumMessage);
                    break;

                case EventType.Pic_SysPhoto:
                    responseMessage = OnEvent_PicSysphotoRequest(requestMessage as PicSysPhotoMessage);
                    break;

                case EventType.Pic_WeiXin:
                    responseMessage = OnEvent_PicWeixinRequest(requestMessage as PicWeiXinMessage);
                    break;

                case EventType.Scan:
                    responseMessage = OnEvent_ScanRequest(requestMessage as QrAlreadySubscribeEventKeyMessage);
                    break;

                case EventType.ScanCode_Push:
                    OnEvent_ScancodePushRequest(requestMessage as ScanCodePushMessage);
                    responseMessage = null;
                    break;

                case EventType.ScanCode_WaitMsg:
                    responseMessage = OnEvent_ScancodeWaitmsgRequest(requestMessage as ScanCodeWaitMessage);
                    break;

                case EventType.Subscribe:
                    if (requestMessage is SubscribeEventMessage)
                    {
                        responseMessage = OnEvent_SubscribeRequest(requestMessage as SubscribeEventMessage);
                    }
                    else
                    {
                        responseMessage = OnEvent_QrSubscribeRequest(requestMessage as QrSubscribeEventKeyMessage);
                    }
                    break;

                case EventType.TemplateSendJobFinish:
                    responseMessage = OnEvent_TemplateSendJobFinishRequest(requestMessage as TemplateMessageSendPushMessage);
                    break;

                case EventType.UnSubscribe:
                    responseMessage = OnEvent_UnsubscribeRequest(requestMessage as UnSubscribeEventMessage);
                    break;

                case EventType.View:
                    responseMessage = OnEvent_ViewRequest(requestMessage as ViewMessage);
                    break;

                default:
                    throw new NotSupportedException("不支持的请求事件类型：" + requestMessage.EventType);
            }
            return responseMessage;
        }

        #region Event Push

        /// <summary>
        /// Event事件类型请求之LOCATION
        /// </summary>
        public virtual IResponseMessage OnEvent_LocationRequest(ReportingLocationEventMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求之QrSubscribe（通过二维码关注事件）
        /// </summary>
        public virtual IResponseMessage OnEvent_QrSubscribeRequest(QrSubscribeEventKeyMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求之subscribe
        /// </summary>
        public virtual IResponseMessage OnEvent_SubscribeRequest(SubscribeEventMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求之unsubscribe
        /// </summary>
        public virtual IResponseMessage OnEvent_UnsubscribeRequest(UnSubscribeEventMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求之CLICK
        /// </summary>
        public virtual IResponseMessage OnEvent_ClickRequest(ClickMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// Event事件类型请求之scan
        /// </summary>
        public virtual IResponseMessage OnEvent_ScanRequest(QrAlreadySubscribeEventKeyMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件之URL跳转视图（View）
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_ViewRequest(ViewMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 事件推送群发结果
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_MassSendJobFinishRequest(MassSendPushMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 发送模板消息返回结果
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_TemplateSendJobFinishRequest(TemplateMessageSendPushMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 弹出拍照或者相册发图
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_PicPhotoOrAlbumRequest(PicPhotoOrAlbumMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 扫码推事件
        /// </summary>
        /// <returns></returns>
        public virtual void OnEvent_ScancodePushRequest(ScanCodePushMessage requestMessage)
        {
        }

        /// <summary>
        /// 扫码推事件且弹出“消息接收中”提示框
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_ScancodeWaitmsgRequest(ScanCodeWaitMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 弹出地理位置选择器
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_LocationSelectRequest(LocationSelectMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 弹出微信相册发图器
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_PicWeixinRequest(PicWeiXinMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 弹出系统拍照发图
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_PicSysphotoRequest(PicSysPhotoMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 多客服接入会话
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_Kf_Create_SessionRequest(CreateSessionMessage requestMessage)
        {
            if (requestMessage == null) throw new ArgumentNullException("requestMessage");
            return null;
        }

        /// <summary>
        /// 多客服关闭会话
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_Kf_Close_SessionRequest(CloseSessionMessage requestMessage)
        {
            return null;
        }

        /// <summary>
        /// 多客服转接会话
        /// </summary>
        /// <returns></returns>
        public virtual IResponseMessage OnEvent_Kf_Switch_SessionRequest(SwitchSessionMessage requestMessage)
        {
            return null;
        }

        #endregion Event Push

        #endregion Receive Message
    }
}