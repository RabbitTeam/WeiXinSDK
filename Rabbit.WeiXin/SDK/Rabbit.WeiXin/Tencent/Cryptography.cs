using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Tencent
{
    /// <summary>
    /// 腾讯提供的微信加解密服务。
    /// </summary>
    public class Cryptography
    {
        private static int HostToNetworkOrder(int inval)
        {
            var outval = 0;
            for (var i = 0; i < 4; i++)
                outval = (outval << 8) + ((inval >> (i * 8)) & 255);
            return outval;
        }

        ///  <summary>
        ///  AES解密方法。
        ///  </summary>
        ///  <param name="input">需要解密的内容。</param>
        ///  <param name="encodingAesKey">解密Key。</param>
        /// <param name="appId">公众号的AppId。</param>
        /// <returns>解密后的内容。</returns>
        public static string AES_decrypt(string input, string encodingAesKey, out string appId)
        {
            var key = Convert.FromBase64String(encodingAesKey + "=");
            var iv = new byte[16];
            Array.Copy(key, iv, 16);
            var btmpMsg = AES_decrypt(input, iv, key);

            var len = BitConverter.ToInt32(btmpMsg, 16);
            len = IPAddress.NetworkToHostOrder(len);

            var bMsg = new byte[len];
            var bAppid = new byte[btmpMsg.Length - 20 - len];
            Array.Copy(btmpMsg, 20, bMsg, 0, len);
            Array.Copy(btmpMsg, 20 + len, bAppid, 0, btmpMsg.Length - 20 - len);
            var oriMsg = Encoding.UTF8.GetString(bMsg);
            appId = Encoding.UTF8.GetString(bAppid);

            return oriMsg;
        }

        /// <summary>
        /// AES加密。
        /// </summary>
        /// <param name="input">需要加密的内容。</param>
        /// <param name="encodingAesKey">加密Key。</param>
        /// <param name="appId">公众号的AppId。</param>
        /// <returns>加密后的内容。</returns>
        public static string AES_encrypt(string input, string encodingAesKey, string appId)
        {
            var key = Convert.FromBase64String(encodingAesKey + "=");
            var iv = new byte[16];
            Array.Copy(key, iv, 16);
            var randcode = CreateRandCode(16);
            var bRand = Encoding.UTF8.GetBytes(randcode);
            var bAppid = Encoding.UTF8.GetBytes(appId);
            var btmpMsg = Encoding.UTF8.GetBytes(input);
            var bMsgLen = BitConverter.GetBytes(HostToNetworkOrder(btmpMsg.Length));
            var bMsg = new byte[bRand.Length + bMsgLen.Length + bAppid.Length + btmpMsg.Length];

            Array.Copy(bRand, bMsg, bRand.Length);
            Array.Copy(bMsgLen, 0, bMsg, bRand.Length, bMsgLen.Length);
            Array.Copy(btmpMsg, 0, bMsg, bRand.Length + bMsgLen.Length, btmpMsg.Length);
            Array.Copy(bAppid, 0, bMsg, bRand.Length + bMsgLen.Length + btmpMsg.Length, bAppid.Length);

            return AES_encrypt(bMsg, iv, key);
        }

        private static string CreateRandCode(int codeLen)
        {
            const string codeSerial = "2,3,4,5,6,7,a,c,d,e,f,h,i,j,k,m,n,p,r,s,t,A,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,U,V,W,X,Y,Z";
            if (codeLen == 0)
            {
                codeLen = 16;
            }
            var arr = codeSerial.Split(',');
            var code = string.Empty;
            var rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (var i = 0; i < codeLen; i++)
            {
                var randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }

        private static string AES_encrypt(byte[] input, byte[] iv, byte[] key)
        {
            using (var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Padding = PaddingMode.None,
                Mode = CipherMode.CBC,
                Key = key,
                IV = iv
            })
            {
                //秘钥的大小，以位为单位
                //支持的块大小
                //填充模式
                //aes.Padding = PaddingMode.PKCS7;
                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff;

                #region 自己进行PKCS7补位，用系统自己带的不行

                var msg = new byte[input.Length + 32 - input.Length % 32];
                Array.Copy(input, msg, input.Length);
                var pad = Kcs7Encoder(input.Length);
                Array.Copy(pad, 0, msg, input.Length, pad.Length);

                #endregion 自己进行PKCS7补位，用系统自己带的不行

                #region 注释的也是一种方法，效果一样

                //ICryptoTransform transform = aes.CreateEncryptor();
                //byte[] xBuff = transform.TransformFinalBlock(msg, 0, msg.Length);

                #endregion 注释的也是一种方法，效果一样

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(msg, 0, msg.Length);
                        xBuff = ms.ToArray();
                    }
                }

                var output = Convert.ToBase64String(xBuff);
                return output;
            }
        }

        private static byte[] Kcs7Encoder(int textLength)
        {
            const int blockSize = 32;
            // 计算需要填充的位数
            var amountToPad = blockSize - (textLength % blockSize);
            if (amountToPad == 0)
            {
                amountToPad = blockSize;
            }
            // 获得补位所用的字符
            var padChr = Chr(amountToPad);
            var tmp = string.Empty;
            for (var index = 0; index < amountToPad; index++)
            {
                tmp += padChr;
            }
            return Encoding.UTF8.GetBytes(tmp);
        }

        /**
         * 将数字转化成ASCII码对应的字符，用于对明文进行补码
         *
         * @param a 需要转化的数字
         * @return 转化得到的字符
         */

        private static char Chr(int a)
        {
            var target = (byte)(a & 0xFF);
            return (char)target;
        }

        private static byte[] AES_decrypt(string input, byte[] iv, byte[] key)
        {
            using (var aes = new RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.None,
                Key = key,
                IV = iv
            })
            {
                var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
                byte[] xBuff;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        var xXml = Convert.FromBase64String(input);
                        var msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                        Array.Copy(xXml, msg, xXml.Length);
                        cs.Write(xXml, 0, xXml.Length);
                        xBuff = Decode2(ms.ToArray());
                    }
                }
                return xBuff;
            }
        }

        private static byte[] Decode2(byte[] decrypted)
        {
            var pad = decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            var res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }
    }
}