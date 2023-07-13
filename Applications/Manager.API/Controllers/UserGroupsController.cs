using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/usergroups")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class UserGroupsController : ApiController
    {
        private readonly IOptions<AppSettings> appSettings;
        private readonly IUserGroupService userGroupService;

        public UserGroupsController(IOptions<AppSettings> appSettings, IUserGroupService userGroupService)
        {
            this.appSettings = appSettings;
            this.userGroupService = userGroupService;
        }

        /// <summary>
        /// 用户分组信息
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet("{uId}")]
        public async Task<IActionResult> GetUserGroup(Guid uId)
        {
            var res = await userGroupService.GetUserGroupByUId(uId);
            return res != null ? Ok(Success(res)) : Ok(Fail("获取用户分组失败"));
        }

        /// <summary>
        /// 用户分组：新增grp中的成员
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Add([FromQuery] Guid uId, [FromQuery] string grp)
        {
            /* 1.获取当前用户的Grp【是否存在】
             * 2.判定新增的分组是否已经存在
             * 3.序列化
             * 4.新增博客用户分组
             */

            if (string.IsNullOrEmpty(grp))
            {
                return Ok(Fail("分组为空"));
            }
            // 1.获取当前用户的Grp【是否存在】
            var userGroup = await userGroupService.GetUserGroupBy(x => x.UId == uId, true);
            if (userGroup != null)
            {
                dynamic groupArray = userGroup.Grp.DesObj();
                //2.判定新增的分组是否已经存在
                foreach (var item in groupArray)
                {
                    if (item == grp)
                        return Ok(Fail("用户分组已经存在"));
                }
                //3.序列化
                groupArray.Add(grp);
                userGroup.Grp = JsonHelper.SerJArray(groupArray);
                //4.新增博客用户分组
                var res = await userGroupService.ModifyUserGroup(userGroup);
                return res ? Ok(Success()) : Ok(Fail("新增用户分组失败"));
            }
            else
            {
                return Ok(Fail("博客用户分组不存在"));
            }
        }

        /// <summary>
        /// 用户分组：删除grp中的某一项
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Del([FromQuery] Guid uId, [FromQuery] string grp)
        {
            /* 1.获取当前用户的Grp【是否存在】
             * 2.判定删除的分组是否已经存在
             * 3.序列化
             * 4.新增博客用户分组
             */

            if (string.IsNullOrEmpty(grp))
            {
                return Ok(Fail("分组为空"));
            }

            var curGrp = $"\"{grp}\"";
            // 1.获取当前用户的Grp【是否存在】
            var userGroup = await userGroupService.GetUserGroupBy(x => x.UId == uId, true);
            if (userGroup != null)
            {
                //2.转变为 ArrayList
                string[] gArray = userGroup.Grp[1..^1].Split(',');
                var delRes = false;
                for (int i = 0; i < gArray.Length; i++)
                {
                    var curItem = gArray[i];
                    if (curItem.Trim() == curGrp.Trim())
                    {
                        ArrayList al = new(gArray);
                        al.RemoveAt(i);
                        gArray = (string[])al.ToArray(typeof(string));
                        delRes = true;
                        break;
                    }
                }
                if (!delRes)
                {
                    return Ok(Fail("用户分组不存在"));
                }
                var Group = '[' + string.Join(',', gArray) + ']';
                userGroup.Grp = Group;
                //3.删除博客用户分组
                var res = await userGroupService.ModifyUserGroup(userGroup);
                return res ? Ok(Success()) : Ok(Fail("删除用户分组失败"));
            }
            else
            {
                return Ok(Fail("用户分组不存在"));
            }
        }
    }
}