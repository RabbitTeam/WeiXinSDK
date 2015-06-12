using Rabbit.WeiXin.Api.Utility;
using System;

namespace Rabbit.WeiXin.Api.Material
{
    /// <summary>
    /// 素材帮助。
    /// </summary>
    internal static class MaterialHelper
    {
        /// <summary>
        /// 根据文件字节组得到素材类型（缩略图类型除外）。
        /// </summary>
        /// <param name="bytes">文件字节组。</param>
        /// <returns>素材类型。</returns>
        public static MaterialType GetMaterialType(byte[] bytes)
        {
            MaterialType type;
            var extension = FileHelper.GetExtensions(bytes);
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    type = MaterialType.Image;
                    break;

                case ".amr":
                case ".mp3":
                    type = MaterialType.Voice;
                    break;

                case ".mp4":
                    type = MaterialType.Video;
                    break;

                default:
                    throw new NotSupportedException("不支持的文件扩展名：" + extension);
            }
            return type;
        }
    }
}