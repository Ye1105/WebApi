using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Accounts
{
    public class RolePermission
    {
        [Key]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        [JsonProperty("rId")]
        public Guid RId { get; set; }

        /// <summary>
        /// 模块Id
        /// </summary>
        [JsonProperty("mId")]
        public Guid MId { get; set; }
    }
}