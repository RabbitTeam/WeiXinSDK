using Rabbit.WeiXin.MP.Messages;
using Rabbit.WeiXin.MP.Messages.Response;
using Rabbit.WeiXin.MP.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.Utility.Extensions
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    public static class SerializeExtensions
    {
        /// <summary>
        /// 序列化Response消息
        /// </summary>
        /// <param name="responseMessage">Response消息</param>
        /// <returns></returns>
        public static string Serialize(this IResponseMessage responseMessage)
        {
            var facotry = new ResponseMessageFactory(new MessageFormatterFactory());
            return facotry.GetXmlByReponseMessage(responseMessage);
        }
    }
}
