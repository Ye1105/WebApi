using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class UpdateSchoolRequest
    {
        /// <summary>
        /// 学校
        /// </summary>
        [JsonProperty("school")]
        public string School { get; set; }


        /// <summary>
        /// 院系
        /// </summary>
        [JsonProperty("department")]
        public string Department { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 入学时间
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}
