using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Rabbit.WeiXin.Tests.Utility
{
    public static class ApiTestHelper
    {
        private static readonly ConcurrentDictionary<string, byte[]> Dictionary = new ConcurrentDictionary<string, byte[]>();

        public const string MusicUrl = "http://www.chunsun.cc/星辰泪.mp3";
        public const string HqMusicUrl = "http://www.chunsun.cc/星辰泪.mp3";

        public static byte[] GetJpgBytes()
        {
            return GetByte("1.jpg");
        }

        public static byte[] GetAmrBytes()
        {
            return GetByte("1.amr");
        }

        public static byte[] GetMp3Bytes()
        {
            return GetByte("1.mp3");
        }

        public static byte[] GetMp4Bytes()
        {
            return GetByte("1.mp4");
        }

        #region Private Method

        private static byte[] GetByte(string name)
        {
            var assembly = Assembly.Load(new AssemblyName(typeof(ApiTestHelper).AssemblyQualifiedName));
            using (var stream = assembly.GetManifestResourceStream("Rabbit.WeiXin.Tests.Resoueces." + name))
            {
                if (stream == null)
                    throw new Exception("找不到资源：" + name);
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        #endregion Private Method
    }
}