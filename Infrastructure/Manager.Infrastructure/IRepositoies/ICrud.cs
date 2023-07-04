using Manager.Core.Page;
using System.Linq.Expressions;

namespace Manager.Infrastructure.IRepositoies
{
    public interface ICrud<T> where T : class
    {
        Task<T> Select(Expression<Func<T, bool>> selWhere, bool isTrack = true);

    }
}
