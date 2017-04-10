using Xunit;
using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.MP.Messages;
using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Messages.Events.Card;
using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using Rabbit.WeiXin.MP.Messages.Events.CustomService;
using Rabbit.WeiXin.MP.Messages.Events.WiFi;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.Utility;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    
    public class RequestMessageFormatter
    {
        #region Field

        private readonly IRequestMessageFactory _requestMessageFactory = DefaultDependencyResolver.Instance.GetService<IRequestMessageFactory>();

        #endregion Field

        [Fact]
        public void TextMessageTest()
        {
            const string xmlContent = @"<xml>
 <ToUserName><![CDATA[toUser]]></ToUserName>
 <FromUserName><![CDATA[fromUser]]></FromUserName>
 <CreateTime>1348831860</CreateTime>
 <MsgType><![CDATA[text]]></MsgType>
 <Content><![CDATA[this is a test]]></Content>
 <MsgId>1234567890123456</MsgId>
 </xml>";

            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageText>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(RequestMessageType.Text, model.MessageType);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1348831860), model.CreateTime);
            Assert.Equal("this is a test", model.Content);
            Assert.Equal(1234567890123456, model.MessageId);
        }

        [Fact]
        public void ImageMessageTest()
        {
            const string xmlContent = @" <xml>
 <ToUserName><![CDATA[toUser]]></ToUserName>
 <FromUserName><![CDATA[fromUser]]></FromUserName>
 <CreateTime>1348831860</CreateTime>
 <MsgType><![CDATA[image]]></MsgType>
 <PicUrl><![CDATA[http://www.chunsun.cc/1.jpg]]></PicUrl>
 <MediaId><![CDATA[1234567890-1234567]]></MediaId>
 <MsgId>1234567890123456</MsgId>
 </xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageImage>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1348831860), model.CreateTime);
            Assert.Equal(1234567890123456, model.MessageId);
            Assert.Equal("1234567890-1234567", model.MediaId);
            Assert.Equal(RequestMessageType.Image, model.MessageType);

            Assert.Equal(model.PicUrl, new Uri("http://www.chunsun.cc/1.jpg"));
        }

        [Fact]
        public void VoiceMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1357290913</CreateTime>
<MsgType><![CDATA[voice]]></MsgType>
<MediaId><![CDATA[1234567890-1234567]]></MediaId>
<Format><![CDATA[Format]]></Format>
<MsgId>1234567890123456</MsgId>
</xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageVoice>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.Equal(1234567890123456, model.MessageId);
            Assert.Equal("1234567890-1234567", model.MediaId);
            Assert.Equal(RequestMessageType.Voice, model.MessageType);

            Assert.Equal(model.Format, "Format");
        }

        [Fact]
        public void VideoMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1357290913</CreateTime>
<MsgType><![CDATA[video]]></MsgType>
<MediaId><![CDATA[1234567890-1234567]]></MediaId>
<ThumbMediaId><![CDATA[1234567890-12345678]]></ThumbMediaId>
<MsgId>1234567890123456</MsgId>
</xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageVideo>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.Equal(1234567890123456, model.MessageId);
            Assert.Equal("1234567890-1234567", model.MediaId);
            Assert.Equal(RequestMessageType.Video, model.MessageType);

            Assert.Equal("1234567890-12345678", model.ThumbMediaId);
        }

        [Fact]
        public void ShortVideoMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1357290913</CreateTime>
<MsgType><![CDATA[shortvideo]]></MsgType>
<MediaId><![CDATA[1234567890-1234567]]></MediaId>
<ThumbMediaId><![CDATA[1234567890-12345678]]></ThumbMediaId>
<MsgId>1234567890123456</MsgId>
</xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageShortVideo>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.Equal("1234567890-1234567", model.MediaId);
            Assert.Equal(RequestMessageType.ShortVideo, model.MessageType);

            Assert.Equal(model.ThumbMediaId, "1234567890-12345678");
        }

        [Fact]
        public void LocationMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1351776360</CreateTime>
<MsgType><![CDATA[location]]></MsgType>
<Location_X>23.134521</Location_X>
<Location_Y>113.358803</Location_Y>
<Scale>20</Scale>
<Label><![CDATA[位置信息]]></Label>
<MsgId>1234567890123456</MsgId>
</xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageLocation>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1351776360), model.CreateTime);
            Assert.Equal(1234567890123456, model.MessageId);
            Assert.Equal(RequestMessageType.Location, model.MessageType);

            Assert.Equal(23.134521, model.X);
            Assert.Equal(113.358803, model.Y);
            Assert.Equal("位置信息", model.Label);
            Assert.Equal(20, model.Scale);
        }

        [Fact]
        public void LinkMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1351776360</CreateTime>
