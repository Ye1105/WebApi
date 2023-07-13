using Manager.Core.Enums;
using Manager.Core.Models.Users;
using Manager.Core.Page;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Manager.Server.Services
{
    public class UserFocusService : IUserFocusService
    {
        private readonly IBase baseService;

        public UserFocusService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> AddUserFocus(UserFocus userFocus)
        {
            return await baseService.AddAsync(userFocus) > 0;
        }

        public async Task<UserFocus?> GetUserFocusBy(Expression<Func<UserFocus, bool>> expression, bool isTrack = true)
        {
            return await baseService.FirstOrDefaultAsync(expression, isTrack);
        }

        public async Task<PagedList<UserFocus>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", RelationType relationType = RelationType.FOCUS, Guid? uId = null, string? grp = null, sbyte? relation = null, sbyte? channel = null, DateTime? startTime = null, DateTime? endTime = null)
        {
            var query = baseService.Entities<UserFocus>();

            query = relationType == RelationType.FOCUS ? query.Where(x => x.UId == uId) : query.Where(x => x.BuId == uId);

            if (grp != null)
            {
                query = query.Where(x => x.Grp.Contains($"\"{grp}\""));
            }

            if (relation != null)
            {
                query = query.Where(x => x.Relation == relation);
            }

            if (channel != null)
            {
                query = query.Where(x => x.Channel == channel);
            }

            query = query.ApplySort(orderBy);

            return await PagedList<UserFocus>.CreateAsync(query, pageIndex, pageSize, offset);
        }

        public async Task<bool> ModifyUserFocus(UserFocus userFocus)
        {
            return await baseService.UpdateAsync(userFocus) > 0;
        }

        public async Task<int> GetUserFocusCountBy(Expression<Func<UserFocus, bool>> expression)
        {
            return await baseService.Entities<UserFocus>().Where(expression).CountAsync();
        }

        public async Task<bool> DelUserFocus(UserFocus userFocus)
        {
            return await baseService.DelAsync(userFocus) > 0;
        }

        public async Task<int> BatchDeleteUserFocus(Expression<Func<UserFocus, bool>> expression)
        {
            return await baseService.DelAsync(expression);
        }
    }
}