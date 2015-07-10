using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.Utility;
using System;
using System.ComponentModel.DataAnnotations;

namespace Rabbit.WeiXin.MP.Api.GroupMessage
{
    /// <summary>
    /// 一个抽象的群组消息服务。
    /// </summary>
    public interface IGroupMessageService
    {
        /// <summary>
        /// 发送群组消息。
        /// </summary>
        /// <param name="filter">群组的筛选信息。</param>
        /// <param name="message">群组消息。</param>
        void SendByGroup(GroupFilter filter, GroupMessage message);
    }

    /// <summary>
    /// 群组消息服务实现。
    /// </summary>
    public sealed class GroupMessageService : IGroupMessageService
    {
        #region Field

        private readonly AccountModel _accountModel;

        #endregion Field

        #region Constructor

        /// <summary>
        /// 初始化一个新的群发服务实例。
        /// </summary>
        /// <param name="accountModel">账号模型。</param>
        public GroupMessageService(AccountModel accountModel)
        {
            _accountModel = accountModel;
        }

        #endregion Constructor

        #region Implementation of IGroupMessageService

        /// <summary>
        /// 发送群组消息。
        /// </summary>
        /// <param name="filter">群组的筛选信息。</param>
        /// <param name="message">群组消息。</param>
        public void SendByGroup(GroupFilter filter, GroupMessage message)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token=" + _accountModel.GetAccessToken();

            object filterObj;

            if (filter.GroupId.HasValue)
            {
                filterObj = new { is_to_all = false, group_id = filter.GroupId.Value };
            }
            else
            {
                filterObj = new { is_to_all = true };
            }

            string msgtype;

            switch (message.Type)
            {
                case GroupMessageType.News:
                    msgtype = "mpnews";
                    break;

                case GroupMessageType.Card:
                    msgtype = "wxcard";
                    break;

                case GroupMessageType.Image:
                    msgtype = "image";
                    break;

                case GroupMessageType.Music:
                    msgtype = "music";
                    break;

                case GroupMessageType.Text:
                    msgtype = "text";
                    break;

                case GroupMessageType.Video:
                    msgtype = "mpvideo";
                    var videoMessage = (GroupMessageVideo)message;
                    var uploadVideoUrl = "https://file.api.weixin.qq.com/cgi-bin/media/uploadvideo?access_token=" + _accountModel.GetAccessToken();
                    var result = WeiXinHttpHelper.PostString(uploadVideoUrl, new
                    {
                        media_id = videoMessage.MediaId,
                        title = videoMessage.Title,
                        description = videoMessage.Description
                    });
                    videoMessage.MediaId = JObject.Parse(result)["media_id"].Value<string>();
                    break;

                case GroupMessageType.Voice:
                    msgtype = "voice";
                    break;

                default:
                    throw new NotSupportedException("不支持的消息类型：" + message.Type);
            }

            var postJson = JsonConvert.SerializeObject(new
            {
                filter = filterObj,
                msgtype,
            });
            var postObj = JObject.Parse(postJson);
            postObj[msgtype] = JObject.Parse(JsonConvert.SerializeObject(message));

            WeiXinHttpHelper.Post(url, postObj.ToString());
        }

