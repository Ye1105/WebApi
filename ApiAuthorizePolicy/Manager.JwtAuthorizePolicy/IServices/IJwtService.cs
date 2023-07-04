namespace Manager.JwtAuthorizePolicy.IServices
{
    public interface IJwtService
    {
        /// <summary>
        /// add refreshToken
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Tuple<bool, string> AddRefreshToken(Guid uId, string refreshToken);

        /// <summary>
        /// 查询 refreshToken 是否存在
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> ExsitRefreshToken(Guid uId, string refreshToken);
    }
}