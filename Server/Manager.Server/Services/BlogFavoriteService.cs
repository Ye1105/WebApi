using FreeRedis;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Serilog;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogFavoriteService : IBlogFavoriteService
    {
        private readonly IBase baseService;
        private readonly IProcedure procService;

        /// <summary>
        ///【前缀】博客收藏数量
        /// </summary>
        private readonly string Prefix_BlogFavoriteCount = "BlogFavorite:Count:";

        /// <summary>
        ///【前缀】博客用户是否收藏
        /// </summary>
        private readonly string Prefix_IsBlogFavorite = "BlogFavorite:IsFavorite:";

        /// <summary>
        ///【前缀】收藏博客分页
        /// </summary>
        private readonly string Prefix_BlogByFavoritePagedList = "BlogFavorite:PagedList:";

        public BlogFavoriteService(IBase baseService, IProcedure procService)
        {
            this.baseService = baseService;
            this.procService = procService;
        }

        public async Task<Tuple<bool, string>> AddBlogFavorite(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 新增收藏
                 * 删除缓存
                 */

                var blogFavorite = new BlogFavorite
                {
                    BId = bId,
                    UId = uId,
                    Created = DateTime.Now
                };
                var res = await baseService.AddAsync(blogFavorite) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //博客的收藏数量
                    var keyNameBlogFavoriteCount = $"{Prefix_BlogFavoriteCount}{bId}";
                    //博客的收藏用户
                    var keyNameBlogFavorite = $"{Prefix_IsBlogFavorite}{bId}_{uId}";
                    //博客[收藏]的分页列表
                    var keyNameBlogFavoritePagedList = $"{Prefix_BlogByFavoritePagedList}{uId}";

                    await cli.DelAsync(keyNameBlogFavoriteCount, keyNameBlogFavorite, keyNameBlogFavoritePagedList);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error("BlogFavorite_AddBlogFavorite {0}", ex.ToString());
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> DelBlogFavorite(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 校验收藏
                 * 删除收藏
                 * 删除缓存
                 */

                var blogFavorite = await baseService.FirstOrDefaultAsync<BlogFavorite>(x => x.BId == bId && x.UId == uId, true);
                if (blogFavorite == null)
                    return Tuple.Create(false, "收藏不存在");
                var res = await baseService.DelAsync(blogFavorite) > 0;

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);
                    //博客的收藏数量
                    var keyNameBlogFavoriteCount = $"{Prefix_BlogFavoriteCount}{bId}";
                    //博客的收藏用户
                    var keyNameBlogFavorite = $"{Prefix_IsBlogFavorite}{bId}_{uId}";
                    //博客[收藏]的分页列表
                    var keyNameBlogFavoritePagedList = $"{Prefix_BlogByFavoritePagedList}{uId}";

                    await cli.DelAsync(keyNameBlogFavoriteCount, keyNameBlogFavorite, keyNameBlogFavoritePagedList);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                Log.Error("BlogFavorite_DelBlogFavorite {0}", ex.ToString());
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<long?> GetBlogFavoriteCountBy(Guid bId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{Prefix_BlogFavoriteCount}{bId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var count = await cli.GetAsync(keyName);
                    return Convert.ToInt64(count);
                }
                else
                {
                    var count = await baseService.Entities<BlogFavorite>().Where(x => x.BId == bId).CountAsync();

                    await cli.SetExAsync(keyName, 300, count);

                    return await GetBlogFavoriteCountBy(bId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"BlogFavorite_GetBlogFavoriteCountBy:{ex}");
                return Convert.ToInt64(0);
            }
        }

        public async Task<bool?> GetIsBlogFavoriteByUser(Guid bId, Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 */

                var keyName = $"{Prefix_IsBlogFavorite}{bId}_{uId}";

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
                    var count = await baseService.Entities<BlogFavorite>().Where(x => x.BId == bId && x.UId == uId).CountAsync();
                    //设置缓存
                    await cli.SetExAsync(keyName, 300, count);

                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"BlogFavorite_GetIsBlogFavoriteByUser:{ex}");
                return false;
            }
        }

        public async Task<PagedList<Blog?>?> GetPagedList(Guid wId, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true)
        {
            try
            {
                /*
                 * 1.是否命中缓存
                 * 2.命中缓存
                 * 2.1 获取键的类型 【string 类型代表没有数据，zset 类型戴白点赞列表有数据】
                 * 2.2 zset ZRevRangeAsync 分页
                 * 3.未命中缓存
                 * 3.1 没有数据 设置 string key
                 * 3.2 有数据  设置 zset key
                 * 4. 递归循环一次当前方法
                 */

                var keyName = $"{Prefix_BlogByFavoritePagedList}{wId}";

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

                            var favoriteCount = await cli.ZCardAsync(keyName);

                            var members = await cli.ZRevRangeAsync(keyName, (pageIndex - 1) * pageSize + offset, pageIndex * pageSize - 1 + offset);

                            var blogs = new List<Blog?>();

                            foreach (var item in members)
                            {
                                var blog = item.DesObj<Blog>();
                                blogs.Add(blog);
                            }

                            return PagedList<Blog?>.Create(blogs, favoriteCount, pageIndex, pageSize, offset);

                        default:
                            break;
                    }
                }
                else
                {
                    var sql = "SELECT A.*,B.Created FavoriteCreated FROM blog A inner join blog_favorite B on A.Id=B.BId where B.UId=@uId";

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
                                Top = Convert.ToSByte(item.Top),
                                Created = Convert.ToDateTime(item.Created),
                                Status = Convert.ToSByte(item.Status),
                            };

                            pipe.ZAdd(keyName, DateHelper.ConvertDateTimeToLong(item.FavoriteCreated), blog.SerObj());
                        }

                        pipe.Expire(keyName, 300);

                        object[] ret = pipe.EndPipe();

                        return await GetPagedList(wId, pageIndex, pageSize, offset);
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
                Log.Error($"BlogFavorite_GetPagedList:{ex}");
                return null;
            }
        }
    }
}