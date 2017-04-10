namespace Rabbit.WeiXin.MP.Messages.Request
{
    /// <summary>
    /// 地理位置请求消息。
    /// </summary>
    public class RequestMessageLocation : RequestMessageBase
    {
        /// <summary>
        /// 地理位置维度
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

        #region Overrides of MessageBase

        /// <summary>
        /// 消息类型。
        /// </summary>
        public override RequestMessageType MessageType
        {
            get { return RequestMessageType.Location; }
        }

        #endregion Overrides of MessageBase
    }
}