using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/videos")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [TypeFilter(typeof(CustomExceptionFilterAttribute))]
    public class VideosController : ApiController
    {
        private readonly IBlogVideoLikeService blogVideoLikeService;
        private readonly IBlogVideoService blogVideoService;

        public VideosController(IBlogVideoLikeService blogVideoLikeService, IBlogVideoService blogVideoService)
        {
            this.blogVideoLikeService = blogVideoLikeService;
            this.blogVideoService = blogVideoService;
        }

        /// <summary>
        /// 视频列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Paged([FromQuery] GetBlogVideoListRequest req)
        {
            var result = await blogVideoService.PagedAsync(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.BId, req.UId, req.Title, req.Channel, req.Collection, req.Type, req.StartTime, req.EndTime, Status.ENABLE);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //当前视频点赞数
                    item.Like = await blogVideoLikeService.CountAsync(item.Id);
                    //关联当前用户是否点赞
                    item.IsLike = await blogVideoLikeService.ExsitAsync(item.Id, UId);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(Success("获取博客视频列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 新增点赞
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{vId}/likes")]
        public async Task<IActionResult> AddBlogVideoLike(Guid vId)
        {
            var res = await blogVideoLikeService.AddAsync(vId, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 删除点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{vId}/likes")]
        public async Task<IActionResult> DeleteBlogVideoLike(Guid vId)
        {
            var res = await blogVideoLikeService.DeleteAsync(vId, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }
    }
}