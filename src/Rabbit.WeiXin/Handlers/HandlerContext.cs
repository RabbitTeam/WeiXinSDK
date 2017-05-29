using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.MP.Messages.Request;
using Rabbit.WeiXin.MP.Messages.Response;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NET

using System.Web;

#endif

namespace Rabbit.WeiXin.Handlers
{
    /// <summary>
    /// 一个抽象的处理上下文。
    /// </summary>
    public interface IHandlerContext
    {
#if NET

        /// <summary>
        /// 一个请求。
        /// </summary>
        HttpRequestBase Request { get; }

#endif

        /// <summary>
        /// 消息内容。
        /// </summary>
        string Content { get; }

        /// <summary>
        /// 最终响应给微信的Xml内容。
        /// </summary>
        string ResponseXml { get; set; }

        /// <summary>
        /// 处理环境。
        /// </summary>
        IDictionary<string, object> Environment { get; }

        /// <summary>
        /// 从环境中得到一个值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">值的key。</param>
        /// <returns>值。</returns>
        T Get<T>(string key);

        /// <summary>
        /// 设置一个环境值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">值的key。</param>
        /// <param name="value">具体值。</param>
        /// <returns>处理上下文。</returns>
        IHandlerContext Set<T>(string key, T value);
    }

    /// <summary>
    /// 一个默认的处理上下文。
    /// </summary>
    public sealed class HandlerContext : IHandlerContext
    {
#if NET

        /// <summary>
        /// 初始化一个新的处理上下文。
        /// </summary>
        /// <param name="request">一个请求。</param>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> 为null。</exception>
        public HandlerContext(HttpRequest request)
            : this(new HttpRequestWrapper(request.NotNull("request")))
        {
        }

        /// <summary>
        /// 初始化一个新的处理上下文。
        /// </summary>
        /// <param name="request">一个请求。</param>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> 为null。</exception>
        public HandlerContext(HttpRequestBase request)
        {
            Request = request.NotNull("request");
            Environment = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Content = Encoding.UTF8.GetString(request.InputStream.ReadBytes(request.ContentLength));
            this.SetRequestParameters(request.QueryString.AllKeys.ToDictionary(i => i, i => request.QueryString[i]));

            //设置默认的依赖解析器。
            this.SetDependencyResolver(DefaultDependencyResolver.Instance);
        }

#endif

        /// <summary>
        /// 初始化一个新的处理上下文。
        /// </summary>
        /// <param name="content">消息内容。</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> 为null。</exception>
        public HandlerContext(string content)
        {
            Content = content.NotNull(nameof(content));
            Environment = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            //设置默认的依赖解析器。
            this.SetDependencyResolver(DefaultDependencyResolver.Instance);
        }

        #region Implementation of IHandlerContext

#if NET

        /// <summary>
        /// 一个请求。
        /// </summary>
        public HttpRequestBase Request { get; }

#endif

        /// <summary>
        /// 消息内容。
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// 最终响应给微信的Xml内容。
        /// </summary>
        public string ResponseXml { get; set; }

        /// <summary>
        /// 处理环境。
        /// </summary>
        public IDictionary<string, object> Environment { get; }

        /// <summary>
        /// 从环境中得到一个值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">值的key（不区分大小写）。</param>
        /// <returns>值。</returns>
        public T Get<T>(string key)
        {
            object value;
            if (Environment.TryGetValue(key, out value))
                return (T)value;

            return default(T);
        }

        /// <summary>
        /// 设置一个环境值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">值的key（不区分大小写）。</param>
        /// <param name="value">具体值。</param>
        /// <returns>处理上下文。</returns>
        public IHandlerContext Set<T>(string key, T value)
        {
            Environment[key] = value;

            return this;
        }

        #endregion Implementation of IHandlerContext
    }

    /// <summary>
    /// 处理上下文扩展方法。
    /// </summary>
    public static partial class HandlerContextExtensions
    {
        /// <summary>
        /// 设置依赖解析器。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="dependencyResolver">依赖解析器实例。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> 为 null。</exception>
        /// <returns>处理上下文。</returns>
        public static IHandlerContext SetDependencyResolver(this IHandlerContext context, IDependencyResolver dependencyResolver)
        {
            context.NotNull("context").Environment["Rabbit.WeiXin.DependencyResolver"] = dependencyResolver.NotNull("dependencyResolver");

            return context;
        }

