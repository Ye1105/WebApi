using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/forwards")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class ForwardsController : ApiController
    {
        private readonly IBlogForwardService blogForwardService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IBlogService blogService;
        private readonly IBlogForwardLikeService blogForwardLikeService;

        public ForwardsController(IBlogForwardService blogForwardService, IAccountInfoService accountInfoService, IBlogService blogService, IBlogForwardLikeService blogForwardLikeService)
        {
            this.blogForwardService = blogForwardService;
            this.accountInfoService = accountInfoService;
            this.blogService = blogService;
            this.blogForwardLikeService = blogForwardLikeService;
        }

        /// <summary>
        /// 转发列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Paged([FromQuery] GetBlogForwardListRequest req)
        {
            /*
             * 1.转发列表
             * 2.转发用户的个人信息
             * 3.被转发用户的个人信息
             * 4.当 scope 不为 null 时关联 blog 查询
             * 5.当前转发记录的点赞数
             * 6.当前网站的登录用户是否对当前的转发记录点赞
             */

            var result = await blogForwardService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, isTrack: false, req.OrderBy, req.Id, req.UId, req.BaseBId, req.PrevBId, req.BuId, req.PrevCId, req.StartTime, req.EndTime, UId, req.Scope, Status.ENABLE);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //2.转发用户的个人信息
                    item.UInfo = await accountInfoService.FirstOrDefaultAsync(item.UId);
                    //3.被转发用户的个人信息
                    item.BuInfo = await accountInfoService.FirstOrDefaultAsync(item.BuId);
                    //4.当 scope 不为 null 时关联 blog 查询
                    if (req.Scope != null)
                    {
                        item.Blog = await blogService.FirstOrDefaultAsync(x => x.Id == item.Id, false);
                        if (item.Blog != null)
                        {
                            await blogService.GetBlogRelation(item.Blog, UId);
                        }
                    }
                    //5.当前转发记录的点赞数
                    item.Like = await blogForwardLikeService.GetBlogForwardLikeCountBy(item.Id.Value);
                    //6,当前网站的登录用户是否对当前的转发记录点赞
                    item.IsLike = await blogForwardLikeService.GetIsBlogForwardLikeByUser(item.Id.Value, UId);
                }
                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };

                return Ok(Success("获取博客转发列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 新增转发
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBlogForward([FromBody] AddBlogForwardRequest req)
        {
            /*
             * 0.Json Schema 参数校验
             * 1.新增【blog、blogForward】事务
             */

            var id = Guid.NewGuid();
            var dt = DateTime.Now;
            var status = (sbyte)Status.ENABLE;

            // 0.参数校验[为实现，目标是为了规范message]

            // 1.新增【blog、blogComment、blogForward】事务

            var blog = new Blog()
            {
                Id = id,
                UId = UId,
                Sort = (sbyte)req.Blog.Sort,
                Type = (sbyte)req.Blog.Type,
                FId = req.Blog.FId,
                Body = req.Blog.Body,
                Created = dt,
                Status = status
            };

            var blogForward = new BlogForward()
            {
                Id = id,
                UId = UId,
                Message = req.BlogForward.Message,
                BaseBId = req.BlogForward.BaseBId,
                PrevBId = req.BlogForward.PrevBId,
                BuId = req.BlogForward.BuId,
                PrevCId = req.BlogForward.PrevCId,
                Created = dt,
                Status = status
            };

            var res = await blogForwardService.AddBlogForward(blog, blogForward);
            return res ? Ok(Success("转发博客成功")) : Ok(Fail("转发博客失败"));
        }

        /// <summary>
        ///博客转发：删除转发
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogForward(Guid id)
        {
            var res = await blogForwardService.DeleteBlogForward(id, UId);
            return res.Item1 ? Ok(Success("删除转发成功")) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 博客转发：新增点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{fId}/likes")]
        public async Task<IActionResult> AddBlogForwardLike(Guid fId)
        {
            var res = await blogForwardLikeService.AddBlogForwadLike(fId, UId);
            return res.Item1 ? Ok(Success("博客转发点赞成功")) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{fId}/likes")]
        public async Task<IActionResult> DeleteBlogForwardLike(Guid fId)
        {
            var res = await blogForwardLikeService.DelBlogForwardLike(fId, UId);

            return res.Item1 ? Ok(Success("取消博客转发点赞成功")) : Ok(Fail(res.Item2));
        }
    }
}