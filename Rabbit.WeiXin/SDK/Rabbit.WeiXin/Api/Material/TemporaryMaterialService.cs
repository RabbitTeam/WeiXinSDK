using ChunSun.PublicPlatform.Services.Utility;
using Newtonsoft.Json;
using Rabbit.WeiXin.Api.Utility;
using Rabbit.WeiXin.Utility;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Rabbit.WeiXin.Api.Material
{
    /// <summary>
    /// 一个抽象的临时素材服务接口。
    /// </summary>
    public interface ITemporaryMaterialService
    {
        /// <summary>
        /// 添加一个临时素材。
        /// </summary>
        /// <param name="bytes">素材内容。</param>
        /// <param name="type">素材类型。</param>
        /// <returns>添加临时素材的结果模型。</returns>
        AddTemporaryMaterialResultModel AddTemporary(byte[] bytes, MaterialType type);

        /// <summary>
        /// 获取一个临时素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        /// <returns>临时素材模型。</returns>
        TemporaryMaterialModel GetTemporary(string mediaId);
    }

    /// <summary>
    /// 临时素材服务扩展方法。
    /// </summary>
    public static class TemporaryMaterialServiceExtensions
    {
        /// <summary>
        /// 添加一个临时素材。
        /// </summary>
        /// <param name="temporaryMaterialService">临时素材服务。</param>
        /// <param name="bytes">素材内容。</param>
        /// <returns>添加临时素材的结果模型。</returns>
        public static AddTemporaryMaterialResultModel AddTemporary(this ITemporaryMaterialService temporaryMaterialService, byte[] bytes)
        {
            var type = MaterialHelper.GetMaterialType(bytes);
            return temporaryMaterialService.NotNull("temporaryMaterialService").AddTemporary(bytes, type);
        }

        /// <summary>
        /// 添加一个临时缩略图素材。
        /// </summary>
        /// <param name="temporaryMaterialService">临时素材服务。</param>
        /// <param name="bytes">缩略图素材内容。</param>
        /// <returns>添加临时素材的结果模型。</returns>
        public static AddTemporaryMaterialResultModel AddTemporaryThumbnails(this ITemporaryMaterialService temporaryMaterialService, byte[] bytes)
        {
            return temporaryMaterialService.NotNull("temporaryMaterialService").AddTemporary(bytes, MaterialType.Thumb);
        }
    }

    /// <summary>
    /// 临时素材服务的实现。
    /// </summary>
    public sealed class TemporaryMaterialService : MaterialServiceBase, ITemporaryMaterialService
    {
        private readonly AccountModel _accountModel;

        public TemporaryMaterialService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #region Implementation of ITemporaryMaterialService

        /// <summary>
        /// 添加一个临时素材。
        /// </summary>
        /// <param name="bytes">素材内容。</param>
        /// <param name="type">素材类型。</param>
        /// <returns>添加临时素材的结果模型。</returns>
        public AddTemporaryMaterialResultModel AddTemporary(byte[] bytes, MaterialType type)
        {
            return Upload<AddTemporaryMaterialResultModel>(string.Format("https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", _accountModel.GetAccessToken(), type), bytes, "file");
        }

        /// <summary>
        /// 获取一个临时素材。
        /// </summary>
        /// <param name="mediaId">素材Id。</param>
        /// <returns>临时素材模型。</returns>
        public TemporaryMaterialModel GetTemporary(string mediaId)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", _accountModel.GetAccessToken(), mediaId);
            WebHeaderCollection responseHeaders;
            var data = HttpHelper.Get(url, out responseHeaders);
            ResultHelper.TryThrowError(data);
            return TemporaryMaterialModel.CreateByHeades(responseHeaders, data);
        }

        #endregion Implementation of ITemporaryMaterialService
    }

    #region Help Class

    /// <summary>
    /// 添加临时素材结果模型。
    /// </summary>
    public class AddTemporaryMaterialResultModel
    {
        /// <summary>
        /// 素材类型
        /// </summary>
        public MaterialType Type { get; set; }

        /// <summary>
        /// 素材Id。
        /// </summary>
        [JsonProperty("media_id")]
        public string MediaId { get; set; }

        [JsonProperty("thumb_media_id")]
        internal string ThumbnailsMediaId
        {
            get { return MediaId; }
            set { MediaId = value; }
        }

        /// <summary>
        /// 创建时间戳。
        /// </summary>
        [JsonProperty("created_at")]
        public ulong CreateTimestamp { get; set; }

        private DateTime? _createTime;

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                if (!_createTime.HasValue)
                    _createTime = DateTimeHelper.GetTimeByTimeStamp(CreateTimestamp);
                return _createTime.Value;
            }
        }
    }

    /// <summary>
    /// 临时素材模型。
    /// </summary>
    public class TemporaryMaterialModel
    {
        /// <summary>
        /// 内容类型。
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        public DateTime AddTime { get; set; }

        /// <summary>
        /// 内容长度。
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// 内容字节组。
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 根据响应头和数据字节组创建一个素材模型。
        /// </summary>
        /// <param name="header">响应头。</param>
        /// <param name="datas">数据字节组。</param>
        /// <returns>素材模型。</returns>
        public static TemporaryMaterialModel CreateByHeades(WebHeaderCollection header, byte[] datas)
        {
            return new TemporaryMaterialModel
            {
                ContentType = header["Content-Type"],
                AddTime = DateTime.Parse(header["Date"]),
                ContentLength = long.Parse(header["Content-Length"]),
                FileName = Regex.Match(header["Content-disposition"], "\"(.+)\"").Groups[1].Value,
                Data = datas
            };
        }
    }

    #endregion Help Class
}