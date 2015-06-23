namespace Rabbit.WeiXin.MP.Api.Material
{
    /// <summary>
    /// 素材类型。
    /// </summary>
    public enum MaterialType
    {
        /// <summary>
        /// 图文（jpg，1mb）。
        /// </summary>
        Image = 0,

        /// <summary>
        /// 语音（mp3/amr，2mb长度在60秒以内）。
        /// </summary>
        Voice = 1,

        /// <summary>
        /// 视频（mp4，10mb）。
        /// </summary>
        Video = 2,

        /// <summary>
        /// 封面（jpg，64kb）。
        /// </summary>
        Thumb = 3
    }
}