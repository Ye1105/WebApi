using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Users;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/relations")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class RelationsController : ApiController
    {
        private readonly IUserFocusService userFocusService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IAccountService accountService;

        public RelationsController(IUserFocusService userFocusService, IAccountInfoService accountInfoService, IAccountService accountService)
        {
            this.userFocusService = userFocusService;
            this.accountInfoService = accountInfoService;
            this.accountService = accountService;
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="buId"></param>
        ///// <param name="uId"></param>
        ///// <returns></returns>
        //[HttpGet("{buId}/{uId}")]
        //public async Task<IActionResult> GetUserFocus(Guid buId)
        //{
        //    var res = await userFocusService.GetUserFocusBy(x => x.BuId == buId && x.UId == uId);
        //    return res != null ? Ok(Success(res)) : Ok(Fail("关注关系不存在"));
        //}

        #region Paged

        /// <summary>
        /// 关注列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("focus")]
        public async Task<IActionResult> FocusPaged([FromQuery] GetRelationRequest req)
        {
            /*
             * 1.jsonschema 参数校验
             * 2.分页数据
             */

            //1.jsonschema 参数校验
            var jsonSchema = await JsonSchemas.GetSchema("focus-paged");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //2.分页数据
            var result = await userFocusService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, RelationType.FOCUS, UId, req.Grp, req.Relation, req.Channel, req.StartTime, req.EndTime);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    item.AccountInfo = await accountInfoService.FirstOrDefaultAsync(item.BuId, isCache: false);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(Success("获取关注列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 粉丝列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("fans")]
        public async Task<IActionResult> FansPaged([FromQuery] GetRelationRequest req)
        {
            /*
             * 1.jsonschema 参数校验
             * 2.分页数据
             */

            //1.jsonschema 参数校验
            var jsonSchema = await JsonSchemas.GetSchema("fans-paged");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            var result = await userFocusService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, RelationType.FAN, UId, req.Grp, req.Relation, req.Channel, req.StartTime, req.EndTime);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    item.AccountInfo = await accountInfoService.FirstOrDefaultAsync(item.UId, isCache: false);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(Success("获取粉丝列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        #endregion Paged

        #region Update

        /// <summary>
        /// 编辑备注
        /// </summary>
        /// <param name="uId">被关注人的用户Id</param>
        /// <param name="remarkName">被关注人的备注名</param>
        /// <returns></returns>
        [HttpPatch("names")]
        public async Task<IActionResult> UpdateRemarkName([FromQuery] Guid uId, [FromQuery] string remarkName)
        {
            /*
             * 0.参数校验
             * 1.数据是否存在
             * 2.判断备注名是否存在
             * 3.编辑操作
             */

            //0.参数校验（备注名）
            if (string.IsNullOrWhiteSpace(remarkName))
            {
                return Ok(Fail("备注名不能为空"));
            }

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.UId == UId && x.BuId == uId);
            if (userFocus != null)
            {
                //2.判断备注名是否存在
                var count = await userFocusService.GetUserFocusCountBy(x => x.UId == UId && x.BuId != uId && x.RemarkName == remarkName);
                if (count > 0)
                {
                    return Ok(Fail("备注名已存在"));
                }
                //3.编辑操作
                userFocus.RemarkName = remarkName;
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(Success("修改备注名成功")) : Ok(Fail("修改备注名失败"));
            }
            else
            {
                return Ok(Fail("关注关系不存在"));
            }
        }

        /// <summary>
        /// 编辑 Relation
        /// </summary>
        /// <param name="uId">被关注人的uId</param>
        /// <param name="relation">关注关系</param>
        /// <returns></returns>
        [HttpPatch("{uId}/relation/{relation}")]
        public async Task<IActionResult> UpdateRelation(Guid uId, Relation relation)
        {
            /*
             * 参数校验 RelationEnum
             * 1.数据是否存在
             * 2.编辑操作
             */

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.UId == UId && x.BuId == uId);
            if (userFocus != null)
            {
                //2.编辑操作
                userFocus.Relation = (sbyte)relation;
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(Success("修改关注关系成功")) : Ok(Fail("修改关注关系失败"));
            }
            else
            {
                return Ok(Fail("关注关系不存在"));
            }
        }

        /// <summary>
        /// 编辑分组
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPatch("{uId}/grps/update")]
        public async Task<IActionResult> UpdateGroup(Guid uId, [FromBody] UpdateGroupRequest req)
        {
            /*
             * 通过 EditUserFocusGroupRequest 中的定义，校验 Grp 是否是 JSON 数组
             * 1.数据是否存在与数据库
             * 2.编辑操作
             */

            //0.jsonschema 参数校验
            var jsonSchema = await JsonSchemas.GetSchema("focus-group-update");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            //1.数据是否存在
            var userFocus = await userFocusService.GetUserFocusBy(x => x.UId == UId && x.BuId == uId, true);
            if (userFocus != null)
            {
                //2.编辑操作
                userFocus.Grp = req.Grps ==
                    null || req.Grps.Length == 0 ? "[]"
                    :
                    req.Grps.SerObj();
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(Success()) : Ok(Fail("编辑用户关系分组失败"));
            }
            else
            {
                return Ok(Fail("用户关系分组不存在"));
            }
        }

        #endregion Update

        #region Add

        /// <summary>
        /// 关注某人
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        [HttpPost("{uId}/{channel}")]
        public async Task<IActionResult> Add(Guid uId, FocusChannel channel)
        {
            var account = await accountService.GetAccountBy(x => x.UId == uId);
            if (account == null)
            {
                return Ok(Fail("用户不存在"));
            }

            var exsit = await userFocusService.GetUserFocusCountBy(x => x.UId == UId && x.BuId == uId);
            if (exsit > 0)
            {
                return Ok(Fail("已关注"));
            }

            var userFocus = new UserFocus()
            {
                Id = Guid.NewGuid(),
                BuId = uId,
                UId = UId,
                RemarkName = "",
                Grp = "[]",
                Channel = (sbyte)channel,
                Created = DateTime.Now,
                Relation = (sbyte)FocusRelation.FOCUS
            };

            var res = await userFocusService.AddUserFocus(userFocus);
            return res ? Ok(Success("关注成功")) : Ok(Fail("关注失败"));
        }

        /// <summary>
        /// 新增分组
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="grp"></param>
        /// <returns></returns>
        [HttpPatch("{uId}/grps/add")]
        public async Task<IActionResult> AddGroup(Guid uId, [FromQuery] string grp)
        {
            //0.参数校验（分组）
            if (string.IsNullOrWhiteSpace(grp))
            {
                return Ok(Fail("分组不能为空"));
            }

            var userFocus = await userFocusService.GetUserFocusBy(x => x.UId == UId && x.BuId == uId, true);
            if (userFocus != null)
            {
                dynamic groupArray = userFocus.Grp.DesObj();
                //2.判定新增的分组是否已经存在
                foreach (var item in groupArray)
                {
                    if (item == grp)
                        return Ok(Fail("分组已存在"));
                }
                //3.序列化
                groupArray.Add(grp);
                userFocus.Grp = JsonHelper.SerJArray(groupArray);
                //4.新增用户分组
                var res = await userFocusService.ModifyUserFocus(userFocus);
                return res ? Ok(Success()) : Ok(Fail("新增用户分组失败"));
            }
            else
            {
                return Ok(Fail("用户分组不存在"));
            }
        }

        #endregion Add

        #region Del

        /// <summary>
        ///删除单个
        /// </summary>
        /// <param name="uId">被关注人用户id</param>
        /// <returns></returns>
        [HttpDelete("{uId}")]
        public async Task<IActionResult> DeleteFocus(Guid uId)
        {
            var userFocus = await userFocusService.GetUserFocusBy(x => x.UId == UId && x.BuId == uId);
            if (userFocus != null)
            {
                var res = await userFocusService.DelUserFocus(userFocus);
                return res ? Ok(Success("")) : Ok(Fail("删除用户关系失败"));
            }
            else
            {
                return Ok(Fail("用户关系不存在"));
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<IActionResult> BatchDeleteFocus([FromBody] BatchDeleteUserFocusRequest req)
        {
            var res = await userFocusService.BatchDeleteUserFocus(x => req.UIds.Contains(x.BuId.Value) && x.UId == UId) > 0;
            return res ? Ok(Success("")) : Ok(Fail("删除用户关系失败或无需删除"));
        }

        #endregion Del
    }
}