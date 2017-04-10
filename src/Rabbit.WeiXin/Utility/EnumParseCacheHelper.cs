using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rabbit.WeiXin.Utility
{
    internal static class EnumParseCacheHelper
    {
        #region Field

        private static readonly ConcurrentDictionary<string, KeyValuePair<object, bool>> CacheDictionary = new ConcurrentDictionary<string, KeyValuePair<object, bool>>();

        #endregion Field

        public static T Parse<T>(string value) where T : struct
        {
            T result;
            if (!TryParse(value, out result))
                throw new NotSupportedException(string.Format("无法将 {0} 转换为指定的类型 {1}。", value, typeof(T).FullName));

            return result;
        }

        public static bool TryParse<T>(string value, out T result) where T : struct
        {
            var item = CacheDictionary.GetOrAdd(value, key =>
            {
                T temp;
                var isSuccess = Enum.TryParse(value, true, out temp);
                return new KeyValuePair<object, bool>(temp, isSuccess);
            });

            result = (T)item.Key;

            return item.Value;
        }
    }
}