﻿using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("v1/api/registers")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    public class RegistersController : ApiController
    {
        private readonly ITencentService tencentService;
        private readonly IAccountService accountService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IOptions<AppSettings> appSettings;

        public RegistersController(ITencentService tencentService, IAccountService accountService, IAccountInfoService accountInfoService, IOptions<AppSettings> appSettings)
        {
            this.tencentService = tencentService;
            this.accountService = accountService;
            this.accountInfoService = accountInfoService;
            this.appSettings = appSettings;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            /*
             * 1.通过手机号 和 Sms 查询是否存在腾讯 Sms 信息
             * 2.如果存在，与当前时间比较判定是否超时
             * 3.1判定 账号 或者 手机号 是否存在
             * 3.2判定 昵称是否存在
             * 4.创建账号信息【Account,AccountInfo】
             */

            var jsonSchema = await JsonSchemas.GetSchema("register");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //正式服环境
            if (appSettings.Value.ServerStatus == (sbyte)ServerType.F0RMAL)
            {
                //1.通过手机号 和 Sms 查询是否存在腾讯 Sms 信息
                var curSms = tencentService.GetTencentSms(req.Phone, req.Sms);
                if (curSms == null)
                {
                    return Ok(Fail("验证码不存在"));
                }

                //2.如果存在，与当前时间比较判定是否超时
                var timeSpan = DateHelper.SecondDiff(curSms.Created.Value, DateTime.Now);
                if (timeSpan > 300)  //5分钟内有效
                {
                    return Ok(Fail("验证码的存储时间和用户发送的注册时间间隔大于5分钟", "验证码过期"));
                }
            }

            //3.1 判定 账号  是否存在
            var curAccName = await accountService.GetAccountBy(x => x.Name == req.Name, false);
            if (curAccName != null)
            {
                return Ok(Fail("账号名称已存在"));
            }

            //3.1 判定 手机号  是否存在
            var curAccPhone = await accountService.GetAccountBy(x => x.Phone == req.Phone, false);
            if (curAccPhone != null)
            {
                return Ok(Fail("手机号已存在"));
            }

            //3.3 判定 昵称 是否存在
            var curAccInfo = await accountInfoService.FirstOrDefaultAsync(x => x.NickName == req.NickName, false);
            if (curAccInfo != null)
            {
                return Ok(Fail("账号昵称已存在"));
            }

            //4.创建账号信息【Account,AccountInfo】
            var res = await accountService.CreateAccount(req.Name, req.Pwd, req.Phone, req.NickName);

            return res ? Ok(Success("注册成功")) : Ok(Fail("注册失败"));
        }
    }
}