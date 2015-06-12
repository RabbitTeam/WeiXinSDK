using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rabbit.WeiXin.Api.TemplateMessage;

namespace Rabbit.WeiXin.Tests
{
    [TestClass]
    public class TemplateMessageServiceTest : ApiTestBase
    {
        #region Field

        private readonly ITemplateMessageService _templateMessageService;

        #endregion Field

        #region Constructor

        public TemplateMessageServiceTest()
        {
            _templateMessageService = new TemplateMessageService(AccountModel);
        }

        #endregion Constructor

        #region Test Method

        [TestMethod]
        public void SendTest()
        {
            _templateMessageService.Send(OpenId, "4Hdi5ry8Hxb6HtEHTTrEd7yLgAIrRLG4-6t55ammLS0", "http://cn.bing.com", "#FF0000", new
            {
                first = new TemplateMessageFieldDataItem("您好，您对微信影城影票的抢购未成功，已退款。", "#173177"),
                reason = new TemplateMessageFieldDataItem("未抢购成功"),
                refund = new TemplateMessageFieldDataItem("70元"),
                remark = new TemplateMessageFieldDataItem("如有疑问，请致电13912345678联系我们，或回复M来了解详情。")
            });
        }

        #endregion Test Method
    }
}