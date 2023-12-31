﻿using Manager.Core.Models.Blogs;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogImageLikeService : IBlogImageLikeService
    {
        private readonly IBase baseService;

        public BlogImageLikeService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Tuple<bool, string>> AddAsync(Guid iId, Guid uId)
        {
            try
            {
                /*
                 * 新增点赞
                 * 删除缓存
                 */

                var imageLike = new BlogImageLike
                {
                    IId = iId,
                    UId = uId,
                    Created = DateTime.Now
                };
                var res = await baseService.AddAsync(imageLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //图片的点赞数量
                    var keyNameImageLikeCount = $"{RedisConstants.PREFIX_IMAGE_LIKE_COUNT}{iId}";
                    //图片的点赞用户
                    var keyNameImageLike = $"{RedisConstants.PREFIX_IMAGE_ISLIKE}{iId}:{uId}";

                    await cli.DelAsync(keyNameImageLikeCount, keyNameImageLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"ImageLike_AddBlogImageLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DeleteAsync(Guid iId, Guid uId)
        {
            try
            {
                /*
                 * 校验点赞
                 * 删除点赞
                 * 删除缓存
                 */

                var imageLike = await baseService.FirstOrDefaultAsync<BlogImageLike>(x => x.IId == iId && x.UId == uId, true);
                if (imageLike == null)
                    return Tuple.Create(false, "点赞不存在");
                var res = await baseService.DelAsync(imageLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //图片的点赞数量
                    var keyNameImageLikeCount = $"{RedisConstants.PREFIX_IMAGE_LIKE_COUNT}{iId}";
                    //图片的点赞用户
                    var keyNameImageLike = $"{RedisConstants.PREFIX_IMAGE_ISLIKE}{iId}:{uId}";

                    await cli.DelAsync(keyNameImageLikeCount, keyNameImageLike);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"ImageLike_DelBlogImageLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<long?> CountAsync(Guid iId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_IMAGE_LIKE_COUNT}{iId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var count = await cli.GetAsync(keyName);
                    return Convert.ToInt64(count);
                }
                else
                {
                    var count = await baseService.Entities<BlogImageLike>().Where(x => x.IId == iId).CountAsync();

                    await cli.SetExAsync(keyName, 300, count);

                    return await CountAsync(iId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ImageLike_GetBlogImageLikeCountBy:{ex}");
                return Convert.ToInt64(0);
            }
        }

        public async Task<bool?> ExsitAsync(Guid iId, Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_IMAGE_ISLIKE}{iId}:{uId}";

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
                    var count = await baseService.Entities<BlogImageLike>().Where(x => x.IId == iId && x.UId == uId).CountAsync();
                    //设置缓存
                    await cli.SetExAsync(keyName, 300, count);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ImageLike_GetIsBlogImageLikeByUser:{ex}");
                return false;
            }
        }
    }
}