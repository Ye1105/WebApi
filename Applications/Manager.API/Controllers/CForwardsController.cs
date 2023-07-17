using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.RequestModels;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/cforwards")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CForwardsController : ApiController
    {
        private readonly IBlogService blogService;

        public CForwardsController(IBlogService blogService)
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
            //bool validator = JsonSchemaHelper.Validator<AddBlogCommentAndForwardRequest>(req, out IList<string> errorMessages);
            //if (!validator)
            //{
            //    return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            //}

            var id = Guid.NewGuid();
            var dt = DateTime.Now;
            var status = (sbyte)Status.ENABLE;

            //1.判断 blog 是否存在
            var v = await blogService.FirstOrDefaultAsync(x => x.Id == req.BlogComment.BId && x.Status == (sbyte)Status.ENABLE, false);
            if (v == null)
            {
                return Ok(Fail("博客不存在"));
            }

            // 2.新增【blog、blogComment、blogForward】事务
            var blog = new Blog()
            {
                Id = id,
                UId = UId,
                Sort = (sbyte)req.Blog.Sort,
                Type = (sbyte)req.Blog.Type,
                Body = req.Blog.Body,
                Top = (sbyte)BoolType.NO,
                Created = dt,
                Status = status,
                FId = req.Blog.FId
            };

            var blogComment = new BlogComment()
            {
                Id = id,
                BId = req.BlogComment.BId,
                UId = UId,
                BuId = req.BlogComment.BuId,
                Message = req.BlogComment.Message,
                Type = (sbyte)req.BlogComment.Type,
                PId = req.BlogComment.PId,
                Grp = req.BlogComment.Grp,
                Created = dt,
                Top = (sbyte)BoolType.NO,
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

            var res = await blogService.AddBlogCommentAndForward(blog, blogComment, blogForward);
            return res ? Ok(Success("评论并转发博客成功")) : Ok(Fail("评论转发博客失败"));
        }
    }
}