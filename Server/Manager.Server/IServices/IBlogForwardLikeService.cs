namespace Manager.Server.IServices
{
    public interface IBlogForwardLikeService
    {
        /// <summary>
        /// 新增博客转发点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> AddAsync(Guid fId, Guid uId);

        /// <summary>
        /// 取消博客转发点赞
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>

        Task<Tuple<bool, string>> DeleteAsync(Guid fId, Guid uId);

        /// <summary>
        /// 博客转发点赞数
        /// </summary>
        /// <param name="fId"></param>
        /// <returns></returns>
        Task<long?> CountAsync(Guid fId);

        /// <summary>
        /// 当前用户是否点赞转发
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid fId, Guid uId);
    }
}