using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rabbit.WeiXin.MP.Api.Utility;
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

    public abstract class GroupMessage
    {
        [JsonIgnore]
        public abstract GroupMessageType Type { get; }
    }

    public sealed class GroupMessageNews : GroupMessage
    {
        public GroupMessageNews()
        {
        }

        public GroupMessageNews(string mediaId)
        {
            MediaId = mediaId;
        }

        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.News; }
        }

        #endregion Overrides of GroupMessage
    }

    public sealed class GroupMessageText : GroupMessage
    {
        public GroupMessageText()
        {
        }

        public GroupMessageText(string content)
        {
            Content = content;
        }

        [Required, JsonProperty("content")]
        public string Content { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.Text; }
        }

        #endregion Overrides of GroupMessage
    }

    public sealed class GroupMessageVoice : GroupMessage
    {
        public GroupMessageVoice()
        {
        }

        public GroupMessageVoice(string mediaId)
        {
            MediaId = mediaId;
        }

        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.Voice; }
        }

        #endregion Overrides of GroupMessage
    }

    public sealed class GroupMessageImage : GroupMessage
    {
        public GroupMessageImage()
        {
        }

        public GroupMessageImage(string mediaId)
        {
            MediaId = mediaId;
        }

        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.Image; }
        }

        #endregion Overrides of GroupMessage
    }

    public sealed class GroupMessageVideo : GroupMessage
    {
        public GroupMessageVideo()
        {
        }

        public GroupMessageVideo(string mediaId, string title, string description)
        {
            MediaId = mediaId;
            Title = title;
            Description = description;
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [Required, JsonProperty("media_id")]
        public string MediaId { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.Video; }
        }

        #endregion Overrides of GroupMessage
    }

    public sealed class GroupMessageCard : GroupMessage
    {
        public GroupMessageCard()
        {
        }

        public GroupMessageCard(string cardId)
        {
            CardId = cardId;
        }

        [Required, JsonProperty("card_id")]
        public string CardId { get; set; }

        #region Overrides of GroupMessage

        public override GroupMessageType Type
        {
            get { return GroupMessageType.Video; }
        }

        #endregion Overrides of GroupMessage
    }

    #endregion Help Class
}