using Manager.Core.Models.Logs;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IMailService
    {
        /// <summary>
        /// 发送邮箱
        /// </summary>
        /// <param name="authorizationCode">邮件发送方:POP3/SMPT 授权Code</param>
        /// <param name="host">邮件发送方:SMTP host主机</param>
        /// <param name="displayName">邮件发送方:displayName</param>
        /// <param name="mailSender">邮件发送方:发送邮箱验证码主账号</param>
        /// <param name="mailRecipient">邮件接收方:接收邮箱验证码主账号</param>
        /// <param name="sms">发送的内容</param>
        public Task<bool> SendMail(string authorizationCode, string host, string displayName, string mailSender, string mailRecipient, string sms, Manager.Core.Enums.MailType mailType);

        /// <summary>
        ///
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        public Task<LogMailSMS?> GetLogMailSmsBy(Expression<Func<LogMailSMS, bool>> expression, bool isTrack = true);


        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<LogMailSMS?> FirstOrDefaultAsync(Expression<Func<LogMailSMS, bool>> expression, bool isTrack = true);
    }
}