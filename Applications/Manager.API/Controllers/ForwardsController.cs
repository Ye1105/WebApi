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
    public class ForwardsController : ControllerBase
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
        public async Task<IActionResult> GetBlogForwardList([FromQuery] GetBlogForwardListRequest req)
        {
            /*
             * 1.转发列表
             * 2.转发用户的个人信息
             * 3.被转发用户的个人信息
             * 4.当 scope 不为 null 时关联 blog 查询
             * 5.当前转发记录的点赞数
             * 6.当前网站的登录用户是否对当前的转发记录点赞
             */

            var result = await blogForwardService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.Id, req.UId, req.BaseBId, req.PrevBId, req.BuId, req.PrevCId, req.StartTime, req.EndTime, req.WId, req.Scope, req.Status);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //2.转发用户的个人信息
                    item.UInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.UId);
                    //3.被转发用户的个人信息
                    item.BuInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.BuId);
                    //4.当 scope 不为 null 时关联 blog 查询
                    if (req.Scope != null)
                    {
                        item.Blog = await blogService.GetBlogBy(x => x.Id == item.Id, false);
                        if (item.Blog != null && req.WId != null && req.WId != Guid.Empty)
                        {
                            await blogService.GetBlogRelation(item.Blog, req.WId);
                        }
                    }
                    //5.当前转发记录的点赞数
                    item.Like = (await blogForwardLikeService.GetBlogForwardLikeCountBy(item.Id.Value)).Int();
                    //6,当前网站的登录用户是否对当前的转发记录点赞
                    if (req.WId != null && req.WId != Guid.Empty)
                    {
                        item.IsLike = await blogForwardLikeService.GetIsBlogForwardLikeByUser(item.Id.Value, req.WId.Value) != null;
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

                return Ok(ApiResult.Success("获取博客转发列表成功", JsonData));
            }
            return Ok(ApiResult.Fail("暂无数据"));
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

            //0.参数校验
            bool validator = JsonSchemaHelper.Validator<AddBlogForwardRequest>(req, out IList<string> errorMessages);
            if (!validator)
            {
                return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            }

            var id = Guid.NewGuid();
            var dt = DateTime.Now;
            var status = (sbyte)Status.Enable;

            // 1.新增【blog、blogComment、blogForward】事务
            var blog = new Blog()
            {
                Id = id,
                UId = req.Blog.UId,
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
                UId = req.BlogForward.UId,
                Message = req.BlogForward.Message,
                BaseBId = req.BlogForward.BaseBId,
                PrevBId = req.BlogForward.PrevBId,
                BuId = req.BlogForward.BuId,
                PrevCId = req.BlogForward.PrevCId,
                Created = dt,
                Status = status
            };

            var res = await blogForwardService.AddBlogForward(blog, blogForward);
            return res ? Ok(ApiResult.Success("转发博客成功")) : Ok(ApiResult.Fail("转发博客失败"));
        }

        /// <summary>
        ///博客转发：删除转发
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogForward(Guid id)
        {
            var res = await blogForwardService.DeleteBlogForward(id);
            return res.Item1 ? Ok(ApiResult.Success("删除转发成功")) : Ok(ApiResult.Fail(res.Item2));
        }

        /// <summary>
        /// 博客转发：新增点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{fId}/likes/{uId}")]
        public async Task<IActionResult> AddBlogForwardLike(Guid fId, Guid uId)
        {
            var res = await blogForwardLikeService.AddBlogForwadLike(fId, uId);
            return res.Item1 ? Ok(ApiResult.Success("博客转发点赞成功")) : Ok(ApiResult.Fail(res.Item2));
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{fId}/likes/{uId}")]
        public async Task<IActionResult> DeleteBlogForwardLike(Guid fId, Guid uId)
        {
            var res = await blogForwardLikeService.DelBlogForwardLike(fId, uId);

            return res.Item1 ? Ok(ApiResult.Success("取消博客转发点赞成功")) : Ok(ApiResult.Fail(res.Item2));
        }
    }
}