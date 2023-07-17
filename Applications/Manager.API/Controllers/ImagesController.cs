using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/images")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class ImagesController : ApiController
    {
        private readonly IBlogImageService blogImageService;
        private readonly IBlogImageLikeService blogImageLikeService;

        public ImagesController(IBlogImageService blogImageService, IBlogImageLikeService blogImageLikeService)
        {
            this.blogImageService = blogImageService;
            this.blogImageLikeService = blogImageLikeService;
        }

        /// <summary>
        /// 图片列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Paged([FromQuery] GetBlogImageListRequest req)
        {
            var result = await blogImageService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, isTrack: false, req.OrderBy, req.BId, req.UId, req.StartTime, req.EndTime, Status.ENABLE);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //当前图片点赞数
                    item.Like = await blogImageLikeService.GetBlogImageLikeCountBy(item.Id);
                    //关联当前用户是否点赞
                    item.IsLike = await blogImageLikeService.GetIsBlogImageLikeByUser(item.Id, UId);
                }

                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(Success("获取博客图片列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 新增点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{iId}/likes")]
        public async Task<IActionResult> AddBlogImageLike(Guid iId)
        {
            var res = await blogImageLikeService.AddBlogImageLike(iId, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 博客图片点赞：删除点赞
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{iId}/likes")]
        public async Task<IActionResult> DeleteBlogImageLike(Guid iId)
        {
            var res = await blogImageLikeService.DelBlogImageLike(iId, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }
    }
}