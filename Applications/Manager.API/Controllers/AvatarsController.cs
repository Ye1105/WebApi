using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Logs;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/avatars")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class AvatarsController : Controller
    {
        private readonly ILogAvatarService logAvatarService;

        public AvatarsController(ILogAvatarService logAvatarService)
        {
            this.logAvatarService = logAvatarService;
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="avatar">头像地址</param>
        /// <param name="blurhash">模糊哈希</param>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromForm] Guid uId, [FromForm] string avatar, [FromForm] string blurhash, [FromForm] int height, [FromForm] int width)
        {
            /*
             * 1.序列化json参数
             * 2.上传头像
             */

            var logAvatar = new LogAvatar()
            {
                Id = Guid.NewGuid(),
                UId = uId,
                Blurhash = blurhash,
                Url = avatar,
                Height = height,
                Width = width,
                Created = DateTime.Now,
                Status = (sbyte)Status.UnderReview
            };

            var res = await logAvatarService.AddLogAvatar(logAvatar);

            return res.Item1 ? Ok(ApiResult.Success("上传头像成功")) : Ok(ApiResult.Fail(res.Item2));
        }

        /// <summary>
        /// 用户头像：分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLogAvatarList([FromQuery] AvatarRequest req)
        {
            var result = await logAvatarService.GetPagedList(req.UId, req.PageIndex, req.PageSize, req.OffSet, req.OrderBy);

            if (result != null && result.Any())
            {
                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(ApiResult.Success(JsonData));
            }
            else
                return Ok(ApiResult.Fail("查询头像分页列表为空"));
        }
    }
}