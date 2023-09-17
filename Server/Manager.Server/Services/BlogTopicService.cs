using Manager.Core.Models.Blogs;
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
    }
}