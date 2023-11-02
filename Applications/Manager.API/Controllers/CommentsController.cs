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
    [Route("v1/api/comments")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CommentsController : ApiController
    {
        private readonly IBlogCommentService blogCommentService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IBlogCommentLikeService blogCommentLikeService;
        private readonly IBlogService blogService;

        public CommentsController(IBlogCommentService blogCommentService, IAccountInfoService accountInfoService, IBlogCommentLikeService blogCommentLikeService, IBlogService blogService)
        {
            this.blogCommentService = blogCommentService;
            this.accountInfoService = accountInfoService;
            this.blogCommentLikeService = blogCommentLikeService;
            this.blogService = blogService;
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Paged([FromQuery] GetBlogCommentListRequest req)
        {
            /*
             * 1.评论列表
             * 2.评论用户的个人信息
             * 3.被评论用户的个人信息
             * 4.1 评论点赞数
             * 4.2 通过 wId!=null&&wId!=Guid.Empty 当前网站登录的用户id是否对当前评论或回复【点赞】
             * 5.通过 type=(sbyte)CommentTypeEnum.Comment 获取【评论的回复数】、【最新一条的回复用户数据】
             *
             */

            var typeList = new List<CommentType>();
            if (!string.IsNullOrEmpty(req.Types))
            {
                var arary = req.Types.Split(',');
                foreach (var type in arary)
                {
                    typeList.Add((CommentType)Convert.ToInt16(type));
                }
            }
            var types = typeList.ToArray();

            //1.评论列表
            var result = await blogCommentService.PagedAsync(req.PageIndex, req.PageSize, req.OffSet, isTrack: false, req.OrderBy, req.Id, req.BId, req.UId, types, req.PId, req.Grp, req.StartTime, req.EndTime, Status.ENABLE);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //关联数据
                    await CommentRelation(item, UId);

                    //5.0 通过 type=(sbyte)CommentTypeEnum.Comment 则获取评论的回复数 和 点赞第一高的回复数据
                    if (item.Type == (sbyte)CommentType.COMMENT)
                    {
                        //5.1 评论回复数
                        item.ReplyCount = await blogCommentService.CountAsync(x => x.Grp == item.Grp && (x.Type == (sbyte)CommentType.REPLY_FIRST_LEVEL || x.Type == (sbyte)CommentType.REPLY_SECOND_LEVEL) && x.Status == (sbyte)Status.ENABLE);

                        //5.2 评论最新回复的用户数据
                        if (item.ReplyCount > 0)
                        {
                            // 置顶 | 按时间降序 的三条数据
                            var replys = await blogCommentService.PagedAsync(b => b.Grp == item.Grp && (b.Type == (sbyte)CommentType.REPLY_FIRST_LEVEL || b.Type == (sbyte)CommentType.REPLY_SECOND_LEVEL) && b.Status == (sbyte)Status.ENABLE, pageIndex: 1, pageSize: 3, offset: 0, isTrack: false, "Top desc,Created desc");

                            foreach (var reply in replys)
                            {
                                var data = await CommentRelation(reply, UId);

                                item.ReplyList.Add(data);
                            }
                        }
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

                return Ok(Success("获取博客评论列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        private async Task<BlogComment> CommentRelation(BlogComment item, Guid? wId)
        {
            //2.评论用户的个人信息
            item.UInfo = await accountInfoService.FirstOrDefaultAsync(item.UId);

            //3.被评论用户的个人信息
            item.BuInfo = await accountInfoService.FirstOrDefaultAsync(item.BuId);

            //4.2通过 wId != null && wId != Guid.Empty 当前网站登录的用户id是否对当前评论或回复【点赞】
            if (wId != null && wId != Guid.Empty)
            {
                item.IsLike = await blogCommentLikeService.ExsitAsync(item.Id, wId.Value);
            }

            return item;
        }

        /// <summary>
        /// 增加【评论】|| 【回复】
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddBlogCommentRequest req)
        {
            /*
             * 0.参数校验 JSON SCHEMA
             * 1.参数组合
             * 2.增加评论
             */

            //0.参数校验 JSON SCHEMA
            //bool validator = JsonSchemaHelper.Validator<AddBlogCommentRequest>(req, out IList<string> errorMessages);
            //if (!validator)
            //{
            //    return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            //}

            //FIX: 重复性校验，短时间内不能重复提交多条相同的评论

            //1.参数组合
            var blogComment = new BlogComment()
            {
                Id = Guid.NewGuid(),
                BId = req.BId,
                BuId = req.BuId,
                UId = UId,
                Message = req.Message,
                Like = 0,
                Type = (sbyte)req.Type,
                PId = req.Type == CommentType.COMMENT ? Guid.Empty : req.PId,
                Grp = req.Type == CommentType.COMMENT ? Guid.NewGuid() : req.Grp,
                Created = DateTime.Now,
                Top = (sbyte)BoolType.NO,
                Status = (sbyte)Status.ENABLE
            };
            //2.增加评论
            var res = await blogCommentService.AddAsync(blogComment);

            return res ? Ok(Success("评论成功", new { comment = blogComment })) : Ok(Fail("评论失败"));
        }

        /// <summary>
        /// 评论并转发博客
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("forward")]
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
                Sort = (sbyte)BlogSort.PUBLIC,
                Type = (sbyte)BlogType.TEXT,
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
                PId = req.BlogComment.Type == CommentType.COMMENT ? Guid.Empty : req.BlogComment.PId,
                Grp = req.BlogComment.Type == CommentType.COMMENT ? Guid.NewGuid() : req.BlogComment.Grp,
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

            return res ? Ok(Success("评论转发成功", new { comment = blogComment, forward = blogForward })) : Ok(Fail("评论转发失败"));
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{type}/{grp}/{id}")]
        public async Task<IActionResult> DelComment(CommentType type, Guid grp, Guid id)
        {
            var res = await blogCommentService.DeleteAsync(type, grp, id);
            return Ok(res.Item1 ? Success(res.Item3, new { count = res.Item2 }) : Fail(res.Item3));
        }

        /// <summary>
        /// 新增点赞
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{id}/likes")]
        public async Task<IActionResult> AddCommentLike(Guid id)
        {
            var res = await blogCommentLikeService.AddAsync(id, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 博客评论：取消点赞
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/likes")]
        public async Task<IActionResult> DeleteCommentLike(Guid id)
        {
            var res = await blogCommentLikeService.DeleteAsync(id, UId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }
    }
}