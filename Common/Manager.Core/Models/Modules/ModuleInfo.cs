using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.Models.Modules
{
    public class ModuleInfo
    {
        /// <summary>
        /// 菜单栏Id
        /// </summary>
        [Key]
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 路由
        /// </summary>
        [JsonProperty("route")]
        public string? Route { get; set; }

        /// <summary>
        /// 控制器
        /// </summary>
        [JsonProperty("controller")]
        public string Controller { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        [JsonProperty("action")]
        public string Action { get; set; }

        /// <summary>
        /// 父菜单栏Id
        /// </summary>
        [JsonProperty("pId")]
        public Guid? PId { get; set; }

        /// <summary>
        /// 是否是菜单栏
        /// </summary>
        [JsonProperty("isMenu")]
        public sbyte? IsMenu { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [JsonProperty("type")]
        public sbyte? Type { get; set; }

        /// <summary>
        /// 排序Id
        /// </summary>
        [JsonProperty("order")]
        public int? Order { get; set; } = 0;

        /// <summary>
        /// 0 启用  1 禁用  2 审核中  3 审核失败
        /// </summary>
        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.Enable;
    }
}