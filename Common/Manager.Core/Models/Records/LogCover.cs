using Manager.Core.Models.Accounts;
using Manager.Core.Settings;
using Manager.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manager.Core.Models.Logs
{
    public class LogCover
    {
        [Key]
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uId")]
        public Guid UId { get; set; }

        [JsonProperty("blurhash")]
        public string? Blurhash { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [JsonIgnore]
        public AccountInfo? AccountInfo { get; set; }

        [JsonProperty("status")]
        public sbyte? Status { get; set; } = (sbyte)Enums.Status.ENABLE;

        [JsonIgnore]
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
                    return $"{Configurations.AppSettings["TencentCosTwo"].DesObj<TencentCosTwoConfig>().BucketURL}/{UId}/cover/{Url}";
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