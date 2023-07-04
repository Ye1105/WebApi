using Manager.Core.Enums;
using Manager.Core.Models.Blogs;
using Manager.Core.Models.Users;
using Manager.Core.Page;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace Manager.Server.Services
{
    public class BlogForwardService : IBlogForwardService
    {
        private readonly IBase baseService;

        public BlogForwardService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> AddBlogForward(Blog blog, BlogForward blogForward)
        {
            var dic = new Dictionary<object, CrudEnum>
            {
                { blog, CrudEnum.Create },
                { blogForward, CrudEnum.Create }
            };
            return await baseService.BatchTransactionAsync(dic);
        }

        public async Task<Tuple<bool, string>> DeleteBlogForward(Guid id)
        {
            /*
             * 1.判断blog blogforward 中是否存在
             * 2.禁用数据
             */

            var blog = await baseService.FirstOrDefaultAsync<Blog>(x => x.Id == id, true);
            if (blog == null)
            {
                return Tuple.Create(false, "博客不存在");
            }

            var blogForward = await baseService.FirstOrDefaultAsync<BlogForward>(x => x.Id == id, true);
            if (blogForward == null)
            {
                return Tuple.Create(false, "转发不存在");
            }

            blog.Status = (sbyte)Status.Disable;
            blogForward.Status = (sbyte)Status.Disable;

            var dic = new Dictionary<object, CrudEnum>
            {
                { blog, CrudEnum.Update },
                { blogForward, CrudEnum.Update }
            };

            var res = await baseService.BatchTransactionAsync(dic);

            return res ? Tuple.Create(true, "") : Tuple.Create(false, "删除转发失败");
        }

        public async Task<int> GetBlogForwardCountBy(Expression<Func<BlogForward, bool>> expression, bool isTrack = true)
        {
            return await baseService.Entities<BlogForward>().Where(expression).CountAsync();
        }

        public async Task<PagedList<BlogForward>?> GetPagedList(int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "", Guid? id = null, Guid? uId = null, Guid? baseBId = null, Guid? prevBId = null, Guid? buId = null, Guid? prevCId = null, DateTime? startTime = null, DateTime? endTime = null, Guid? wId = null, ForwardScope? scope = null, Status? status = null)
        {
            var query = baseService.Entities<BlogForward>();

            if (scope != null)
            {
                switch (scope)
                {
                    //【@我的】动态 => blog_forward 中的 BuId 是当前登录网站的用户 id
                    case ForwardScope.At:
                        query = query.Where(x => x.BuId == wId);
                        break;
                    //【@我的】【关注人】的动态 =>  我关注的人转发了我的评论或者博客
                    case ForwardScope.Focus:
                        query = from b in baseService.Entities<UserFocus>()
                                join p in baseService.Entities<BlogForward>()
                                on b.BuId equals p.UId
                                where b.UId == wId && p.BuId == wId
                                select p;
                        break;
                    // 【@我的】【原创】动态 => 转发我的原创blog
                    case ForwardScope.Orgion:
                        query = query.Where(x => x.BuId == wId && x.PrevBId == x.BaseBId);
                        break;

                    default:
                        return null;
                }
            }

            if (id != null)
            {
                query = query.Where(x => x.Id == id);
            }

            if (uId != null)
            {
                query = query.Where(x => x.UId == uId);
            }

            if (baseBId != null)
            {
                query = query.Where(x => x.BaseBId == baseBId);
            }

            if (prevBId != null)
            {
                query = query.Where(x => x.PrevBId == prevBId);
            }

            if (buId != null)
            {
                query = query.Where(x => x.BuId == buId);
            }

            if (prevCId != null)
            {
                query = query.Where(x => x.PrevCId == prevCId);
            }

            if (startTime != null)
            {
                query = query.Where(x => x.Created >= startTime);
            }

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
            return await PagedList<BlogForward>.CreateAsync(query, pageIndex, pageSize, offset);
        }
    }
}