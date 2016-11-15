using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System.Collections;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Sport
{
    /// <summary>
    /// 获取deviceid和二维码串服务接口
    /// </summary>
    public interface IDeviceQrTicketService
    {
        DeviceQrTicketModel Get(string productId = "1");

        DeviceAuthorizeRetBaseInfo DeviceAuthorize(string deviceId, string mac);

        string GetDeviceBindOpenId(string deviceType, string deviceId);

        string SetStep(string openid, long timestamp, int step);

        bool BindDevice();

        bool UnBindDevice();
    }

    public sealed class DeviceQrTicketService : IDeviceQrTicketService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个卡券服务。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public DeviceQrTicketService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IDeviceQrTicketService

        public DeviceQrTicketModel Get(string productId = "1")
        {
            var url = "https://api.weixin.qq.com/device/getqrcode?access_token=" + _accountModel.GetAccessToken() + "&product_id=" + productId;
            return WeiXinHttpHelper.GetResultByJson<DeviceQrTicketModel>(url);
        }

        public DeviceAuthorizeRetBaseInfo DeviceAuthorize(string deviceId, string mac)
        {
            var url = "https://api.weixin.qq.com/device/authorize_device?access_token=" + _accountModel.GetAccessToken();

            var deviceList = new ArrayList
            {
                new {
                id = deviceId,
                mac,
                connect_protocol = "1",
                auth_key = "",
                close_strategy = "1",
                conn_strategy = "1",
                crypt_method = "0",
                auth_ver = "0",
                manu_mac_pos = "-1",
                ser_mac_pos = "-1"
                }
            };
            object postData = new { device_num = "1", device_list = deviceList, op_type = "1" };
            var postJson = JsonConvert.SerializeObject(postData);
            var postObj = JObject.Parse(postJson);
            var content = postObj.ToString();
            var json = WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(content));
            var infoObj = JObject.Parse(json)["resp"][0]["base_info"];
            var infoJson = infoObj.ToString();
            var model = JsonConvert.DeserializeObject<DeviceAuthorizeRetBaseInfo>(infoJson);
            return model;
        }

        public string GetDeviceBindOpenId(string deviceType, string deviceId)
        {
            var url = $"https://api.weixin.qq.com/device/get_openid?access_token={_accountModel.GetAccessToken()}&device_type={deviceType}&device_id={deviceId}";
            var infoObj = WeiXinHttpHelper.GetString(url);
            var openIdList = JObject.Parse(infoObj)["open_id"].ToArray();
            return openIdList.Any() ? openIdList.First().ToString() : "";
        }

        public string SetStep(string openid, long timestamp, int step)
        {
            var url = $"https://api.weixin.qq.com/hardware/bracelet/setstep?access_token={_accountModel.GetAccessToken()}&openid={openid}&timestamp={timestamp}&step={step}";
            // TODO 检查是否成功
            return WeiXinHttpHelper.GetString(url);
        }

        public bool BindDevice()
        {
            return true;
        }

        public bool UnBindDevice()
        {
            return true;
        }

        #endregion Implementation of IDeviceQrTicketService
    }

    public sealed class DeviceQrTicketModel
    {
        /// <summary>
        /// 微信运动设备id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 获取的二维码串
        /// </summary>
        public string QrTicket { get; set; }
    }

    public sealed class DeviceAuthorizeRetBaseInfo
    {
        /// <summary>
        /// 设备id
        /// </summary>
        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [JsonProperty("device_type")]
        public string DeviceType { get; set; }
    }
}