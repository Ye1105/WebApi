using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/accountinfos")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class AccountInfosController : ApiController
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
        [HttpPut]
        public async Task<IActionResult> EditAccountInfo([FromBody] EditAccountInfoRequest req)
        {
            /*
             * 0.Json Schema 参数校验
             * 1.判断表用户信息是否存在
             * 2.修改数据
             */

            //0.1 Json Schema 参数校验
            var jsonSchema = await JsonSchemas.GetSchema("accountinfo-edit");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            //1.2 判断表用户昵称是否存在
            var exsit = await accountInfoService.FirstOrDefaultAsync(x => x.UId != UId && x.NickName == req.NickName, false);
            if (exsit != null)
            {
                return Ok(Fail("账号昵称已存在"));
            }

            // 2. 修改数据
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

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }
    }
}