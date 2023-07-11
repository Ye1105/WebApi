using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

/*https://docs.microsoft.com/zh-cn/dotnet/csharp/properties*/

namespace Manager.Core.Models.Accounts
{
    /// <summary>
    /// 账号表
    /// </summary>
    public class Account
    {
        [Key]
        [JsonIgnore]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [JsonProperty("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [JsonProperty("mail")]
        public string? Mail { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        [JsonIgnore]
        public string? Password { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        /// <summary>
        /// 0 启用  1 禁用  2 审核中  3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;
    }
}