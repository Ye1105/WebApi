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
    [Route("v1/api/likes")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class LikesController : ApiController
    {
        private readonly IBlogLikeService blogLikeService;
        private readonly IBlogService blogService;

        public LikesController(IBlogLikeService blogLikeService, IBlogService blogService)
        {
            this.blogLikeService = blogLikeService;
            this.blogService = blogService;
        }

        /// <summary>
        /// 点赞的博客列表
        /// </summary>
        /// <param name="wId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet("blogs")]
        public async Task<IActionResult> LikeBlogPaged([FromQuery] GetBlogLikeListRequest req)
        {
            var result = await blogLikeService.GetPagedList(UId, req.PageIndex, req.PageSize, req.OffSet);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    await blogService.GetBlogRelation(item, UId);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };
                return Ok(Success("获取点赞的博客列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }
    }
}