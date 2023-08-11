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
        /// �û���¼���ֻ���+���롿
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost("logins")]
        public async Task<IActionResult> LoginPwd([FromForm] string phone, [FromForm] string pwd)
        {
            /*
             * 1.jsonschema У��
             * 2.�˺��Ƿ����
             * 3.��ȡ�˺ű� �˺���Ϣ��
             * 4.�˺�Token��֤
             */

            //1.jsonschema У��

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
                return Ok(Fail(errorMessages, "��������"));
            }

            //2.�˺��Ƿ����
            var account = await accountService.GetAccountBy(phone, pwd, isTrack: false);
            if (account == null)
            {
                return Ok(Fail("�˺Ż����벻��ȷ"));
            }
            else
            {
                /* �˺�״̬ */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"�˺�״̬:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */

                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("�˺���֤ʧ��"));
                }

                /* RefreshToken ���� Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, isCache: true);

                return Ok(Success("�˺ŵ�¼�ɹ�", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }

        /// <summary>
        /// �û���¼���ֻ���+Sms��
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        [HttpPost("logins/sms")]
        public async Task<IActionResult> LoginSms([FromForm] string phone, [FromForm] string sms)
        {
            /*
             * 1.У����� �� phone sms
             * 2.�޶�ʱ�������Ƿ���ڶ�Ӧ��sms
             * 3.�˺��Ƿ����
             * 4.�˺�Token��֤
             */

            //1.1У����� phone �Ƿ�Ϊ�ֻ���
            if (!Regex.IsMatch(phone, RegexHelper.PhonePattern))
            {
                return Ok(Fail("���ǺϷ����ֻ���"));
            }

            //1.2У����� phone �Ƿ�Ϊ��֤��
            if (appSettings.Value.ServerStatus == (sbyte)ServerType.F0RMAL)
            {
                if (!Regex.IsMatch(sms, RegexHelper.SmsPattern))
                {
                    return Ok(Fail("��֤���ʽ����ȷ"));
                }

                var dt = DateTime.Now;

                //2.�޶�ʱ����5�������Ƿ���ڶ�Ӧ��sms
                var res = tencentService.GetTencentSms(phone, sms, dt.AddMinutes(-5), dt);
                if (!res)
                {
                    return Ok(Fail("��֤�����"));
                }
            }

            //�˺��Ƿ����
            var account = await accountService.GetAccountBy(x => x.Phone == phone, false);
            if (account == null)
            {
                return Ok(Fail("�ֻ��Ų�����"));
            }
            else
            {
                /* �˺�״̬ */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"�˺�״̬:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("�˺���֤ʧ��"));
                }

                /* RefreshToken ���� Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, true);

                return Ok(Success("�˺ŵ�½�ɹ�", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }


        /// <summary>
        /// �û���¼������+���롿
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpPost("logins/mail")]
        public async Task<IActionResult> LoginMail([FromForm] string mail, [FromForm] string pwd)
        {
            /*
             * 1.У����� �� phone sms
             * 2.�޶�ʱ�������Ƿ���ڶ�Ӧ��sms
             * 3.�˺��Ƿ����
             * 4.�˺�Token��֤
             */

            //1.1У����� phone �Ƿ�����
            if (!Regex.IsMatch(mail, RegexHelper.MailPattern))
            {
                return Ok(Fail("���ǺϷ�������"));
            }

            //�˺��Ƿ����
            var account = await accountService.GetAccountBy(x => x.Mail == mail && x.Password == Md5Helper.MD5(pwd), false);
            if (account == null)
            {
                return Ok(Fail("���䲻����"));
            }
            else
            {
                /* �˺�״̬ */
                if (account.Status != (sbyte)Status.ENABLE)
                {
                    return Ok(Fail($"�˺�״̬:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(Fail("�˺���֤ʧ��"));
                }

                /* RefreshToken ���� Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.FirstOrDefaultAsync(account.UId, true);

                return Ok(Success("�˺ŵ�½�ɹ�", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }
    }
}