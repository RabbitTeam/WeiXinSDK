using System;
using System.IO;

namespace Rabbit.WeiXin.Utility.Extensions
{
    /// <summary>
    /// 流扩展方法。
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// 将流读取成字节组。
        /// </summary>
        /// <param name="stream">流。</param>
        /// <returns>字节组。</returns>
        public static byte[] ReadBytes(this Stream stream)
        {
            if (!stream.NotNull("stream").CanRead)
                throw new NotSupportedException(stream + "不支持读取。");

            Action trySeekBegin = () =>
            {
                if (!stream.CanSeek)
                    return;

                stream.Seek(0, SeekOrigin.Begin);
            };

            try
            {
                trySeekBegin();

                var buffer = new byte[(int)stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                return buffer;
            }
            finally
            {
                trySeekBegin();
            }
        }
    }
}