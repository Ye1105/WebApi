namespace Manager.Server.IServices
{
    public interface IBlogCommentLikeService
    {
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        //Task<long?> CountAsync(Guid cId);

        /// <summary>
        /// 当前登录用户是否对评论点赞
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="uId"></param>
        /// <returns></returns>
        Task<bool?> ExsitAsync(Guid cId, Guid uId);

        /// <summary>
        /// 增加博客评论的点赞
        /// </summary>
        /// <param name="cId">评论id</param>
        /// <param name="uId">当前网站登录的用户id</param>
        /// <returns></returns>

        Task<Tuple<bool, string>> AddAsync(Guid cId, Guid uId);

        /// <summary>
        /// 取消博客评论的点赞
        /// </summary>
        /// <param name="cId"></param>
        /// <param name="uId">当前网站登录的用户id</param>
        /// <returns></returns>
        Task<Tuple<bool, string>> DeleteAsync(Guid cId, Guid uId);
    }
}