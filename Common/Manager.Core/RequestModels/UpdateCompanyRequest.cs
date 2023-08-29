using Newtonsoft.Json;

namespace Manager.Core.RequestModels
{
    public class UpdateCompanyRequest
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonProperty("company")]
        public string Company { get; set; }

        /// <summary>
        /// 部门/职位
        /// </summary>
        [JsonProperty("department")]
        public string Department { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        [JsonProperty("industry")]
        public string Industry { get; set; }


        /// <summary>
        /// 开始年份
        /// </summary>
        [JsonProperty("start")]
        public string Start { get; set; }


        /// <summary>
        /// 结束年份
        /// </summary>
        [JsonProperty("end")]
        public string End { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }


        /// <summary>
        /// 仅显示行业
        /// </summary>
        [JsonProperty("isShowIndustryOnly")]
        public bool IsShowIndustryOnly { get; set; }
    }
}
