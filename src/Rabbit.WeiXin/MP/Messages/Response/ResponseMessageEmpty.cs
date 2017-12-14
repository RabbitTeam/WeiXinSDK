using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.MP.Messages.Response
{
    /// <summary>
    /// 空文本响应消息。
    /// </summary>
    public sealed class ResponseMessageEmpty : ResponseMessageBase
    {
        /// <summary>
        /// 初始化一个新的文本响应消息。
        /// </summary>
        public ResponseMessageEmpty()
        {
        }

        /// <summary>
        /// 文本消息内容
        /// </summary>
        [Required]
        public string Content { get { return "success"; } }

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override ResponseMessageType MessageType => ResponseMessageType.Text;
    }
}