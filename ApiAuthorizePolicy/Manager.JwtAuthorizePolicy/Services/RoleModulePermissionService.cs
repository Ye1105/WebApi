using Manager.Core.AuthorizationModels;
using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Manager.Core.Models.Modules;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.JwtAuthorizePolicy.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.JwtAuthorizePolicy.Services
{
    public class RoleModulePermissionService : IRoleModulePermissionService
    {
        private readonly string Prefix_RolePermission = "RolePermission";

        private readonly IBase baseService;

        public RoleModulePermissionService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<List<AccountRole>> GetAccountRolesAsync(Expression<Func<AccountRole, bool>> express, bool isTrack = true)
        {
            return await baseService.GetListByAsync(express, isTrack);
        }

        public async Task<RolePermissionViewModel?> GetRolePermissionAsync()
        {
            try
            {
                /*
                 * 1.缓存是否命中
                 * 2.命中则直接获取缓存值
                 * 3.未命中则从mysql获取值，然后更新缓存值，并返回
                 */

                var keyName = Prefix_RolePermission;

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                //命中缓存
                if (res)
                {
                    var value = await cli.GetAsync(keyName);

                    return string.IsNullOrWhiteSpace(value) ? null : value.DesObj<RolePermissionViewModel>();
                }
                else
                {
                    var data = new RolePermissionViewModel
                    {
                        RolePermissions = await baseService.EntitiesNoTrack<RolePermission>().ToListAsync(),
                        ModuleInfos = await baseService.EntitiesNoTrack<ModuleInfo>().Where(x => x.Status == (sbyte)Status.Enable).ToListAsync()
                    };

                    if (data != null)
                    {
                        //expire 24 小时  60 * 60 *24
                        await cli.SetExAsync(keyName, 86400, data.SerObj());
                    }
                    else
                    {
                        //expire 5 minutes
                        await cli.SetExAsync(keyName, 86400, "");
                    }

                    return await GetRolePermissionAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetPermissionRoleAsync:{0}", ex.ToString());
                return null;
            }
        }
    }
}