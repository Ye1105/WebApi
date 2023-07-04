using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Users;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/userfocuses")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class UserFocusesController : ControllerBase
    {
        private readonly IUserFocusService userFocusService;
        private readonly IAccountInfoService accountInfoService;

        public UserFocusesController(IUserFocusService userFocusService, IAccountInfoService accountInfoService)
        {
            this.userFocusService = userFocusService;
            this.accountInfoService = accountInfoService;
        }

        /// <summary>
        /// 用户关系：单个
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpGet("{buId}/{uId}")]
        public async Task<IActionResult> GetUserFocus(Guid buId, Guid uId)
        {
            var res = await userFocusService.GetUserFocusBy(x => x.BuId == buId && x.UId == uId);
            return res != null ? Ok(ApiResult.Success(res)) : Ok(ApiResult.Fail("关注关系不存在"));
        }

        /// <summary>
        /// 用户关系：分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserFocusList([FromQuery] GetUserFocusRequest req)
        {
            sbyte? relation = req.Relation == null ? null : (sbyte)req.Relation.Value;

            var result = await userFocusService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.UId, req.BuId, req.Grp, relation, req.Channel, req.StartTime, req.EndTime);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //如果当前登录网站的用户id不为空为Guid.Empty,
                    if (req.WId != null && req.WId != Guid.Empty)
                    {
                        if (req.WId != item.BuId)
                        {
                            var userFous = await userFocusService.GetUserFocusBy(x => x.UId == req.WId && x.BuId == item.BuId);
                            item.SelfRelation = userFous != null ? userFous.Relation : (sbyte)FocusRelationEnum.UnFocus;
                        }
                        else
                        {
                            item.SelfRelation = (sbyte)FocusRelationEnum.Self;
                        }
                    }

                    if (req.IsUInfo != null && req.IsUInfo.Value)
                    {
                        item.UInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.UId);
                    }

                    if (req.IsBuInfo != null && req.IsBuInfo.Value)
                    {
                        item.BuInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.UId);
                    }
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(ApiResult.Success("获取关注关系列表成功", JsonData));
            }
            return Ok(ApiResult.Fail("暂无数据"));
        }

        /// <summary>
        /// 用户关系：新增
        /// </summary>
        /// <param name="buId"></param>
        /// <param name="uId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [HttpPost("{buId}/{uId}/{channel}")]
        public async Task<IActionResult> AddUserFocus(Guid buId, Guid uId, FocusChannelEnum channel)
        {
            var userFocus = new UserFocus()
            {
                Id = Guid.NewGuid(),
                BuId = buId,
                UId = uId,
                RemarkName = "",
                Grp = "[]",
                Channel = (sbyte)channel,
                Created = DateTime.Now,
                Relation = (sbyte)FocusRelationEnum.Focus
            };

            var res = await userFocusService.AddUserFocus(userFocus);
            return res ? Ok(ApiResult.Success("添加关注关系成功")) : Ok(ApiResult.Fail("添加关注关系失败"));
        }

        /// <summary>
        /// 用户关系：编辑备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <param name="remarkName"></param>
        /// <returns></returns>
        [HttpPatch("{id}/names")]
        public async Task<IActionResult> EditUserFocusRemarkName(Guid id, [FromQuery] Guid uId, [FromQuery] string remarkName)
        {
            /*
             * 1.数据是否存在
             * 2.判断备注名是否存在
             * 3.编辑操作
             */

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.Id == id);
            if (userFocus != null)
            {
                //2.判断备注名是否存在
                var count = await userFocusService.GetUserFocusCountBy(x => x.UId == uId && x.RemarkName == remarkName);
                if (count > 0)
                {
                    return Ok(ApiResult.Fail("备注名已存在"));
                }
                //3.编辑操作
                userFocus.RemarkName = remarkName;
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(ApiResult.Success("修改备注名成功")) : Ok(ApiResult.Fail("修改备注名失败"));
            }
            else
            {
                return Ok(ApiResult.Fail("关注关系不存在"));
            }
        }

        /// <summary>
        /// 用户关系：编辑Relation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        [HttpPatch("{id}/relations/{relation}")]
        public async Task<IActionResult> EditUserFocusRelation(Guid id, RelationEnum relation)
        {
            /*
             * 参数校验 RelationEnum
             * 1.数据是否存在
             * 2.编辑操作
             */

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.Id == id);
            if (userFocus != null)
            {
                //2.编辑操作
                userFocus.Relation = (sbyte)relation;
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(ApiResult.Success("修改关注关系成功")) : Ok(ApiResult.Fail("修改关注关系失败"));
            }
            else
            {
                return Ok(ApiResult.Fail("关注关系不存在"));
            }
        }

        /// <summary>
        /// 用户关系：新增分组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpPatch("{id}/grpnews")]
        public async Task<IActionResult> AddUserFocusGroup(Guid id, [FromQuery] string grp)
        {
            if (string.IsNullOrWhiteSpace(grp))
            {
                return Ok(ApiResult.Fail("分组为空"));
            }

            var userFocus = await userFocusService.GetUserFocusBy(x => x.Id == id, true);
            if (userFocus != null)
            {
                dynamic groupArray = userFocus.Grp.DesObj();
                //2.判定新增的分组是否已经存在
                foreach (var item in groupArray)
                {
                    if (item == grp)
                        return Ok(ApiResult.Fail("分组已经存在"));
                }
                //3.序列化
                groupArray.Add(grp);
                userFocus.Grp = JsonHelper.SerJArray(groupArray);
                //4.新增用户分组
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(ApiResult.Success()) : Ok(ApiResult.Fail("新增用户分组失败"));
            }
            else
            {
                return Ok(ApiResult.Fail("用户分组不存在"));
            }
        }

        /// <summary>
        /// 用户关系：编辑分组
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <param name="grp">grp:分组 </param>
        /// <returns></returns>
        [HttpPatch("{id}/grpedits")]
        public async Task<IActionResult> EditUserFocusGroup([FromBody] EditUserFocusGroupRequest req)
        {
            /*
             * 通过 EditUserFocusGroupRequest 中的定义，校验 Grp 是否是 JSON 数组
             * 1.数据是否存在与数据库
             * 2.编辑操作
             */

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.Id == req.Id, true);
            if (userFocus != null)
            {
                //2.编辑操作
                userFocus.Grp = req.Grp ==
                    null || req.Grp.Length == 0 ? "[]"
                    :
                    req.Grp.SerObj();
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(ApiResult.Success()) : Ok(ApiResult.Fail("编辑用户关系分组失败"));
            }
            else
            {
                return Ok(ApiResult.Fail("用户关系分组不存在"));
            }
        }

        /// <summary>
        ///用户关系: 删除单个
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserFocus(Guid id)
        {
            var userFocus = await userFocusService.GetUserFocusBy(x => x.Id == id);
            if (userFocus != null)
            {
                var res = await userFocusService.DelUserFocus(userFocus);
                return res ? Ok(ApiResult.Success("")) : Ok(ApiResult.Fail("删除用户关系失败"));
            }
            else
            {
                return Ok(ApiResult.Fail("用户关系不存在"));
            }
        }

        /// <summary>
        /// 用户关系:批量删除
        /// </summary>
        /// <param name="req"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<IActionResult> BatchDeleteUserFocus([FromBody] BatchDeleteUserFocusRequest req)
        {
            var res = await userFocusService.BatchDeleteUserFocus(x => req.Ids.Contains(x.Id) && x.UId == req.UId) > 0;
            return res ? Ok(ApiResult.Success("")) : Ok(ApiResult.Fail("删除用户关系失败或无需删除"));
        }
    }
}