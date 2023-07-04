using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/ranks")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class RanksController : ControllerBase
    {
        private readonly IRankService rankService;
        private readonly IBlogService blogService;

        public RanksController(IRankService rankService, IBlogService blogService)
        {
            this.rankService = rankService;
            this.blogService = blogService;
        }

        /// <summary>
        /// 【榜单：热榜】
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("top")]
        public async Task<IActionResult> GetTopRankList([FromQuery] GetTopRankListRequest req)
        {
            var result = await rankService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.WId);
            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    await blogService.GetBlogRelation(item, req.WId);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };
                return Ok(ApiResult.Success("获取博客热门排行成功", JsonData));
            }
            return Ok(ApiResult.Fail("暂无数据"));
        }
    }
}