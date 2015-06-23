using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rabbit.WeiXin.MP.Api.Utility
{
    /// <summary>
    /// 文件帮助类
    /// </summary>
    internal static class FileHelper
    {
        private static readonly string[] ImageFileExtensions = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };

        private static readonly IDictionary<string, string> FileFormats = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {".gif","7173"},
            {".jpg","255216"},
            {".jpeg","255216"},
            {".png","13780"},
            {".bmp","6677"},
            {".mp3","7368"},
            {".wma","4838"},
            {".wav","8273"},
            {".amr","3533"},
            {".mp4","00"},
        };

        private static readonly IDictionary<string, string> ContentTypeExtensionsMapping = new Dictionary<string, string>
        {
            {".gif", "image/gif"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".png", "image/png"},
            {".bmp", "application/x-bmp"},
            {".mp3", "audio/mp3"},
            {".wma", "audio/x-ms-wma"},
            {".wav", "audio/wav"},
            {".amr", "audio/amr"},
            {".mp4","video/mpeg4"}
        };

        /// <summary>
        /// 根据文件名称来验证是否是一个图片类型的文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <returns>如果是一个图片类型的文件则返回true，否则返回false。</returns>
        public static bool IsImage(string fileName)
        {
            //文件名称为空。
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            //文件扩展名为空。
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension))
                return false;

            //文件扩展名是否在系统定义的文件扩展名之内。
            return ImageFileExtensions.Contains(extension.ToLower());
        }

        /// <summary>
        /// 根据文件信息来验证是否是一个图片类型的文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="stream">流。</param>
        /// <returns>如果是一个图片类型的文件则返回true，否则返回false。</returns>
        public static bool IsImage(string fileName, Stream stream)
        {
            if (stream == null || stream.Length < 2)
                return false;
            var bytes = new[] { (byte)stream.ReadByte(), (byte)stream.ReadByte() };
            stream.Seek(0, SeekOrigin.Begin);
            return IsImage(fileName, bytes);
        }

        /// <summary>
        /// 根据文件信息来验证是否是一个图片类型的文件。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <param name="fileBytes">文件字节组。</param>
        /// <returns>如果是一个图片类型的文件则返回true，否则返回false。</returns>
        public static bool IsImage(string fileName, byte[] fileBytes)
        {
            //文件字节组是否符合规范。
            if (fileBytes == null || fileBytes.Length < 2 || !IsImage(fileName))
                return false;
            var extension = Path.GetExtension(fileName);
            var fileCode = extension == null ? null : FileFormats[extension.ToLower()];

            //文件Code是否符合对应的文件类型。
            return fileCode != null && fileCode.Equals(GetFileCode(fileBytes));
        }

        public static bool IsJpg(byte[] fileBytes)
        {
            var fileCode = GetFileCode(fileBytes);
            return fileCode == "255216";
        }

        /// <summary>
        /// 根据文件内容得到内容类型。
        /// </summary>
        /// <param name="bytes">文件内容。</param>
        /// <returns>内容类型。</returns>
        public static string GetContentType(byte[] bytes)
        {
            var fileCode = GetFileCode(bytes);
            var item = FileFormats.First(i => i.Value.Equals(fileCode));
            var extensions = item.Key;
            return ContentTypeExtensionsMapping.ContainsKey(extensions)
                ? ContentTypeExtensionsMapping[extensions]
                : null;
        }

        /// <summary>
        /// 获取一个随机的文件名称。
        /// </summary>
        /// <param name="data">文件字节组（主要用来得到文件扩展名）。</param>
        /// <returns>一个随机的文件名称。</returns>
        public static string GetRandomFileName(byte[] data)
        {
            var fileCode = GetFileCode(data);
            var item = FileFormats.First(i => i.Value.Equals(fileCode));
            return Guid.NewGuid().ToString("n") + item.Key;
        }

        /// <summary>
        /// 根据文件字节组得到文件扩展名。
        /// </summary>
        /// <param name="bytes">文件字节组。</param>
        /// <returns>扩展名称（如：.jpg）。</returns>
        public static string GetExtensions(byte[] bytes)
        {
            var fileCode = GetFileCode(bytes);
            return FileFormats.FirstOrDefault(i => i.Value.Equals(fileCode)).Key;
        }

        #region Private Method

        private static string GetFileCode(byte[] bytes)
        {
            return bytes[0].ToString(CultureInfo.InvariantCulture) + bytes[1];
        }

        #endregion Private Method

        /*
                /// <summary>
                /// 根据文件名称的扩展名获取一个随机的文件名称。
                /// </summary>
                /// <param name="fileName">文件名称。</param>
                /// <param name="extension">文件扩展名（需要加.）。</param>
                /// <returns>随机的文件名称。</returns>
                public static string GetRandomFileNameByFileName(string fileName, string extension = null)
                {
                    if (string.IsNullOrWhiteSpace(extension))
                        extension = Path.GetExtension(fileName);
                    else if (!extension.StartsWith("."))
                        extension = extension.Insert(0, ".");

                    var name = DateTime.Now.ToString("yyyyMMddHHmmss_") + new Random().Next(1000, 9999);
                    return extension == null ? name : name + extension;
                }

                /// <summary>
                /// 文件是否是一个Png格式的图片。
                /// </summary>
                /// <param name="fileBytes">文件字节组。</param>
                /// <returns>如果是一张Png则返回true，否则返回false。</returns>
                public static bool IsPng(byte[] fileBytes)
                {
                    return IsFormat(".png", fileBytes);
                }

                /// <summary>
                /// 文件是否是一个Zip格式的文件。
                /// </summary>
                /// <param name="fileName">文件名称。</param>
                /// <returns>如果是Zip文件则返回true，否则返回false。</returns>
                public static bool IsZip(string fileName)
                {
                    fileName.NotEmptyOrWhiteSpace("fileName");

                    return string.Equals(".zip", Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase);
                }

                /// <summary>
                /// 文件是否是一个Zip格式的文件。
                /// </summary>
                /// <param name="fileName">文件名称。</param>
                /// <param name="stream">文件流。</param>
                /// <returns>如果是Zip文件则返回true，否则返回false。</returns>
                public static bool IsZip(string fileName, Stream stream)
                {
                    stream.NotNull("stream");

                    if (!IsZip(fileName) || stream.Length < 2)
                        return false;

                    var bytes = new[] { (byte)stream.ReadByte(), (byte)stream.ReadByte() };

                    stream.Seek(0, SeekOrigin.Begin);

                    return IsFormat(".zip", bytes);
                }

                /// <summary>
                /// 文件是否是一个Zip格式的文件。
                /// </summary>
                /// <param name="fileBytes">文件字节组。</param>
                /// <returns>如果是Zip文件则返回true，否则返回false。</returns>
                public static bool IsZip(byte[] fileBytes)
                {
                    return IsFormat(".zip", fileBytes);
                }

                private static bool IsFormat(string extension, byte[] fileBytes)
                {
                    if (fileBytes == null || fileBytes.Length < 2 || !FileFormats.ContainsKey(extension))
                        return false;

                    var fileCode = FileFormats[extension];
                    return fileCode.Equals(fileBytes[0].ToString(CultureInfo.InvariantCulture) + fileBytes[1]);
                }*/
    }
}