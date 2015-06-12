using Rabbit.WeiXin.Messages;
using System;
using System.Collections.Generic;

namespace Rabbit.WeiXin.DependencyInjection
{
    /// <summary>
    /// 默认的依赖解析器实现。
    /// </summary>
    public class DefaultDependencyResolver : IDependencyResolver
    {
        #region Field

        private static readonly IDependencyResolver DependencyResolver = new DefaultDependencyResolver();

        private static readonly ISignatureService SignatureService = new SignatureService();
        private static readonly IRequestMessageFactory RequestMessageFactory = new RequestMessageFactory();
        private static readonly IResponseMessageFactory ResponseMessageFactory = new ResponseMessageFactory();

        private readonly IDictionary<Type, Func<object>> _mappings = new Dictionary<Type, Func<object>>
        {
            {typeof(ISignatureService),()=>SignatureService},
            {typeof(IRequestMessageFactory),()=>RequestMessageFactory},
            {typeof(IResponseMessageFactory),()=>ResponseMessageFactory}
        };

        #endregion Field

        #region Property

        /// <summary>
        /// 实例。
        /// </summary>
        public static IDependencyResolver Instance { get { return DependencyResolver; } }

        #endregion Property

        #region Implementation of IDependencyResolver

        /// <summary>
        /// 得到一个服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>服务实例。</returns>
        public virtual object GetService(Type serviceType)
        {
            return _mappings[serviceType]();
        }

        /// <summary>
        /// 获取多个服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>服务实例数组。</returns>
        public virtual IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }

        #endregion Implementation of IDependencyResolver
    }
}