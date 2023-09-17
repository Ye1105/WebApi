using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Org.BouncyCastle.Ocsp;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/accounts")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class AccountsController : ApiController
    {
        private readonly IAccountService accountService;

        public AccountsController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        /// <summary>
        /// 修改账号邮箱
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        [HttpPatch("mail")]
        public async Task<IActionResult> UpdateMail([FromForm] string mail)
        {
            /*
             * 0.参数校验
             * 1.账号是否存在
             * 2.判断 mail 是否已经存在
             * 3.修改邮箱数据
             */

            //0.参数校验
            if (!Regex.IsMatch(mail, RegexHelper.MailPattern))
            {
                return Ok(Fail("邮箱格式不正确", "参数错误"));
            }


            //1.账号是否存在
            var account = await accountService.FirstOrDefaultAsync(x => x.UId == UId);
            if (account == null)
            {
                return Ok(Fail("账号不存在"));
            }

            var accountMail = await accountService.FirstOrDefaultAsync(x => x.Mail.ToLower() == mail.ToLower() && x.UId != UId, false);
            if (accountMail != null)
            {
                return Ok(Fail("邮箱已存在"));
            }

            //3.修改账号信息
            //account.Name = req.Name;
            account.Mail = mail;

            return await accountService.UpdateAsync(account) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }


        /// <summary>
        /// 修改账号手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPatch("phone")]
        public async Task<IActionResult> UpdatePhone([FromForm] string phone)
        {
            /*
             * 0.参数校验
             * 1.账号是否存在
             * 2.判断 mail 是否已经存在
             * 3.修改邮箱数据
             */

            //0.参数校验
            if (!Regex.IsMatch(phone, RegexHelper.PhonePattern))
            {
                return Ok(Fail("手机格式不正确", "参数错误"));
            }

            //1.账号是否存在
            var account = await accountService.FirstOrDefaultAsync(x => x.UId == UId);
            if (account == null)
            {
                return Ok(Fail("账号不存在"));
            }

            var accountPhone = await accountService.FirstOrDefaultAsync(x => x.Phone == phone && x.UId != UId, false);
            if (accountPhone != null)
            {
                return Ok(Fail("手机号已存在"));
            }

            //3.修改账号信息
            account.Phone = phone;

            return await accountService.UpdateAsync(account) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }
    }
}