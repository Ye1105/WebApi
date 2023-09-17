using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;

namespace Manager.Server.Services
{
    public class RankService : IRankService
    {
        private readonly IBase baseService;

        public RankService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public Task<PagedList<Blog>?> PagedAsync(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, Guid? id = null, Guid? wId = null)
        {
            throw new NotImplementedException();
        }
    }
}