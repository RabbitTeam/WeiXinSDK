using Rabbit.WeiXin.MP.Messages.Events;
using System.Linq;
using System.Xml.Linq;

namespace Rabbit.WeiXin.MP.Serialization.Providers.Event
{
    internal sealed class ShakearoundUserShakeMessageFormatter : XmlMessageFormatterBase<ShakearoundUserShakeMessage>
    {
        #region Overrides of XmlMessageFormatterBase<ShakearoundUserShakeMessage>

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="container">Xml容器。</param>
        /// <returns>消息实例。</returns>
        public override ShakearoundUserShakeMessage Deserialize(XContainer container)
        {
            var message = SetBaseInfo(container, new ShakearoundUserShakeMessage());

            message.ChosenBeacon = GetBeaconInfo(container.Element("ChosenBeacon"));
            message.AroundBeacons =
                container.Element("AroundBeacons").Elements("AroundBeacon").Select(GetBeaconInfo).ToArray();

            return message;
        }

        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="graph">消息实例。</param>
        /// <returns>xml内容。</returns>
        public override string Serialize(ShakearoundUserShakeMessage graph)
        {
            throw new System.NotImplementedException();
        }

        #endregion Overrides of XmlMessageFormatterBase<ShakearoundUserShakeMessage>

        #region Private Method

        private ShakearoundUserShakeMessage.BeaconInfo GetBeaconInfo(XElement container)
        {
            return new ShakearoundUserShakeMessage.BeaconInfo
            {
                Uuid = GetValue(container, "Uuid"),
                Major = GetValue(container, "Major"),
                Minor = GetValue(container, "Minor"),
                Distance = GetDouble(container, "Distance")
            };
        }

        #endregion Private Method
    }
}