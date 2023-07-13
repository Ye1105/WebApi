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
    [Route("v1/api/covers")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CoversController : ApiController
    {
        private readonly ILogCoverService logCoverService;

        public CoversController(ILogCoverService logCoverService)
        {
            this.logCoverService = logCoverService;
        }

        /// <summary>
        /// 用户封面：分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CoverPaged([FromQuery] QueryParameters req)
        {
            var result = await logCoverService.GetPagedList(UId, req.PageIndex, req.PageSize, req.OffSet, req.OrderBy);

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

        /// <summary>
        /// 上传封面
        /// </summary>
        /// <param name="uId">用户Id</param>
        /// <param name="cover"></param>
        /// <param name="avatar">头像地址</param>
        /// <param name="blurhash">模糊哈希</param>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadCover([FromForm] string cover, [FromForm] string blurhash, [FromForm] int height, [FromForm] int width)
        {
            /*
             * 1.序列化json参数
             * 2.上传封面
             */

            var req = new
            {
                cover,
                blurhash,
                height,
                width
            };

            var jsonSchema = await JsonSchemas.GetSchema("cover-add");

            var schema = JSchema.Parse(jsonSchema);

            var validate = JObject.Parse(JsonConvert.SerializeObject(req)).IsValid(schema, out IList<string> errorMessages);
            if (!validate)
            {
                return Ok(Fail(errorMessages, "参数错误"));
            }

            var logCover = new LogCover()
            {
                Id = Guid.NewGuid(),
                UId = UId,
                Blurhash = blurhash,
                Url = cover,
                Height = height,
                Width = width,
                Created = DateTime.Now,
                Status = (sbyte)Status.UNDER_REVIEW
            };

            var res = await logCoverService.AddLogCover(logCover);

            return res.Item1 ? Ok(Success("上传封面成功")) : Ok(Fail(res.Item2));
        }
    }
}