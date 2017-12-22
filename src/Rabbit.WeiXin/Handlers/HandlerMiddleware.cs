using System.Collections.Generic;
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
            MiddleBag = new Dictionary<string, object>();
            if (next != null && MiddleBag.Count > 0)
            {
                foreach (var item in MiddleBag)
                {
                    if (next.MiddleBag.ContainsKey(item.Key))
                    {
                        next.MiddleBag[item.Key] = item.Value;
                    }
                    else
                    {
                        next.MiddleBag.Add(item.Key, item.Value);
                    }
                }

            }
        }

        /// <summary>
        /// 下一个处理中间件。
        /// </summary>
        protected HandlerMiddleware Next { get; private set; }

        /// <summary>
        /// 中间件消息传递器
        /// </summary>
        protected Dictionary<string, object> MiddleBag { get; private set; } = new Dictionary<string, object>();

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public abstract Task Invoke(IHandlerContext context);
    }
}