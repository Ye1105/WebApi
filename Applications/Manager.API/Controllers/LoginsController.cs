using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.JwtAuthorizePolicy.IServices;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("v1/api")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    public class LoginsController : ControllerBase
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
        [HttpGet("logins")]
        public async Task<IActionResult> GetLoginPwd([FromForm] string phone, [FromForm] string pwd)
        {
            /*
             * 1.У����� phone �Ƿ�Ϊ�ֻ���
             * 2.�˺��Ƿ����
             * 3.��ȡ�˺ű� �˺���Ϣ��
             * 4.�˺�Token��֤
             */

            //1.У����� phone �Ƿ�Ϊ�ֻ���
            var validator = phone.Validator(
                RegexHelper.PhonePattern,
                (phone, pattern) => Regex.IsMatch(phone, pattern)
            );
            if (!validator)
            {
                return Ok(ApiResult.Fail("���ǺϷ����ֻ���"));
            }

            //2.�˺��Ƿ����
            var account = await accountService.GetAccountBy(phone, pwd, false);
            if (account == null)
            {
                return Ok(ApiResult.Fail("�˺Ż����벻��ȷ"));
            }
            else
            {
                /* �˺�״̬ */
                if (account.Status != (sbyte)Status.Enable)
                {
                    return Ok(ApiResult.Fail($"�˺�״̬:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */

                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(ApiResult.Fail("�˺���֤ʧ��"));
                }

                /* RefreshToken ���� Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(ApiResult.Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(account.UId, true);

                return Ok(ApiResult.Success("�˺���֤�ɹ�", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }

        /// <summary>
        /// �û���¼���ֻ���+Sms��
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="sms"></param>
        /// <returns></returns>
        [HttpGet("logins/sms")]
        public async Task<IActionResult> GetLoginSms([FromForm] string phone, [FromForm] string sms)
        {
            /*
             * 1.У����� �� phone sms
             * 2.�޶�ʱ�������Ƿ���ڶ�Ӧ��sms
             * 3.�˺��Ƿ����
             * 4.�˺�Token��֤
             */

            //1.1У����� phone �Ƿ�Ϊ�ֻ���
            var validator = phone.Validator(
                RegexHelper.PhonePattern,
                (phone, pattern) => Regex.IsMatch(phone, pattern)
            );
            if (!validator)
            {
                return Ok(ApiResult.Fail("���ǺϷ����ֻ���"));
            }

            //1.2У����� phone �Ƿ�Ϊ��֤��
            if (appSettings.Value.ServerStatus == (sbyte)ServerStatusEnum.Formal)
            {
                if (!sms.Validator
                    (RegexHelper.SmsPattern,
                    (sms, pattern) => Regex.IsMatch(sms, pattern))
                )
                {
                    return Ok(ApiResult.Fail("��֤���ʽ����ȷ"));
                }

                var dt = DateTime.Now;

                //2.�޶�ʱ����5�������Ƿ���ڶ�Ӧ��sms
                var res = tencentService.GetTencentSms(phone, sms, dt.AddMinutes(-5), dt);
                if (!res)
                {
                    return Ok(ApiResult.Fail("��֤�����"));
                }
            }

            //�˺��Ƿ����
            var account = await accountService.GetAccountBy(x => x.Phone == phone, false);
            if (account == null)
            {
                return Ok(ApiResult.Fail("�ֻ��Ų�����"));
            }
            else
            {
                /* �˺�״̬ */
                if (account.Status != (sbyte)Status.Enable)
                {
                    return Ok(ApiResult.Fail($"�˺�״̬:{EnumDescriptionAttribute.GetEnumDescription((Status)account.Status)}"));
                }

                /* AccessToken RefreshToken */
                if (!authenticateService.IsAuthenticated(new AuthenticateRequest() { Id = account.Id, UId = account.UId }, out string AccessToken, out string RefreshToken))
                {
                    return Ok(ApiResult.Fail("�˺���֤ʧ��"));
                }

                /* RefreshToken ���� Redis */
                var tokenRes = jWTService.AddRefreshToken(account.UId, RefreshToken);
                if (!tokenRes.Item1)
                {
                    return Ok(ApiResult.Fail(tokenRes.Item2));
                }

                /* AccountInfo */
                var accountInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(account.UId, true);

                return Ok(ApiResult.Success("�˺���֤�ɹ�", new { account, accountInfo, AccessToken, RefreshToken }));
            }
        }
    }
}