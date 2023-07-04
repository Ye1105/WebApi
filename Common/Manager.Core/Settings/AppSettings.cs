using Newtonsoft.Json;

namespace Manager.Core.Settings
{
    /// <summary>
    /// 对应appsettings中的App节点的配置信息
    /// </summary>
    public class AppSettings
    {
        public bool MainSite { set; get; }

        public RedisCache RedisChatCache { get; set; }

        public RedisCache RedisCache { set; get; }

        public RedisCache SignalrRedisCache { set; get; }

        public JwtConfig JwtBearer { set; get; }

        public TencentSmsConfig TencentSms { set; get; }

        /// <summary>
        /// Tencent Cos Video Config
        /// </summary>
        public TencentCosConfig TencentCos { set; get; }

        /// <summary>
        /// Tencent Cos Image Config
        /// </summary>
        public TencentCosTwoConfig TencentCosTwo { set; get; }

        public SmsConfig Sms { get; set; }

        public MailConfig Mail { get; set; }

        public string CrosAddress { get; set; }

        public string DocumentAddress { get; set; }

        public string ClientAddress { get; set; }

        public string DocumentDisk { get; set; }

        public string ConnectionString { get; set; }

        public sbyte ServerStatus { get; set; }
    }

    public class TencentCosTwoConfig
    {
        [JsonProperty("SecretId")]
        public string SecretId { get; set; }

        [JsonProperty("SecretKey")]
        public string SecretKey { get; set; }

        [JsonProperty("Bucket")]
        public string Bucket { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }

        [JsonProperty("BucketURL")]
        public string BucketURL { get; set; }
    }

    public class TencentCosConfig
    {
        [JsonProperty("SecretId")]
        public string SecretId { get; set; }

        [JsonProperty("SecretKey")]
        public string SecretKey { get; set; }

        [JsonProperty("Bucket")]
        public string Bucket { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }

        [JsonProperty("BucketURL")]
        public string BucketURL { get; set; }
    }

    public class MailConfig
    {
        /// <summary>
        /// POP3/SMPT 授权码
        /// </summary>
        public string AuthorizatioCode { get; set; }

        /// <summary>
        /// 发送邮件的账号主体
        /// </summary>
        public string MailAccount { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }
    }

    public class SmsConfig
    {
        /// <summary>
        /// 每个用户每日最大发送短信数
        /// </summary>
        public int DayLimit { get; set; }
    }

    public class RedisCache
    {
        public string RedisConnection { set; get; }
        public int DatabaseId { set; get; }
    }

    public class TencentSmsConfig
    {
        [JsonProperty("SecretId")]
        public string SecretId { get; set; }

        [JsonProperty("SecretKey")]
        public string SecretKey { get; set; }

        [JsonProperty("SmsSdkAppId")]
        public string SmsSdkAppId { get; set; }

        [JsonProperty("SignName")]
        public string SignName { get; set; }

        [JsonProperty("TemplateId")]
        public string TemplateId { get; set; }
    }

    public class JwtConfig
    {
        /// <summary>
        /// AccessToken密钥
        /// </summary>
        [JsonProperty("SecurityKey")]
        public string SecurityKey { get; set; }

        /// <summary>
        /// RefreshToken密钥
        /// </summary>
        [JsonProperty("RefreshSecurityKey")]
        public string RefreshSecurityKey { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        [JsonProperty("Issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// 受众人
        /// </summary>
        [JsonProperty("Audience")]
        public string Audience { get; set; }

        [JsonProperty("AccessExpiration")]
        public int AccessExpiration { get; set; }

        [JsonProperty("RefreshExpiration")]
        public int RefreshExpiration { get; set; }
    }
}