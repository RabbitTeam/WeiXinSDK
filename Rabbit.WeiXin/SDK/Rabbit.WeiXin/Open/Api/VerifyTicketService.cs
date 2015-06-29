using Rabbit.WeiXin.Open.Messages.Events;
using Rabbit.WeiXin.Utility.Extensions;
using System.IO;
using System.Xml;
using Tencent;

namespace Rabbit.WeiXin.Open.Api
{
    /// <summary>
    /// 一个抽象的核实票据服务。
    /// </summary>
    public interface IVerifyTicketService
    {
        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="postContent">请求主体内容。</param>
        void Set(string postContent);

        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="model">核实票据模型。</param>
        void Set(ComponentVerifyTicketPush model);

        /// <summary>
        /// 获取核实票据信息。
        /// </summary>
        /// <returns>核实票据模型。</returns>
        ComponentVerifyTicketPush Get();
    }

    /// <summary>
    /// 核实票据服务实现。
    /// </summary>
    public sealed class VerifyTicketService : IVerifyTicketService
    {
        #region Field

        private readonly AccountModel _accountModel;
        private ComponentVerifyTicketPush _model;

        #endregion Field

        #region Constructor

        public VerifyTicketService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IVerifyTicketService

        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="postContent">请求主体内容。</param>
        public void Set(string postContent)
        {
            //加载xml
            var content = postContent;
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(content);

            //得到加密的内容。
            var encryptContent = xmlDocument.FirstChild["Encrypt"].InnerText;

            string appId = null;
            //解密。
            content = Cryptography.AES_decrypt(encryptContent, _accountModel.EncodingAesKey, ref appId);
            xmlDocument.LoadXml(content);

            //创建值。
            Set(new ComponentVerifyTicketPush
            {
                AppId = appId,
                ComponentVerifyTicket = xmlDocument.FirstChild["ComponentVerifyTicket"].InnerText,
                CreateTime = Utility.DateTimeHelper.GetTimeByTimeStampString(xmlDocument.FirstChild["CreateTime"].InnerText)
            });
        }

        /// <summary>
        /// 设置核实票据信息。
        /// </summary>
        /// <param name="model">核实票据模型。</param>
        public void Set(ComponentVerifyTicketPush model)
        {
            _model = model.NotNull("model");
        }

        /// <summary>
        /// 获取核实票据信息。
        /// </summary>
        /// <returns>核实票据模型。</returns>
        public ComponentVerifyTicketPush Get()
        {
            return _model;
        }

        #endregion Implementation of IVerifyTicketService

        /// <summary>
        /// 核实票据服务扩展方法。
        /// </summary>
        public static class VerifyTicketServiceExtensions
        {
            /// <summary>
            /// 设置核实票据信息。
            /// </summary>
            /// <param name="verifyTicketService">一个抽象的核实票据服务。</param>
            /// <param name="postStream">请求主体流。</param>
            public static void Set(IVerifyTicketService verifyTicketService, Stream postStream)
            {
                verifyTicketService.NotNull("verifyTicketService");
                postStream.NotNull("postStream");

                using (var reader = new StreamReader(postStream))
                {
                    verifyTicketService.Set(reader.ReadToEnd());
                }
            }
        }
    }
}