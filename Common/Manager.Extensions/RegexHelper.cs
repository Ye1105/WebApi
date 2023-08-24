namespace Manager.Extensions
{
    public static class RegexHelper
    {
        /// <summary>
        /// 手机号正则
        /// </summary>
        public static readonly string PhonePattern = @"^1[3456789]\d{9}$";

        /// <summary>
        /// 邮箱正则
        /// </summary>
        public static readonly string MailPattern = @"(@)(.+)$";

        /// <summary>
        ///  匹配字符【话题】 #**#
        /// </summary>
        public static readonly string TopicPattern = @"\#[\u4e00-\u9fa5a-zA-Z0-9\p{P}*]{1,30}\#";

        /// <summary>
        /// 验证码正则
        /// </summary>
        public static readonly string SmsPattern = @"^[0-9]{6}";

        /// <summary>
        /// 昵称正则
        /// </summary>
        public static readonly string NickNamePattern = @"^[\u4E00-\u9FA5]{2,5}$";

        /// <summary>
        /// 用户简介
        /// </summary>
        public static readonly string DescriptionPattern = @"^.{1,50}$";

        /// <summary>
        /// 数值类型
        /// </summary>
        public static readonly string NumberPattern = @"^[0-9]\d*$";

        /// <summary>
        /// 校验参数
        /// </summary>
        /// <param name="context">需要校验的内容</param>
        /// <param name="pattern">校验规则</param>
        /// <param name="regex">校验的具体方法</param>
        /// <returns></returns>
        public static bool Validator(this string context, string pattern, Func<string, string, bool> regex)
        {
            return regex(context, pattern);
        }

        /// <summary>
        ///  返回符合正则的集合
        /// </summary>
        /// <param name="context">文本</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="regex">委托</param>
        /// <param name="res">返回结果</param>
        /// <returns></returns>
        public static bool RegexList(this string context, string pattern, Func<string, string, List<string>?> regex, out List<string> res)
        {
            res = regex(context, pattern);
            return res != null && res.Any();
        }
    }
}