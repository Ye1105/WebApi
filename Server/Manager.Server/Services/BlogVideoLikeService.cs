using Manager.Core.Models.Blogs;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogVideoLikeService : IBlogVideoLikeService
    {
        private readonly IBase baseService;

        /// <summary>
        ///【前缀】博客视频点赞数量
        /// </summary>
        private readonly string Prefix_VideoLikeCount = "VideoLike:Count:";

        /// <summary>
        ///【前缀】博客视频用户是否点赞
        /// </summary>
        private readonly string Prefix_IsVideoLike = "VideoLike:IsLike:";

        /// <summary>
        ///【前缀】点赞博客视频分页
        /// </summary>
        //private readonly string Prefix_VideoByLikePagedList = "VideoLike:PagedList:";

        public BlogVideoLikeService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Tuple<bool, string>> AddBlogVideoLike(Guid vId, Guid uId)
        {
            try
            {
                /*
                 * 新增点赞
                 * 删除缓存
                 */

                var videoLike = new BlogVideoLike
                {
                    VId = vId,
                    UId = uId,
                    Created = DateTime.Now
                };
                var res = await baseService.AddAsync(videoLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //视频的点赞数量
                    var keyNameVideoLikeCount = $"{Prefix_VideoLikeCount}{vId}";
                    //视频的点赞用户
                    var keyNameVideoLike = $"{Prefix_IsVideoLike}{vId}_{uId}";

                    await cli.DelAsync(keyNameVideoLikeCount, keyNameVideoLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"VideoLike_AddBlogVideoLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DelBlogVideoLike(Guid vId, Guid uId)
        {
            try
            {
                /*
                 * 校验点赞
                 * 删除点赞
                 * 删除缓存
                 */

                var videoLike = await baseService.FirstOrDefaultAsync<BlogVideoLike>(x => x.VId == vId && x.UId == uId, true);
                if (videoLike == null)
                    return Tuple.Create(false, "点赞不存在");
                var res = await baseService.DelAsync(videoLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //视频的点赞数量
                    var keyNameVideoLikeCount = $"{Prefix_VideoLikeCount}{vId}";
                    //视频的点赞用户
                    var keyNameVideoLike = $"{Prefix_IsVideoLike}{vId}_{uId}";

                    await cli.DelAsync(keyNameVideoLikeCount, keyNameVideoLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"VideoLike_DelBlogVideoLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<long?> GetBlogVideoLikeCountBy(Guid vId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{Prefix_VideoLikeCount}{vId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var count = await cli.GetAsync(keyName);
                    return Convert.ToInt64(count);
                }
                else
                {
                    var count = await baseService.Entities<BlogVideoLike>().Where(x => x.VId == vId).CountAsync();

                    await cli.SetExAsync(keyName, 300, count);

                    return await GetBlogVideoLikeCountBy(vId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"VideoLike_GetBlogVideoLikeCountBy:{ex}");
                return Convert.ToInt64(0);
            }
        }

        public async Task<bool?> GetIsBlogVideoLikeByUser(Guid vId, Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{Prefix_IsVideoLike}{vId}_{uId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                //命中缓存
                if (res)
                {
                    var value = await cli.GetAsync(keyName);
                    return value.Int() > 0;
                }
                else
                {
                    var count = await baseService.Entities<BlogVideoLike>().Where(x => x.VId == vId && x.UId == uId).CountAsync();
                    //设置缓存
                    await cli.SetExAsync(keyName, 300, count);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"VideoLike_GetIsBlogVideoLikeByUse:{ex}");
                return false;
            }
        }
    }
}