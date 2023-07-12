﻿using Manager.API.Utility;
using Manager.API.Utility.Filters;
using Manager.Core;
using Manager.Core.AuthorizationModels;
using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Models.Users;
using Manager.Core.RequestModels;
using Manager.Extensions;
using Manager.Server.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.RegularExpressions;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/api/blogs")]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionInfo.V1))]
    [TypeFilter(typeof(CustomLogAsyncActionFilterAttribute))]
    [CustomExceptionFilter]
    public class BlogsController : ApiController
    {
        private readonly IBlogService blogService;
        private readonly IBlogTopicService blogTopicService;
        private readonly IBlogLikeService blogLikeService;
        private readonly IBlogFavoriteService blogFavoriteService;
        private readonly object CreateLock = new();

        public BlogsController(IBlogService blogService, IBlogTopicService blogTopicService, IBlogLikeService blogLikeService, IBlogFavoriteService blogFavoriteService)
        {
            this.blogService = blogService;
            this.blogTopicService = blogTopicService;
            this.blogLikeService = blogLikeService;
            this.blogFavoriteService = blogFavoriteService;
        }

        /// <summary>
        /// 发表博客
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateBlog([FromBody] CreateBlogRequest req)
        {
            try
            {
                lock (CreateLock)
                {
                    /*
                     * 0. Json Schema 参数校验
                     * 1. 敏感词校验
                     * 2. blog 参数赋值
                     * 3.1 重复性校验：1分钟内是否发布超过10条博客
                     * 3.2 重复性校验：10分钟内是否存在相同的博客
                     * 4.1 创建blog
                     * 4.2 如果发表的是图片blog，创建图片实例
                     * 4.3 如果发布的是视频blog，创建视频实例
                     * 5. 正则:是否有新话题产生
                     * 6. 写入数据
                     * 7. 返回数据
                     */

                    //0.Json Schema 参数校验
                    //bool validator = JsonSchemaHelper.Validator<CreateBlogRequest>(req, out IList<string> errorMessages);
                    //if (!validator)
                    //{
                    //    return Ok(ApiResult.Fail(errorMessages, "参数错误"));
                    //}

                    //1.敏感词校验

                    //2.blog 参数赋值
                    var dt = DateTime.Now;
                    var bId = Guid.NewGuid();

                    //3.1 重复性校验：1分钟内是否发布超过10条博客
                    var filterCount = blogService.GetBlogCountBySync(x => x.UId == req.UId && x.Created > dt.AddMinutes(-1), false);
                    if (filterCount > 10)
                    {
                        return Ok(Fail("1分钟内发布超过10条博客", "您发布博客的频率过高，请稍后重试"));
                    }

                    //3.2 重复性校验：10分钟内是否存在相同的博客
                    var filterBody = blogService.GetBlogCountBySync(x => x.UId == req.UId && x.Created > dt.AddMinutes(-10) && x.Body == req.Body, false);
                    if (filterCount > 0)
                    {
                        return Ok(Fail("10分钟内有已有重复博客", "文本内容相同，请隔10分钟后发布"));
                    }

                    //4.1创建blog
                    var blog = new Blog()
                    {
                        Id = bId,
                        UId = req.UId,
                        Sort = (sbyte)req.Sort,
                        Type = (sbyte)req.Type,
                        Body = req.Body,
                        FId = Guid.Empty,
                        Created = dt,
                        Top = (sbyte)BoolType.NO,
                        Status = req.Type == BlogType.TEXT ? (sbyte)Status.ENABLE : (sbyte)Status.UNDER_REVIEW
                    };

                    //4.2 如果发布的是图片blog，创建图片实例
                    if (req.Type == (int)BlogType.IMAGE)
                    {
                        if (req.Images != null && req.Images.Any())
                        {
                            blog.Images = new List<BlogImage>();
                            foreach (var item in req.Images)
                            {
                                blog.Images.Add(new BlogImage()
                                {
                                    Id = Guid.NewGuid(),
                                    UId = req.UId,
                                    BId = bId,
                                    Url = item.Url,
                                    Blurhash = item.Blurhash,
                                    Width = item.Width,
                                    Height = item.Height,
                                    Created = dt,
                                    Status = (sbyte)Status.UNDER_REVIEW,
                                });
                            };
                        }
                        else
                        {
                            return Ok(Fail("图片列表为空"));
                        }
                    }

                    //4.3 如果发布的是视频blog，创建视频实例
                    if ((sbyte)req.Type == (sbyte)BlogType.VIDEO)
                    {
                        if (req.Video != null)
                        {
                            var v = req.Video;

                            blog.Video = new BlogVideo()
                            {
                                Id = Guid.NewGuid(),
                                UId = req.UId,
                                BId = bId,
                                Title = v.Title,
                                Channel = v.Channel.SerObj(),
                                Collection = v.Collection.SerObj(),
                                Type = v.Type.SerObj(),
                                Url = v.Url,
                                Duration = v.Duration,
                                Created = dt,
                                Status = (sbyte)Status.UNDER_REVIEW,
                                CUrl = v.CUrl,
                                CWidth = v.CWidth,
                                CHeight = v.CHeight,
                                CHashblur = v.CHashblur
                            };
                        }
                        else
                        {
                            return Ok(Fail("视频为空"));
                        }
                    }

                    List<BlogTopic> blogTopics = new();
                    UserTopic? userTopic = null;
                    //5. 正则:是否有新话题产生
                    if (RegexHelper.RegexList(
                            req.Body,
                            RegexHelper.TopicPattern,
                            (context, pattern) =>
                            {
                                List<string> list = new();
                                foreach (Match match in Regex.Matches(context, pattern, RegexOptions.Multiline).Cast<Match>()) //MatchCollection
                                {
                                    if (match != null)
                                    {
                                        list.Add(match.Value);
                                    }
                                }
                                return list;
                            },
                            out List<string> tags)
                        )
                    {
                        tags.ForEach(t =>
                        {
                            //判定是否已经存在话题 blog_topic user_topic
                            var topic = blogTopicService.GetBlogTopicBySync(x => x.Title == t, false);
                            if (topic == null)
                            {
                                var Id = Guid.NewGuid();
                                blogTopics.Add(
                                    new BlogTopic()
                                    {
                                        Id = Id,
                                        Title = t,
                                        DiscussCount = 0,
                                        ReadCount = 0,
                                        SearchCount = 0,
                                        Type = 0,
                                        Status = (sbyte)Status.ENABLE,
                                        Created = dt,
                                    }
                                );
                                userTopic = new UserTopic()
                                {
                                    Id = Guid.NewGuid(),
                                    TopicId = Id,
                                    UId = req.UId,
                                    Created = dt
                                };
                            }
                            else
                            {
                                userTopic = new UserTopic()
                                {
                                    Id = Guid.NewGuid(),
                                    TopicId = topic.Id,
                                    UId = req.UId,
                                    Created = dt
                                };
                            }
                        });
                    }

                    //6.写入数据
                    if (blogService.CreateBlogSync(blog, blogTopics, userTopic))
                    {
                        // 6. 返回数据
                        var newBlog = blogService.GetBlogBySync(x => x.Id == bId);
                        if (newBlog == null)
                        {
                            return Ok(Success("发布博客失败", new { }));
                        }
                        else
                        {
                            blogService.GetBlogRelation(newBlog, req.UId).Wait();
                            return Ok(Success("发布博客成功", newBlog));
                        }
                    }
                    return Ok(Fail("发布博客失败"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"CreateBlog 【{0} 】【{1}】", "发布博客异常", ex.ToString());
                return Ok(Fail(ex.ToString(), "发布博客异常"));
            }
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBlogList([FromQuery] GetBlogListRequest req)
        {
            var result = await blogService.GetPagedList(req.PageIndex, req.PageSize, req.OffSet, false, req.OrderBy, req.Id, req.WId, req.UId, req.Sort == null ? null : (sbyte)req.Sort.Value, req.Type == null ? null : (sbyte)req.Type.Value, req.IsFId, req.StartTime, req.EndTime, req.Scope == null ? null : (sbyte)req.Scope.Value, req.Grp, req.Status);

            if (result != null && result.Any())
            {
                foreach (var item in result)
                {
                    await blogService.GetBlogRelation(item, req.WId);
                }
                var JsonData = new
                {
                    pageCount = result.TotalPages,
                    currentPage = result.CurrentPage,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    list = result
                };
                return Ok(Success("获取博客列表成功", JsonData));
            }
            return Ok(Fail("暂无数据"));
        }

        /// <summary>
        /// 修改博客分类
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sort"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Policy = Policys.VIP)]
        [HttpPatch("{id}/sort/{sort}")]
        public async Task<IActionResult> EditBlogSort(Guid id, BlogSort sort)
        {
            var blog = await blogService.GetBlogBy(x => x.Id == id && x.Status == (sbyte)Status.ENABLE && x.Sort != (sbyte)sort);
            if (blog == null)
            {
                return Ok(Fail(""));
            }

            blog.Sort = (sbyte)sort;

            if (await blogService.ModifyBlog(blog))
            {
                return Ok(Success("修改成功"));
            }
            else
            {
                return Ok(Fail("修改失败"));
            }
        }

        /// <summary>
        /// 博客置顶
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPatch("{id}/{uId}/top")]
        public async Task<IActionResult> SetBlogTop(Guid id, Guid uId)
        {
            /*
             * 0.判定当前用户是否已经有博客置顶
             * 1.没有则置顶
             */

            var count = await blogService.GetBlogCountBy(x => x.UId == uId && x.Top == (sbyte)BoolType.YES && x.Status == (sbyte)Status.ENABLE);
            if (count > 0)
            {
                return Ok(Fail("已存在置顶博客", "已存在置顶博客，请先取消置顶"));
            }

            var blog = await blogService.GetBlogBy(x => x.Id == id && x.FId == Guid.Empty && x.Top == (sbyte)BoolType.NO && x.Status == (sbyte)Status.ENABLE);
            if (blog != null)
            {
                blog.Top = (sbyte)BoolType.YES;

                var res = await blogService.ModifyBlog(blog);

                return res ? Ok(Success("博客置顶成功")) : Ok(Fail("博客置顶失败"));
            }
            else
            {
                return Ok(Fail(""));
            }
        }

        /// <summary>
        /// 博客：取消置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/untop")]
        public async Task<IActionResult> DeleteBlogTop(Guid id)
        {
            var blog = await blogService.GetBlogBy(x => x.Id == id && x.FId == Guid.Empty && x.Status == (sbyte)Status.ENABLE && x.Top == (sbyte)BoolType.YES);
            if (blog != null)
            {
                blog.Top = (sbyte)BoolType.NO;

                var res = await blogService.ModifyBlog(blog);

                return res ? Ok(Success("取消置顶成功")) : Ok(Fail("取消置顶失败"));
            }
            else
            {
                return Ok(Fail("博客不存在"));
            }
        }

        /// <summary>
        /// 删除博客
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/{uId}")]
        public async Task<IActionResult> DeleteBlog(Guid id, Guid uId)
        {
            /* 1. 判定当前blog是否时转发blog
             * 1.1如果是原创blog
             * 1.1.0 原创blog type 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
             * 1.1.1 事务删除【博客、视频】
             * 1.1.2 事务删除【博客、图片】
             * 1.1.3 只删除博客
             * 1.2  事务删除转发
             */

            var blog = await blogService.GetBlogBy(x => x.Id == id && x.UId == uId && x.Status == (sbyte)Status.ENABLE);
            if (blog == null)
            {
                return Ok(Fail("博客不存在"));
            }

            var res = await blogService.DelBlog(blog);

            return res.Item1 ? Ok(Success("删除成功")) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 博客点赞
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize(Policy = Policys.VIP)]
        [HttpPost("{bId}/likes/{uId}")]
        public async Task<IActionResult> AddBlogLike(Guid bId, Guid uId)
        {
            var res = await blogLikeService.AddBlogLike(bId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 删除点赞
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{bId}/likes/{uId}")]
        public async Task<IActionResult> DeleteBlogLike(Guid bId, Guid uId)
        {
            var res = await blogLikeService.DelBlogLike(bId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 博客收藏
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpPost("{bId}/favours/{uId}")]
        public async Task<IActionResult> AddBlogFavorite(Guid bId, Guid uId)
        {
            /*
             * 0.参数校验 JSON SCHEMA
             * 1.参数组合
             * 2.增加博客收藏
             */

            ////0.参数校验 JSON SCHEMA
            //bool validator = JsonSchemaHelper.Validator<AddBlogFavoriteRequest>(req, out IList<string> errorMessages);
            //if (!validator)
            //{
            //    return Ok(ApiResult.Fail(errorMessages, "参数错误"));
            //}

            //2.增加博客收藏
            var res = await blogFavoriteService.AddBlogFavorite(bId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="bId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        [HttpDelete("{bId}/favours/{uId}")]
        public async Task<IActionResult> DeleteBlogFavorite(Guid bId, Guid uId)
        {
            var res = await blogFavoriteService.DelBlogFavorite(bId, uId);
            return res.Item1 ? Ok(Success()) : Ok(Fail(res.Item2));
        }

        /// <summary>
        /// 某年每月博客数量
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet("{uId}/groupcounts/{year}")]
        public async Task<IActionResult> GetBlogGroupCount(Guid uId, int year)
        {
            var res = await blogService.GetBlogCountGroupbyMonth(uId, year);
            return Ok(Success(res));
        }
    }
}