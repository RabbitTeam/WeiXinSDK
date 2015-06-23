using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Store
{
    /// <summary>
    /// 门店状态。
    /// </summary>
    public enum StoreStatus
    {
        /// <summary>
        /// 系统错误。
        /// </summary>
        SystemError = 1,

        /// <summary>
        /// 审核中。
        /// </summary>
        Checking = 2,

        /// <summary>
        /// 审核通过。
        /// </summary>
        Checked = 3,

        /// <summary>
        /// 审核失败。
        /// </summary>
        CheckFailure = 4
    }

    /// <summary>
    /// 门店更新状态。
    /// </summary>
    public enum StoreUpdateStatus
    {
        /// <summary>
        /// 扩展字段正在更新中，尚未生效， 不允许再次更新。
        /// </summary>
        Updating = 1,

        /// <summary>
        /// 扩展字段没有在更新中或更新已生效， 可以再次更新。
        /// </summary>
        Updated = 0
    }

    /// <summary>
    /// 门店简单的信息模型。
    /// </summary>
    public class StoreInfoSimpleModel
    {
        /// <summary>
        /// 门店的电话（纯数字，区号、分机号均由“-”隔开）。
        /// </summary>
        [Required, JsonProperty("telephone")]
        public string TelePhone { get; set; }

        /// <summary>
        /// 图片列表，url形式，可以有多张图片，尺寸为 640*340px。必须为上一接口生成的 url。
        /// </summary>
        [Required, JsonIgnore]
        public string[] PhotoList { get; set; }

        /// <summary>
        /// 推荐品，餐厅可为推荐菜；酒店为推荐套房；景点为 推荐游玩景点等，针对自己行业的推荐内容。
        /// </summary>
        [JsonProperty("recommend")]
        public string Recommend { get; set; }

        /// <summary>
        /// 特色服务，如免费wifi，免费停车，送货上门等商户 能提供的特色功能或服务。
        /// </summary>
        [Required, JsonProperty("special")]
        public string Special { get; set; }

        /// <summary>
        /// 商户简介，主要介绍商户信息等。
        /// </summary>
        [JsonProperty("introduction")]
        public string Introduction { get; set; }

        /// <summary>
        /// 营业时间，24小时制表示，用“-”连接，如 8:00-20:00。
        /// </summary>
        [Required, JsonProperty("open_time")]
        internal string OpenTime
        {
            get { return StartTime.ToString("h':'mm") + "-" + EndTime.ToString("h':'mm"); }
            set
            {
                var temp = value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                StartTime = TimeSpan.Parse(temp[0]);
                EndTime = TimeSpan.Parse(temp[1]);
            }
        }

        /// <summary>
        /// 开始营业的时间。
        /// </summary>
        [Required, JsonIgnore]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// 结束营业的时间。
        /// </summary>
        [Required, JsonIgnore]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// 人均价格，大于0的整数。
        /// </summary>
        [JsonProperty("avg_price")]
        public uint AvgPrice { get; set; }
    }

    /// <summary>
    /// 门店信息模型。
    /// </summary>
    public class StoreInfoModelBase : StoreInfoSimpleModel
    {
        /// <summary>
        /// 商户自己的 id，用于后续审核通过收到 poi_id 的通 知时，做对应关系。请商户自己保证唯一识别。
        /// </summary>
        [JsonProperty("sid")]
        public string CustomStoreId { get; set; }

        /// <summary>
        /// 门店名称（仅为商户名，如：国美、麦当劳，不应包 含地区、店号等信息，错误示例：北京国美）。
        /// </summary>
        [Required, JsonProperty("business_name")]
        public string Name { get; set; }

        /// <summary>
        /// 分店名称（不应包含地区信息、不应与门店名重复， 错误示例：北京王府井店。
        /// </summary>
        [JsonProperty("branch_name")]
        public string BranchName { get; set; }

        /// <summary>
        /// 门店所在的省份（直辖市填城市名,如：北京市）。
        /// </summary>
        [Required, JsonProperty("province")]
        public string Province { get; set; }

        /// <summary>
        /// 门店所在的城市。
        /// </summary>
        [Required, JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// 门店所在地区。
        /// </summary>
        [JsonProperty("district")]
        public string District { get; set; }

        /// <summary>
        /// 门店所在的详细街道地址（不要填写省市信息）
        /// </summary>
        [Required, JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// 门店的类型（详细分类参见分类附表，不同级分类用 “,”隔开，如：美食，川菜，火锅）。
        /// </summary>
        [Required, JsonProperty("categories")]
        internal string[] CategoriesString
        {
            get
            {
                return new[] { Categories == null ? string.Empty : string.Join(",", Categories) };
            }
            set
            {
                var result = value.FirstOrDefault();
                if (result == null)
                {
                    Categories = new Collection<string>();
                }
                else
                {
                    Categories = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
        }

        /// <summary>
        /// 门店的类型（详细分类参见分类附表）。
        /// </summary>
        [Required, JsonIgnore]
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// 门店所在地理位置的经度
        /// </summary>
        [Required, JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// 门店所在地理位置的纬度（经纬度均为火星坐标，最 好选用腾讯地图标记的坐标）
        /// </summary>
        [Required, JsonProperty("latitude")]
        public double Latitude { get; set; }
    }

    /// <summary>
    /// 创建门店的模型。
    /// </summary>
    public sealed class CreateStoreModel : StoreInfoModelBase
    {
    }

    /// <summary>
    /// 门店信息模型。
    /// </summary>
    public sealed class StoreInfoModel : StoreInfoModelBase
    {
        /// <summary>
        /// 门店Id。
        /// </summary>
        [JsonProperty("poi_id")]
        public ulong? StoreId { get; set; }

        /// <summary>
        /// 门店状态数字。
        /// </summary>
        [JsonProperty("available_state")]
        internal int StatusNumber
        {
            get
            {
                return (int)Status;
            }
            set { Status = (StoreStatus)Enum.Parse(typeof(StoreStatus), value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// 门店更新状态数组。
        /// </summary>
        [JsonProperty("update_status")]
        internal int UpdateStatusNumber
        {
            get
            {
                return (int)UpdateStatus;
            }
            set { UpdateStatus = (StoreUpdateStatus)Enum.Parse(typeof(StoreUpdateStatus), value.ToString(CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// 门店状态（不为CheckedStoreId为null）。
        /// </summary>
        [JsonIgnore]
        public StoreStatus Status { get; set; }

        /// <summary>
        /// 门店更新状态。
        /// </summary>
        [JsonIgnore]
        public StoreUpdateStatus UpdateStatus { get; set; }
    }

    /// <summary>
    /// 更新门店模型。
    /// </summary>
    public sealed class UpdateStoreModel : StoreInfoSimpleModel
    {
        /// <summary>
        /// 门店Id。
        /// </summary>
        [JsonProperty("poi_id")]
        public ulong? StoreId { get; set; }
    }

    /// <summary>
    /// 一个抽象的门店服务。
    /// </summary>
    public interface IStoreService
    {
        /// <summary>
        /// 上传图片。
        /// </summary>
        /// <param name="data">图片数据（大小限制1MB，支持JPG格式）。</param>
        /// <returns>如果上传成功则返回图片的绝对url地址。</returns>
        string UploadImage(byte[] data);

        /// <summary>
        /// 创建门店。
        /// </summary>
        /// <param name="model">门店模型。</param>
        void Create(CreateStoreModel model);

        /// <summary>
        /// 获取门店信息。
        /// </summary>
        /// <param name="storeId">门店Id（服务器推送的数据）。</param>
        /// <returns>门店信息。</returns>
        StoreInfoModel Get(ulong storeId);

        /// <summary>
        /// 获取门店列表。
        /// </summary>
        /// <param name="skip">跳过多少条数据。</param>
        /// <param name="take">获取多少条数据（最大为50）。</param>
        /// <returns>门店信息数组。</returns>
        StoreInfoModel[] GetList(uint skip = 0, ushort take = 20);

        /// <summary>
        /// 删除门店。
        /// </summary>
        /// <param name="storeId">门店Id。</param>
        /// <remarks>删除已经成功创建的门店。请商户慎重调用该接口，门店信息被删除后，可能会影响其他与门店相关的业务使用，如卡券等。同样，该门店信息也不会在微信的商户详情页显示，不会再推荐入附近功能。</remarks>
        void Delete(ulong storeId);

        /// <summary>
        /// 更新门店。
        /// </summary>
        /// <param name="model">更新门店模型。</param>
        void Update(UpdateStoreModel model);
    }

    /// <summary>
    /// 门店服务实现。
    /// </summary>
    public sealed class StoreService : IStoreService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        public StoreService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IStoreService

        /// <summary>
        /// 上传图片。
        /// </summary>
        /// <param name="data">图片数据（大小限制1MB，支持JPG格式）。</param>
        /// <returns>如果上传成功则返回图片的绝对url地址。</returns>
        public string UploadImage(byte[] data)
        {
            var url = "http://file.api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + _accountModel.GetAccessToken();
            var createBytes = new CreateBytes();
            var postData = createBytes.JoinBytes(new ArrayList
            {
                createBytes.CreateFieldData("buffer", FileHelper.GetRandomFileName(data), "image/jpeg", data)
            });
            var json = WeiXinHttpHelper.PostString(url, postData, createBytes.ContentType);
            return JObject.Parse(json)["url"].Value<string>();
        }

        /// <summary>
        /// 创建门店。
        /// </summary>
        /// <param name="model">门店模型。</param>
        public void Create(CreateStoreModel model)
        {
            var url = "http://api.weixin.qq.com/cgi-bin/poi/addpoi?access_token=" + _accountModel.GetAccessToken();
            var json = JsonConvert.SerializeObject(new
            {
                business = new
                {
                    base_info = model
                }
            });
            var obj = JObject.Parse(json);
            var baseObj = obj["business"]["base_info"];
            baseObj["photo_list"] = JArray.Parse(JsonConvert.SerializeObject(model.PhotoList.Select(i => new { photo_url = i })));
            baseObj["offset_type"] = 1;

            WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(obj.ToString()));
        }

        /// <summary>
        /// 获取门店信息。
        /// </summary>
        /// <param name="storeId">门店Id（服务器推送的数据）。</param>
        /// <returns>门店信息。</returns>
        public StoreInfoModel Get(ulong storeId)
        {
            var url = "http://api.weixin.qq.com/cgi-bin/poi/getpoi?access_token=" + _accountModel.GetAccessToken();

            var json = WeiXinHttpHelper.PostString(url, new { poi_id = storeId });
            var infoObj = JObject.Parse(json)["business"]["base_info"];

            var infoJson = infoObj.ToString();
            var model = JsonConvert.DeserializeObject<StoreInfoModel>(infoJson);
            model.PhotoList = infoObj["photo_list"].Select(i => i["photo_url"].Value<string>()).ToArray();
            return model;
        }

        /// <summary>
        /// 获取门店列表。
        /// </summary>
        /// <param name="skip">跳过多少条数据。</param>
        /// <param name="take">获取多少条数据（最大为50）。</param>
        /// <returns>门店信息数组。</returns>
        public StoreInfoModel[] GetList(uint skip = 0, ushort take = 20)
        {
            var url = "http://api.weixin.qq.com/cgi-bin/poi/getpoilist?access_token=" + _accountModel.GetAccessToken();
            var json = WeiXinHttpHelper.PostString(url, new
            {
                begin = skip,
                limit = take
            });
            var array = (JArray)JObject.Parse(json)["business_list"];

            return array.Select(i =>
            {
                var model = JsonConvert.DeserializeObject<StoreInfoModel>(i.ToString());
                var baseInfo = i["base_info"];
                var photoList = (JArray)baseInfo["photo_list"];
                model.PhotoList = photoList.Select(z => z["photo_url"].Value<string>()).ToArray();
                return model;
            }).ToArray();
        }

        /// <summary>
        /// 删除门店。
        /// </summary>
        /// <param name="storeId">门店Id。</param>
        /// <remarks>删除已经成功创建的门店。请商户慎重调用该接口，门店信息被删除后，可能会影响其他与门店相关的业务使用，如卡券等。同样，该门店信息也不会在微信的商户详情页显示，不会再推荐入附近功能。</remarks>
        public void Delete(ulong storeId)
        {
            var url = "http://api.weixin.qq.com/cgi-bin/poi/delpoi?access_token=" + _accountModel.GetAccessToken();

            WeiXinHttpHelper.Post(url, new
            {
                poi_id = storeId
            });
        }

        /// <summary>
        /// 更新门店。
        /// </summary>
        /// <param name="model">更新门店模型。</param>
        public void Update(UpdateStoreModel model)
        {
            var url = "http://api.weixin.qq.com/cgi-bin/poi/updatepoi?access_token=" + _accountModel.GetAccessToken();

            var json = JsonConvert.SerializeObject(model);

            var obj = JObject.Parse(json);
            if (model.PhotoList != null)
                obj["photo_list"] = new JArray(model.PhotoList.Select(i => new { photo_url = i }));

            WeiXinHttpHelper.PostString(url, Encoding.UTF8.GetBytes(obj.ToString()));
        }

        #endregion Implementation of IStoreService
    }
}