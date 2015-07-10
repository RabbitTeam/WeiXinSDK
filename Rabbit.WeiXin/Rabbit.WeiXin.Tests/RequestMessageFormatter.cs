using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.MP.Messages;
using Rabbit.WeiXin.MP.Messages.Events;
using Rabbit.WeiXin.MP.Messages.Events.Card;
using Rabbit.WeiXin.MP.Messages.Events.CustomMenu;
using Rabbit.WeiXin.MP.Messages.Events.CustomService;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.Utility;
using System;
using System.Linq;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class RequestMessageFormatter
    {
        #region Field

        private readonly IRequestMessageFactory _requestMessageFactory = DefaultDependencyResolver.Instance.GetService<IRequestMessageFactory>();

        #endregion Field

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(RequestMessageType.Text, model.MessageType);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1348831860), model.CreateTime);
            Assert.AreEqual("this is a test", model.Content);
            Assert.AreEqual(1234567890123456, model.MessageId);
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1348831860), model.CreateTime);
            Assert.AreEqual(1234567890123456, model.MessageId);
            Assert.AreEqual("1234567890-1234567", model.MediaId);
            Assert.AreEqual(RequestMessageType.Image, model.MessageType);

            Assert.AreEqual(model.PicUrl, new Uri("http://www.chunsun.cc/1.jpg"));
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.AreEqual(1234567890123456, model.MessageId);
            Assert.AreEqual("1234567890-1234567", model.MediaId);
            Assert.AreEqual(RequestMessageType.Voice, model.MessageType);

            Assert.AreEqual(model.Format, "Format");
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.AreEqual(1234567890123456, model.MessageId);
            Assert.AreEqual("1234567890-1234567", model.MediaId);
            Assert.AreEqual(RequestMessageType.Video, model.MessageType);

            Assert.AreEqual("1234567890-12345678", model.ThumbMediaId);
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1357290913), model.CreateTime);
            Assert.AreEqual("1234567890-1234567", model.MediaId);
            Assert.AreEqual(RequestMessageType.ShortVideo, model.MessageType);

            Assert.AreEqual(model.ThumbMediaId, "1234567890-12345678");
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1351776360), model.CreateTime);
            Assert.AreEqual(1234567890123456, model.MessageId);
            Assert.AreEqual(RequestMessageType.Location, model.MessageType);

            Assert.AreEqual(23.134521, model.X);
            Assert.AreEqual(113.358803, model.Y);
            Assert.AreEqual("位置信息", model.Label);
            Assert.AreEqual(20, model.Scale);
        }

        [TestMethod]
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

            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("fromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1351776360), model.CreateTime);
            Assert.AreEqual(1234567890123456, model.MessageId);
            Assert.AreEqual(RequestMessageType.Link, model.MessageType);

            Assert.AreEqual("公众平台官网链接", model.Title);
            Assert.AreEqual("公众平台官网链接描述", model.Description);
            Assert.AreEqual(new Uri("http://www.chunsun.cc/"), model.Url);
        }

        #region Event

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Subscribe, model.EventType);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.UnSubscribe, model.EventType);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Subscribe, model.EventType);

            Assert.AreEqual("qrscene_123123", model.EventKey);
            Assert.AreEqual("TICKET", model.Ticket);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Scan, model.EventType);

            Assert.AreEqual("SCENE_VALUE", model.EventKey);
            Assert.AreEqual("TICKET", model.Ticket);
        }

        [TestMethod]
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
            Assert.AreEqual("gh_7f083739789a", model.ToUserName);
            Assert.AreEqual("oia2TjuEGTNoeX76QEjQNrcURxG8", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1395658920), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.TemplateSendJobFinish, model.EventType);
            Assert.AreEqual(200163836, model.MessageId);
            Assert.AreEqual(TemplateMessageSendStatus.Success, model.Status);
        }

        [TestMethod]
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
            Assert.AreEqual("gh_3e8adccde292", model.ToUserName);
            Assert.AreEqual("oR5Gjjl_eiZoUpGozMo7dbBJ362A", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1394524295), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.MassSendJobFinish, model.EventType);
            Assert.AreEqual(1988, model.MessageId);
            Assert.AreEqual(MassSendStatus.Success, model.Status);
            Assert.AreEqual((ulong)100, model.TotalCount);
            Assert.AreEqual((ulong)80, model.FilterCount);
            Assert.AreEqual((ulong)75, model.SentCount);
            Assert.AreEqual((ulong)5, model.ErrorCount);
        }

        #region CustomMenu

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Click, model.EventType);

            Assert.AreEqual("EVENTKEY", model.EventKey);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.View, model.EventType);

            Assert.AreEqual("www.qq.com", model.EventKey);
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408090502), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.ScanCode_Push, model.EventType);
            Assert.AreEqual(ScanType.QrCode, model.Type);

            Assert.AreEqual("6", model.EventKey);
            Assert.AreEqual("1", model.Result);
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408090606), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.ScanCode_WaitMsg, model.EventType);
            Assert.AreEqual(ScanType.QrCode, model.Type);

            Assert.AreEqual("6", model.EventKey);
            Assert.AreEqual("2", model.Result);
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408090651), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Pic_SysPhoto, model.EventType);

            Assert.AreEqual("6", model.EventKey);

            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("1b5f7c23b5bf75682a53e7b6d163e185", model.PictureMd5List.First());
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408090816), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Pic_Photo_Or_Album, model.EventType);

            Assert.AreEqual("6", model.EventKey);

            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("5a75aaca956d97be686719218f275c6b", model.PictureMd5List.First());
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408090816), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Pic_WeiXin, model.EventType);

            Assert.AreEqual("6", model.EventKey);

            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("5a75aaca956d97be686719218f275c6b", model.PictureMd5List.First());
        }

        [TestMethod]
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
            Assert.AreEqual("gh_e136c6e50636", model.ToUserName);
            Assert.AreEqual("oMgHVjngRipVsoxg6TuX3vz6glDg", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1408091189), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Location_Select, model.EventType);

            Assert.AreEqual("6", model.EventKey);

            Assert.AreEqual(23, model.X);
            Assert.AreEqual(113, model.Y);
            Assert.AreEqual((uint)15, model.Scale);
            Assert.AreEqual(" 广州市海珠区客村艺苑路 106号", model.Label);
            Assert.AreEqual("test", model.Poiname);
        }

        #endregion CustomMenu

        #region CustomService

        [TestMethod]
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
            Assert.AreEqual("touser", model.ToUserName);
            Assert.AreEqual("fromuser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.KF_Create_Session, model.EventType);

            Assert.AreEqual("test1@test", model.Account);
        }

        [TestMethod]
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
            Assert.AreEqual("touser", model.ToUserName);
            Assert.AreEqual("fromuser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.KF_Close_Session, model.EventType);

            Assert.AreEqual("test1@test", model.Account);
        }

        [TestMethod]
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
            Assert.AreEqual("touser", model.ToUserName);
            Assert.AreEqual("fromuser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(1399197672), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.KF_Switch_Session, model.EventType);

            Assert.AreEqual("test1@test", model.FromAccount);
            Assert.AreEqual("test2@test", model.ToAccount);
        }

        #endregion CustomService

        #region Card

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_Pass_Check, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_Not_Pass_Check, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_User_Get, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
            Assert.AreEqual("FriendUser", model.FriendUserName);
            Assert.AreEqual(true, model.IsGiveByFriend);
            Assert.AreEqual("12312312", model.UserCardCode);
            Assert.AreEqual(null, model.OldUserCardCode);
            Assert.AreEqual(3, model.OuterId);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_User_Delete, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
            Assert.AreEqual("12312312", model.UserCardCode);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_User_Consume, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
            Assert.AreEqual("12312312", model.UserCardCode);
            Assert.AreEqual(CardEventUserConsumeCardMessage.CardConsumeSource.Api, model.ConsumeSource);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_User_View, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
            Assert.AreEqual("12312312", model.UserCardCode);
        }

        [TestMethod]
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
            Assert.AreEqual("toUser", model.ToUserName);
            Assert.AreEqual("FromUser", model.FromUserName);
            Assert.AreEqual(DateTimeHelper.GetTimeByTimeStamp(123456789), model.CreateTime);
            Assert.AreEqual(RequestMessageType.Event, model.MessageType);
            Assert.AreEqual(EventType.Card_UserEnterSession, model.EventType);

            Assert.AreEqual("cardid", model.CardId);
            Assert.AreEqual("12312312", model.UserCardCode);
        }

        #endregion Card

        #endregion Event
    }
}