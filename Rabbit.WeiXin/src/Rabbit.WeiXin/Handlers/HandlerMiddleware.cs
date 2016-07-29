using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers
{
    /// <summary>
    /// 一个抽象的处理中间件。
    /// </summary>
    public abstract class HandlerMiddleware
    {
        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        protected HandlerMiddleware(HandlerMiddleware next)
        {
            Next = next;
        }

        /// <summary>
        /// 下一个处理中间件。
        /// </summary>
        protected HandlerMiddleware Next { get; private set; }

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public abstract Task Invoke(IHandlerContext context);
    }
}