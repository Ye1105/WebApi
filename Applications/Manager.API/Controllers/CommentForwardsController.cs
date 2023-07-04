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
    [Route("v1/api/comment-forwards")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CommentForwardsController : ControllerBase
    {
        private readonly IBlogService blogService;

        public CommentForwardsController(IBlogService blogService)
        {
            this.blogService = blogService;
        }

        /// <summary>
        /// 评论并转发博客
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBlogCommentAndForward([FromBody] AddBlogCommentAndForwardRequest req)
        {
            /*
             * 0.Json Schema 参数校验
             * 1.判断 blog 是否存在
             * 2.新增【blog、blogComment、blogForward】事务
             */

            //0.参数校验
            bool validator = JsonSchemaHelper.Validator<AddBlogCommentAndForwardRequest>(req, out IList<string> errorMessages);
            if (!validator)
            {
                return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            }

            var id = Guid.NewGuid();
            var dt = DateTime.Now;
            var status = (sbyte)Status.Enable;

            //1.判断 blog 是否存在
            var v = await blogService.GetBlogBy(x => x.Id == req.BlogComment.BId && x.Status == (sbyte)Status.Enable, false);
            if (v == null)
            {
                return Ok(ApiResult.Fail("博客不存在"));
            }

            // 2.新增【blog、blogComment、blogForward】事务
            var blog = new Blog()
            {
                Id = id,
                UId = req.Blog.UId,
                Sort = (sbyte)req.Blog.Sort,
                Type = (sbyte)req.Blog.Type,
                FId = req.Blog.FId,
                Body = req.Blog.Body,
                Top = (sbyte)TopEnum.no,
                Created = dt,
                Status = status
            };

            var blogComment = new BlogComment()
            {
                Id = id,
                BId = req.BlogComment.BId,
                UId = req.BlogComment.UId,
                BuId = req.BlogComment.BuId,
                Message = req.BlogComment.Message,
                Type = (sbyte)req.BlogComment.Type,
                PId = req.BlogComment.PId,
                Grp = req.BlogComment.Grp,
                Created = dt,
                Top = (sbyte)TopEnum.no,
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

            var res = await blogService.AddBlogCommentAndForward(blog, blogComment, blogForward);
            return res ? Ok(ApiResult.Success("评论并转发博客成功")) : Ok(ApiResult.Fail("评论转发博客失败"));
        }
    }
}