        /// <summary>
        /// 获取依赖解析器。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="Exception">在当前上下文中找不到依赖解析器。</exception>
        /// <returns>依赖解析器。</returns>
        public static IDependencyResolver GetDependencyResolver(this IHandlerContext context)
        {
            var dependencyResolver = context.NotNull("context").Get<IDependencyResolver>("Rabbit.WeiXin.DependencyResolver");

            if (dependencyResolver == null)
                throw new Exception("在当前上下文中找不到依赖解析器，您可以通过 SetDependencyResolver 方法设置一个依赖解析器。");

            return dependencyResolver;
        }

        /// <summary>
        /// 获取请求消息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="Exception">在当前上下文中找不到请求消息。</exception>
        /// <returns>请求消息实例。</returns>
        public static IRequestMessageBase GetRequestMessage(this IHandlerContext context)
        {
            var requestMessage = context.NotNull("content").Get<IRequestMessageBase>("Rabbit.WeiXin.RequestMessage");

            if (requestMessage == null)
                throw new Exception("在当前上下文中找不到请求消息，请确保注册的处理中间件中有包含对请求消息创建的处理动作。");
            return requestMessage;
        }

        /// <summary>
        /// 设置请求消息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="requestMessage">请求消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestMessage"/> 为 null。</exception>
        /// <returns>处理上下文。</returns>
        internal static IHandlerContext SetRequestMessage(this IHandlerContext context, IRequestMessageBase requestMessage)
        {
            context.NotNull("context").Environment["Rabbit.WeiXin.RequestMessage"] = requestMessage.NotNull("requestMessage");

            return context;
        }

        /// <summary>
        /// 获取响应消息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="Exception">在当前上下文中找不到响应消息。</exception>
        /// <returns>响应消息实例。</returns>
        public static IResponseMessage GetResponseMessage(this IHandlerContext context)
        {
            var responseMessage = context.NotNull("context").Get<IResponseMessage>("Rabbit.WeiXin.ResponseMessage");
            return responseMessage;
        }

        /// <summary>
        /// 设置响应消息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="responseMessage">响应消息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <returns>处理上下文。</returns>
        public static IHandlerContext SetResponseMessage(this IHandlerContext context, IResponseMessage responseMessage)
        {
            context.NotNull("context").Environment["Rabbit.WeiXin.ResponseMessage"] = responseMessage;

            return context;
        }

        /// <summary>
        /// 获取消息处理基本信息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="Exception">在当前上下文中找不到消息处理基本信息。</exception>
        /// <returns>消息处理基本信息。</returns>
        public static MessageHandlerBaseInfo GetMessageHandlerBaseInfo(this IHandlerContext context)
        {
            var info = context.NotNull("context").Get<MessageHandlerBaseInfo>("Rabbit.WeiXin.MessageHandlerBaseInfo");
            if (info == null)
                throw new Exception("在当前上下文中找不到消息处理基本信息，请确保向处理上下文注册了消息处理基本信息。");
            return info;
        }

        /// <summary>
        /// 设置消息处理基本信息。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="baseInfo">消息处理基本信息。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="baseInfo"/> 为 null。</exception>
        /// <returns>处理上下文。</returns>
        public static IHandlerContext SetMessageHandlerBaseInfo(this IHandlerContext context, MessageHandlerBaseInfo baseInfo)
        {
            context.NotNull("context").Environment["Rabbit.WeiXin.MessageHandlerBaseInfo"] = baseInfo.NotNull("baseInfo");

            return context;
        }

        /// <summary>
        /// 设置请求参数。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="parameters">请求参数。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <returns>处理上下文。</returns>
        public static IHandlerContext SetRequestParameters(this IHandlerContext context,
            IDictionary<string, string> parameters)
        {
            return context.NotNull(nameof(context)).Set("Rabbit.WeiXin.RequestParameters", parameters ?? new Dictionary<string, string>());
        }

        /// <summary>
        /// 获取请求参数。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 null。</exception>
        /// <returns>请求参数。</returns>
        public static IDictionary<string, string> GetRequestParameters(this IHandlerContext context)
        {
            var parameters = context
                .NotNull(nameof(context))
                .Get<IDictionary<string, string>>("Rabbit.WeiXin.RequestParameters");
            if (parameters != null)
                return parameters;
            parameters = new Dictionary<string, string>();
            context.SetRequestParameters(parameters);
            return parameters;
        }
    }
}