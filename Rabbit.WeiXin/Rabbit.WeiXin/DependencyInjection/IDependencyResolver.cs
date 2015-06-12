using Rabbit.WeiXin.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.WeiXin.DependencyInjection
{
    /// <summary>
    /// 一个抽象的依赖解析器。
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// 得到一个服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>服务实例。</returns>
        object GetService(Type serviceType);

        /// <summary>
        /// 获取多个服务实例。
        /// </summary>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>服务实例数组。</returns>
        IEnumerable<object> GetServices(Type serviceType);
    }

    /// <summary>
    /// 依赖解析器扩展。
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// 得到一个服务实例。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="dependencyResolver">依赖解析器。</param>
        /// <returns>服务实例。</returns>
        public static T GetService<T>(this IDependencyResolver dependencyResolver)
        {
            return (T)dependencyResolver.NotNull("dependencyResolver").GetService(typeof(T));
        }

        /// <summary>
        /// 获取多个服务实例。
        /// </summary>
        /// <typeparam name="T">服务类型。</typeparam>
        /// <param name="dependencyResolver">依赖解析器。</param>
        /// <returns>服务实例数组。</returns>
        public static IEnumerable<T> GetServices<T>(this IDependencyResolver dependencyResolver)
        {
            return dependencyResolver.NotNull("dependencyResolver").GetServices(typeof(T)).OfType<T>().ToArray();
        }
    }
}