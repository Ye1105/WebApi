using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Schema;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/accountinfos")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class AccountInfosController : ControllerBase
    {
        private readonly IAccountInfoService accountInfoService;

        public AccountInfosController(IAccountInfoService accountInfoService)
        {
            this.accountInfoService = accountInfoService;
        }

        /// <summary>
        /// 修改账号信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("{uId}")]
        public async Task<IActionResult> EditAccountInfo([FromBody] EditAccountInfoRequest req)
        {
            /*
             * 0.Json Schema 参数校验
             * 1.判断表用户信息是否存在
             * 2.修改数据
             */

            //0.Json Schema 参数校验
            var validator = JsonSchemaHelper.Validator<EditAccountInfoRequest>(req, out IList<ValidationError> errorMessages);
            if (!validator)
            {
                return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            }

            //1.判断表用户信息是否存在
            var accountInfo = await accountInfoService.GetAccountInfoBy(x => x.UId == req.UId, true);
            if (accountInfo == null)
            {
                return Ok(ApiResult.Fail("账号信息表不存在"));
            }

            // 2.修改数据
            accountInfo.NickName = req.NickName;
            accountInfo.Sex = (sbyte)req.Sex;
            accountInfo.Location = req.Location.SerObj();
            accountInfo.Hometown = req.Hometown.SerObj();
            accountInfo.Company = req.Company.SerObj();
            accountInfo.School = req.School.SerObj();
            accountInfo.Emotion = (sbyte)req.Emotion;
            accountInfo.Describe = req.Describe;
            accountInfo.Tag = req.Tag.SerObj();
            accountInfo.Birthday = req.Birthday;

            return await accountInfoService.ModifyAccountInfo(accountInfo) ? Ok(ApiResult.Success("修改成功")) : Ok(ApiResult.Fail("修改失败"));
        }
    }
}