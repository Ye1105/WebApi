using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Settings;
using Manager.Core.Tencent;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("v1/api/sms")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    public class SmsController : ApiController
    {
        private readonly ITencentService tencentService;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IMailService mailService;
        private readonly IAccountService accountService;

        public SmsController(ITencentService tencentService, IOptions<AppSettings> appSettings, IMailService mailService, IAccountService accountService)
        {
            this.tencentService = tencentService;
            this.appSettings = appSettings;
            this.mailService = mailService;
            this.accountService = accountService;
        }

        /// <summary>
        /// tencent sms
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>

        [HttpPost("tencents/{phone}")]
        public async Task<IActionResult> SendTencentSms(string phone)
        {
            /*
             * 1.校验参数 phone 是否为手机号
             * 2.校验1分钟内是否已经存在有已发送短信
             * 3.获取每个用户每天发送短信数的上限数配置
             * 4.校验当前手机号当日已发短信数是否已超上限
             * 5.调用sms第三方接口
             */

            //1.校验参数 phone 是否为手机号
            if (!Regex.IsMatch(RegexHelper.PhonePattern, RegexHelper.PhonePattern))
            {
                return Ok(Fail("不是合法的手机号"));
            }

            //2.校验1分钟内是否已经存在有已发送短信
            var minuteLimitRes = tencentService.GetTencentSms(phone, DateTime.Now.AddMinutes(-1));
            if (minuteLimitRes)
            {
                return Ok(Fail($"1分钟内已经存在已发送短信"));
            }

            //3.获取每个用户每天发送短信数的上限数配置
            var dayLimit = appSettings.Value.Sms.DayLimit;

            //4.校验当前手机号当日已发短信数是否已超上限
            var dayLimitRes = tencentService.ExceedUpSmsDayLimitCount(phone, dayLimit);
            if (dayLimitRes)
            {
                return Ok(Fail($"当前手机号当日发短信数已超上限{dayLimit}条"));
            }

            //获取sms配置信息
            var tencentSmsCofig = appSettings.Value.TencentSms;
            var tencentSendSmsConfig = new TencentSendSmsConfig()
            {
                SecretId = tencentSmsCofig.SecretId,
                SecretKey = tencentSmsCofig.SecretKey,
                SignName = tencentSmsCofig.SignName,
                SmsSdkAppId = tencentSmsCofig.SmsSdkAppId,
                TemplateId = tencentSmsCofig.TemplateId,
                PhoneNumberSet = new string[] { phone },
                TemplateParamSet = new string[] { RandHelper.RndomNum(6), "5" }
            };

            //5.调用sms第三方接口
            var res = await tencentService.SendSMS(tencentSendSmsConfig);
            if (res.Item1)
            {
                return Ok(Success("验证码发送成功"));
            }
            else
            {
                return Ok(Fail(res.Item2, "验证码发送失败"));
            }
        }

        /// <summary>
        /// mail sms
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost("mails/{mail}/{type}")]
        public async Task<IActionResult> SendMailSms(string mail, Manager.Core.Enums.MailType type)
        {
            /*
             * 1.邮箱参数校验
             * 2.邮箱对应的账号是否存在
             * 3.是否重复发送
             * 4.发送邮件
             * 5.写入发送邮箱Sms记录
             */

            //1.邮箱参数校验
            if (!Regex.IsMatch(mail, RegexHelper.MailPattern))
            {
                return Ok(Fail("不是合法的邮箱"));
            }

            //2.邮箱对应的账号是否存在
            var res = await accountService.GetAccountBy(x => x.Mail == mail, false);
            if (res == null)
            {
                return Ok(Fail("账号不存在"));
            }

            //3.是否重复发送
            var mailExsit = await mailService.FirstOrDefaultAsync(x => x.Mail == mail && x.Created >= DateTime.Now.AddMinutes(-1));
            if (mailExsit != null)
            {
                return Ok(Fail("已发送"));
            }


            //发送邮件Sms配置信息
            var sms = RandHelper.RndomNum(6);
            var authorizationCode = appSettings.Value.Mail.AuthorizatioCode;
            var mailSender = appSettings.Value.Mail.MailAccount;
            var host = appSettings.Value.Mail.Host;
            var displayName = appSettings.Value.Mail.DisplayName;

            //3.发送邮件
            if (await mailService.SendMail(authorizationCode, host, displayName, mailSender, mail, sms, type))
            {
                return Ok(Success("验证码发送成功"));
            }
            else
            {
                return Ok(Fail("验证码发送失败"));
            }
        }
    }
}