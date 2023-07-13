using Manager.Core.Models.Accounts;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Users
{
    public class UserFocus
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
        /// 被关注的用户 => BefocusUserId
        /// </summary>
        [JsonProperty("buId")]
        public Guid? BuId { get; set; }

        /// <summary>
        /// 备注名
        /// </summary>
        [JsonProperty("remarkName")]
        public string? RemarkName { get; set; }

        /// <summary>
        /// 分组
        /// </summary>
        [JsonProperty("grp")]
        public string Grp { get; set; }

        /// <summary>
        /// -1 表示未关注的、广告、热搜的blog  0 已关注但不是特别关注，自己的blog  1 特别关注的
        /// </summary>
        [JsonProperty("relation")]
        public sbyte? Relation { get; set; }

        /// <summary>
        /// 关注的渠道 0 Mobile 1 HTML5
        /// </summary>
        [JsonProperty("channel")]
        public sbyte? Channel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [NotMapped]
        [JsonProperty("accountInfo")]
        public AccountInfo? AccountInfo { get; set; }
    }
}