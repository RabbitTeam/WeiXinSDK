using System.Collections;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Utility
{
    public sealed class CreateBytes
    {
        private readonly Encoding _encoding = Encoding.UTF8;

        public string Boundary
        {
            get
            {
                var contentType = ContentType;
                var array = contentType.Split(new[]
				{
					';'
				});
                if (array[0].Trim().ToLower() != "multipart/form-data")
                {
                    return null;
                }
                var array2 = array[1].Split(new[]
				{
					'='
				});
                return "--" + array2[1];
            }
        }

        public string ContentType
        {
            get
            {
                return "multipart/form-data; boundary=---------------------------7d5b915500cee";
            }
        }

        /// <summary>
        /// 拼接所有的二进制数组为一个数组
        /// </summary>
        /// <param name="byteArrays">数组</param>
        /// <returns></returns>
        /// <remarks>加上结束边界</remarks>
        public byte[] JoinBytes(ArrayList byteArrays)
        {
            var num = 0;
            var s = Boundary + "--\r\n";
            var bytes = _encoding.GetBytes(s);
            byteArrays.Add(bytes);
            var num2 = byteArrays.Cast<byte[]>().Sum(b => b.Length);
            var array = new byte[num2];
            foreach (byte[] array2 in byteArrays)
            {
                array2.CopyTo(array, num);
                num += array2.Length;
            }
            return array;
        }

        /// <summary>
        /// 获取普通表单区域二进制数组
        /// </summary>
        /// <param name="fieldName">表单名</param>
        /// <param name="fieldValue">表单值</param>
        /// <returns></returns>
        /// <remarks>
        /// -----------------------------7d52ee27210a3c\r\nContent-Disposition: form-data; name=\"表单名\"\r\n\r\n表单值\r\n
        /// </remarks>
        public byte[] CreateFieldData(string fieldName, string fieldValue)
        {
            var format = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            var s = string.Format(format, fieldName, fieldValue);
            return _encoding.GetBytes(s);
        }

        /// <summary>
        /// 获取文件上传表单区域二进制数组
        /// </summary>
        /// <param name="fieldName">表单名</param>
        /// <param name="filename">文件名</param>
        /// <param name="contentType">文件类型</param>
        /// <param name="fileBytes">文件字节组。</param>
        /// <returns>二进制数组</returns>
        public byte[] CreateFieldData(string fieldName, string filename, string contentType, byte[] fileBytes)
        {
            var format = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            var s = string.Format(format, fieldName, filename, contentType);
            var bytes = _encoding.GetBytes(s);
            var bytes2 = _encoding.GetBytes("\r\n");
            var array = new byte[bytes.Length + fileBytes.Length + bytes2.Length];
            bytes.CopyTo(array, 0);
            fileBytes.CopyTo(array, bytes.Length);
            bytes2.CopyTo(array, bytes.Length + fileBytes.Length);
            return array;
        }
    }
}