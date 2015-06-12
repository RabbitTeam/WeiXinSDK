using Rabbit.WeiXin.Messages;
using Rabbit.WeiXin.Messages.Request;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 忽略重复消息处理中间件。
    /// </summary>
    public class IgnoreRepeatMessageHandlerMiddleware : HandlerMiddleware
    {
        #region Field

        private static readonly IList<KeyValuePair<string, DateTime>> MessageIdentity = new List<KeyValuePair<string, DateTime>>();

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public IgnoreRepeatMessageHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        #endregion Constructor

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            var requestMessage = context.GetRequestMessage();

            //得到消息的唯一标识。
            var identity = GetMessageIdentity(requestMessage);

            #region 删除无效的消息标识以节省资源

            for (var i = 0; i < MessageIdentity.Count; i++)
            {
                var item = MessageIdentity[i];
                //消息处理时间大于30秒则删除该条标识。
                if (item.Value.AddSeconds(30) < DateTime.Now)
                {
                    MessageIdentity.Remove(item);
                }
            }

            #endregion 删除无效的消息标识以节省资源

            //如果消息已经被标识为处理则跳过。
            if (MessageIdentity.Any(i => i.Key == identity))
                return EmptyHandlerMiddleware.Instance.Invoke(context);

            //标识消息正在处理。
            MessageIdentity.Add(new KeyValuePair<string, DateTime>(identity, DateTime.Now));

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware

        #region Private Method

        private static string GetMessageIdentity(IMessageBase requestMessage)
        {
            var message = requestMessage as IRequestMessage;
            if (message != null)
            {
                return message.MessageId.ToString(CultureInfo.InvariantCulture);
            }

            return requestMessage.FromUserName + requestMessage.CreateTime;
        }

        #endregion Private Method
    }
}