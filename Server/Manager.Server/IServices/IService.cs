using Manager.Core.Page;
using NPOI.SS.Formula.Functions;
using System.Linq.Expressions;

namespace Manager.Server.IServices
{
    public interface IService
    {
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression, bool isTrack = true, string orderBy = "");

        Task<PagedList<T>> PagedAsync<T>(Expression<Func<T, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "");

        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> expression, bool isTrack = true, string orderBy = "");

        Task<int> CountAsync<T>(Expression<Func<T, bool>> expression, bool isTrack = true);

        Task<Tuple<bool, string>> AddAsync<T>(T t);

        Task<Tuple<bool, string>> DeleteAsync<T>(Expression<Func<T, bool>> expression);

        Task<bool?> ExsitAsync(Expression<Func<T, bool>> expression);
    }
}