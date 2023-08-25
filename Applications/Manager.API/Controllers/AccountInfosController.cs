using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text.RegularExpressions;

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
        /// 修改账号昵称
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [HttpPatch("nickname")]
        public async Task<IActionResult> UpdateNickName([FromForm] string nickName)
        {
            if (!Regex.IsMatch(nickName, RegexHelper.NickNamePattern))
            {
                return Ok(Fail("邮箱格式不正确", "参数错误"));
            }


            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            //1.2 判断表用户昵称是否存在
            var exsit = await accountInfoService.FirstOrDefaultAsync(x => x.UId != UId && x.NickName == nickName, false);
            if (exsit != null)
            {
                return Ok(Fail("账号昵称已存在"));
            }

            // 2. 修改数据
            accountInfo.NickName = nickName;

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }

        /// <summary>
        /// 修改简介
        /// </summary>
        /// <param name="describe"></param>
        /// <returns></returns>
        [HttpPatch("describe")]
        public async Task<IActionResult> UpdateDescribe([FromForm] string? describe)
        {

            if (describe != null)
            {
                if (!Regex.IsMatch(describe, RegexHelper.DescriptionPattern))
                {
                    return Ok(Fail("简介格式不正确", "参数错误"));
                }
            }

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Describe = describe;

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }

        /// <summary>
        /// 修改性别
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        [HttpPatch("sex")]
        public async Task<IActionResult> UpdateDescribe([FromForm] Sex sex)
        {


            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Sex = (sbyte)sex;

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }

        /// <summary>
        /// 修改生日
        /// </summary>
        /// <param name="birthday"></param>
        /// <returns></returns>
        [HttpPatch("birthday")]
        public async Task<IActionResult> UpdateBirthday([FromForm] DateTime? birthday)
        {

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Birthday = birthday;

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }


        /// <summary>
        /// 修改感情状态
        /// </summary>
        /// <param name="emotion"></param>
        /// <returns></returns>
        [HttpPatch("emotion")]
        public async Task<IActionResult> UpdateEmotion([FromForm] EmotionEnum emotion)
        {

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Emotion = (sbyte)emotion;

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }


        /// <summary>
        /// 省市
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpPatch("location")]
        public async Task<IActionResult> UpdateLocation([FromForm] UpdateProvinceCityRequest location)
        {

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Location = location.SerObj();

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }

        /// <summary>
        /// 修改故乡
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpPatch("hometown")]
        public async Task<IActionResult> UpdateHometown([FromForm] UpdateProvinceCityRequest location)
        {

            //1.1 判断表用户信息是否存在
            var accountInfo = await accountInfoService.FirstOrDefaultAsync(x => x.UId == UId, true);
            if (accountInfo == null)
            {
                return Ok(Fail("账号信息表不存在"));
            }

            // 2. 修改数据
            accountInfo.Hometown = location.SerObj();

            return await accountInfoService.UpdateAsync(accountInfo) ? Ok(Success("修改成功")) : Ok(Fail("修改失败"));
        }



        /// <summary>
        /// 修改账号信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAccountInfo([FromBody] UpdateAccountInfoRequest req)
        {
            /*
             * 0.Json Schema 参数校验
             * 1.判断表用户信息是否存在
             * 2.修改数据
             */

            //0.1 Json Schema 参数校验
            var jsonSchema = await JsonSchemas.GetSchema("accountinfo-update");

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