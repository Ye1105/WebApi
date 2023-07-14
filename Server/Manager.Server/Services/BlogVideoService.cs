using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Serilog;
using System.Linq.Expressions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogVideoService : IBlogVideoService
    {
        private readonly IBase baseService;

        public BlogVideoService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<BlogVideo?> GetBlogVideoBy(Expression<Func<BlogVideo, bool>> expression, bool isTrack = true)
        {
            return await baseService.FirstOrDefaultAsync(expression, isTrack);
        }

        public async Task<List<BlogVideo>> GetBlogVideoListBy(Expression<Func<BlogVideo, bool>> expression, bool isTrack = true)
        {
            return await baseService.QueryAsync(expression, isTrack);
        }

        public async Task<BlogVideo?> GetBlogVideoById(Guid id)
        {
            try
            {
                /*
               * 1.缓存是否命中
               * 2.命中则直接获取缓存值
               * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
               */
                var keyName = $"{RedisConstants.PREFIX_VIDEO}{id}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var value = await cli.GetAsync(keyName);

                    return JsonHelper.DesObj<BlogVideo>(value);
                }
                else
                {
                    var video = await baseService.FirstOrDefaultAsync<BlogVideo>(x => x.BId == id && x.Status == (sbyte)Status.ENABLE, false);

                    //expire 5 minutes
                    await cli.SetExAsync(keyName, 300, video.SerObj());

                    return await GetBlogVideoById(id);
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetBlogVideoById:{0}", ex);
                return null;
            }
        }

        public async Task<PagedList<BlogVideo>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? bId = null, Guid? uId = null, string? title = "", string? channel = null, string? collection = null, string? type = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var query = baseService.Entities<BlogVideo>();

            if (uId != null && uId != Guid.Empty)
            {
                query = query.Where(x => x.UId == uId);
            }

            if (bId != null && bId != Guid.Empty)
            {
                query = query.Where(x => x.BId == bId);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title == title);
            }

            if (channel != null)
            {
                query = query.Where(x => x.Channel.Contains($"\"{channel}\""));
            }

            if (collection != null)
            {
                query = query.Where(x => x.Collection.Contains($"\"{collection}\""));
            }

            if (type != null)
            {
                query = query.Where(x => x.Type.Contains($"\"{type}\""));
            }

            if (startTime != null)
            {
                query = query.Where(x => x.Created >= startTime);
            }

            if (endTime != null)
            {
                query = query.Where(x => x.Created < endTime);
            }

            query = query.Where(x => x.Status == (int)Status.ENABLE);

            query = query.ApplySort(orderBy);

            return await PagedList<BlogVideo>.CreateAsync(query, pageIndex, pageSize, offset);
        }
    }
}