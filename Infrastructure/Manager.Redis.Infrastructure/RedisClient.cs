using FreeRedis;

namespace Manager.Redis.Infrastructure
{
    public class RedisClient
    {
        private static readonly string connectionString = "127.0.0.1:6379";
        private static IRedisClient? instance;

        private RedisClient()
        {
        }

        public static FreeRedis.RedisClient Instance(RedisBaseEnum index)
        {
            instance ??= new FreeRedis.RedisClient(connectionString + ",defaultDatabase=0");
#if DEBUG
            //instance.Notice += (s, e) => Log.Information(e.Log);
#endif
            return instance.GetDatabase((sbyte)index);
        }

        /// <summary>
        /// redis 数据库枚举
        /// </summary>
        public enum RedisBaseEnum
        {
            Zeroth = 0,
            First = 1,
            Second = 2,
            Third = 3,
            Fourth = 4,
            Five = 5,
            Sixth = 6,
            Seven = 7,
            Eighth = 8,
            Ninth = 9,
            Ten = 10,
            Eleven = 11,
            Twelfth = 12,
            Thirteenth = 13,
        }
    }
}