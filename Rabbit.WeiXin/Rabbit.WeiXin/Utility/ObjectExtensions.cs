using System;

namespace Rabbit.WeiXin.Utility
{
    /// <summary>
    /// 对象扩展方法。
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// 不允许为Null。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>对象实例。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> 为null。</exception>
        public static T NotNull<T>(this T instance, string name) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(name.NotEmpty("name"));
            return instance;
        }

        /// <summary>
        /// 不允许空字符串。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>字符串。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为空。</exception>
        public static string NotEmpty(this string str, string name)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(name.NotEmpty("name"));
            return str;
        }

        /// <summary>
        /// 不允许空和只包含空格的字符串。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>字符串。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> 为空或者全为空格。</exception>
        public static string NotEmptyOrWhiteSpace(this string str, string name)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(name.NotEmpty("name"));
            return str;
        }
    }
}