using FreeRedis;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;
using System.Linq.Dynamic.Core;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogLikeService : IBlogLikeService
    {
        private readonly IBase baseService;
        private readonly IProcedure procService;

        public BlogLikeService(IBase baseService, IProcedure procService)
        {
            this.baseService = baseService;
            this.procService = procService;
        }

        public async Task<Tuple<bool, string>> AddAsync(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 新增点赞
                 * 删除缓存
                 */

                var blogLike = new BlogLike
                {
                    BId = bId,
                    UId = uId,
                    Created = DateTime.Now
                };
                var res = await baseService.AddAsync(blogLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //博客的点赞数量
                    var keyNameBlogLikeCount = $"{RedisConstants.PREFIX_BLOG_LIKE_COUNT}{bId}";
                    //博客的点赞用户
                    var keyNameBlogLike = $"{RedisConstants.PREFIX_BLOG_ISLIKE}{bId}:{uId}";
                    //博客[点赞]的分页列表
                    var keyNameBlogLikePagedList = $"{RedisConstants.PREFIX_BLOG_LIKE_PAGED}{uId}";

                    await cli.DelAsync(keyNameBlogLikeCount, keyNameBlogLike, keyNameBlogLikePagedList);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"BlogLike_AddBlogLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DeleteAsync(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 校验点赞
                 * 删除点赞
                 * 删除缓存
                 */

                var blogLike = await baseService.FirstOrDefaultAsync<BlogLike>(x => x.BId == bId && x.UId == uId, true);
                if (blogLike == null)
                    return Tuple.Create(false, "点赞不存在");
                var res = await baseService.DelAsync(blogLike) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //博客的点赞数量
                    var keyNameBlogLikeCount = $"{RedisConstants.PREFIX_BLOG_LIKE_COUNT}{bId}";
                    //博客的点赞用户
                    var keyNameBlogLike = $"{RedisConstants.PREFIX_BLOG_ISLIKE}{bId}:{uId}";
                    //博客[点赞]的分页列表
                    var keyNameBlogLikePagedList = $"{RedisConstants.PREFIX_BLOG_LIKE_PAGED}{uId}";

                    await cli.DelAsync(keyNameBlogLikeCount, keyNameBlogLike, keyNameBlogLikePagedList);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error($"BlogLike_DelBlogLike:{ex}");
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<long?> CountAsync(Guid bId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_BLOG_LIKE_COUNT}{bId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var count = await cli.GetAsync(keyName);
                    return Convert.ToInt64(count);
                }
                else
                {
                    var count = await baseService.Entities<BlogLike>().Where(x => x.BId == bId).CountAsync();

                    await cli.SetExAsync(keyName, 300, count);

                    return await CountAsync(bId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"BlogLike_GetBlogLikeCountBy:{ex}");
                return Convert.ToInt64(0);
            }
        }

        public async Task<bool?> ExsitAsync(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{RedisConstants.PREFIX_BLOG_ISLIKE}{bId}:{uId}";

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
                    var count = await baseService.Entities<BlogLike>().Where(x => x.BId == bId && x.UId == uId).CountAsync();
                    //设置缓存
                    await cli.SetExAsync(keyName, 300, count);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"BlogLike_GetIsBlogLikeByUser:{ex}");
                return false;
            }
        }

        public async Task<PagedList<Blog?>?> PagedAsync(Guid wId, int pageIndex = 1, int pageSize = 10, int offset = 0)
        {
            try
            {
                /*
                 * 1.是否命中缓存
                 * 2.命中缓存
                 * 2.1 获取键的类型 【string 类型代表没有数据，zset 类型代表点赞列表有数据】
                 * 2.2 zset ZRevRangeAsync 分页
                 * 3.未命中缓存
                 * 3.1 没有数据 设置 string key
                 * 3.2 有数据  设置 zset key
                 * 4. 递归循环一次当前方法
                 */

                var keyName = $"{RedisConstants.PREFIX_BLOG_LIKE_PAGED}{wId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                //1.是否命中缓存
                var res = await cli.ExistsAsync(keyName);

                //2.1命中缓存
                if (res)
                {
                    //2.2 获取键的类型
                    //2.3 string 类型代表没有数据，zset 类型戴白点赞列表有数据
                    var type = cli.Type(keyName);
                    switch (type)
                    {
                        case KeyType.@string:

                            return null;

                        case KeyType.zset:

                            var likeCount = await cli.ZCardAsync(keyName);

                            var members = await cli.ZRevRangeAsync(keyName, (pageIndex - 1) * pageSize + offset, pageIndex * pageSize - 1 + offset);

                            var blogs = new List<Blog?>();

                            foreach (var item in members)
                            {
                                var blog = item.DesObj<Blog>();
                                blogs.Add(blog);
                            }

                            return PagedList<Blog?>.Create(blogs, likeCount, pageIndex, pageSize, offset);

                        default:
                            break;
                    }
                }
                else
                {
                    //var data = await baseService.Entities<BlogLike>().Where(x => x.UId == wId).AsNoTracking().ToListAsync();

                    var sql = "SELECT A.*,B.Created LikeCreated FROM blog A inner join blog_like B on A.Id=B.BId where B.UId=@uId";

                    var data = await procService.ExecSqlAsync(sql, new MySqlParameter[] { new MySqlParameter("@uId", wId) });

                    if (data != null && data.Any())
                    {
                        var pipe = cli.StartPipe();

                        foreach (var item in data)
                        {
                            var blog = new Blog()
                            {
                                Id = Guid.Parse(item.Id),
                                UId = Guid.Parse(item.UId),
                                Sort = Convert.ToSByte(item.Sort),
                                Type = Convert.ToSByte(item.Type),
                                Body = item.Body,
                                FId = Guid.Parse(item.FId),
                                Created = Convert.ToDateTime(item.Created),
                                Top = Convert.ToSByte(item.Top),
                                Status = Convert.ToSByte(item.Status),
                            };

                            pipe.ZAdd(keyName, DateHelper.ConvertDateTimeToLong(item.LikeCreated), blog.SerObj());
                        }

                        pipe.Expire(keyName, 300);

                        object[] ret = pipe.EndPipe();

                        return await PagedAsync(wId, pageIndex, pageSize, offset);
                    }
                    else
                    {
                        await cli.SetExAsync(keyName, 300, "");
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"BlogLike_GetPagedList:{ex}");
                return null;
            }
        }
    }
}