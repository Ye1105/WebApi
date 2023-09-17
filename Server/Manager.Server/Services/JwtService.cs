using Manager.Core.Settings;
using Manager.Extensions;
using NPOI.SS.Formula.Functions;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.Server.Services
{
    public class JwtService : IJwtService
    {
        public Tuple<bool, string> AddAsync(Guid uId, string refreshToken)
        {
            try
            {
                using var cli = Instance(RedisBaseEnum.Zeroth);

                using var lockObj = cli.Lock("AddRefreshToken", 5);

                var pipe = cli.StartPipe();

                //7天
                pipe.SetExAsync($"{RedisConstants.JWT_REFRESH_TOKEN}{uId.Str()}", 60 * 60 * 24 * 7, refreshToken);

                pipe.EndPipe();

                lockObj.Unlock();

                return Tuple.Create(true, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> ExsitAsync(Guid uId, string refreshToken)
        {
            //return crudJWT.ExsitRefreshToken(uId, refreshToken);
            using var redis = Instance(RedisBaseEnum.Zeroth);

            var res = await redis.GetAsync($"{RedisConstants.JWT_REFRESH_TOKEN}{uId.Str()}");
            if (res == null)
            {
                return Tuple.Create(false, "用户Token不存在");
            }
            else
            {
                if (res == refreshToken)
                {
                    return Tuple.Create(true, "");
                }
                else
                {
                    return Tuple.Create(false, "用户Token不匹配");
                }
            }
        }
    }
}