<MsgType><![CDATA[link]]></MsgType>
<Title><![CDATA[公众平台官网链接]]></Title>
<Description><![CDATA[公众平台官网链接描述]]></Description>
<Url><![CDATA[http://www.chunsun.cc/]]></Url>
<MsgId>1234567890123456</MsgId>
</xml>";
            var model = _requestMessageFactory.CreateRequestMessage<RequestMessageLink>(xmlContent);

            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1351776360), model.CreateTime);
            Assert.Equal(1234567890123456, model.MessageId);
            Assert.Equal(RequestMessageType.Link, model.MessageType);

            Assert.Equal("公众平台官网链接", model.Title);
            Assert.Equal("公众平台官网链接描述", model.Description);
            Assert.Equal(new Uri("http://www.chunsun.cc/"), model.Url);
        }

        #region Event

        [Fact]
        public void SubscribeMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[subscribe]]></Event>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<SubscribeEventMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Subscribe, model.EventType);
        }

        [Fact]
        public void UnSubscribeMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[unsubscribe]]></Event>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<UnSubscribeEventMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.UnSubscribe, model.EventType);
        }

        [Fact]
        public void QrSubscribeMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[subscribe]]></Event>
<EventKey><![CDATA[qrscene_123123]]></EventKey>
<Ticket><![CDATA[TICKET]]></Ticket>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<QrSubscribeEventKeyMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Subscribe, model.EventType);

            Assert.Equal("qrscene_123123", model.EventKey);
            Assert.Equal("TICKET", model.Ticket);
        }

        [Fact]
        public void QrAlreadySubscribeMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[SCAN]]></Event>
<EventKey><![CDATA[SCENE_VALUE]]></EventKey>
<Ticket><![CDATA[TICKET]]></Ticket>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<QrAlreadySubscribeEventKeyMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Scan, model.EventType);

            Assert.Equal("SCENE_VALUE", model.EventKey);
            Assert.Equal("TICKET", model.Ticket);
        }

        [Fact]
        public void TemplateSendMessageTest()
        {
            const string xmlContent = @"<xml>
           <ToUserName><![CDATA[gh_7f083739789a]]></ToUserName>
           <FromUserName><![CDATA[oia2TjuEGTNoeX76QEjQNrcURxG8]]></FromUserName>
           <CreateTime>1395658920</CreateTime>
           <MsgType><![CDATA[event]]></MsgType>
           <Event><![CDATA[TEMPLATESENDJOBFINISH]]></Event>
           <MsgID>200163836</MsgID>
           <Status><![CDATA[success]]></Status>
           </xml>";

            var model = _requestMessageFactory.CreateRequestMessage<TemplateMessageSendPushMessage>(xmlContent);
            Assert.Equal("gh_7f083739789a", model.ToUserName);
            Assert.Equal("oia2TjuEGTNoeX76QEjQNrcURxG8", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1395658920), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.TemplateSendJobFinish, model.EventType);
            Assert.Equal(200163836, model.MessageId);
            Assert.Equal(TemplateMessageSendStatus.Success, model.Status);
        }

        [Fact]
        public void MassSendMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[gh_3e8adccde292]]></ToUserName>
<FromUserName><![CDATA[oR5Gjjl_eiZoUpGozMo7dbBJ362A]]></FromUserName>
<CreateTime>1394524295</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[MASSSENDJOBFINISH]]></Event>
<MsgID>1988</MsgID>
<Status><![CDATA[sendsuccess]]></Status>
<TotalCount>100</TotalCount>
<FilterCount>80</FilterCount>
<SentCount>75</SentCount>
<ErrorCount>5</ErrorCount>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<MassSendPushMessage>(xmlContent);
            Assert.Equal("gh_3e8adccde292", model.ToUserName);
            Assert.Equal("oR5Gjjl_eiZoUpGozMo7dbBJ362A", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1394524295), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.MassSendJobFinish, model.EventType);
            Assert.Equal(1988, model.MessageId);
            Assert.Equal(MassSendStatus.Success, model.Status);
            Assert.Equal((ulong)100, model.TotalCount);
            Assert.Equal((ulong)80, model.FilterCount);
            Assert.Equal((ulong)75, model.SentCount);
            Assert.Equal((ulong)5, model.ErrorCount);
        }

        #region CustomMenu

        [Fact]
        public void CustomMenuClickMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[CLICK]]></Event>
