using Manager.Core.Enums;
using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class AddBlogRequest
    {

        /// <summary>
        /// 种类： 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告
        /// </summary>
        [JsonProperty("sort")]
        public BlogSort Sort { get; set; }

        /// <summary>
        /// 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
        /// </summary>
        [JsonProperty("type")]
        public BlogType Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }


        /// <summary>
        /// 图片列表
        /// </summary>
        [JsonProperty("images")]
        public IList<BlogImageRequest>? Images { get; set; }

        /// <summary>
        /// 视频[json 对象]
        /// </summary>
        [JsonProperty("video")]
        public BlogVideoRequest? Video { get; set; }
    }

    public class BlogImageRequest
    {
        /// <summary>
        /// 地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// blurhash
        /// </summary>
        [JsonProperty("blurhash")]
        public string Blurhash { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class BlogVideoRequest
    {
        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 频道
        /// </summary>
        [JsonProperty("channel")]
        public IList<string> Channel { get; set; }

        /// <summary>
        /// 集合
        /// </summary>
        [JsonProperty("collection")]
        public IList<string> Collection { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [JsonProperty("type")]
        public IList<string> Type { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// 封面地址
        /// </summary>
        [JsonProperty("cUrl")]
        public string CUrl { get; set; }

        /// <summary>
        /// 封面宽度
        /// </summary>
        [JsonProperty("cWidth")]
        public int CWidth { get; set; }

        /// <summary>
        /// 封面高度
        /// </summary>
        [JsonProperty("cHeight")]
        public int CHeight { get; set; }

        /// <summary>
        /// 封面hash
        /// </summary>
        [JsonProperty("cHashblur")]
        public string CHashblur { get; set; }
    }
}