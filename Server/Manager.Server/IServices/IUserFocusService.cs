using Manager.Core.Enums;
using Manager.Core.Models.Users;
using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IUserFocusService
    {
        /// <summary>
        /// 获取关注关系表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="isTrack"></param>
        /// <returns></returns>
        Task<UserFocus?> GetUserFocusBy(Expression<Func<UserFocus, bool>> expression, bool isTrack = true);

        /// <summary>
        /// 关注数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> GetUserFocusCountBy(Expression<Func<UserFocus, bool>> expression);

        /// <summary>
        /// 增加用户关注关系
        /// </summary>
        /// <param name="userFocus"></param>
        /// <returns></returns>
        Task<bool> AddUserFocus(UserFocus userFocus);

        /// <summary>
        /// 关注关系分页查询列表
        /// </summary>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="offset">索引偏移</param>
        /// <param name="isTrack">是否追踪</param>
        /// <param name="orderBy">排序</param>
        /// <param name="relationType">关系类型</param>
        /// <param name="uId">用户id</param>
        /// <param name="grp">分组成员</param>
        /// <param name="relation">关系 0:关注  1:特别关注</param>
        /// <param name="channel">渠道 0:web  1: mobile</param>
        /// <param name="startTime">开始时间 null:不筛选</param>
        /// <param name="endTime">结束时间 null:不筛选</param>
        /// <returns></returns>
        Task<PagedList<UserFocus>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", RelationType relationType = RelationType.FOCUS, Guid? uId = null, string? grp = null, sbyte? relation = null, sbyte? channel = null, DateTime? startTime = null, DateTime? endTime = null);

        /// <summary>
        /// 修改关注关系
        /// </summary>
        /// <param name="userFocus"></param>
        /// <returns></returns>
        Task<bool> ModifyUserFocus(UserFocus userFocus);

        /// <summary>
        /// 删除关注关系
        /// </summary>
        /// <param name="userFocus"></param>
        /// <returns></returns>
        Task<bool> DelUserFocus(UserFocus userFocus);

        /// <summary>
        /// 批量删除关注关系
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<int> BatchDeleteUserFocus(Expression<Func<UserFocus, bool>> expression);
    }
}