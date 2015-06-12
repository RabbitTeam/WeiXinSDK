using Rabbit.WeiXin.Api.Utility;
using System;
using System.Collections;

namespace Rabbit.WeiXin.Api.Material
{
    /// <summary>
    /// 素材服务基类。
    /// </summary>
    public abstract class MaterialServiceBase
    {
        #region Protected Method

        /// <summary>
        /// 上传。
        /// </summary>
        /// <typeparam name="TResult">结果类型。</typeparam>
        /// <param name="url">请求地址。</param>
        /// <param name="bytes">内容。</param>
        /// <param name="fieldName">文件名称。</param>
        /// <param name="func">其他字段数据。</param>
        /// <returns>结果。</returns>
        protected static TResult Upload<TResult>(string url, byte[] bytes, string fieldName, Func<CreateBytes, byte[][]> func = null) where TResult : class
        {
            var createBytes = new CreateBytes();
            var list = new ArrayList
            {
                createBytes.CreateFieldData(fieldName, FileHelper.GetRandomFileName(bytes), FileHelper.GetContentType(bytes), bytes),
            };
            if (func != null)
                foreach (var b in func(createBytes))
                {
                    list.Add(b);
                }
            var data = createBytes.JoinBytes(list);

            return WeiXinHttpHelper.PostResultByJson<TResult>(url, data, createBytes.ContentType);
        }

        #endregion Protected Method
    }
}