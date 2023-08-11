using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Server.IServices;
using Manager.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("v1/api")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class LoginsController : ApiController
    {
        private readonly IAccountService accountService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IAuthenticateService authenticateService;
        private readonly IJwtService jWTService;
        private readonly ITencentService tencentService;
        private readonly IOptions<AppSettings> appSettings;

        public LoginsController(IAccountService accountService, IAccountInfoService accountInfoService, IAuthenticateService authenticateService, IJwtService jWTService, ITencentService tencentService, IOptions<AppSettings> appSettings)
        {
            this.accountService = accountService;
            this.accountInfoService = accountInfoService;
            this.authenticateService = authenticateService;
            this.jWTService = jWTService;
            this.tencentService = tencentService;
            this.appSettings = appSettings;
        }

        /// <summary>
        /// 用户登录【手机号+密码】
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost("logins")]
        public async Task<IActionResult> LoginPwd([FromForm] string phone, [FromForm] string pwd)
        {
            /*
             * 1.jsonschema 校验
             * 2.账号是否存在
             * 3.获取账号表 账号信息表
             * 4.账号Token认证
             */

            //1.jsonschema 校验

            var req = new
            {
                phone,
                pwd
            };

            var jsonSchema = await JsonSchemas.GetSchema("login-pwd");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //2.账号是否存在
            var account = await accountService.GetAccountBy(phone, pwd, isTrack: false);
            if (account == null)
            {
                return Ok(Fail("账号或密码不正确"));
            }
            else
            {
                /* 账号状态 */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"账号状态:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */

                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("账号认证失败"));
                }

                /* RefreshToken 存入 Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, isCache: true);

                return Ok(Success("账号登录成功", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }

        /// <summary>
        /// 用户登录【手机号+Sms】
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        [HttpPost("logins/sms")]
        public async Task<IActionResult> LoginSms([FromForm] string phone, [FromForm] string sms)
        {
            /*
             * 1.校验参数 ： phone sms
             * 2.限定时间间隔内是否存在对应的sms
             * 3.账号是否存在
             * 4.账号Token认证
             */

            //1.1校验参数 phone 是否为手机号
            if (!Regex.IsMatch(phone, RegexHelper.PhonePattern))
            {
                return Ok(Fail("不是合法的手机号"));
            }

            //1.2校验参数 phone 是否为验证码
            if (appSettings.Value.ServerStatus == (sbyte)ServerType.F0RMAL)
            {
                if (!Regex.IsMatch(sms, RegexHelper.SmsPattern))
                {
                    return Ok(Fail("验证码格式不正确"));
                }

                var dt = DateTime.Now;

                //2.限定时间间隔5分钟内是否存在对应的sms
                var res = tencentService.GetTencentSms(phone, sms, dt.AddMinutes(-5), dt);
                if (!res)
                {
                    return Ok(Fail("验证码过期"));
                }
            }

            //账号是否存在
            var account = await accountService.GetAccountBy(x => x.Phone == phone, false);
            if (account == null)
            {
                return Ok(Fail("手机号不存在"));
            }
            else
            {
                /* 账号状态 */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"账号状态:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("账号认证失败"));
                }

                /* RefreshToken 存入 Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, true);

                return Ok(Success("账号登陆成功", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }


        /// <summary>
        /// 用户登录【邮箱+密码】
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost("logins/mail")]
        public async Task<IActionResult> LoginMail([FromForm] string mail, [FromForm] string pwd)
        {
            /*
             * 1.校验参数 ： phone sms
             * 2.限定时间间隔内是否存在对应的sms
             * 3.账号是否存在
             * 4.账号Token认证
             */

            //1.1校验参数 phone 是否邮箱
            if (!Regex.IsMatch(mail, RegexHelper.MailPattern))
            {
                return Ok(Fail("不是合法的邮箱"));
            }

            //账号是否存在
            var account = await accountService.GetAccountBy(x => x.Mail == mail && x.Password == Md5Helper.MD5(pwd), false);
            if (account == null)
            {
                return Ok(Fail("邮箱不存在"));
            }
            else
            {
                /* 账号状态 */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"账号状态:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("账号认证失败"));
                }

                /* RefreshToken 存入 Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, true);

                return Ok(Success("账号登陆成功", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }
    }
}