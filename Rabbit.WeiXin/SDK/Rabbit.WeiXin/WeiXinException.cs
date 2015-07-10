using System;
using System.Runtime.Serialization;

namespace Rabbit.WeiXin
{
    /// <summary>
    /// 微信异常信息。
    /// </summary>
    [Serializable]
    public class WeiXinException : Exception
    {
        /// <summary>
        /// 初始化一个新的微信异常。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误消息。</param>
        public WeiXinException(int errorCode, string message)
            : base(string.Format("调用接口失败，错误码：{0}，错误信息：{1}", errorCode, message))
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode { get; set; }

        #region Overrides of Exception

        /// <summary>
        /// 当在派生类中重写时，用关于异常的信息设置 <see cref="T:System.Runtime.Serialization.SerializationInfo"/>。
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo"/>，它存有有关所引发的异常的序列化对象数据。</param><param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext"/>，它包含有关源或目标的上下文信息。</param><exception cref="T:System.ArgumentNullException"><paramref name="info"/> 参数是空引用（Visual Basic 中为 Nothing）。</exception><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ErrorCode", ErrorCode);
            base.GetObjectData(info, context);
        }

        #endregion Overrides of Exception
    }
}