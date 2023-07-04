﻿using Manager.Core.Settings;
using Manager.Extensions;
using Manager.JwtAuthorizePolicy.IServices;
using static Manager.Redis.Infrastructure.RedisClient;

namespace Manager.JwtAuthorizePolicy.Services
{
    public class JwtService : IJwtService
    {
        public Tuple<bool, string> AddRefreshToken(Guid uId, string refreshToken)
        {
            try
            {
                using var cli = Instance(RedisBaseEnum.Zeroth);

                using var lockObj = cli.Lock("AddRefreshToken", 5);

                var pipe = cli.StartPipe();

                pipe.HSet(RedisConstants.JwtRefreshToken, uId.Str(), refreshToken);

                pipe.EndPipe();

                lockObj.Unlock();

                return Tuple.Create(true, "");
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, ex.ToString());
            }
        }

        public async Task<Tuple<bool, string>> ExsitRefreshToken(Guid uId, string refreshToken)
        {
            //return crudJWT.ExsitRefreshToken(uId, refreshToken);
            using var redis = Instance(RedisBaseEnum.Zeroth);

            var res = await redis.HGetAsync(RedisConstants.JwtRefreshToken, uId.Str());
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