<EventKey><![CDATA[EVENTKEY]]></EventKey>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ClickMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Click, model.EventType);

            Assert.Equal("EVENTKEY", model.EventKey);
        }

        [Fact]
        public void CustomMenuViewMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[VIEW]]></Event>
<EventKey><![CDATA[www.qq.com]]></EventKey>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ViewMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.View, model.EventType);

            Assert.Equal("www.qq.com", model.EventKey);
        }

        [Fact]
        public void CustomMenuScanCodePushMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090502</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[scancode_push]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType>
<ScanResult><![CDATA[1]]></ScanResult>
</ScanCodeInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ScanCodePushMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408090502), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.ScanCode_Push, model.EventType);
            Assert.Equal(ScanType.QrCode, model.Type);

            Assert.Equal("6", model.EventKey);
            Assert.Equal("1", model.Result);
        }

        [Fact]
        public void CustomMenuScanCodeWaitMessageMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090606</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[scancode_waitmsg]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<ScanCodeInfo><ScanType><![CDATA[qrcode]]></ScanType>
<ScanResult><![CDATA[2]]></ScanResult>
</ScanCodeInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ScanCodeWaitMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408090606), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.ScanCode_WaitMsg, model.EventType);
            Assert.Equal(ScanType.QrCode, model.Type);

            Assert.Equal("6", model.EventKey);
            Assert.Equal("2", model.Result);
        }

        [Fact]
        public void CustomMenuPicSysPhotoMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090651</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[pic_sysphoto]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<SendPicsInfo><Count>1</Count>
<PicList><item><PicMd5Sum><![CDATA[1b5f7c23b5bf75682a53e7b6d163e185]]></PicMd5Sum>
</item>
</PicList>
</SendPicsInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<PicSysPhotoMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408090651), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Pic_SysPhoto, model.EventType);

            Assert.Equal("6", model.EventKey);

            Assert.Equal(1, model.Count);
            Assert.Equal("1b5f7c23b5bf75682a53e7b6d163e185", model.PictureMd5List.First());
        }

        [Fact]
        public void CustomMenuPicPhotoOrAlbumMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090816</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[pic_photo_or_album]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<SendPicsInfo><Count>1</Count>
<PicList><item><PicMd5Sum><![CDATA[5a75aaca956d97be686719218f275c6b]]></PicMd5Sum>
</item>
</PicList>
</SendPicsInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<PicPhotoOrAlbumMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408090816), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Pic_Photo_Or_Album, model.EventType);

            Assert.Equal("6", model.EventKey);

            Assert.Equal(1, model.Count);
            Assert.Equal("5a75aaca956d97be686719218f275c6b", model.PictureMd5List.First());
        }

        [Fact]
        public void CustomMenuPicWeiXinMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408090816</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[pic_weixin]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<SendPicsInfo><Count>1</Count>
