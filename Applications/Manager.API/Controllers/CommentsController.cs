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
    [Route("v1/api/comments")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [CustomExceptionFilter]
    public class CommentsController : ControllerBase
    {
        private readonly IBlogCommentService blogCommentService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IBlogCommentLikeService blogCommentLikeService;

        public CommentsController(IBlogCommentService blogCommentService, IAccountInfoService accountInfoService, IBlogCommentLikeService blogCommentLikeService)
        {
            this.blogCommentService = blogCommentService;
            this.accountInfoService = accountInfoService;
            this.blogCommentLikeService = blogCommentLikeService;
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBlogCommentList([FromQuery] GetBlogCommentListRequest req)
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

            //1.评论列表
            var result = await blogCommentService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.Id, req.BId, req.UId, req.Types, req.PId, req.Grp, req.StartTime, req.EndTime, req.Status);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    //关联数据
                    await BlogCommentRelation(item, req.WId);

                    //5.0 通过 type=(sbyte)CommentTypeEnum.Comment 则获取评论的回复数 和 点赞第一高的回复数据
                    if (item.Type == (sbyte)CommentTypeEnum.Comment)
                    {
                        //5.1 评论回复数
                        item.ReplyCount = await blogCommentService.GetBlogCommentCountBy(x => x.Grp == item.Grp && (x.Type == (sbyte)CommentTypeEnum.ReplyFir || x.Type == (sbyte)CommentTypeEnum.ReplySec) && x.Status == (sbyte)Status.Enable);

                        //5.2 评论最新回复的用户数据
                        if (item.ReplyCount > 0)
                        {
                            // 置顶 | 按时间降序 的三条数据
                            var replys = await blogCommentService.GetPagedList(b => b.Grp == item.Grp && (b.Type == (sbyte)CommentTypeEnum.ReplyFir || b.Type == (sbyte)CommentTypeEnum.ReplySec) && b.Status == (sbyte)Status.Enable, 1, 3, 0, false, "Top desc,Created desc");

                            foreach (var reply in replys)
                            {
                                var data = await BlogCommentRelation(reply, req.WId);

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

                return Ok(ApiResult.Success("获取博客评论列表成功", JsonData));
            }
            return Ok(ApiResult.Fail("暂无数据"));
        }

        private async Task<BlogComment> BlogCommentRelation(BlogComment item, Guid? wId)
        {
            //2.评论用户的个人信息
            item.UInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.UId);

            //3.被评论用户的个人信息
            item.BuInfo = await accountInfoService.GetAccountInfoAndAvatarAndCoverById(item.BuId);

            //4.1点赞数
            item.Like = (await blogCommentLikeService.GetBlogCommentLikeCountBy(item.Id)).Int();

            //4.2通过 wId != null && wId != Guid.Empty 当前网站登录的用户id是否对当前评论或回复【点赞】
            if (wId != null && wId != Guid.Empty)
            {
                item.IsLike = await blogCommentLikeService.GetIsBlogCommentLikeByUser(item.Id, wId.Value);
            }

            return item;
        }

        /// <summary>
        /// 增加【评论】|| 【回复】
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBlogComment([FromBody] AddBlogCommentRequest req)
        {
            /*
             * 0.参数校验 JSON SCHEMA
             * 1.参数组合
             * 2.增加评论
             */

            //0.参数校验 JSON SCHEMA
            bool validator = JsonSchemaHelper.Validator<AddBlogCommentRequest>(req, out IList<string> errorMessages);
            if (!validator)
            {
                return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            }
            //1.参数组合
            var blogComment = new BlogComment()
            {
                Id = Guid.NewGuid(),
                BId = req.BId,
                UId = req.UId,
                BuId = req.BuId,
                Message = req.Message,
                Like = 0,
                Type = (sbyte)req.Type,
                PId = req.PId,
                Grp = req.Grp,
                Created = DateTime.Now,
                Top = (sbyte)TopEnum.no,
                Status = (sbyte)Status.Enable
            };
            //2.增加评论
            var res = await blogCommentService.AddBlogComment(blogComment);
            return res ? Ok(ApiResult.Success("评论成功")) : Ok(ApiResult.Fail("评论失败"));
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grp"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{type}/{grp}/{id}")]
        public async Task<IActionResult> DeleteBlogComment(CommentTypeEnum type, Guid grp, Guid id)
        {
            var res = await blogCommentService.DeleteBlogComment(type, grp, id);
            return Ok(res.Item1 ? ApiResult.Success(res.Item2) : ApiResult.Fail(res.Item2));
        }

        /// <summary>
        /// 新增点赞
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{id}/likes/{uId}")]
        public async Task<IActionResult> AddBlogCommentLike(Guid id, Guid uId)
        {
            var res = await blogCommentLikeService.AddBlogCommentLike(id, uId);
            return res.Item1 ? Ok(ApiResult.Success()) : Ok(ApiResult.Fail(res.Item2));
        }

        /// <summary>
        /// 博客评论：取消点赞
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/likes/{uId}")]
        public async Task<IActionResult> DeleteBlogCommentLike(Guid id, Guid uId)
        {
            var res = await blogCommentLikeService.DeleteBlogCommentLike(id, uId);
            return res.Item1 ? Ok(ApiResult.Success()) : Ok(ApiResult.Fail(res.Item2));
        }
    }
}