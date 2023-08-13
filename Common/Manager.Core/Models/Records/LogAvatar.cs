﻿using Manager.Core.Models.Accounts;
using Manager.Core.Settings;
using Manager.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Logs
{
    public class LogAvatar
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [JsonProperty("uId")]
        public Guid UId { get; set; }
        /// <summary>
        /// blurhash
        /// </summary>
        [JsonProperty("blurhash")]
        public string? Blurhash { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }
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

        [JsonIgnore]
        public AccountInfo? AccountInfo { get; set; }


        /// <summary>
        /// 头像地址
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
                    return $"{Configurations.AppSettings["TencentCosTwo"].DesObj<TencentCosTwoConfig>().BucketURL}{Url}";
                }
                return _FullUrl;
            }
            set
            {
                _FullUrl = value;
            }
        }

    }
}
