using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogCommentLikeService : IBlogCommentLikeService
    {
        private readonly IBase baseService;

        public BlogCommentLikeService(IBase baseService)
        {
            this.baseService = baseService;
        }

        // public async Task<long?> CountAsync(Guid cId)
        // {
        //try
        //{
        //    /*
        //     * 1.缓存是否命中
        //     * 2.命中则直接获取缓存值
        //     * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
        //     */

        //    var keyName = $"{RedisConstants.PREFIX_COMMENT_LIKE_COUNT}{cId}";

        //    using var cli = Instance(RedisBaseEnum.Zeroth);

        //    var res = await cli.ExistsAsync(keyName);

        //    if (res)
        //    {
        //        var count = await cli.GetAsync(keyName);
        //        return Convert.ToInt64(count);
        //    }
        //    else
        //    {
        //        var count = await baseService.Entities<BlogCommentLike>().Where(x => x.CId == cId).CountAsync();

        //        await cli.SetExAsync(keyName, 300, count);

        //        return await CountAsync(cId);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Log.Error($"BlogCommentLike_GetBlogCommentLikeCountBy:{ex}");
        //    return Convert.ToInt64(0);
        //}
        //}

        public async Task<bool?> ExsitAsync(Guid cId, Guid uId)
        {
            return await baseService.Entities<BlogCommentLike>().Where(x => x.CId == cId && x.UId == uId).CountAsync() > 0;
            //try
            //{
            //    /*
            //     * 1.缓存是否命中
            //     * 2.命中则直接获取缓存值
            //     * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
            //     */

            //    var keyName = $"{RedisConstants.PREFIX_COMMENT_ISLIKE}{cId}:{uId}";

            //    using var cli = Instance(RedisBaseEnum.Zeroth);

            //    var res = await cli.ExistsAsync(keyName);

            //    //命中缓存
            //    if (res)
            //    {
            //        var value = await cli.GetAsync(keyName);
            //        return value.Int() > 0;
            //    }
            //    else
            //    {
            //        var count = await baseService.Entities<BlogCommentLike>().Where(x => x.CId == cId && x.UId == uId).CountAsync();
            //        //设置缓存
            //        await cli.SetExAsync(keyName, 300, count);

            //        return await ExsitAsync(cId, uId);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error($"BlogCommentLike_GetIsBlogCommentLikeByUser:{ex}");
            //    return false;
            //}
        }

        public async Task<Tuple<bool, string>> AddAsync(Guid cId, Guid uId)
        {
            try
            {
                /*
                * 新增点赞
                * 删除缓存
                */

                var blogComment = await baseService.FirstOrDefaultAsync<BlogComment>(x => x.Id == cId && x.Status == (int)Status.ENABLE, isTrack: true);
                if (blogComment == null)
                    return Tuple.Create(false, "评论不存在");

                blogComment.Like++;

                var commentLike = new BlogCommentLike()
                {
                    CId = cId,
                    UId = uId,
                    Created = DateTime.Now,
                };

                var dic = new Dictionary<object, CrudEnum>()
                {
                    {blogComment,CrudEnum.UPDATE },
                    { commentLike,CrudEnum.CREATE}
                };

                var res = await baseService.BatchTransactionAsync(dic);

                //if (res)
                //{
                //    using var cli = Instance(RedisBaseEnum.Zeroth);

                //    //评论的点赞数量
                //    //var keyNameBlogCommentLikeCount = $"{RedisConstants.PREFIX_COMMENT_LIKE_COUNT}{cId}";

                //    //await cli.DelAsync(keyNameBlogCommentLikeCount, keyNameBlogCommentLike);

                //    //当前用户是否点赞
                //    //var keyNameBlogCommentLike = $"{RedisConstants.PREFIX_COMMENT_ISLIKE}{cId}:{uId}";

                //    //await cli.DelAsync(keyNameBlogCommentLike);
                //}

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"BlogCommentLike_AddBlogCommentLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DeleteAsync(Guid cId, Guid uId)
        {
            try
            {
                /*
                 * 校验点赞
                 * 删除点赞
                 * 删除缓存
                 */

                var blogComment = await baseService.FirstOrDefaultAsync<BlogComment>(x => x.Id == cId && x.Status == (int)Status.ENABLE, isTrack: true);
                if (blogComment == null)
                    return Tuple.Create(false, "评论不存在");

                var commentLike = await baseService.FirstOrDefaultAsync<BlogCommentLike>(x => x.CId == cId && x.UId == uId, true);
                if (commentLike == null)
                    return Tuple.Create(false, "点赞不存在");

                blogComment.Like--;

                var dic = new Dictionary<object, CrudEnum>()
                {
                    { blogComment,CrudEnum.UPDATE },
                    { commentLike,CrudEnum.DELETE}
                };

                var res = await baseService.BatchTransactionAsync(dic);

                //if (res)
                //{
                //    using var cli = Instance(RedisBaseEnum.Zeroth);

                //    //评论的点赞数量
                //    //var keyNameBlogCommentLikeCount = $"{RedisConstants.PREFIX_COMMENT_LIKE_COUNT}{cId}";

                //    // await cli.DelAsync(keyNameBlogCommentLikeCount, keyNameBlogCommentLike);

                //    //当前用户是否点赞
                //    //var keyNameBlogCommentLike = $"{RedisConstants.PREFIX_COMMENT_ISLIKE}{cId}:{uId}";

                //    //await cli.DelAsync(keyNameBlogCommentLike);
                //}

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"BlogCommentLike_DeleteBlogCommentLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }
    }
}