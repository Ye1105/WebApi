using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.RequestModels;
using Manager.Extensions;
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
        public async Task<IActionResult> GetBlogVideoList([FromQuery] GetBlogVideoListRequest req)
        {
            var result = await blogVideoService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.BId, req.UId, req.Title, req.Channel, req.Collection, req.Type, req.StartTime, req.EndTime);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //当前视频点赞数
                    item.Like = (await blogVideoLikeService.GetBlogVideoLikeCountBy(item.Id)).Int();
                    if (req.WId != null && req.WId != Guid.Empty)
                    {
                        //关联当前用户是否点赞
                        item.IsLike = await blogVideoLikeService.GetIsBlogVideoLikeByUser(item.Id, req.WId.Value) != null;
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
        [HttpPost("{vId}/likes/{uId}")]
        public async Task<IActionResult> AddBlogVideoLike(Guid vId, Guid uId)
        {
            var res = await blogVideoLikeService.AddBlogVideoLike(vId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 删除点赞
        /// </summary>
        /// <param name="vId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{vId}/likes/{uId}")]
        public async Task<IActionResult> DeleteBlogVideoLike(Guid vId, Guid uId)
        {
            var res = await blogVideoLikeService.DelBlogVideoLike(vId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }
    }
}