<PicList><item><PicMd5Sum><![CDATA[5a75aaca956d97be686719218f275c6b]]></PicMd5Sum>
</item>
</PicList>
</SendPicsInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<PicWeiXinMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408090816), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Pic_WeiXin, model.EventType);

            Assert.Equal("6", model.EventKey);

            Assert.Equal(1, model.Count);
            Assert.Equal("5a75aaca956d97be686719218f275c6b", model.PictureMd5List.First());
        }

        [Fact]
        public void CustomMenuLocationSelectMessageTest()
        {
            const string xmlContent = @"<xml><ToUserName><![CDATA[gh_e136c6e50636]]></ToUserName>
<FromUserName><![CDATA[oMgHVjngRipVsoxg6TuX3vz6glDg]]></FromUserName>
<CreateTime>1408091189</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[location_select]]></Event>
<EventKey><![CDATA[6]]></EventKey>
<SendLocationInfo><Location_X><![CDATA[23]]></Location_X>
<Location_Y><![CDATA[113]]></Location_Y>
<Scale><![CDATA[15]]></Scale>
<Label><![CDATA[ 广州市海珠区客村艺苑路 106号]]></Label>
<Poiname><![CDATA[test]]></Poiname>
</SendLocationInfo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<LocationSelectMessage>(xmlContent);
            Assert.Equal("gh_e136c6e50636", model.ToUserName);
            Assert.Equal("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1408091189), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Location_Select, model.EventType);

            Assert.Equal("6", model.EventKey);

            Assert.Equal(23, model.X);
            Assert.Equal(113, model.Y);
            Assert.Equal((uint)15, model.Scale);
            Assert.Equal(" 广州市海珠区客村艺苑路 106号", model.Label);
            Assert.Equal("test", model.Poiname);
        }

        #endregion CustomMenu

        #region CustomService

        [Fact]
        public void CreateSessionMessageTest()
        {
            const string xmlContent = @"<xml>
     <ToUserName><![CDATA[touser]]></ToUserName>
     <FromUserName><![CDATA[fromuser]]></FromUserName>
     <CreateTime>1399197672</CreateTime>
     <MsgType><![CDATA[event]]></MsgType>
     <Event><![CDATA[kf_create_session]]></Event>
     <KfAccount><![CDATA[test1@test]]></KfAccount>
 </xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CreateSessionMessage>(xmlContent);
            Assert.Equal("touser", model.ToUserName);
            Assert.Equal("fromuser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.KF_Create_Session, model.EventType);

            Assert.Equal("test1@test", model.Account);
        }

        [Fact]
        public void CloseSessionMessageTest()
        {
            const string xmlContent = @"<xml>
     <ToUserName><![CDATA[touser]]></ToUserName>
     <FromUserName><![CDATA[fromuser]]></FromUserName>
     <CreateTime>1399197672</CreateTime>
     <MsgType><![CDATA[event]]></MsgType>
     <Event><![CDATA[kf_close_session]]></Event>
     <KfAccount><![CDATA[test1@test]]></KfAccount>
 </xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CloseSessionMessage>(xmlContent);
            Assert.Equal("touser", model.ToUserName);
            Assert.Equal("fromuser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.KF_Close_Session, model.EventType);

            Assert.Equal("test1@test", model.Account);
        }

        [Fact]
        public void SwitchSessionMessageTest()
        {
            const string xmlContent = @"<xml>
     <ToUserName><![CDATA[touser]]></ToUserName>
     <FromUserName><![CDATA[fromuser]]></FromUserName>
     <CreateTime>1399197672</CreateTime>
     <MsgType><![CDATA[event]]></MsgType>
     <Event><![CDATA[kf_switch_session]]></Event>
     <FromKfAccount><![CDATA[test1@test]]></FromKfAccount>
     <ToKfAccount><![CDATA[test2@test]]></ToKfAccount>
 </xml>";

            var model = _requestMessageFactory.CreateRequestMessage<SwitchSessionMessage>(xmlContent);
            Assert.Equal("touser", model.ToUserName);
            Assert.Equal("fromuser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.KF_Switch_Session, model.EventType);

            Assert.Equal("test1@test", model.FromAccount);
            Assert.Equal("test2@test", model.ToAccount);
        }

        #endregion CustomService

        #region Card

        [Fact]
        public void CardEventPassCheckMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[card_pass_check]]></Event>  //不通过为card_not_pass_check
<CardId><![CDATA[cardid]]></CardId>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventPassCheckMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_Pass_Check, model.EventType);

            Assert.Equal("cardid", model.CardId);
        }

        [Fact]
        public void CardEventNotPassCheckMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[card_not_pass_check]]></Event>
<CardId><![CDATA[cardid]]></CardId>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventNotPassCheckMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_Not_Pass_Check, model.EventType);

            Assert.Equal("cardid", model.CardId);
        }

        [Fact]
        public void CardEventUserGetMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<FriendUserName><![CDATA[FriendUser]]></FriendUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[user_get_card]]></Event>
<CardId><![CDATA[cardid]]></CardId>
<IsGiveByFriend>1</IsGiveByFriend>
<UserCardCode><![CDATA[12312312]]></UserCardCode>
<OuterId>3</OuterId>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventUserGetMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_User_Get, model.EventType);

            Assert.Equal("cardid", model.CardId);
            Assert.Equal("FriendUser", model.FriendUserName);
            Assert.Equal(true, model.IsGiveByFriend);
            Assert.Equal("12312312", model.UserCardCode);
            Assert.Equal(null, model.OldUserCardCode);
            Assert.Equal(3, model.OuterId);
        }

        [Fact]
        public void CardEventUserDeleteMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[user_del_card]]></Event>
<CardId><![CDATA[cardid]]></CardId>
<UserCardCode><![CDATA[12312312]]></UserCardCode>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventUserDeleteMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_User_Delete, model.EventType);

            Assert.Equal("cardid", model.CardId);
            Assert.Equal("12312312", model.UserCardCode);
        }

        [Fact]
        public void CardEventUserConsumeCardMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[user_consume_card]]></Event>
