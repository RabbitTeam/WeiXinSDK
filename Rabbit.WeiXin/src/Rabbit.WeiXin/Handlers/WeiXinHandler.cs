using Rabbit.WeiXin.Handlers.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers
{
    /// <summary>
    /// 一个抽象的处理程序。
    /// </summary>
    public interface IWeiXinHandler
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        Task Execute(IHandlerContext context);
    }

    /// <summary>
    /// 默认的处理程序。
    /// </summary>
    public sealed class DefaultWeiXinHandler : IWeiXinHandler
    {
        #region Field

        private readonly IHandlerBuilder _builder;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的默认微信处理程序。
        /// </summary>
        /// <param name="builder">处理构造者。</param>
        public DefaultWeiXinHandler(IHandlerBuilder builder)
        {
            _builder = builder;
        }

        #endregion Constructor

        #region Implementation of IHandler

        /// <summary>
        /// 执行。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public Task Execute(IHandlerContext context)
        {
            var middlewareItems = (ICollection<KeyValuePair<object, object[]>>)_builder.Properties["Rabbit.Middlewares"];

            var fristMiddleware = GetFirstMiddleware(middlewareItems);

            return fristMiddleware.Invoke(context);
        }

        #endregion Implementation of IHandler

        #region Private Method

        private static HandlerMiddleware GetFirstMiddleware(ICollection<KeyValuePair<object, object[]>> middlewareItems)
        {
            if (middlewareItems.Count == 0)
                return EmptyHandlerMiddleware.Instance;

            var middlewares = new List<HandlerMiddleware>();
            foreach (var item in middlewareItems.Reverse())
            {
                var lastMiddleware = middlewares.LastOrDefault() ?? EmptyHandlerMiddleware.Instance;

                IEnumerable<object> args = new object[] { lastMiddleware };
                if (item.Value != null && item.Value.Any())
                {
                    args = args.Concat(item.Value);
                }

                middlewares.Add(GetHandlerMiddleware(item.Key, args.ToArray()));
            }
            middlewares.Reverse();
            return middlewares.FirstOrDefault();
        }

        private static HandlerMiddleware GetHandlerMiddleware(object middleware, object[] args)
        {
            if (middleware is HandlerMiddleware)
            {
                return middleware as HandlerMiddleware;
            }

            if (middleware is Type)
            {
                var type = middleware as Type;
                if (!typeof(HandlerMiddleware).IsAssignableFrom(type))
                    throw new NotSupportedException("无法将类型：" + type.FullName + "，注册为处理中间件。");

                var constructor = type.GetConstructors().First();
                return constructor.Invoke(args) as HandlerMiddleware;
            }

            throw new NotSupportedException("无法将类型：" + middleware.GetType().FullName + "，注册为处理中间件。");
        }

        #endregion Private Method
    }
}