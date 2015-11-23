using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using Rabbit.WeiXin.Utility.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Rabbit.WeiXin.MP.Api.Material
{
    /// <summary>
    /// 一个抽象的永久素材服务接口。
    /// </summary>
    public interface IForeverMaterialService
    {
        /// <summary>
        /// 添加一个其他类型的永久素材。
        /// </summary>
        /// <param name="bytes">素材内容。</param>
        /// <param name="type">素材类型。</param>
        /// <returns>添加素材的结果模型。</returns>
        AddMaterialResultModel AddOther(byte[] bytes, MaterialType type);

        /// <summary>
        /// 获取一个其他类型的永久素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        /// <returns>素材内容。</returns>
        byte[] GetOther(string mediaId);

        /// <summary>
        /// 获取永久素材列表。
        /// </summary>
        /// <param name="filter">获取素材列表的筛选信息。</param>
        /// <returns>获取素材列表的结果模型。</returns>
        GetMaterialListResultModel GetList(GetMaterialListFilter filter);

        /// <summary>
        /// 删除一个永久素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        void Delete(string mediaId);

        /// <summary>
        /// 获取永久素材数量。
        /// </summary>
        /// <returns>获取素材数量的结果模型。</returns>
        GetMaterialCountResultModel GetMaterialCount(string accessToken);

        /// <summary>
        /// 添加一个视频素材。
        /// </summary>
        /// <param name="data">视频内容。</param>
        /// <param name="model">添加视频素材模型。</param>
        /// <returns>添加素材结果模型。</returns>
        AddMaterialResultModel AddVoide(byte[] data, AddVoideMaterialModel model);

        /// <summary>
        /// 获取一个永久的视频素材。
        /// </summary>
        /// <param name="mediaId">视频素材Id。</param>
        /// <returns>视频素材结果。</returns>
        GetVideoResultModel GetVideo(string mediaId);

        /// <summary>
        /// 添加多图文素材。
        /// </summary>
        /// <param name="articles">文章模型。</param>
        /// <returns>图文素材Id。</returns>
        string AddNews(NewsArticleModel[] articles);

        /// <summary>
        /// 获取一个永久的图文素材。
        /// </summary>
        /// <param name="mediaId">图文素材Id。</param>
        /// <returns>图文信息。</returns>
        NewsArticleModel[] GetNews(string mediaId);
    }

    /// <summary>
    /// 永久素材服务的实现。
    /// </summary>
    public sealed class ForeverMaterialService : MaterialServiceBase, IForeverMaterialService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的永久素材服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public ForeverMaterialService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IMediaService

        /// <summary>
        /// 添加一个其他类型的永久素材。
        /// </summary>
        /// <param name="bytes">素材内容。</param>
        /// <param name="type">素材类型。</param>
        /// <returns>添加素材的结果模型。</returns>
        public AddMaterialResultModel AddOther(byte[] bytes, MaterialType type)
        {
            return Upload<AddMaterialResultModel>(string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}", _accountModel.GetAccessToken()), bytes, "media",
                createBytes => new[] { createBytes.CreateFieldData("type", type.ToString().ToLower()) });
        }

        /// <summary>
        /// 获取一个其他类型的永久素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        /// <returns>素材内容。</returns>
        public byte[] GetOther(string mediaId)
        {
            return GetMaterial(mediaId);
        }

        /// <summary>
        /// 获取永久素材列表。
        /// </summary>
        /// <param name="filter">获取素材列表的筛选信息。</param>
        /// <returns>获取素材列表的结果模型。</returns>
        public GetMaterialListResultModel GetList(GetMaterialListFilter filter)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=" + _accountModel.GetAccessToken();
            return WeiXinHttpHelper.PostResultByJson<GetMaterialListResultModel>(url, filter);
        }

        /// <summary>
        /// 删除一个永久素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        public void Delete(string mediaId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=" + _accountModel.GetAccessToken();
            WeiXinHttpHelper.Post(url, new { media_id = mediaId });
        }

        /// <summary>
        /// 获取永久素材数量。
        /// </summary>
        /// <returns>获取素材数量的结果模型。</returns>
        public GetMaterialCountResultModel GetMaterialCount(string accessToken)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token=" + _accountModel.GetAccessToken();
            return WeiXinHttpHelper.GetResultByJson<GetMaterialCountResultModel>(url);
        }

        /// <summary>
        /// 添加一个视频素材。
        /// </summary>
        /// <param name="data">视频内容。</param>
        /// <param name="model">添加视频素材模型。</param>
        /// <returns>添加素材结果模型。</returns>
        public AddMaterialResultModel AddVoide(byte[] data, AddVoideMaterialModel model)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token=" + _accountModel.GetAccessToken();
            return Upload<AddMaterialResultModel>(url, data, "media", createBytes =>
                new[]
                {
                    createBytes.CreateFieldData("description", string.Format("{{\"title\":\"{0}\", \"introduction\":\"{1}\"}}", model.Title, model.Description))
                });
        }

        /// <summary>
        /// 获取一个永久的视频素材。
        /// </summary>
        /// <param name="mediaId">视频素材Id。</param>
        /// <returns>视频素材结果。</returns>
        public GetVideoResultModel GetVideo(string mediaId)
        {
            var data = GetMaterial(mediaId);
            var content = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<GetVideoResultModel>(content);
        }

        /// <summary>
        /// 添加多图文素材。
        /// </summary>
        /// <param name="articles">文章模型。</param>
        /// <returns>图文素材Id。</returns>
        public string AddNews(NewsArticleModel[] articles)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=" + _accountModel.GetAccessToken();

            var data = WeiXinHttpHelper.Post(url, new { articles });
            var content = Encoding.UTF8.GetString(data);
            ResultHelper.TryThrowError(content);
            return JObject.Parse(content).Value<string>("media_id");
        }

        /// <summary>
        /// 获取一个永久的图文素材。
        /// </summary>
        /// <param name="mediaId">图文素材Id。</param>
        /// <returns>图文信息。</returns>
        public NewsArticleModel[] GetNews(string mediaId)
        {
            var data = GetMaterial(mediaId);
            var content = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<NewsArticleModel[]>(((JArray)JObject.Parse(content)["news_item"]).ToString());
        }

        #endregion Implementation of IMediaService

        #region Private Method

        private byte[] GetMaterial(string mediaId)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=" + _accountModel.GetAccessToken();
            return WeiXinHttpHelper.Post(url, new { media_id = mediaId });
        }

        #endregion Private Method
    }

    /// <summary>
    /// 永久素材服务扩展方法。
    /// </summary>
    public static class ForeverMaterialServiceExtensions
    {
        /// <summary>
        /// 添加一个其他类型的永久素材。
        /// </summary>
        /// <param name="foreverMaterialService">永久素材服务。</param>
        /// <param name="bytes">素材内容。</param>
        /// <returns>添加素材的结果模型。</returns>
        public static AddMaterialResultModel AddOther(this IForeverMaterialService foreverMaterialService, byte[] bytes)
        {
            var type = MaterialHelper.GetMaterialType(bytes);
            return foreverMaterialService.NotNull("foreverMaterialService").AddOther(bytes, type);
        }

        /// <summary>
        /// 添加一个缩略图类型的永久素材。
        /// </summary>
        /// <param name="foreverMaterialService">永久素材服务。</param>
        /// <param name="bytes">素材内容。</param>
        /// <returns>添加素材的结果模型。</returns>
        public static AddMaterialResultModel AddOtherThumbnails(this IForeverMaterialService foreverMaterialService, byte[] bytes)
        {
            return foreverMaterialService.NotNull("foreverMaterialService").AddOther(bytes, MaterialType.Thumb);
        }
    }

    #region Help Class

    /// <summary>
    /// 添加永久素材的结果模型。
    /// </summary>
    public class AddMaterialResultModel
    {
        /// <summary>
        /// 素材Id。
        /// </summary>
        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        /// <summary>
        /// 素材Url。
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 获取素材列表的结果模型。
    /// </summary>
    public class GetMaterialListResultModel
    {
        /// <summary>
        /// 素材列表项。
        /// </summary>
        public sealed class MaterialListItem
        {
            /// <summary>
            /// 素材Id。
            /// </summary>
            [JsonProperty("media_id")]
            public string MediaId { get; set; }

            /// <summary>
            /// 素材名称。
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 最后更新时间。
            /// </summary>
            [JsonProperty("update_time")]
            public long LastUpdateTime { get; set; }

            /// <summary>
            /// 素材url。
            /// </summary>
            public string Url { get; set; }
        }

        /// <summary>
        /// 该类型的素材的总数
        /// </summary>
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        /// <summary>
        /// 本次调用获取的素材的数量
        /// </summary>
        [JsonProperty("item_count")]
        public int Count { get; set; }

        /// <summary>
        /// 素材项数组。
        /// </summary>
        [JsonProperty("item")]
        public MaterialListItem[] Items { get; set; }
    }

    /// <summary>
    /// 获取素材列表的筛选信息。
    /// </summary>
    public class GetMaterialListFilter
    {
        /// <summary>
        /// 素材类型。
        /// </summary>
        [JsonIgnore]
        public MaterialType Type { get; set; }

        /// <summary>
        /// 素材类型的字符串信息。
        /// </summary>
        [JsonProperty("type")]
        public string TypeString
        {
            get { return Type.ToString().ToLower(); }
        }

        /// <summary>
        /// 跳过多少条数据后开始获取。
        /// </summary>
        [JsonProperty("offset")]
        public int Skip { get; set; }

        /// <summary>
        /// 希望得到多少条数据。
        /// </summary>
        [JsonProperty("count")]
        public ushort Take { get; set; }
    }

    /// <summary>
    /// 获取素材数量的结果模型。
    /// </summary>
    public class GetMaterialCountResultModel
    {
        /// <summary>
        /// 语音素材数量。
        /// </summary>
        [JsonProperty("voice_count")]
        public uint VoiceCount { get; set; }

        /// <summary>
        /// 视频素材数量。
        /// </summary>
        [JsonProperty("video_count")]
        public uint VideoCount { get; set; }

        /// <summary>
        /// 图片素材数量。
        /// </summary>
        [JsonProperty("image_count")]
        public uint ImageCount { get; set; }

        /// <summary>
        /// 图文素材数量。
        /// </summary>
        [JsonProperty("news_count")]
        public uint NewsCount { get; set; }
    }

    /// <summary>
    /// 图文文章模型。
    /// </summary>
    public class NewsArticleModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 图文消息的封面图片素材id（必须是永久mediaID）
        /// </summary>
        [JsonProperty("thumb_media_id")]
        public string ThumbnailsMediaId { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }

        /// <summary>
        /// 图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
        /// </summary>
        [JsonProperty("digest")]
        public string Description { get; set; }

        /// <summary>
        /// 是否显示封面
        /// </summary>
        [JsonIgnore]
        public bool IsShowConverPicture
        {
            get { return IsShowConverPictureNumber == 1; }
            set
            {
                IsShowConverPictureNumber = value ? (ushort)1 : (ushort)0;
            }
        }

        /// <summary>
        /// 是否显示封面
        /// </summary>
        [JsonProperty("show_cover_pic")]
        internal ushort IsShowConverPictureNumber { get; set; }

        /// <summary>
        /// 图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 图文消息的原文地址，即点击“阅读原文”后的URL
        /// </summary>
        [JsonProperty("content_source_url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// 获取视频素材的结果模型。
    /// </summary>
    public class GetVideoResultModel
    {
        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 说明。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 视频地址。
        /// </summary>
        [JsonProperty("down_url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// 添加视频素材模型。
    /// </summary>
    public class AddVoideMaterialModel
    {
        /// <summary>
        /// 视频素材的标题
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 视频素材的描述
        /// </summary>
        [Required]
        public string Description { get; set; }
    }

    #endregion Help Class
}