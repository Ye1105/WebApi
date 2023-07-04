using Manager.Core.Models.Accounts;
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
    public class AccountInfoService : IAccountInfoService
    {
        private readonly IBase baseService;

        public AccountInfoService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<AccountInfo?> GetAccountInfoBy(Expression<Func<AccountInfo, bool>> expression, bool isTrack = true)
        {
            var accountInfo = await baseService.FirstOrDefaultAsync(expression, isTrack);
            return accountInfo;
        }

        public async Task<AccountInfo?> GetAccountInfoAndAvatarAndCoverById(Guid? uId, bool isCache = false)
        {
            try
            {
                var keyName = $"{RedisConstants.Prefix_AccountInfoAndAvatarAndCover}{uId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                var res = await cli.ExistsAsync(keyName);

                //命中缓存
                if (res)
                {
                    var value = await cli.GetAsync(keyName);

                    return string.IsNullOrWhiteSpace(value) ? null : value.DesObj<AccountInfo>();
                }
                else
                {
                    if (isCache)
                    {
                        /*
                         * 1.缓存是否命中
                         * 2.命中则直接获取缓存值
                         * 3.未命中则从mysql获取值，然后更新缓存值，并返回
                         */

                        //关联查询 => 头像 封面
                        var accountInfo = await baseService.Entities<AccountInfo>()
                                                .Where(x => x.UId == uId)
                                                .Include(x => x.Avatar)
                                                .Include(x => x.Cover)
                                                .FirstOrDefaultAsync();

                        if (accountInfo != null)
                        {
                            //expire 5 minutes
                            await cli.SetExAsync(keyName, 300, accountInfo.SerObj());
                        }
                        else
                        {
                            //expire 5 minutes
                            await cli.SetExAsync(keyName, 300, "");
                        }
                        return await GetAccountInfoAndAvatarAndCoverById(uId, isCache);
                    }
                    else
                    {
                        var accountInfo = await baseService.Entities<AccountInfo>()
                        .Where(x => x.UId == uId)
                        .Include(x => x.Avatar)
                        .Include(x => x.Cover)
                        .FirstOrDefaultAsync();

                        return accountInfo ?? null;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetAccountInfoAndAvatarAndCoverById:{0}", ex.ToString());
                return null;
            }
        }

        public async Task<bool> ModifyAccountInfo(AccountInfo accountInfo)
        {
            /*
             * 更新用户信息表
             * 删除redis缓存
             */

            var res = await baseService.ModifyAsync(accountInfo) > 0;

            if (res)
            {
                using var cli = Instance(RedisBaseEnum.Zeroth);

                var keyName = $"{RedisConstants.Prefix_AccountInfoAndAvatarAndCover}{accountInfo.UId}";

                await cli.DelAsync(keyName);
            }

            return res;
        }
    }
}