        #endregion Implementation of IGroupMessageService
    }

    #region Help Class

    /// <summary>
    /// 群组筛选器。
    /// </summary>
    public sealed class GroupFilter
    {
        /// <summary>
        /// 群发的分组Id，如果为null则向所有组发送。
        /// </summary>
        public ulong? GroupId { get; set; }
    }

    /// <summary>
    /// 分组消息类型。
    /// </summary>
    public enum GroupMessageType
    {
        /// <summary>
        /// 图文。
        /// </summary>
        News = 0,

        /// <summary>
        /// 文本。
        /// </summary>
        Text = 1,

        /// <summary>
        /// 语音。
        /// </summary>
        Voice = 2,

        /// <summary>
        /// 音乐。
        /// </summary>
        Music = 3,

        /// <summary>
        /// 图片。
        /// </summary>
        Image = 4,

        /// <summary>
        /// 视频。
        /// </summary>
        Video = 5,

        /// <summary>
        /// 卡券。
        /// </summary>
        Card = 6
    }

    /// <summary>
    /// 群组消息基类。
    /// </summary>
    public abstract class GroupMessage
    {
        /// <summary>
        /// 群组消息类型。
        /// </summary>
        [JsonIgnore]
        public abstract GroupMessageType Type { get; }
    }

    /// <summary>
    /// 群组图文消息。
    /// </summary>
    public sealed class GroupMessageNews : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组图文消息。
        /// </summary>
        public GroupMessageNews()
        {
        }

        /// <summary>
        /// 初始化一个新的群组图文消息。
        /// </summary>
        /// <param name="mediaId">媒体Id。</param>
        public GroupMessageNews(string mediaId)
        {
            MediaId = mediaId;
        }

        /// <summary>
        /// 媒体Id。
        /// </summary>
        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.News; }
        }

        #endregion Overrides of GroupMessage
    }

    /// <summary>
    /// 群组文本消息。
    /// </summary>
    public sealed class GroupMessageText : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组文本消息。
        /// </summary>
        public GroupMessageText()
        {
        }

        /// <summary>
        /// 初始化一个新的群组文本消息。
        /// </summary>
        /// <param name="content">文本内容。</param>
        public GroupMessageText(string content)
        {
            Content = content;
        }

        /// <summary>
        /// 文本内容。
        /// </summary>
        [Required, JsonProperty("content")]
        public string Content { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.Text; }
        }

        #endregion Overrides of GroupMessage
    }

    /// <summary>
    /// 群组语音消息。
    /// </summary>
    public sealed class GroupMessageVoice : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组语音消息。
        /// </summary>
        public GroupMessageVoice()
        {
        }

        /// <summary>
        /// 初始化一个新的群组语音消息。
        /// </summary>
        /// <param name="mediaId">媒体Id。</param>
        public GroupMessageVoice(string mediaId)
        {
            MediaId = mediaId;
        }

        /// <summary>
        /// 媒体Id。
        /// </summary>
        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.Voice; }
        }

        #endregion Overrides of GroupMessage
    }

    /// <summary>
    /// 群组图文消息。
    /// </summary>
    public sealed class GroupMessageImage : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组图文消息。
        /// </summary>
        public GroupMessageImage()
        {
        }

        /// <summary>
        /// 初始化一个新的群组图文消息。
        /// </summary>
        /// <param name="mediaId">媒体Id。</param>
        public GroupMessageImage(string mediaId)
        {
            MediaId = mediaId;
        }

        /// <summary>
        /// 媒体Id。
        /// </summary>
        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.Image; }
        }

        #endregion Overrides of GroupMessage
    }

    /// <summary>
    /// 群组视频消息。
    /// </summary>
    public sealed class GroupMessageVideo : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组视频消息。
        /// </summary>
        public GroupMessageVideo()
        {
        }

        /// <summary>
        /// 初始化一个新的群组视频消息。
        /// </summary>
        /// <param name="mediaId">媒体Id。</param>
        /// <param name="title">标题。</param>
        /// <param name="description">描述。</param>
        public GroupMessageVideo(string mediaId, string title, string description)
        {
            MediaId = mediaId;
            Title = title;
            Description = description;
        }

        /// <summary>
        /// 标题。
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// 媒体Id。
        /// </summary>
        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.Video; }
        }

        #endregion Overrides of GroupMessage
    }

    /// <summary>
    /// 群组卡券消息。
    /// </summary>
    public sealed class GroupMessageCard : GroupMessage
    {
        /// <summary>
        /// 初始化一个新的群组卡券消息。
        /// </summary>
        public GroupMessageCard()
        {
        }

        /// <summary>
        /// 初始化一个群组卡券消息。
        /// </summary>
        /// <param name="cardId">卡券Id。</param>
        public GroupMessageCard(string cardId)
        {
            CardId = cardId;
        }

        /// <summary>
        /// 卡券Id。
        /// </summary>
        [Required, JsonProperty("card_id")]
        public string CardId { get; set; }

        #region Overrides of GroupMessage

        /// <summary>
        /// 群组消息类型。
        /// </summary>
        public override GroupMessageType Type
        {
            get { return GroupMessageType.Video; }
        }

        #endregion Overrides of GroupMessage
    }

    #endregion Help Class
}