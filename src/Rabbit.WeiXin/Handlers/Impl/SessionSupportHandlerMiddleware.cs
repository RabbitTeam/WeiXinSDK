using Rabbit.WeiXin.DependencyInjection;
using Rabbit.WeiXin.Handlers.Impl;
using Rabbit.WeiXin.Utility.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.WeiXin.Handlers.Impl
{
    /// <summary>
    /// 用户会话删除事件参数。
    /// </summary>
    public sealed class UserSessionRemovedEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// 初始化一个新的用户会话删除事件参数。
        /// </summary>
        /// <param name="session">用户会话。</param>
        /// <param name="openId">用户OpenId。</param>
        public UserSessionRemovedEventArgs(IUserSession session, string openId)
        {
            Session = session.NotNull("session");
            OpenId = openId.NotEmptyOrWhiteSpace("openId");
        }

        #endregion Constructor

        /// <summary>
        /// 用户会话。
        /// </summary>
        public IUserSession Session { get; private set; }

        /// <summary>
        /// 用户的OpenId。
        /// </summary>
        public string OpenId { get; private set; }
    }

    /// <summary>
    /// 一个抽象的用户会话接口。
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// 获取或设置一个会话值。
        /// </summary>
        /// <param name="key">会话键。</param>
        /// <returns>会话值。</returns>
        object this[object key] { get; set; }

        /// <summary>
        /// 会话被删除事件。
        /// </summary>
        event EventHandler<UserSessionRemovedEventArgs> Removed;
    }

    /// <summary>
    /// 一个抽象的用户会话集合。
    /// </summary>
    public interface IUserSessionCollection
    {
        /// <summary>
        /// 获取或者添加一个用户会话。
        /// </summary>
        /// <param name="userIdentity">用户标识。</param>
        /// <returns>用户会话。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="userIdentity"/> 为空。</exception>
        IUserSession GetOrAdd(string userIdentity);

        /// <summary>
        /// 压缩会话（清除过期的会话）。
        /// </summary>
        Task Compression();
    }

    /// <summary>
    /// 用户会话集合。
    /// </summary>
    internal class UserSessionCollection : IUserSessionCollection
    {
        #region Property

        /// <summary>
        /// 会话过期时间。
        /// </summary>
        public TimeSpan Timeout { get; private set; }

        #endregion Property

        #region Field

        /// <summary>
        /// 用户会话字典。
        /// </summary>
        private readonly ConcurrentDictionary<string, UserSession> _concurrentDictionary = new ConcurrentDictionary<string, UserSession>();

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的用户会话集合，过期时间为20分钟。
        /// </summary>
        public UserSessionCollection()
            : this(TimeSpan.FromMinutes(20))
        {
        }

        /// <summary>
        /// 初始化一个新的用户会话集合。
        /// </summary>
        /// <param name="timeout">过期时间。</param>
        /// <exception cref="ArgumentOutOfRangeException">超时时间小于30秒。</exception>
        public UserSessionCollection(TimeSpan timeout)
        {
            if (timeout.TotalSeconds < 30)
                throw new ArgumentOutOfRangeException("timeout", timeout, "超时时间不能小于30秒。");

            Timeout = timeout;
        }

        #endregion Constructor

        #region Public Method

        /// <summary>
        /// 获取或者添加一个用户会话。
        /// </summary>
        /// <param name="userIdentity">用户标识。</param>
        /// <returns>用户会话。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="userIdentity"/> 为空。</exception>
        public IUserSession GetOrAdd(string userIdentity)
        {
            return _concurrentDictionary.GetOrAdd(userIdentity.NotEmptyOrWhiteSpace("userIdentity"), k => new UserSession()).UpdateLastActiveTime();
        }

        /// <summary>
        /// 压缩会话（清除过期的会话）。
        /// </summary>
        public Task Compression()
        {
            return Task.Factory.StartNew(() =>
            {
                var now = DateTime.Now;
                foreach (var key in _concurrentDictionary.Where(i => i.Value.LastActiveTime.Add(Timeout) < now).Select(i => i.Key).ToArray())
                {
                    UserSession session;
                    if (_concurrentDictionary.TryRemove(key, out session))
                    {
                        //执行事件。
                        session.OnRemoved(key);
                    }
                }
            });
        }

        #endregion Public Method
    }

    /// <summary>
    /// 用户会话。
    /// </summary>
    internal sealed class UserSession : IUserSession
    {
        #region Field

        private event EventHandler<UserSessionRemovedEventArgs> RemovedHandler;

        private DateTime _lastActiveTime = DateTime.Now;
        private readonly IDictionary<object, object> _session = new Dictionary<object, object>();

        #endregion Field

        #region Property

        /// <summary>
        /// 最后活动时间。
        /// </summary>
        internal DateTime LastActiveTime
        {
            get { return _lastActiveTime; }
        }

        #endregion Property

        #region Internal Method

        /// <summary>
        /// 更新最后活动时间。
        /// </summary>
        internal UserSession UpdateLastActiveTime()
        {
            _lastActiveTime = DateTime.Now;

            return this;
        }

        /// <summary>
        /// 执行会话被删除事件。
        /// </summary>
        /// <param name="openId">用户OpenId。</param>
        internal void OnRemoved(string openId)
        {
            var handler = RemovedHandler;
            if (handler != null)
                handler(this, new UserSessionRemovedEventArgs(this, openId));
        }

        #endregion Internal Method

        #region Implementation of IUserSession

        /// <summary>
        /// 获取或设置一个会话值。
        /// </summary>
        /// <param name="key">会话键。</param>
        /// <returns>会话值。</returns>
        public object this[object key]
        {
            get { return _session[key]; }
            set { _session[key] = value; }
        }

        /// <summary>
        /// 会话被删除事件。
        /// </summary>
        public event EventHandler<UserSessionRemovedEventArgs> Removed
        {
            add { RemovedHandler += value; }
            remove { RemovedHandler -= value; }
        }

        #endregion Implementation of IUserSession
    }

    /// <summary>
    /// 会话支持处理中间件。
    /// </summary>
    public sealed class SessionSupportHandlerMiddleware : HandlerMiddleware, IDisposable
    {
        #region Field

        private IUserSessionCollection _sessionCollection;

        #endregion Field

        #region Overrides of HandlerMiddleware

        /// <summary>
        /// 初始化一个新的处理中间件。
        /// </summary>
        /// <param name="next">下一个处理中间件。</param>
        public SessionSupportHandlerMiddleware(HandlerMiddleware next)
            : base(next)
        {
        }

        /// <summary>
        /// 调用。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>任务。</returns>
        public override Task Invoke(IHandlerContext context)
        {
            _sessionCollection = context.GetDependencyResolver().GetService<IUserSessionCollection>();

            var requestMessage = context.GetRequestMessage();
            //设置用户会话。
            context.SetSession(_sessionCollection.GetOrAdd(requestMessage.FromUserName));

            return Next.Invoke(context);
        }

        #endregion Overrides of HandlerMiddleware

        #region Implementation of IDisposable

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (_sessionCollection != null)
                _sessionCollection.Compression();
        }

        #endregion Implementation of IDisposable
    }
}

namespace Rabbit.WeiXin.Handlers
{
    /// <summary>
    /// 处理上下文扩展方法。
    /// </summary>
    public static partial class HandlerContextExtensions
    {
        /// <summary>
        /// 获取当前用户的会话。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <returns>用户会话。</returns>
        public static IUserSession GetSession(this IHandlerContext context)
        {
            return context.NotNull("handlerContext").Environment["Rabbit.WeiXin.Session"] as IUserSession;
        }

        /// <summary>
        /// 设置当前用户的会话。
        /// </summary>
        /// <param name="context">处理上下文。</param>
        /// <param name="session">用户会话。</param>
        /// <returns>处理上下文。</returns>
        internal static IHandlerContext SetSession(this IHandlerContext context, IUserSession session)
        {
            context.NotNull("handlerContext").Environment["Rabbit.WeiXin.Session"] = session;
            return context;
        }
    }
}