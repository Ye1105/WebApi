namespace Manager.Core.Tencent
{
    public class TencentSendSmsConfig : TencentConfig
    {
        public string SmsSdkAppId { get; set; }

        /// <summary>
        /// 短信签名
        /// </summary>
        public string SignName { get; set; }

        /// <summary>
        /// 短信模板Id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 发送的手机
        /// </summary>
        public string[] PhoneNumberSet { get; set; }

        /// <summary>
        /// {0} 验证码 {1} 过期时间
        /// </summary>
        public string[] TemplateParamSet { get; set; }
    }
}