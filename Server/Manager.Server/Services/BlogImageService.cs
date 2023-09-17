using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using System.Linq.Expressions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class BlogImageService : IBlogImageService
    {
        private readonly IBase baseService;

        public BlogImageService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<List<BlogImage>> QueryAsync(Expression<Func<BlogImage, bool>> expression, bool isTrack = true)
        {
            return await baseService.QueryAsync(expression, isTrack);
        }

        public async Task<List<BlogImage>?> QueryAsync(Guid id)
        {
            /*
             * 1.缓存是否命中
             * 2.命中则直接获取缓存值
             * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
             */
            var keyName = $"{RedisConstants.PREFIE_IMAGE}{id}";

            using var cli = Instance(RedisBaseEnum.Zeroth);

            var res = await cli.ExistsAsync(keyName);

            if (res)
            {
                var value = await cli.GetAsync(keyName);

                return JsonHelper.DesObj<List<BlogImage>>(value);
            }
            else
            {
                var imageList = await baseService.QueryAsync<BlogImage>(x => x.BId == id && x.Status == (sbyte)Status.ENABLE, false);

                //expire 5 minutes
                await cli.SetExAsync(keyName, 300, imageList.SerObj());

                return await QueryAsync(id);
            }
        }

        public async Task<PagedList<BlogImage>?> PagedAsync(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? bId = null, Guid? uId = null, DateTime? startTime = null, DateTime? endTime = null, Status status = Status.ENABLE)
        {
            var query = baseService.Entities<BlogImage>();

            if (uId != null && uId != Guid.Empty)
            {
                query = query.Where(x => x.UId == uId);
            }

            if (bId != null && bId != Guid.Empty)
            {
                query = query.Where(x => x.BId == bId);
            }

            if (startTime != null)
            {
                query = query.Where(x => x.Created >= startTime);
            }

            if (endTime != null)
            {
                query = query.Where(x => x.Created < endTime);
            }


            query = query.Where(x => x.Status == (sbyte)status);

            query = query.ApplySort(orderBy);

            return await PagedList<BlogImage>.CreateAsync(query, pageIndex, pageSize, offset);
        }

        public async Task<bool> UpdateAsync(BlogImage blogImage)
        {
            return await baseService.UpdateAsync(blogImage) > 0;
        }
    }
}