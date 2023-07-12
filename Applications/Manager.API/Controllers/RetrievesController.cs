using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("v1/api/retrieves")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    public class RetrievesController : ApiController
    {
        private readonly ITencentService tencentService;
        private readonly IAccountService accountService;
        private readonly IMailService mailService;

        public RetrievesController(ITencentService tencentService, IAccountService accountService, IMailService mailService)
        {
            this.tencentService = tencentService;
            this.accountService = accountService;
            this.mailService = mailService;
        }

        /// <summary>
        /// 手机号重置密码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPatch("phones")]
        public async Task<IActionResult> SetLoginRetrievePhone([FromBody] RetrievePhoneRequest req)
        {
            /*
             * 1.手机号校验
             * 2.验证码是否过期
             * 3.更新账号密码
             */

            //1.手机号校验
            var validator = req.Phone.Validator(
                RegexHelper.PhonePattern,
                (phone, pattern) => Regex.IsMatch(phone, pattern)
            );
            if (!validator)
            {
                return Ok(Fail("不是合法的手机号"));
            }

            //2.验证码是否过期
            var dt = DateTime.Now;
            var res = tencentService.GetTencentSms(req.Phone, req.Sms, dt.AddMinutes(-5), dt);
            if (!res)
            {
                return Ok(Fail("验证码不存在或已过期"));
            }

            //3.更新账号密码
            if (await accountService.ModifyAccountPassword(x => x.Phone == req.Phone, req.Password))
            {
                return Ok(Success("密码重置成功"));
            }

            return Ok(Fail("密码重置失败"));
        }

        /// <summary>
        /// 邮箱重置密码
        /// </summary>
        /// <param name="req"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        [HttpPatch("mails")]
        public async Task<IActionResult> SetLoginRetrieveMail([FromBody] RetrieveMailRequest req)
        {
            /*
             * 1.邮箱校验
             * 2.验证码是否过期
             * 3.更新账号密码
             */

            //1.邮箱校验
            var validator = req.Mail.Validator(
                RegexHelper.MailPattern,
                (mail, pattern) => Regex.IsMatch(mail, pattern)
            );
            if (!validator)
            {
                return Ok(Fail("不是合法的邮箱"));
            }

            //2.验证码是否过期
            var dt = DateTime.Now;
            var res = await mailService.GetLogMailSmsBy(x => x.Mail.ToLower() == req.Mail.ToLower() && x.Sms == req.Sms && x.Created >= dt.AddMinutes(-5) && x.Created < dt, false);
            if (res == null)
            {
                return Ok(Fail("验证码过期"));
            }

            //3.更新账号密码
            if (await accountService.ModifyAccountPassword(x => x.Mail == req.Mail, req.Password))
            {
                return Ok(Success("密码重置成功"));
            }

            return Ok(Fail("密码重置失败"));
        }
    }
}