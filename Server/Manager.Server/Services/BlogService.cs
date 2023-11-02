using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Models.Users;
using Manager.Core.Page;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogService : IBlogService
    {
        /*
         *【我是粉丝】我关注但未关注我的：私密的、好友圈的看不到
         */
        private List<sbyte> FanSortList { get; set; } = new List<sbyte>() { (sbyte)BlogSort.PUBLIC, (sbyte)BlogSort.FAN, (sbyte)BlogSort.HOT_PUSH, (sbyte)BlogSort.ADVERTISE };

        /*
         * 【好友圈的】：只有私密的看不到
         */
        private List<sbyte> FriendSortList { get; set; } = new List<sbyte>() { (sbyte)BlogSort.PUBLIC, (sbyte)BlogSort.FRIEND, (sbyte)BlogSort.FAN, (sbyte)BlogSort.HOT_PUSH, (sbyte)BlogSort.ADVERTISE };

        private readonly IBase baseService;
        private readonly IAccountInfoService accountInfoService;
        private readonly IBlogImageService blogImageService;
        private readonly IBlogVideoService blogVideoService;
        private readonly IBlogCommentService blogCommentService;
        private readonly IBlogForwardService blogForwardService;
        private readonly IUserFocusService userFocus;
        private readonly IProcedure procService;
        private readonly IBlogFavoriteService blogFavoriteService;
        private readonly IBlogLikeService blogLikeService;

        public BlogService(IBase baseService, IAccountInfoService accountInfoService, IBlogImageService blogImageService, IBlogVideoService blogVideoService, IBlogCommentService blogCommentService, IBlogForwardService blogForwardService, IUserFocusService userFocus, IProcedure procService, IBlogFavoriteService blogFavoriteService, IBlogLikeService blogLikeService)
        {
            this.baseService = baseService;
            this.accountInfoService = accountInfoService;
            this.blogImageService = blogImageService;
            this.blogVideoService = blogVideoService;
            this.blogCommentService = blogCommentService;
            this.blogForwardService = blogForwardService;
            this.userFocus = userFocus;
            this.procService = procService;
            this.blogFavoriteService = blogFavoriteService;
            this.blogLikeService = blogLikeService;
        }

        public async Task<bool> AddAsync(Blog blog, List<BlogTopic>? blogTopic, UserTopic? userTopic)
        {
            //blog表
            var dic = new Dictionary<object, CrudEnum>
            {
                { blog, CrudEnum.CREATE }
            };
            //blog_image表
            if (blog.Images != null && blog.Images.Any())
            {
                blog.Images.ToList().ForEach(i => dic.Add(i, CrudEnum.CREATE));
            }
            //blog_video表
            if (blog.Video != null)
            {
                dic.Add(blog.Video, CrudEnum.CREATE);
            }
            //blog_topic表
            if (blogTopic != null && blogTopic.Any())
            {
                blogTopic.ForEach(b => dic.Add(b, CrudEnum.CREATE));
            }
            //user_topic表
            if (userTopic != null)
            {
                dic.Add(userTopic, CrudEnum.CREATE);
            }
            return await baseService.BatchTransactionAsync(dic);
        }

        public bool AddSync(Blog blog, List<BlogTopic>? blogTopic, UserTopic? userTopic)
        {
            //blog表
            var dic = new Dictionary<object, CrudEnum>
            {
                { blog, CrudEnum.CREATE }
            };
            //blog_image表
            if (blog.Images != null && blog.Images.Any())
            {
                blog.Images.ToList().ForEach(i => dic.Add(i, CrudEnum.CREATE));
            }
            //blog_video表
            if (blog.Video != null)
            {
                dic.Add(blog.Video, CrudEnum.CREATE);
            }
            //blog_topic表
            if (blogTopic != null && blogTopic.Any())
            {
                blogTopic.ForEach(b => dic.Add(b, CrudEnum.CREATE));
            }
            //user_topic表
            if (userTopic != null)
            {
                dic.Add(userTopic, CrudEnum.CREATE);
            }
            return baseService.BatchTransactionSync(dic);
        }

        public async Task<int> CountAsync(Expression<Func<Blog, bool>> expression, bool isTrack = true)
        {
            return await baseService.Entities<Blog>().Where(expression).CountAsync();
        }

        public int CountSync(Expression<Func<Blog, bool>> expression, bool isTrack = true)
        {
            return baseService.Entities<Blog>().Where(expression).Count();
        }

        public async Task<PagedList<Blog>?> PagedAsync(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? wId = null, Guid? uId = null, sbyte? sort = null, sbyte? type = null, BlogForwardType? fId = null, DateTime? startTime = null, DateTime? endTime = null, int? scope = null, string? grp = null, Status? status = null)
        {
            var query = baseService.Entities<Blog>();

            if (scope != null)
            {
                IQueryable<Guid?> friendsList;

                switch (scope)
                {
                    #region [主页的博客]=>我自己+朋友圈+我是粉丝的博客

                    case (sbyte)BlogScope.HOME:

                        //我自己发布的博客
                        if (wId != null && wId != Guid.Empty)
                        {
                            query = query.Where(x => x.UId == wId);
                        }

                        //我关注的人
                        var focusList = baseService.Entities<UserFocus>()
                            .Where(x => x.UId == wId).Select(x => x.BuId);

                        if (focusList != null && focusList.Any())
                        {
                            /*
                             * 朋友圈（相互关注的人）
                             * 言下之意：我关注的用户中也有用户关注了我，这部分用户组成了朋友圈
                             */

                            friendsList = from A in baseService.Entities<UserFocus>()
                                          join B in baseService.Entities<UserFocus>()
                                             on A.BuId equals B.UId
                                          where A.UId == wId && B.BuId == wId
                                          select B.UId;

                            //如果存在朋友圈
                            if (friendsList != null && friendsList.Any())
                            {
                                //fanlist:筛选出我关注但并未关注过我的这部分用户，即：我是对方粉丝这部分用户
                                var fansList = focusList.Where(x => !friendsList.Contains(x));
                                /*
                                 * 筛选的目的：
                                 * 【好友圈的】：只有私密的看不到
                                 * 【我是粉丝】：我关注但未关注我的：私密的、好友圈的看不到
                                 */
                                var blogs = baseService.Entities<Blog>().Where(x =>
                                    ((friendsList.Contains(x.UId)) && FriendSortList.Contains(x.Sort))
                                    ||
                                    ((fansList.Contains(x.UId)) && FanSortList.Contains(x.Sort))
                                );

                                query = query.Union(blogs);
                            }
                            else
                            {
                                /*
                                 * 言下之意：我关注的人里面没有关注我的人，那我只要去查询出我关注的这部分人即可
                                 * 0.公开  1.仅自己可见 2.好友圈 3.粉丝  4.热推  5.广告
                                 * 只能看sort为【公开 粉丝 热推 和 广告】的blog
                                 */

                                var blogs = baseService.Entities<Blog>().Where(x =>
                                (focusList.Contains(x.UId)) && FanSortList.Contains(x.Sort)
                                );

                                query = query.Union(blogs);
                            }
                        }

                        //热推和广告的博客
                        query = query.Union(baseService.Entities<Blog>().Where(x => x.Sort == (sbyte)BlogSort.HOT_PUSH || x.Sort == (sbyte)BlogSort.ADVERTISE));

                        break;

                    #endregion [主页的博客]=>我自己+朋友圈+我是粉丝的博客

                    #region [朋友圈的博客]=>只查询朋友圈的博客

                    case (sbyte)BlogScope.FRIEND:

                        /*
                         * 好友圈（相互关注的人）
                         * 言下之意：我关注的用户中也有用户关注了我，这部分用户组成了朋友圈
                         */
                        friendsList = from A in baseService.Entities<UserFocus>()
                                      join B in baseService.Entities<UserFocus>()
                                         on A.BuId equals B.UId
                                      where A.UId == wId && B.BuId == wId
                                      select B.UId;

                        //如果存在朋友圈
                        if (friendsList != null && friendsList.Any())
                        {
                            /*
                             * 【好友圈的】：只有私密的看不到
                             */
                            query = query.Where(x => friendsList.Contains(x.UId) && x.Sort != (int)BlogSort.PRIVATE);
                        }
                        else
                        {
                            return null;
                        }

                        break;

                    #endregion [朋友圈的博客]=>只查询朋友圈的博客

                    #region [特别关注的博客]=>只查询特别关注的博客

                    case (sbyte)BlogScope.FOCUS:

                        //我特别关注的人
                        focusList = baseService.Entities<UserFocus>()
                            .Where("UId=@0 and Relation=@1", wId, (int)FocusRelation.SPECIAL_FOCUS)
                            .Select(x => x.BuId);

                        if (focusList != null && focusList.Any())
                        {
                            //判定这部分特别关注的人中哪些是【朋友圈】，哪些是【我是粉丝】
                            friendsList = from A in baseService.Entities<UserFocus>()
                                          join B in baseService.Entities<UserFocus>()
                                             on A.BuId equals B.UId
                                          where A.UId == wId && A.Relation == (int)FocusRelation.SPECIAL_FOCUS && B.BuId == wId
                                          select B.UId;

                            if (friendsList != null && friendsList.Any())
                            {
                                /*
                                 * 特别关注的用户中，存在有朋友圈的这种情况
                                 * fanlist:筛选出我关注但并未关注过我的这部分用户，即：我是对方粉丝这部分用户
                                 */
                                var fansList = focusList.Where(x => !friendsList.Contains(x));

                                if (fansList != null && fansList.Any())
                                {
                                    /*
                                     * 我关注的人= 朋友圈的人 【互关】 + 我是粉丝 【我关注但未关注我】
                                     */
                                    query = query.Where(x =>
                                      (friendsList.Contains(x.UId) && FriendSortList.Contains(x.Sort))
                                      ||
                                      (fansList.Contains(x.UId) && FanSortList.Contains(x.Sort))
                                      );
                                }
                                else
                                {
                                    /*
                                     * 我关注的人= 朋友圈的人 【互关】【不包含粉丝】
                                    */
                                    query = query.Where(x => friendsList.Contains(x.UId) && FriendSortList.Contains(x.Sort));
                                }
                            }
                            else
                            {
                                /*
                                 * 【特别关注的用户中，没有关注我的】这种情况【注：即不存在朋友圈】
                                 */
                                query = query.Where(x => focusList.Contains(x.UId) && FanSortList.Contains(x.Sort));
                            }
                        }
                        else
                            return null;
                        break;

                    #endregion [特别关注的博客]=>只查询特别关注的博客

                    #region [自定义分组的博客]=>只查询自定义分组的博客

                    case (sbyte)BlogScope.GROUP:
                        if (!string.IsNullOrWhiteSpace(grp))
                        {
                            MySqlParameter[] myGroupParameter =
                            {
                                 new MySqlParameter("@grp", grp),
                                 new MySqlParameter("@uId", wId)
                            };

                            /*
                             * 在自定义分组中我关注的用户
                             */
                            var groupList = baseService.ExecuteQuery<UserFocus>(
                                "Select * from user_focus where JSON_CONTAINS(Grp,@grp) and UId=@uId",
                                false,
                                myGroupParameter
                            ).Select(x => x.BuId);

                            if (groupList != null && groupList.Count() > 0)
                            {
                                /*
                                 * 满足分组需求且是互相关注的博客【朋友圈博客】
                                 */
                                friendsList = baseService.ExecuteQuery<UserFocus>(
                                    "Select A.* from user_focus A inner join user_focus B  on A.BuId=B.UId where A.UId=@uId && JSON_CONTAINS(A.Grp,@grp) && B.BuId=@uId",
                                    false,
                                    myGroupParameter
                                ).Select(x => x.BuId);

                                if (friendsList != null && friendsList.Count() > 0)
                                {
                                    /*
                                     * 求差集
                                     * 特别关注的用户中，存在有朋友圈的这种情况
                                     * fanlist:筛选出我关注但并未关注过我的这部分用户，即：我是对方粉丝这部分用户
                                     */

                                    var fansList = groupList.Where(x => !friendsList.Contains(x));

                                    if (fansList != null && fansList.Any())
                                    {
                                        query = query.Where(x =>
                                           (friendsList.Contains(x.UId) && FriendSortList.Contains(x.Sort))
                                           ||
                                           (fansList.Contains(x.UId) && FanSortList.Contains(x.Sort))
                                        );
                                    }
                                    else
                                    {
                                        query = query.Where(x => friendsList.Contains(x.UId) && FriendSortList.Contains(x.Sort));
                                    }
                                }
                                else
                                {
                                    /*
                                     * 自定义分组中的用户中，没有关注我的这种情况【注：即不存在朋友圈】
                                     * 0.公开  1.仅自己可见 2.好友圈 3.粉丝  4.热推  5.广告
                                     */
                                    query = query.Where(x => groupList.Contains(x.UId) && FanSortList.Contains(x.Sort));
                                }
                            }
                            else
                                return null;
                        }
                        else
                            return null;

                        break;

                    #endregion [自定义分组的博客]=>只查询自定义分组的博客

                    default:
                        return null;
                }
            }
            //id
            if (id != null && id != Guid.Empty)
            {
                query = query.Where(x => x.Id == id);
            }
            // 用户
            if (uId != null && uId != Guid.Empty)
            {
                query = query.Where(x => x.UId == uId);
            }
            // 种类： 0.公开  1.仅自己可见  2.好友圈  3.粉丝  4.热推  5.广告
            if (sort != null)
            {
                query = query.Where(x => x.Sort == sort);
            }
            // 类型  0.全部 1.普通文字(表情) 2.头条文章 3.图片 4.音乐 5.视频
            if (type != null && type != (sbyte)BlogType.ALL)
            {
                query = query.Where(x => x.Type == type);
            }
            // 是否原创
            if (fId != null && fId != 0)
            {
                var res = fId!.Value;
                query = fId!.Value == BlogForwardType.ORIGION ? query.Where(x => x.FId == Guid.Empty) : query.Where(x => x.FId != Guid.Empty);
            }
            // 开始时间
            if (startTime != null)
            {
                query = query.Where(x => x.Created >= startTime);
            }
            // 结束时间
            if (endTime != null)
            {
                query = query.Where(x => x.Created < endTime);
            }

            //状态
            if (status != null)
            {
                query = query.Where(x => x.Status == (int)status);
            }

            //排序
            query = query.ApplySort(orderBy);

            //整合分页数据
            return await PagedList<Blog>.CreateAsync(query, pageIndex, pageSize, offset);
        }

        public async Task GetBlogRelation(Blog blog, Guid? wId)
        {
            /*
             * 1.通过 Type 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
             * 判定是否需要关联 images or video 【已通过 Include 关联查询，此查询步骤废弃】
             * 2.获取 accountInfo 表
             * 3.获取博客评论数量
             * 4.获取博客转发数量
             * 5.【废弃】获取博客收藏数量（暂不需要）
             * 6.获取博客点赞数量
             * 7.获取博客收藏数量
             * 8.获取当前登录用户是否点赞博客
             * 9.获取当前登录用户是否收藏博客
             * 10.用户关注表
             * 11.判断是否转发blog
             */

            if (blog == null) return;

            //1.1 关联 images
            if (blog.Type == (int)BlogType.IMAGE)
            {
                blog.Images = await blogImageService.QueryAsync(blog.Id);
            }

            //1.2 关联 video 表
            if (blog.Type == (int)BlogType.VIDEO)
            {
                blog.Video = await blogVideoService.FirstOrDefaultAsync(blog.Id);
            }

            //2.获取 accountInfo 表
            blog.AccountInfo = await accountInfoService.FirstOrDefaultAsync(blog.UId, isCache: false);

            //3.获取博客评论数
            blog.Comment = await blogCommentService.CountAsync(x => x.BId == blog.Id && x.Status == (sbyte)Status.ENABLE);

            //4.获取博客转发数
            //判定当前blog是原创blog
            blog.Forward = blog.FId == Guid.Empty ? await blogForwardService.CountAsync(x => x.BaseBId == blog.Id && x.Status == (sbyte)Status.ENABLE) : await blogForwardService.CountAsync(x => x.PrevBId == blog.Id && x.Status == (sbyte)Status.ENABLE);

            //6.获取博客点赞数量
            blog.Like = await blogLikeService.CountAsync(blog.Id);

            //7.获取博客收藏数量
            blog.Favorite = await blogFavoriteService.CountAsync(blog.Id);

            if (wId != null && wId != Guid.Empty)
            {
                //8.获取当前登录用户是否点赞博客
                blog.IsLike = await blogLikeService.ExsitAsync(blog.Id, wId.Value);

                //9.获取当前登录用户是否收藏博客
                blog.IsFavorite = await blogFavoriteService.ExsitAsync(blog.Id, wId.Value);

                //10.关系
                blog.UserFocus = await userFocus.FirstOrDefaultAsync(x => x.BuId == blog.UId && x.UId == wId);
            }
            //通过  blog.FId == Guid.Empty 判断
            if (blog.FId != null && blog.FId != Guid.Empty)
            {
                /*
                 * 11.获取关联的 blog
                 */
                blog.FBlog = (await PagedAsync(pageIndex: 1, pageSize: 1, offset: 0, isTrack: false, orderBy: "", blog.FId, wId, null, null, null, null, null, null, null, null, Status.ENABLE))?.FirstOrDefault();
                if (blog.FBlog != null) await GetBlogRelation(blog.FBlog, wId);
            }
        }

        public async Task<IEnumerable<dynamic>> GetBlogCountGroupbyMonth(Guid uId, int year)
        {
            //【存储过程】获取当前用户当前年每月的博客数
            //-- call keysye.SelBlogCountGroubyMonth(2022, '0f8fad5b-d9cb-469f-a165-70867728950e');
            // return await procService.ExecSpAsync(Settings.Proc_SelBlogCountGroubyMonth, mySqlParameter);

            MySqlParameter[] mySqlParameter =
            {
                new MySqlParameter("@uId",uId),
                new MySqlParameter("@year",year)
            };

            var sql = $" SELECT year(Created) Year,month(Created) Month,count(*) Count FROM blog where UId =@uId and Status = {(sbyte)Status.ENABLE} and year(Created)= @year group by month(Created)";

            var res = await procService.ExecSqlAsync(sql, mySqlParameter);
            return res;
        }

        public class daaaa
        {
            public string? Year { get; set; }

            public string? Month { get; set; }

            public string? Count { get; set; }
        }

        public async Task<Blog?> FirstOrDefaultAsync(Expression<Func<Blog, bool>> expression, bool isTrack = true)
        {
            return await baseService.FirstOrDefaultAsync(expression, isTrack);
        }

        public Blog? FirstOrDefaultSync(Expression<Func<Blog, bool>> expression, bool isTrack = true)
        {
            return baseService.FirstOrDefault(expression, isTrack);
        }

        public async Task<bool> UpdateAsync(Blog blog)
        {
            return await baseService.UpdateAsync(blog) > 0;
        }

        public async Task<Tuple<bool, string>> DeleteAsync(Blog blog)
        {
            try
            {
                /* 1. 判定当前blog是否时转发blog
                 * 1.1如果是原创blog
                 * 1.1.0 原创blog type 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
                 * 1.1.1 事务删除【博客、视频】
                 * 1.1.2 事务删除【博客、图片】
                 * 1.1.3 只删除博客
                 * 1.2  事务删除转发
                 */

                blog.Status = (sbyte)Status.DISABLE;
                var dic = new Dictionary<object, CrudEnum>
                {
                    { blog, CrudEnum.UPDATE }
                };
                /*
                 * 1. 判定当前blog是否时转发blog
                 * 1.1如果是原创blog
                 */
                if (blog.FId == Guid.Empty)
                {
                    // 2,原创blog type 类型  -1.全部  0.图片  1.视频   2.头条文章  3.音乐  4.普通文字(表情)
                    // 2.1 事务删除【博客、视频】
                    if (blog.Type == (sbyte)BlogType.VIDEO)
                    {
                        var blogVideo = await blogVideoService.FirstOrDefaultAsync(x => x.BId == blog.Id && x.Status == (int)Status.ENABLE);
                        if (blogVideo == null)
                        {
                            return Tuple.Create(false, "博客关联的视频不存在");
                        }
                        else
                        {
                            blogVideo.Status = (int)Status.DISABLE;
                            dic.Add(blogVideo, CrudEnum.UPDATE);
                            var res = await baseService.BatchTransactionAsync(dic);

                            //删除blog对应blogVideo的缓存

                            if (res)
                            {
                                using var cli = Instance(RedisBaseEnum.Zeroth);

                                var keyVideo = $"{RedisConstants.PREFIX_VIDEO}{blog.Id}";

                                await cli.DelAsync(keyVideo);
                            }

                            return res ? Tuple.Create(true, "") : Tuple.Create(false, "删除博客失败");
                        }
                    }
                    // 2.2 事务删除【博客、图片】
                    else if (blog.Type == (sbyte)BlogType.IMAGE)
                    {
                        var blogImages = await blogImageService.QueryAsync(x => x.BId == blog.Id && x.Status == (int)Status.ENABLE);
                        if (blogImages != null && blogImages.Any())
                        {
                            foreach (var item in blogImages)
                            {
                                item.Status = (sbyte)Status.DISABLE;
                                dic.Add(item, CrudEnum.UPDATE);
                            }
                            var res = await baseService.BatchTransactionAsync(dic);

                            //删除blog对应blogImages的缓存

                            if (res)
                            {
                                using var cli = Instance(RedisBaseEnum.Zeroth);

                                var keyImages = $"{RedisConstants.PREFIE_IMAGE}{blog.Id}";

                                await cli.DelAsync(keyImages);
                            }

                            return res ? Tuple.Create(true, "") : Tuple.Create(false, "删除博客失败");
                        }
                        else
                        {
                            return Tuple.Create(false, "博客关联的图片不存在");
                        }
                    }
                    // 2.3 只删除博客
                    else
                    {
                        var res = await baseService.UpdateAsync(blog) > 0;
                        return res ? Tuple.Create(true, "") : Tuple.Create(false, "删除博客失败");
                    }
                }
                // 1.2  事务删除转发
                else
                {
                    var blogForward = await baseService.FirstOrDefaultAsync<BlogForward>(x => x.Id == blog.Id && x.Status == (int)Status.ENABLE, true);
                    if (blogForward == null)
                    {
                        return Tuple.Create(false, "关联的转发博客不存在");
                    }
                    else
                    {
                        blogForward.Status = (sbyte)Status.DISABLE;
                        dic.Add(blogForward, CrudEnum.UPDATE);
                        var res = await baseService.BatchTransactionAsync(dic);
                        return res ? Tuple.Create(true, "") : Tuple.Create(false, "删除博客失败");
                    }
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.ToString().Str());
            }
        }

        public async Task<bool> AddBlogCommentAndForward(Blog blog, BlogComment blogComment, BlogForward blogForward)
        {
            var dic = new Dictionary<object, CrudEnum>
            {
                { blog, CrudEnum.CREATE},
                { blogComment, CrudEnum.CREATE},
                { blogForward, CrudEnum.CREATE}
            };
            return await baseService.BatchTransactionAsync(dic);
        }
    }
}