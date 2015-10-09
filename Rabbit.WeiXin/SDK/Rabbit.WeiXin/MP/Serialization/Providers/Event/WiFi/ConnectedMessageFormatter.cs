using Rabbit.WeiXin.MP.Messages.Events.WiFi;
using Rabbit.WeiXin.Utility;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event.WiFi
{
    internal sealed class ConnectedMessageFormatter : XmlMessageFormatterBase<ConnectedMessage>
    {
        #region Overrides of XmlMessageFormatterBase<ConnectedMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override ConnectedMessage Deserialize(XContainer container)
        {
            return SetBaseInfo(container, new ConnectedMessage
            {
                ConnectTime = DateTimeHelper.GetTimeByTimeStamp(GetULong(container, "ConnectTime")),
                PlaceId = GetValue(container, "PlaceId"),
                DeviceNo = GetValue(container, "DeviceNo")
            });
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(ConnectedMessage graph)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<ConnectedMessage>
    }
}