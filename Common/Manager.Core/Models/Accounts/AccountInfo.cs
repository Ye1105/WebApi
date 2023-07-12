using Manager.Core.Models.Logs;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Accounts
{
    /// <summary>
    /// 账号信息表
    /// </summary>
    public class AccountInfo
    {
        [Key]
        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonProperty("nickName")]
        public string? NickName { get; set; }

        /// <summary>
        /// 性别： 0 男性  1中性  2 女性
        /// </summary>
        [JsonProperty("sex")]
        public sbyte? Sex { get; set; }

        /// <summary>
        /// vip等级
        /// </summary>
        [JsonProperty("vip")]
        public sbyte? Vip { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        [JsonProperty("location")]
        public string? Location { get; set; }

        /// <summary>
        /// 家乡
        /// </summary>
        [JsonProperty("hometown")]
        public string? Hometown { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [JsonProperty("company")]
        public string? Company { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        [JsonProperty("school")]
        public string? School { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [JsonProperty("birthday")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 情感状态
        /// </summary>
        [JsonProperty("emotion")]
        public sbyte? Emotion { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("describe")]
        public string? Describe { get; set; } = string.Empty;

        /// <summary>
        /// 标签
        /// </summary>
        [JsonProperty("tag")]
        public string? Tag { get; set; }

        /// <summary>
        /// 官方证书
        /// </summary>
        [JsonProperty("officialCert")]
        public string? OfficialCert { get; set; }

        /// <summary>
        /// 当前头像Id
        /// </summary>
        [JsonIgnore]
        [JsonProperty("avatarId")]
        public Guid? AvatarId { get; set; }

        /// <summary>
        /// 当前封面Id
        /// </summary>
        [JsonIgnore]
        [JsonProperty("coverId")]
        public Guid? CoverId { get; set; }

        /// <summary>
        /// 当前头像
        /// </summary>
        [NotMapped]
        [JsonProperty("avatar")]
        public LogAvatar? Avatar { get; set; }

        /// <summary>
        /// 当前封面
        /// </summary>
        [NotMapped]
        [JsonProperty("cover")]
        public LogCover? Cover { get; set; }
    }
}