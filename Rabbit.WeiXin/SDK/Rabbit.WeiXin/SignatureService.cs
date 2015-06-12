using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Rabbit.WeiXin
{
    /// <summary>
    /// 一个抽象的签名服务。
    /// </summary>
    public interface ISignatureService
    {
        /// <summary>
        /// 签名检验。
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="token">令牌。</param>
        /// <returns>检验成功则返回true，否则返回false。</returns>
        bool Check(string signature, string timestamp, string nonce, string token);
    }

    internal sealed class SignatureService : ISignatureService
    {
        #region Implementation of ISignatureService

        /// <summary>
        /// 签名检验。
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="token">令牌。</param>
        /// <returns>检验成功则返回true，否则返回false。</returns>
        public bool Check(string signature, string timestamp, string nonce, string token)
        {
            return signature.Equals(GetSignature(timestamp, nonce, token));
        }

        #endregion Implementation of ISignatureService

        #region Private Method

        private static string GetSignature(string timestamp, string nonce, string token = null)
        {
            var arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
            var arrString = string.Join("", arr);
            var sha1 = SHA1.Create();
            var sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
            var enText = new StringBuilder();
            foreach (var b in sha1Arr)
            {
                enText.AppendFormat("{0:x2}", b);
            }

            return enText.ToString();
        }

        #endregion Private Method
    }
}