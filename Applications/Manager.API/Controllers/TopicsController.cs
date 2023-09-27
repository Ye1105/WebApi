using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    /// <summary>
    /// 话题控制器
    /// </summary>
    [ApiController]
    [Route("v1/api/topics")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class TopicsController : ApiController
    {
        private readonly IBlogTopicService blogTopicService;

        public TopicsController(IBlogTopicService blogTopicService)
        {
            this.blogTopicService = blogTopicService;
        }

        /// <summary>
        /// 搜索匹配关键字前十的话题
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            /*
             * FIX：目前测试，后期使用全文索引器的检索接口
             */
            var result = await blogTopicService.PagedAsync(x =>
               x.Status == (sbyte)Status.ENABLE && x.Title.Contains(keyword),
                pageIndex: 1,
                pageSize: 10,
                offset: 0,
                isTrack: false,
                orderBy: "readcount desc,created desc"
            );

            return Ok(Success(new { list = result }));
        }
    }
}
