using System;
using System.Collections.Generic;

namespace Rabbit.WeiXin.Utility
{
    internal static class EnumParseCacheHelper
    {
        #region Field

        private static readonly IDictionary<string, KeyValuePair<object, bool>> CacheDictionary = new Dictionary<string, KeyValuePair<object, bool>>();

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
            KeyValuePair<object, bool> item;

            //如果缓存中存在则直接返回。
            if (CacheDictionary.TryGetValue(value, out item))
            {
                result = (T)item.Key;
                return item.Value;
            }

            var isSuccess = Enum.TryParse(value, true, out result);

            //添加到缓存中。
            CacheDictionary[value] = new KeyValuePair<object, bool>(result, isSuccess);

            return isSuccess;
        }
    }
}