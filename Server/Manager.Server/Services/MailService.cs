using Manager.Core.Models.Logs;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Manager.Server.Services
{
    public class MailService : IMailService
    {
        private readonly IBase baseService;

        public MailService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<LogMailSMS?> GetLogMailSmsBy(Expression<Func<LogMailSMS, bool>> expression, bool isTrack = true)
        {
            var res = await baseService.QueryAsync(expression, 1, 1, 0, false, "created desc");
            return res.FirstOrDefault();
        }

        public async Task<bool> SendMail(string authorizationCode, string host, string displayName, string mailSender, string mailRecipient, string sms)
        {
            var client = new SmtpClient(host)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailSender, authorizationCode)
            };

            //邮件发送方
            MailAddress from = new(mailSender, displayName, Encoding.UTF8);
            //邮件接收方
            MailAddress to = new(mailRecipient);
            //指定消息内容
            MailMessage message = new(from, to)
            {
                //标题
                Subject = "验证码",

                SubjectEncoding = System.Text.Encoding.UTF8,

                Body = "您好，以下是重置密码操作所需的验证码："
            };

            message.Body += Environment.NewLine + Environment.NewLine + sms;

            message.Body += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "验证码5分钟内有效，提供他人可能导致账号被盗，请勿泄露，谨防被盗。";

            message.BodyEncoding = System.Text.Encoding.UTF8;

            await client.SendMailAsync(message);

            var logMailSMS = new LogMailSMS()
            {
                Id = Guid.NewGuid(),
                Mail = mailRecipient,
                Sms = sms,
                Created = DateTime.Now
            };
            return await baseService.AddAsync(logMailSMS) > 0;
        }
    }
}