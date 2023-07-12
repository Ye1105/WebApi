using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Page;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Manager.Server.Services
{
    public class BlogCommentService : IBlogCommentService
    {
        private readonly IBase baseService;

        public BlogCommentService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<int> GetBlogCommentCountBy(Expression<Func<BlogComment, bool>> expression, bool isTrack = true)
        {
            return await baseService.Entities<BlogComment>().Where(expression).CountAsync();
        }

        public async Task<PagedList<BlogComment>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? bId = null, Guid? uId = null, CommentType[]? types = null, Guid? pId = null, Guid? grp = null, DateTime? startTime = null, DateTime? endTime = null, Status? status = null)
        {
            var query = baseService.Entities<BlogComment>();

            if (id != null)
            {
                query = query.Where(x => x.Id == id);
            }

            if (bId != null)
            {
                query = query.Where(x => x.BId == bId);
            }

            if (uId != null)
            {
                query = query.Where(x => x.UId == uId);
            }
            //0.评论   1.【回复】来评论【评论】  2.【回复】来评论【回复】
            if (types != null && types.Any())
            {
                query = query.Where(x => types.Contains((CommentType)x.Type));
            }
            //分支
            if (pId != null)
            {
                query = query.Where(x => x.PId == pId);
            }
            //分组
            if (grp != null)
            {
                query = query.Where(x => x.Grp == grp);
            }
            // 开始时间
            if (startTime != null)
            {
                query = query.Where(x => x.Created >= startTime);
            }
            // 结束时间
            if (endTime != null)
            {
                query = query.Where(x => x.Created < endTime);
            }

            //状态
            if (status != null)
            {
                query = query.Where(x => x.Status == (int)status);
            }

            //排序
            query = query.ApplySort(orderBy);

            //整合分页数据
            return await PagedList<BlogComment>.CreateAsync(query, pageIndex, pageSize, offset);
        }

        public Task<PagedList<BlogComment>> GetPagedList(Expression<Func<BlogComment, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "")
        {
            return baseService.QueryPagedAsync(whereLambda, pageIndex, pageSize, offset, isTrack, orderBy);
        }

        public async Task<bool> AddBlogComment(BlogComment blogComment)
        {
            return await baseService.AddAsync(blogComment) > 0;
        }

        public async Task<Tuple<bool, string>> DeleteBlogComment(CommentType type, Guid grp, Guid id)
        {
            if (type == CommentType.COMMENT)
            {
                var collection = await baseService.Entities<BlogComment>().Where(x => x.Grp == grp).ToListAsync();
                if (collection != null && collection.Any())
                {
                    foreach (var item in collection)
                    {
                        item.Status = (sbyte)Status.DISABLE;
                    }
                    var res = await baseService.UpdateRangeAsync(collection) > 0;
                    return res ? Tuple.Create(true, "删除评论成功") : Tuple.Create(false, "删除评论分组失败");
                }
                else
                {
                    return Tuple.Create(false, "评论不存在");
                }
            }
            else
            {
                //遍历删除当前节点的所有子节点
                var collection = new List<BlogComment>();
                var current = await baseService.FirstOrDefaultAsync<BlogComment>(x => x.Id == id, true);
                if (current != null)
                {
                    collection.Add(current);
                    var childs = RecursionList(new List<BlogComment>(), baseService.Entities<BlogComment>(), id);
                    if (childs != null && childs.Any())
                    {
                        collection.AddRange(childs);
                    }
                    foreach (var item in collection)
                    {
                        item.Status = (sbyte)Status.DISABLE;
                    }
                    var res = await baseService.UpdateRangeAsync(collection) > 0;
                    return res ? Tuple.Create(true, "删除评论成功") : Tuple.Create(false, "删除评论分组失败");
                }
                else
                {
                    return Tuple.Create(false, "评论不存在");
                }
            }
        }

        /// <summary>
        /// 遍历所有子节点
        /// </summary>
        /// <param name="res"></param>
        /// <param name="blogComments"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        private List<BlogComment> RecursionList(List<BlogComment> res, IQueryable<BlogComment> blogComments, Guid pId)
        {
            var collection = blogComments.Where(x => x.PId == pId).ToList();
            if (collection != null && collection.Any())
            {
                foreach (var item in collection)
                {
                    res.Add(item);
                    RecursionList(res, blogComments, item.Id);
                }
            }
            return res;
        }
    }
}