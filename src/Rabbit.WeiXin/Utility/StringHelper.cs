using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.Utility
{

    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public class StringHelper
    {
        static Random random = new Random();
        static string[] arr = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };


        /// <summary>
        /// 获得随机长度字符串
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                result.Append(arr[random.Next(37)]);
            }
            return result.ToString();
        }
    }
}