<CardId><![CDATA[cardid]]></CardId>
<UserCardCode><![CDATA[12312312]]></UserCardCode>
<ConsumeSource><![CDATA[(FROM_API)]]></ConsumeSource>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventUserConsumeCardMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_User_Consume, model.EventType);

            Assert.Equal("cardid", model.CardId);
            Assert.Equal("12312312", model.UserCardCode);
            Assert.Equal(CardEventUserConsumeCardMessage.CardConsumeSource.Api, model.ConsumeSource);
        }

        [Fact]
        public void CardEventUserViewMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[user_view_card]]></Event>
<CardId><![CDATA[cardid]]></CardId>
<UserCardCode><![CDATA[12312312]]></UserCardCode>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventUserViewMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_User_View, model.EventType);

            Assert.Equal("cardid", model.CardId);
            Assert.Equal("12312312", model.UserCardCode);
        }

        [Fact]
        public void CardEventUserEnterSessionMessageTest()
        {
            const string xmlContent = @"<xml> <ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[user_enter_session_from_card]]></Event>
<CardId><![CDATA[cardid]]></CardId>
<UserCardCode><![CDATA[12312312]]></UserCardCode>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<CardEventUserEnterSessionMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.Card_UserEnterSession, model.EventType);

            Assert.Equal("cardid", model.CardId);
            Assert.Equal("12312312", model.UserCardCode);
        }

        #endregion Card

        #region WiFi

        [Fact]
        public void WiFiConnectedMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[FromUser]]></FromUserName>
<CreateTime>123456789</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[WifiConnected]]></Event>
<ConnectTime>0</ConnectTime>
<ExpireTime>0</ExpireTime>
<VendorId>![CDATA[3001224419]]</VendorId>
<ShopId><![CDATA[PlaceId]]></ShopId>
<DeviceNo><![CDATA[DeviceNo]]></DeviceNo>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ConnectedMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("FromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.WifiConnected, model.EventType);

            Assert.Equal("PlaceId", model.ShopId);
            Assert.Equal("DeviceNo", model.DeviceNo);
        }

        #endregion WiFi

        #region Shakearound

        [Fact]
        public void ShakearoundUserShakeMessageTest()
        {
            const string xmlContent = @"<xml>
<ToUserName><![CDATA[toUser]]></ToUserName>
<FromUserName><![CDATA[fromUser]]></FromUserName>
<CreateTime>1433332012</CreateTime>
    <MsgType><![CDATA[event]]></MsgType>
    <Event><![CDATA[ShakearoundUserShake]]></Event>
    <ChosenBeacon>
        <Uuid><![CDATA[uuid]]></Uuid>
        <Major>major</Major>
        <Minor>minor</Minor>
        <Distance>0.057</Distance>
    </ChosenBeacon>
    <AroundBeacons>
        <AroundBeacon>
            <Uuid><![CDATA[uuid]]></Uuid>
            <Major>major</Major>
            <Minor>minor</Minor>
            <Distance>166.816</Distance>
        </AroundBeacon>
        <AroundBeacon>
            <Uuid><![CDATA[uuid]]></Uuid>
            <Major>major</Major>
            <Minor>minor</Minor>
            <Distance>15.013</Distance>
        </AroundBeacon>
    </AroundBeacons>
</xml>";

            var model = _requestMessageFactory.CreateRequestMessage<ShakearoundUserShakeMessage>(xmlContent);
            Assert.Equal("toUser", model.ToUserName);
            Assert.Equal("fromUser", model.FromUserName);
            Assert.Equal(DateTimeHelper.GetTimeByTimeStamp(1433332012), model.CreateTime);
            Assert.Equal(RequestMessageType.Event, model.MessageType);
            Assert.Equal(EventType.ShakearoundUserShake, model.EventType);

            Assert.NotNull(model.ChosenBeacon);
            Assert.Equal("uuid", model.ChosenBeacon.Uuid);
            Assert.Equal("major", model.ChosenBeacon.Major);
            Assert.Equal("minor", model.ChosenBeacon.Minor);
            Assert.Equal(0.057, model.ChosenBeacon.Distance);

            Assert.NotNull(model.AroundBeacons);
            Assert.Equal(2, model.AroundBeacons.Length);
            Assert.Equal("uuid", model.AroundBeacons[0].Uuid);
            Assert.Equal("major", model.AroundBeacons[0].Major);
            Assert.Equal("minor", model.AroundBeacons[0].Minor);
            Assert.Equal(166.816, model.AroundBeacons[0].Distance);

            Assert.Equal("uuid", model.AroundBeacons[1].Uuid);
            Assert.Equal("major", model.AroundBeacons[1].Major);
            Assert.Equal("minor", model.AroundBeacons[1].Minor);
            Assert.Equal(15.013, model.AroundBeacons[1].Distance);
        }

        #endregion Shakearound

        #endregion Event
    }
}