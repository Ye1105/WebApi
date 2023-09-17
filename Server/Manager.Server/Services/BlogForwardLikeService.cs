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
    public class BlogForwardLikeService : IBlogForwardLikeService
    {
        private readonly IBase baseService;

        public BlogForwardLikeService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Tuple<bool, string>> AddAsync(Guid fId, Guid uId)
        {
            try
            {
                /*
                 * 新增点赞
                 * 删除缓存
                 */

                var forwardLike = new BlogForwardLike
                {
                    FId = fId,
                    UId = uId,
                    Created = DateTime.Now
                };
                var res = await baseService.AddAsync(forwardLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //转发的点赞数量
                    var keyNameForwardLikeCount = $"{RedisConstants.PREFIX_FORWARD_LIKE_COUNT}{fId}";
                    //转发的点赞用户
                    var keyNameForwardLike = $"{RedisConstants.PREFIX_FORWARD_ISLIKE}{fId}:{uId}";

                    await cli.DelAsync(keyNameForwardLikeCount, keyNameForwardLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"ForwardLike_AddBlogForwadLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DeleteAsync(Guid fId, Guid uId)
        {
            try
            {
                /*
                 * 校验点赞
                 * 删除点赞
                 * 删除缓存
                 */

                var forwardLike = await baseService.FirstOrDefaultAsync<BlogForwardLike>(x => x.FId == fId && x.UId == uId, true);
                if (forwardLike == null)
                    return Tuple.Create(false, "点赞不存在");
                var res = await baseService.DelAsync(forwardLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //转发的点赞数量
                    var keyNameForwardLikeCount = $"{RedisConstants.PREFIX_FORWARD_LIKE_COUNT}{fId}";
                    //转发的点赞用户
                    var keyNameForwardLike = $"{RedisConstants.PREFIX_FORWARD_ISLIKE}{fId}:{uId}";

                    await cli.DelAsync(keyNameForwardLikeCount, keyNameForwardLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"ForwardLike_DelBlogForwardLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<long?> CountAsync(Guid fId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_FORWARD_LIKE_COUNT}{fId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var count = await cli.GetAsync(keyName);
                    return Convert.ToInt64(count);
                }
                else
                {
                    var count = await baseService.Entities<BlogForwardLike>().Where(x => x.FId == fId).CountAsync();

                    await cli.SetExAsync(keyName, 300, count);

                    return await CountAsync(fId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ForwardLike_GetBlogForwardLikeCountBy:{ex}");
                return Convert.ToInt64(0);
            }
        }

        public async Task<bool?> ExsitAsync(Guid fId, Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_FORWARD_ISLIKE}{fId}:{uId}";

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
                    var count = await baseService.Entities<BlogForwardLike>().Where(x => x.FId == fId && x.UId == uId).CountAsync();
                    //设置缓存
                    await cli.SetExAsync(keyName, 300, count);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ForwardLike_GetIsBlogForwardLikeByUser:{ex}");
                return false;
            }
        }
    }
}