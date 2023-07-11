﻿using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        /// 修改账号
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("{uId}")]
        public async Task<IActionResult> EditAccount([FromBody] EditAccountRequest req)
        {
            /*
             * 0.参数校验
             * 1.账号是否存在
             * 2.判断 mail 是否已经存在
             * 3.修改邮箱数据
             */

            //0.参数校验
            //if (!mail.Validator(
            //   RegexHelper.MailPattern,
            //   (mail, pattern) => Regex.IsMatch(mail, pattern))
            //)
            //{
            //    return Ok(ApiResult.Fail("邮箱格式不正确"));
            //}

            //1.账号是否存在
            var account = await accountService.GetAccountBy(x => x.UId == req.UId);
            if (account == null)
            {
                return Ok(Fail("账号不存在"));
            }

            //2.判断除当前用户之外的 name mail phone 是否已经有存在
            var accountName = await accountService.GetAccountBy(x => x.Name == req.Name && x.UId != req.UId, false);
            if (accountName != null)
            {
                return Ok(Fail("账号名称已存在"));
            }

            var accountMail = await accountService.GetAccountBy(x => x.Mail.ToLower() == req.Mail.ToLower() && x.UId != req.UId, false);
            if (accountMail != null)
            {
                return Ok(Fail("邮箱已存在"));
            }

            var accountPhone = await accountService.GetAccountBy(x => x.Phone == req.Phone && x.UId != req.UId, false);
            if (accountPhone != null)
            {
                return Ok(Fail("手机号已存在"));
            }

            //3.修改账号信息
            account.Name = req.Name;
            account.Mail = req.Mail;
            account.Phone = req.Phone;

            return await accountService.ModifyAccount(account) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }
    }
}