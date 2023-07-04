using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Accounts
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class RoleInfo
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        [Key]
        [JsonProperty("rId")]
        public Guid RId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}