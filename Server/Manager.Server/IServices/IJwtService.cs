namespace Manager.Server.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// add refreshToken
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Tuple<bool, string> AddAsync(Guid uId, string refreshToken);

        /// <summary>
        /// 查询 refreshToken 是否存在
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> ExsitAsync(Guid uId, string refreshToken);
    }
}