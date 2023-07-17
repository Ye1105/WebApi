using Manager.Core.Settings;
using Manager.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Blogs
{
    public class BlogVideo
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid? UId { get; set; }

        /// <summary>
        /// blog表Id
        /// </summary>
        [JsonProperty("bId")]
        public Guid? BId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 频道
        /// </summary>
        [JsonProperty("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 集合
        /// </summary>
        [JsonProperty("collection")]
        public string Collection { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        [JsonIgnore]
        [JsonProperty("url")]
        public string? Url { get; set; }

        private string _FullUrl { get; set; }
        [NotMapped]
        [JsonProperty("fullUrl")]
        public string FullUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_FullUrl))
                {
                    return $"{Configurations.AppSettings["TencentCos"].DesObj<TencentCosTwoConfig>().BucketURL}{Url}";
                }
                return _FullUrl;
            }
            set
            {
                _FullUrl = value;
            }
        }


        /// <summary>
        /// 时长
        /// </summary>
        [JsonProperty("duration")]
        public int? Duration { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// 0 启用  1 禁用 2 审核中 3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;

        /// <summary>
        /// 视频封面
        /// </summary>
        [JsonProperty("cover")]
        public string? Cover { get; set; }


        [NotMapped]
        [JsonProperty("fullCUrl")]
        public string FullCUrl
        { get { return $"{Configurations.AppSettings["TencentCosTwo"].DesObj<TencentCosTwoConfig>().BucketURL}{Url}"; } }


        /// <summary>
        /// 点赞数
        /// </summary>
        [NotMapped]
        [JsonProperty("like")]
        public long? Like { get; set; }

        /// <summary>
        /// 当前网站的登录用户是否点赞
        /// </summary>
        [NotMapped]
        [JsonProperty("isLike")]
        public bool? IsLike { get; set; } = false;
    }
}