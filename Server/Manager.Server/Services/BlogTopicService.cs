using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using System.Linq.Expressions;

namespace Manager.Server.Services
{
    public class BlogTopicService : IBlogTopicService
    {
        private readonly IBase baseService;

        public BlogTopicService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<BlogTopic> FirstOrDefaultAsync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true)
        {
            return await baseService.FirstOrDefaultAsync(expression, isTrack);
        }

        public BlogTopic FirstOrDefaultSync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true)
        {
            return baseService.FirstOrDefault(expression, isTrack);
        }

        public async Task<PagedList<BlogTopic>> PagedAsync(Expression<Func<BlogTopic, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "")
        {
            return await baseService.QueryPagedAsync(whereLambda, pageIndex, pageSize, offset, isTrack, orderBy);
        }

        public async Task<List<BlogTopic>?> QueryAsync(Expression<Func<BlogTopic, bool>> expression, bool isTrack = true, string orderBy = "")
        {
            return await baseService.QueryAsync(expression, isTrack, orderBy);
        }
    }
}