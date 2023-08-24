using Manager.Core.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Manager.Core.RequestModels
{
    public class UpdateAccountInfoRequest
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [JsonProperty("nickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [JsonProperty("sex")]
        public Sex Sex { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }

        /// <summary>
        /// 家乡
        /// </summary>
        [JsonProperty("hometown")]
        public Hometown Hometown { get; set; }

        /// <summary>
        ///公司集合
        /// </summary>
        [JsonProperty("company")]
        public IList<string> Company { get; set; }

        /// <summary>
        ///学校集合
        /// </summary>
        [JsonProperty("school")]
        public IList<string> School { get; set; }

        /// <summary>
        /// 情感状态
        /// </summary>
        [JsonProperty("emotion")]
        public EmotionEnum Emotion { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("describe")]
        public string Describe { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [JsonProperty("tag")]
        public IList<string> Tag { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }
    }

    /// <summary>
    /// 地址
    /// </summary>
    public class Location
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }
    }

    /// <summary>
    /// 家乡
    /// </summary>
    public class Hometown
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }
    }
}