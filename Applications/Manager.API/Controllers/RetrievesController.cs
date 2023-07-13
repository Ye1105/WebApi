using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

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
        public async Task<IActionResult> RetrievePhone([FromBody] RetrievePhoneRequest req)
        {
            /*
             * 1.jsonschema 校验
             * 2.验证码是否过期
             * 3.更新账号密码
             */

            //1. jsonschema

            var jsonSchema = await JsonSchemas.GetSchema("retrieve-sms");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //2.验证码是否过期
            var dt = DateTime.Now;
            var res = tencentService.GetTencentSms(req.Phone, req.Sms, dt.AddMinutes(-5), dt);
            if (!res)
            {
                return Ok(Fail("验证码不存在或已过期"));
            }

            //3.更新账号密码
            if (await accountService.ModifyAccountPassword(x => x.Phone == req.Phone, req.Pwd))
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
        public async Task<IActionResult> RetrieveMail([FromBody] RetrieveMailRequest req)
        {
            /*
             * 1.jsonschema 校验
             * 2.验证码是否过期
             * 3.更新账号密码
             */

            //1. jsonschema
            var jsonSchema = await JsonSchemas.GetSchema("retrieve-mail");
            var schema = JSchema.Parse(jsonSchema);
            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //2.验证码是否过期
            var dt = DateTime.Now;
            var res = await mailService.GetLogMailSmsBy(x => x.Mail.ToLower() == req.Mail.ToLower() && x.Sms == req.Sms && x.Created >= dt.AddMinutes(-5) && x.Created < dt, false);
            if (res == null)
            {
                return Ok(Fail("验证码过期"));
            }

            //3.更新账号密码
            if (await accountService.ModifyAccountPassword(x => x.Mail == req.Mail, req.Pwd))
            {
                return Ok(Success("密码重置成功"));
            }

            return Ok(Fail("密码重置失败"));
        }
    }
}