using Manager.Core.Models.Users;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IBase baseService;

        public UserGroupService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<UserGroup?> GetUserGroupBy(Expression<Func<UserGroup, bool>> expression, bool isTrack = true)
        {
            return await baseService.FirstOrDefaultAsync(expression, isTrack);
        }

        public async Task<bool> ModifyUserGroup(UserGroup userGroup)
        {
            var res = await baseService.UpdateAsync(userGroup) > 0;

            if (res)
            {
                using var cli = Instance(RedisBaseEnum.Zeroth);

                var keyName = $"{RedisConstants.PREFIX_USER_GROUP}{userGroup.UId}";

                await cli.DelAsync(keyName);
            }

            return res;
        }

        public async Task<UserGroup?> GetUserGroupByUId(Guid uId)
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回值
                 *
                 */
                var keyName = $"{RedisConstants.PREFIX_USER_GROUP}{uId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                if (res)
                {
                    var userGroup = await cli.GetAsync(keyName);

                    return userGroup.DesObj<UserGroup>();
                }
                else
                {
                    var userGroup = await baseService.Entities<UserGroup>().FirstOrDefaultAsync(x => x.UId == uId);

                    await cli.SetExAsync(keyName, 300, userGroup.SerObj());

                    return await GetUserGroupByUId(uId);
                }
            }
            catch (Exception ex)
            {
                Log.Error("UserGroupService_GetUserGroupByUId: {0}", ex);
                return null;
            }
        }
    }
}