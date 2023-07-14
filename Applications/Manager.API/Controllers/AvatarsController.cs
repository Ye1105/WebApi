using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.API.Utility.Schemas;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Logs;
using Manager.Core.Page;
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
    [Route("v1/api/avatars")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class AvatarsController : ApiController
    {
        private readonly ILogAvatarService logAvatarService;

        public AvatarsController(ILogAvatarService logAvatarService)
        {
            this.logAvatarService = logAvatarService;
        }

        /// <summary>
        /// 用户头像：分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AvatarPaged([FromQuery] QueryParameters req)
        {
            var result = await logAvatarService.GetPagedList(UId, req.PageIndex, req.PageSize, req.OffSet, req.OrderBy);

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

                return Ok(Success(JsonData));
            }
            else
                return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="avatar">头像地址</param>
        /// <param name="blurhash">模糊哈希</param>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromForm] string avatar, [FromForm] string blurhash, [FromForm] int height, [FromForm] int width)
        {
            /*
             * 1.序列化json参数
             * 2.上传头像
             */

            var req = new
            {
                avatar,
                blurhash,
                height,
                width
            };

            var jsonSchema = await JsonSchemas.GetSchema("avatar-add");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            var logAvatar = new LogAvatar()
            {
                Id = Guid.NewGuid(),
                UId = UId,
                Blurhash = blurhash,
                Url = avatar,
                Height = height,
                Width = width,
                Created = DateTime.Now,
                Status = (sbyte)Status.UNDER_REVIEW
            };

            var res = await logAvatarService.AddLogAvatar(logAvatar);

            return res.Item1 ? Ok(Success("上传头像成功")) : Ok(Fail(res.Item2));
        }
    }
}