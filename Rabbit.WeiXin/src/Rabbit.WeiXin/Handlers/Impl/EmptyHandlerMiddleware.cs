using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 一个空的处理中间件。
    /// </summary>
    public sealed class EmptyHandlerMiddleware : HandlerMiddleware
    {
        /// <summary>
        /// 实例。
        /// </summary>
        public static readonly HandlerMiddleware Instance = new EmptyHandlerMiddleware(null);

        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public EmptyHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            return Task.Factory.StartNew(() => { });
        }

        #endregion Overrides of HandlerMiddleware
    }
}