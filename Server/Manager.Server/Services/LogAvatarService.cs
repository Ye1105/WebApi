using FreeRedis;
using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Manager.Core.Models.Logs;
using Manager.Core.Page;
using Manager.Core.Settings;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using Serilog;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class LogAvatarService : ILogAvatarService
    {
        private readonly IBase baseService;

        private readonly string Prefix_AvatarPagedList = "Avatar:PagedList:";

        public LogAvatarService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Tuple<bool, string>> AddLogAvatar(LogAvatar logAvatar)
        {
            try
            {
                var accountInfo = await baseService.FirstOrDefaultAsync<AccountInfo>(x => x.UId == logAvatar.UId, true);
                if (accountInfo == null)
                {
                    return Tuple.Create(false, "账号信息不存在");
                }

                accountInfo.AvatarId = logAvatar.Id;

                var dic = new Dictionary<object, CrudEnum>
                {
                    { logAvatar, CrudEnum.Create },
                    { accountInfo, CrudEnum.Update }
                };

                var res = await baseService.BatchTransactionAsync(dic);

                if (res)
                {
                    using var cli = Instance(RedisBaseEnum.Zeroth);

                    var keyName = $"{RedisConstants.Prefix_AccountInfoAndAvatarAndCover}{logAvatar.UId}";

                    var keyNamePagedList = $"{Prefix_AvatarPagedList}{logAvatar.UId}";

                    await cli.DelAsync(keyName, keyNamePagedList);
                }

                return Tuple.Create(res, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<PagedList<LogAvatar?>?> GetPagedList(Guid uId, int pageIndex = 1, int pageSize = 10, int offset = 0, string orderBy = "")
        {
            try
            {
                /*
                 * 1.是否命中缓存
                 * 2.命中缓存
                 * 2.1 获取键的类型 【string 类型代表没有数据，zset 类型代表列表有数据】
                 * 2.2 zset ZRevRangeAsync 分页
                 * 3.未命中缓存
                 * 3.1 没有数据 设置 string key
                 * 3.2 有数据  设置 zset key
                 * 4. 递归循环一次当前方法
                 */

                var keyName = $"{Prefix_AvatarPagedList}{uId}";

                using var cli = Instance(RedisBaseEnum.Zeroth);

                //1.是否命中缓存
                var res = await cli.ExistsAsync(keyName);
                //2.1命中缓存
                if (res)
                {
                    //2.2 获取键的类型
                    //2.3 string 类型代表没有数据，zset 类型代表点赞列表有数据
                    var type = cli.Type(keyName);
                    switch (type)
                    {
                        case KeyType.@string:

                            return null;

                        case KeyType.zset:

                            var avatarCount = await cli.ZCardAsync(keyName);

                            var members = await cli.ZRevRangeAsync(keyName, (pageIndex - 1) * pageSize + offset, pageIndex * pageSize - 1 + offset);

                            var avatars = new List<LogAvatar?>();

                            foreach (var item in members)
                            {
                                //反序列化
                                var data = item.DesObj<LogAvatar>();

                                avatars.Add(data);
                            }

                            return PagedList<LogAvatar?>.Create(avatars, avatarCount, pageIndex, pageSize, offset);

                        default:
                            break;
                    }
                }
                else
                {
                    var count = await baseService.Entities<LogAvatar>().Where(x => x.UId == uId && x.Status == (sbyte)Status.Enable).CountAsync();
                    if (count == 0)
                    {
                        await cli.SetExAsync(keyName, 300, "");
                    }
                    else
                    {
                        var data = await baseService.Entities<LogAvatar>().Where(x => x.UId == uId && x.Status == (sbyte)Status.Enable).AsNoTracking().ToListAsync();
                        var pipe = cli.StartPipe();

                        foreach (var item in data)
                        {
                            pipe.ZAdd(keyName, DateHelper.ConvertDateTimeToLong(item.Created), item.SerObj());
                        }
                        pipe.Expire(keyName, 300);

                        object[] ret = pipe.EndPipe();

                        return await GetPagedList(uId, pageIndex, pageSize, offset);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("LogAvatarService_GetPagedList:{0}", ex);
                return null;
            }
        }
    }
}